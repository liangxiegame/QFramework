/****************************************************************************
 * Copyright (c) 2020.10 ~ 12 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace QFramework
{
    public class PackageKitWindow : EasyEditorWindow
    {
        class LocaleText
        {
            public static string QFrameworkSettings
            {
                get { return Language.IsChinese ? "QFramework 设置" : "QFramework Settings"; }
            }
        }

        private const float toolbarHeight = 20;

        class Styles
        {
            public static GUIStyle box = "box";
            public static GUIStyle toolbarSeachTextFieldPopup = "ToolbarSeachTextFieldPopup";
            public static GUIStyle searchCancelButton = "ToolbarSeachCancelButton";
            public static GUIStyle searchCancelButtonEmpty = "ToolbarSeachCancelButtonEmpty";
            public static GUIStyle foldout = "Foldout";
            public static GUIStyle toolbarDropDown = "ToolbarDropDown";
            public static GUIStyle selectionRect = "SelectionRect";
        }

        [MenuItem(FrameworkMenuItems.Preferences, false, FrameworkMenuItemsPriorities.Preferences)]
        [MenuItem(FrameworkMenuItems.PackageKit, false, FrameworkMenuItemsPriorities.Preferences)]
        public static void OpenWindow()
        {
            var packageKitWindow = Create<PackageKitWindow>(true);

            if (packageKitWindow.Openning)
            {
                packageKitWindow.Close();
            }
            else
            {
                packageKitWindow.titleContent = new GUIContent(LocaleText.QFrameworkSettings);
                packageKitWindow.position = new Rect(50, 100, 1000, 800);
                packageKitWindow.Open();
            }
        }

        private const string URL_FEEDBACK = "https://qframework.cn/community";

        [MenuItem(FrameworkMenuItems.Feedback, false, FrameworkMenuItemsPriorities.Feedback)]
        private static void Feedback()
        {
            Application.OpenURL(URL_FEEDBACK);
        }


        public class PackageKitViewRenderInfo
        {
            public IPackageKitView Interface { get; private set; }
            
            public string DisplayName { get;  set; }

            public string GroupName { get; set; }

            public int RenderOrder { get;  set; }

            public PackageKitViewRenderInfo(QFramework.IPackageKitView @interface)
            {
                Interface = @interface;

                var displayName = @interface.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), false)
                    .FirstOrDefault() as DisplayNameAttribute;
                DisplayName = displayName != null ? displayName.DisplayName : @interface.GetType().Name;

                var renderOrder = @interface.GetType()
                    .GetCustomAttributes(typeof(PackageKitRenderOrderAttribute), false)
                    .FirstOrDefault() as PackageKitRenderOrderAttribute;

                RenderOrder = renderOrder != null ? renderOrder.Order : int.MaxValue;

                var group = @interface.GetType()
                    .GetCustomAttributes(typeof(PackageKitGroupAttribute), false)
                    .FirstOrDefault() as PackageKitGroupAttribute;

                GroupName = group != null
                    ? group.GroupName
                    : "未分组";
            }
        }

        public Dictionary<MutableTuple<string, bool>, List<PackageKitViewRenderInfo>> mPackageKitViewRenderInfos = null;

        public string SelectedDisplayName
        {
            get { return EditorPrefs.GetString("QF_SELECTED_DISPLAY_NAME", string.Empty); }
            set { EditorPrefs.SetString("QF_SELECTED_DISPLAY_NAME", value); }
        }

        
        [PackageKitIgnore]
        class ReflectRenderInfo : IPackageKitView
        {
            public string AssemblyName;
            public string TypeName;
            public string InitFunc;
            public string OnGUIFunc;
            public string OnWindowGUIEndFunc;
            public string DisposeFunc;
            public string GroupName;
            public string DisplayName;
            public int RenderOrder;

            public override string ToString()
            {
                return AssemblyName;
            }

            private Assembly mAssembly;
            private Type mType;
            private object mRenderObj;
            private MethodInfo mInitFunc;
            private MethodInfo mOnGUIFunc;
            private MethodInfo mOnWindowGUIEndFunc;
            private MethodInfo mDisposeFunc;
            private PropertyInfo mEditorWindowProperty;

            private object[] mEmptyParams = new Object[] { };

            public void Load()
            {
                mAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.FullName.StartsWith(AssemblyName));
                mType = mAssembly.GetTypes().FirstOrDefault(t => t.FullName == TypeName);
                mRenderObj = Activator.CreateInstance(mType);
                mInitFunc = mType.GetMethod(InitFunc, BindingFlags.Instance | BindingFlags.Public);
                mOnGUIFunc = mType.GetMethod(OnGUIFunc, BindingFlags.Instance | BindingFlags.Public);
                
                if (!string.IsNullOrEmpty(OnWindowGUIEndFunc))
                {
                    mOnWindowGUIEndFunc =
                        mType.GetMethod(OnWindowGUIEndFunc, BindingFlags.Instance | BindingFlags.Public);
                }

                mDisposeFunc = mType.GetMethod(DisposeFunc, BindingFlags.Instance | BindingFlags.Public);
                mEditorWindowProperty = mType.GetProperty("EditorWindow", BindingFlags.Instance | BindingFlags.Public);
            }


            public EditorWindow EditorWindow
            {
                get => mEditorWindowProperty?.GetMethod.Invoke(mRenderObj, mEmptyParams) as EditorWindow;
                set
                {
                    mEditorWindowProperty?.SetMethod.Invoke(mRenderObj, new object[] { value });
                }
            }

            public void Init()
            {
                mInitFunc?.Invoke(mRenderObj,mEmptyParams);
            }

            public void OnUpdate()
            {
            }

            public void OnGUI()
            {
                mOnGUIFunc?.Invoke(mRenderObj,mEmptyParams);
            }

            public void OnWindowGUIEnd()
            {
                mOnWindowGUIEndFunc?.Invoke(mRenderObj,mEmptyParams);
            }
            
            public void OnDispose()
            {
                mDisposeFunc?.Invoke(mRenderObj,mEmptyParams);
            }

            public void OnShow()
            {
            }

            public void OnHide()
            {
            }
        }
        
        protected override void Init()
        {
            PackageApplication.Views = null;
            RemoveAllChildren();

            var reflectRenderInfos = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith("packagekitconfig.json")).Select(path =>
            {
                var jsonText = File.ReadAllText(path);

                var infoNew = JsonUtility.FromJson<ReflectRenderInfo>(jsonText);

                infoNew.Load();
                var renderInfo = new PackageKitViewRenderInfo(infoNew);
                renderInfo.DisplayName = infoNew.DisplayName;
                renderInfo.GroupName = infoNew.GroupName;
                renderInfo.RenderOrder = infoNew.RenderOrder;
                renderInfo.Interface.EditorWindow = this;
                renderInfo.Interface.Init();
                return renderInfo;
            });

            mPackageKitViewRenderInfos = PackageApplication.Views
                .Select(view => new PackageKitViewRenderInfo(view))
                .Concat(reflectRenderInfos)
                .GroupBy(renderInfo => renderInfo.GroupName)
                .Select(g => new KeyValuePair<MutableTuple<string, bool>, List<PackageKitViewRenderInfo>>(
                    new MutableTuple<string, bool>(g.Key, true),
                    g.OrderBy(renderInfo => renderInfo.RenderOrder).ToList()))
                .ToDictionary(p => p.Key, p => p.Value);

            if (string.IsNullOrEmpty(SelectedDisplayName))
            {
                mSelectedViewRender = mPackageKitViewRenderInfos.FirstOrDefault().Value.FirstOrDefault();
            }
            else
            {
                mSelectedViewRender = mPackageKitViewRenderInfos
                                          .SelectMany(p => p.Value)
                                          .FirstOrDefault(p => p.DisplayName == SelectedDisplayName) ??
                                      mPackageKitViewRenderInfos.FirstOrDefault().Value.FirstOrDefault();
            }

            mSelectedViewRender.Interface.OnShow();
            

            // 创建双屏
            mSplitView = new VerticalSplitView
            {
                fistPan = rect =>
                {
                    GUILayout.BeginArea(rect);
                    GUILayout.BeginVertical();
                    GUILayout.Space(toolbarHeight);
                    GUILayout.EndVertical();
                    LeftSelectView("");
                    GUILayout.EndArea();
                },
                secondPan = rect =>
                {
                    GUILayout.BeginArea(rect);
                    GUILayout.BeginVertical();
                    GUILayout.Space(toolbarHeight);
                    GUILayout.EndVertical();

                    if (mSelectedViewRender != null)
                    {
                        mSelectedViewRender.Interface.OnGUI();
                    }

                    GUILayout.EndArea();
                }
            };
        }

        private VerticalSplitView mSplitView;


        public override void OnGUI()
        {
            base.OnGUI();

            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();

            var r = GUILayoutUtility.GetLastRect();
            mSplitView.OnGUI(new Rect(new Vector2(0, r.yMax),
                new Vector2(position.width, position.height - r.height)));

            mSelectedViewRender?.Interface?.OnWindowGUIEnd();
            RenderEndCommandExecutor.ExecuteCommand();
        }

        private PackageKitViewRenderInfo mSelectedViewRender = null;

        private void LeftSelectView(string search)
        {
            foreach (var packageKitViewRenderInfo in mPackageKitViewRenderInfos.OrderBy(kv => kv.Key.Item1))
            {
                GUILayout.BeginVertical("box");
                if (EditorGUILayout.Foldout(packageKitViewRenderInfo.Key.Item2, packageKitViewRenderInfo.Key.Item1,
                    true))
                {
                    GUILayout.EndVertical();

                    packageKitViewRenderInfo.Key.Item2 = true;

                    foreach (var drawer in packageKitViewRenderInfo.Value)
                    {
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal("box");
                        GUILayout.Space(20);
                        GUILayout.Label(drawer.DisplayName);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        var rect = GUILayoutUtility.GetLastRect();

                        if (mSelectedViewRender == drawer)
                        {
                            GUI.Box(rect, "", Styles.selectionRect);
                        }

                        if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
                        {
                            mSelectedViewRender.Interface.OnHide();
                            mSelectedViewRender = drawer;
                            mSelectedViewRender.Interface.OnShow();
                            SelectedDisplayName = mSelectedViewRender.DisplayName;
                            Event.current.Use();
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    packageKitViewRenderInfo.Key.Item2 = false;
                    GUILayout.EndVertical();
                }
            }
        }


        public override void OnClose()
        {
            if (mPackageKitViewRenderInfos != null)
            {
                mPackageKitViewRenderInfos.SelectMany(r => r.Value)
                    .Where(view => view != null)
                    .ToList()
                    .ForEach(view => view.Interface.OnDispose());
            }

            RemoveAllChildren();
        }

        public override void OnUpdate()
        {
            if (mSelectedViewRender != null)
            {
                mSelectedViewRender.Interface.OnUpdate();
            }
        }
    }
}