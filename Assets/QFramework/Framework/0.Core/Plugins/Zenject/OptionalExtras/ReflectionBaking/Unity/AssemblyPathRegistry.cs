using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree;
using UnityEditorInternal;
using UnityEngine;

namespace Zenject.ReflectionBaking
{
    public class AssemblyPathRegistry
    {
        static List<string> _assemblies;

        public static List<string> GetAllGeneratedAssemblyRelativePaths()
        {
            if (_assemblies == null)
            {
                _assemblies = LookupAllGeneratedAssemblyPaths();
                Assert.IsNotNull(_assemblies);
            }

            return _assemblies;
        }

        static bool IsManagedAssembly(string systemPath)
        {
            DllType dllType = InternalEditorUtility.DetectDotNetDll(systemPath);
            return dllType != DllType.Unknown && dllType != DllType.Native;
        }

        static List<string> LookupAllGeneratedAssemblyPaths()
        {
            var assemblies = new List<string>(20);

            // We could also add the ones in the project but we probably don't want to edit those
            //FindAssemblies(Application.dataPath, 120, assemblies);

            FindAssemblies(Application.dataPath + "/../Library/ScriptAssemblies/", 2, assemblies);

            return assemblies;
        }

        public static void FindAssemblies(string systemPath, int maxDepth, List<string> result)
        {
            if (maxDepth > 0)
            {
                if (Directory.Exists(systemPath))
                {
                    var dirInfo = new DirectoryInfo(systemPath);

                    result.AddRange(
                        dirInfo.GetFiles().Select(x => x.FullName)
                        .Where(IsManagedAssembly)
                        .Select(ReflectionBakingInternalUtil.ConvertAbsoluteToAssetPath));

                    var directories = dirInfo.GetDirectories();

                    for (int i = 0; i < directories.Length; i++)
                    {
                        DirectoryInfo current = directories[i];

                        FindAssemblies(current.FullName, maxDepth - 1, result);
                    }
                }
            }
        }
    }
}
