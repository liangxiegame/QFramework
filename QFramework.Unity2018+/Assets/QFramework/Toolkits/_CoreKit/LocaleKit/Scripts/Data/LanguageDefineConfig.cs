/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace QFramework
{
    public class LanguageDefineConfig : ScriptableObject
    {
        public List<LanguageDefine> LanguageDefines = new List<LanguageDefine>()
        {
            new LanguageDefine()
            {
                Language = Language.English
            },
            new LanguageDefine()
            {
                Language = Language.ChineseSimplified
            }
        };

#if UNITY_EDITOR
        private static readonly Lazy<string> Dir =
            new Lazy<string>(() => "Assets/QFrameworkData/LocaleKit/Resources/".CreateDirIfNotExists());

        private const string FileName = "LanguageDefineConfig.asset";

        // private static LanguageDefineConfig mInstance;

        public static LanguageDefineConfig Default
        {
            get
            {
                // if (mInstance) return mInstance;

                var filePath = Dir.Value + FileName;

                if (File.Exists(filePath))
                {
                    return AssetDatabase.LoadAssetAtPath<LanguageDefineConfig>(filePath);
                }
                else
                {
                    var retValue = CreateInstance<LanguageDefineConfig>();

                    retValue.Save();


                    return retValue;
                }
            }
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
#endif
    }
}