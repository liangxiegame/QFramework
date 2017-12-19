using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace QFramework
{
    public class AssemblyResolver
    {
        readonly AppDomain mAppDomain;
        string[] mBasePaths;
        List<Assembly> mAssemblies;

        public AssemblyResolver(AppDomain appDomain, string[] basePaths)
        {
            mAppDomain = appDomain;
//            mAppDomain.AssemblyResolve += OnAssemblyResolve;
            mBasePaths = basePaths;
            mAssemblies = new List<Assembly>();
        }


        public void LoadUnityAssemblies()
        {
            var assembly = ReflectionUtil.GetAssemblyCSharp();
            mAssemblies.Add(assembly);
            assembly = ReflectionUtil.GetAssemblyCSharpEditor();
            mAssemblies.Add(assembly);
        }

        public void Load(string path)
        {
            Log.I("AppDomain load: {0}", path);
            var assembly = mAppDomain.Load(path);
            mAssemblies.Add(assembly);
        }

        Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly assembly = null;
            try
            {
                Log.I("  - Loading: " + args.Name);
                assembly = Assembly.LoadFrom(args.Name);
            }
            catch (Exception)
            {
                var name = new AssemblyName(args.Name).Name;
                if (!name.EndsWith(".dll", StringComparison.Ordinal))
                {
                    name += ".dll";
                }

                var path = ResolvePath(name);
                if (path != null)
                {
                    assembly = Assembly.LoadFrom(path);
                }
            }

            return assembly;
        }

        string ResolvePath(string assemblyName)
        {
            foreach (var basePath in mBasePaths)
            {
                var path = basePath + Path.DirectorySeparatorChar + assemblyName;
                if (File.Exists(path))
                {
                    Log.I("    - Resolved: " + path);
                    return path;
                }
            }

            Log.W("    - Could not resolve: " + assemblyName);
            return null;
        }

        public Type[] GetTypes()
        {
            var types = new List<Type>();
            foreach (var assembly in mAssemblies)
            {
                try
                {
                    types.AddRange(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types.AddRange(ex.Types.Where(type => type != null));
                }
            }

            return types.ToArray();
        }
    }
}