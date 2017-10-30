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
        public static Func<IObservable<TResult>> FromAsyncPattern<TResult>(Func<AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
        {
            return () =>
            {
                var subject = new AsyncSubject<TResult>();
                try
                {
                    begin(iar =>
                    {
                        TResult result;
                        try
                        {
                            result = end(iar);
                        }
                        catch (Exception exception)
                        {
                            subject.OnError(exception);
                            return;
                        }
                        subject.OnNext(result);
                        subject.OnCompleted();
                    }, null);
                }
                catch (Exception exception)
                {
                    return Observable.Throw<TResult>(exception,Scheduler.DefaultSchedulers.AsyncConversions);
                }
                return subject.AsObservable();
            };
        }

        public static Func<T1, IObservable<TResult>> FromAsyncPattern<T1, TResult>(Func<T1, AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
        {
            return x =>
            {
                var subject = new AsyncSubject<TResult>();
                try
                {
                    begin(x, iar =>
                    {
                        TResult result;
                        try
                        {
                            result = end(iar);
                        }
                        catch (Exception exception)
                        {
                            subject.OnError(exception);
                            return;
                        }
                        subject.OnNext(result);
                        subject.OnCompleted();
                    }, null);
                }
                catch (Exception exception)
                {
                    return Observable.Throw<TResult>(exception,Scheduler.DefaultSchedulers.AsyncConversions);
                }
                return subject.AsObservable();
            };
        }

        public static Func<T1, T2, IObservable<TResult>> FromAsyncPattern<T1, T2, TResult>(Func<T1, T2, AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
        {
            return (x, y) =>
            {
                var subject = new AsyncSubject<TResult>();
                try
                {
                    begin(x, y, iar =>
                    {
                        TResult result;
                        try
                        {
                            result = end(iar);
                        }
                        catch (Exception exception)
                        {
                            subject.OnError(exception);
                            return;
                        }
                        subject.OnNext(result);
                        subject.OnCompleted();
                    }, null);
                }
                catch (Exception exception)
                {
                    return Observable.Throw<TResult>(exception,Scheduler.DefaultSchedulers.AsyncConversions);
                }
                return subject.AsObservable();
            };
        }

        public static Func<IObservable<Unit>> FromAsyncPattern(Func<AsyncCallback, object, IAsyncResult> begin, Action<IAsyncResult> end)
        {
            return FromAsyncPattern(begin, iar =>
            {
                end(iar);
                return Unit.Default;
            });
        }

        public static Func<T1, IObservable<Unit>> FromAsyncPattern<T1>(Func<T1, AsyncCallback, object, IAsyncResult> begin, Action<IAsyncResult> end)
        {
            return FromAsyncPattern(begin, iar =>
            {
                end(iar);
                return Unit.Default;
            });
        }

        public static Func<T1, T2, IObservable<Unit>> FromAsyncPattern<T1, T2>(Func<T1, T2, AsyncCallback, object, IAsyncResult> begin, Action<IAsyncResult> end)
        {
            return FromAsyncPattern(begin, iar =>
            {
                end(iar);
                return Unit.Default;
            });
        }
    }
}