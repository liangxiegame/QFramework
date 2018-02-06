using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public static partial class GameObjectExtensions
    {
        /// <summary>Returns a collection of GameObjects that contains the ancestors of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> Ancestors(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                var e = item.Ancestors().GetEnumerator();
                while (e.MoveNext())
                {
                    yield return e.Current;
                }
            }
        }

        /// <summary>Returns a collection of GameObjects that contains every GameObject in the source collection, and the ancestors of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> AncestorsAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                var e = item.AncestorsAndSelf().GetEnumerator();
                while (e.MoveNext())
                {
                    yield return e.Current;
                }
            }
        }

        /// <summary>Returns a collection of GameObjects that contains the descendant GameObjects of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> Descendants(this IEnumerable<GameObject> source, Func<Transform, bool> descendIntoChildren = null)
        {
            foreach (var item in source)
            {
                var e = item.Descendants(descendIntoChildren).GetEnumerator();
                while (e.MoveNext())
                {
                    yield return e.Current;
                }
            }
        }

        /// <summary>Returns a collection of GameObjects that contains every GameObject in the source collection, and the descendent GameObjects of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> DescendantsAndSelf(this IEnumerable<GameObject> source, Func<Transform, bool> descendIntoChildren = null)
        {
            foreach (var item in source)
            {
                var e = item.DescendantsAndSelf(descendIntoChildren).GetEnumerator();
                while (e.MoveNext())
                {
                    yield return e.Current;
                }
            }
        }

        /// <summary>Returns a collection of the child GameObjects of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> Children(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                var e = item.Children().GetEnumerator();
                while (e.MoveNext())
                {
                    yield return e.Current;
                }
            }
        }

        /// <summary>Returns a collection of GameObjects that contains every GameObject in the source collection, and the child GameObjects of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> ChildrenAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                var e = item.ChildrenAndSelf().GetEnumerator();
                while (e.MoveNext())
                {
                    yield return e.Current;
                }
            }
        }

        /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
        /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
        /// <param name="detachParent">set to parent = null.</param>
        public static void Destroy(this IEnumerable<GameObject> source, bool useDestroyImmediate = false, bool detachParent = false)
        {
            if (detachParent)
            {
                var l = new List<GameObject>(source); // avoid halloween problem
                var e = l.GetEnumerator(); // get struct enumerator for avoid unity's compiler bug(avoid boxing)
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, true);
                }
            }
            else
            {
                foreach (var item in source)
                {
                    item.Destroy(useDestroyImmediate, false); // doesn't detach.
                }
            }
        }

        /// <summary>Returns a collection of specified component in the source collection.</summary>
        public static IEnumerable<T> OfComponent<T>(this IEnumerable<GameObject> source)
            where T : UnityEngine.Component
        {
            foreach (var item in source)
            {
#if UNITY_EDITOR
                var cache = ComponentCache<T>.Instance;
                item.GetComponents<T>(cache);
                if (cache.Count != 0)
                {
                    var component = cache[0];
                    cache.Clear();
                    yield return component;
                }
#else
                        
                var component = item.GetComponent<T>();
                if (component != null)
                {
                    yield return component;
                }
#endif
            }
        }


#if UNITY_EDITOR
        class ComponentCache<T>
        {
            public static readonly List<T> Instance = new List<T>(); // for no allocate on UNITY_EDITOR
        }
#endif

        /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
        public static int ToArrayNonAlloc<T>(this IEnumerable<T> source, ref T[] array)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (array.Length == index)
                {
                    var newSize = (index == 0) ? 4 : index * 2;
                    Array.Resize(ref array, newSize);
                }
                array[index++] = item;
            }

            return index;
        }
    }
}