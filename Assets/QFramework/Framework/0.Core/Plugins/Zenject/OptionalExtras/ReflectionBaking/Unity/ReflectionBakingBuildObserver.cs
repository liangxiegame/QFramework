using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ModestTree;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Zenject.ReflectionBaking.Mono.Cecil;
using Debug = UnityEngine.Debug;

namespace Zenject.ReflectionBaking
{
    public static class ReflectionBakingBuildObserver
    {
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompiled;
        }

        static void OnAssemblyCompiled(string assemblyAssetPath, CompilerMessage[] messages)
        {
#if !UNITY_2018
            if (Application.isEditor && !BuildPipeline.isBuildingPlayer)
            {
                return;
            }
#endif

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WSAPlayer)
            {
                Log.Warn("Zenject reflection baking skipped because it is not currently supported on WSA platform!");
            }
            else
            {
                TryWeaveAssembly(assemblyAssetPath);
            }
        }

        static void TryWeaveAssembly(string assemblyAssetPath)
        {
            var settings = ReflectionBakingInternalUtil.TryGetEnabledSettingsInstance();

            if (settings == null)
            {
                return;
            }

            if (settings.AllGeneratedAssemblies && settings.ExcludeAssemblies.Contains(assemblyAssetPath))
            {
                return;
            }

            if (!settings.AllGeneratedAssemblies && !settings.IncludeAssemblies.Contains(assemblyAssetPath))
            {
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var assemblyFullPath = ReflectionBakingInternalUtil.ConvertAssetPathToSystemPath(assemblyAssetPath);

            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = new UnityAssemblyResolver(),
                // Is this necessary?
                //ReadSymbols = true,
            };

            var module = ModuleDefinition.ReadModule(assemblyFullPath, readerParameters);

            var assemblyRefNames = module.AssemblyReferences.Select(x => x.Name.ToLower()).ToList();

            if (!assemblyRefNames.Contains("zenject-usage"))
            {
                // Zenject-usage is used by the generated methods
                // Important that we do this check otherwise we can corrupt some dlls that don't have access to it
                return;
            }

            var assemblyName = Path.GetFileNameWithoutExtension(assemblyAssetPath);
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.GetName().Name == assemblyName).OnlyOrDefault();

            Assert.IsNotNull(assembly, "Could not find unique assembly '{0}' in currently loaded list of assemblies", assemblyName);

            int numTypesChanged = ReflectionBakingModuleEditor.WeaveAssembly(
                module, assembly, settings.NamespacePatterns);

            if (numTypesChanged > 0)
            {
                var writerParams = new WriterParameters()
                {
                    // Is this necessary?
                    //WriteSymbols = true
                };

                module.Write(assemblyFullPath, writerParams);

                Debug.Log("Added reflection baking to '{0}' types in assembly '{1}', took {2:0.00} seconds"
                    .Fmt(numTypesChanged, Path.GetFileName(assemblyAssetPath), stopwatch.Elapsed.TotalSeconds));
            }
        }
    }
}
