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
    [DisplayNameCN("API 文档")]
    [DisplayNameEN("API Doc")]
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(0)]
    internal class APIDoc : IPackageKitView, IUnRegisterList
    {
        public EditorWindow EditorWindow { get; set; }
        private ClassAPIRenderInfo mSelectionClassAPIRenderInfo;

        private List<ClassAPIGroupRenderInfo> mGroupRenderInfos = new List<ClassAPIGroupRenderInfo>();

        private MDViewer mMDViewer;


        public Type Type { get; } = typeof(APIDoc);

        public void Init()
        {
            var skin = Resources.Load<GUISkin>("Skin/MarkdownSkinQS");
            mMDViewer = new MDViewer(skin, string.Empty, "");

            Vector2 scrollPos = Vector2.zero;
            mSplitView = new VerticalSplitView(240)
            {
                FirstPan = (rect) =>
                {
                    GUILayout.BeginArea(rect);
                    mSplitView.DrawExpandButtonLeft();

                    scrollPos = GUILayout.BeginScrollView(scrollPos);
                    foreach (var groupRenderInfo in mGroupRenderInfos)
                    {
                        GUILayout.BeginVertical("box");
                        if (EditorGUILayout.Foldout(groupRenderInfo.Open.Value, groupRenderInfo.GroupName, true))
                        {
                            groupRenderInfo.Open.Value = true;
                            GUILayout.EndVertical();
                            foreach (var classAPIRenderInfo in groupRenderInfo.ClassAPIRenderInfos)
                            {
                                GUILayout.BeginVertical("box");
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(20); // indent
                                GUILayout.Label(classAPIRenderInfo.DisplayMenuName);
                                GUILayout.EndHorizontal();
                                GUILayout.EndVertical();

                                IMGUIHelper.LastRectSelectionCheck(classAPIRenderInfo,
                                    mSelectionClassAPIRenderInfo,
                                    () =>
                                    {
                                        mSelectionClassAPIRenderInfo = classAPIRenderInfo;
                                        UpdateDoc();
                                    });
                            }
                        }
                        else
                        {
                            groupRenderInfo.Open.Value = false;
                            GUILayout.EndVertical();
                        }
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndArea();
                },
                SecondPan = (rect) =>
                {
                    GUILayout.BeginArea(rect);
                    mSplitView.DrawExpandButtonRight();
                    GUILayout.Space(8);

                    if (mSelectionClassAPIRenderInfo != null)
                    {
                        var lastRect = GUILayoutUtility.GetLastRect();
                        mMDViewer.DrawWithRect(new Rect(lastRect.x, lastRect.y + lastRect.height,
                            rect.width - 210, rect.height - lastRect.y - lastRect.height));
                    }


                    GUILayout.EndArea();
                },
            };
        }

        private VerticalSplitView mSplitView;

        private static GUIStyle mSelectionRect = "SelectionRect";

        private APIDocLocale mLocaleText = new APIDocLocale();

        void UpdateDoc()
        {
            mSelectionClassAPIRenderInfo.Parse();
            new StringBuilder()
                .Append("#### **").Append(mSelectionClassAPIRenderInfo.ClassName).AppendLine("**")
                .AppendLine()
                .Append("class in ").AppendLine(mSelectionClassAPIRenderInfo.Namespace)
                .AppendLine()
                // Description
                .Append("**").Append(APIDocLocale.Description).AppendLine("**")
                .Append("> ").AppendLine(mSelectionClassAPIRenderInfo.Description)
                .AppendLine()
                // ExampleCode
                .Self(builder =>
                {
                    if (mSelectionClassAPIRenderInfo.ExampleCode.IsNotNullAndEmpty())
                    {
                        builder
                            .Append("**").Append(APIDocLocale.ExampleCode).AppendLine("**")
                            .AppendLine()
                            .AppendLine("```")
                            .AppendLine(mSelectionClassAPIRenderInfo.ExampleCode)
                            .AppendLine("```");
                    }
                })
                .AppendLine()
                // Properties
                .Self(builder =>
                {
                    if (mSelectionClassAPIRenderInfo.Properties.Any())
                    {
                        builder
                            .Append("**").Append(APIDocLocale.Properties).AppendLine("**")
                            .AppendLine()
                            .AppendLine("--------");


                        foreach (var property in mSelectionClassAPIRenderInfo.Properties)
                        {
                            builder.AppendLine()
                                .Self(property.BuildString);
                        }
                    }
                })
                .AppendLine()
                // Methods
                .Self(builder =>
                {
                    if (mSelectionClassAPIRenderInfo.Methods.Any())
                    {
                        builder
                            .Append("**").Append(APIDocLocale.Methods).AppendLine("**")
                            .AppendLine()
                            .AppendLine("--------");


                        foreach (var method in mSelectionClassAPIRenderInfo.Methods)
                        {
                            builder.AppendLine()
                                .Self(method.BuildString);
                        }
                    }
                })
                .ToString()
                .Self(mMDViewer.UpdateText);
        }

        public void OnShow()
        {
            LocaleKitEditor.IsCN.Register(_ => { UpdateDoc(); }).AddToUnregisterList(this);

            mGroupRenderInfos.Clear();

            mGroupRenderInfos = PackageKitAssemblyCache.GetAllTypes()
                .Where(t => t.HasAttribute<ClassAPIAttribute>())
                .Select(t => new ClassAPIRenderInfo(t, t.GetAttribute<ClassAPIAttribute>(false)))
                .GroupBy(c => c.GroupName)
                .OrderBy(c => c.Key)
                .Select(g => new ClassAPIGroupRenderInfo(g.Key)
                {
                    ClassAPIRenderInfos = g.OrderBy(c => c.RenderOrder).ToList()
                }).ToList();


            if (mGroupRenderInfos.Count > 0)
            {
                var firstGroup = mGroupRenderInfos.First();
                firstGroup.Open.Value = true;

                mSelectionClassAPIRenderInfo = firstGroup.ClassAPIRenderInfos.First();
                UpdateDoc();
            }
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
            mGroupRenderInfos.Clear();
        }

        public void OnDispose()
        {
            this.UnRegisterAll();
            mMDViewer = null;
        }

        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
    }
}
#endif