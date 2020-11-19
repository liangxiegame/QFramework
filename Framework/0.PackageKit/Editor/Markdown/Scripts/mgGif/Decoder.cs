using UnityEngine;
using System;
using System.IO;

namespace MG.GIF
{
    public class Decoder
    {
        [Flags]
        private enum ImageFlag
        {
            Interlaced = 0x40,
            ColourTable = 0x80,
            TableSizeMask = 0x07,
            BitDepthMask = 0x70,
        }

        private enum Block
        {
            Image = 0x2C,
            Extension = 0x21,
            End = 0x3B
        }

        private enum Extension
        {
            GraphicControl = 0xF9,
            Comments = 0xFE,
            PlainText = 0x01,
            ApplicationData = 0xFF
        }

        private ImageList Images;

        // colour
        private Color32[] GlobalColourTable = null;
        private Color32[] ActiveColourTable = null;
        private Color32 BackgroundColour = new Color32(0x00, 0x00, 0x00, 0xFF);
        private Color32 ClearColour = new Color32(0x00, 0x00, 0x00, 0x00);
        private ushort TransparentIndex = 0xFFFF;

        // current controls
        private ushort ControlDelay = 0;
        private Disposal ControlDispose = Disposal.None;

        // current image
        private ushort ImageLeft;
        private ushort ImageTop;
        private ushort ImageWidth;
        private ushort ImageHeight;
        private ImageFlag ImageFlags;
        private bool ImageInterlaced;
        private byte LzwMinimumCodeSize;


        //------------------------------------------------------------------------------

        public static ImageList Parse(byte[] data)
        {
            try
            {
                return new Decoder().Decode(data);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                return null;
            }
        }

        //------------------------------------------------------------------------------

        public ImageList Decode(byte[] data)
        {
            if (data == null || data.Length <= 12)
            {
                throw new Exception("Invalid data");
            }

            Images = new ImageList();

            using (var r = new BinaryReader(new MemoryStream(data)))
            {
                ReadHeader(r);
                ReadBlocks(r);
            }

            return Images;
        }

        //------------------------------------------------------------------------------

        private Color32[] ReadColourTable(ImageFlag flags, BinaryReader r)
        {
            var tableSize = (int) Math.Pow(2, (int) (flags & ImageFlag.TableSizeMask) + 1);
            var colourTable = new Color32[tableSize];

            for (var i = 0; i < tableSize; i++)
            {
                colourTable[i] = new Color32(
                    r.ReadByte(),
                    r.ReadByte(),
                    r.ReadByte(),
                    0xFF
                );
            }

            return colourTable;
        }

        //------------------------------------------------------------------------------

        protected void ReadHeader(BinaryReader r)
        {
            // signature

            Images.Version = new string(r.ReadChars(6));

            if (Images.Version != "GIF87a" && Images.Version != "GIF89a")
            {
                throw new Exception("Unsupported GIF version");
            }

            // read header

            Images.Width = r.ReadUInt16();
            Images.Height = r.ReadUInt16();
            ImageFlags = (ImageFlag) r.ReadByte();
            var bgIndex = r.ReadByte();

            r.ReadByte(); // aspect ratio

            Images.BitDepth = (int) (ImageFlags & ImageFlag.BitDepthMask) >> 4 + 1;

            if ((ImageFlags & ImageFlag.ColourTable) == ImageFlag.ColourTable)
            {
                GlobalColourTable = ReadColourTable(ImageFlags, r);

                if (bgIndex < GlobalColourTable.Length)
                {
                    BackgroundColour = GlobalColourTable[bgIndex];
                }
            }
        }

        //------------------------------------------------------------------------------

        protected void ReadBlocks(BinaryReader r)
        {
            while (true)
            {
                var block = (Block) r.ReadByte();

                switch (block)
                {
                    case Block.Image:
                        ReadImageBlock(r);
                        break;

                    case Block.Extension:

                        var ext = (Extension) r.ReadByte();

                        switch (ext)
                        {
                            case Extension.GraphicControl:
                                ReadControlBlock(r);
                                break;

                            default:
                                SkipBlock(r);
                                break;
                        }

                        break;

                    case Block.End:
                        return;

                    default:
                        throw new Exception("Unexpected block type");
                }
            }
        }

        //------------------------------------------------------------------------------

        private void SkipBlock(BinaryReader r)
        {
            var blockSize = r.ReadByte();

            while (blockSize != 0x00)
            {
                r.BaseStream.Seek(blockSize, SeekOrigin.Current);
                blockSize = r.ReadByte();
            }
        }


        //------------------------------------------------------------------------------

        private void ReadControlBlock(BinaryReader r)
        {
            var blockSize = r.ReadByte();

            var flags = r.ReadByte();

            switch (flags & 0x1C)
            {
                case 0x04:
                    ControlDispose = Disposal.DoNotDispose;
                    break;
                case 0x08:
                    ControlDispose = Disposal.RestoreBackground;
                    break;
                case 0x0C:
                    ControlDispose = Disposal.ReturnToPrevious;
                    break;
                default:
                    ControlDispose = Disposal.None;
                    break;
            }

            ControlDelay = r.ReadUInt16();

            // has transparent colour?

            var transparentColour = r.ReadByte();

            if ((flags & 0x01) == 0x01)
            {
                TransparentIndex = transparentColour;
            }
            else
            {
                TransparentIndex = 0xFFFF;
            }

            r.ReadByte(); // terminator
        }

        //------------------------------------------------------------------------------

        protected Color32[] Deinterlace(Color32[] input, int width)
        {
            var output = new Color32[input.Length];
            var numRows = input.Length / width;
            var writePos = input.Length - width; // NB: work backwards due to Y-coord flip

            for (var row = 0; row < numRows; row++)
            {
                var copyRow = 0;

                // every 8th row starting at 0
                if (row % 8 == 0)
                {
                    copyRow = row / 8;
                }
                // every 8th row starting at 4
                else if ((row + 4) % 8 == 0)
                {
                    var o = numRows / 8;
                    copyRow = o + (row - 4) / 8;
                }
                // every 4th row starting at 2
                else if ((row + 2) % 4 == 0)
                {
                    var o = numRows / 4;
                    copyRow = o + (row - 2) / 4;
                }
                // every 2nd row starting at 1
                else // if( ( r + 1 ) % 2 == 0 )
                {
                    var o = numRows / 2;
                    copyRow = o + (row - 1) / 2;
                }

                Array.Copy(input, (numRows - copyRow - 1) * width, output, writePos, width);
                writePos -= width;
            }

            return output;
        }

        //------------------------------------------------------------------------------

        protected void ReadImageBlock(BinaryReader r)
        {
            // read image block header

            ImageLeft = r.ReadUInt16();
            ImageTop = r.ReadUInt16();
            ImageWidth = r.ReadUInt16();
            ImageHeight = r.ReadUInt16();
            ImageFlags = (ImageFlag) r.ReadByte();
            ImageInterlaced = (ImageFlags & ImageFlag.Interlaced) == ImageFlag.Interlaced;

            if (ImageWidth == 0 || ImageHeight == 0)
            {
                return;
            }

            if ((ImageFlags & ImageFlag.ColourTable) == ImageFlag.ColourTable)
            {
                ActiveColourTable = ReadColourTable(ImageFlags, r);
            }
            else
            {
                ActiveColourTable = GlobalColourTable;
            }

            LzwMinimumCodeSize = r.ReadByte();


            // compressed image data

            var lzwData = ReadImageBlocks(r);


            // this disposal method determines whether we start with a previous image

            OutputBuffer = null;

            switch (ControlDispose)
            {
                case Disposal.None:
                case Disposal.DoNotDispose:
                {
                    var prev = Images.Images.Count > 0 ? Images.Images[Images.Images.Count - 1] : null;

                    if (prev != null && prev.RawImage != null)
                    {
                        OutputBuffer = prev.RawImage.Clone() as Color32[];
                    }
                }
                    break;


                case Disposal.ReturnToPrevious:

                    for (int i = Images.Images.Count - 1; i >= 0; i--)
                    {
                        var prev = Images.Images[i];

                        if (prev.DisposalMethod == Disposal.None || prev.DisposalMethod == Disposal.DoNotDispose)
                        {
                            OutputBuffer = prev.RawImage.Clone() as Color32[];
                            break;
                        }
                    }

                    break;

                case Disposal.RestoreBackground:
                default:
                    break;
            }

            if (OutputBuffer == null)
            {
                var size = Images.Width * Images.Height;

                OutputBuffer = new Color32[size];

                for (int i = 0; i < size; i++)
                {
                    OutputBuffer[i] = ClearColour;
                }
            }

            // create image

            var img = new Image(Images);

            img.Delay = ControlDelay * 10; // (gif are in 1/100th second) convert to ms
            img.DisposalMethod = ControlDispose;

            //var sw = new Stopwatch(); sw.Start();
            img.RawImage = DecompressLZW(lzwData);
            //sw.Stop(); UnityEngine.Debug.Log( $"{sw.ElapsedTicks} ticks, {sw.ElapsedMilliseconds}ms" );

            if (ImageInterlaced)
            {
                img.RawImage = Deinterlace(img.RawImage, ImageWidth);
            }

            Images.Add(img);
        }


        //------------------------------------------------------------------------------

        private byte[] ReadImageBlocks(BinaryReader r)
        {
            var startPos = r.BaseStream.Position;

            // get total size

            var totalBytes = 0;
            var blockSize = r.ReadByte();

            while (blockSize != 0x00)
            {
                totalBytes += blockSize;
                r.BaseStream.Seek(blockSize, SeekOrigin.Current);

                blockSize = r.ReadByte();
            }

            if (totalBytes == 0)
            {
                return null;
            }

            // read bytes

            var buffer = new byte[totalBytes];
            r.BaseStream.Seek(startPos, SeekOrigin.Begin);

            var offset = 0;
            blockSize = r.ReadByte();

            while (blockSize != 0x00)
            {
                r.Read(buffer, offset, blockSize);
                offset += blockSize;

                blockSize = r.ReadByte();
            }

            return buffer;
        }

        //------------------------------------------------------------------------------
        // LZW

        int LzwClearCode;
        int LzwEndCode;
        int LzwCodeSize;
        int LzwNextSize;
        int LzwMaximumCodeSize;

        // the code spends 95% of the time here so optimised for performance using pre-allocated buffers (cut down on allocation overhead)

        int LzwNumCodes = 0;

        int[]
            LzwCodeIndices =
                new int[4098]; // codes can be upto 12 bytes long, this is the maximum number of possible codes (2^12 + 2 for clear and end code)

        ushort[]
            LzwCodeBuffer =
                new ushort[64 * 1024]; // 64k buffer for codes - should be plenty but we dynamically resize if required

        int LzwCodeBufferLen = 0; // end of data (next write position)

        int PixelNum;
        Color32[] OutputBuffer;

        // lookup colour based on code and write to correct position

        private void WritePixel(ushort code)
        {
            var row = ImageTop + PixelNum / ImageWidth;
            var col = ImageLeft + PixelNum % ImageWidth;

            if (row < Images.Height && col < Images.Width)
            {
                // reverse row (flip in Y) because gif coordinates start at the top-left (unity is bottom-left)

                var index = (Images.Height - row - 1) * Images.Width + col;

                if (code != TransparentIndex)
                {
                    OutputBuffer[index] = code < ActiveColourTable.Length ? ActiveColourTable[code] : BackgroundColour;
                }
            }

            PixelNum++;
        }

        //------------------------------------------------------------------------------
        // decompress LZW data and write colours to OutputBuffer
        // Optimsed for performance
        // LzwCodeSize setup before call
        // OutputBuffer should be initialised before hand with default values (so despose and transparency works correctly)

        private Color32[] DecompressLZW(byte[] lzwData)
        {
            // setup codes

            LzwCodeSize = LzwMinimumCodeSize + 1;
            LzwNextSize = (int) Math.Pow(2, LzwCodeSize);
            LzwMaximumCodeSize = (int) Math.Pow(2, LzwMinimumCodeSize);
            LzwClearCode = LzwMaximumCodeSize;
            LzwEndCode = LzwClearCode + 1;

            // initialise buffers

            LzwCodeBufferLen = 0;
            LzwNumCodes = LzwMaximumCodeSize + 2;

            // write initial code sequences

            for (ushort i = 0; i < LzwNumCodes; i++)
            {
                LzwCodeIndices[i] = LzwCodeBufferLen;
                LzwCodeBuffer[LzwCodeBufferLen++] = 1; // length
                LzwCodeBuffer[LzwCodeBufferLen++] = i; // code
            }


            // LZW decode loop

            PixelNum = 0; // number of pixel being processed (used to find row and column of output)

            int previousCode = -1; // last code processed
            int bitsAvailable = 0; // number of bits available to read in the shift register
            int inputDataPosition = 0; // next read position from the input stream
            uint
                shiftRegister =
                    0; // shift register holds the bytes coming in from the input stream, we shift down by the number of bits

            while (inputDataPosition != lzwData.Length || bitsAvailable > 0)
            {
                // get next code

                int bitsLeftToRead = LzwCodeSize;

                // consume any existing bits in the shift register

                if (bitsAvailable > 0)
                {
                    var numBits = Mathf.Min(bitsLeftToRead, bitsAvailable);
                    shiftRegister >>= numBits;
                    bitsLeftToRead -= numBits;
                    bitsAvailable -= numBits;
                }

                // add new bytes to shift register from input stream

                if (bitsAvailable == 0)
                {
                    if (inputDataPosition < lzwData.Length - 1)
                    {
                        // add two bytes if we can
                        shiftRegister |= ((uint) lzwData[inputDataPosition++] << 16) |
                                         ((uint) lzwData[inputDataPosition++] << 24);
                        bitsAvailable = 16;
                    }
                    else if (inputDataPosition < lzwData.Length)
                    {
                        // or if just one byte left
                        shiftRegister |= (uint) lzwData[inputDataPosition++] << 16;
                        bitsAvailable = 8;
                    }
                }

                // read any remaining bits required

                if (bitsLeftToRead > 0)
                {
                    shiftRegister >>= bitsLeftToRead;
                    bitsAvailable -= bitsLeftToRead;
                }

                // mask out code bits and shift to end

                int curCode = (int) ((shiftRegister & 0x0000FFFF) >> (16 - LzwCodeSize));

                // process code

                ushort newCode = 0;

                if (curCode == LzwClearCode)
                {
                    // reset codes
                    LzwCodeSize = LzwMinimumCodeSize + 1;
                    LzwNextSize = (int) Math.Pow(2, LzwCodeSize);
                    LzwNumCodes = LzwMaximumCodeSize + 2;

                    // reset buffer write pos
                    LzwCodeBufferLen = LzwNumCodes * 2;

                    // clear previous code
                    previousCode = -1;

                    continue;
                }
                else if (curCode == LzwEndCode)
                {
                    // stop
                    break;
                }
                else if (curCode < LzwNumCodes)
                {
                    // write existing code

                    // get position of code in buffer

                    var bufferPos = LzwCodeIndices[curCode];
                    var codeLength = LzwCodeBuffer[bufferPos++];

                    // get first code

                    newCode = LzwCodeBuffer[bufferPos];

                    // output colours

                    for (int i = 0; i < codeLength; i++)
                    {
                        WritePixel(LzwCodeBuffer[bufferPos++]);
                    }
                }
                else if (previousCode >= 0)
                {
                    // write previous code

                    // get position of code in buffer

                    var bufferPos = LzwCodeIndices[previousCode];
                    var codeLength = LzwCodeBuffer[bufferPos++];

                    // get first code

                    newCode = LzwCodeBuffer[bufferPos];

                    // output colours

                    for (int i = 0; i < codeLength; i++)
                    {
                        WritePixel(LzwCodeBuffer[bufferPos++]);
                    }

                    WritePixel(newCode);
                }
                else
                {
                    continue;
                }

                // create new code

                if (previousCode >= 0 && LzwNumCodes != LzwCodeIndices.Length)
                {
                    // get previous code from buffer
                    var bufferPosition = LzwCodeIndices[previousCode];
                    var codeLength = LzwCodeBuffer[bufferPosition++];

                    // resize buffer if required (should be rare)

                    if (LzwCodeBufferLen + codeLength + 1 >= LzwCodeBuffer.Length)
                    {
                        Array.Resize(ref LzwCodeBuffer, LzwCodeBuffer.Length * 2);
                    }

                    // add new code

                    LzwCodeIndices[LzwNumCodes++] = LzwCodeBufferLen;
                    LzwCodeBuffer[LzwCodeBufferLen++] = (ushort) (codeLength + 1);

                    // write previous code sequence

                    for (int i = 0; i < codeLength; i++)
                    {
                        LzwCodeBuffer[LzwCodeBufferLen++] = LzwCodeBuffer[bufferPosition + i];
                    }

                    // append new code

                    LzwCodeBuffer[LzwCodeBufferLen++] = newCode;
                }

                // increase code size?

                if (LzwNumCodes >= LzwNextSize && LzwCodeSize < 12)
                {
                    LzwCodeSize++;
                    LzwNextSize = (int) Math.Pow(2, LzwCodeSize);
                }

                // remeber last code processed
                previousCode = curCode;
            }

            return OutputBuffer;
        }
    }
}