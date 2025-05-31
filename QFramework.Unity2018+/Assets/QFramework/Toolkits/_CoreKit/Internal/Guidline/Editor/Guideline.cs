/****************************************************************************
 * Copyright (c) 2016 ~ 2024 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Collections;
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
        
        public class GuidelineItem : GuidelineItemFolder
        {
            public string FilePath;
        }

        public class GuidelineItemFolder
        {
            public string Name;
            public bool Open;
            public List<GuidelineItemFolder> Folders { get; } = new List<GuidelineItemFolder>();
            public int Indent = 0;

            public void DrawGUI(Guideline guideline)
            {
                foreach (var guidelineItemGroup in Folders.OrderBy(g => g.Name))
                {
                    if (guidelineItemGroup is GuidelineItem)
                    {
                        var guidelineItem = guidelineItemGroup as GuidelineItem;

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(Indent * 10);

                        GUILayout.BeginVertical("box");

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(guidelineItem.Name);
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();

                        var rect = GUILayoutUtility.GetLastRect();

                        if (Equals(guideline.mSelectedView, guidelineItem))
                        {
                            GUI.Box(rect, "", mSelectionRect);
                        }

                        if (rect.Contains(Event.current.mousePosition) &&
                            Event.current.type == EventType.MouseUp)
                        {
                            guideline.mSelectedView = guidelineItem;
                            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(guideline.mSelectedView.FilePath);
                            guideline.mMarkdownViewer.UpdateText(textAsset.text);
                            guideline.mMarkdownViewer.MarkdownFilePath = guideline.mSelectedView.FilePath;
                            guideline.mMarkdownViewer.ResetScrollPos();
                            Event.current.Use();
                        }

                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        if (guidelineItemGroup.Folders.Count == 0) continue;
                        GUILayout.BeginHorizontal();
                        if (Indent != 0)
                        {
                            GUILayout.Space((Indent) * 10);
                        }

                        GUILayout.BeginVertical("box");

                        if (EditorGUILayout.Foldout(guidelineItemGroup.Open, guidelineItemGroup.Name,
                                true))
                        {
                            guidelineItemGroup.Open = true;
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();

                            guidelineItemGroup.DrawGUI(guideline);
                        }
                        else
                        {
                            guidelineItemGroup.Open = false;
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                        }
                    }
                }
            }
        }

        private List<GuidelineItem> mViews = null;

        private GuidelineItemFolder mRootFolder = null;

        private VerticalSplitView mSplitView = null;
        private Rect mLeftRect;
        private Rect mRightRect;
        private IMGUILayout mLeftLayout;
        private IMGUILayout mRightLayout;

        private GuidelineItem mSelectedView = null;

        private MDViewer mMarkdownViewer;

        public void Init()
        {
        }

        private EditorCoroutine mRefreshCoroutine = null;

        IEnumerator Refresh(string folderPath,string searchKey)
        {
            mRootFolder.Folders.Clear();

            var searching = searchKey.IsNotNullAndEmpty();
            foreach (var guidelineItem in mViews)
            {
                var pathToRoot = guidelineItem.FilePath.RemoveString(folderPath);

                var names = pathToRoot.Split(Path.DirectorySeparatorChar);

                var currentGroup = mRootFolder;
                var indent = 1;
                foreach (var name in names)
                {
                    if (name.IsNullOrEmpty())
                    {
                        continue;
                    }

                    indent++;
                    if (name.EndsWith(".md"))
                    {
                        if (!searching || name.ToLower().Contains(searchKey.ToLower()))
                        {
                            guidelineItem.Indent = indent;
                            currentGroup.Folders.Add(guidelineItem);
                        }
                    }
                    else
                    {
                        var childGroup = currentGroup.Folders.FirstOrDefault(g => g.Name == name);

                        if (childGroup != null)
                        {
                            currentGroup = childGroup;
                        }
                        else
                        {
                            childGroup = new GuidelineItemFolder()
                            {
                                Name = name,
                                Indent = indent,
                                Open = searching
                            };
                            currentGroup.Folders.Add(childGroup);
                            currentGroup = childGroup;
                        }
                        yield return null;
                    }
                }
            }
        }

        public void OnShow()
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
                    Name = filePath.GetFileName(),
                    FilePath = filePath,
                });
            }


            if (mViews.Count > 0)
            {
                mSelectedView = mViews.First();
                mMarkdownViewer.UpdateText(AssetDatabase.LoadAssetAtPath<TextAsset>(mSelectedView.FilePath).text);
            }

            mRootFolder = new GuidelineItemFolder()
            {
            };
            
            var searchKey = string.Empty;

            if (mRefreshCoroutine != null)
            {
                mRefreshCoroutine.Stop();
                mRefreshCoroutine = null;
            }
            
            mRefreshCoroutine = EditorCoroutine.Start(Refresh(folderPath,searchKey), () =>
            {
                mRefreshCoroutine = null;
            });

            // 创建双屏
            mSplitView = new VerticalSplitView(240)
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
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(LocaleText.Search);
                    searchKey = EditorGUILayout.TextField(searchKey,GUILayout.Height(20));
                    GUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (mRefreshCoroutine != null)
                        {
                            mRefreshCoroutine.Stop();
                            mRefreshCoroutine = null;
                        }
                        
                        mRefreshCoroutine = EditorCoroutine.Start(Refresh(folderPath,searchKey), () =>
                        {
                            mRefreshCoroutine = null;
                        });
                    }

                    scrollPos = GUILayout.BeginScrollView(scrollPos);

                    mRootFolder.DrawGUI(this);

                    GUILayout.EndScrollView();

                    if (GUILayout.Button(LocaleKitEditor.IsCN.Value ? "导出" : "Export"))
                    {
                        var builder = new StringBuilder();

                        void Export(GuidelineItemFolder currentFolder)
                        {
                            foreach (var guidelineItemGroup in currentFolder.Folders)
                            {
                                if (guidelineItemGroup is GuidelineItem)
                                {
                                    var guidelineItem = guidelineItemGroup as GuidelineItem;
                                    var content = File.ReadAllText(guidelineItem.FilePath);
                                    builder.Append(content);
                                    builder.AppendLine();
                                }
                                else
                                {
                                    builder.Append("# " + guidelineItemGroup.Name);
                                    builder.AppendLine();

                                    Export(guidelineItemGroup);
                                }
                            }
                        }

                        Export(mRootFolder);

                        var framework = PackageKit.Interface.GetModel<ILocalPackageVersionModel>()
                            .GetByName("Framework");


                        var guidelineText = LocaleKitEditor.IsCN.Value ? "使用指南 " : "Guideline";

                        var savedPath = EditorUtility.SaveFilePanel($"QFramework {framework.Version} {guidelineText}",
                            Application.dataPath,
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

        public void OnHide()
        {
            EditorApplication.update -= Update;
            mMarkdownViewer = null;
            mCurrentWindow = null;
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
        }
        
        class LocaleText
        {
            public static string Search =>
                LocaleKitEditor.IsCN.Value ? "搜索:" : "Search:";
            
        }
    }
}
#endif