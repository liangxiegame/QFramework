/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QFramework
{
    public static class PackageKitAssemblyCache
    {
        private static readonly Lazy<List<Assembly>> mCachedAssemblies = new Lazy<List<Assembly>>(() =>
        {
            var cachedAssemblies = new List<Assembly>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("QF") || assembly.FullName.Contains("Kit") ||
                    assembly.FullName.StartsWith("Assembly-CSharp"))
                {
                    if (!cachedAssemblies.Contains(assembly))
                    {
                        cachedAssemblies.Add(assembly);
                    }
                }
            }

            return cachedAssemblies;
        });


        public static IEnumerable<Type> GetAllTypes()
        {
            return mCachedAssemblies.Value.SelectMany(a => a.GetTypes());
        }


        public static IEnumerable<Type> GetDerivedTypes<T>(bool includeAbstract = false, bool includeBase = true)
        {
            var type = typeof(T);
            if (includeBase)
                yield return type;
            if (includeAbstract)
            {
                foreach (var t in mCachedAssemblies.Value.SelectMany(assembly => assembly
                             .GetTypes()
                             .Where(x => type.IsAssignableFrom(x))))
                {
                    yield return t;
                }
            }
            else
            {
                var items = new List<Type>();
                foreach (var assembly in mCachedAssemblies.Value)
                {
                    try
                    {
                        items.AddRange(assembly.GetTypes()
                            .Where(x => type.IsAssignableFrom(x) && !x.IsAbstract));
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                }

                foreach (var item in items)
                    yield return item;
            }
        }
    }
}