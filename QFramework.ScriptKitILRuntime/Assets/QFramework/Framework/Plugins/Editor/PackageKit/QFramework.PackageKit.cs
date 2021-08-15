/****************************************************************************
 * Copyright (c) 2020.10 liangxie
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
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace QFramework
{
    /// <summary>
    /// some net work util
    /// </summary>
    public static class Network
    {
        public static bool IsReachable
        {
            get { return Application.internetReachability != NetworkReachability.NotReachable; }
        }
    }

    // [InitializeOnLoad]
    // public class PackageCheck
    // {
    //     enum CheckStatus
    //     {
    //         WAIT,
    //         COMPARE,
    //         NONE
    //     }
    //
    //     private CheckStatus mCheckStatus;
    //
    //     private double mNextCheckTime = 0;
    //
    //     private double mCheckInterval = 60;
    //
    //     ControllerNode<PackageKit> mControllerNode = ControllerNode<PackageKit>.Allocate();
    //
    //     static PackageCheck()
    //     {
    //         if (!EditorApplication.isPlayingOrWillChangePlaymode && Network.IsReachable)
    //         {
    //             PackageCheck packageCheck = new PackageCheck()
    //             {
    //                 mCheckStatus = CheckStatus.WAIT,
    //                 mNextCheckTime = EditorApplication.timeSinceStartup,
    //             };
    //
    //             EditorApplication.update = packageCheck.CustomUpdate;
    //         }
    //     }
    //
    //     private void CustomUpdate()
    //     {
    //         // 添加网络判断
    //         if (!Network.IsReachable) return;
    //
    //         switch (mCheckStatus)
    //         {
    //             case CheckStatus.WAIT:
    //                 if (EditorApplication.timeSinceStartup >= mNextCheckTime)
    //                 {
    //                     mCheckStatus = CheckStatus.COMPARE;
    //                 }
    //
    //                 break;
    //
    //             case CheckStatus.COMPARE:
    //
    //                 ProcessCompare();
    //
    //                 break;
    //         }
    //     }
    //
    //
    //     private void GoToWait()
    //     {
    //         mCheckStatus = CheckStatus.WAIT;
    //
    //         mNextCheckTime = EditorApplication.timeSinceStartup + mCheckInterval;
    //     }
    //
    //     private bool ReCheckConfigDatas()
    //     {
    //         mCheckInterval = 60;
    //
    //         return true;
    //     }
    //
    //     private void ProcessCompare()
    //     {
    //         if (Network.IsReachable)
    //         {
    //             new PackageManagerServer().GetAllRemotePackageInfoV5((packageDatas, res) =>
    //             {
    //                 if (packageDatas == null)
    //                 {
    //                     return;
    //                 }
    //
    //                 if (new PackageManagerModel().VersionCheck)
    //                 {
    //                     CheckNewVersionDialog(packageDatas, PackageInfosRequestCache.Get().PackageRepositories);
    //                 }
    //             });
    //         }
    //
    //         ReCheckConfigDatas();
    //         GoToWait();
    //     }
    //
    //     private bool CheckNewVersionDialog(List<PackageRepository> requestPackageDatas,
    //         List<PackageRepository> cachedPackageDatas)
    //     {
    //         var installedPackageVersionsModel = mControllerNode.GetModel<ILocalPackageVersionModel>();
    //         foreach (var requestPackageData in requestPackageDatas)
    //         {
    //             var cachedPacakgeData =
    //                 cachedPackageDatas.Find(packageData => packageData.name == requestPackageData.name);
    //
    //             var installedPackageVersion = installedPackageVersionsModel.GetByName(requestPackageData.name);
    //
    //             if (installedPackageVersion == null)
    //             {
    //             }
    //             else if (cachedPacakgeData == null &&
    //                      requestPackageData.VersionNumber > installedPackageVersion.VersionNumber ||
    //                      cachedPacakgeData != null && requestPackageData.Installed &&
    //                      requestPackageData.VersionNumber > cachedPacakgeData.VersionNumber &&
    //                      requestPackageData.VersionNumber > installedPackageVersion.VersionNumber)
    //             {
    //                 ShowDisplayDialog(requestPackageData.name);
    //                 return false;
    //             }
    //         }
    //
    //         return true;
    //     }
    //
    //
    //     private static void ShowDisplayDialog(string packageName)
    //     {
    //         var result = EditorUtility.DisplayDialog("PackageManager",
    //             string.Format("{0} 有新版本更新,请前往查看(如需不再提示请点击前往查看，并取消勾选 Version Check)", packageName),
    //             "前往查看", "稍后查看");
    //
    //         if (result)
    //         {
    //             EditorApplication.ExecuteMenuItem(FrameworkMenuItems.Preferences);
    //         }
    //     }
    //
    //     ~PackageCheck()
    //     {
    //         mControllerNode = null;
    //     }
    // }

    public static class User
    {
        public static Property<string> Username = new Property<string>(LoadString("username"));
        public static Property<string> Password = new Property<string>(LoadString("password"));
        public static Property<string> Token = new Property<string>(LoadString("token"));

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

    public class InstallPackage : AbstractCommand
    {
        private readonly PackageRepository mRequestPackageData;

        private void OnProgressChanged(float progress)
        {
            EditorUtility.DisplayProgressBar("插件更新",
                string.Format("插件下载中 {0:P2}", progress), progress);
        }

        public InstallPackage(PackageRepository requestPackageData)
        {
            mRequestPackageData = requestPackageData;
        }

        protected override void OnExecute()
        {
            var tempFile = "Assets/" + mRequestPackageData.name + ".unitypackage";

            Log.I(mRequestPackageData.latestDownloadUrl + ">>>>>>:");

            EditorUtility.DisplayProgressBar("插件更新", "插件下载中 ...", 0.1f);

            EditorHttp.Download(mRequestPackageData.latestDownloadUrl, response =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    File.WriteAllBytes(tempFile, response.Bytes);

                    EditorUtility.ClearProgressBar();

                    AssetDatabase.ImportPackage(tempFile, false);

                    File.Delete(tempFile);

                    AssetDatabase.Refresh();

                    Log.I("PackageManager:插件下载成功");


                    this.GetModel<ILocalPackageVersionModel>()
                        .Reload();
                }
                else
                {
                    EditorUtility.ClearProgressBar();

                    EditorUtility.DisplayDialog(mRequestPackageData.name,
                        "插件安装失败,请联系 liangxiegame@163.com 或者加入 QQ 群:623597263" + response.Error + ";", "OK");
                }
            }, OnProgressChanged);
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
        public const string PackageKit = "QFramework/PackageKit... %#e";

        public const string Feedback = "QFramework/Feedback";
    }

    public static class FrameworkMenuItemsPriorities
    {
        public const int Preferences = 1;

        public const int Feedback = 11;
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

    public class SubWindow : EditorWindow, IMGUILayout
    {
        public string Id { get; set; }
        public bool Visible { get; set; }

        public Func<bool> VisibleCondition { get; set; }

        void IMGUIView.DrawGUI()
        {
        }

        IMGUILayout IMGUIView.Parent { get; set; }

        private GUIStyleProperty mStyle = new GUIStyleProperty(() => new GUIStyle());

        public GUIStyleProperty Style
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


        public void PushCommand(System.Action command)
        {
            RenderEndCommandExecutor.PushCommand(command);
        }

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

        protected bool mShowing = false;


        protected abstract void OnInit();
        protected abstract void OnDispose();
    }




    public static class WindowExtension
    {
        public static T PushCommand<T>(this T view, System.Action command) where T : IMGUIView
        {
            RenderEndCommandExecutor.PushCommand(command);
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
            Log.I(assetRootPath);
            Log.I(Path.GetFullPath(absPath));
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

            Log.I(standardOutput);
            if (standardError.Length > 0)
            {
                if (isThrowExcpetion)
                {
                    Log.E(standardError);
                    throw new Exception(standardError);
                }

                Log.E(standardError);
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


    public class EnumPopupView : View
    {
        public Property<Enum> ValueProperty { get; set; }

        public EnumPopupView(Enum initValue)
        {
            ValueProperty = new Property<Enum>(initValue);
            ValueProperty.Value = initValue;
            Style = new GUIStyleProperty(() => EditorStyles.popup);
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