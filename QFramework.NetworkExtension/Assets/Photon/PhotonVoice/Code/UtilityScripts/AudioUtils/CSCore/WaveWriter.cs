using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CSCore.Codecs.WAV
{
    /// <summary>
    ///     Encoder for wave files.
    /// </summary>
    public class WaveWriter : IDisposable, IWriteable
    {
        private readonly WaveFormat _waveFormat;

        private readonly long _waveStartPosition;
        private int _dataLength;
        private bool _isDisposed;
        private Stream _stream;
        private BinaryWriter _writer;
        private bool _isDisposing;

        private readonly bool _closeStream;

        /// <summary>
        /// Signals if the object has already been disposed
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        /// <summary>
        /// Signals if the object is in a disposing state
        /// </summary>
        public bool IsDisposing
        {
            get
            {
                return _isDisposing;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaveWriter" /> class.
        /// </summary>
        /// <param name="fileName">Filename of the destination file. This filename should typically end with the .wav extension.</param>
        /// <param name="waveFormat">
        ///     Format of the waveform-audio data. Note that the <see cref="WaveWriter" /> won't convert any
        ///     data.
        /// </param>
        public WaveWriter(string fileName, WaveFormat waveFormat)
            : this(File.OpenWrite(fileName), waveFormat)
        {
            _closeStream = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaveWriter" /> class.
        /// </summary>
        /// <param name="stream">Destination stream which should be used to store the</param>
        /// <param name="waveFormat">
        ///     Format of the waveform-audio data. Note that the <see cref="WaveWriter" /> won't convert any
        ///     data.
        /// </param>
        public WaveWriter(Stream stream, WaveFormat waveFormat)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanWrite)
                throw new ArgumentException("Stream not writeable.", "stream");
            if (!stream.CanSeek)
                throw new ArgumentException("Stream not seekable.", "stream");

            _isDisposing = false;
            _isDisposed = false;
            _stream = stream;
            _waveStartPosition = stream.Position;
            _writer = new BinaryWriter(stream);
            for (int i = 0; i < 44; i++)
            {
                _writer.Write((byte) 0);
            }
            _waveFormat = waveFormat;

            WriteHeader();

            _closeStream = false;
        }

        /// <summary>
        ///     Disposes the <see cref="WaveWriter" /> and writes down the wave header.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Writes down all audio data of the <see cref="IWaveSource" /> to a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="source">The source to write down to the file.</param>
        /// <param name="deleteFileIfAlreadyExists">if set to <c>true</c> the file will be overritten if it already exists.</param>
        /// <param name="maxlength">The maximum number of bytes to write. Use -1 to write an infinte number of bytes.</param>
        /// <remarks>
        /// This method is obsolete. Use the <see cref="Extensions.WriteToWaveStream" /> extension instead.
        /// </remarks>
        [Obsolete("Use the Extensions.WriteToWaveStream extension instead.")]
        public static void WriteToFile(string filename, IWaveSource source, bool deleteFileIfAlreadyExists,
            int maxlength = -1)
        {
            if (deleteFileIfAlreadyExists && File.Exists(filename))
                File.Delete(filename);

            int r = 0;
            var buffer = new byte[source.WaveFormat.BytesPerSecond];
            using (var w = new WaveWriter(filename, source.WaveFormat))
            {
                int read;
                while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    w.Write(buffer, 0, read);
                    r += read;
                    if (maxlength != -1 && r > maxlength)
                        break;
                }
            }
        }

        /// <summary>
        ///     Encodes a single sample.
        /// </summary>
        /// <param name="sample">The sample to encode.</param>
        public void WriteSample(float sample)
        {
            CheckObjectDisposed();

            if (sample < -1 || sample > 1)
                sample = Math.Max(-1, Math.Min(1, sample));

            if (_waveFormat.IsPCM())
            {
                switch (_waveFormat.BitsPerSample)
                {
                    case 8:
                        Write((byte) (byte.MaxValue * sample));
                        break;
                    case 16:
                        Write((short) (short.MaxValue * sample));
                        break;
                    case 24:
                        byte[] buffer = BitConverter.GetBytes((int)(0x7fffff * sample));
                        Write(new[] {buffer[0], buffer[1], buffer[2]}, 0, 3);
                        break;
                    case 32:
                        Write((int) (int.MaxValue * sample));
                        break;

                    default:
                        throw new InvalidOperationException("Invalid Waveformat",
                            new InvalidOperationException("Invalid BitsPerSample while using PCM encoding."));
                }
            }
            else if (_waveFormat.IsIeeeFloat())
                Write(sample);
            else if (_waveFormat.WaveFormatTag == AudioEncoding.Extensible && _waveFormat.BitsPerSample == 32)
                Write(UInt16.MaxValue * (int)sample);
            else
            {
                throw new InvalidOperationException(
                    "Invalid Waveformat: Waveformat has to be PCM[8, 16, 24, 32] or IeeeFloat[32]");
            }
        }

        /// <summary>
        ///     Encodes multiple samples.
        /// </summary>
        /// <param name="samples">Float array which contains the samples to encode.</param>
        /// <param name="offset">Zero-based offset in the <paramref name="samples" /> array.</param>
        /// <param name="count">Number of samples to encode.</param>
        public void WriteSamples(float[] samples, int offset, int count)
        {
            CheckObjectDisposed();

            for (int i = offset; i < offset + count; i++)
            {
                WriteSample(samples[i]);
            }
        }

        /// <summary>
        ///     Encodes raw data in the form of a byte array.
        /// </summary>
        /// <param name="buffer">Byte array which contains the data to encode.</param>
        /// <param name="offset">Zero-based offset in the <paramref name="buffer" />.</param>
        /// <param name="count">Number of bytes to encode.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            CheckObjectDisposed();

            _stream.Write(buffer, offset, count);
            _dataLength += count;
        }

        /// <summary>
        ///     Writes down a single byte.
        /// </summary>
        /// <param name="value">Byte to write down.</param>
        public void Write(byte value)
        {
            CheckObjectDisposed();

            _writer.Write(value);
            _dataLength++;
        }

        /// <summary>
        ///     Writes down a single 16 bit integer value.
        /// </summary>
        /// <param name="value">Value to write down.</param>
        public void Write(short value)
        {
            CheckObjectDisposed();

            _writer.Write(value);
            _dataLength += 2;
        }

        /// <summary>
        ///     Writes down a single 32 bit integer value.
        /// </summary>
        /// <param name="value">Value to write down.</param>
        public void Write(int value)
        {
            CheckObjectDisposed();

            _writer.Write(value);
            _dataLength += 4;
        }

        /// <summary>
        ///     Writes down a single 32 bit float value.
        /// </summary>
        /// <param name="value">Value to write down.</param>
        public void Write(float value)
        {
            CheckObjectDisposed();

            _writer.Write(value);
            _dataLength += 4;
        }

        private void WriteHeader()
        {
            _writer.Flush();

            long currentPosition = _stream.Position;
            _stream.Position = _waveStartPosition;

            WriteRiffHeader();
            WriteFmtChunk();
            WriteDataChunk();

            _writer.Flush();

            _stream.Position = currentPosition;
        }

        private void WriteRiffHeader()
        {
            _writer.Write(Encoding.UTF8.GetBytes("RIFF"));
            _writer.Write((int) (_stream.Length - 8));
            _writer.Write(Encoding.UTF8.GetBytes("WAVE"));
        }

        private void WriteFmtChunk()
        {
            AudioEncoding tag = _waveFormat.WaveFormatTag;
            if (tag == AudioEncoding.Extensible && _waveFormat is WaveFormatExtensible)
                tag = AudioSubTypes.EncodingFromSubType((_waveFormat as WaveFormatExtensible).SubFormat);

            _writer.Write(Encoding.UTF8.GetBytes("fmt "));
            _writer.Write((int)16);
            _writer.Write((short) tag);
            _writer.Write((short)_waveFormat.Channels);
            _writer.Write((int)_waveFormat.SampleRate);
            _writer.Write((int)_waveFormat.BytesPerSecond);
            _writer.Write((short) _waveFormat.BlockAlign);
            _writer.Write((short)_waveFormat.BitsPerSample);
        }

        private void WriteDataChunk()
        {
            _writer.Write(Encoding.UTF8.GetBytes("data"));
            _writer.Write(_dataLength);
        }

        private void CheckObjectDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("WaveWriter");
        }

        /// <summary>
        ///     Disposes the <see cref="WaveWriter" /> and writes down the wave header.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to release only unmanaged
        ///     resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (!disposing) return;
            
            try
            {
                _isDisposing = true;
                WriteHeader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WaveWriter::Dispose: " + ex);
            }
            finally
            {
                if (_closeStream)
                {
                    if (_writer != null)
                    {
                        _writer.Close();
                        _writer = null;
                    }

                    if (_stream != null)
                    {
                        _stream.Dispose();
                        _stream = null;
                    }
                }

                _isDisposing = false;
            }
            
            
            _isDisposed = true;
        }

        /// <summary>
        ///     Destructor of the <see cref="WaveWriter" /> which calls the <see cref="Dispose(bool)" /> method.
        /// </summary>
        ~WaveWriter()
        {
            Dispose(false);
        }
    }
}