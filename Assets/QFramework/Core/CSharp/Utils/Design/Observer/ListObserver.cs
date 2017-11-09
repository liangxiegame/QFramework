/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

    public class ListObserver<T> : IObserver<T>
    {
        private readonly ImmutableList<IObserver<T>> mObservers;

        public ListObserver(ImmutableList<IObserver<T>> observers)
        {
            mObservers = observers;
        }

        public void OnCompleted()
        {
            mObservers.Data.ForEach(observer => observer.OnCompleted());
        }

        public void OnError(Exception error)
        {
            mObservers.Data.ForEach(observer => observer.OnError(error));
        }

        public void OnNext(T value)
        {
            mObservers.Data.ForEach(observer => observer.OnNext(value));
        }

        public IObserver<T> Add(IObserver<T> observer)
        {
            return new ListObserver<T>(mObservers.Add(observer));
        }

        public IObserver<T> Remove(IObserver<T> observer)
        {
            var i = Array.IndexOf(mObservers.Data, observer);
            if (i < 0)
                return this;

            if (mObservers.Data.Length == 2)
            {
                return mObservers.Data[1 - i];
            }
            else
            {
                return new ListObserver<T>(mObservers.Remove(observer));
            }
        }
    }

    public class EmptyObserver<T> : IObserver<T>
    {
        public static readonly EmptyObserver<T> Instance = new EmptyObserver<T>();

        EmptyObserver()
        {

        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
        }
    }

    public class ThrowObserver<T> : IObserver<T>
    {
        public static readonly ThrowObserver<T> Instance = new ThrowObserver<T>();

        ThrowObserver()
        {

        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(T value)
        {
        }
    }

    public class DisposedObserver<T> : IObserver<T>
    {
        public static readonly DisposedObserver<T> Instance = new DisposedObserver<T>();

        DisposedObserver()
        {

        }

        public void OnCompleted()
        {
            throw new ObjectDisposedException("");
        }

        public void OnError(Exception error)
        {
            throw new ObjectDisposedException("");
        }

        public void OnNext(T value)
        {
            throw new ObjectDisposedException("");
        }
    }
}