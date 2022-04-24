using System;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace UnityEditor.Purchasing
{
    public class MigrationCleanupTooling : EditorWindow
    {
        private const string k_MigrateCatalogButtonText = "Move Product Catalog Files";
        private const string k_MigrateBillingModeButtonText = "Move Billing Mode Files";
        private const string k_MigrateObfuscationsButtonText = "Move Obfuscator Files";
        private const string k_RemoveAssetStoreButtonText = "Uninstall Asset Store Package";
        private const string k_AssetStorePath = "Assets/Plugins/UnityPurchasing";
        private const string k_AssetStoreMetaFile = "Assets/Plugins/UnityPurchasing.meta";
        private const string k_AssetStoreBackupPath = "Assets/Plugins/UnityPurchasing~";

        private const string k_AssetStoreBackupMessage = "If you have made modifications directly to any file within the Asset Store Plugin, you can find a backup here: Assets/Plugins/UnityPurchasing~.\nIf you are directly using any of the Demo assets or code, please install the Samples of In-App Purchasing version 3 from the Package Manager UI.";

        private const string k_CatalogPath = "Assets/Resources/IAPProductCatalog.json";
        private const string k_PrevCatalogPath = "Assets/Plugins/UnityPurchasing/Resources/IAPProductCatalog.json";
        private const string k_BillingModePath = "Assets/Resources/BillingMode.json";
        private const string k_PrevBillingModePath = "Assets/Plugins/UnityPurchasing/Resources/BillingMode.json";
        private const string k_PrevObfuscatorPath = "Assets/Plugins/UnityPurchasing/generated";
        private const string k_BackupObfuscatorPath = "Assets/Plugins/UnityPurchasing~/generated";
        private const string k_BadObfuscatorPath = "Assets/Resources/UnityPurchasing/generated";
        private const string k_ObfuscatorPath = "Assets/Scripts/UnityPurchasing/generated";
        private const string k_ObfuscationClassSuffix = "Tangle.cs";

#if !UNITY_UNIFIED_IAP

#if UNITY_2020_1_OR_NEWER
        private const string k_GotoPackageUpdateText = "Go To Purchasing Settings";
#elif UNITY_2019_3_OR_NEWER
        private const string k_GotoPackageUpdateText = "Go To Package Manager";
#else
        private const string k_GotoPackageUpdateText = "Open the Package Manager and manually update your In-App Purchasing package";
#endif

#if UNITY_2020_1_OR_NEWER
        private const string k_IapSettingsPath = "Project/Services/In-App Purchasing";
#endif

#endif // !UNITY_UNIFIED_IAP

        private const string k_IapPackageName = "com.unity.purchasing";

        private static string m_LatestPackageVersion;
        private static SearchRequest m_SearchRequest;

        /// <summary>
        /// String used to specify the path for an item in the menu bar to access the Migration UI.
        /// </summary>
        public const string CleanIapForMigrationMenuPath = "Window/Unity IAP/Clean Libraries for Migration to IAP Version 3";

        /// <summary>
        /// Validation function to flag Migration UI accessibility from the menu bar.
        /// </summary>
        [MenuItem(CleanIapForMigrationMenuPath, true)]
        public static bool CleanIapForMigrationValidation()
        {
            if (string.IsNullOrEmpty(m_LatestPackageVersion))
            {
                if (m_SearchRequest == null || m_SearchRequest.IsCompleted)
                {
                    CheckLatestPackageVersion();
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Opens the Migration UI in an editor window.
        /// </summary>
        [MenuItem(CleanIapForMigrationMenuPath, false, 999)]
        public static void CleanIapForMigration()
        {
            EditorWindow.GetWindow(typeof(MigrationCleanupTooling));
        }

        void OnGUI()
        {
            int latestMajorVer = 0;
            if (!string.IsNullOrEmpty(m_LatestPackageVersion) && TryGetMajorVersion(m_LatestPackageVersion, out latestMajorVer))
            {
                if (latestMajorVer >= 3)
                {
                    OnMigrateGUI();
                    return;
                }
            }

            OnDisclamerGUI();
        }

        void OnDisclamerGUI()
        {
            const string k_Disclaimer =
                "This window exists to provide support to update your In-App Purchasing package to version 3 or higher.\nIf you are reading this, this means this version is not publicly available yet.\nVersion 3 will encompass what is currently in the Asset Store plugin. As such, you will need to safely remove the plugin before installing that.\nThis menu item will provide features faciliating this. Please return here at that time.";
            var style = new GUIStyle(EditorStyles.label);
            style.wordWrap = true;
            GUILayout.Label(k_Disclaimer, style);
        }

        void OnMigrateGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            bool needRemoveAssetStore = Directory.Exists(k_AssetStorePath);
            bool needMigrateCatalog = needRemoveAssetStore && DoesPrevCatalogPathExist() && !DoesNewCatalogPathExist();
            bool needMigrateBilling = needRemoveAssetStore && DoesPrevBillingModePathExist() && !DoesNewBillingModePathExist();
            bool needMigrateObfuscations = needRemoveAssetStore && IsObfuscationMigrationNeeded();

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            GUILayout.Label("Product Catalog Migrated: *{" + !needMigrateCatalog + "}*");
            GUILayout.Label("Billing Mode Migrated: *{" + !needMigrateBilling + "}*");
            GUILayout.Label("Obfuscator Files Migrated: *{" + !needMigrateObfuscations + "}*");
            if (!needMigrateBilling && !needMigrateCatalog && !needMigrateObfuscations)
            {
                EditorGUILayout.Space();
                GUILayout.Label("Asset store Package Removed: *{" + !needRemoveAssetStore + "}*");
#if !UNITY_UNIFIED_IAP
                if (!needRemoveAssetStore)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label("It is now safe to migrate to version 3:");
#if !UNITY_2019_3_OR_NEWER
                    var style = new GUIStyle(EditorStyles.label);
                    style.wordWrap = true;
                    GUILayout.Label(k_GotoPackageUpdateText, style); //No quick links to Packman available in this version. Show instructions instead
#endif
                }
#endif
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical();
            if (needMigrateCatalog)
            {
                if (GUILayout.Button(k_MigrateCatalogButtonText))
                {
                    MigrateProductCatalog();
                }

            }
            else
            {
                GUILayout.Label("Step Complete");
            }

            if (needMigrateBilling)
            {
                if (GUILayout.Button(k_MigrateBillingModeButtonText))
                {
                    MigrateBillingMode();
                }
            }
            else
            {
                GUILayout.Label("Step Complete");
            }

            if (needMigrateObfuscations)
            {
                if (GUILayout.Button(k_MigrateObfuscationsButtonText))
                {
                    MigrateObfuscations();
                }
            }
            else
            {
                GUILayout.Label("Step Complete");
            }

            if (!needMigrateBilling && !needMigrateCatalog && !needMigrateObfuscations)
            {
                EditorGUILayout.Space();
                if (needRemoveAssetStore)
                {
                    if (GUILayout.Button(k_RemoveAssetStoreButtonText))
                    {
                        UninstallAssetStorePackage();
                    }
                }
                else
                {
                    GUILayout.Label("Step Complete");

#if !UNITY_UNIFIED_IAP && UNITY_2019_3_OR_NEWER
                    EditorGUILayout.Space();
                    if (GUILayout.Button(k_GotoPackageUpdateText))
                    {
#if UNITY_2020_1_OR_NEWER
                        SettingsService.OpenProjectSettings(k_IapSettingsPath);
#else
                        PackageManager.UI.Window.Open(k_IapPackageName);
#endif
                        Close();
                    }
#endif
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (!needRemoveAssetStore && Directory.Exists(k_AssetStoreBackupPath))
            {
                EditorGUILayout.Space();
                var style = new GUIStyle(EditorStyles.label);
                style.wordWrap = true;
                GUILayout.Label(k_AssetStoreBackupMessage, style);
            }
            GUILayout.EndVertical();
        }

        private void UninstallAssetStorePackage()
        {
            Directory.CreateDirectory(k_AssetStoreBackupPath);

            try
            {
                if (File.Exists(k_BackupObfuscatorPath + ".meta"))
                {
                    FileUtil.ReplaceDirectory(k_BackupObfuscatorPath, k_PrevObfuscatorPath);
                    FileUtil.DeleteFileOrDirectory(k_BackupObfuscatorPath);

                    FileUtil.ReplaceFile(k_BackupObfuscatorPath + ".meta", k_PrevObfuscatorPath + ".meta");
                    FileUtil.DeleteFileOrDirectory(k_BackupObfuscatorPath + ".meta");
                }

                if (Directory.Exists(k_AssetStoreBackupPath))
                {
                    Directory.Delete(k_AssetStoreBackupPath);
                }

                FileUtil.MoveFileOrDirectory(k_AssetStorePath, k_AssetStoreBackupPath);
                FileUtil.DeleteFileOrDirectory(k_AssetStoreMetaFile);
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        [InitializeOnLoadMethod]
        private static void CheckLatestPackageVersion()
        {
            m_LatestPackageVersion = string.Empty;

            // Look for a specific package
            m_SearchRequest = Client.Search(k_IapPackageName);
            EditorApplication.update += SearchPackageProgress;
        }

        private static void SearchPackageProgress()
        {
            if (m_SearchRequest.IsCompleted)
            {
                EditorApplication.update -= SearchPackageProgress;
                if (m_SearchRequest.Status == StatusCode.Success)
                {
                    foreach (var package in m_SearchRequest.Result)
                    {
                        if (package.name.Equals(k_IapPackageName))
                        {
                            m_LatestPackageVersion = package.versions.compatible.Reverse().FirstOrDefault();
                        }
                    }
                }
                else if (m_SearchRequest.Status >= StatusCode.Failure)
                {
                    Debug.LogError(m_SearchRequest.Error.message);
                }
            }
        }

        private bool TryGetMajorVersion(string versionName, out int majorVersion)
        {
            return int.TryParse(versionName.Split('.')[0], out majorVersion);
        }

        private static void MigrateResourceFile(string newPath, string oldPath)
        {
            try
            {
                FileInfo file = new FileInfo(newPath);
                file.Directory.Create();

                if (File.Exists(newPath))
                {
                    return;
                }

                if (File.Exists(oldPath))
                {
                    AssetDatabase.CopyAsset(oldPath, newPath);
                }

            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

        }

        private static void MigrateProductCatalog()
        {
            MigrateResourceFile(k_CatalogPath, k_PrevCatalogPath);
        }

        private static bool DoesPrevCatalogPathExist()
        {
            return File.Exists(k_PrevCatalogPath);
        }

        private static bool DoesNewCatalogPathExist()
        {
            return File.Exists(k_CatalogPath);
        }

        private static void MigrateBillingMode()
        {
            MigrateResourceFile(k_BillingModePath, k_PrevBillingModePath);
        }

        private static bool DoesPrevBillingModePathExist()
        {
            return File.Exists(k_PrevBillingModePath);
        }

        private static bool DoesNewBillingModePathExist()
        {
            return File.Exists(k_BillingModePath);
        }

        private static void MigrateObfuscations()
        {
            try
            {
                if (DoPrevObfuscationFilesExist())
                {
                    CopyObfuscatorFiles(k_PrevObfuscatorPath);
                    ArchiveObfuscatorFiles(k_PrevObfuscatorPath, k_BackupObfuscatorPath);
                }

                if (DoBadObfuscationFilesExist())
                {
                    CopyObfuscatorFiles(k_BadObfuscatorPath);
                    ArchiveObfuscatorFiles(k_BadObfuscatorPath, k_BackupObfuscatorPath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static void CopyObfuscatorFiles(string oldObfuscatorPath)
        {
            Directory.CreateDirectory(k_ObfuscatorPath);

            foreach (var oldFile in Directory.GetFiles(oldObfuscatorPath))
            {
                var fileName = Path.GetFileName(oldFile);
                if (fileName.EndsWith(k_ObfuscationClassSuffix))
                {
                    var newFile = k_ObfuscatorPath + "/" + fileName;

                    if (File.Exists(newFile))
                    {
                        break;
                    }

                    AssetDatabase.CopyAsset(oldFile, newFile);
                }
            }
        }

        private static void ArchiveObfuscatorFiles(string oldObfuscatorPath, string archivedObfuscatorPath)
        {
            Directory.CreateDirectory(k_BackupObfuscatorPath);

            try
            {
                FileUtil.ReplaceDirectory(oldObfuscatorPath, archivedObfuscatorPath);
                FileUtil.DeleteFileOrDirectory(oldObfuscatorPath);

                FileUtil.ReplaceFile(oldObfuscatorPath + ".meta", archivedObfuscatorPath + ".meta");
                FileUtil.DeleteFileOrDirectory(oldObfuscatorPath + ".meta");

                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static bool DoPrevObfuscationFilesExist()
        {
            return (Directory.Exists(k_PrevObfuscatorPath) && (Directory.GetFiles(k_PrevObfuscatorPath).Length > 0));
        }

        private static bool DoBadObfuscationFilesExist()
        {
            return (Directory.Exists(k_BadObfuscatorPath) && (Directory.GetFiles(k_BadObfuscatorPath).Length > 0));
        }

        private static bool IsObfuscationMigrationNeeded()
        {
            return (DoPrevObfuscationFilesExist() && FindUncopiedOldObfucscatorFiles(k_PrevObfuscatorPath)) ||
                   (DoBadObfuscationFilesExist() && FindUncopiedOldObfucscatorFiles(k_BadObfuscatorPath));
        }

        private static bool FindUncopiedOldObfucscatorFiles(string oldObfuscatorPath)
        {
            foreach (var oldFile in Directory.GetFiles(oldObfuscatorPath))
            {
                var fileName = Path.GetFileName(oldFile);
                if (fileName.EndsWith(k_ObfuscationClassSuffix))
                {
                    var newFile = k_ObfuscatorPath + "/" + fileName;

                    if (!File.Exists(newFile))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
