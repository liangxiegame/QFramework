
using System.Net;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace QFramework
{
    public class PackageMaker : IMGUIEditorWindow
    {
        private PackageVersion mPackageVersion;

        private static void MakePackage()
        {
            var path = MouseSelector.GetSelectedPathOrFallback();

            if (!string.IsNullOrEmpty(path))
            {
                if (Directory.Exists(path))
                {
                    var installPath = string.Empty;

                    if (path.EndsWith("/"))
                    {
                        installPath = path;
                    }
                    else
                    {
                        installPath = path + "/";
                    }

                    new PackageVersion
                    {
                        InstallPath = installPath,
                        Version = "v0.0.0"
                    }.Save();

                    AssetDatabase.Refresh();
                }
            }
        }

        [MenuItem("Assets/@QPM - Publish Package", true)]
        static bool ValiedateExportPackage()
        {
            return User.Logined;
        }

        [MenuItem("Assets/@QPM - Publish Package", priority = 2)]
        public static void publishPackage()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                EditorUtility.DisplayDialog("Package Manager", "请连接网络", "确定");
                return;
            }

            var selectObject = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

            if (selectObject == null || selectObject.Length > 1)
            {
                return;
            }

            if (!EditorUtility.IsPersistent(selectObject[0]))
            {
                return;
            }

            var path = AssetDatabase.GetAssetPath(selectObject[0]);

            if (!Directory.Exists(path))
            {
                return;
            }

            var mInstance = (PackageMaker) GetWindow(typeof(PackageMaker), true);

            mInstance.titleContent = new GUIContent(selectObject[0].name);

            mInstance.position = new Rect(Screen.width / 2, Screen.height / 2, 258, 500);

            mInstance.Show();
        }

        private VerticalLayout RootLayout = null;

        protected override void Init()
        {
            RootLayout = new VerticalLayout("box");

            OnRefresh();
        }

        public void OnRefresh()
        {
            if (Progress == UploadProgress.STATE_GENERATE_INIT)
            {
                RootLayout.Clear();

                // 当前版本号
                var versionLine = new HorizontalLayout().AddTo(RootLayout);
                new LabelView("当前版本号").Width(100).AddTo(versionLine);
                new LabelView(mPackageVersion.Version).Width(100).AddTo(versionLine);

                // 发布版本号 
                var publishedVertionLine = new HorizontalLayout().AddTo(RootLayout);
                new LabelView("发布版本号").Width(100).AddTo(publishedVertionLine);
                new TextView(mVersionText).Width(100).AddTo(publishedVertionLine)
                    .Content.Bind(content => mVersionText = content);


                var typeLine = new HorizontalLayout().AddTo(RootLayout);
                new LabelView("类型").Width(100).AddTo(typeLine);

                new EnumPopupView(mPackageVersion.Type).AddTo(typeLine)
                    .ValueProperty.Bind(value => mPackageVersion.Type = (PackageType) value);


                var accessRightLine = new HorizontalLayout().AddTo(RootLayout);

                new LabelView("权限").Width(100).AddTo(accessRightLine);

                new EnumPopupView(mPackageVersion.AccessRight).AddTo(accessRightLine)
                    .ValueProperty.Bind(v => mPackageVersion.AccessRight = (PackageAccessRight) v);

                new LabelView("发布说明:").Width(150).AddTo(RootLayout);

                new TextAreaView(mReleaseNote).Width(250).Height(300).AddTo(RootLayout)
                    .Content.Bind(releaseNote => mReleaseNote = releaseNote);

                var docLine = new HorizontalLayout().AddTo(RootLayout);

                new LabelView("文档地址:").Width(52).AddTo(docLine);
                new TextView(mPackageVersion.DocUrl).Width(150).AddTo(docLine)
                    .Content.Bind(value => mPackageVersion.DocUrl = value);

                new ButtonView("Paste", () => { mPackageVersion.DocUrl = GUIUtility.systemCopyBuffer; }).AddTo(
                    docLine);


                if (User.Logined)
                {
                    new ButtonView("发布", () =>
                    {
                        if (mReleaseNote.Length < 2)
                        {
                            ShowErrorMsg("请输入版本修改说明");
                            return;
                        }

                        if (!IsVersionValide(mVersionText))
                        {
                            ShowErrorMsg("请输入正确的版本号");
                            return;
                        }

                        mPackageVersion.Version = mVersionText;
                        mPackageVersion.Readme = new ReleaseItem(mVersionText, mReleaseNote,
                            User.Username.Value,
                            DateTime.Now);

                        mPackageVersion.Save();

                        AssetDatabase.Refresh();

                        RenderEndCommandExecuter.PushCommand(() =>
                        {
                            Publish(mPackageVersion, false);
                        });
                    }).AddTo(RootLayout);

                    new ButtonView("发布并删除本地", () => { }).AddTo(RootLayout);
                }
            }
            else
            {
                RootLayout.Clear();

                new CustomView(() =>
                {
                    EditorGUI.LabelField(new Rect(100, 200, 200, 200), NoticeMessage,
                        EditorStyles.boldLabel);
                }).AddTo(RootLayout);
            }

            if (Progress == UploadProgress.STATE_GENERATE_COMPLETE)
            {
                if (EditorUtility.DisplayDialog("上传结果", UpdateResult, "OK"))
                {
                    AssetDatabase.Refresh();

                    Progress = UploadProgress.STATE_GENERATE_INIT;
                    Close();
                }
            }
        }

        public class UploadProgress
        {
            public const byte STATE_GENERATE_INIT      = 0;
            public const byte STATE_GENERATE_UPLOADING = 2;
            public const byte STATE_GENERATE_COMPLETE  = 3;
        }
        
        public string UpdateResult  = "";
        public string NoticeMessage = "";
        public byte   Progress      = UploadProgress.STATE_GENERATE_INIT;

        public void Publish(PackageVersion packageVersion, bool deleteLocal)
        {
            NoticeMessage = "插件上传中,请稍后...";
            Progress = UploadProgress.STATE_GENERATE_UPLOADING;

            OnRefresh();
            UploadPackage.DoUpload(packageVersion, () =>
            {
                if (deleteLocal)
                {
                    Directory.Delete(packageVersion.InstallPath, true);
                    AssetDatabase.Refresh();
                }

                UpdateResult = "上传成功";
                Progress = UploadProgress.STATE_GENERATE_COMPLETE;
                OnRefresh();
            });
        }
        

        private void OnEnable()
        {
            var selectObject = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

            if (selectObject == null || selectObject.Length > 1)
            {
                return;
            }

            var packageFolder = AssetDatabase.GetAssetPath(selectObject[0]);

            var files = Directory.GetFiles(packageFolder, "PackageVersion.json", SearchOption.TopDirectoryOnly);

            if (files.Length <= 0)
            {
                MakePackage();
            }

            mPackageVersion = PackageVersion.Load(packageFolder);
        }

        public override void OnUpdate()
        {
        }
        

        public override void OnClose()
        {
        }

        public static bool IsVersionValide(string version)
        {
            if (version == null)
            {
                return false;
            }

            var t = version.Split('.');
            return t.Length == 3;
        }


        private string mVersionText = string.Empty;

        private string mReleaseNote = string.Empty;

        public override void OnGUI()
        {
            base.OnGUI();

            RootLayout.DrawGUI();
            
            RenderEndCommandExecuter.ExecuteCommand();
        }

        private static void ShowErrorMsg(string content)
        {
            EditorUtility.DisplayDialog("error", content, "OK");
        }
    }

    public class PackageLoginViewUpdateEvent
    {
        public bool Logined     { get; set; }
        public bool InLoginView { get; set; }
    }

    public interface IPackageLoginCommand
    {
        void Execute();
    }

    public class LoginOutCommand : IPackageLoginCommand
    {
        [Inject]
        public PackageLoginModel Model { get; set; }

        public void Execute()
        {
            User.Clear();

            TypeEventSystem.Send(new PackageLoginViewUpdateEvent()
            {
                Logined = Model.Logined,
                InLoginView = Model.InLoginView
            });
        }
    }

    public class OpenRegisterView : IPackageLoginCommand
    {
        [Inject]
        public PackageLoginModel Model { get; set; }

        public void Execute()
        {
            Model.InLoginView = false;

            TypeEventSystem.Send(new PackageLoginViewUpdateEvent()
            {
                Logined = Model.Logined,
                InLoginView = Model.InLoginView
            });
        }
    }


    public class LoginCommand : IPackageLoginCommand
    {
        private string mUsername { get; set; }
        private string mPassword { get; set; }

        [Inject]
        public PackageLoginModel Model { get; set; }

        public LoginCommand(string username, string password)
        {
            mUsername = username;
            mPassword = password;
        }

        public void Execute()
        {
            GetTokenAction.DoGetToken(mUsername, mPassword, token =>
            {
                User.Username.Value = mUsername;
                User.Password.Value = mPassword;
                User.Token.Value = token;
                User.Save();

                TypeEventSystem.Send(new PackageLoginViewUpdateEvent()
                {
                    Logined = Model.Logined,
                    InLoginView = Model.InLoginView
                });
            });
        }
    }

    public interface IPackageLoginServer
    {
    }

    public class PackageLoginServer
    {
        public void Login(string username, string password)
        {
        }
    }

    public class PackageLoginApp
    {
        private IQFrameworkContainer mContainer { get; set; }

        public PackageLoginApp()
        {
            mContainer = new QFrameworkContainer();

            mContainer.RegisterInstance(new PackageLoginModel());

            mContainer.Register<IPackageLoginServer, PackageLoginServer>();

            TypeEventSystem.Register<IPackageLoginCommand>(OnCommandExecute);
        }

        void OnCommandExecute(IPackageLoginCommand command)
        {
            mContainer.Inject(command);
            command.Execute();
        }

        public void Dispose()
        {
            TypeEventSystem.UnRegister<IPackageLoginCommand>(OnCommandExecute);

            mContainer.Clear();
            mContainer = null;
        }
    }

    public class PackageLoginModel
    {
        public bool InLoginView = true;

        public bool Logined
        {
            get { return User.Logined; }
        }
    }

    public class RegisterView : VerticalLayout
    {
        public RegisterView()
        {
            var usernameLine = new HorizontalLayout().AddTo(this);
            new LabelView("username:").AddTo(usernameLine);
            new TextView("").AddTo(usernameLine);

            var passwordLine = new HorizontalLayout().AddTo(this);

            new LabelView("password:").AddTo(passwordLine);

            new TextView("").PasswordMode().AddTo(passwordLine);

            new ButtonView("注册", () => { }).AddTo(this);


            new ButtonView("返回注册", () => { TypeEventSystem.Send<IPackageLoginCommand>(new OpenRegisterView()); })
                .AddTo(this);
        }
    }

    public class PackageLoginStartUpCommand : IPackageLoginCommand
    {
        [Inject]
        public PackageLoginModel Model { get; set; }


        public void Execute()
        {
            TypeEventSystem.Send(new PackageLoginViewUpdateEvent()
            {
                InLoginView = Model.InLoginView,
                Logined = Model.Logined
            });
        }
    }


    public class PackageLoginView : VerticalLayout, IPackageKitView
    {
        public IQFrameworkContainer Container { get; set; }

        PackageLoginApp mPackageLoginApp = new PackageLoginApp();

        public int RenderOrder
        {
            get { return 3; }
        }

        public bool Ignore { get; private set; }

        public bool Enabled
        {
            get { return true; }
        }

        public void Init(IQFrameworkContainer container)
        {
            var expendLayout = new TreeNode(false, LocaleText.UserInfo)
                .AddTo(this);

            mRefreshLayout = new VerticalLayout("box");

            expendLayout.Add2Spread(mRefreshLayout);

            TypeEventSystem.Register<PackageLoginViewUpdateEvent>(OnRefresh);

            TypeEventSystem.Send<IPackageLoginCommand>(new PackageLoginStartUpCommand());
        }

        private VerticalLayout mRefreshLayout;


        void OnRefresh(PackageLoginViewUpdateEvent updateEvent)
        {
            mRefreshLayout.Clear();

            if (updateEvent.Logined)
            {
                new ButtonView("注销",
                    () =>
                    {
                        this.PushCommand(() => { TypeEventSystem.Send<IPackageLoginCommand>(new LoginOutCommand()); });
                    }).AddTo(mRefreshLayout);
            }
            else
            {
                if (updateEvent.InLoginView)
                {
                    new LoginView().AddTo(mRefreshLayout);
                }
                else
                {
                    new RegisterView().AddTo(mRefreshLayout);
                }
            }
        }

        void IPackageKitView.OnUpdate()
        {
        }

        public void OnGUI()
        {
            this.DrawGUI();
        }


        public class LocaleText
        {
            public static string UserInfo
            {
                get { return Language.IsChinese ? "用户信息" : "User Info"; }
            }
        }

        public void OnDispose()
        {
            TypeEventSystem.UnRegister<PackageLoginViewUpdateEvent>(OnRefresh);
            mPackageLoginApp.Dispose();
            mPackageLoginApp = null;
        }
    }

    public class LoginView : VerticalLayout
    {
        public string Username = "";
        public string Password = "";

        public LoginView()
        {
            var usernameLine = new HorizontalLayout().AddTo(this);
            new LabelView("username:").AddTo(usernameLine);
            new TextView(Username)
                .AddTo(usernameLine)
                .Content.Bind(username => Username = username);

            var passwordLine = new HorizontalLayout().AddTo(this);

            new LabelView("password:").AddTo(passwordLine);
            new TextView("").PasswordMode().AddTo(passwordLine)
                .Content.Bind(password => Password = password);

            new ButtonView("登录",
                    () =>
                    {
                        this.PushCommand(() =>
                        {
                            TypeEventSystem.Send<IPackageLoginCommand>(new LoginCommand(Username, Password));
                        });
                    })
                .AddTo(this);

            new ButtonView("注册", () => { Application.OpenURL("http://master.liangxiegame.com/user/register"); })
                .AddTo(this);
        }
    }

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
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
#endif
#endif

#if UNITY_IPHONE
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces(); ;  
            foreach (NetworkInterface adapter in adapters)  
            {  
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))  
                {  
                    UnicastIPAddressInformationCollection uniCast = adapter.GetIPProperties().UnicastAddresses;  
                    if (uniCast.Count > 0)  
                    {  
                        foreach (UnicastIPAddressInformation uni in uniCast)  
                        {  
                            //得到IPv4的地址。 AddressFamily.InterNetwork指的是IPv4  
                            if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
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
                new PackageManagerServer().GetAllRemotePackageInfo((packageDatas) =>
                {
                    if (packageDatas == null)
                    {
                        return;
                    }

                    if (new PackageManagerModel().VersionCheck)
                    {
                        CheckNewVersionDialog(packageDatas, PackageInfosRequestCache.Get().PackageDatas);
                    }
                });
            }

            ReCheckConfigDatas();
            GoToWait();
        }

        private static bool CheckNewVersionDialog(List<PackageData> requestPackageDatas,
            List<PackageData> cachedPackageDatas)
        {
            foreach (var requestPackageData in requestPackageDatas)
            {
                var cachedPacakgeData =
                    cachedPackageDatas.Find(packageData => packageData.Name == requestPackageData.Name);

                var installedPackageVersion = InstalledPackageVersions.Get()
                    .Find(packageVersion => packageVersion.Name == requestPackageData.Name);

                if (installedPackageVersion == null)
                {
                }
                else if (cachedPacakgeData == null &&
                         requestPackageData.VersionNumber > installedPackageVersion.VersionNumber ||
                         cachedPacakgeData != null && requestPackageData.Installed &&
                         requestPackageData.VersionNumber > cachedPacakgeData.VersionNumber &&
                         requestPackageData.VersionNumber > installedPackageVersion.VersionNumber)
                {
                    ShowDisplayDialog(requestPackageData.Name);
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

    public class GetTokenAction
    {
        [Serializable]
        public class ResultFormatData
        {
            public string token;
        }

        public static void DoGetToken(string username, string password, Action<string> onTokenGetted)
        {
            var form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);

            EditorHttp.Post("https://api.liangxiegame.com/qf/v4/token", form, response =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    Debug.Log(response.Text);

                    var responseJson =
                        JsonUtility.FromJson<QFrameworkServerResultFormat<ResultFormatData>>(response.Text);

                    var code = responseJson.code;

                    if (code == 1)
                    {
                        var token = responseJson.data.token;
                        onTokenGetted(token);
                    }
                }
                else if (response.Type == ResponseType.EXCEPTION)
                {
                    Debug.LogError(response.Error);
                }
            });
        }
    }

    public interface IPackageManagerServer
    {
        void DeletePackage(string packageId, Action onResponse);

        void GetAllRemotePackageInfo(Action<List<PackageData>> onResponse);
    }

    [Serializable]
    public class QFrameworkServerResultFormat<T>
    {
        public int code;

        public string msg;

        public T data;
    }

    public class PackageManagerServer : IPackageManagerServer
    {
        public void DeletePackage(string packageId, Action onResponse)
        {
            var form = new WWWForm();

            form.AddField("username", User.Username.Value);
            form.AddField("password", User.Password.Value);
            form.AddField("id", packageId);

            
            EditorHttp.Post("https://api.liangxiegame.com/qf/v4/package/delete", form, (response) =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    var result = JsonUtility.FromJson<QFrameworkServerResultFormat<object>>(response.Text);

                    if (result.code == 1)
                    {
                        Debug.Log("删除成功");

                        onResponse();
                    }
                }
            });
        }

        public void GetAllRemotePackageInfo(Action<List<PackageData>> onResponse)
        {
            if (User.Logined)
            {
                var form = new WWWForm();

                form.AddField("username", User.Username.Value);
                form.AddField("password", User.Password.Value);

                EditorHttp.Post("https://api.liangxiegame.com/qf/v4/package/list", form,
                    (response) => OnResponse(response, onResponse));
            }
            else
            {
                EditorHttp.Post("https://api.liangxiegame.com/qf/v4/package/list", new WWWForm(),
                    (response) => OnResponse(response, onResponse));
            }
        }

        [Serializable]
        public class ResultPackage
        {
            public string id;
            public string name;
            public string version;
            public string downloadUrl;
            public string installPath;
            public string releaseNote;
            public string createAt;
            public string username;
            public string accessRight;
            public string type;
        }

        void OnResponse(EditorHttpResponse response, Action<List<PackageData>> onResponse)
        {
            if (response.Type == ResponseType.SUCCEED)
            {
                var responseJson =
                    JsonUtility.FromJson<QFrameworkServerResultFormat<List<ResultPackage>>>(response.Text);

                if (responseJson.code == 1)
                {
                    
                    var packageInfosJson = responseJson.data;

                    var packageDatas = new List<PackageData>();
                    foreach (var packageInfo in packageInfosJson)
                    {
                        var name = packageInfo.name;

                        var package = packageDatas.Find(packageData => packageData.Name == name);

                        if (package == null)
                        {
                            package = new PackageData()
                            {
                                Name = name,
                            };

                            packageDatas.Add(package);
                        }

                        var id = packageInfo.id;
                        var version = packageInfo.version;
                        var url = packageInfo.downloadUrl;
                        var installPath = packageInfo.installPath;
                        var releaseNote = packageInfo.releaseNote;
                        var createAt = packageInfo.createAt;
                        var creator = packageInfo.username;
                        var releaseItem = new ReleaseItem(version, releaseNote, creator, DateTime.Parse(createAt), id);
                        var accessRightName = packageInfo.accessRight;
                        var typeName = packageInfo.type;

                        var packageType = PackageType.FrameworkModule;

                        switch (typeName)
                        {
                            case "fm":
                                packageType = PackageType.FrameworkModule;
                                break;
                            case "s":
                                packageType = PackageType.Shader;
                                break;
                            case "agt":
                                packageType = PackageType.AppOrGameDemoOrTemplate;
                                break;
                            case "p":
                                packageType = PackageType.Plugin;
                                break;
                            case "master":
                                packageType = PackageType.Master;
                                break;
                        }

                        var accessRight = PackageAccessRight.Public;

                        switch (accessRightName)
                        {
                            case "public":
                                accessRight = PackageAccessRight.Public;
                                break;
                            case "private":
                                accessRight = PackageAccessRight.Private;
                                break;
                        }

                        package.PackageVersions.Add(new PackageVersion()
                        {
                            Id = id,
                            Version = version,
                            DownloadUrl = url,
                            InstallPath = installPath,
                            Type = packageType,
                            AccessRight = accessRight,
                            Readme = releaseItem,
                        });

                        package.readme.AddReleaseNote(releaseItem);
                    }

                    packageDatas.ForEach(packageData =>
                    {
                        packageData.PackageVersions.Sort((a, b) =>
                            b.VersionNumber - a.VersionNumber);
                        packageData.readme.items.Sort((a, b) =>
                            b.VersionNumber - a.VersionNumber);
                    });

                    onResponse(packageDatas);

                    new PackageInfosRequestCache()
                    {
                        PackageDatas = packageDatas
                    }.Save();
                }
            }

            onResponse(null);
        }
    }

    public static class UploadPackage
    {
        private static string UPLOAD_URL
        {
            get { return "https://api.liangxiegame.com/qf/v4/package/add"; }
        }

        public static void DoUpload(PackageVersion packageVersion, System.Action succeed)
        {
            EditorUtility.DisplayProgressBar("插件上传", "打包中...", 0.1f);

            var fileName = packageVersion.Name + "_" + packageVersion.Version + ".unitypackage";
            var fullpath = PackageManagerView.ExportPaths(fileName, packageVersion.InstallPath);
            var file = File.ReadAllBytes(fullpath);

            var form = new WWWForm();
            form.AddField("username", User.Username.Value);
            form.AddField("password", User.Password.Value);
            form.AddField("name", packageVersion.Name);
            form.AddField("version", packageVersion.Version);
            form.AddBinaryData("file", file);
            form.AddField("version", packageVersion.Version);
            form.AddField("releaseNote", packageVersion.Readme.content);
            form.AddField("installPath", packageVersion.InstallPath);
            form.AddField("accessRight", packageVersion.AccessRight.ToString().ToLower());
            form.AddField("docUrl", packageVersion.DocUrl);

            if (packageVersion.Type == PackageType.FrameworkModule)
            {
                form.AddField("type", "fm");
            }
            else if (packageVersion.Type == PackageType.Shader)
            {
                form.AddField("type", "s");
            }
            else if (packageVersion.Type == PackageType.AppOrGameDemoOrTemplate)
            {
                form.AddField("type", "agt");
            }
            else if (packageVersion.Type == PackageType.Plugin)
            {
                form.AddField("type", "p");
            }
            else if (packageVersion.Type == PackageType.Master)
            {
                form.AddField("type", "master");
            }

            Debug.Log(fullpath);

            EditorUtility.DisplayProgressBar("插件上传", "上传中...", 0.2f);

            EditorHttp.Post(UPLOAD_URL, form, (response) =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    EditorUtility.ClearProgressBar();
                    Debug.Log(response.Text);
                    if (succeed != null)
                    {
                        succeed();
                    }

                    File.Delete(fullpath);
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("插件上传", string.Format("上传失败!{0}", response.Error), "确定");
                    File.Delete(fullpath);
                }
            });
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

                    if (item.author == User.Username.Value)
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
        public static void Do(PackageData mRequestPackageData)
        {
            var tempFile = "Assets/" + mRequestPackageData.Name + ".unitypackage";

            Debug.Log(mRequestPackageData.DownloadUrl + ">>>>>>:");

            EditorUtility.DisplayProgressBar("插件更新", "插件下载中 ...", 0.1f);

            EditorHttp.Download(mRequestPackageData.DownloadUrl, response =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    File.WriteAllBytes(tempFile, response.Bytes);

                    EditorUtility.ClearProgressBar();

                    AssetDatabase.ImportPackage(tempFile, false);

                    File.Delete(tempFile);

                    mRequestPackageData.SaveVersionFile();

                    AssetDatabase.Refresh();

                    Debug.Log("PackageManager:插件下载成功");

                    InstalledPackageVersions.Reload();
                }
                else
                {
                    EditorUtility.ClearProgressBar();

                    EditorUtility.DisplayDialog(mRequestPackageData.Name,
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

    [Serializable]
    public class ReleaseItem
    {
        public ReleaseItem()
        {
        }

        public ReleaseItem(string version, string content, string author, DateTime date, string packageId = "")
        {
            this.version = version;
            this.content = content;
            this.author = author;
            this.date = date.ToString("yyyy 年 MM 月 dd 日 HH:mm");
            PackageId = packageId;
        }

        public string version = "";
        public string content = "";
        public string author  = "";
        public string date    = "";
        public string PackageId = "";


        public int VersionNumber
        {
            get
            {
                if (string.IsNullOrEmpty(version))
                {
                    return 0;
                }

                var numbersStr = version.Replace("v", string.Empty).Split('.');

                var retNumber = numbersStr[2].ParseToInt();
                retNumber += numbersStr[1].ParseToInt() * 100;
                retNumber += numbersStr[0].ParseToInt() * 10000;

                return retNumber;
            }
        }
    }

    [Serializable]
    public class Readme
    {
        public List<ReleaseItem> items;

        public ReleaseItem GetItem(string version)
        {
            if (items == null || items.Count == 0)
            {
                return null;
            }

            return items.First(s => s.version == version);
        }

        public void AddReleaseNote(ReleaseItem pluginReadme)
        {
            if (items == null)
            {
                items = new List<ReleaseItem> {pluginReadme};
            }
            else
            {
                bool exist = false;
                foreach (var item in items)
                {
                    if (item.version == pluginReadme.version)
                    {
                        item.content = pluginReadme.content;
                        item.author = pluginReadme.author;
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    items.Add(pluginReadme);
                }
            }
        }
    }

    [Serializable]
    public class PackageData
    {
        public string Id;

        public string Name = "";


        public string version
        {
            get { return PackageVersions.FirstOrDefault() == null ? string.Empty : PackageVersions.First().Version; }
        }

        public string DownloadUrl
        {
            get
            {
                return PackageVersions.FirstOrDefault() == null ? string.Empty : PackageVersions.First().DownloadUrl;
            }
        }

        public string InstallPath
        {
            get
            {
                return PackageVersions.FirstOrDefault() == null ? string.Empty : PackageVersions.First().InstallPath;
            }
        }

        public string DocUrl
        {
            get { return PackageVersions.FirstOrDefault() == null ? string.Empty : PackageVersions.First().DocUrl; }
        }

        public PackageType Type
        {
            get { return PackageVersions.FirstOrDefault() == null ? PackageType.Master : PackageVersions.First().Type; }
        }

        public PackageAccessRight AccessRight
        {
            get
            {
                return PackageVersions.FirstOrDefault() == null
                    ? PackageAccessRight.Public
                    : PackageVersions.First().AccessRight;
            }
        }

        public Readme readme;

        public List<PackageVersion> PackageVersions = new List<PackageVersion>();

        public PackageData()
        {
            readme = new Readme();
        }

        public int VersionNumber
        {
            get
            {
                var numbersStr = version.Replace("v", string.Empty).Split('.');

                var retNumber = numbersStr[2].ParseToInt();
                retNumber += numbersStr[1].ParseToInt() * 100;
                retNumber += numbersStr[0].ParseToInt() * 10000;
                return retNumber;
            }
        }

        public bool Installed
        {
            get { return Directory.Exists(InstallPath); }
        }

        public void SaveVersionFile()
        {
            PackageVersions.First().Save();
        }
    }

    public enum PackageType
    {
        FrameworkModule, //fm
        Shader, //s
        UIKitComponent, //uc
        Plugin, // p
        AppOrGameDemoOrTemplate, //agt
        DocumentsOrTutorial, //doc
        Master, // master
    }

    public enum PackageAccessRight
    {
        Public,
        Private
    }

    [Serializable]
    public class PackageVersion
    {
        public string Id;

        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(InstallPath))
                {
                    var name = InstallPath.Replace("\\", "/");
                    var dirs = name.Split('/');
                    return dirs[dirs.Length - 2];
                }

                return string.Empty;
            }
        }

        public string Version = "v0.0.0";

        public PackageType Type;

        public PackageAccessRight AccessRight;

        public int VersionNumber
        {
            get
            {
                var numbersStr = Version.Replace("v", string.Empty).Split('.');

                var retNumber = numbersStr[2].ParseToInt();
                retNumber += numbersStr[1].ParseToInt() * 100;
                retNumber += numbersStr[0].ParseToInt() * 10000;

                return retNumber;
            }
        }

        public string DownloadUrl;

        public string InstallPath = "Assets/QFramework/Framework/";

        public string FileName
        {
            get { return Name + "_" + Version + ".unitypackage"; }
        }

        public string DocUrl;

        public ReleaseItem Readme = new ReleaseItem();

        public void Save()
        {
            var json = JsonUtility.ToJson(this);

            if (!Directory.Exists(InstallPath))
            {
                Directory.CreateDirectory(InstallPath);
            }

            File.WriteAllText(InstallPath + "/PackageVersion.json", json);
        }

        public static PackageVersion Load(string filePath)
        {
            if (filePath.EndsWith("/"))
            {
                filePath += "PackageVersion.json";
            }
            else if (!filePath.EndsWith("PackageVersion.json"))
            {
                filePath += "/PackageVersion.json";
            }

            return JsonUtility.FromJson<PackageVersion>(File.ReadAllText(filePath));
        }
    }

    public static class PackageKitExtension
    {
        /// <summary>
        /// 解析成数字类型
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="defaulValue"></param>
        /// <returns></returns>
        public static int ParseToInt(this string selfStr, int defaulValue = 0)
        {
            var retValue = defaulValue;
            return int.TryParse(selfStr, out retValue) ? retValue : defaulValue;
        }
    }

    public class PackageView : HorizontalLayout
    {
        class LocaleText
        {
            public static string Doc
            {
                get { return Language.IsChinese ? "文档" : "Doc"; }
            }

            public static string Import
            {
                get { return Language.IsChinese ? "导入" : "Import"; }
            }

            public static string Update
            {
                get { return Language.IsChinese ? "更新" : "Update"; }
            }

            public static string Reimport
            {
                get { return Language.IsChinese ? "再次导入" : "Reimport"; }
            }

            public static string ReleaseNotes
            {
                get { return Language.IsChinese ? "版本说明" : "Release Notes"; }
            }
        }

        private PackageData mPackageData;

        public PackageView(PackageData packageData) : base(null)
        {
            this.mPackageData = packageData;

            Refresh();
        }

        protected override void OnRefresh()
        {
            Clear();

            new SpaceView(2).AddTo(this);

            new LabelView(mPackageData.Name)
                .FontBold()
                .Width(150)
                .AddTo(this);

            new LabelView(mPackageData.version)
                .TextMiddleCenter()
                .Width(80)
                .AddTo(this);

            var installedPackage = InstalledPackageVersions.FindVersionByName(mPackageData.Name);

            new LabelView(installedPackage != null ? installedPackage.Version : " ")
                .TextMiddleCenter()
                .Width(80)
                .AddTo(this);

            new LabelView(mPackageData.AccessRight.ToString())
                .TextMiddleCenter()
                .Width(80)
                .AddTo(this);

            if (!string.IsNullOrEmpty(mPackageData.DocUrl))
            {
                new ButtonView(LocaleText.Doc, () => { }).AddTo(this);
            }
            else
            {
                new SpaceView(40).AddTo(this);
            }


            if (installedPackage == null)
            {
                new ButtonView(LocaleText.Import, () =>
                    {
                        InstallPackage.Do(mPackageData);

                        PackageApplication.Container.Resolve<PackageKitWindow>().Close();
                    })
                    .Width(90)
                    .AddTo(this);
            }

            else if (mPackageData.VersionNumber > installedPackage.VersionNumber)
            {
                new ButtonView(LocaleText.Update, () =>
                    {
                        var path = Application.dataPath.Replace("Assets", mPackageData.InstallPath);

                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }

                        this.PushCommand(() =>
                        {
                            AssetDatabase.Refresh();

                            InstallPackage.Do(mPackageData);

                            PackageApplication.Container.Resolve<PackageKitWindow>().Close();
                        });
                    })
                    .Width(90)
                    .AddTo(this);
            }
            else if (mPackageData.VersionNumber == installedPackage.VersionNumber)
            {
                new ButtonView(LocaleText.Reimport, () =>
                    {
                        var path = Application.dataPath.Replace("Assets", mPackageData.InstallPath);

                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }

                        this.PushCommand(() =>
                        {
                            AssetDatabase.Refresh();

                            InstallPackage.Do(mPackageData);

                            PackageApplication.Container.Resolve<PackageKitWindow>().Close();
                        });
                    })
                    .Width(90)
                    .AddTo(this);
            }
            else if (mPackageData.VersionNumber < installedPackage.VersionNumber)
            {
                new SpaceView(94).AddTo(this);
            }

            new ButtonView(LocaleText.ReleaseNotes,
                    () => { ReadmeWindow.Init(mPackageData.readme, mPackageData.PackageVersions.First()); }).Width(100)
                .AddTo(this);
        }
    }

    public class HeaderView : HorizontalLayout
    {
        public HeaderView()
        {
            HorizontalStyle = "box";

            new LabelView(LocaleText.PackageName)
                .Width(150)
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.ServerVersion)
                .Width(80)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.LocalVersion)
                .Width(80)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.AccessRight)
                .Width(80)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.Doc)
                .Width(40)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.Action)
                .Width(100)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.ReleaseNote)
                .Width(100)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);
        }

        class LocaleText
        {
            public static string PackageName
            {
                get { return Language.IsChinese ? " 模块名" : " Package Name"; }
            }

            public static string ServerVersion
            {
                get { return Language.IsChinese ? "服务器版本" : "Server Version"; }
            }

            public static string LocalVersion
            {
                get { return Language.IsChinese ? "本地版本" : "Local Version"; }
            }

            public static string AccessRight
            {
                get { return Language.IsChinese ? "访问权限" : "Access Right"; }
            }

            public static string Doc
            {
                get { return Language.IsChinese ? "文档" : "Doc"; }
            }

            public static string Action
            {
                get { return Language.IsChinese ? "动作" : "Action"; }
            }

            public static string ReleaseNote
            {
                get { return Language.IsChinese ? "版本说明" : "ReleaseNote Note"; }
            }
        }
    }

    public interface IEditorStrangeMVCCommand
    {
        void Execute();
    }

    public class PackageManagerApp
    {
        public IQFrameworkContainer Container = new QFrameworkContainer();

        public PackageManagerApp()
        {
            // 注册好 自己的实例
            Container.RegisterInstance<IQFrameworkContainer>(Container);

            // 配置命令的执行
            TypeEventSystem.Register<IEditorStrangeMVCCommand>(OnCommandExecute);

            InstalledPackageVersions.Reload();

            // 注册好 model
            var model = new PackageManagerModel
            {
                PackageDatas = PackageInfosRequestCache.Get().PackageDatas
            };

            Container.RegisterInstance(model);

            Container.Register<IPackageManagerServer, PackageManagerServer>();
        }

        void OnCommandExecute(IEditorStrangeMVCCommand command)
        {
            Container.Inject(command);
            command.Execute();
        }

        public void Dispose()
        {
            TypeEventSystem.UnRegister<IEditorStrangeMVCCommand>(OnCommandExecute);

            Container.Clear();
            Container = null;
        }
    }

    [Serializable]
    public class PackageInfosRequestCache
    {
        public List<PackageData> PackageDatas = new List<PackageData>();

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

    public class PackageManagerModel
    {
        public PackageManagerModel()
        {
            PackageDatas = PackageInfosRequestCache.Get().PackageDatas;
        }

        public List<PackageData> PackageDatas = new List<PackageData>();

        public bool VersionCheck
        {
            get { return EditorPrefs.GetBool("QFRAMEWORK_VERSION_CHECK", true); }
            set { EditorPrefs.SetBool("QFRAMEWORK_VERSION_CHECK", value); }
        }
    }

    public class PackageManagerViewUpdate
    {
        public List<PackageData> PackageDatas { get; set; }

        public bool VersionCheck { get; set; }
    }

    public class PackageManagerStartUpCommand : IEditorStrangeMVCCommand
    {
        [Inject]
        public PackageManagerModel Model { get; set; }

        [Inject]
        public IPackageManagerServer Server { get; set; }

        public void Execute()
        {
            TypeEventSystem.Send(new PackageManagerViewUpdate()
            {
                PackageDatas = Model.PackageDatas,
                VersionCheck = Model.VersionCheck
            });

            Server.GetAllRemotePackageInfo(list =>
            {
                TypeEventSystem.Send(new PackageManagerViewUpdate()
                {
                    PackageDatas = PackageInfosRequestCache.Get().PackageDatas,
                    VersionCheck = Model.VersionCheck
                });
            });
        }
    }

    public class PackageManagerView : IPackageKitView
    {
        private static readonly string EXPORT_ROOT_DIR = Path.Combine(Application.dataPath, "../");

        public static string ExportPaths(string exportPackageName, params string[] paths)
        {
            if (Directory.Exists(paths[0]))
            {
                if (paths[0].EndsWith("/"))
                {
                    paths[0] = paths[0].Remove(paths[0].Length - 1);
                }

                var filePath = Path.Combine(EXPORT_ROOT_DIR, exportPackageName);
                AssetDatabase.ExportPackage(paths,
                    filePath, ExportPackageOptions.Recurse);
                AssetDatabase.Refresh();

                return filePath;
            }

            return string.Empty;
        }


        PackageManagerApp mPackageManagerApp = new PackageManagerApp();

        private Vector2 mScrollPos;


        private Action mOnToolbarIndexChanged;

        public int ToolbarIndex
        {
            get { return EditorPrefs.GetInt("PM_TOOLBAR_INDEX", 0); }
            set
            {
                EditorPrefs.SetInt("PM_TOOLBAR_INDEX", value);

                if (mOnToolbarIndexChanged != null)
                {
                    mOnToolbarIndexChanged.Invoke();
                }
            }
        }

        private string[] mToolbarNamesLogined =
            {"Framework", "Plugin", "UIKitComponent", "Shader", "AppOrTemplate", "Private", "Master"};

        private string[] mToolbarNamesUnLogined = {"Framework", "Plugin", "UIKitComponent", "Shader", "AppOrTemplate"};

        public string[] ToolbarNames
        {
            get { return User.Logined ? mToolbarNamesLogined : mToolbarNamesUnLogined; }
        }

        public IEnumerable<PackageData> SelectedPackageType(List<PackageData> packageDatas)
        {
            switch (ToolbarIndex)
            {
                case 0:
                    return packageDatas.Where(packageData => packageData.Type == PackageType.FrameworkModule)
                        .OrderBy(p => p.Name);
                case 1:
                    return packageDatas.Where(packageData => packageData.Type == PackageType.Plugin)
                        .OrderBy(p => p.Name);
                case 2:
                    return packageDatas.Where(packageData => packageData.Type == PackageType.UIKitComponent)
                        .OrderBy(p => p.Name);
                case 3:
                    return packageDatas.Where(packageData => packageData.Type == PackageType.Shader)
                        .OrderBy(p => p.Name);
                case 4:
                    return packageDatas.Where(packageData =>
                        packageData.Type == PackageType.AppOrGameDemoOrTemplate).OrderBy(p => p.Name);
                case 5:
                    return packageDatas.Where(packageData =>
                        packageData.AccessRight == PackageAccessRight.Private).OrderBy(p => p.Name);
                case 6:
                    return packageDatas.Where(packageData => packageData.Type == PackageType.Master)
                        .OrderBy(p => p.Name);
                default:
                    return packageDatas.Where(packageData => packageData.Type == PackageType.FrameworkModule)
                        .OrderBy(p => p.Name);
            }
        }

        public IQFrameworkContainer Container { get; set; }

        public int RenderOrder
        {
            get { return 1; }
        }

        public bool Ignore { get; private set; }

        public bool Enabled
        {
            get { return true; }
        }

        private VerticalLayout mRootLayout = new VerticalLayout();

        public void Init(IQFrameworkContainer container)
        {
            TypeEventSystem.Register<PackageManagerViewUpdate>(OnRefresh);

            // 执行
            TypeEventSystem.Send<IEditorStrangeMVCCommand>(new PackageManagerStartUpCommand());
        }

        void OnRefresh(PackageManagerViewUpdate viewUpdateEvent)
        {
            mRootLayout = new VerticalLayout();

            var treeNode = new TreeNode(true, LocaleText.FrameworkPackages).AddTo(mRootLayout);

            var verticalLayout = new VerticalLayout("box");

            treeNode.Add2Spread(verticalLayout);

            new ToolbarView(ToolbarIndex)
                .Menus(ToolbarNames.ToList())
                .AddTo(verticalLayout)
                .Index.Bind(newIndex => ToolbarIndex = newIndex);


            new HeaderView()
                .AddTo(verticalLayout);

            var packageList = new VerticalLayout("box")
                .AddTo(verticalLayout);

            var scroll = new ScrollLayout()
                .Height(240)
                .AddTo(packageList);

            new SpaceView(2).AddTo(scroll);

            mOnToolbarIndexChanged = () =>
            {
                scroll.Clear();

                foreach (var packageData in SelectedPackageType(viewUpdateEvent.PackageDatas))
                {
                    new SpaceView(2).AddTo(scroll);
                    new PackageView(packageData).AddTo(scroll);
                }
            };

            foreach (var packageData in SelectedPackageType(viewUpdateEvent.PackageDatas))
            {
                new SpaceView(2).AddTo(scroll);
                new PackageView(packageData).AddTo(scroll);
            }
        }

        public void OnUpdate()
        {
        }

        public void OnGUI()
        {
            mRootLayout.DrawGUI();
        }

        public void OnDispose()
        {
            TypeEventSystem.UnRegister<PackageManagerViewUpdate>(OnRefresh);

            mPackageManagerApp.Dispose();
            mPackageManagerApp = null;
        }


        class LocaleText
        {
            public static string FrameworkPackages
            {
                get { return Language.IsChinese ? "框架模块" : "Framework Packages"; }
            }

            public static string VersionCheck
            {
                get { return Language.IsChinese ? "版本检测" : "Version Check"; }
            }
        }
    }

    public class EditorHttpResponse
    {
        public ResponseType Type;

        public byte[] Bytes;

        public string Text;

        public string Error;
    }

    public enum ResponseType
    {
        SUCCEED,
        EXCEPTION,
        TIMEOUT,
    }

    public static class EditorHttp
    {
        public class EditorWWWExecuter
        {
            private WWW                        mWWW;
            private Action<EditorHttpResponse> mResponse;
            private Action<float>              mOnProgress;
            private bool                       mDownloadMode;

            public EditorWWWExecuter(WWW www, Action<EditorHttpResponse> response, Action<float> onProgress = null,
                bool downloadMode = false)
            {
                mWWW = www;
                mResponse = response;
                mOnProgress = onProgress;
                mDownloadMode = downloadMode;
                EditorApplication.update += Update;
            }

            void Update()
            {
                if (mWWW != null && mWWW.isDone)
                {
                    if (string.IsNullOrEmpty(mWWW.error))
                    {
                        if (mDownloadMode)
                        {
                            if (mOnProgress != null)
                            {
                                mOnProgress(1.0f);
                            }

                            mResponse(new EditorHttpResponse()
                            {
                                Type = ResponseType.SUCCEED,
                                Bytes = mWWW.bytes
                            });
                        }
                        else
                        {
                            mResponse(new EditorHttpResponse()
                            {
                                Type = ResponseType.SUCCEED,
                                Text = mWWW.text
                            });
                        }
                    }
                    else
                    {
                        mResponse(new EditorHttpResponse()
                        {
                            Type = ResponseType.EXCEPTION,
                            Error = mWWW.error
                        });
                    }

                    Dispose();
                }

                if (mWWW != null && mDownloadMode)
                {
                    if (mOnProgress != null)
                    {
                        mOnProgress(mWWW.progress);
                    }
                }
            }

            void Dispose()
            {
                mWWW.Dispose();
                mWWW = null;

                EditorApplication.update -= Update;
            }
        }


        public static void Get(string url, Action<EditorHttpResponse> response)
        {
            new EditorWWWExecuter(new WWW(url), response);
        }

        public static void Post(string url, WWWForm form, Action<EditorHttpResponse> response)
        {
            new EditorWWWExecuter(new WWW(url, form), response);
        }

        public static void Download(string url, Action<EditorHttpResponse> response, Action<float> onProgress = null)
        {
            new EditorWWWExecuter(new WWW(url), response, onProgress, true);
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

        [MenuItem(FrameworkMenuItems.Preferences, false, FrameworkMenuItemsPriorities.Preferences)]
        [MenuItem(FrameworkMenuItems.PackageKit, false, FrameworkMenuItemsPriorities.Preferences)]
        private static void Open()
        {
            var packageKitWindow = Create<PackageKitWindow>(true);
            packageKitWindow.titleContent = new GUIContent(LocaleText.QFrameworkSettings);
            packageKitWindow.position = new Rect(100, 100, 690, 800);
            packageKitWindow.Show();
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

        protected override void Init()
        {
            var label = GUI.skin.label;
            PackageApplication.Container = null;

            RemoveAllChidren();

            mPackageKitViews = PackageApplication.Container
                .ResolveAll<IPackageKitView>()
                .OrderBy(view => view.RenderOrder)
                .ToList();

            PackageApplication.Container.RegisterInstance(this);
        }

        public override void OnGUI()
        {
            base.OnGUI();
            mPackageKitViews.ForEach(view => view.OnGUI());

            RenderEndCommandExecuter.ExecuteCommand();
        }

        public override void OnClose()
        {
            mPackageKitViews.ForEach(view => view.OnDispose());

            RemoveAllChidren();
        }
    }

    public static class PackageApplication
    {
        public static  List<Assembly>                  CachedAssemblies { get; set; }
        private static Dictionary<Type, IEventManager> mEventManagers;

        private static Dictionary<Type, IEventManager> EventManagers
        {
            get { return mEventManagers ?? (mEventManagers = new Dictionary<Type, IEventManager>()); }
            set { mEventManagers = value; }
        }

        private static QFrameworkContainer mContainer;

        public static QFrameworkContainer Container
        {
            get
            {
                if (mContainer != null) return mContainer;
                mContainer = new QFrameworkContainer();
                InitializeContainer(mContainer);
                return mContainer;
            }
            set
            {
                mContainer = value;
                if (mContainer == null)
                {
                    IEventManager eventManager;
                    EventManagers.TryGetValue(typeof(ISystemResetEvents), out eventManager);
                    EventManagers.Clear();
                    var events = eventManager as EventManager<ISystemResetEvents>;
                    if (events != null)
                    {
                        events.Signal(_ => _.SystemResetting());
                    }
                }
            }
        }

        public static IEnumerable<Type> GetDerivedTypes<T>(bool includeAbstract = false, bool includeBase = true)
        {
            var type = typeof(T);
            if (includeBase)
                yield return type;
            if (includeAbstract)
            {
                foreach (var assembly in CachedAssemblies)
                {
                    foreach (var t in assembly
                        .GetTypes()
                        .Where(x => type.IsAssignableFrom(x)))
                    {
                        yield return t;
                    }
                }
            }
            else
            {
                var items = new List<Type>();
                foreach (var assembly in CachedAssemblies)
                {
                    try
                    {
                        items.AddRange(assembly.GetTypes()
                            .Where(x => type.IsAssignableFrom(x) && !x.IsAbstract));
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
//						InvertApplication.Log(ex.Message);
                    }
                }

                foreach (var item in items)
                    yield return item;
            }
        }

        public static System.Action ListenFor(Type eventInterface, object listenerObject)
        {
            var listener = listenerObject;

            IEventManager manager;
            if (!EventManagers.TryGetValue(eventInterface, out manager))
            {
                EventManagers.Add(eventInterface,
                    manager = (IEventManager) Activator.CreateInstance(
                        typeof(EventManager<>).MakeGenericType(eventInterface)));
            }

            var m = manager;


            return m.AddListener(listener);
        }

        private static IPackageKitView[] mPlugins;

        public static IPackageKitView[] Plugins
        {
            get { return mPlugins ?? (mPlugins = Container.ResolveAll<IPackageKitView>().ToArray()); }
            set { mPlugins = value; }
        }

        private static void InitializeContainer(IQFrameworkContainer container)
        {
            mPlugins = null;
            container.RegisterInstance(container);
            var pluginTypes = GetDerivedTypes<IPackageKitView>(false, false).ToArray();
//			// Load all plugins
            foreach (var diagramPlugin in pluginTypes)
            {
//				if (pluginTypes.Any(p => p.BaseType == diagramPlugin)) continue;
                var pluginInstance = Activator.CreateInstance((Type) diagramPlugin) as IPackageKitView;
                if (pluginInstance == null) continue;
                container.RegisterInstance(pluginInstance, diagramPlugin.Name, false);
                container.RegisterInstance(pluginInstance.GetType(), pluginInstance);
                if (pluginInstance.Enabled)
                {
                    foreach (var item in diagramPlugin.GetInterfaces())
                    {
                        ListenFor(item, pluginInstance);
                    }
                }
            }

            container.InjectAll();

            foreach (var diagramPlugin in Plugins.OrderBy(p => p.RenderOrder).Where(p => !p.Ignore))
            {
                if (diagramPlugin.Enabled)
                {
                    var start = DateTime.Now;
                    diagramPlugin.Container = Container;
                    diagramPlugin.Init(Container);
                }
            }

            foreach (var diagramPlugin in Plugins.OrderBy(p => p.RenderOrder).Where(p => !p.Ignore))
            {
                if (diagramPlugin.Enabled)
                {
                    var start = DateTime.Now;
                    container.Inject(diagramPlugin);
//					diagramPlugin.Loaded(Container);
//					diagramPlugin.LoadTime = DateTime.Now.Subtract(start);
                }
            }

            SignalEvent<ISystemResetEvents>(_ => _.SystemRestarted());
        }

        public static void SignalEvent<TEvents>(Action<TEvents> action) where TEvents : class
        {
            IEventManager manager;
            if (!EventManagers.TryGetValue(typeof(TEvents), out manager))
            {
                EventManagers.Add(typeof(TEvents), manager = new EventManager<TEvents>());
            }

            var m = manager as EventManager<TEvents>;
            m.Signal(action);
        }

        static PackageApplication()
        {
            CachedAssemblies = new List<Assembly>
            {
                typeof(int).Assembly, typeof(List<>).Assembly
            };

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("QF") || assembly.FullName.StartsWith("Assembly-CSharp-Editor"))
                {
                    CachedAssembly(assembly);
                }
            }
        }

        public static void CachedAssembly(Assembly assembly)
        {
            if (CachedAssemblies.Contains(assembly)) return;
            CachedAssemblies.Add(assembly);
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

        public void AddChild(IView view)
        {
            mChildren.Add(view);
            view.Parent = this;
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
        public class EventRecord
        {
            public int Key;

            public Action<object> OnEvent;
        }

        List<EventRecord> mPrivteEventRecords = new List<EventRecord>();

        protected List<EventRecord> mEventRecords
        {
            get { return mPrivteEventRecords; }
        }

        protected void RegisterEvent<T>(T key, Action<object> onEvent) where T : IConvertible
        {
            EventDispatcher.Register(key, onEvent);

            mEventRecords.Add(new EventRecord
            {
                Key = key.ToInt32(null),
                OnEvent = onEvent
            });
        }

        protected void UnRegisterAll()
        {
            mEventRecords.ForEach(record => { EventDispatcher.UnRegister(record.Key, record.OnEvent); });

            mEventRecords.Clear();
        }

        protected void SendEvent<T>(T key, object arg) where T : IConvertible
        {
            EventDispatcher.Send(key, arg);
        }

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
            UnRegisterAll();
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
            ViewController.View.DrawGUI();

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

        public TreeNode(bool spread, string content, int indent = 0)
        {
            Content = content;
            Spread = new Property<bool>(spread);

            Style = new GUIStyle(EditorStyles.foldout);

            mFirstLine.AddTo(this);
            mFirstLine.AddChild(new SpaceView(indent));

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

        public void AddChild(IView view)
        {
            Children.Add(view);
            view.Parent = this;
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
            Children.Clear();
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
        void AddChild(IView view);

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
                    }

                    setted = true;
                }
            }
        }

        private T mValue;

        /// <summary>
        /// TODO:注销也要做下
        /// </summary>
        /// <param name="setter"></param>
        public void Bind(Action<T> setter)
        {
            mSetter += setter;
            mBindings.Add(setter);
        }

        private List<Action<T>> mBindings = new List<Action<T>>();

        public void UnBindAll()
        {
            foreach (var binding in mBindings)
            {
                mSetter -= binding;
            }
        }

        private event Action<T> mSetter;
    }

    public static class EditorUtils
    {
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
        public ButtonView(string text, Action onClickEvent)
        {
            Text = text;
            OnClickEvent = onClickEvent;
            //Style = new GUIStyle(GUI.skin.button);
        }

        public string Text { get; set; }

        public Action OnClickEvent { get; set; }

        protected override void OnGUI()
        {
            if (GUILayout.Button(Text, GUI.skin.button, LayoutStyles))
            {
                OnClickEvent.Invoke();
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

    public class EnumPopupView : View //where T : struct
    {
        public Property<Enum> ValueProperty { get; private set; }

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

            Style = new GUIStyle(EditorStyles.popup);
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
        public TextAreaView(string content)
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
        public TextView(string content)
        {
            Content = new Property<string>(content);
            //Style = GUI.skin.textField;
        }

        public Property<string> Content { get; set; }

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
