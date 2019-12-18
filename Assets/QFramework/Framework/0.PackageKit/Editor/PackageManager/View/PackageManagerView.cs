using UnityEngine;

namespace QFramework.PackageKit
{
  public class PackageManagerView : IPackageKitView
    {
        PackageManagerApp mPackageManagerApp = new PackageManagerApp();

        private Vector2 mScrollPos;

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


        private ToolbarView mCategoriesSelectorView = null;

        public void Init(IQFrameworkContainer container)
        {
// view
            mRootLayout = new VerticalLayout();
            var treeNode = new TreeNode(true, LocaleText.FrameworkPackages).AddTo(mRootLayout);

            var verticalLayout = new VerticalLayout("box");

            treeNode.Add2Spread(verticalLayout);
            
            
            mCategoriesSelectorView = new ToolbarView(0)
                .AddTo(verticalLayout);
            
            mCategoriesSelectorView.Index.Bind(newIndex =>
            {
                TypeEventSystem.Send<IEditorStrangeMVCCommand>(new PackageManagerSelectCategoryCommand()
                {
                    CategoryIndex = newIndex
                });
            });

            new HeaderView()
                .AddTo(verticalLayout);

            var packageList = new VerticalLayout("box")
                .AddTo(verticalLayout);

            mScrollLayout = new ScrollLayout()
                .Height(240)
                .AddTo(packageList);

            TypeEventSystem.Register<PackageManagerViewUpdate>(OnRefresh);

            // 执行
            TypeEventSystem.Send<IEditorStrangeMVCCommand>(new PackageManagerStartUpCommand());
            
            var bindingSet = BindKit.CreateBindingSet(this, new PackageManagerViewModel());
        }

        private ScrollLayout mScrollLayout = null;

        private void OnRefresh(PackageManagerViewUpdate viewUpdateEvent)
        {
            mScrollLayout.Clear();

            mCategoriesSelectorView.Menus(viewUpdateEvent.Categories);
            mCategoriesSelectorView.Index.Bind(newIndex =>
            {
                TypeEventSystem.Send<IEditorStrangeMVCCommand>(new PackageManagerSelectCategoryCommand()
                {
                    CategoryIndex = newIndex
                });
            });

            new SpaceView(2).AddTo(mScrollLayout);

            foreach (var packageData in viewUpdateEvent.PackageDatas)
            {
                new SpaceView(2).AddTo(mScrollLayout);
                new PackageView(packageData).AddTo(mScrollLayout);
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

            mScrollLayout = null;
            mCategoriesSelectorView = null;
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
}