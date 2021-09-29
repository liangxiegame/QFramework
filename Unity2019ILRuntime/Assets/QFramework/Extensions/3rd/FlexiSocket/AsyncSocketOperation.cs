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
    /// Async socket operation
    /// </summary>
    public abstract class AsyncSocketOperation : IEnumerator, IDisposable
    {
        private IEnumerator _handler;

        /// <summary>
        /// Target socket
        /// </summary>
        protected internal readonly Socket socket;

        /// <summary>
        /// Async result
        /// </summary>
        protected internal IAsyncResult ar;

        /// <summary>
        /// Is this operation has completed(either successful or failed)
        /// </summary>
        public bool IsCompleted
        {
            get { return !((IEnumerator) this).MoveNext(); }
        }

        /// <summary>
        /// Exception
        /// </summary>
        public Exception Exception { get; protected set; }

        /// <summary>
        /// If this operation has completed successfully
        /// </summary>
        public abstract bool IsSuccessful { get; }

        protected internal AsyncSocketOperation(Socket socket)
        {
            this.socket = socket;
        }

        #region Implementation of IEnumerator

        bool IEnumerator.MoveNext()
        {
            if (_handler == null)
                _handler = GetEnumerator();
            return _handler.MoveNext();
        }

        void IEnumerator.Reset()
        {
            if (_handler == null)
                _handler = GetEnumerator();
            _handler.Reset();
        }

        object IEnumerator.Current
        {
            get
            {
                if (_handler == null)
                    _handler = GetEnumerator();
                return _handler.Current;
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Dispose the async operation
        /// </summary>
        public abstract void Dispose();

        #endregion

        protected abstract IEnumerator GetEnumerator();
    }
}