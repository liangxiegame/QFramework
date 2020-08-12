using System.Collections.Generic;
using System.ComponentModel;
using QFramework.PackageKit.Command;
using QFramework.PackageKit.State;
using UnityEngine;

namespace QFramework.PackageKit
{
    [DisplayName("PackageKit 插件管理")]
    [PackageKitRenderOrder(1)]
    public class PackageManagerView : QFramework.IPackageKitView
    {
        PackageManagerApp mPackageManagerApp = new PackageManagerApp();

        private Vector2 mScrollPos;

        public IQFrameworkContainer Container { get; set; }
        
        public bool Ignore { get; private set; }

        public bool Enabled
        {
            get { return true; }
        }

        private VerticalLayout mRootLayout = new VerticalLayout();

        private ToolbarView mCategoriesSelectorView = null;
        private ToolbarView mAccessRightView = null;

        public List<string> Categories
        {
            get { return null; }
            set
            {
                mCategoriesSelectorView.Menus(value);
                Container.Resolve<PackageKitWindow>().Repaint();
            }
        }

        public List<PackageRepository> PackageRepositories
        {
            get { return null; }
            set { OnRefreshList(value); }
        }

        DisposableList mDisposableList = new DisposableList();


        public void Init(IQFrameworkContainer container)
        {
            Container = container;

            PackageManagerApp.Send<PackageManagerInitCommand>();

            mRootLayout = new VerticalLayout();

            new LabelView(LocaleText.FrameworkPackages).FontSize(12).AddTo(mRootLayout);

            var verticalLayout = new VerticalLayout("box").AddTo(mRootLayout);

            var searchView = new HorizontalLayout("box")
                .AddTo(verticalLayout);

            searchView.AddChild(new LabelView("搜索:")
                .FontBold()
                .FontSize(12)
                .Width(40));

            searchView.AddChild(
                new TextView().Height(20)
                    .Do(search =>
                    {
                        search.Content
                            .Bind(key => { PackageManagerApp.Send(new SearchCommand(key)); }).AddTo(mDisposableList);
                    })
            );

            mAccessRightView = new ToolbarView()
                .Menus(new List<string>()
                    {"all", PackageAccessRight.Public.ToString(), PackageAccessRight.Private.ToString()})
                .AddTo(verticalLayout)
                .Do(self =>
                {
                    self.Index.Bind(value =>
                    {
                        PackageManagerState.AccessRightIndex.Value = value;
                        PackageManagerApp.Send(new SearchCommand(PackageManagerState.SearchKey.Value));
                    }).AddTo(mDisposableList);
                });

            mCategoriesSelectorView = new ToolbarView()
                .AddTo(verticalLayout)
                .Do(self =>
                {
                    self.Index.Bind(value =>
                    {
                        PackageManagerState.CategoryIndex.Value = value;
                        PackageManagerApp.Send(new SearchCommand(PackageManagerState.SearchKey.Value));
                    }).AddTo(mDisposableList);
                });

            new PackageListHeaderView()
                .AddTo(verticalLayout);

            var packageList = new VerticalLayout("box")
                .AddTo(verticalLayout);

            mRepositoryList = new ScrollLayout()
                .Height(600)
                .AddTo(packageList);

            PackageManagerState.Categories.Bind(value => { Categories = value; }).AddTo(mDisposableList);

            PackageManagerState.PackageRepositories
                .Bind(list => { this.PackageRepositories = list; }).AddTo(mDisposableList);
        }

        private ScrollLayout mRepositoryList = null;


        private void OnRefreshList(List<PackageRepository> packageRepositories)
        {
            mRepositoryList.Clear();
            mRepositoryList.AddChild(new SpaceView(2));

            foreach (var packageRepository in packageRepositories)
            {
                mRepositoryList
                    .AddChild(new SpaceView(2))
                    .AddChild(new PackageRepositoryView(packageRepository));
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
            mDisposableList.Dispose();
            mDisposableList = null;
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