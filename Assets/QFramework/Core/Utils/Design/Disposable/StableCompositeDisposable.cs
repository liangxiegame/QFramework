/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    
    /// <summary>
    /// Represents a group of disposable resources that are disposed together.
    /// </summary>
    public abstract class StableCompositeDisposable : ICancelable
    {
        /// <summary>
        /// Creates a new group containing two disposable resources that are disposed together.
        /// </summary>
        /// <param name="disposable1">The first disposable resoruce to add to the group.</param>
        /// <param name="disposable2">The second disposable resoruce to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable Create(IDisposable disposable1, IDisposable disposable2)
        {
            if (disposable1 == null) throw new ArgumentNullException("disposable1");
            if (disposable2 == null) throw new ArgumentNullException("disposable2");

            return new Binary(disposable1, disposable2);
        }

        /// <summary>
        /// Creates a new group containing three disposable resources that are disposed together.
        /// </summary>
        /// <param name="disposable1">The first disposable resoruce to add to the group.</param>
        /// <param name="disposable2">The second disposable resoruce to add to the group.</param>
        /// <param name="disposable3">The third disposable resoruce to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable Create(IDisposable disposable1, IDisposable disposable2, IDisposable disposable3)
        {
            if (disposable1 == null) throw new ArgumentNullException("disposable1");
            if (disposable2 == null) throw new ArgumentNullException("disposable2");
            if (disposable3 == null) throw new ArgumentNullException("disposable3");

            return new Trinary(disposable1, disposable2, disposable3);
        }

        /// <summary>
        /// Creates a new group containing four disposable resources that are disposed together.
        /// </summary>
        /// <param name="disposable1">The first disposable resoruce to add to the group.</param>
        /// <param name="disposable2">The second disposable resoruce to add to the group.</param>
        /// <param name="disposable3">The three disposable resoruce to add to the group.</param>
        /// <param name="disposable4">The four disposable resoruce to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable Create(IDisposable disposable1, IDisposable disposable2, IDisposable disposable3, IDisposable disposable4)
        {
            if (disposable1 == null) throw new ArgumentNullException("disposable1");
            if (disposable2 == null) throw new ArgumentNullException("disposable2");
            if (disposable3 == null) throw new ArgumentNullException("disposable3");
            if (disposable4 == null) throw new ArgumentNullException("disposable4");

            return new Quaternary(disposable1, disposable2, disposable3, disposable4);
        }

        /// <summary>
        /// Creates a new group of disposable resources that are disposed together.
        /// </summary>
        /// <param name="disposables">Disposable resources to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable Create(params IDisposable[] disposables)
        {
            if (disposables == null) throw new ArgumentNullException("disposables");

            return new NAry(disposables);
        }

        /// <summary>
        /// Creates a new group of disposable resources that are disposed together. Array is not copied, it's unsafe but optimized.
        /// </summary>
        /// <param name="disposables">Disposable resources to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable CreateUnsafe(IDisposable[] disposables)
        {
            return new NAryUnsafe(disposables);
        }

        /// <summary>
        /// Creates a new group of disposable resources that are disposed together.
        /// </summary>
        /// <param name="disposables">Disposable resources to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable Create(IEnumerable<IDisposable> disposables)
        {
            if (disposables == null) throw new ArgumentNullException("disposables");

            return new NAry(disposables);
        }

        /// <summary>
        /// Disposes all disposables in the group.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public abstract bool IsDisposed
        {
            get;
        }

        private class Binary : StableCompositeDisposable
        {
            int mDisposedCallCount = -1;
            private volatile IDisposable mDisposable1;
            private volatile IDisposable mDisposable2;

            public Binary(IDisposable disposable1, IDisposable disposable2)
            {
                mDisposable1 = disposable1;
                mDisposable2 = disposable2;
            }

            public override bool IsDisposed
            {
                get
                {
                    return mDisposedCallCount != -1;
                }
            }

            public override void Dispose()
            {
                if (Interlocked.Increment(ref mDisposedCallCount) == 0)
                {
                    mDisposable1.Dispose();
                    mDisposable2.Dispose();
                }
            }
        }

        private class Trinary : StableCompositeDisposable
        {
            int mDisposedCallCount = -1;
            private volatile IDisposable mDisposable1;
            private volatile IDisposable mDisposable2;
            private volatile IDisposable mDisposable3;

            public Trinary(IDisposable disposable1, IDisposable disposable2, IDisposable disposable3)
            {
                mDisposable1 = disposable1;
                mDisposable2 = disposable2;
                mDisposable3 = disposable3;
            }

            public override bool IsDisposed
            {
                get
                {
                    return mDisposedCallCount != -1;
                }
            }

            public override void Dispose()
            {
                if (Interlocked.Increment(ref mDisposedCallCount) == 0)
                {
                    mDisposable1.Dispose();
                    mDisposable2.Dispose();
                    mDisposable3.Dispose();
                }
            }
        }

        private class Quaternary : StableCompositeDisposable
        {
            int mDisposedCallCount = -1;
            private volatile IDisposable mDisposable1;
            private volatile IDisposable mDisposable2;
            private volatile IDisposable mDisposable3;
            private volatile IDisposable mDisposable4;

            public Quaternary(IDisposable disposable1, IDisposable disposable2, IDisposable disposable3, IDisposable disposable4)
            {
                mDisposable1 = disposable1;
                mDisposable2 = disposable2;
                mDisposable3 = disposable3;
                mDisposable4 = disposable4;
            }

            public override bool IsDisposed
            {
                get
                {
                    return mDisposedCallCount != -1;
                }
            }

            public override void Dispose()
            {
                if (Interlocked.Increment(ref mDisposedCallCount) == 0)
                {
                    mDisposable1.Dispose();
                    mDisposable2.Dispose();
                    mDisposable3.Dispose();
                    mDisposable4.Dispose();
                }
            }
        }

        private class NAry : StableCompositeDisposable
        {
            int mDisposedCallCount = -1;
            private volatile List<IDisposable> mDisposables;

            public NAry(IDisposable[] disposables)
                : this((IEnumerable<IDisposable>)disposables)
            {
            }

            public NAry(IEnumerable<IDisposable> disposables)
            {
                mDisposables = new List<IDisposable>(disposables);

                //
                // Doing this on the list to avoid duplicate enumeration of disposables.
                //
                if (mDisposables.Contains(null)) throw new ArgumentException("Disposables can't contains null", "disposables");
            }

            public override bool IsDisposed
            {
                get
                {
                    return mDisposedCallCount != -1;
                }
            }

            public override void Dispose()
            {
                if (Interlocked.Increment(ref mDisposedCallCount) == 0)
                {
                    foreach (var d in mDisposables)
                    {
                        d.Dispose();
                    }
                }
            }
        }

        private class NAryUnsafe : StableCompositeDisposable
        {
            int mDisposedCallCount = -1;
            private volatile IDisposable[] mDisposables;

            public NAryUnsafe(IDisposable[] disposables)
            {
                mDisposables = disposables;
            }

            public override bool IsDisposed
            {
                get
                {
                    return mDisposedCallCount != -1;
                }
            }

            public override void Dispose()
            {
                if (Interlocked.Increment(ref mDisposedCallCount) == 0)
                {
                    var len = mDisposables.Length;
                    for (var i = 0; i < len; i++)
                    {
                        mDisposables[i].Dispose();
                    }
                }
            }
        }
    }
}