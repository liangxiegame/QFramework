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
    
    public static partial class Observable
    {
        public static IConnectableObservable<T> Multicast<T>(this IObservable<T> source, ISubject<T> subject)
        {
            return new ConnectableObservable<T>(source, subject);
        }

        public static IConnectableObservable<T> Publish<T>(this IObservable<T> source)
        {
            return source.Multicast(new Subject<T>());
        }

        public static IConnectableObservable<T> Publish<T>(this IObservable<T> source, T initialValue)
        {
            return source.Multicast(new BehaviorSubject<T>(initialValue));
        }

        public static IConnectableObservable<T> PublishLast<T>(this IObservable<T> source)
        {
            return source.Multicast(new AsyncSubject<T>());
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source)
        {
            return source.Multicast(new ReplaySubject<T>());
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(scheduler));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, int bufferSize)
        {
            return source.Multicast(new ReplaySubject<T>(bufferSize));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, int bufferSize, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(bufferSize, scheduler));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, TimeSpan window)
        {
            return source.Multicast(new ReplaySubject<T>(window));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, TimeSpan window, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(window, scheduler));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, int bufferSize, TimeSpan window, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(bufferSize, window, scheduler));
        }

        public static IObservable<T> RefCount<T>(this IConnectableObservable<T> source)
        {
            return new RefCountObservable<T>(source);
        }

        /// <summary>
        /// same as Publish().RefCount()
        /// </summary>
        public static IObservable<T> Share<T>(this IObservable<T> source)
        {
            return source.Publish().RefCount();
        }
    }
}