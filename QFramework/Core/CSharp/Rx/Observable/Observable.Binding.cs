using System;

namespace QFramework.Core.Rx
{
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

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, Utils.Scheduler.IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(scheduler));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, int bufferSize)
        {
            return source.Multicast(new ReplaySubject<T>(bufferSize));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, int bufferSize, Utils.Scheduler.IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(bufferSize, scheduler));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, TimeSpan window)
        {
            return source.Multicast(new ReplaySubject<T>(window));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, TimeSpan window, Utils.Scheduler.IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(window, scheduler));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, int bufferSize, TimeSpan window, Utils.Scheduler.IScheduler scheduler)
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