/****************************************************************************
 * Copyright (c) 2016 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    internal static class User
    {
        public static BindableProperty<string> Username = new BindableProperty<string>(LoadString("username"));
        public static BindableProperty<string> Password = new BindableProperty<string>(LoadString("password"));
        public static BindableProperty<string> Token = new BindableProperty<string>(LoadString("token"));

        public static bool Logined
        {
            get
            {
                return !string.IsNullOrEmpty(Token.Value) &&
                       !string.IsNullOrEmpty(Username.Value) &&
                       !string.IsNullOrEmpty(Password.Value);
            }
        }


        public static void Save()
        {
            Username.SaveString("username");
            Password.SaveString("password");
            Token.SaveString("token");
        }

        public static void Clear()
        {
            Username.Value = string.Empty;
            Password.Value = string.Empty;
            Token.Value = string.Empty;
            Save();
        }

        public static void SaveString(this BindableProperty<string> selfProperty, string key)
        {
            EditorPrefs.SetString(key, selfProperty.Value);
        }


        public static string LoadString(string key)
        {
            return EditorPrefs.GetString(key, string.Empty);
        }
    }


    public class ReadmeWindow : EditorWindow
    {
        private Readme mReadme;

        private Vector2 mScrollPos = Vector2.zero;

        private PackageVersion mPackageVersion;


        public static void Init(Readme readme, PackageVersion packageVersion)
        {
            var readmeWin = (ReadmeWindow)GetWindow(typeof(ReadmeWindow), true, packageVersion.Name, true);
            readmeWin.mReadme = readme;
            readmeWin.mPackageVersion = packageVersion;
            readmeWin.position = new Rect(Screen.width / 2, Screen.height / 2, 600, 300);
            readmeWin.Show();
        }

        public void OnGUI()
        {
            mScrollPos = GUILayout.BeginScrollView(mScrollPos, true, true, GUILayout.Width(580), GUILayout.Height(300));

            GUILayout.Label("类型:" + mPackageVersion.Type);

            mReadme.items.ForEach(item =>
            {
                EasyIMGUI
                    .Custom()
                    .OnGUI(() =>
                    {
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();

                        GUILayout.Label("version: " + item.version, GUILayout.Width(130));
                        GUILayout.Label("author: " + item.author);
                        GUILayout.Label("date: " + item.date);

                        if (item.author == User.Username.Value || User.Username.Value == "liangxie")
                        {
                            if (GUILayout.Button("删除"))
                            {
//                            RenderEndCommandExecuter.PushCommand(() =>
//                            {
                                new PackageManagerServer().DeletePackage(item.PackageId,
                                    () => { mReadme.items.Remove(item); });
//                            });
                            }
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Label(item.content);
                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();
                    }).DrawGUI();
            });

            GUILayout.EndScrollView();
        }
    }

    [Serializable]
    public class PackageInfosRequestCache
    {
        public List<PackageRepository> PackageRepositories = new List<PackageRepository>();

        private static string mFilePath
        {
            get
            {
                var dirPath = Application.dataPath + "/.qframework/PackageManager/";

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                return dirPath + "PackageInfosRequestCache.json";
            }
        }

        public static PackageInfosRequestCache Get()
        {
            if (File.Exists(mFilePath))
            {
                var cacheJson = File.ReadAllText(mFilePath);

                if (cacheJson.IsTrimNotNullAndEmpty())
                {
                    return new PackageInfosRequestCache();
                }
                try
                {
                    var retValue = JsonUtility.FromJson<PackageInfosRequestCache>(cacheJson);

                    if (retValue.PackageRepositories == null)
                    {
                        return new PackageInfosRequestCache();
                    }
                }
                catch (Exception)
                {
                    return new PackageInfosRequestCache();
                }
            }

            return new PackageInfosRequestCache();
        }

        public void Save()
        {
            File.WriteAllText(mFilePath, JsonUtility.ToJson(this));
        }
    }


    public static class FrameworkMenuItems
    {
        public const string Preferences = "QFramework/Preferences... %e";
        public const string PackageKit = "QFramework/PackageKit... %#e";

        public const string Feedback = "QFramework/Feedback";
    }

    public static class FrameworkMenuItemsPriorities
    {
        public const int Preferences = 1;

        public const int Feedback = 11;
    }
    

    public class SubWindow : EditorWindow, IMGUILayout
    {
        public string Id { get; set; }
        public bool Visible { get; set; }

        public Func<bool> VisibleCondition { get; set; }

        void IMGUIView.DrawGUI()
        {
        }

        IMGUILayout IMGUIView.Parent { get; set; }

        private FluentGUIStyle mStyle = new FluentGUIStyle(() => new GUIStyle());

        public FluentGUIStyle Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        Color IMGUIView.BackgroundColor { get; set; }


        private List<IMGUIView> mPrivateChildren = new List<IMGUIView>();

        private List<IMGUIView> mChildren
        {
            get { return mPrivateChildren; }
            set { mPrivateChildren = value; }
        }

        void IMGUIView.RefreshNextFrame()
        {
        }

        void IMGUIView.AddLayoutOption(GUILayoutOption option)
        {
        }

        void IMGUIView.RemoveFromParent()
        {
        }

        void IMGUIView.Refresh()
        {
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public IMGUILayout AddChild(IMGUIView view)
        {
            mChildren.Add(view);
            view.Parent = this;
            return this;
        }

        public void RemoveChild(IMGUIView view)
        {
            mChildren.Add(view);
            view.Parent = null;
        }

        public void Clear()
        {
            mChildren.Clear();
        }

        private void OnGUI()
        {
            mChildren.ForEach(view => view.DrawGUI());
        }

        public void Dispose()
        {
        }
    }

    public abstract class Window : EditorWindow, IDisposable
    {
        public IMGUIViewController ViewController { get; set; }


        private void OnGUI()
        {
            if (ViewController != null)
            {
                ViewController.View.DrawGUI();
            }

            RenderEndCommandExecutor.ExecuteCommand();
        }

        public void Dispose()
        {
            OnDispose();
        }


        protected abstract void OnDispose();
    }


    public static class MouseSelector
    {
        public static string GetSelectedPathOrFallback()
        {
            var path = string.Empty;

            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                }
            }

            return path;
        }
    }


    internal class ColorView : IMGUIAbstractView
    {
        public ColorView(Color color)
        {
            Color = new BindableProperty<Color>(color);
        }

        public BindableProperty<Color> Color { get; private set; }

        protected override void OnGUI()
        {
            Color.Value = EditorGUILayout.ColorField(Color.Value, LayoutStyles);
        }
    }


    internal class EnumPopupView : IMGUIAbstractView
    {
        public BindableProperty<Enum> ValueProperty { get; set; }

        public EnumPopupView(Enum initValue)
        {
            ValueProperty = new BindableProperty<Enum>(initValue);
            ValueProperty.Value = initValue;
            Style = new FluentGUIStyle(() => EditorStyles.popup);
        }

        protected override void OnGUI()
        {
            Enum enumType = ValueProperty.Value;
            ValueProperty.Value = EditorGUILayout.EnumPopup(enumType, Style.Value, LayoutStyles);
        }
    }


    public abstract class IMGUIViewController
    {
        public VerticalLayout View = new VerticalLayout();

        public abstract void SetUpView();
    }
}
#endif