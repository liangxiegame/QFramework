using System.Collections.Generic;

namespace QFramework
{
    public static class ListPool<T>
    {
        static Stack<List<T>> mListStack = new Stack<List<T>>(8);

        public static List<T> Get()
        {
            if (mListStack.Count == 0)
            {
                return new List<T>(8);
            }

            return mListStack.Pop();
        }

        public static void Release(List<T> toRelease)
        {
            toRelease.Clear();
            mListStack.Push(toRelease);
        }
    }

    public static class ListPoolExtensions
    {
        public static void Release2Pool<T>(this List<T> toRelease)
        {
            ListPool<T>.Release(toRelease);
        }
    }
}