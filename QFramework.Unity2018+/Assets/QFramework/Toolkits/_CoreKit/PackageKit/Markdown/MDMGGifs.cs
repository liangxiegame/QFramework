//#define mgGIF_UNSAFE

using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Runtime.InteropServices; // unsafe

namespace QFramework
{
    ////////////////////////////////////////////////////////////////////////////////

    public class MDGifImage : ICloneable
    {
        public int Width;
        public int Height;
        public int Delay; // milliseconds
        public Color32[] RawImage;

        public MDGifImage()
        {
        }

        public MDGifImage(MDGifImage img)
        {
            Width = img.Width;
            Height = img.Height;
            Delay = img.Delay;
            RawImage = img.RawImage != null ? (Color32[])img.RawImage.Clone() : null;
        }

        public object Clone()
        {
            return new MDGifImage(this);
        }

        public Texture2D CreateTexture()
        {
            var tex = new Texture2D(Width, Height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            tex.SetPixels32(RawImage);
            tex.Apply();

            return tex;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

#if mgGIF_UNSAFE
    unsafe
#endif
    public class Decoder : IDisposable
    {
        public string Version;
        public ushort Width;
        public ushort Height;
        public Color32 BackgroundColour;


        //------------------------------------------------------------------------------
        // GIF format enums

        [Flags]
        enum ImageFlag
        {
            Interlaced = 0x40,
            ColourTable = 0x80,
            TableSizeMask = 0x07,
            BitDepthMask = 0x70,
        }

        enum Block
        {
            Image = 0x2C,
            Extension = 0x21,
            End = 0x3B
        }

        enum Extension
        {
            GraphicControl = 0xF9,
            Comments = 0xFE,
            PlainText = 0x01,
            ApplicationData = 0xFF
        }

        enum Disposal
        {
            None = 0x00,
            DoNotDispose = 0x04,
            RestoreBackground = 0x08,
            ReturnToPrevious = 0x0C
        }

        [Flags]
        enum ControlFlags
        {
            HasTransparency = 0x01,
            DisposalMask = 0x0C
        }


        //------------------------------------------------------------------------------

        const uint NoCode = 0xFFFF;
        const ushort NoTransparency = 0xFFFF;

        // input stream to decode
        byte[] Input;
        int D;

        // colour table
        Color32[] GlobalColourTable;
        Color32[] LocalColourTable;
        Color32[] ActiveColourTable;
        ushort TransparentIndex;

        // current image
        MDGifImage Image = new MDGifImage();
        ushort ImageLeft;
        ushort ImageTop;
        ushort ImageWidth;
        ushort ImageHeight;

        Color32[] Output;
        Color32[] PreviousImage;

        readonly int[] Pow2 = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096 };

        //------------------------------------------------------------------------------
        // ctor

        public Decoder(byte[] data)
            : this()
        {
            Load(data);
        }

        public Decoder Load(byte[] data)
        {
            Input = data;
            D = 0;

            GlobalColourTable = new Color32[256];
            LocalColourTable = new Color32[256];
            TransparentIndex = NoTransparency;
            Output = null;
            PreviousImage = null;

            Image.Delay = 0;

            return this;
        }


        //------------------------------------------------------------------------------
        // reading data utility functions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte ReadByte()
        {
            return Input[D++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ushort ReadUInt16()
        {
            return (ushort)(Input[D++] | Input[D++] << 8);
        }

        //------------------------------------------------------------------------------

        void ReadHeader()
        {
            if (Input == null || Input.Length <= 12)
            {
                throw new Exception("Invalid data");
            }

            // signature

            Version = Encoding.ASCII.GetString(Input, 0, 6);
            D = 6;

            if (Version != "GIF87a" && Version != "GIF89a")
            {
                throw new Exception("Unsupported GIF version");
            }

            // read header

            Width = ReadUInt16();
            Height = ReadUInt16();

            Image.Width = Width;
            Image.Height = Height;

            var flags = (ImageFlag)ReadByte();
            var bgIndex = ReadByte(); // background colour

            ReadByte(); // aspect ratio

            if (flags.HasFlag(ImageFlag.ColourTable))
            {
                ReadColourTable(GlobalColourTable, flags);
            }

            BackgroundColour = GlobalColourTable[bgIndex];
        }

        //------------------------------------------------------------------------------

        public MDGifImage NextImage()
        {
            // if at start of data, read header

            if (D == 0)
            {
                ReadHeader();
            }

            // read blocks until we find an image block

            while (true)
            {
                var block = (Block)ReadByte();

                switch (block)
                {
                    case Block.Image:
                    {
                        // return the image if we got one

                        var img = ReadImageBlock();

                        if (img != null)
                        {
                            return img;
                        }
                    }
                        break;

                    case Block.Extension:
                    {
                        var ext = (Extension)ReadByte();

                        if (ext == Extension.GraphicControl)
                        {
                            ReadControlBlock();
                        }
                        else
                        {
                            SkipBlocks();
                        }
                    }
                        break;

                    case Block.End:
                    {
                        // end block - stop!
                        return null;
                    }

                    default:
                    {
                        throw new Exception("Unexpected block type");
                    }
                }
            }
        }

        //------------------------------------------------------------------------------

        Color32[] ReadColourTable(Color32[] colourTable, ImageFlag flags)
        {
            var tableSize = Pow2[(int)(flags & ImageFlag.TableSizeMask) + 1];

            for (var i = 0; i < tableSize; i++)
            {
                colourTable[i] = new Color32(
                    Input[D++],
                    Input[D++],
                    Input[D++],
                    0xFF
                );
            }

            return colourTable;
        }

        //------------------------------------------------------------------------------

        void SkipBlocks()
        {
            var blockSize = Input[D++];

            while (blockSize != 0x00)
            {
                D += blockSize;
                blockSize = Input[D++];
            }
        }

        //------------------------------------------------------------------------------

        void ReadControlBlock()
        {
            // read block

            ReadByte(); // block size (0x04)
            var flags = (ControlFlags)ReadByte(); // flags
            Image.Delay = ReadUInt16() * 10; // delay (1/100th -> milliseconds)
            var transparentColour = ReadByte(); // transparent colour
            ReadByte(); // terminator (0x00)

            // has transparent colour?

            if (flags.HasFlag(ControlFlags.HasTransparency))
            {
                TransparentIndex = transparentColour;
            }
            else
            {
                TransparentIndex = NoTransparency;
            }

            // dispose of current image

            switch ((Disposal)(flags & ControlFlags.DisposalMask))
            {
                default:
                case Disposal.None:
                case Disposal.DoNotDispose:
                    // remember current image in case we need to "return to previous"
                    PreviousImage = Output;
                    break;

                case Disposal.RestoreBackground:
                    // empty image - don't track
                    Output = new Color32[Width * Height];
                    break;

                case Disposal.ReturnToPrevious:

                    // return to previous image

                    Output = new Color32[Width * Height];

                    if (PreviousImage != null)
                    {
                        Array.Copy(PreviousImage, Output, Output.Length);
                    }

                    break;
            }
        }

        //------------------------------------------------------------------------------

        MDGifImage ReadImageBlock()
        {
            // read image block header

            ImageLeft = ReadUInt16();
            ImageTop = ReadUInt16();
            ImageWidth = ReadUInt16();
            ImageHeight = ReadUInt16();
            var flags = (ImageFlag)ReadByte();

            // bad image if we don't have any dimensions

            if (ImageWidth == 0 || ImageHeight == 0)
            {
                return null;
            }

            // read colour table

            if (flags.HasFlag(ImageFlag.ColourTable))
            {
                ActiveColourTable = ReadColourTable(LocalColourTable, flags);
            }
            else
            {
                ActiveColourTable = GlobalColourTable;
            }

            if (Output == null)
            {
                Output = new Color32[Width * Height];
                PreviousImage = Output;
            }

            // read image data

            DecompressLZW();

            // deinterlace

            if (flags.HasFlag(ImageFlag.Interlaced))
            {
                Deinterlace();
            }

            // return image

            Image.RawImage = Output;
            return Image;
        }

        //------------------------------------------------------------------------------
        // decode interlaced images

        void Deinterlace()
        {
            var numRows = Output.Length / Width;
            var writePos = Output.Length - Width; // NB: work backwards due to Y-coord flip
            var input = Output;

            Output = new Color32[Output.Length];

            for (var row = 0; row < numRows; row++)
            {
                int copyRow;

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

                Array.Copy(input, (numRows - copyRow - 1) * Width, Output, writePos, Width);

                writePos -= Width;
            }
        }

        //------------------------------------------------------------------------------
        // DecompressLZW()

#if mgGIF_UNSAFE
        bool        Disposed = false;

        int         CodesLength;
        IntPtr      CodesHandle;
        ushort*     pCodes;

        IntPtr      CurBlock;
        uint*       pCurBlock;

        const int   MaxCodes = 4096;
        IntPtr      Indices;
        ushort**    pIndicies;

        public Decoder()
        {
            // unmanaged allocations

            CodesLength = 128 * 1024;
            CodesHandle = Marshal.AllocHGlobal( CodesLength * sizeof( ushort ) );
            pCodes = (ushort*) CodesHandle.ToPointer();

            CurBlock = Marshal.AllocHGlobal( 64 * sizeof( uint ) );
            pCurBlock = (uint*) CurBlock.ToPointer();

            Indices = Marshal.AllocHGlobal( MaxCodes * sizeof( ushort* ) );
            pIndicies = (ushort**) Indices.ToPointer();
        }

        protected virtual void Dispose( bool disposing )
        {
            if( Disposed )
            {
                return;
            }

            // release unmanaged resources

            Marshal.FreeHGlobal( CodesHandle );
            Marshal.FreeHGlobal( CurBlock );
            Marshal.FreeHGlobal( Indices );
            
            Disposed = true;
        }

        ~Decoder()
        {
            Dispose( false );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        void DecompressLZW()
        {
            var pCodeBufferEnd = pCodes + CodesLength;

            fixed( byte* pData = Input )
            {
                fixed( Color32* pOutput = Output, pColourTable = ActiveColourTable )
                {
                    var row =
 ( Height - ImageTop - 1 ) * Width; // start at end of array as we are reversing the row order
                    var safeWidth = ImageLeft + ImageWidth > Width ? Width - ImageLeft : ImageWidth;

                    var pWrite = &pOutput[ row + ImageLeft ];
                    var pRow = pWrite;
                    var pRowEnd = pWrite + ImageWidth;
                    var pImageEnd = pWrite + safeWidth;

                    // setup codes

                    int minimumCodeSize = Input[ D++ ];

                    if( minimumCodeSize > 11 )
                    {
                        minimumCodeSize = 11;
                    }

                    var codeSize = minimumCodeSize + 1;
                    var nextSize = Pow2[ codeSize ];
                    var maximumCodeSize = Pow2[ minimumCodeSize ];
                    var clearCode = maximumCodeSize;
                    var endCode = maximumCodeSize + 1;

                    // initialise buffers

                    var numCodes = maximumCodeSize + 2;
                    var pCodesEnd = pCodes;

                    for( ushort i = 0; i < numCodes; i++ )
                    {
                        pIndicies[ i ] = pCodesEnd;
                        *pCodesEnd++ = 1;
                        *pCodesEnd++ = i;
                    }

                    // LZW decode loop

                    uint previousCode = NoCode;   // last code processed
                    uint mask = (uint) ( nextSize - 1 ); // mask out code bits
                    uint shiftRegister =
 0;        // shift register holds the bytes coming in from the input stream, we shift down by the number of bits

                    int  bitsAvailable = 0;        // number of bits available to read in the shift register
                    int  bytesAvailable = 0;        // number of bytes left in current block

                    uint* pD = pCurBlock;           // pointer to next bits in current block

                    while( true )
                    {
                        // get next code

                        uint curCode = shiftRegister & mask;

                        // did we read enough bits?

                        if( bitsAvailable >= codeSize )
                        {
                            // we had enough bits in the shift register so shunt it down
                            bitsAvailable -= codeSize;
                            shiftRegister >>= codeSize;
                        }
                        else
                        {
                            // not enough bits in register, so get more

                            // if start of new block

                            if( bytesAvailable <= 0 )
                            {
                                // read blocksize

                                var pBlock = &pData[ D++ ];
                                bytesAvailable = *pBlock++;
                                D += bytesAvailable;

                                // exit if end of stream

                                if( bytesAvailable == 0 )
                                {
                                    return;
                                }

                                // copy block into buffer

                                pCurBlock[ ( bytesAvailable - 1 ) / 4 ] = 0; // zero last entry
                                Buffer.MemoryCopy( pBlock, pCurBlock, 256, bytesAvailable );

                                // reset data pointer
                                pD = pCurBlock;
                            }

                            // load shift register from data pointer

                            shiftRegister = *pD++;
                            int newBits = bytesAvailable >= 4 ? 32 : bytesAvailable * 8;
                            bytesAvailable -= 4;

                            // read remaining bits

                            if( bitsAvailable > 0 )
                            {
                                var bitsRemaining = codeSize - bitsAvailable;
                                curCode |= ( shiftRegister << bitsAvailable ) & mask;
                                shiftRegister >>= bitsRemaining;
                                bitsAvailable = newBits - bitsRemaining;
                            }
                            else
                            {
                                curCode = shiftRegister & mask;
                                shiftRegister >>= codeSize;
                                bitsAvailable = newBits - codeSize;
                            }
                        }

                        // process code

                        if( curCode == clearCode )
                        {
                            // reset codes
                            codeSize = minimumCodeSize + 1;
                            nextSize = Pow2[ codeSize ];
                            numCodes = maximumCodeSize + 2;

                            // reset buffer write pos
                            pCodesEnd = &pCodes[ numCodes * 2 ];

                            // clear previous code
                            previousCode = NoCode;
                            mask = (uint)( nextSize - 1 );

                            continue;
                        }
                        else if( curCode == endCode )
                        {
                            // stop
                            break;
                        }

                        bool plusOne = false;
                        ushort* pCodePos = null;

                        if( curCode < numCodes )
                        {
                            // write existing code
                            pCodePos = pIndicies[ curCode ];
                        }
                        else if( previousCode != NoCode )
                        {
                            // write previous code
                            pCodePos = pIndicies[ previousCode ];
                            plusOne = true;
                        }
                        else
                        {
                            continue;
                        }


                        // output colours

                        var codeLength = *pCodePos++;
                        var newCode = *pCodePos;
                        var pEnd = pCodePos + codeLength;

                        do
                        {
                            var code = *pCodePos++;

                            if( code != TransparentIndex && pWrite < pImageEnd )
                            {
                                *pWrite = pColourTable[ code ];
                            }

                            if( ++pWrite == pRowEnd )
                            {
                                pRow -= Width;
                                pWrite = pRow;
                                pRowEnd = pRow + ImageWidth;
                                pImageEnd = pRow + safeWidth;

                                if( pWrite < pOutput )
                                {
                                    SkipBlocks();
                                    return;
                                }
                            }
                        }
                        while( pCodePos < pEnd );

                        if( plusOne )
                        {
                            if( newCode != TransparentIndex && pWrite < pImageEnd )
                            {
                                *pWrite = pColourTable[ newCode ];
                            }

                            if( ++pWrite == pRowEnd )
                            {
                                pRow -= Width;
                                pWrite = pRow;
                                pRowEnd = pRow + ImageWidth;
                                pImageEnd = pRow + safeWidth;

                                if( pWrite < pOutput )
                                {
                                    break;
                                }
                            }
                        }

                        // create new code

                        if( previousCode != NoCode && numCodes != MaxCodes )
                        {
                            // get previous code from buffer

                            pCodePos = pIndicies[ previousCode ];
                            codeLength = *pCodePos++;

                            // resize buffer if required (should be rare)

                            if( pCodesEnd + codeLength + 1 >= pCodeBufferEnd )
                            {
                                var pBase = pCodes;

                                // realloc buffer
                                CodesLength *= 2;
                                CodesHandle =
 Marshal.ReAllocHGlobal( CodesHandle, (IntPtr)( CodesLength * sizeof( ushort ) ) );

                                pCodes = (ushort*) CodesHandle.ToPointer();
                                pCodeBufferEnd = pCodes + CodesLength;

                                // rebase pointers

                                pCodesEnd = pCodes + ( pCodesEnd - pBase );

                                for( int i = 0; i < numCodes; i++ )
                                {
                                    pIndicies[ i ] = pCodes + ( pIndicies[ i ] - pBase );
                                }

                                pCodePos = pIndicies[ previousCode ];
                                pCodePos++;
                            }

                            // add new code

                            pIndicies[ numCodes++ ] = pCodesEnd;
                            *pCodesEnd++ = (ushort)( codeLength + 1 );

                            // copy previous code sequence

                            Buffer.MemoryCopy( pCodePos, pCodesEnd, codeLength * sizeof( ushort ), codeLength * sizeof( ushort ) );
                            pCodesEnd += codeLength;

                            // append new code

                            *pCodesEnd++ = newCode;
                        }

                        // increase code size?

                        if( numCodes >= nextSize && codeSize < 12 )
                        {
                            nextSize = Pow2[ ++codeSize ];
                            mask = (uint)( nextSize - 1 );
                        }

                        // remember last code processed
                        previousCode = curCode;
                    }

                    // consume any remaining blocks
                    SkipBlocks();
                }
            }
        }

#else

        // dispose isn't needed for the safe implementation but keep here for interface parity

        public Decoder()
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }


        int[] Indices = new int[4096];
        ushort[] Codes = new ushort[128 * 1024];
        uint[] CurBlock = new uint[64];

        void DecompressLZW()
        {
            // output write position

            int row = (Height - ImageTop - 1) * Width; // reverse rows for unity texture coords
            int col = ImageLeft;
            int rightEdge = ImageLeft + ImageWidth;

            // setup codes

            int minimumCodeSize = Input[D++];

            if (minimumCodeSize > 11)
            {
                minimumCodeSize = 11;
            }

            var codeSize = minimumCodeSize + 1;
            var nextSize = Pow2[codeSize];
            var maximumCodeSize = Pow2[minimumCodeSize];
            var clearCode = maximumCodeSize;
            var endCode = maximumCodeSize + 1;

            // initialise buffers

            var codesEnd = 0;
            var numCodes = maximumCodeSize + 2;

            for (ushort i = 0; i < numCodes; i++)
            {
                Indices[i] = codesEnd;
                Codes[codesEnd++] = 1; // length
                Codes[codesEnd++] = i; // code
            }

            // LZW decode loop

            uint previousCode = NoCode; // last code processed
            uint mask = (uint)(nextSize - 1); // mask out code bits
            uint
                shiftRegister =
                    0; // shift register holds the bytes coming in from the input stream, we shift down by the number of bits

            int bitsAvailable = 0; // number of bits available to read in the shift register
            int bytesAvailable = 0; // number of bytes left in current block

            int blockPos = 0;

            while (true)
            {
                // get next code

                uint curCode = shiftRegister & mask;

                if (bitsAvailable >= codeSize)
                {
                    bitsAvailable -= codeSize;
                    shiftRegister >>= codeSize;
                }
                else
                {
                    // reload shift register


                    // if start of new block

                    if (bytesAvailable <= 0)
                    {
                        // read blocksize
                        bytesAvailable = Input[D++];

                        // exit if end of stream
                        if (bytesAvailable == 0)
                        {
                            return;
                        }

                        // read block
                        CurBlock[(bytesAvailable - 1) / 4] = 0; // zero last entry
                        Buffer.BlockCopy(Input, D, CurBlock, 0, bytesAvailable);
                        blockPos = 0;
                        D += bytesAvailable;
                    }

                    // load shift register

                    shiftRegister = CurBlock[blockPos++];
                    int newBits = bytesAvailable >= 4 ? 32 : bytesAvailable * 8;
                    bytesAvailable -= 4;

                    // read remaining bits

                    if (bitsAvailable > 0)
                    {
                        var bitsRemaining = codeSize - bitsAvailable;
                        curCode |= (shiftRegister << bitsAvailable) & mask;
                        shiftRegister >>= bitsRemaining;
                        bitsAvailable = newBits - bitsRemaining;
                    }
                    else
                    {
                        curCode = shiftRegister & mask;
                        shiftRegister >>= codeSize;
                        bitsAvailable = newBits - codeSize;
                    }
                }

                // process code

                if (curCode == clearCode)
                {
                    // reset codes
                    codeSize = minimumCodeSize + 1;
                    nextSize = Pow2[codeSize];
                    numCodes = maximumCodeSize + 2;

                    // reset buffer write pos
                    codesEnd = numCodes * 2;

                    // clear previous code
                    previousCode = NoCode;
                    mask = (uint)(nextSize - 1);

                    continue;
                }
                else if (curCode == endCode)
                {
                    // stop
                    break;
                }

                bool plusOne = false;
                int codePos = 0;

                if (curCode < numCodes)
                {
                    // write existing code
                    codePos = Indices[curCode];
                }
                else if (previousCode != NoCode)
                {
                    // write previous code
                    codePos = Indices[previousCode];
                    plusOne = true;
                }
                else
                {
                    continue;
                }


                // output colours

                var codeLength = Codes[codePos++];
                var newCode = Codes[codePos];

                for (int i = 0; i < codeLength; i++)
                {
                    var code = Codes[codePos++];

                    if (code != TransparentIndex && col < Width)
                    {
                        Output[row + col] = ActiveColourTable[code];
                    }

                    if (++col == rightEdge)
                    {
                        col = ImageLeft;
                        row -= Width;

                        if (row < 0)
                        {
                            SkipBlocks();
                            return;
                        }
                    }
                }

                if (plusOne)
                {
                    if (newCode != TransparentIndex && col < Width)
                    {
                        Output[row + col] = ActiveColourTable[newCode];
                    }

                    if (++col == rightEdge)
                    {
                        col = ImageLeft;
                        row -= Width;

                        if (row < 0)
                        {
                            break;
                        }
                    }
                }

                // create new code

                if (previousCode != NoCode && numCodes != Indices.Length)
                {
                    // get previous code from buffer

                    codePos = Indices[previousCode];
                    codeLength = Codes[codePos++];

                    // resize buffer if required (should be rare)

                    if (codesEnd + codeLength + 1 >= Codes.Length)
                    {
                        Array.Resize(ref Codes, Codes.Length * 2);
                    }

                    // add new code

                    Indices[numCodes++] = codesEnd;
                    Codes[codesEnd++] = (ushort)(codeLength + 1);

                    // copy previous code sequence

                    var stop = codesEnd + codeLength;

                    while (codesEnd < stop)
                    {
                        Codes[codesEnd++] = Codes[codePos++];
                    }

                    // append new code

                    Codes[codesEnd++] = newCode;
                }

                // increase code size?

                if (numCodes >= nextSize && codeSize < 12)
                {
                    nextSize = Pow2[++codeSize];
                    mask = (uint)(nextSize - 1);
                }

                // remember last code processed
                previousCode = curCode;
            }

            // skip any remaining blocks
            SkipBlocks();
        }
#endif // mgGIF_UNSAFE

        public static string Ident()
        {
            var v = "1.1";
            var e = BitConverter.IsLittleEndian ? "L" : "B";

#if ENABLE_IL2CPP
            var b = "N";
#else
            var b = "M";
#endif

#if mgGIF_UNSAFE
            var s = "U";
#else
            var s = "S";
#endif

#if NET_4_6
            var n = "4.x";
#else
            var n = "2.0";
#endif

            return $"{v} {e}{s}{b} {n}";
        }
    }
}