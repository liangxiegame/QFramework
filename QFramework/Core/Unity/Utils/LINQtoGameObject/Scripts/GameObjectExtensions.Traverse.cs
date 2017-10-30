using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
    // API Frontend

    public static partial class GameObjectExtensions
    {
        // Traverse Game Objects, based on Axis(Parent, Child, Children, Ancestors/Descendants, BeforeSelf/ObjectsBeforeAfter)

        /// <summary>Gets the parent GameObject of this GameObject. If this GameObject has no parent, returns null.</summary>
        public static GameObject Parent(this GameObject origin)
        {
            if (origin == null) return null;

            var parentTransform = origin.transform.parent;
            if (parentTransform == null) return null;

            return parentTransform.gameObject;
        }

        /// <summary>Gets the first child GameObject with the specified name. If there is no GameObject with the speficided name, returns null.</summary>
        public static GameObject Child(this GameObject origin, string name)
        {
            if (origin == null) return null;

            var child = origin.transform.FindChild(name); // transform.find can get inactive object
            if (child == null) return null;
            return child.gameObject;
        }

        /// <summary>Returns a collection of the child GameObjects.</summary>
        public static ChildrenEnumerable Children(this GameObject origin)
        {
            return new ChildrenEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the child GameObjects.</summary>
        public static ChildrenEnumerable ChildrenAndSelf(this GameObject origin)
        {
            return new ChildrenEnumerable(origin, true);
        }

        /// <summary>Returns a collection of the ancestor GameObjects of this GameObject.</summary>
        public static AncestorsEnumerable Ancestors(this GameObject origin)
        {
            return new AncestorsEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this element, and the ancestors of this GameObject.</summary>
        public static AncestorsEnumerable AncestorsAndSelf(this GameObject origin)
        {
            return new AncestorsEnumerable(origin, true);
        }

        /// <summary>Returns a collection of the descendant GameObjects.</summary>
        public static DescendantsEnumerable Descendants(this GameObject origin, Func<Transform, bool> descendIntoChildren = null)
        {
            return new DescendantsEnumerable(origin, false, descendIntoChildren);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and all descendant GameObjects of this GameObject.</summary>
        public static DescendantsEnumerable DescendantsAndSelf(this GameObject origin, Func<Transform, bool> descendIntoChildren = null)
        {
            return new DescendantsEnumerable(origin, true, descendIntoChildren);
        }

        /// <summary>Returns a collection of the sibling GameObjects before this GameObject.</summary>
        public static BeforeSelfEnumerable BeforeSelf(this GameObject origin)
        {
            return new BeforeSelfEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects before this GameObject.</summary>
        public static BeforeSelfEnumerable BeforeSelfAndSelf(this GameObject origin)
        {
            return new BeforeSelfEnumerable(origin, true);
        }

        /// <summary>Returns a collection of the sibling GameObjects after this GameObject.</summary>
        public static AfterSelfEnumerable AfterSelf(this GameObject origin)
        {
            return new AfterSelfEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects after this GameObject.</summary>
        public static AfterSelfEnumerable AfterSelfAndSelf(this GameObject origin)
        {
            return new AfterSelfEnumerable(origin, true);
        }

        // Implements hand struct enumerator.

        public struct ChildrenEnumerable : IEnumerable<GameObject>
        {
            readonly GameObject origin;
            readonly bool withSelf;

            public ChildrenEnumerable(GameObject origin, bool withSelf)
            {
                this.origin = origin;
                this.withSelf = withSelf;
            }

            /// <summary>Returns a collection of specified component in the source collection.</summary>
            public OfComponentEnumerable<T> OfComponent<T>()
                where T : Component
            {
                return new OfComponentEnumerable<T>(ref this);
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            /// <param name="detachParent">set to parent = null.</param>
            public void Destroy(bool useDestroyImmediate = false, bool detachParent = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, false);
                }
                if (detachParent)
                {
                    origin.transform.DetachChildren();
                    if (withSelf)
                    {
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)
                        origin.transform.SetParent(null);
#else
                        origin.transform.parent = null;
#endif
                    }
                }
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (predicate(item))
                    {
                        item.Destroy(useDestroyImmediate, false);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                // check GameObject is destroyed only on GetEnumerator timing
                return (origin == null)
                    ? new Enumerator(null, withSelf, false)
                    : new Enumerator(origin.transform, withSelf, true);
            }

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            int GetChildrenSize()
            {
                return origin.transform.childCount + (withSelf ? 1 : 0);
            }

            public void ForEach(Action<GameObject> action)
            {
                var e = this.GetEnumerator();
                while (e.MoveNext())
                {
                    action(e.Current);
                }
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(ref GameObject[] array)
            {
                var index = 0;

                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? GetChildrenSize() : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? GetChildrenSize() : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? GetChildrenSize() : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? GetChildrenSize() : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    var state = let(item);

                    if (!filter(state)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? GetChildrenSize() : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(state);
                }

                return index;
            }

            public GameObject[] ToArray()
            {
                var array = new GameObject[GetChildrenSize()];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                var array = new GameObject[GetChildrenSize()];
                var len = ToArrayNonAlloc(filter, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, T> selector)
            {
                var array = new T[GetChildrenSize()];
                var len = ToArrayNonAlloc<T>(selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
            {
                var array = new T[GetChildrenSize()];
                var len = ToArrayNonAlloc(filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
            {
                var array = new T[GetChildrenSize()];
                var len = ToArrayNonAlloc(let, filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject First()
            {
                var e = this.GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public GameObject FirstOrDefault()
            {
                var e = this.GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            #endregion

            public struct Enumerator : IEnumerator<GameObject>
            {
                readonly int childCount; // childCount is fixed when GetEnumerator is called.

                readonly Transform originTransform;
                readonly bool canRun;

                bool withSelf;
                int currentIndex;
                GameObject current;

                public Enumerator(Transform originTransform, bool withSelf, bool canRun)
                {
                    this.originTransform = originTransform;
                    this.withSelf = withSelf;
                    this.childCount = canRun ? originTransform.childCount : 0;
                    this.currentIndex = -1;
                    this.canRun = canRun;
                    this.current = null;
                }

                public bool MoveNext()
                {
                    if (!canRun) return false;

                    if (withSelf)
                    {
                        current = originTransform.gameObject;
                        withSelf = false;
                        return true;
                    }

                    currentIndex++;
                    if (currentIndex < childCount)
                    {
                        var child = originTransform.GetChild(currentIndex);
                        current = child.gameObject;
                        return true;
                    }

                    return false;
                }

                public GameObject Current { get { return current; } }
                object IEnumerator.Current { get { return current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }

            public struct OfComponentEnumerable<T> : IEnumerable<T>
                where T : Component
            {
                ChildrenEnumerable parent;

                public OfComponentEnumerable(ref ChildrenEnumerable parent)
                {
                    this.parent = parent;
                }

                public OfComponentEnumerator<T> GetEnumerator()
                {
                    return new OfComponentEnumerator<T>(ref this.parent);
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #region LINQ

                public void ForEach(Action<T> action)
                {
                    var e = this.GetEnumerator();
                    while (e.MoveNext())
                    {
                        action(e.Current);
                    }
                }

                public T First()
                {
                    var e = this.GetEnumerator();
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else
                    {
                        throw new InvalidOperationException("sequence is empty.");
                    }
                }

                public T FirstOrDefault()
                {
                    var e = this.GetEnumerator();
                    return (e.MoveNext())
                        ? e.Current
                        : null;
                }

                public T[] ToArray()
                {
                    var array = new T[parent.GetChildrenSize()];
                    var len = ToArrayNonAlloc(ref array);
                    if (array.Length != len)
                    {
                        Array.Resize(ref array, len);
                    }
                    return array;
                }

                /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
                public int ToArrayNonAlloc(ref T[] array)
                {
                    var index = 0;
                    var e = this.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (array.Length == index)
                        {
                            var newSize = (index == 0) ? parent.GetChildrenSize() : index * 2;
                            Array.Resize(ref array, newSize);
                        }
                        array[index++] = e.Current;
                    }

                    return index;
                }

                #endregion
            }

            public struct OfComponentEnumerator<T> : IEnumerator<T>
                where T : Component
            {
                Enumerator enumerator; // enumerator is mutable
                T current;

#if UNITY_EDITOR
                static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                public OfComponentEnumerator(ref ChildrenEnumerable parent)
                {
                    this.enumerator = parent.GetEnumerator();
                    this.current = default(T);
                }

                public bool MoveNext()
                {
                    while (enumerator.MoveNext())
                    {
#if UNITY_EDITOR
                        enumerator.Current.GetComponents<T>(componentCache);
                        if (componentCache.Count != 0)
                        {
                            current = componentCache[0];
                            componentCache.Clear();
                            return true;
                        }
#else
                        
                        var component = enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            current = component;
                            return true;
                        }
#endif
                    }

                    return false;
                }

                public T Current { get { return current; } }
                object IEnumerator.Current { get { return current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }
        }

        public struct AncestorsEnumerable : IEnumerable<GameObject>
        {
            readonly GameObject origin;
            readonly bool withSelf;

            public AncestorsEnumerable(GameObject origin, bool withSelf)
            {
                this.origin = origin;
                this.withSelf = withSelf;
            }

            /// <summary>Returns a collection of specified component in the source collection.</summary>
            public OfComponentEnumerable<T> OfComponent<T>()
                where T : Component
            {
                return new OfComponentEnumerable<T>(ref this);
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, false);
                }
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (predicate(item))
                    {
                        item.Destroy(useDestroyImmediate, false);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                // check GameObject is destroyed only on GetEnumerator timing
                return (origin == null)
                    ? new Enumerator(null, null, withSelf, false)
                    : new Enumerator(origin, origin.transform, withSelf, true);
            }

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            public void ForEach(Action<GameObject> action)
            {
                var e = this.GetEnumerator();
                while (e.MoveNext())
                {
                    action(e.Current);
                }
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(ref GameObject[] array)
            {
                var index = 0;

                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    var state = let(item);

                    if (!filter(state)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(state);
                }

                return index;
            }

            public GameObject[] ToArray()
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(filter, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc<T>(selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(let, filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject First()
            {
                var e = this.GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public GameObject FirstOrDefault()
            {
                var e = this.GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            #endregion

            public struct Enumerator : IEnumerator<GameObject>
            {
                readonly bool canRun;

                GameObject current;
                Transform currentTransform;
                bool withSelf;

                public Enumerator(GameObject origin, Transform originTransform, bool withSelf, bool canRun)
                {
                    this.current = origin;
                    this.currentTransform = originTransform;
                    this.withSelf = withSelf;
                    this.canRun = canRun;
                }

                public bool MoveNext()
                {
                    if (!canRun) return false;

                    if (withSelf)
                    {
                        // withSelf, use origin and originTransform
                        withSelf = false;
                        return true;
                    }

                    var parentTransform = currentTransform.parent;
                    if (parentTransform != null)
                    {
                        current = parentTransform.gameObject;
                        currentTransform = parentTransform;
                        return true;
                    }

                    return false;
                }

                public GameObject Current { get { return current; } }
                object IEnumerator.Current { get { return current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }

            public struct OfComponentEnumerable<T> : IEnumerable<T>
                where T : Component
            {
                AncestorsEnumerable parent;

                public OfComponentEnumerable(ref AncestorsEnumerable parent)
                {
                    this.parent = parent;
                }

                public OfComponentEnumerator<T> GetEnumerator()
                {
                    return new OfComponentEnumerator<T>(ref parent);
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #region LINQ

                public void ForEach(Action<T> action)
                {
                    var e = this.GetEnumerator();
                    while (e.MoveNext())
                    {
                        action(e.Current);
                    }
                }

                public T First()
                {
                    var e = this.GetEnumerator();
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else
                    {
                        throw new InvalidOperationException("sequence is empty.");
                    }
                }

                public T FirstOrDefault()
                {
                    var e = this.GetEnumerator();
                    return (e.MoveNext())
                        ? e.Current
                        : null;
                }

                public T[] ToArray()
                {
                    var array = new T[4];
                    var len = ToArrayNonAlloc(ref array);
                    if (array.Length != len)
                    {
                        Array.Resize(ref array, len);
                    }
                    return array;
                }

                /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
                public int ToArrayNonAlloc(ref T[] array)
                {
                    var index = 0;
                    var e = this.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (array.Length == index)
                        {
                            var newSize = (index == 0) ? 4 : index * 2;
                            Array.Resize(ref array, newSize);
                        }
                        array[index++] = e.Current;
                    }

                    return index;
                }

                #endregion
            }

            public struct OfComponentEnumerator<T> : IEnumerator<T>
                where T : Component
            {
                Enumerator enumerator; // enumerator is mutable
                T current;

#if UNITY_EDITOR
                static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                public OfComponentEnumerator(ref AncestorsEnumerable parent)
                {
                    this.enumerator = parent.GetEnumerator();
                    this.current = default(T);
                }

                public bool MoveNext()
                {
                    while (enumerator.MoveNext())
                    {
#if UNITY_EDITOR
                        enumerator.Current.GetComponents<T>(componentCache);
                        if (componentCache.Count != 0)
                        {
                            current = componentCache[0];
                            componentCache.Clear();
                            return true;
                        }
#else
                        
                        var component = enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            current = component;
                            return true;
                        }
#endif
                    }

                    return false;
                }

                public T Current { get { return current; } }
                object IEnumerator.Current { get { return current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }
        }

        public struct DescendantsEnumerable : IEnumerable<GameObject>
        {
            static readonly Func<Transform, bool> alwaysTrue = _ => true;

            readonly GameObject origin;
            readonly bool withSelf;
            readonly Func<Transform, bool> descendIntoChildren;

            public DescendantsEnumerable(GameObject origin, bool withSelf, Func<Transform, bool> descendIntoChildren)
            {
                this.origin = origin;
                this.withSelf = withSelf;
                this.descendIntoChildren = descendIntoChildren ?? alwaysTrue;
            }

            /// <summary>Returns a collection of specified component in the source collection.</summary>
            public OfComponentEnumerable<T> OfComponent<T>()
                where T : Component
            {
                return new OfComponentEnumerable<T>(ref this);
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, false);
                }
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (predicate(item))
                    {
                        item.Destroy(useDestroyImmediate, false);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                // check GameObject is destroyed only on GetEnumerator timing
                if (origin == null)
                {
                    return new Enumerator(null, withSelf, false, null, descendIntoChildren);
                }

                InternalUnsafeRefStack refStack;
                if (InternalUnsafeRefStack.RefStackPool.Count != 0)
                {
                    refStack = InternalUnsafeRefStack.RefStackPool.Dequeue();
                    refStack.Reset();
                }
                else
                {
                    refStack = new InternalUnsafeRefStack(6);
                }

                return new Enumerator(origin.transform, withSelf, true, refStack, descendIntoChildren);
            }

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            void ResizeArray<T>(ref int index, ref T[] array)
            {
                if (array.Length == index)
                {
                    var newSize = (index == 0) ? 4 : index * 2;
                    Array.Resize(ref array, newSize);
                }
            }

            void DescendantsCore(ref Transform transform, ref Action<GameObject> action)
            {
                if (!descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    action(child.gameObject);
                    DescendantsCore(ref child, ref action);
                }
            }

            void DescendantsCore(ref Transform transform, ref int index, ref GameObject[] array)
            {
                if (!descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    ResizeArray(ref index, ref array);
                    array[index++] = child.gameObject;
                    DescendantsCore(ref child, ref index, ref array);
                }
            }

            void DescendantsCore(ref Func<GameObject, bool> filter, ref Transform transform, ref int index, ref GameObject[] array)
            {
                if (!descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    var childGameObject = child.gameObject;
                    if (filter(childGameObject))
                    {
                        ResizeArray(ref index, ref array);
                        array[index++] = childGameObject;
                    }
                    DescendantsCore(ref filter, ref child, ref index, ref array);
                }
            }

            void DescendantsCore<T>(ref Func<GameObject, T> selector, ref Transform transform, ref int index, ref T[] array)
            {
                if (!descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    ResizeArray(ref index, ref array);
                    array[index++] = selector(child.gameObject);
                    DescendantsCore(ref selector, ref child, ref index, ref array);
                }
            }

            void DescendantsCore<T>(ref Func<GameObject, bool> filter, ref Func<GameObject, T> selector, ref Transform transform, ref int index, ref T[] array)
            {
                if (!descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    var childGameObject = child.gameObject;
                    if (filter(childGameObject))
                    {
                        ResizeArray(ref index, ref array);
                        array[index++] = selector(childGameObject);
                    }
                    DescendantsCore(ref filter, ref selector, ref child, ref index, ref array);
                }
            }

            void DescendantsCore<TState, T>(ref Func<GameObject, TState> let, ref Func<TState, bool> filter, ref Func<TState, T> selector, ref Transform transform, ref int index, ref T[] array)
            {
                if (!descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    var state = let(child.gameObject);
                    if (filter(state))
                    {
                        ResizeArray(ref index, ref array);
                        array[index++] = selector(state);
                    }
                    DescendantsCore(ref let, ref filter, ref selector, ref child, ref index, ref array);
                }
            }

            /// <summary>Use publiciterator for performance optimization.</summary>
            /// <param name="action"></param>
            public void ForEach(Action<GameObject> action)
            {
                if (withSelf)
                {
                    action(origin);
                }
                var originTransform = origin.transform;
                DescendantsCore(ref originTransform, ref action);
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(ref GameObject[] array)
            {
                var index = 0;
                if (withSelf)
                {
                    ResizeArray(ref index, ref array);
                    array[index++] = origin;
                }

                var originTransform = origin.transform;
                DescendantsCore(ref originTransform, ref index, ref array);

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                var index = 0;
                if (withSelf && filter(origin))
                {
                    ResizeArray(ref index, ref array);
                    array[index++] = origin;
                }
                var originTransform = origin.transform;
                DescendantsCore(ref filter, ref originTransform, ref index, ref array);

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                if (withSelf)
                {
                    ResizeArray(ref index, ref array);
                    array[index++] = selector(origin);
                }
                var originTransform = origin.transform;
                DescendantsCore(ref selector, ref originTransform, ref index, ref array);

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                if (withSelf && filter(origin))
                {
                    ResizeArray(ref index, ref array);
                    array[index++] = selector(origin);
                }
                var originTransform = origin.transform;
                DescendantsCore(ref filter, ref selector, ref originTransform, ref index, ref array);

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
            {
                var index = 0;
                if (withSelf)
                {
                    var state = let(origin);
                    if (filter(state))
                    {
                        ResizeArray(ref index, ref array);
                        array[index++] = selector(state);
                    }
                }

                var originTransform = origin.transform;
                DescendantsCore(ref let, ref filter, ref selector, ref originTransform, ref index, ref array);

                return index;
            }

            public GameObject[] ToArray()
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(filter, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc<T>(selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(let, filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject First()
            {
                var e = this.GetEnumerator();
                try
                {
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else
                    {
                        throw new InvalidOperationException("sequence is empty.");
                    }
                }
                finally
                {
                    e.Dispose();
                }
            }

            public GameObject FirstOrDefault()
            {
                var e = this.GetEnumerator();
                try
                {
                    return (e.MoveNext())
                        ? e.Current
                        : null;
                }
                finally
                {
                    e.Dispose();
                }
            }

            #endregion

            public class InternalUnsafeRefStack
            {
                public static Queue<InternalUnsafeRefStack> RefStackPool = new Queue<InternalUnsafeRefStack>();

                public int size = 0;
                public Enumerator[] array; // Pop = this.array[--size];

                public InternalUnsafeRefStack(int initialStackDepth)
                {
                    array = new GameObjectExtensions.DescendantsEnumerable.Enumerator[initialStackDepth];
                }

                public void Push(ref Enumerator e)
                {
                    if (size == array.Length)
                    {
                        Array.Resize(ref array, array.Length * 2);
                    }
                    array[size++] = e;
                }

                public void Reset()
                {
                    size = 0;
                }
            }

            public struct Enumerator : IEnumerator<GameObject>
            {
                readonly int childCount; // childCount is fixed when GetEnumerator is called.

                readonly Transform originTransform;
                bool canRun;

                bool withSelf;
                int currentIndex;
                GameObject current;
                InternalUnsafeRefStack sharedStack;
                Func<Transform, bool> descendIntoChildren;

                public Enumerator(Transform originTransform, bool withSelf, bool canRun, InternalUnsafeRefStack sharedStack, Func<Transform, bool> descendIntoChildren)
                {
                    this.originTransform = originTransform;
                    this.withSelf = withSelf;
                    this.childCount = canRun ? originTransform.childCount : 0;
                    this.currentIndex = -1;
                    this.canRun = canRun;
                    this.current = null;
                    this.sharedStack = sharedStack;
                    this.descendIntoChildren = descendIntoChildren;
                }

                public bool MoveNext()
                {
                    if (!canRun) return false;

                    while (sharedStack.size != 0)
                    {
                        if (sharedStack.array[sharedStack.size - 1].MoveNextCore(true, out current))
                        {
                            return true;
                        }
                    }

                    if (!withSelf && !descendIntoChildren(originTransform))
                    {
                        // reuse
                        canRun = false;
                        InternalUnsafeRefStack.RefStackPool.Enqueue(sharedStack);
                        return false;
                    }

                    if (MoveNextCore(false, out current))
                    {
                        return true;
                    }
                    else
                    {
                        // reuse
                        canRun = false;
                        InternalUnsafeRefStack.RefStackPool.Enqueue(sharedStack);
                        return false;
                    }
                }

                bool MoveNextCore(bool peek, out GameObject current)
                {
                    if (withSelf)
                    {
                        current = originTransform.gameObject;
                        withSelf = false;
                        return true;
                    }

                    ++currentIndex;
                    if (currentIndex < childCount)
                    {
                        var item = originTransform.GetChild(currentIndex);
                        if (descendIntoChildren(item))
                        {
                            var childEnumerator = new Enumerator(item, true, true, sharedStack, descendIntoChildren);
                            sharedStack.Push(ref childEnumerator);
                            return sharedStack.array[sharedStack.size - 1].MoveNextCore(true, out current);
                        }
                        else
                        {
                            current = item.gameObject;
                            return true;
                        }
                    }

                    if (peek)
                    {
                        sharedStack.size--; // Pop
                    }

                    current = null;
                    return false;
                }

                public GameObject Current { get { return current; } }
                object IEnumerator.Current { get { return current; } }

                public void Dispose()
                {
                    if (canRun)
                    {
                        canRun = false;
                        InternalUnsafeRefStack.RefStackPool.Enqueue(sharedStack);
                    }
                }

                public void Reset() { throw new NotSupportedException(); }
            }

            public struct OfComponentEnumerable<T> : IEnumerable<T>
                where T : Component
            {
                DescendantsEnumerable parent;

                public OfComponentEnumerable(ref DescendantsEnumerable parent)
                {
                    this.parent = parent;
                }

                public OfComponentEnumerator<T> GetEnumerator()
                {
                    return new OfComponentEnumerator<T>(ref parent);
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #region LINQ

                public T First()
                {
                    var e = this.GetEnumerator();
                    try
                    {
                        if (e.MoveNext())
                        {
                            return e.Current;
                        }
                        else
                        {
                            throw new InvalidOperationException("sequence is empty.");
                        }
                    }
                    finally
                    {
                        e.Dispose();
                    }
                }

                public T FirstOrDefault()
                {
                    var e = this.GetEnumerator();
                    try
                    {
                        return (e.MoveNext())
                            ? e.Current
                            : null;
                    }
                    finally
                    {
                        e.Dispose();
                    }
                }

                /// <summary>Use publiciterator for performance optimization.</summary>
                public void ForEach(Action<T> action)
                {
                    if (parent.withSelf)
                    {
                        T component = default(T);
#if UNITY_EDITOR
                        parent.origin.GetComponents<T>(componentCache);
                        if (componentCache.Count != 0)
                        {
                            component = componentCache[0];
                            componentCache.Clear();
                        }
#else
                        component = parent.origin.GetComponent<T>();
#endif

                        if (component != null)
                        {
                            action(component);
                        }
                    }

                    var originTransform = parent.origin.transform;
                    OfComponentDescendantsCore(ref originTransform, ref action);
                }


                public T[] ToArray()
                {
                    var array = new T[4];
                    var len = ToArrayNonAlloc(ref array);
                    if (array.Length != len)
                    {
                        Array.Resize(ref array, len);
                    }
                    return array;
                }

#if UNITY_EDITOR
                static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                void OfComponentDescendantsCore(ref Transform transform, ref Action<T> action)
                {
                    if (!parent.descendIntoChildren(transform)) return;

                    var childCount = transform.childCount;
                    for (int i = 0; i < childCount; i++)
                    {
                        var child = transform.GetChild(i);

                        T component = default(T);
#if UNITY_EDITOR
                        child.GetComponents<T>(componentCache);
                        if (componentCache.Count != 0)
                        {
                            component = componentCache[0];
                            componentCache.Clear();
                        }
#else
                        component = child.GetComponent<T>();
#endif

                        if (component != null)
                        {
                            action(component);
                        }
                        OfComponentDescendantsCore(ref child, ref action);
                    }
                }

                void OfComponentDescendantsCore(ref Transform transform, ref int index, ref T[] array)
                {
                    if (!parent.descendIntoChildren(transform)) return;

                    var childCount = transform.childCount;
                    for (int i = 0; i < childCount; i++)
                    {
                        var child = transform.GetChild(i);
                        T component = default(T);
#if UNITY_EDITOR
                        child.GetComponents<T>(componentCache);
                        if (componentCache.Count != 0)
                        {
                            component = componentCache[0];
                            componentCache.Clear();
                        }
#else
                        component = child.GetComponent<T>();
#endif

                        if (component != null)
                        {
                            if (array.Length == index)
                            {
                                var newSize = (index == 0) ? 4 : index * 2;
                                Array.Resize(ref array, newSize);
                            }

                            array[index++] = component;
                        }
                        OfComponentDescendantsCore(ref child, ref index, ref array);
                    }
                }

                /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
                public int ToArrayNonAlloc(ref T[] array)
                {
                    var index = 0;
                    if (parent.withSelf)
                    {
                        T component = default(T);
#if UNITY_EDITOR
                        parent.origin.GetComponents<T>(componentCache);
                        if (componentCache.Count != 0)
                        {
                            component = componentCache[0];
                            componentCache.Clear();
                        }
#else
                        component = parent.origin.GetComponent<T>();
#endif

                        if (component != null)
                        {
                            if (array.Length == index)
                            {
                                var newSize = (index == 0) ? 4 : index * 2;
                                Array.Resize(ref array, newSize);
                            }

                            array[index++] = component;
                        }
                    }

                    var originTransform = parent.origin.transform;
                    OfComponentDescendantsCore(ref originTransform, ref index, ref array);

                    return index;
                }

                #endregion
            }

            public struct OfComponentEnumerator<T> : IEnumerator<T>
                where T : Component
            {
                Enumerator enumerator; // enumerator is mutable
                T current;

#if UNITY_EDITOR
                static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                public OfComponentEnumerator(ref DescendantsEnumerable parent)
                {
                    this.enumerator = parent.GetEnumerator();
                    this.current = default(T);
                }

                public bool MoveNext()
                {
                    while (enumerator.MoveNext())
                    {
#if UNITY_EDITOR
                        enumerator.Current.GetComponents<T>(componentCache);
                        if (componentCache.Count != 0)
                        {
                            current = componentCache[0];
                            componentCache.Clear();
                            return true;
                        }
#else
                        
                        var component = enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            current = component;
                            return true;
                        }
#endif
                    }

                    return false;
                }

                public T Current { get { return current; } }
                object IEnumerator.Current { get { return current; } }

                public void Dispose()
                {
                    enumerator.Dispose();
                }

                public void Reset() { throw new NotSupportedException(); }
            }
        }

        public struct BeforeSelfEnumerable : IEnumerable<GameObject>
        {
            readonly GameObject origin;
            readonly bool withSelf;

            public BeforeSelfEnumerable(GameObject origin, bool withSelf)
            {
                this.origin = origin;
                this.withSelf = withSelf;
            }

            /// <summary>Returns a collection of specified component in the source collection.</summary>
            public OfComponentEnumerable<T> OfComponent<T>()
                where T : Component
            {
                return new OfComponentEnumerable<T>(ref this);
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, false);
                }
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (predicate(item))
                    {
                        item.Destroy(useDestroyImmediate, false);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                // check GameObject is destroyed only on GetEnumerator timing
                return (origin == null)
                    ? new Enumerator(null, withSelf, false)
                    : new Enumerator(origin.transform, withSelf, true);
            }

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            public void ForEach(Action<GameObject> action)
            {
                var e = this.GetEnumerator();
                while (e.MoveNext())
                {
                    action(e.Current);
                }
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(ref GameObject[] array)
            {
                var index = 0;

                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    var state = let(item);

                    if (!filter(state)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(state);
                }

                return index;
            }

            public GameObject[] ToArray()
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(filter, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc<T>(selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(let, filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject First()
            {
                var e = this.GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public GameObject FirstOrDefault()
            {
                var e = this.GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            #endregion

            public struct Enumerator : IEnumerator<GameObject>
            {
                readonly int childCount; // childCount is fixed when GetEnumerator is called.
                readonly Transform originTransform;
                bool canRun;

                bool withSelf;
                int currentIndex;
                GameObject current;
                Transform parent;

                public Enumerator(Transform originTransform, bool withSelf, bool canRun)
                {
                    this.originTransform = originTransform;
                    this.withSelf = withSelf;
                    this.currentIndex = -1;
                    this.canRun = canRun;
                    this.current = null;
                    this.parent = originTransform.parent;
                    this.childCount = (parent != null) ? parent.childCount : 0;
                }

                public bool MoveNext()
                {
                    if (!canRun) return false;

                    if (parent == null) goto RETURN_SELF;

                    currentIndex++;
                    if (currentIndex < childCount)
                    {
                        var item = parent.GetChild(currentIndex);

                        if (item == originTransform)
                        {
                            goto RETURN_SELF;
                        }

                        current = item.gameObject;
                        return true;
                    }

                    RETURN_SELF:
                    if (withSelf)
                    {
                        current = originTransform.gameObject;
                        withSelf = false;
                        canRun = false; // reached self, run complete.
                        return true;
                    }

                    return false;
                }

                public GameObject Current { get { return current; } }
                object IEnumerator.Current { get { return current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }

            public struct OfComponentEnumerable<T> : IEnumerable<T>
                where T : Component
            {
                BeforeSelfEnumerable parent;

                public OfComponentEnumerable(ref BeforeSelfEnumerable parent)
                {
                    this.parent = parent;
                }

                public OfComponentEnumerator<T> GetEnumerator()
                {
                    return new OfComponentEnumerator<T>(ref parent);
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #region LINQ

                public void ForEach(Action<T> action)
                {
                    var e = this.GetEnumerator();
                    while (e.MoveNext())
                    {
                        action(e.Current);
                    }
                }

                public T First()
                {
                    var e = this.GetEnumerator();
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else
                    {
                        throw new InvalidOperationException("sequence is empty.");
                    }
                }

                public T FirstOrDefault()
                {
                    var e = this.GetEnumerator();
                    return (e.MoveNext())
                        ? e.Current
                        : null;
                }

                public T[] ToArray()
                {
                    var array = new T[4];
                    var len = ToArrayNonAlloc(ref array);
                    if (array.Length != len)
                    {
                        Array.Resize(ref array, len);
                    }
                    return array;
                }

                /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
                public int ToArrayNonAlloc(ref T[] array)
                {
                    var index = 0;
                    var e = this.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (array.Length == index)
                        {
                            var newSize = (index == 0) ? 4 : index * 2;
                            Array.Resize(ref array, newSize);
                        }
                        array[index++] = e.Current;
                    }

                    return index;
                }

                #endregion
            }

            public struct OfComponentEnumerator<T> : IEnumerator<T>
                where T : Component
            {
                Enumerator enumerator; // enumerator is mutable
                T current;

#if UNITY_EDITOR
                static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                public OfComponentEnumerator(ref BeforeSelfEnumerable parent)
                {
                    this.enumerator = parent.GetEnumerator();
                    this.current = default(T);
                }

                public bool MoveNext()
                {
                    while (enumerator.MoveNext())
                    {
#if UNITY_EDITOR
                        enumerator.Current.GetComponents<T>(componentCache);
                        if (componentCache.Count != 0)
                        {
                            current = componentCache[0];
                            componentCache.Clear();
                            return true;
                        }
#else
                        
                        var component = enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            current = component;
                            return true;
                        }
#endif
                    }

                    return false;
                }

                public T Current { get { return current; } }
                object IEnumerator.Current { get { return current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }
        }

        public struct AfterSelfEnumerable : IEnumerable<GameObject>
        {
            readonly GameObject origin;
            readonly bool withSelf;

            public AfterSelfEnumerable(GameObject origin, bool withSelf)
            {
                this.origin = origin;
                this.withSelf = withSelf;
            }

            /// <summary>Returns a collection of specified component in the source collection.</summary>
            public OfComponentEnumerable<T> OfComponent<T>()
                where T : Component
            {
                return new OfComponentEnumerable<T>(ref this);
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, false);
                }
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (predicate(item))
                    {
                        item.Destroy(useDestroyImmediate, false);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                // check GameObject is destroyed only on GetEnumerator timing
                return (origin == null)
                    ? new Enumerator(null, withSelf, false)
                    : new Enumerator(origin.transform, withSelf, true);
            }

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            public void ForEach(Action<GameObject> action)
            {
                var e = this.GetEnumerator();
                while (e.MoveNext())
                {
                    action(e.Current);
                }
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(ref GameObject[] array)
            {
                var index = 0;

                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    var state = let(item);

                    if (!filter(state)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(state);
                }

                return index;
            }

            public GameObject[] ToArray()
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(filter, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc<T>(selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(let, filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject First()
            {
                var e = this.GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public GameObject FirstOrDefault()
            {
                var e = this.GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            #endregion

            public struct Enumerator : IEnumerator<GameObject>
            {
                readonly int childCount; // childCount is fixed when GetEnumerator is called.
                readonly Transform originTransform;
                readonly bool canRun;

                bool withSelf;
                int currentIndex;
                GameObject current;
                Transform parent;

                public Enumerator(Transform originTransform, bool withSelf, bool canRun)
                {
                    this.originTransform = originTransform;
                    this.withSelf = withSelf;
                    this.currentIndex = (originTransform != null) ? originTransform.GetSiblingIndex() + 1 : 0;
                    this.canRun = canRun;
                    this.current = null;
                    this.parent = originTransform.parent;
                    this.childCount = (parent != null) ? parent.childCount : 0;
                }

                public bool MoveNext()
                {
                    if (!canRun) return false;

                    if (withSelf)
                    {
                        current = originTransform.gameObject;
                        withSelf = false;
                        return true;
                    }

                    if (currentIndex < childCount)
                    {
                        current = parent.GetChild(currentIndex).gameObject;
                        currentIndex++;
                        return true;
                    }

                    return false;
                }

                public GameObject Current { get { return current; } }
                object IEnumerator.Current { get { return current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }

            public struct OfComponentEnumerable<T> : IEnumerable<T>
                where T : Component
            {
                AfterSelfEnumerable parent;

                public OfComponentEnumerable(ref AfterSelfEnumerable parent)
                {
                    this.parent = parent;
                }

                public OfComponentEnumerator<T> GetEnumerator()
                {
                    return new OfComponentEnumerator<T>(ref this.parent);
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #region LINQ

                public void ForEach(Action<T> action)
                {
                    var e = this.GetEnumerator();
                    while (e.MoveNext())
                    {
                        action(e.Current);
                    }
                }

                public T First()
                {
                    var e = this.GetEnumerator();
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else
                    {
                        throw new InvalidOperationException("sequence is empty.");
                    }
                }

                public T FirstOrDefault()
                {
                    var e = this.GetEnumerator();
                    return (e.MoveNext())
                        ? e.Current
                        : null;
                }

                public T[] ToArray()
                {
                    var array = new T[4];
                    var len = ToArrayNonAlloc(ref array);
                    if (array.Length != len)
                    {
                        Array.Resize(ref array, len);
                    }
                    return array;
                }

                /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
                public int ToArrayNonAlloc(ref T[] array)
                {
                    var index = 0;
                    var e = this.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (array.Length == index)
                        {
                            var newSize = (index == 0) ? 4 : index * 2;
                            Array.Resize(ref array, newSize);
                        }
                        array[index++] = e.Current;
                    }

                    return index;
                }

                #endregion
            }

            public struct OfComponentEnumerator<T> : IEnumerator<T>
                where T : Component
            {
                Enumerator enumerator; // enumerator is mutable
                T current;

#if UNITY_EDITOR
                static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                public OfComponentEnumerator(ref AfterSelfEnumerable parent)
                {
                    this.enumerator = parent.GetEnumerator();
                    this.current = default(T);
                }

                public bool MoveNext()
                {
                    while (enumerator.MoveNext())
                    {
#if UNITY_EDITOR
                        enumerator.Current.GetComponents<T>(componentCache);
                        if (componentCache.Count != 0)
                        {
                            current = componentCache[0];
                            componentCache.Clear();
                            return true;
                        }
#else
                        
                        var component = enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            current = component;
                            return true;
                        }
#endif
                    }

                    return false;
                }

                public T Current { get { return current; } }
                object IEnumerator.Current { get { return current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }
        }
    }
}