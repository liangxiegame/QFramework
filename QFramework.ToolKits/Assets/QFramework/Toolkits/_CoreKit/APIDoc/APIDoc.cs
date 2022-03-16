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
using System.Linq;
using System.Text;
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
            var skin = Resources.Load<GUISkin>("Skin/MarkdownSkinQS");
            mMDViewer = new MDViewer(skin, "", "");
        }

        private List<Type> mTypes = new List<Type>();
        private VerticalSplitView mSplitView;

        private static GUIStyle mSelectionRect = "SelectionRect";

        private Type mSelectionType;

        private MDViewer mMDViewer;

        void UpdateDoc()
        {
            new StringBuilder()
                .AppendLine("* 命名空间:" + mSelectionType.Namespace)
                .AppendLine("* 类名:" + mSelectionType.Name)
                .AppendLine("* 描述:" + mSelectionType.GetFirstAttribute<APIDescriptionCNAttribute>(false).Description)
                .ToString()
                .Self(mMDViewer.UpdateText);
        }

        public void OnShow()
        {
            mTypes.Clear();
            foreach (var type in PackageKitAssemblyCache.GetAllTypes())
            {
                var classAPIAttribute = type.GetFirstAttribute<ClassAPIAttribute>(false);

                if (classAPIAttribute != null)
                {
                    mTypes.Add(type);
                }
            }

            if (mTypes.Count > 0)
            {
                mSelectionType = mTypes.First();
                UpdateDoc();
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
                            () =>
                            {
                                mSelectionType = type;
                                UpdateDoc();
                            });
                    }

                    GUILayout.EndArea();
                },
                SecondPan = (rect) =>
                {
                    GUILayout.BeginArea(rect);
                    mSplitView.DrawExpandButtonRight();

                    if (mSelectionType != null)
                    {
                        var lastRect = GUILayoutUtility.GetLastRect();
                        mMDViewer.DrawWithRect(new Rect(lastRect.x, lastRect.y + lastRect.height,
                            rect.width - 210, rect.height - lastRect.y - lastRect.height));
                    }


                    GUILayout.EndArea();
                },
            };
        }


        public void OnUpdate()
        {
            mMDViewer.Update();
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
            mMDViewer = null;
        }
    }
}
#endif