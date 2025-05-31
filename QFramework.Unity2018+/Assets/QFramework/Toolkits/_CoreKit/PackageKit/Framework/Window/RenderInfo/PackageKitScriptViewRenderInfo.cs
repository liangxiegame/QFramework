/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace QFramework
{
    [PackageKitIgnore]
    internal class PackageKitScriptViewRenderInfo : IPackageKitView
    {
        public string Language;
        public string LuaFileName;
        public string AssemblyName;
        public string TypeName;
        public string InitFunc;
        public string OnGUIFunc;
        public string OnShowFunc;
        public string OnHideFunc;
        public string OnUpdateFunc;
        public string OnWindowGUIEndFunc;
        public string DisposeFunc;
        public string GroupName;
        public string DisplayName;
        public string DisplayNameCN;
        public string DisplayNameEN;
        public int RenderOrder;

        public override string ToString()
        {
            if (Language == "lua")
            {
                return LuaFileName;
            }

            return mType.FullName;
        }

        #region C#

        private Assembly mAssembly;
        private Type mType;
        private object mRenderObj;
        private MethodInfo mInitFunc;
        private MethodInfo mOnGUIFunc;
        private MethodInfo mOnWindowGUIEndFunc;
        private MethodInfo mOnUpdateFunc;
        private MethodInfo mOnShowFunc;
        private MethodInfo mOnHideFunc;
        private MethodInfo mDisposeFunc;
        private PropertyInfo mEditorWindowProperty;

        private object[] mEmptyParams = new Object[] { };

        #endregion

        #region Lua

        private Script mScript;
        private DynValue mLuaTable;

        #endregion

        public bool Load()
        {
            if (Language == "lua")
            {
                return true;
            }
            else
            {
                mAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(assembly => assembly.FullName.StartsWith(AssemblyName));
                mType = mAssembly?.GetTypes().FirstOrDefault(t => t.FullName == TypeName);
                if (mType == null) return false;
                mRenderObj = Activator.CreateInstance(mType);
                mInitFunc = mType.GetMethod(InitFunc, BindingFlags.Instance | BindingFlags.Public);
                mOnGUIFunc = mType.GetMethod(OnGUIFunc, BindingFlags.Instance | BindingFlags.Public);

                if (OnShowFunc.IsNotNullAndEmpty())
                {
                    mOnShowFunc = mType.GetMethod(OnShowFunc, BindingFlags.Instance | BindingFlags.Public);
                }


                if (OnHideFunc.IsNotNullAndEmpty())
                {
                    mOnHideFunc = mType.GetMethod(OnHideFunc, BindingFlags.Instance | BindingFlags.Public);
                }


                if (OnUpdateFunc.IsNotNullAndEmpty())
                {
                    mOnUpdateFunc = mType.GetMethod(OnUpdateFunc, BindingFlags.Instance | BindingFlags.Public);
                }


                if (OnWindowGUIEndFunc.IsNotNullAndEmpty())
                {
                    mOnWindowGUIEndFunc =
                        mType.GetMethod(OnWindowGUIEndFunc, BindingFlags.Instance | BindingFlags.Public);
                }


                mDisposeFunc = mType.GetMethod(DisposeFunc, BindingFlags.Instance | BindingFlags.Public);
                mEditorWindowProperty = mType.GetProperty("EditorWindow", BindingFlags.Instance | BindingFlags.Public);
                return true;
            }
        }

        public EditorWindow EditorWindow
        {
            get => mEditorWindowProperty?.GetMethod.Invoke(mRenderObj, mEmptyParams) as EditorWindow;
            set { mEditorWindowProperty?.SetMethod.Invoke(mRenderObj, new object[] { value }); }
        }

        public void Init()
        {
            if (Language == "lua")
            {
            }
            else
            {
                mInitFunc?.Invoke(mRenderObj, mEmptyParams);
            }
        }

        public void OnUpdate()
        {
            if (Language == "lua")
            {
            }
            else
            {
                mOnUpdateFunc?.Invoke(mRenderObj, mEmptyParams);
            }
        }

        public void OnGUI()
        {
            if (Language == "lua")
            {
                if (mScript != null && mLuaTable != null && mLuaTable.Table["OnGUI"] != null)
                {
                    mScript.Call(mLuaTable.Table["OnGUI"]);
                }
            }
            else
            {
                mOnGUIFunc?.Invoke(mRenderObj, mEmptyParams);
            }
        }

        public void OnWindowGUIEnd()
        {
            if (Language == "lua")
            {
            }
            else
            {
                mOnWindowGUIEndFunc?.Invoke(mRenderObj, mEmptyParams);
            }
        }

        public void OnDispose()
        {
            if (Language == "lua")
            {
            }
            else
            {
                mDisposeFunc?.Invoke(mRenderObj, mEmptyParams);
            }
        }

        public void OnShow()
        {
            if (Language == "lua")
            {
                var scriptFilePath = AssetDatabase.FindAssets($"{LuaFileName} t:TextAsset")
                    .Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault(p => p.EndsWith(".lua"));

                if (scriptFilePath == null) return;
                var scriptText = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptFilePath);
                if (!scriptText) return;
                mScript = new Script();
                mScript.RegisterQFrameworkEditorTypes();
                mScript.Options.DebugPrint = Debug.Log;
                mScript.Options.ScriptLoader = new UnityEditorScriptLoader(); 
                mLuaTable = mScript.DoString(scriptText.text);

                if (mLuaTable != null && mLuaTable.Table["OnShow"] != null)
                {
                    mScript.Call(mLuaTable.Table["OnShow"]);
                }
            }
            else
            {
                mOnShowFunc?.Invoke(mRenderObj, mEmptyParams);
            }
        }

        public void OnHide()
        {
            if (Language == "lua")
            {
                if (mScript != null && mLuaTable != null && mLuaTable.Table["OnHide"] != null)
                {
                    mScript.Call(mLuaTable.Table["OnHide"]);
                    mLuaTable = null;
                }

                mScript = null;
            }
            else
            {
                mOnHideFunc?.Invoke(mRenderObj, mEmptyParams);
            }
        }
    }
}
#endif