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
using System.Collections;
using System.Net.Sockets;

namespace QFramework
{ 
    /// <summary>
    /// Async send operation
    /// </summary>
    public sealed class AsyncSend : AsyncIOOperation
    {
        private readonly byte[] _buffer;

        /// <summary>
        /// Send completed callback
        /// </summary>
        public event SentCallback Completed;

        public AsyncSend(Socket socket, IProtocol protocol, byte[] data) : base(socket, protocol)
        {
            _buffer = protocol.Encode(data);
        }

        #region Overrides of AsyncSocketOperation

        public override bool IsSuccessful
        {
            get { return IsCompleted && Exception == null && Error == SocketError.Success; }
        }

        public override void Dispose()
        {
            try
            {
                if (ar != null && !ar.IsCompleted)
                    socket.EndSend(ar);
            }
            catch
            {
                // ignored
            }
        }

        protected override IEnumerator GetEnumerator()
        {
            var length = _buffer.Length;
            while (length > 0)
            {
                try
                {
                    SocketError error;
                    ar = socket.BeginSend(_buffer, _buffer.Length - length, _buffer.Length, SocketFlags.None, out error,
                        null, null);

                    Error = error;

                    if (Error != SocketError.Success)
                    {
                        OnCompleted(false, Exception, Error);
                        yield break;
                    }
                }
                catch (Exception ex)
                {
                    Exception = ex;
                    OnCompleted(false, Exception, Error);
                    yield break;
                }

                while (!ar.IsCompleted)
                    yield return null;

                try
                {
                    SocketError error;
                    length -= socket.EndSend(ar, out error);
                    Error = error;

                    if (Error != SocketError.Success)
                    {
                        OnCompleted(false, Exception, Error);
                        yield break;
                    }
                }
                catch (Exception ex)
                {
                    Exception = ex;
                    OnCompleted(false, Exception, Error);
                    yield break;
                }
            }
            OnCompleted(true, Exception, Error);
        }

        #endregion

        private void OnCompleted(bool success, Exception exception, SocketError error)
        {
            var handler = Completed;
            if (handler != null) handler(success, exception, error);
        }
    }
}