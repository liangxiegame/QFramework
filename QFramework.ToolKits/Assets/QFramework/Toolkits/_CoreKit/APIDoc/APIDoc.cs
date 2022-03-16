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

        private List<Type> mTypes = new List<Type>();
        private VerticalSplitView mSplitView;

        private static GUIStyle mSelectionRect = "SelectionRect";

        private Type mSelectionType;

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

            mSplitView = new VerticalSplitView(240)
            {
                FirstPan = (rect) =>
                {
                    GUILayout.BeginArea(rect);
                    mSplitView.DrawExpandButtonLeft();

                    foreach (var type in mTypes)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical("box");
                        GUILayout.Label(type.Name);
                        GUILayout.EndVertical();
                        GUILayout.Space(5); // padding
                        GUILayout.EndHorizontal();

                        IMGUIGestureHelper.LastRectSelectionCheck(type, mSelectionType,
                            () => { mSelectionType = type; });
                    }

                    GUILayout.EndArea();
                },
                SecondPan = (rect) =>
                {
                    GUILayout.BeginArea(rect);
                    mSplitView.DrawExpandButtonRight();

                    if (mSelectionType != null)
                    {
                        GUILayout.Label(mSelectionType.FullName);
                    }

                    GUILayout.EndArea();
                },
            };
        }


        public void OnUpdate()
        {
        }

        public void OnGUI()
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            mSplitView?.OnGUI(new Rect(new Vector2(0, lastRect.yMax),
                new Vector2(EditorWindow.position.width, EditorWindow.position.height - lastRect.height)));
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