/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace QFramework
{
    [PackageKitIgnore]
    internal class ReflectRenderInfo : IPackageKitView
    {
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
            return mType.FullName;
        }

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

        
        public bool Load()
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


        public EditorWindow EditorWindow
        {
            get => mEditorWindowProperty?.GetMethod.Invoke(mRenderObj, mEmptyParams) as EditorWindow;
            set { mEditorWindowProperty?.SetMethod.Invoke(mRenderObj, new object[] { value }); }
        }
        

        

        public void Init()
        {
            mInitFunc?.Invoke(mRenderObj, mEmptyParams);
        }

        public void OnUpdate()
        {
            mOnUpdateFunc?.Invoke(mRenderObj, mEmptyParams);
        }

        public void OnGUI()
        {
            mOnGUIFunc?.Invoke(mRenderObj, mEmptyParams);
        }

        public void OnWindowGUIEnd()
        {
            mOnWindowGUIEndFunc?.Invoke(mRenderObj, mEmptyParams);
        }

        public void OnDispose()
        {
            mDisposeFunc?.Invoke(mRenderObj, mEmptyParams);
        }

        public void OnShow()
        {
            mOnShowFunc?.Invoke(mRenderObj, mEmptyParams);
        }

        public void OnHide()
        {
            mOnHideFunc?.Invoke(mRenderObj, mEmptyParams);
        }
    }
}
#endif