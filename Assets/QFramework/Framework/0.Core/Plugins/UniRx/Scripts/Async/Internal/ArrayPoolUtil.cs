﻿#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UniRx.Async.Internal
{
    public static class ArrayPoolUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureCapacity<T>(ref T[] array, int index, ArrayPool<T> pool)
        {
            if (array.Length <= index)
            {
                EnsureCapacityCore(ref array, index, pool);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void EnsureCapacityCore<T>(ref T[] array, int index, ArrayPool<T> pool)
        {
            if (array.Length <= index)
            {
                var newSize = array.Length * 2;
                var newArray = pool.Rent((index < newSize) ? newSize : (index * 2));
                Array.Copy(array, 0, newArray, 0, array.Length);

                pool.Return(array, clearArray: !RuntimeHelpersAbstraction.IsWellKnownNoReferenceContainsType<T>());

                array = newArray;
            }
        }

        public static RentArray<T> Materialize<T>(IEnumerable<T> source)
        {
            if (source is T[] array)
            {
                return new RentArray<T>(array, array.Length, null);
            }

            var defaultCount = 4;
            if (source is ICollection<T> coll)
            {
                defaultCount = coll.Count;
                var pool = ArrayPool<T>.Shared;
                var buffer = pool.Rent(defaultCount);
                coll.CopyTo(buffer, 0);
                return new RentArray<T>(buffer, coll.Count, pool);
            }
            else if (source is IReadOnlyCollection<T> rcoll)
            {
                defaultCount = rcoll.Count;
            }

            if (defaultCount == 0)
            {
                return new RentArray<T>(Array.Empty<T>(), 0, null);
            }

            {
                var pool = ArrayPool<T>.Shared;

                var index = 0;
                var buffer = pool.Rent(defaultCount);
                foreach (var item in source)
                {
                    EnsureCapacity(ref buffer, index, pool);
                    buffer[index++] = item;
                }

                return new RentArray<T>(buffer, index, pool);
            }
        }

        public struct RentArray<T> : IDisposable
        {
            public readonly T[] Array;
            public readonly int Length;
            ArrayPool<T> pool;

            public RentArray(T[] array, int length, ArrayPool<T> pool)
            {
                this.Array = array;
                this.Length = length;
                this.pool = pool;
            }

            public void Dispose()
            {
                DisposeManually(!RuntimeHelpersAbstraction.IsWellKnownNoReferenceContainsType<T>());
            }

            public void DisposeManually(bool clearArray)
            {
                if (pool != null)
                {
                    if (clearArray)
                    {
                        System.Array.Clear(Array, 0, Length);
                    }

                    pool.Return(Array, clearArray: false);
                    pool = null;
                }
            }
        }
    }
}

#endif