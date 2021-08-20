/****************************************************************************
 * Copyright (c) 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QFramework
{
    public static class PackageApplication
    {
        public static List<Assembly> CachedAssemblies { get; set; }

        private static IQFrameworkContainer mContainer = null;

        public static IQFrameworkContainer Container
        {
            get
            {
                if (mContainer != null) return mContainer;
                mContainer = new QFrameworkContainer();
                InitializeContainer(mContainer);
                return mContainer;
            }
            set { mContainer = value; }
        }

        public static IEnumerable<Type> GetDerivedTypes<T>(bool includeAbstract = false, bool includeBase = true)
        {
            var type = typeof(T);
            if (includeBase)
                yield return type;
            if (includeAbstract)
            {
                foreach (var t in CachedAssemblies.SelectMany(assembly => assembly
                    .GetTypes()
                    .Where(x => type.IsAssignableFrom(x))))
                {
                    yield return t;
                }
            }
            else
            {
                var items = new List<Type>();
                foreach (var assembly in CachedAssemblies)
                {
                    try
                    {
                        items.AddRange(assembly.GetTypes()
                            .Where(x => type.IsAssignableFrom(x) && !x.IsAbstract));
                    }
                    catch (Exception ex)
                    {
                        Log.I(ex.Message);
                    }
                }

                foreach (var item in items)
                    yield return item;
            }
        }


        private static IPackageKitView[] mViews;

        public static IPackageKitView[] Views
        {
            get { return mViews ?? (mViews = Container.ResolveAll<IPackageKitView>().ToArray()); }
            set { mViews = value; }
        }

        private static void InitializeContainer(IQFrameworkContainer container)
        {
            mViews = null;
            container.RegisterInstance(container);
            var viewTypes = GetDerivedTypes<IPackageKitView>(false, false).ToArray();

            foreach (var view in viewTypes)
            {
                var viewInstance = Activator.CreateInstance(view) as IPackageKitView;
                if (viewInstance == null) continue;
                container.RegisterInstance(viewInstance, view.Name, false);
                container.RegisterInstance(viewInstance.GetType(), viewInstance);
            }

            container.InjectAll();

            foreach (var view in Views)
            {
                view.Container = Container;
                view.Init(Container);
            }

            foreach (var view in Views)
            {
                container.Inject(view);
            }
        }

        static PackageApplication()
        {
            CachedAssemblies = new List<Assembly>
            {
                typeof(int).Assembly, typeof(List<>).Assembly
            };

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("QF") || assembly.FullName.StartsWith("Assembly-CSharp-Editor"))
                {
                    CachedAssembly(assembly);
                }
            }
        }

        public static void CachedAssembly(Assembly assembly)
        {
            if (CachedAssemblies.Contains(assembly)) return;
            CachedAssemblies.Add(assembly);
        }
    }
}