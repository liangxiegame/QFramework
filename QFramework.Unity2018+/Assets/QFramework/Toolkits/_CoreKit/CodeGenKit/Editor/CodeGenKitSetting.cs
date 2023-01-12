/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class CodeGenKitSetting : ScriptableObject
    {
        public bool IsUseNamespace = true;
        public bool IsDefaultNamespace => Namespace == "QFramework.Example";
        public string Namespace = "QFramework.Example";
        public string ScriptDir = "Assets/Scripts/Game";
        public string PrefabDir = "Assets/Art/Prefab";

        private static CodeGenKitSetting mInstance;

        public static CodeGenKitSetting Load()
        {
            if (mInstance) return mInstance;

            var filePath = Dir.Value + FileName;

            if (File.Exists(filePath))
            {
                mInstance = AssetDatabase.LoadAssetAtPath<CodeGenKitSetting>(filePath);
                var jsonString = File.ReadAllText(filePath);
                if (mInstance != null && jsonString != "" && !jsonString.Contains("IsUseNamespace"))
                {
                    mInstance.IsUseNamespace = true;
                }
                return mInstance;
            }

            return mInstance = CreateInstance<CodeGenKitSetting>();
        }

        public static string LoadSettingNamespace()
        {
            var cgks = Load();
            if (cgks.IsUseNamespace)
            {
                if (string.IsNullOrEmpty(cgks.Namespace))
                {
                    return "QFramework.Example";
                }
                return cgks.Namespace;
            }
            return "";
        }

        public void Save()
        {
            var filePath = Dir.Value + FileName;
            if (!File.Exists(filePath))
            {
                AssetDatabase.CreateAsset(this, Dir.Value + FileName);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static readonly Lazy<string> Dir =
            new Lazy<string>(() => "Assets/QFrameworkData/CodeGenKit/".CreateDirIfNotExists());

        private const string FileName = "Setting.asset";
    }
}
#endif