/****************************************************************************
 * Copyright (c) 2016 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class Guideline
    {
        public class GuidelineItem
        {
            public string FolderName;
            public string FileName;
            public string FilePath;
        }

        public class GuidelineItemGroup
        {
            public string FolderName;
            public bool Open;
            public List<GuidelineItem> Items { get; set; }

            public bool IsRoot;
        }

        private List<GuidelineItem> mViews = null;
        private List<GuidelineItemGroup> mGroups = null;

        private VerticalSplitView mSplitView = null;
        private Rect mLeftRect;
        private Rect mRightRect;
        private IMGUILayout mLeftLayout;
        private IMGUILayout mRightLayout;

        private GuidelineItem mSelectedView = null;

        private MDViewer mMarkdownViewer;

        public void Init()
        {
            EditorApplication.update += Update;
            mViews = new List<GuidelineItem>();

            var positionMarkForLoad = Resources.Load<TextAsset>("EditorGuideline/PositionMarkForLoad");

            var path = AssetDatabase.GetAssetPath(positionMarkForLoad);
            var folderPath = path.GetFolderPath();
            var folderName = folderPath.GetFileName();
            var markdownFilePaths = Directory.GetFiles(folderPath, "*.md", SearchOption.AllDirectories);

            mMarkdownViewer = new MDViewer(Resources.Load<GUISkin>("Skin/MarkdownSkinQS"), path, "");

            foreach (var filePath in markdownFilePaths)
            {
                mViews.Add(new GuidelineItem()
                {
                    FileName = filePath.GetFileNameWithoutExtend(),
                    FolderName = filePath.GetFolderPath().GetFileName(),
                    FilePath = filePath,
                });
            }


            if (mViews.Count > 0)
            {
                mSelectedView = mViews.First();
                mMarkdownViewer.UpdateText(AssetDatabase.LoadAssetAtPath<TextAsset>(mSelectedView.FilePath).text);
            }

            mGroups = mViews.GroupBy(v => v.FolderName).OrderBy(g =>
            {
                var number = g.Key.Split('.').First();
                if (int.TryParse(number, out var order))
                {
                    return order;
                }

                return -1;
            }).Select(g => new GuidelineItemGroup()
            {
                FolderName = g.Key,
                IsRoot = g.Key == folderName,
                Items = g.ToList()
            }).ToList();

            // 创建双屏
            mSplitView = new VerticalSplitView(180)
            {
                FirstPan = rect =>
                {
                    mLeftRect = rect;
                    mLeftLayout.DrawGUI();
                },
                SecondPan = rect =>
                {
                    mRightRect = rect;
                    mRightLayout.DrawGUI();
                }
            };

            var scrollPos = Vector2.zero;

            mLeftLayout = EasyIMGUI.Area().WithRectGetter(() => mLeftRect)
                .AddChild(EasyIMGUI.Custom().OnGUI(() =>
                {
                    GUILayout.BeginHorizontal();


                    GUILayout.BeginVertical();
                    GUILayout.Space(20);
                    GUILayout.EndVertical();

                    if (mSplitView.Expand.Value)
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("<"))
                        {
                            mSplitView.Expand.Value = false;
                        }
                    }

                    GUILayout.EndHorizontal();
                }))
                .AddChild(EasyIMGUI.Custom().OnGUI(() =>
                {
                    scrollPos = GUILayout.BeginScrollView(scrollPos);

                    foreach (var guidelineItemGroup in mGroups)
                    {
                        if (guidelineItemGroup.IsRoot)
                        {
                            foreach (var guidelineItem in guidelineItemGroup.Items)
                            {
                                GUILayout.BeginVertical("box");

                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label(guidelineItem.FileName);
                                    GUILayout.FlexibleSpace();
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.EndVertical();

                                var rect = GUILayoutUtility.GetLastRect();

                                if (Equals(mSelectedView, guidelineItem))
                                {
                                    GUI.Box(rect, "", mSelectionRect);
                                }

                                if (rect.Contains(Event.current.mousePosition) &&
                                    Event.current.type == EventType.MouseUp)
                                {
                                    mSelectedView = guidelineItem;
                                    var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(mSelectedView.FilePath);
                                    mMarkdownViewer.UpdateText(textAsset.text);
                                    mMarkdownViewer.MarkdownFilePath = mSelectedView.FilePath;
                                    mMarkdownViewer.ResetScrollPos();
                                    Event.current.Use();
                                }
                            }
                        }
                        else
                        {
                            GUILayout.BeginVertical("box");

                            if (EditorGUILayout.Foldout(guidelineItemGroup.Open, guidelineItemGroup.FolderName,
                                    true))
                            {
                                guidelineItemGroup.Open = true;
                                GUILayout.EndVertical();

                                foreach (var guidelineItem in guidelineItemGroup.Items)
                                {
                                    GUILayout.BeginVertical("box");

                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.Space(20);
                                        GUILayout.Label(guidelineItem.FileName);
                                        GUILayout.FlexibleSpace();
                                    }
                                    GUILayout.EndHorizontal();

                                    GUILayout.EndVertical();

                                    var rect = GUILayoutUtility.GetLastRect();

                                    if (Equals(mSelectedView, guidelineItem))
                                    {
                                        GUI.Box(rect, "", mSelectionRect);
                                    }

                                    if (rect.Contains(Event.current.mousePosition) &&
                                        Event.current.type == EventType.MouseUp)
                                    {
                                        mSelectedView = guidelineItem;
                                        var textAsset =
                                            AssetDatabase.LoadAssetAtPath<TextAsset>(mSelectedView.FilePath);
                                        mMarkdownViewer.UpdateText(textAsset.text);
                                        mMarkdownViewer.MarkdownFilePath = mSelectedView.FilePath;
                                        mMarkdownViewer.ResetScrollPos();
                                        Event.current.Use();
                                    }
                                }
                            }
                            else
                            {
                                guidelineItemGroup.Open = false;
                                GUILayout.EndVertical();
                            }
                        }
                    }


                    GUILayout.EndScrollView();

                    if (GUILayout.Button(LocaleKitEditor.IsCN.Value ? "导出" : "Export"))
                    {
                        var builder = new StringBuilder();
                        foreach (var guidelineItemGroup in mGroups)
                        {
                            builder.Append("# " + guidelineItemGroup.FolderName);
                            builder.AppendLine();
                            foreach (var guidelineItem in guidelineItemGroup.Items)
                            {
                                var content = File.ReadAllText(guidelineItem.FilePath);
                                builder.Append(content);
                                builder.AppendLine();
                            }
                        }

                        var framework = PackageKit.Interface.GetModel<ILocalPackageVersionModel>().GetByName("Framework");


                        var guidelineText = LocaleKitEditor.IsCN.Value ? "使用指南 " : "Guideline";
                        
                        var savedPath = EditorUtility.SaveFilePanel($"QFramework {framework.Version} {guidelineText}", Application.dataPath,
                            $"QFramework {framework.Version} {guidelineText}", "md");

                        File.WriteAllText(savedPath, builder.ToString());

                        EditorUtility.RevealInFinder(savedPath);
                    }

                    GUILayout.Space(5);
                }));


            mRightLayout = EasyIMGUI.Area().WithRectGetter(() => mRightRect)
                .AddChild(EasyIMGUI.Custom().OnGUI(() =>
                {
                    GUILayout.BeginHorizontal();


                    if (!mSplitView.Expand.Value)
                    {
                        if (GUILayout.Button(">"))
                        {
                            mSplitView.Expand.Value = true;
                        }

                        GUILayout.FlexibleSpace();
                    }

                    GUILayout.BeginVertical();
                    GUILayout.Space(20);
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }))
                .AddChild(EasyIMGUI.Custom().OnGUI(() =>
                {
                    var lastRect = GUILayoutUtility.GetLastRect();
                    mMarkdownViewer.DrawWithRect(new Rect(lastRect.x, lastRect.y + lastRect.height + 5,
                        mRightRect.width - 220, mRightRect.height - lastRect.y - lastRect.height));
                }));

            mCurrentWindow = EditorWindow.focusedWindow;
        }

        private void Update()
        {
            if (mMarkdownViewer != null && mMarkdownViewer.Update())
            {
                mCurrentWindow.Repaint();
            }
        }

        private static GUIStyle mSelectionRect = "SelectionRect";

        private EditorWindow mCurrentWindow;

        public void OnGUI()
        {
            var r = GUILayoutUtility.GetLastRect();
            mSplitView.OnGUI(new Rect(new Vector2(0, r.yMax),
                new Vector2(mCurrentWindow.position.width, mCurrentWindow.position.height - r.height)));
        }

        public void OnDestroy()
        {
            EditorApplication.update -= Update;
            mMarkdownViewer = null;
            mCurrentWindow = null;
        }
    }
}
#endif