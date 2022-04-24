#define NON_COMPILED_UPM

using UnityEditor.Purchasing;
using UnityEditor.Build;
using System;

namespace UnityEditor
{
    [InitializeOnLoad]
    internal class PurchasingImporter
    {
        static PurchasingImporter()
        {
#if !UNITY_UNIFIED_IAP && !NON_COMPILED_UPM
            var importer = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath("b5f4343795a0e4626ac1fe4a9e6fce59" /* UnityEngine.Purchasing.dll */)) as PluginImporter;
            if (importer)
            {
                bool compatible = BuildPipeline.IsFeatureSupported("ENABLE_CLOUD_SERVICES_PURCHASING", EditorUserBuildSettings.activeBuildTarget);
                importer.SetCompatibleWithEditor(compatible);
                importer.SetCompatibleWithAnyPlatform(compatible);
#else
            {
#endif

                PurchasingSettings.ApplyEnableSettings(EditorUserBuildSettings.activeBuildTarget);

#if !UNITY_UNIFIED_IAP
                BuildDefines.getScriptCompilationDefinesDelegates += (target, defines) =>
                {
                    if (PurchasingSettings.enabledForPlatform)
                    {
                        defines.Add("UNITY_PURCHASING");
                    }
                };
#endif
            }
        }
    }
}
