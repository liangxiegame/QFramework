using System.Collections.Generic;

namespace QFramework
{
    public class DictionaryPool<TKey, TValue>
    {
        static Stack<Dictionary<TKey, TValue>> mListStack = new Stack<Dictionary<TKey, TValue>>(8);

        public static Dictionary<TKey, TValue> Get()
        {
            if (mListStack.Count == 0)
            {
                return new Dictionary<TKey, TValue>(8);
            }

            return mListStack.Pop();
        }

        public static void Release(Dictionary<TKey, TValue> toRelease)
        {
            toRelease.Clear();
            mListStack.Push(toRelease);
        }
    }
    
    public static class DictionaryPoolExtensions
    {
        public static void Release2Pool<TKey,TValue>(this Dictionary<TKey, TValue> toRelease)
        {
            DictionaryPool<TKey,TValue>.Release(toRelease);
        }
    }
}