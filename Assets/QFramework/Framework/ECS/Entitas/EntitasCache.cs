using System.Collections.Generic;



namespace QFramework
{
    public static class EntitasCache
    {
        static readonly SimpleObjectCache mCache = new SimpleObjectCache();

        public static List<IComponent> GetIComponentList()
        {
            return mCache.Get<List<IComponent>>();
        }

        public static void PushIComponentList(List<IComponent> list)
        {
            list.Clear();
            mCache.Push(list);
        }

        public static List<int> GetIntList()
        {
            return mCache.Get<List<int>>();
        }

        public static void PushIntList(List<int> list)
        {
            list.Clear();
            mCache.Push(list);
        }

        public static HashSet<int> GetIntHashSet()
        {
            return mCache.Get<HashSet<int>>();
        }

        public static void PushIntHashSet(HashSet<int> hashSet)
        {
            hashSet.Clear();
            mCache.Push(hashSet);
        }

        public static void Reset()
        {
            mCache.Reset();
        }
    }
}