/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [DisplayName("API 文档")]
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(10)]
    internal class APIDoc : IPackageKitView
    {
        public EditorWindow EditorWindow { get; set; }
        
        public void Init()
        {
            
        }

        public List<Type> mTypes = new List<Type>();
        
        public void OnShow()
        {
            foreach (var type in PackageKitAssemblyCache.GetAllTypes())
            {
                var classAPIAttribute = type.GetFirstAttribute<ClassAPIAttribute>(false);

                if (classAPIAttribute != null)
                {
                    mTypes.Add(type);
                }
            }
        }


        public void OnUpdate()
        {
        }

        public void OnGUI()
        {
            foreach (var type in mTypes)
            {
                GUILayout.Label(type.Name);
            }
        }

        public void OnWindowGUIEnd()
        {
        }
        
        public void OnHide()
        {
            mTypes.Clear();
        }

        public void OnDispose()
        {
        }


    }
}
#endif
