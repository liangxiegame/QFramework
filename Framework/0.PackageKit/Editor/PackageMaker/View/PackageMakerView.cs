using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework.PackageKit
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

        DisposableList mDisposableList = new DisposableList();
        
        PackageMakerApp mPackageMakerApp = new PackageMakerApp();

        protected override void Init()
        {
            PackageMakerState.InitState();
            
            RootLayout = new VerticalLayout("box");
            
            var editorView = new VerticalLayout().AddTo(RootLayout);
            var uploadingView = new VerticalLayout().AddTo(RootLayout);
            // var finishView = new VerticalLayout().AddTo(RootLayout);
            
            // 当前版本号
            var versionLine = new HorizontalLayout().AddTo(editorView);
            new LabelView("当前版本号").Width(100).AddTo(versionLine);
            new LabelView(mPackageVersion.Version).Width(100).AddTo(versionLine);
            
            // 发布版本号 
            var publishedVertionLine = new HorizontalLayout().AddTo(editorView);
            new LabelView("发布版本号").Width(100).AddTo(publishedVertionLine);

            var version = new TextView().Width(100).AddTo(publishedVertionLine);
            
            // 类型
            var typeLine = new HorizontalLayout().AddTo(editorView);
            new LabelView("类型").Width(100).AddTo(typeLine);

            var packageType = new EnumPopupView(mPackageVersion.Type).AddTo(typeLine);
            
            var accessRightLine = new HorizontalLayout().AddTo(editorView);
            new LabelView("权限").Width(100).AddTo(accessRightLine);
            var accessRight = new EnumPopupView(mPackageVersion.AccessRight).AddTo(accessRightLine);

            new LabelView("发布说明:").Width(150).AddTo(editorView);

            var releaseNote = new TextAreaView().Width(250).Height(300).AddTo(editorView);
            
            PackageMakerState.InEditorView.BindWithInitialValue(value =>
            {
                editorView.Visible = value;
            }).AddTo(mDisposableList);

            if (User.Logined)
            {
                var publishBtn = new ButtonView("发布").AddTo(editorView);
                
                new ButtonView("发布并删除本地", () => { }).AddTo(editorView);

                publishBtn.OnClick.AddListener(() =>
                {
                    mPackageVersion.Readme.content = releaseNote.Content.Value;
                    mPackageVersion.AccessRight = (PackageAccessRight) accessRight.ValueProperty.Value;
                    mPackageVersion.Type = (PackageType) packageType.ValueProperty.Value;
                    mPackageVersion.Version = version.Content.Value;
                    
                    PackageMakerApp.Send(new PublishPackageCommand(mPackageVersion));
                });
            }

            var notice = new LabelViewWithRect("", 100, 200, 200, 200).AddTo(uploadingView);

            PackageMakerState.NoticeMessage
                .BindWithInitialValue(value =>
                {
                    notice.Content.Value = value;
                }).AddTo(mDisposableList);

            PackageMakerState.InUploadingView.BindWithInitialValue(value =>
            {
                uploadingView.Visible = value;
            }).AddTo(mDisposableList);
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
            mPackageMakerApp.Dispose();
            mPackageMakerApp = null;
            
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