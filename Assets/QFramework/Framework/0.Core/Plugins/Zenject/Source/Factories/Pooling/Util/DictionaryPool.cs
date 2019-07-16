using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class DictionaryPool<TKey, TValue> : StaticMemoryPool<Dictionary<TKey, TValue>>
    {
        static DictionaryPool<TKey, TValue> _instance = new DictionaryPool<TKey, TValue>();

        public DictionaryPool()
        {
            OnSpawnMethod = OnSpawned;
            OnDespawnedMethod = OnDespawned;
        }

        public static DictionaryPool<TKey, TValue> Instance
        {
            get { return _instance; }
        }

        static void OnSpawned(Dictionary<TKey, TValue> items)
        {
            Assert.That(items.IsEmpty());
        }

        static void OnDespawned(Dictionary<TKey, TValue> items)
        {
            items.Clear();
        }
    }
}

