using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun
{

    public static class NestedComponentUtilities
    {

        public static T EnsureRootComponentExists<T, NestedT>(this Transform transform)
            where T : Component
            where NestedT : Component
        {
            var root = GetParentComponent<NestedT>(transform);
            if (root)
            {
                var comp = root.GetComponent<T>();

                if (comp)
                    return comp;

                return root.gameObject.AddComponent<T>();
            }

            return null;
        }

        #region GetComponent Replacements

        // Recycled collections
        private static Queue<Transform> nodesQueue = new Queue<Transform>();
        public static Dictionary<System.Type, ICollection> searchLists = new Dictionary<System.Type, ICollection>();
        private static Stack<Transform> nodeStack = new Stack<Transform>();

        /// <summary>
        /// Find T on supplied transform or any parent. Unlike GetComponentInParent, GameObjects do not need to be active to be found.
        /// </summary>
        public static T GetParentComponent<T>(this Transform t)
            where T : Component
        {
            T found = t.GetComponent<T>();

            if (found)
                return found;

            var par = t.parent;
            while (par)
            {
                found = par.GetComponent<T>();
                if (found)
                    return found;
                par = par.parent;
            }
            return null;
        }


        /// <summary>
        /// Returns all T found between the child transform and its root. Order in List from child to parent, with the root/parent most being last.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static void GetNestedComponentsInParents<T>(this Transform t, List<T> list)
            where T : Component
        {
            list.Clear();

            while (t != null)
            {
                T obj = t.GetComponent<T>();
                if (obj)
                    list.Add(obj);

                t = t.parent;
            }
        }

        public static T GetNestedComponentInChildren<T, NestedT>(this Transform t, bool includeInactive)
            where T : class
            where NestedT : class
        {
            // Look for the most obvious check first on the root.
            var found = t.GetComponent<T>();
            if (!ReferenceEquals(found, null))
                return found;

            // No root found, start testing layer by layer - root is the first layer. Add to queue.
            nodesQueue.Clear();
            nodesQueue.Enqueue(t);

            while (nodesQueue.Count > 0)
            {
                var node = nodesQueue.Dequeue();

                for (int c = 0, ccnt = node.childCount; c < ccnt; ++c)
                {
                    var child = node.GetChild(c);

                    // Ignore branches that are not active
                    if (!includeInactive && !child.gameObject.activeSelf)
                        continue;

                    // Hit a nested node - don't search this node
                    if (!ReferenceEquals(child.GetComponent<NestedT>(), null))
                        continue;

                    // see if what we are looking for is on this node
                    found = child.GetComponent<T>();

                    // Return if we found what we are looking for
                    if (!ReferenceEquals(found, null))
                        return found;

                    // Add node to queue for next depth pass since nothing was found on this layer.
                    nodesQueue.Enqueue(child);
                }

            }
            return found;
        }

        /// <summary>
        /// Same as GetComponentInParent, but will always include inactive objects in search.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="DontRecurseOnT"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T GetNestedComponentInParent<T, NestedT>(this Transform t)
            where T : class
            where NestedT : class
        {
            T found = null;

            Transform node = t;
            do
            {

                found = node.GetComponent<T>();

                if (!ReferenceEquals(found, null))
                    return found;

                // stop search on node with PV
                if (!ReferenceEquals(node.GetComponent<NestedT>(), null))
                    return null;

                node = node.parent;
            }
            while (!ReferenceEquals(node, null));

            return null;
        }

        /// <summary>
        /// UNTESTED
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="StopSearchOnT"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T GetNestedComponentInParents<T, NestedT>(this Transform t)
            where T : class
            where NestedT : class
        {
            // First try root
            var found = t.GetComponent<T>();

            if (!ReferenceEquals(found, null))
                return found;

            /// Get the reverse list of transforms climbing for start up to netobject
            var par = t.parent;

            while (!ReferenceEquals(par, null))
            {
                found = par.GetComponent<T>();
                if (!ReferenceEquals(found, null))
                    return found;

                /// Stop climbing at the NetObj (this is how we detect nesting
                if (!ReferenceEquals(par.GetComponent<NestedT>(), null))
                    return null;

                par = par.parent;
            };

            return null;
        }


        /// <summary>
        /// Finds components of type T on supplied transform, and every parent above that node, inclusively stopping on node StopSearchOnT component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="StopSearchOnT"></typeparam>
        /// <param name="t"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static void GetNestedComponentsInParents<T, NestedT>(this Transform t, List<T> list)
            where T : class
            where NestedT : class
        {

            // Get components on the starting node - this is a given.
            t.GetComponents(list);

            // If the starting node has the stop component, we are done.
            if (!ReferenceEquals(t.GetComponent<NestedT>(), null))
                return;

            var tnode = t.parent;

            // If there is no parent, we are done.
            if (ReferenceEquals(tnode, null))
                return;

            nodeStack.Clear();

            while (true)
            {
                // add new parent to stack
                nodeStack.Push(tnode);

                // if this node has the Stop, we are done recursing up.
                if (!ReferenceEquals(tnode.GetComponent<NestedT>(), null))
                    break;

                // Get the next parent node and add it to the stack
                tnode = tnode.parent;

                // Stop recursing up if the parent is null
                if (ReferenceEquals(tnode, null))
                    break;
            }

            if (nodeStack.Count == 0)
                return;

            System.Type type = typeof(T);

            // Acquire the right searchlist from our pool
            List<T> searchList;
            if (!searchLists.ContainsKey(type))
            {
                searchList = new List<T>();
                searchLists.Add(type, searchList);
            }
            else
            {
                searchList = searchLists[type] as List<T>;
            }

            // Reverse iterate the nodes found. This produces a GetComponentInParent that starts from the parent Stop down to the provided transform
            while (nodeStack.Count > 0)
            {
                var node = nodeStack.Pop();

                node.GetComponents(searchList);
                list.AddRange(searchList);
            }
        }


        /// <summary>
        /// Same as GetComponentsInChildren, but will not recurse into children with component of the DontRecurseOnT type. This allows nesting of PhotonViews/NetObjects to be respected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="list">Pass null and a reused list will be used. Consume immediately.</param>
        public static List<T> GetNestedComponentsInChildren<T, NestedT>(this Transform t, List<T> list, bool includeInactive = true)
            where T : class
            where NestedT : class
        {
            System.Type type = typeof(T);

            // Temp lists are also recycled. Get/Create a reusable List of this type.
            List<T> searchList;
            if (!searchLists.ContainsKey(type))
                searchLists.Add(type, searchList = new List<T>());
            else
                searchList = searchLists[type] as List<T>;

            nodesQueue.Clear();

            if (list == null)
                list = new List<T>();

            // Get components on starting transform - no exceptions
            t.GetComponents(list);

            // Add first layer of children to the queue for next layer processing.
            for (int i = 0, cnt = t.childCount; i < cnt; ++i)
            {
                var child = t.GetChild(i);

                // Ignore inactive nodes (optional)
                if (!includeInactive && !child.gameObject.activeSelf)
                    continue;

                // ignore nested DontRecurseOnT
                if (!ReferenceEquals(child.GetComponent<NestedT>(), null))
                    continue;

                nodesQueue.Enqueue(child);
            }

            // Recurse node layers
            while (nodesQueue.Count > 0)
            {
                var node = nodesQueue.Dequeue();

                // Add found components on this gameobject node
                node.GetComponents(searchList);
                list.AddRange(searchList);

                // Add children to the queue for next layer processing.
                for (int i = 0, cnt = node.childCount; i < cnt; ++i)
                {
                    var child = node.GetChild(i);

                    // Ignore inactive nodes (optional)
                    if (!includeInactive && !child.gameObject.activeSelf)
                        continue;

                    // ignore nested NestedT
                    if (!ReferenceEquals(child.GetComponent<NestedT>(), null))
                        continue;

                    nodesQueue.Enqueue(child);
                }
            }

            return list;
        }

        /// <summary>
        /// Same as GetComponentsInChildren, but will not recurse into children with component of the DontRecurseOnT type. This allows nesting of PhotonViews/NetObjects to be respected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="list">Pass null and a reused list will be used. Consume immediately.</param>
        public static List<T> GetNestedComponentsInChildren<T>(this Transform t, List<T> list, bool includeInactive = true, params System.Type[] stopOn)
            where T : class
        {
            System.Type type = typeof(T);

            // Temp lists are also recycled. Get/Create a reusable List of this type.
            List<T> searchList;
            if (!searchLists.ContainsKey(type))
                searchLists.Add(type, searchList = new List<T>());
            else
                searchList = searchLists[type] as List<T>;

            nodesQueue.Clear();

            // Get components on starting transform - no exceptions
            t.GetComponents(list);

            // Add first layer of children to the queue for next layer processing.
            for (int i = 0, cnt = t.childCount; i < cnt; ++i)
            {
                var child = t.GetChild(i);

                // Ignore inactive nodes (optional)
                if (!includeInactive && !child.gameObject.activeSelf)
                    continue;

                // ignore nested DontRecurseOnT
                bool stopRecurse = false;
                for (int s = 0, scnt = stopOn.Length; s < scnt; ++s)
                {
                    if (!ReferenceEquals(child.GetComponent(stopOn[s]), null))
                    {
                        stopRecurse = true;
                        break;
                    }
                }
                if (stopRecurse)
                    continue;

                nodesQueue.Enqueue(child);
            }

            // Recurse node layers
            while (nodesQueue.Count > 0)
            {
                var node = nodesQueue.Dequeue();

                // Add found components on this gameobject node
                node.GetComponents(searchList);
                list.AddRange(searchList);

                // Add children to the queue for next layer processing.
                for (int i = 0, cnt = node.childCount; i < cnt; ++i)
                {
                    var child = node.GetChild(i);

                    // Ignore inactive nodes (optional)
                    if (!includeInactive && !child.gameObject.activeSelf)
                        continue;

                    // ignore nested NestedT
                    bool stopRecurse = false;
                    for (int s = 0, scnt = stopOn.Length; s < scnt; ++s)
                    {
                        if (!ReferenceEquals(child.GetComponent(stopOn[s]), null))
                        {
                            stopRecurse = true;
                            break;
                        }
                    }

                    if (stopRecurse)
                        continue;

                    nodesQueue.Enqueue(child);
                }
            }

            return list;
        }

        /// <summary>
        /// Same as GetComponentsInChildren, but will not recurse into children with component of the NestedT type. This allows nesting of PhotonViews/NetObjects to be respected.
        /// </summary>
        /// <typeparam name="T">Cast found components to this type. Typically Component, but any other class/interface will work as long as they are assignable from SearchT.</typeparam>
        /// <typeparam name="SearchT">Find components of this class or interface type.</typeparam>
        /// <typeparam name="DontRecurseOnT"></typeparam>
        /// <param name="t"></param>
        /// <param name="includeInactive"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static void GetNestedComponentsInChildren<T, SearchT, NestedT>(this Transform t, bool includeInactive, List<T> list)
            where T : class
            where SearchT : class
        {
            list.Clear();

            // If this is inactive, nothing will be found. Give up now if we are restricted to active.
            if (!includeInactive && !t.gameObject.activeSelf)
                return;

            System.Type searchType = typeof(SearchT);

            // Temp lists are also recycled. Get/Create a reusable List of this type.
            List<SearchT> searchList;
            if (!searchLists.ContainsKey(searchType))
                searchLists.Add(searchType, searchList = new List<SearchT>());
            else
                searchList = searchLists[searchType] as List<SearchT>;

            // Recurse child nodes one layer at a time. Using a Queue allows this to happen without a lot of work.
            nodesQueue.Clear();
            nodesQueue.Enqueue(t);

            while (nodesQueue.Count > 0)
            {
                var node = nodesQueue.Dequeue();

                // Add found components on this gameobject node
                searchList.Clear();
                node.GetComponents(searchList);
                foreach (var comp in searchList)
                {
                    var casted = comp as T;
                    if (!ReferenceEquals(casted, null))
                        list.Add(casted);
                }

                // Add children to the queue for next layer processing.
                for (int i = 0, cnt = node.childCount; i < cnt; ++i)
                {
                    var child = node.GetChild(i);

                    // Ignore inactive nodes (optional)
                    if (!includeInactive && !child.gameObject.activeSelf)
                        continue;

                    // ignore nested DontRecurseOnT
                    if (!ReferenceEquals(child.GetComponent<NestedT>(), null))
                        continue;

                    nodesQueue.Enqueue(child);
                }
            }

        }

        #endregion
    }

}