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
    /// Async disconnect operation
    /// </summary>
    public sealed class AsyncDisconnect : AsyncSocketOperation
    {
        /// <summary>
        /// Disconnected callback
        /// </summary>
        public event DisconnectedCallback Completed;

        public AsyncDisconnect(Socket socket) : base(socket)
        {
        }

        #region Overrides of AsyncSocketOperation

        public override bool IsSuccessful
        {
            get { return IsCompleted && Exception == null; }
        }

        public override void Dispose()
        {
            try
            {
                if (ar != null && !ar.IsCompleted)
                    socket.EndDisconnect(ar);
            }
            catch
            {
                // ignored
            }
        }

        protected override IEnumerator GetEnumerator()
        {
            try
            {
                ar = socket.BeginDisconnect(false, null, null);
            }
            catch (Exception ex)
            {
                Exception = ex;
                OnCompleted(false, Exception);
                yield break;
            }

            while (!ar.IsCompleted)
            {
                yield return null;
            }

            try
            {
                socket.EndDisconnect(ar);
            }
            catch (Exception ex)
            {
                Exception = ex;
                OnCompleted(false, Exception);
                yield break;
            }

            OnCompleted(true, Exception);
        }

        #endregion

        private void OnCompleted(bool success, Exception exception)
        {
            var handler = Completed;
            if (handler != null) handler(success, exception);
        }
    }
}