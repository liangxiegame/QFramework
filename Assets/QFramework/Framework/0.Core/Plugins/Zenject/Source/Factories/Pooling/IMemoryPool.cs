using System;

namespace Zenject
{
    public interface IMemoryPool
    {
        int NumTotal { get; }
        int NumActive { get; }
        int NumInactive { get; }

        Type ItemType
        {
            get;
        }

        /// <summary>
        /// Changes pool size by creating new elements or destroying existing elements
        /// This bypasses the configured expansion method (OneAtATime or Doubling)
        /// </summary>
        void Resize(int desiredPoolSize);

        void Clear();

        /// <summary>
        /// Expands the pool by the additional size.
        /// This bypasses the configured expansion method (OneAtATime or Doubling)
        /// </summary>
        /// <param name="numToAdd">The additional number of items to allocate in the pool</param>
        void ExpandBy(int numToAdd);

        /// <summary>
        /// Shrinks the MemoryPool by removing a given number of elements
        /// This bypasses the configured expansion method (OneAtATime or Doubling)
        /// </summary>
        /// <param name="numToRemove">The amount of items to remove from the pool</param>
        void ShrinkBy(int numToRemove);

        void Despawn(object obj);
    }

    public interface IDespawnableMemoryPool<TValue> : IMemoryPool
    {
        void Despawn(TValue item);
    }

    public interface IMemoryPool<TValue> : IDespawnableMemoryPool<TValue>
    {
        TValue Spawn();
    }

    public interface IMemoryPool<in TParam1, TValue> : IDespawnableMemoryPool<TValue>
    {
        TValue Spawn(TParam1 param);
    }

    public interface IMemoryPool<in TParam1, in TParam2, TValue> : IDespawnableMemoryPool<TValue>
    {
        TValue Spawn(TParam1 param1, TParam2 param2);
    }

    public interface IMemoryPool<in TParam1, in TParam2, in TParam3, TValue> : IDespawnableMemoryPool<TValue>
    {
        TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    public interface IMemoryPool<in TParam1, in TParam2, in TParam3, in TParam4, TValue> : IDespawnableMemoryPool<TValue>
    {
        TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }

    public interface IMemoryPool<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, TValue> : IDespawnableMemoryPool<TValue>
    {
        TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5);
    }

    public interface IMemoryPool<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, TValue> : IDespawnableMemoryPool<TValue>
    {
        TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6);
    }

    public interface IMemoryPool<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, TValue> : IDespawnableMemoryPool<TValue>
    {
        TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7);
    }

    public interface IMemoryPool<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, in TParam8, TValue> : IDespawnableMemoryPool<TValue>
    {
        TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8);
    }
}
