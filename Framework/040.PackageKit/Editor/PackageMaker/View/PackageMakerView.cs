using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework.PackageKit
{
    public class PackageMakerEditor : IMGUIEditorWindow
    {
        private PackageVersion mPackageVersion;


        ControllerNode<PackageMaker> mControllerNode = ControllerNode<PackageMaker>.Allocate();

        static string MakeInstallPath()
        {
            var path = MouseSelector.GetSelectedPathOrFallback();

            if (path.EndsWith("/"))
            {
                return path;
            }

            return path + "/";
        }

        private static void MakePackage()
        {
            var path = MouseSelector.GetSelectedPathOrFallback();

            if (!string.IsNullOrEmpty(path))
            {
                if (Directory.Exists(path))
                {
                    var installPath = MakeInstallPath();

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

            var window = (PackageMakerEditor) GetWindow(typeof(PackageMakerEditor), true);

            window.titleContent = new GUIContent(selectObject[0].name);

            window.position = new Rect(Screen.width / 2, Screen.height / 2, 258, 500);

            window.Show();
        }

        private VerticalLayout RootLayout = null;

        DisposableList mDisposableList = new DisposableList();

        private string mPublishVersion = null;

        protected override void Init()
        {
            PackageMakerState.InitState();

            RootLayout = new VerticalLayout("box");

            var editorView = new VerticalLayout().AddTo(RootLayout);
            var uploadingView = new VerticalLayout().AddTo(RootLayout);
            // var finishView = new VerticalLayout().AddTo(RootLayout);

            // 当前版本号
            var versionLine = new HorizontalLayout().AddTo(editorView);
            EasyIMGUI.Label().Text("当前版本号").Width(100).AddTo(versionLine);
            EasyIMGUI.Label().Text(mPackageVersion.Version).Width(100).AddTo(versionLine);

            // 发布版本号 
            var publishedVersionLine = new HorizontalLayout().AddTo(editorView);

            EasyIMGUI.Label().Text("发布版本号")
                .Width(100)
                .AddTo(publishedVersionLine);

            EasyIMGUI.TextField()
                .Text(mPublishVersion)
                .Width(100)
                .AddTo(publishedVersionLine)
                .Content.Bind(v => mPublishVersion = v);

            // 类型
            var typeLine = new HorizontalLayout().AddTo(editorView);
            EasyIMGUI.Label().Text("类型").Width(100).AddTo(typeLine);

            var packageType = new EnumPopupView(mPackageVersion.Type).AddTo(typeLine);

            var accessRightLine = new HorizontalLayout().AddTo(editorView);
            EasyIMGUI.Label().Text("权限").Width(100).AddTo(accessRightLine);
            var accessRight = new EnumPopupView(mPackageVersion.AccessRight).AddTo(accessRightLine);

            EasyIMGUI.Label().Text("发布说明:").Width(150).AddTo(editorView);

            var releaseNote = EasyIMGUI.TextArea().Width(245)
                .AddTo(editorView);

            PackageMakerState.InEditorView.BindWithInitialValue(value => { editorView.Visible = value; })
                .AddTo(mDisposableList);

            if (User.Logined)
            {
                EasyIMGUI.Button()
                    .Text("发布")
                    .OnClick(() =>
                    {
                        mPackageVersion.Readme.content = releaseNote.Content.Value;
                        mPackageVersion.AccessRight = (PackageAccessRight) accessRight.ValueProperty.Value;
                        mPackageVersion.Type = (PackageType) packageType.ValueProperty.Value;
                        mPackageVersion.Version = mPublishVersion;
                        mControllerNode.SendCommand(new PublishPackageCommand(mPackageVersion));
                        
                    }).AddTo(editorView);
            }

            var notice = new LabelViewWithRect("", 100, 200, 200, 200).AddTo(uploadingView);

            PackageMakerState.NoticeMessage
                .BindWithInitialValue(value => { notice.Content.Value = value; }).AddTo(mDisposableList);

            PackageMakerState.InUploadingView.BindWithInitialValue(value => { uploadingView.Visible = value; })
                .AddTo(mDisposableList);
        }


        private void OnEnable()
        {
            var selectObject = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

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

            mPackageVersion.InstallPath = MakeInstallPath();

            mPublishVersion = mPackageVersion.Version;

            var versionNumbers = mPublishVersion.Split('.');
            var lastVersionNumber = int.Parse(versionNumbers.Last());
            lastVersionNumber++;
            versionNumbers[versionNumbers.Length - 1] = lastVersionNumber.ToString();
            mPublishVersion = string.Join(".", versionNumbers);
        }

        public override void OnUpdate()
        {
        }


        public override void OnClose()
        {
            mControllerNode = null;

            mDisposableList.Dispose();
            mDisposableList = null;
        }


        public override void OnGUI()
        {
            base.OnGUI();

            RootLayout.DrawGUI();

            RenderEndCommandExecuter.ExecuteCommand();
        }
    }
}