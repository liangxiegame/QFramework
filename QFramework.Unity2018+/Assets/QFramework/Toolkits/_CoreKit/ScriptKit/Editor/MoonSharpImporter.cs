/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.IO;
#if UNITY_2020_1_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using UnityEngine;

namespace QFramework
{
    [ScriptedImporter(1, ".lua")]
    public class LuaImporter : ScriptedImporter
    {
        private string mLuaText;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (string.IsNullOrEmpty(mLuaText))
                mLuaText = File.ReadAllText(ctx.assetPath);

            var display = new TextAsset(mLuaText);
            ctx.AddObjectToAsset("Lua Script", display);
            ctx.SetMainObject(display);
        }
    }
}
#endif
