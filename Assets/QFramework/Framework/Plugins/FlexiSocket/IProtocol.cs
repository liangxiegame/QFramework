// *************************************************************************************************
// The MIT License (MIT)
// 
// Copyright (c) 2016 Sean
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *************************************************************************************************
// Project source: https://github.com/theoxuan/FlexiSocket

using System;
using System.IO;
using System.Text;

namespace QF
{
    /// <summary>
    /// Message protocol to encode/decode message, also ensure the received message to be complete and correct
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// Encoding format
        /// </summary>
        Encoding Encoding { get; }

        /// <summary>
        /// Check message's completeness
        /// </summary>
        /// <param name="stream">Message stream</param>
        /// <returns>True if complete</returns>
        /// <remarks>
        /// You might need to reset the stream's <see cref="MemoryStream.Position"/> before checking, but make sure to set it back after checking
        /// <para/>
        /// Do not <see cref="MemoryStream.Close"/> or <see cref="MemoryStream.Dispose"/> the stream anyhow
        /// </remarks>
        /// <example>
        /// <code>
        /// var position = stream.Position;
        /// stream.Seek(0, SeekOrigin.Begin);
        /// var reader = new BinaryReader(stream);
        /// var length = reader.ReadInt32();
        /// stream.Seek(position, SeekOrigin.Begin);
        /// var completed = stream.Length &gt;= length + sizeof (int);
        /// </code>
        /// </example>
        bool IsComplete(MemoryStream stream);

        /// <summary>
        /// Decode message
        /// </summary>
        /// <param name="stream">Message stream</param>
        /// <returns>Decoded data</returns>
        /// <remarks>
        /// You might need to reset the stream's <see cref="MemoryStream.Position"/> before checking
        /// </remarks>
        /// <example>
        /// <code>
        /// var position = stream.Position;
        /// stream.Seek(0, SeekOrigin.Begin);
        /// var reader = new BinaryReader(stream);
        /// var length = reader.ReadInt32();
        /// var message = reader.ReadBytes(length);
        /// </code>
        /// </example>
        byte[] Decode(MemoryStream stream);

        /// <summary>
        /// Encode message
        /// </summary>
        /// <param name="buffer">Message buffer</param>
        /// <returns>Encoded data</returns>
        byte[] Encode(byte[] buffer);
    }

    /// <summary>
    /// Pre-defined protocols
    /// </summary>
    public sealed class Protocols
    {
        /// <summary>
        /// Default protocol
        /// </summary>
        /// <remarks>
        /// No encoding/decoding and no packet checking
        /// <para/>
        /// Each packet will be dispatched directly
        /// </remarks>
        public static readonly IProtocol None = new DefaultProtocol();

        /// <summary>
        /// Head + Body structure type
        /// </summary>
        /// <remarks>
        /// The message head is a 4-byte <seealso cref="int"/> which represents the length of the body
        /// </remarks>
        public static readonly IProtocol BodyLengthPrefix = new BodyLengthPrefixProtocol();

        /// <summary>
        /// Body + Terminate Tag structure type
        /// </summary>
        /// <remarks>
        /// The message tail is <c>&lt;EOF&gt;</c> which represents the end of a string message
        /// <para/>
        /// To specify a custom tag, you might use <see cref="StringTerminatedBy"/>
        /// </remarks>
        public static readonly IProtocol StringTerminated = new StringTerminatedProtocol("<EOF>", Encoding.UTF8);

        /// <summary>
        /// Head + Body structure type
        /// </summary>
        /// <remarks>
        /// The message head is a 4-byte <see cref="int"/> which represents the total length, i.e. head's length(4) plus body's length
        /// <para/>
        /// In addition, it converts LittleEndian to BigEndian(for specific use like php's pack format http://php.net/manual/en/function.pack.php)
        /// </remarks>
        public static readonly IProtocol TotalLengthPrefix = new TotalLengthPrefixProtocol();

        /// <summary>
        /// Body + Terminate Tag structure type
        /// </summary>
        /// <param name="tag">End tag</param>
        /// <param name="encoding">Encoding</param>
        /// <returns></returns>
        /// <remarks>
        /// The message tail is a user-defined tag which represents the end of a string message
        /// </remarks>
        public static IProtocol StringTerminatedBy(string tag, Encoding encoding)
        {
            return new StringTerminatedProtocol(tag, encoding);
        }

        /// <summary>
        /// Fixed-length structure type
        /// </summary>
        /// <param name="length">Message length</param>
        /// <remarks>
        /// Each message has a fixed length of the specified value
        /// </remarks>
        /// <returns></returns>
        public static IProtocol FixedLengthOf(int length)
        {
            return new FixedLengthProtocol(length);
        }

        private sealed class DefaultProtocol : IProtocol
        {
            #region Implementation of IProtocol

            Encoding IProtocol.Encoding
            {
                get { return Encoding.Default; }
            }

            bool IProtocol.IsComplete(MemoryStream stream)
            {
                return true;
            }

            byte[] IProtocol.Decode(MemoryStream stream)
            {
                return stream.ToArray();
            }

            byte[] IProtocol.Encode(byte[] buffer)
            {
                return buffer;
            }

            #endregion
        }

        private sealed class BodyLengthPrefixProtocol : IProtocol
        {
            #region Implementation of IProtocol

            Encoding IProtocol.Encoding
            {
                get { return Encoding.UTF8; }
            }

            bool IProtocol.IsComplete(MemoryStream stream)
            {
                if (stream.Length < sizeof (int))
                    return false;
                var position = stream.Position;
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new BinaryReader(stream);
                var length = reader.ReadInt32();
                stream.Seek(position, SeekOrigin.Begin);
                return stream.Length >= length + sizeof (int);
            }

            byte[] IProtocol.Decode(MemoryStream stream)
            {
                if (stream.Length < sizeof (int))
                    throw new InvalidOperationException();
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new BinaryReader(stream);
                var length = reader.ReadInt32();
                if (length + sizeof (int) > stream.Length)
                    throw new InvalidOperationException();
                return reader.ReadBytes(length);
            }

            byte[] IProtocol.Encode(byte[] buffer)
            {
                var head = BitConverter.GetBytes(buffer.Length);
                var output = new byte[head.Length + buffer.Length];
                head.CopyTo(output, 0);
                buffer.CopyTo(output, head.Length);
                return output;
            }

            #endregion
        }

        private sealed class StringTerminatedProtocol : IProtocol
        {
            private readonly string _tag;
            private readonly Encoding _encoding;

            public StringTerminatedProtocol(string tag, Encoding encoding)
            {
                _tag = tag;
                _encoding = encoding;
            }

            #region Implementation of IProtocol

            Encoding IProtocol.Encoding
            {
                get { return _encoding; }
            }

            bool IProtocol.IsComplete(MemoryStream stream)
            {
                var position = stream.Position;
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();
                stream.Seek(position, SeekOrigin.Begin);
                return text.EndsWith(_tag);
            }

            byte[] IProtocol.Decode(MemoryStream stream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);
                var data = reader.ReadToEnd();
                if (!data.EndsWith(_tag))
                    throw new InvalidOperationException();
                return _encoding.GetBytes(data.Substring(0, data.Length - _tag.Length));
            }

            byte[] IProtocol.Encode(byte[] buffer)
            {
                return _encoding.GetBytes(_encoding.GetString(buffer) + _tag);
            }

            #endregion
        }

        private sealed class FixedLengthProtocol : IProtocol
        {
            private readonly int length;

            public FixedLengthProtocol(int length)
            {
                this.length = length;
            }

            #region Implementation of IProtocol

            Encoding IProtocol.Encoding
            {
                get { return Encoding.UTF8; }
            }

            bool IProtocol.IsComplete(MemoryStream stream)
            {
                return stream.Length >= length;
            }

            byte[] IProtocol.Decode(MemoryStream stream)
            {
                if (stream.Length < length)
                    throw new InvalidOperationException();
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new BinaryReader(stream);
                return reader.ReadBytes(length);
            }

            byte[] IProtocol.Encode(byte[] buffer)
            {
                if (buffer.Length > length)
                    throw new InvalidOperationException();
                var data = new byte[length];
                buffer.CopyTo(data, 0);
                return data;
            }

            #endregion
        }

        private sealed class TotalLengthPrefixProtocol : IProtocol
        {
            #region Implementation of IProtocol

            Encoding IProtocol.Encoding
            {
                get { return Encoding.UTF8; }
            }

            bool IProtocol.IsComplete(MemoryStream stream)
            {
                if (stream.Length < sizeof (int))
                    return false;
                var position = stream.Position;
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new BinaryReader(stream);
                var head = reader.ReadBytes(sizeof (int));
                var length = ToLittleEndian(head);
                stream.Seek(position, SeekOrigin.Begin);
                return stream.Length >= length;
            }

            byte[] IProtocol.Decode(MemoryStream stream)
            {
                if (stream.Length < sizeof (int))
                    throw new InvalidOperationException();
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new BinaryReader(stream);
                var head = reader.ReadBytes(sizeof (int));
                var length = ToLittleEndian(head);
                if (length > stream.Length)
                    throw new InvalidOperationException();
                return reader.ReadBytes(length - sizeof (int));
            }

            byte[] IProtocol.Encode(byte[] buffer)
            {
                var length = buffer.Length + sizeof (int);
                var head = ToBigEndian(length);
                var output = new byte[length];
                head.CopyTo(output, 0);
                buffer.CopyTo(output, sizeof (int));
                return output;
            }

            #endregion

            private byte[] ToBigEndian(int value)
            {
                var bytes = BitConverter.GetBytes(value);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);
                return bytes;
            }

            private int ToLittleEndian(byte[] bytes)
            {
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
        }
    }
}