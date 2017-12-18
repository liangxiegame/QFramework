using System;

namespace QFramework
{
    public static partial class Observable
    {
        public static T Wait<T>(this IObservable<T> source)
        {
            return new QFramework.Wait<T>(source, InfiniteTimeSpan).Run();
        }

        public static T Wait<T>(this IObservable<T> source, TimeSpan timeout)
        {
            return new QFramework.Wait<T>(source, timeout).Run();
        }
    }
}
