/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class UnityEditorScriptLoader : ScriptLoaderBase
    {
        public UnityEditorScriptLoader()
        {
            IgnoreLuaPathGlobal = true;
        }
        
        public override string ResolveModuleName(string modname, Table globalContext)
        {
            return modname;
        }
        
        public override bool ScriptFileExists(string name)
        {
            Debug.Log(name);
            if (name.EndsWith(".lua"))
            {
                return AssetDatabase.FindAssets($@"{name.RemoveString(".lua")} t:TextAsset")
                    .Select(AssetDatabase.GUIDToAssetPath).Any(p => p.EndsWith(name));
            }

            return AssetDatabase.FindAssets($@"{name} t:TextAsset")
                .Select(AssetDatabase.GUIDToAssetPath).Any(p => p.EndsWith(name + ".lua"));
        }

        public override object LoadFile(string file, Table globalContext)
        {
            if (file.EndsWith(".lua"))
            {
                return AssetDatabase.FindAssets($@"{file.RemoveString(".lua")} t:TextAsset")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(p => p.EndsWith(file))
                    .Select(AssetDatabase.LoadAssetAtPath<TextAsset>).First().text;
            }

            return AssetDatabase.FindAssets($@"{file} t:TextAsset")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => p.EndsWith(file + ".lua"))
                .Select(AssetDatabase.LoadAssetAtPath<TextAsset>)
                .First().text;
        }
    }
}
#endif