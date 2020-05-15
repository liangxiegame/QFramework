using QFramework.PackageKit;
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace QFramework
{
    
    /// <summary>
    /// some net work util
    /// </summary>
    public static class Network
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>IP string</returns>
        public static string GetAddressIP()
        {
            var AddressIP = "";
#if !UNITY_WEBGL
#if UNITY_3 || UNITY_4 || UNITY_5 || UNITY_2017 || UNITY_2018_0
            AddressIP = UnityEngine.Network.player.ipAddress;
#else
            //获取本地的IP地址  
            foreach (System.Net.IPAddress _IPAddress in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
#endif
#endif

#if UNITY_IPHONE
            System.Net.NetworkInformation.NetworkInterface[] adapters =  System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces(); ;  
            foreach (System.Net.NetworkInformation.NetworkInterface adapter in adapters)  
            {  
                if (adapter.Supports( System.Net.NetworkInformation.NetworkInterfaceComponent.IPv4))  
                {  
                    System.Net.NetworkInformation.UnicastIPAddressInformationCollection uniCast = adapter.GetIPProperties().UnicastAddresses;  
                    if (uniCast.Count > 0)  
                    {  
                        foreach ( System.Net.NetworkInformation.UnicastIPAddressInformation uni in uniCast)  
                        {  
                            //得到IPv4的地址。 AddressFamily.InterNetwork指的是IPv4  
                            if (uni.Address.AddressFamily ==  System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                AddressIP = uni.Address.ToString();
                            }
                        }  
                    }  
                }  
            }
#endif
            return AddressIP;
        }

        public static bool IsReachable
        {
            get { return Application.internetReachability != NetworkReachability.NotReachable; }
        }
    }

    [InitializeOnLoad]
    public class PackageCheck
    {
        enum CheckStatus
        {
            WAIT,
            COMPARE,
            NONE
        }

        private CheckStatus mCheckStatus;

        private double mNextCheckTime = 0;

        private double mCheckInterval = 60;


        static PackageCheck()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && Network.IsReachable)
            {
                PackageCheck packageCheck = new PackageCheck()
                {
                    mCheckStatus = CheckStatus.WAIT,
                    mNextCheckTime = EditorApplication.timeSinceStartup,
                };

                EditorApplication.update = packageCheck.CustomUpdate;
            }
        }

        private void CustomUpdate()
        {
            // 添加网络判断
            if (!Network.IsReachable) return;

            switch (mCheckStatus)
            {
                case CheckStatus.WAIT:
                    if (EditorApplication.timeSinceStartup >= mNextCheckTime)
                    {
                        mCheckStatus = CheckStatus.COMPARE;
                    }

                    break;

                case CheckStatus.COMPARE:

                    ProcessCompare();

                    break;
            }
        }


        private void GoToWait()
        {
            mCheckStatus = CheckStatus.WAIT;

            mNextCheckTime = EditorApplication.timeSinceStartup + mCheckInterval;
        }

        private bool ReCheckConfigDatas()
        {
            mCheckInterval = 60;

            return true;
        }

        private void ProcessCompare()
        {
            if (Network.IsReachable)
            {
                new PackageManagerServer().GetAllRemotePackageInfoV5((packageDatas,res) =>
                {
                    if (packageDatas == null)
                    {
                        return;
                    }

                    if (new PackageManagerModel().VersionCheck)
                    {
                        CheckNewVersionDialog(packageDatas, PackageInfosRequestCache.Get().PackageRepositories);
                    }
                });
            }

            ReCheckConfigDatas();
            GoToWait();
        }

        private static bool CheckNewVersionDialog(List<PackageRepository> requestPackageDatas,
            List<PackageRepository> cachedPackageDatas)
        {
            foreach (var requestPackageData in requestPackageDatas)
            {
                var cachedPacakgeData =
                    cachedPackageDatas.Find(packageData => packageData.name == requestPackageData.name);

                var installedPackageVersion = InstalledPackageVersions.Get()
                    .Find(packageVersion => packageVersion.Name == requestPackageData.name);

                if (installedPackageVersion == null)
                {
                }
                else if (cachedPacakgeData == null &&
                         requestPackageData.VersionNumber > installedPackageVersion.VersionNumber ||
                         cachedPacakgeData != null && requestPackageData.Installed &&
                         requestPackageData.VersionNumber > cachedPacakgeData.VersionNumber &&
                         requestPackageData.VersionNumber > installedPackageVersion.VersionNumber)
                {
                    ShowDisplayDialog(requestPackageData.name);
                    return false;
                }
            }

            return true;
        }


        private static void ShowDisplayDialog(string packageName)
        {
            var result = EditorUtility.DisplayDialog("PackageManager",
                string.Format("{0} 有新版本更新,请前往查看(如需不再提示请点击前往查看，并取消勾选 Version Check)", packageName),
                "前往查看", "稍后查看");

            if (result)
            {
                EditorApplication.ExecuteMenuItem(FrameworkMenuItems.Preferences);
            }
        }
    }

    public static class User
    {
        public static Property<string> Username = new Property<string>(LoadString("username"));
        public static Property<string> Password = new Property<string>(LoadString("password"));
        public static Property<string> Token    = new Property<string>(LoadString("token"));

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

        public static void SaveString(this Property<string> selfProperty, string key)
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
            var readmeWin = (ReadmeWindow) GetWindow(typeof(ReadmeWindow), true, packageVersion.Name, true);
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
                new CustomView(() =>
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

    public static class InstallPackage
    {
        public static void Do(PackageRepository requestPackageData)
        {
            var tempFile = "Assets/" + requestPackageData.name + ".unitypackage";

            Debug.Log(requestPackageData.latestDownloadUrl + ">>>>>>:");

            EditorUtility.DisplayProgressBar("插件更新", "插件下载中 ...", 0.1f);

            EditorHttp.Download(requestPackageData.latestDownloadUrl, response =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    File.WriteAllBytes(tempFile, response.Bytes);

                    EditorUtility.ClearProgressBar();

                    AssetDatabase.ImportPackage(tempFile, false);

                    File.Delete(tempFile);
                    
                    AssetDatabase.Refresh();

                    Debug.Log("PackageManager:插件下载成功");

                    InstalledPackageVersions.Reload();
                }
                else
                {
                    EditorUtility.ClearProgressBar();

                    EditorUtility.DisplayDialog(requestPackageData.name,
                        "插件安装失败,请联系 liangxiegame@163.com 或者加入 QQ 群:623597263" + response.Error + ";", "OK");
                }
            }, OnProgressChanged);
        }

        private static void OnProgressChanged(float progress)
        {
            EditorUtility.DisplayProgressBar("插件更新",
                string.Format("插件下载中 {0:P2}", progress), progress);
        }
    }

    public class InstalledPackageVersions
    {
        private static List<PackageVersion> mPackageVersions = new List<PackageVersion>();

        public static List<PackageVersion> Get()
        {
            if (mPackageVersions.Count == 0)
            {
                Reload();
            }

            return mPackageVersions;
        }

        public static void Reload()
        {
            mPackageVersions.Clear();

            var versionFiles = Array.FindAll(AssetDatabase.GetAllAssetPaths(),
                name => name.EndsWith("PackageVersion.json"));

            foreach (var fileName in versionFiles)
            {
                var text = File.ReadAllText(fileName);
                mPackageVersions.Add(JsonUtility.FromJson<PackageVersion>(text));
            }
        }

        public static PackageVersion FindVersionByName(string name)
        {
            return Get().Find(installedPackageVersion => installedPackageVersion.Name == name);
        }
    }



 

   

    public interface IEditorStrangeMVCCommand
    {
        void Execute();
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
                return JsonUtility.FromJson<PackageInfosRequestCache>(File.ReadAllText(mFilePath));
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
        public const string PackageKit  = "QFramework/PackageKit... %#e";

        public const string Feedback = "QFramework/Feedback";
    }

    public static class FrameworkMenuItemsPriorities
    {
        public const int Preferences = 1;

        public const int Feedback = 11;
    }

    public interface IPackageKitView
    {
        IQFrameworkContainer Container { get; set; }

        /// <summary>
        /// 1 after 0
        /// </summary>
        int RenderOrder { get; }

        bool Ignore { get; }

        bool Enabled { get; }

        void Init(IQFrameworkContainer container);

        void OnUpdate();
        void OnGUI();

        void OnDispose();
    }

    public class PackageKitWindow : IMGUIEditorWindow
    {
        class LocaleText
        {
            public static string QFrameworkSettings
            {
                get { return Language.IsChinese ? "QFramework 设置" : "QFramework Settings"; }
            }
        }

        public static bool IsOpenening = false;

        [MenuItem(FrameworkMenuItems.Preferences, false, FrameworkMenuItemsPriorities.Preferences)]
        [MenuItem(FrameworkMenuItems.PackageKit, false, FrameworkMenuItemsPriorities.Preferences)]
        private static void Open()
        {
            var packageKitWindow = Create<PackageKitWindow>(true);
            packageKitWindow.titleContent = new GUIContent(LocaleText.QFrameworkSettings);
            packageKitWindow.position = new Rect( 50, 100, 800, 800);
            packageKitWindow.Show();
            IsOpenening = true;
        }

        private const string URL_FEEDBACK = "http://feathub.com/liangxiegame/QFramework";

        [MenuItem(FrameworkMenuItems.Feedback, false, FrameworkMenuItemsPriorities.Feedback)]
        private static void Feedback()
        {
            Application.OpenURL(URL_FEEDBACK);
        }

        public override void OnUpdate()
        {
            mPackageKitViews.ForEach(view => view.OnUpdate());
        }

        public List<IPackageKitView> mPackageKitViews = null;

        private int count = 0;

        protected override void Init()
        {
            var label = GUI.skin.label;

            PackageApplication.Container = null;
            RemoveAllChidren();

            BindKit.Init();

            mPackageKitViews = PackageApplication.Container
                .ResolveAll<IPackageKitView>()
                .OrderBy(view => view.RenderOrder)
                .ToList();

            PackageApplication.Container.RegisterInstance(this);
            PackageApplication.Container.RegisterInstance<EditorWindow>(this);
        }

        public override void OnGUI()
        {
            base.OnGUI();
            mPackageKitViews.ForEach(view => view.OnGUI());

            RenderEndCommandExecuter.ExecuteCommand();
        }

        public override void OnClose()
        {
            if (mPackageKitViews != null)
            {
                mPackageKitViews.Where(view => view != null).ToList().ForEach(view => view.OnDispose());
            }

            RemoveAllChidren();
            
            BindKit.Clear();

            IsOpenening = false;
        }
    }

   

    public interface IEventManager
    {
        System.Action AddListener(object listener);
        void Signal(Action<object> obj);
    }

    public class EventManager<T> : IEventManager where T : class
    {
        private List<T> _listeners;

        public List<T> Listeners
        {
            get { return _listeners ?? (_listeners = new List<T>()); }
            set { _listeners = value; }
        }

        public void Signal(Action<object> obj)
        {
            foreach (var item in Listeners)
            {
                var item1 = item;
                obj(item1);
            }
        }

        public void Signal(Action<T> action)
        {
            foreach (var item in Listeners)
            {
                //InvertApplication.Log(typeof(T).Name + " was signaled on " + item.GetType().Name);
                var item1 = item;
                action(item1);
            }
        }

        public System.Action Subscribe(T listener)
        {
            if (!Listeners.Contains(listener))
                Listeners.Add(listener);

            return () => { Unsubscribe(listener); };
        }

        private void Unsubscribe(T listener)
        {
            Listeners.Remove(listener);
        }

        public System.Action AddListener(object listener)
        {
            return Subscribe(listener as T);
        }
    }

    public interface ISystemResetEvents
    {
        void SystemResetting();
        void SystemRestarted();
    }

    public class Language
    {
        public static bool IsChinese
        {
            get
            {
                return Application.systemLanguage == SystemLanguage.Chinese ||
                       Application.systemLanguage == SystemLanguage.ChineseSimplified;
            }
        }
    }

    public abstract class IMGUIEditorWindow : EditorWindow
    {
        public static T Create<T>(bool utility, string title = null) where T : IMGUIEditorWindow
        {
            return string.IsNullOrEmpty(title) ? GetWindow<T>(utility) : GetWindow<T>(utility, title);
        }

        private readonly List<IView> mChildren = new List<IView>();

        private bool mVisible = true;

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        public void AddChild(IView childView)
        {
            mChildren.Add(childView);
        }

        public void RemoveChild(IView childView)
        {
            mChildren.Remove(childView);
        }

        public List<IView> Children
        {
            get { return mChildren; }
        }

        public void RemoveAllChidren()
        {
            mChildren.Clear();
        }

        public abstract void OnClose();


        public abstract void OnUpdate();

        private void OnDestroy()
        {
            OnClose();
        }

        protected abstract void Init();

        private bool mInited = false;

        public virtual void OnGUI()
        {
            if (!mInited)
            {
                Init();
                mInited = true;
            }

            OnUpdate();

            if (Visible)
            {
                mChildren.ForEach(childView => childView.DrawGUI());
            }
        }
    }

    public class SubWindow : EditorWindow, ILayout
    {
        void IView.Hide()
        {
        }

        void IView.DrawGUI()
        {
        }

        ILayout IView.Parent { get; set; }

        private GUIStyle mStyle = null;

        public GUIStyle Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        Color IView.BackgroundColor { get; set; }


        private List<IView> mPrivateChildren = new List<IView>();

        private List<IView> mChildren
        {
            get { return mPrivateChildren; }
            set { mPrivateChildren = value; }
        }

        void IView.RefreshNextFrame()
        {
        }

        void IView.AddLayoutOption(GUILayoutOption option)
        {
        }

        void IView.RemoveFromParent()
        {
        }

        void IView.Refresh()
        {
        }

        public ILayout AddChild(IView view)
        {
            mChildren.Add(view);
            view.Parent = this;
            return this;
        }

        public void RemoveChild(IView view)
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


    public abstract class View : IView
    {
        private bool mVisible = true;

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        private List<GUILayoutOption> mprivateLayoutOptions = new List<GUILayoutOption>();

        private List<GUILayoutOption> mLayoutOptions
        {
            get { return mprivateLayoutOptions; }
        }

        protected GUILayoutOption[] LayoutStyles { get; private set; }


        private GUIStyle mStyle = new GUIStyle();

        public GUIStyle Style
        {
            get { return mStyle; }
            protected set { mStyle = value; }
        }

        private Color mBackgroundColor = GUI.backgroundColor;

        public Color BackgroundColor
        {
            get { return mBackgroundColor; }
            set { mBackgroundColor = value; }
        }

        public void RefreshNextFrame()
        {
            this.PushCommand(Refresh);
        }

        public void AddLayoutOption(GUILayoutOption option)
        {
            mLayoutOptions.Add(option);
        }

        public void Show()
        {
            Visible = true;
            OnShow();
        }

        protected virtual void OnShow()
        {
        }

        public void Hide()
        {
            Visible = false;
            OnHide();
        }

        protected virtual void OnHide()
        {
        }


        private Color mPreviousBackgroundColor;

        public void DrawGUI()
        {
            BeforeDraw();

            if (Visible)
            {
                mPreviousBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = BackgroundColor;
                OnGUI();
                GUI.backgroundColor = mPreviousBackgroundColor;
            }
        }

        private bool mBeforeDrawCalled = false;

        void BeforeDraw()
        {
            if (!mBeforeDrawCalled)
            {
                OnBeforeDraw();

                LayoutStyles = mLayoutOptions.ToArray();

                mBeforeDrawCalled = true;
            }
        }

        protected virtual void OnBeforeDraw()
        {
        }

        public ILayout Parent { get; set; }

        public void RemoveFromParent()
        {
            Parent.RemoveChild(this);
        }

        public virtual void Refresh()
        {
            OnRefresh();
        }

        protected virtual void OnRefresh()
        {
        }

        protected abstract void OnGUI();

        public void Dispose()
        {
            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
        }
    }

    public abstract class Window : EditorWindow, IDisposable
    {
        public static Window MainWindow { get; protected set; }

        public IMGUIViewController ViewController { get; set; }

        public T CreateViewController<T>() where T : IMGUIViewController, new()
        {
            var t = new T();
            t.SetUpView();
            return t;
        }

        public static void Open<T>(string title) where T : Window
        {
            MainWindow = GetWindow<T>(true);

            if (!MainWindow.mShowing)
            {
                MainWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 800, 600);
                MainWindow.titleContent = new GUIContent(title);
                MainWindow.Init();
                MainWindow.mShowing = true;
                MainWindow.Show();
            }
            else
            {
                MainWindow.mShowing = false;
                MainWindow.Dispose();
                MainWindow.Close();
                MainWindow = null;
            }
        }

        public static SubWindow CreateSubWindow(string name = "SubWindow")
        {
            var window = GetWindow<SubWindow>(true, name);
            window.Clear();
            return window;
        }

        void Init()
        {
            OnInit();
        }


        public void PushCommand(Action command)
        {
            RenderEndCommandExecuter.PushCommand(command);
        }

        private void OnGUI()
        {
            if (ViewController != null)
            {
                ViewController.View.DrawGUI();
            }

            RenderEndCommandExecuter.ExecuteCommand();
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected bool mShowing = false;


        protected abstract void OnInit();
        protected abstract void OnDispose();
    }

    public class RenderEndCommandExecuter
    {
        private static Queue<Action> mPrivateCommands = new Queue<Action>();

        private static Queue<Action> mCommands
        {
            get { return mPrivateCommands; }
        }

        public static void PushCommand(Action command)
        {
            mCommands.Enqueue(command);
        }

        public static void ExecuteCommand()
        {
            while (mCommands.Count > 0)
            {
                mCommands.Dequeue().Invoke();
            }
        }
    }


    public class ExpandLayout : Layout
    {
        public ExpandLayout(string label)
        {
            Label = label;
        }

        public string Label { get; set; }


        protected override void OnGUIBegin()
        {
        }


        protected override void OnGUI()
        {
//            if (GUIHelpers.DoToolbarEx(Label))
//            {
//                foreach (var child in Children)
//                {
//                    child.DrawGUI();
//                }
//            }
        }

        protected override void OnGUIEnd()
        {
        }
    }

    public class HorizontalLayout : Layout
    {
        public string HorizontalStyle { get; set; }

        public HorizontalLayout(string horizontalStyle = null)
        {
            HorizontalStyle = horizontalStyle;
        }

        protected override void OnGUIBegin()
        {
            if (string.IsNullOrEmpty(HorizontalStyle))
            {
                GUILayout.BeginHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal(HorizontalStyle);
            }
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndHorizontal();
        }
    }

    public class ScrollLayout : Layout
    {
        Vector2 mScrollPos = Vector2.zero;

        protected override void OnGUIBegin()
        {
            mScrollPos = GUILayout.BeginScrollView(mScrollPos, LayoutStyles);
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndScrollView();
        }
    }

    public class TreeNode : VerticalLayout
    {
        public Property<bool> Spread = null;

        public string Content;


        HorizontalLayout mFirstLine = new HorizontalLayout();

        private VerticalLayout mSpreadView = new VerticalLayout();

        public TreeNode(bool spread, string content, int indent = 0,bool autosaveSpreadState = false)
        {
            if (autosaveSpreadState)
            {
                spread = EditorPrefs.GetBool(content, spread);
            }

            Content = content;
            Spread = new Property<bool>(spread);

            Style = new GUIStyle(EditorStyles.foldout);

            mFirstLine.AddTo(this);
            mFirstLine.AddChild(new SpaceView(indent));

            if (autosaveSpreadState)
            {
                Spread.Bind(value => EditorPrefs.SetBool(content, value));
            }


            new CustomView(() =>
            {
                Spread.Value = EditorGUILayout.Foldout(Spread.Value, Content, true, Style);
            }).AddTo(mFirstLine);

            new CustomView(() =>
            {
                if (Spread.Value)
                {
                    mSpreadView.DrawGUI();
                }
            }).AddTo(this);
        }

        public TreeNode Add2FirstLine(IView view)
        {
            view.AddTo(mFirstLine);
            return this;
        }

        public TreeNode FirstLineBox()
        {
            mFirstLine.HorizontalStyle = "box";

            return this;
        }

        public TreeNode SpreadBox()
        {
            mSpreadView.VerticalStyle = "box";

            return this;
        }

        public TreeNode Add2Spread(IView view)
        {
            view.AddTo(mSpreadView);
            return this;
        }
    }

    public class VerticalLayout : Layout
    {
        public string VerticalStyle { get; set; }

        public VerticalLayout(string verticalStyle = null)
        {
            VerticalStyle = verticalStyle;
        }

        protected override void OnGUIBegin()
        {
            if (string.IsNullOrEmpty(VerticalStyle))
            {
                GUILayout.BeginVertical(LayoutStyles);
            }
            else
            {
                GUILayout.BeginVertical(VerticalStyle, LayoutStyles);
            }
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndVertical();
        }
    }

    public abstract class Layout : View, ILayout
    {
        protected List<IView> Children = new List<IView>();

        public ILayout AddChild(IView view)
        {
            Children.Add(view);
            view.Parent = this;
            return this;
        }

        public void RemoveChild(IView view)
        {
            this.PushCommand(() =>
            {
                Children.Remove(view);
                view.Parent = null;
            });

            view.Dispose();
        }

        public void Clear()
        {
            this.Children.ForEach(view =>
            {
                view.Parent = null;
                view.Dispose();
            });

            this.Children.Clear();
        }

        public override void Refresh()
        {
            Children.ForEach(view => view.Refresh());
            base.Refresh();
        }

        protected override void OnGUI()
        {
            OnGUIBegin();
            foreach (var child in Children)
            {
                child.DrawGUI();
            }

            OnGUIEnd();
        }

        protected abstract void OnGUIBegin();
        protected abstract void OnGUIEnd();
    }

    public interface IView : IDisposable
    {
        void Show();

        void Hide();

        void DrawGUI();

        ILayout Parent { get; set; }

        GUIStyle Style { get; }

        Color BackgroundColor { get; set; }

        void RefreshNextFrame();

        void AddLayoutOption(GUILayoutOption option);

        void RemoveFromParent();

        void Refresh();
    }

    public interface ILayout : IView
    {
        ILayout AddChild(IView view);

        void RemoveChild(IView view);

        void Clear();
    }

    public static class WindowExtension
    {
        public static T PushCommand<T>(this T view, Action command) where T : IView
        {
            RenderEndCommandExecuter.PushCommand(command);
            return view;
        }
    }

    public static class SubWindowExtension
    {
        public static T Postion<T>(this T subWindow, int x, int y) where T : SubWindow
        {
            var rect = subWindow.position;
            rect.x = x;
            rect.y = y;
            subWindow.position = rect;

            return subWindow;
        }

        public static T Size<T>(this T subWindow, int width, int height) where T : SubWindow
        {
            var rect = subWindow.position;
            rect.width = width;
            rect.height = height;
            subWindow.position = rect;

            return subWindow;
        }

        public static T PostionScreenCenter<T>(this T subWindow) where T : SubWindow
        {
            var rect = subWindow.position;
            rect.x = Screen.width / 2;
            rect.y = Screen.height / 2;
            subWindow.position = rect;

            return subWindow;
        }
    }

    public static class ViewExtension
    {
        public static T Width<T>(this T view, float width) where T : IView
        {
            view.AddLayoutOption(GUILayout.Width(width));
            return view;
        }

        public static T Height<T>(this T view, float height) where T : IView
        {
            view.AddLayoutOption(GUILayout.Height(height));
            return view;
        }

        public static T MaxHeight<T>(this T view, float height) where T : IView
        {
            view.AddLayoutOption(GUILayout.MaxHeight(height));
            return view;
        }

        public static T MinHeight<T>(this T view, float height) where T : IView
        {
            view.AddLayoutOption(GUILayout.MinHeight(height));
            return view;
        }

        public static T ExpandHeight<T>(this T view) where T : IView
        {
            view.AddLayoutOption(GUILayout.ExpandHeight(true));
            return view;
        }


        public static T TextMiddleLeft<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.MiddleLeft;
            return view;
        }

        public static T TextMiddleRight<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.MiddleRight;
            return view;
        }

        public static T TextLowerRight<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.LowerRight;
            return view;
        }

        public static T TextMiddleCenter<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.MiddleCenter;
            return view;
        }

        public static T TextLowerCenter<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.LowerCenter;
            return view;
        }

        public static T Color<T>(this T view, Color color) where T : IView
        {
            view.BackgroundColor = color;
            return view;
        }

        public static T FontColor<T>(this T view, Color color) where T : IView
        {
            view.Style.normal.textColor = color;
            return view;
        }

        public static T FontBold<T>(this T view) where T : IView
        {
            view.Style.fontStyle = FontStyle.Bold;
            return view;
        }

        public static T FontNormal<T>(this T view) where T : IView
        {
            view.Style.fontStyle = FontStyle.Normal;
            return view;
        }

        public static T FontSize<T>(this T view, int fontSize) where T : IView
        {
            view.Style.fontSize = fontSize;
            return view;
        }
    }

    public static class LayoutExtension
    {
        public static T AddTo<T>(this T view, ILayout parent) where T : IView
        {
            parent.AddChild(view);
            return view;
        }
    }

    [Serializable]
    public class IntProperty : Property<int>
    {
        public int Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }
    }


    [Serializable]
    public class Property<T>
    {
        public Property()
        {
        }

        private bool setted = false;

        public Property(T initValue)
        {
            mValue = initValue;
        }

        public T Value
        {
            get { return mValue; }
            set
            {
                if (value == null || !value.Equals(mValue) || !setted)
                {
                    mValue = value;

                    if (mSetter != null)
                    {
                        mSetter.Invoke(mValue);
                        OnValueChanged.Invoke(mValue);
                    }

                    setted = true;
                }
            }
        }
        
        private T mValue;
        
        public void Bind(Action<T> setter)
        {
            mSetter += setter;
        }

        public void UnBindAll()
        {
            mSetter = null;
        }

        private event Action<T> mSetter = t=>{};
        public UnityEvent<T> OnValueChanged = new OnPropertyChangedEvent<T>();

    }

    public class OnPropertyChangedEvent<T> : UnityEvent<T>
    {
        
    }

    public static class EditorUtils
    {
        public static string GetSelectedPathOrFallback()
        {
            var path = string.Empty;

            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    return path;
                }
            }

            return path;
        }

        public static void MarkCurrentSceneDirty()
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public static string CurrentSelectPath
        {
            get { return Selection.activeObject == null ? null : AssetDatabase.GetAssetPath(Selection.activeObject); }
        }

        public static string AssetsPath2ABSPath(string assetsPath)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            return assetRootPath.Substring(0, assetRootPath.Length - 6) + assetsPath;
        }

        public static string ABSPath2AssetsPath(string absPath)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            Debug.Log(assetRootPath);
            Debug.Log(Path.GetFullPath(absPath));
            return "Assets" + Path.GetFullPath(absPath).Substring(assetRootPath.Length).Replace("\\", "/");
        }


        public static string AssetPath2ReltivePath(string path)
        {
            if (path == null)
            {
                return null;
            }

            return path.Replace("Assets/", "");
        }

        public static bool ExcuteCmd(string toolName, string args, bool isThrowExcpetion = true)
        {
            Process process = new Process();
            process.StartInfo.FileName = toolName;
            process.StartInfo.Arguments = args;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            OuputProcessLog(process, isThrowExcpetion);
            return true;
        }

        public static void OuputProcessLog(Process p, bool isThrowExcpetion)
        {
            string standardError = string.Empty;
            p.BeginErrorReadLine();

            p.ErrorDataReceived += (sender, outLine) => { standardError += outLine.Data; };

            string standardOutput = string.Empty;
            p.BeginOutputReadLine();
            p.OutputDataReceived += (sender, outLine) => { standardOutput += outLine.Data; };

            p.WaitForExit();
            p.Close();

            Debug.Log(standardOutput);
            if (standardError.Length > 0)
            {
                if (isThrowExcpetion)
                {
                    Debug.LogError(standardError);
                    throw new Exception(standardError);
                }

                Debug.LogError(standardError);
            }
        }

        public static Dictionary<string, string> ParseArgs(string argString)
        {
            int curPos = argString.IndexOf('-');
            Dictionary<string, string> result = new Dictionary<string, string>();

            while (curPos != -1 && curPos < argString.Length)
            {
                int nextPos = argString.IndexOf('-', curPos + 1);
                string item = string.Empty;

                if (nextPos != -1)
                {
                    item = argString.Substring(curPos + 1, nextPos - curPos - 1);
                }
                else
                {
                    item = argString.Substring(curPos + 1, argString.Length - curPos - 1);
                }

                item = StringTrim(item);
                int splitPos = item.IndexOf(' ');

                if (splitPos == -1)
                {
                    string key = StringTrim(item);
                    result[key] = "";
                }
                else
                {
                    string key = StringTrim(item.Substring(0, splitPos));
                    string value = StringTrim(item.Substring(splitPos + 1, item.Length - splitPos - 1));
                    result[key] = value;
                }

                curPos = nextPos;
            }

            return result;
        }

        public static string GetFileMD5Value(string absPath)
        {
            if (!File.Exists(absPath))
                return "";

            MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
            FileStream file = new FileStream(absPath, FileMode.Open);
            byte[] retVal = md5CSP.ComputeHash(file);
            file.Close();
            string result = "";

            for (int i = 0; i < retVal.Length; i++)
            {
                result += retVal[i].ToString("x2");
            }

            return result;
        }


        public static string StringTrim(string str, params char[] trimer)
        {
            int startIndex = 0;
            int endIndex = str.Length;

            for (int i = 0; i < str.Length; ++i)
            {
                if (!IsInCharArray(trimer, str[i]))
                {
                    startIndex = i;
                    break;
                }
            }

            for (int i = str.Length - 1; i >= 0; --i)
            {
                if (!IsInCharArray(trimer, str[i]))
                {
                    endIndex = i;
                    break;
                }
            }

            if (startIndex == 0 && endIndex == str.Length)
            {
                return string.Empty;
            }

            return str.Substring(startIndex, endIndex - startIndex + 1);
        }

        public static string StringTrim(string str)
        {
            return StringTrim(str, ' ', '\t');
        }

        static bool IsInCharArray(char[] array, char c)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i] == c)
                {
                    return true;
                }
            }

            return false;
        }
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

    public class BoxView : View
    {
        public string Text;

        public BoxView(string text)
        {
            Text = text;
            //Style = new GUIStyle(GUI.skin.box);
        }

        protected override void OnGUI()
        {
            GUILayout.Box(Text, GUI.skin.box, LayoutStyles);
        }
    }

    public class ColorView : View
    {
        public ColorView(Color color)
        {
            Color = new Property<Color>(color);
        }

        public Property<Color> Color { get; private set; }

        protected override void OnGUI()
        {
            Color.Value = EditorGUILayout.ColorField(Color.Value, LayoutStyles);
        }
    }

    public class ButtonView : View
    {
        public ButtonView(string text = null, Action onClickEvent = null)
        {
            Text = text;
            OnClickEvent = onClickEvent;
        }

        public string Text { get; set; }

        public Action OnClickEvent { get; set; }
        public UnityEvent OnClick = new UnityEvent();

        protected override void OnGUI()
        {
            if (GUILayout.Button(Text, GUI.skin.button, LayoutStyles))
            {
                if (OnClickEvent != null)
                {
                    OnClickEvent.Invoke();
                }

                OnClick.Invoke();
            }
        }
    }

    public class CustomView : View
    {
        public CustomView(Action onGuiAction)
        {
            OnGUIAction = onGuiAction;
        }

        public Action OnGUIAction { get; set; }

        protected override void OnGUI()
        {
            OnGUIAction.Invoke();
        }
    }

    public class EnumPopupView : View
    {
        public Property<Enum> ValueProperty { get; set; }

        public EnumPopupView(Enum initValue)
        {
            ValueProperty = new Property<Enum>(initValue);
            ValueProperty.Value = initValue;


            try
            {
                Style = new GUIStyle(EditorStyles.popup);
            }
            catch (Exception e)
            {
            }
        }

        protected override void OnGUI()
        {
            Enum enumType = ValueProperty.Value;
            ValueProperty.Value = EditorGUILayout.EnumPopup(enumType, Style, LayoutStyles);
        }
    }

    public class FlexibaleSpaceView : View
    {
        protected override void OnGUI()
        {
            GUILayout.FlexibleSpace();
        }
    }

    public class ImageButtonView : View
    {
        private Texture2D mTexture2D { get; set; }

        private Action mOnClick { get; set; }

        public ImageButtonView(string texturePath, Action onClick)
        {
            mTexture2D = Resources.Load<Texture2D>(texturePath);
            mOnClick = onClick;

            //Style = new GUIStyle(GUI.skin.button);
        }

        protected override void OnGUI()
        {
            if (GUILayout.Button(mTexture2D, LayoutStyles))
            {
                mOnClick.Invoke();
            }
        }
    }

    public class LabelView : View
    {
        public string Content { get; set; }

        public LabelView(string content)
        {
            Content = content;
            try
            {
                Style = new GUIStyle(GUI.skin.label);
            }
            catch (Exception e)
            {
            }
        }

        protected override void OnGUI()
        {
            GUILayout.Label(Content, Style, LayoutStyles);
        }
    }

    public class PopupView : View
    {
        public Property<int> IndexProperty { get; private set; }

        public string[] MenuArray { get; private set; }

        public PopupView(int initValue, string[] menuArray)
        {
            MenuArray = menuArray;
            IndexProperty = new Property<int>(initValue);
            IndexProperty.Value = initValue;

            // Style = new GUIStyle(EditorStyles.popup);
        }

        protected override void OnGUI()
        {
            IndexProperty.Value = EditorGUILayout.Popup(IndexProperty.Value, MenuArray, LayoutStyles);
        }
    }

    public class SpaceView : View
    {
        public int Pixel { get; set; }

        public SpaceView(int pixel = 10)
        {
            Pixel = pixel;
        }

        protected override void OnGUI()
        {
            GUILayout.Space(Pixel);
        }
    }

    public class TextAreaView : View
    {
        public TextAreaView(string content = "")
        {
            Content = new Property<string>(content);
            //Style = new GUIStyle(GUI.skin.textArea);
        }

        public Property<string> Content { get; set; }

        protected override void OnGUI()
        {
            Content.Value = EditorGUILayout.TextArea(Content.Value, GUI.skin.textArea, LayoutStyles);
        }
    }

    public class TextView : View
    {
        public TextView(string content = "", Action<string> onValueChanged = null)
        {
            Content = new Property<string>(content);
            //Style = GUI.skin.textField;

            Content.Bind(_ => OnValueChanged.Invoke());

            if (onValueChanged != null)
            {
                Content.Bind(onValueChanged);
            }
        }

        public Property<string> Content;

        protected override void OnGUI()
        {
            if (mPasswordMode)
            {
                Content.Value = EditorGUILayout.PasswordField(Content.Value, GUI.skin.textField, LayoutStyles);
            }
            else
            {
                Content.Value = EditorGUILayout.TextField(Content.Value, GUI.skin.textField, LayoutStyles);
            }
        }
        
        public UnityEvent OnValueChanged = new UnityEvent();


        private bool mPasswordMode = false;

        public TextView PasswordMode()
        {
            mPasswordMode = true;
            return this;
        }
    }

    public class ToggleView : View
    {
        public string Text { get; private set; }

        public ToggleView(string text, bool initValue = false)
        {
            Text = text;
            Toggle = new Property<bool>(initValue);

            try
            {
                Style = new GUIStyle(GUI.skin.toggle);
            }
            catch (System.Exception e)
            {
            }
        }

        public Property<bool> Toggle { get; private set; }

        protected override void OnGUI()
        {
            // Toggle.Value = GUILayout.Toggle(Toggle.Value, Text, Style, LayoutStyles);
            Toggle.Value = GUILayout.Toggle(Toggle.Value, Text, LayoutStyles);
        }
    }

    public class ToolbarView : View
    {
        public ToolbarView(int defaultIndex = 0)
        {
            Index.Value = defaultIndex;
            Index.Bind(index => MenuSelected[index].Invoke(MenuNames[index]));
        }


        public ToolbarView Menus(List<string> menuNames)
        {
            this.MenuNames = menuNames;
            // empty
            this.MenuSelected = MenuNames.Select(menuName => new Action<string>((str => { }))).ToList();
            return this;
        }

        public ToolbarView AddMenu(string name, Action<string> onMenuSelected)
        {
            MenuNames.Add(name);
            MenuSelected.Add(onMenuSelected);
            return this;
        }

        List<string> MenuNames = new List<string>();

        List<Action<string>> MenuSelected = new List<Action<string>>();

        public Property<int> Index = new Property<int>(0);

        protected override void OnGUI()
        {
            Index.Value = GUILayout.Toolbar(Index.Value, MenuNames.ToArray(), GUI.skin.button, LayoutStyles);
        }
    }

    public static class EventDispatcher
    {
        private static Dictionary<int, Action<object>> mRegisteredEvents = new Dictionary<int, Action<object>>();

        public static void Register<T>(T key, Action<object> onEvent) where T : IConvertible
        {
            int intKey = key.ToInt32(null);

            Action<object> registerdEvent;
            if (!mRegisteredEvents.TryGetValue(intKey, out registerdEvent))
            {
                registerdEvent = (_) => { };
                registerdEvent += onEvent;
                mRegisteredEvents.Add(intKey, registerdEvent);
            }
            else
            {
                mRegisteredEvents[intKey] += onEvent;
            }
        }

        public static void UnRegister<T>(T key, Action<object> onEvent) where T : IConvertible
        {
            int intKey = key.ToInt32(null);

            Action<object> registerdEvent;
            if (!mRegisteredEvents.TryGetValue(intKey, out registerdEvent))
            {
            }
            else
            {
                registerdEvent -= onEvent;
            }
        }

        public static void UnRegisterAll<T>(T key) where T : IConvertible
        {
            int intKey = key.ToInt32(null);
            mRegisteredEvents.Remove(intKey);
        }

        public static void Send<T>(T key, object arg = null) where T : IConvertible
        {
            int intKey = key.ToInt32(null);

            Action<object> registeredEvent;
            if (mRegisteredEvents.TryGetValue(intKey, out registeredEvent))
            {
                registeredEvent.Invoke(arg);
            }
        }
    }

    public static class ColorUtil
    {
        public static string ToText(this Color color)
        {
            return string.Format("{0}@{1}@{2}@{3}", color.r, color.g, color.b, color.a);
        }

        public static Color ToColor(this string colorText)
        {
            var channels = colorText.Split('@');
            return new Color(
                float.Parse(channels[0]),
                float.Parse(channels[1]),
                float.Parse(channels[2]),
                float.Parse(channels[3]));
        }
    }

    public abstract class IMGUIViewController
    {
        public VerticalLayout View = new VerticalLayout();

        public abstract void SetUpView();
    }
}


namespace Dependencies.PackageKit
{
    using Object = UnityEngine.Object;


    // http://stackoverflow.com/questions/1171812/multi-key-dictionary-in-c
    public class Tuple<T1, T2> //FUCKING Unity: struct is not supported in Mono
    {
        public readonly T1 Item1;
        public readonly T2 Item2;

        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            Tuple<T1, T2> p = obj as Tuple<T1, T2>;
            if (obj == null) return false;

            if (Item1 == null)
            {
                if (p.Item1 != null) return false;
            }
            else
            {
                if (p.Item1 == null || !Item1.Equals(p.Item1)) return false;
            }

            if (Item2 == null)
            {
                if (p.Item2 != null) return false;
            }
            else
            {
                if (p.Item2 == null || !Item2.Equals(p.Item2)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (Item1 != null)
                hash ^= Item1.GetHashCode();
            if (Item2 != null)
                hash ^= Item2.GetHashCode();
            return hash;
        }
    }

    // Kanglai: Using Dictionary rather than List!
    public class TypeMappingCollection : Dictionary<Tuple<Type, string>, Type>
    {
        public Type this[Type from, string name = null]
        {
            get
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                Type mapping = null;
                if (this.TryGetValue(key, out mapping))
                {
                    return mapping;
                }

                return null;
            }
            set
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                this[key] = value;
            }
        }
    }

    public class TypeInstanceCollection : Dictionary<Tuple<Type, string>, object>
    {
        public object this[Type from, string name = null]
        {
            get
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                object mapping = null;
                if (this.TryGetValue(key, out mapping))
                {
                    return mapping;
                }

                return null;
            }
            set
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                this[key] = value;
            }
        }
    }

    public class TypeRelationCollection : Dictionary<Tuple<Type, Type>, Type>
    {
        public Type this[Type from, Type to]
        {
            get
            {
                Tuple<Type, Type> key = new Tuple<Type, Type>(from, to);
                Type mapping = null;
                if (this.TryGetValue(key, out mapping))
                {
                    return mapping;
                }

                return null;
            }
            set
            {
                Tuple<Type, Type> key = new Tuple<Type, Type>(from, to);
                this[key] = value;
            }
        }
    }
}
#endif