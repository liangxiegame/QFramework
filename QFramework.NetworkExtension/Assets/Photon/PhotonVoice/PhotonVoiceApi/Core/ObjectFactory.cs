using System;

namespace Photon.Voice
{
    /// <summary>
    /// Uniform interface to <see cref="ObjectPool{TType, TInfo}"/> and single reusable object.
    /// </summary>
    /// <typeparam name="TType">Object type.</typeparam>
    /// <typeparam name="TInfo">Type of property used to check 2 objects identity (like integral length of array).</typeparam>
    public interface ObjectFactory<TType, TInfo> : IDisposable
    {
        TInfo Info { get; }
        TType New();
        TType New(TInfo info);
        void Free(TType obj);
        void Free(TType obj, TInfo info);
    }

    // Object factory implementation skipped, we use only arrays for now

    /// <summary>
    /// Array factory returnig the same array instance as long as it requested with the same array length. If length changes, new array instance created.
    /// </summary>
    /// <typeparam name="T">Array element type.</typeparam>
    public class FactoryReusableArray<T> : ObjectFactory<T[], int>
    {
        T[] arr;
        public FactoryReusableArray(int size)
        {
            this.arr = new T[size];
        }

        public int Info { get { return arr.Length; } }

        public T[] New()
        {
            return arr;
        }

        public T[] New(int size)
        {
            if (arr.Length != size)
            {
                arr = new T[size];
            }
            return arr;
        }

        public void Free(T[] obj)
        {
        }

        public void Free(T[] obj, int info)
        {
        }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// <see cref="PrimitiveArrayPool{T}"/> as wrapped in object factory interface.
    /// </summary>
    /// <typeparam name="T">Array element type.</typeparam>
    public class FactoryPrimitiveArrayPool<T> : ObjectFactory<T[], int>
    {
        PrimitiveArrayPool<T> pool;
        public FactoryPrimitiveArrayPool(int capacity, string name)
        {
            pool = new PrimitiveArrayPool<T>(capacity, name);
        }

        public FactoryPrimitiveArrayPool(int capacity, string name, int info)
        {
            pool = new PrimitiveArrayPool<T>(capacity, name, info);
        }

        public int Info { get { return pool.Info; } }

        public T[] New()
        {
            return pool.AcquireOrCreate();
        }

        public T[] New(int size)
        {
            return pool.AcquireOrCreate(size);
        }

        public void Free(T[] obj)
        {
            pool.Release(obj);
        }

        public void Free(T[] obj, int info)
        {
            pool.Release(obj, info);
        }

        public void Dispose()
        {
            pool.Dispose();
        }
    }
}
