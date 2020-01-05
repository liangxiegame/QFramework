using System.Collections.Generic;
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
        private ToolbarView mAccessRightView = null;

        public List<PackageData> PackageDatas
        {
            get { return null; }
            set { OnRefresh(value); }
        }

        public List<string> Categories
        {
            get { return null; }
            set { mCategoriesSelectorView.Menus(value); }
        }
        public void Init(IQFrameworkContainer container)
        {
            var bindingSet = BindKit.CreateBindingSet(this, new PackageManagerViewModel()
                .InjectSelfWithContainer(mPackageManagerApp.Container)
                .Init());

            mRootLayout = new VerticalLayout();
            var treeNode = new TreeNode(true, LocaleText.FrameworkPackages).AddTo(mRootLayout);

            var verticalLayout = new VerticalLayout("box");

            treeNode.Add2Spread(verticalLayout);

            
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
                        bindingSet.Bind(search.Content)
                            .For(v => v.OnValueChanged)
                            .To(vm => vm.Search)
                            .CommandParameter(search.Content);
                    })
            );

            mAccessRightView = new ToolbarView()
                .Menus(new List<string>(){"all",PackageAccessRight.Public.ToString(),PackageAccessRight.Private.ToString()})
                .AddTo(verticalLayout)
                .Do(self =>
                {
                    bindingSet.Bind(self.Index).For(v => v.Value, v => v.OnValueChanged)
                        .To(vm => vm.AccessRightIndex);
                });
            
            mCategoriesSelectorView = new ToolbarView()
                .AddTo(verticalLayout)
                .Do(self =>
                {
                    bindingSet.Bind(self.Index).For(v => v.Value, v => v.OnValueChanged)
                        .To(vm => vm.CategoryIndex);
                });
            
            new HeaderView()
                .AddTo(verticalLayout);

            var packageList = new VerticalLayout("box")
                .AddTo(verticalLayout);

            mScrollLayout = new ScrollLayout()
                .Height(240)
                .AddTo(packageList);

            // 执行
            TypeEventSystem.Send<IEditorStrangeMVCCommand>(new PackageManagerStartUpCommand());

            bindingSet.Bind().For((v) => v.PackageDatas)
                .To(vm => vm.PackageDatas);

            bindingSet.Bind().For(v => v.Categories)
                .To(vm => vm.Categories);
            
            bindingSet.Build();
        }

        private ScrollLayout mScrollLayout = null;

        private void OnRefresh(List<PackageData> packageDatas)
        {
            mScrollLayout.Clear();
            mScrollLayout.AddChild(new SpaceView(2));

            foreach (var packageData in packageDatas)
            {
                mScrollLayout
                    .AddChild(new SpaceView(2))
                    .AddChild(new PackageView(packageData));
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

            BindKit.ClearBindingSet(this);
            
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