namespace Zenject
{
    public class PoolableStaticMemoryPool<TValue> : StaticMemoryPool<TValue>
        where TValue : class, IPoolable, new()
    {
        public PoolableStaticMemoryPool()
            : base(OnSpawned, OnDespawned)
        {
        }

        static void OnSpawned(TValue value)
        {
            value.OnSpawned();
        }

        static void OnDespawned(TValue value)
        {
            value.OnDespawned();
        }
    }

    public class PoolableStaticMemoryPool<TParam1, TValue> : StaticMemoryPool<TParam1, TValue>
        where TValue : class, IPoolable<TParam1>, new()
    {
        public PoolableStaticMemoryPool()
            : base(OnSpawned, OnDespawned)
        {
        }

        static void OnSpawned(TParam1 p1, TValue value)
        {
            value.OnSpawned(p1);
        }

        static void OnDespawned(TValue value)
        {
            value.OnDespawned();
        }
    }

    public class PoolableStaticMemoryPool<TParam1, TParam2, TValue> : StaticMemoryPool<TParam1, TParam2, TValue>
        where TValue : class, IPoolable<TParam1, TParam2>, new()
    {
        public PoolableStaticMemoryPool()
            : base(OnSpawned, OnDespawned)
        {
        }

        static void OnSpawned(TParam1 p1, TParam2 p2, TValue value)
        {
            value.OnSpawned(p1, p2);
        }

        static void OnDespawned(TValue value)
        {
            value.OnDespawned();
        }
    }

    public class PoolableStaticMemoryPool<TParam1, TParam2, TParam3, TValue> : StaticMemoryPool<TParam1, TParam2, TParam3, TValue>
        where TValue : class, IPoolable<TParam1, TParam2, TParam3>, new()
    {
        public PoolableStaticMemoryPool()
            : base(OnSpawned, OnDespawned)
        {
        }

        static void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TValue value)
        {
            value.OnSpawned(p1, p2, p3);
        }

        static void OnDespawned(TValue value)
        {
            value.OnDespawned();
        }
    }

    public class PoolableStaticMemoryPool<TParam1, TParam2, TParam3, TParam4, TValue> : StaticMemoryPool<TParam1, TParam2, TParam3, TParam4, TValue>
        where TValue : class, IPoolable<TParam1, TParam2, TParam3, TParam4>, new()
    {
        public PoolableStaticMemoryPool()
            : base(OnSpawned, OnDespawned)
        {
        }

        static void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TValue value)
        {
            value.OnSpawned(p1, p2, p3, p4);
        }

        static void OnDespawned(TValue value)
        {
            value.OnDespawned();
        }
    }

    public class PoolableStaticMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TValue> : StaticMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
        where TValue : class, IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5>, new()
    {
        public PoolableStaticMemoryPool()
            : base(OnSpawned, OnDespawned)
        {
        }

        static void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TValue value)
        {
            value.OnSpawned(p1, p2, p3, p4, p5);
        }

        static void OnDespawned(TValue value)
        {
            value.OnDespawned();
        }
    }

    public class PoolableStaticMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue> : StaticMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue>
        where TValue : class, IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>, new()
    {
        public PoolableStaticMemoryPool()
            : base(OnSpawned, OnDespawned)
        {
        }

        static void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TValue value)
        {
            value.OnSpawned(p1, p2, p3, p4, p5, p6);
        }

        static void OnDespawned(TValue value)
        {
            value.OnDespawned();
        }
    }

    public class PoolableStaticMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TValue> : StaticMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TValue>
        where TValue : class, IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>, new()
    {
        public PoolableStaticMemoryPool()
            : base(OnSpawned, OnDespawned)
        {
        }

        static void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7, TValue value)
        {
            value.OnSpawned(p1, p2, p3, p4, p5, p6, p7);
        }

        static void OnDespawned(TValue value)
        {
            value.OnDespawned();
        }
    }
}
