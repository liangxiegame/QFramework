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

using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace QFramework
{
    [DisplayName("PackageKit 插件管理")]
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(1)]
    public class PackageManagerView : QFramework.IPackageKitView
    {
        ControllerNode<PackageKit> mControllerNode = ControllerNode<PackageKit>.Allocate();

        private Vector2 mScrollPos;

        public IQFrameworkContainer Container { get; set; }

        private VerticalLayout mRootLayout = new VerticalLayout();

        private IToolbar mCategoriesSelectorView = null;

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

            mControllerNode.SendCommand<PackageManagerInitCommand>();

            mRootLayout = new VerticalLayout();

            EasyIMGUI.Label().Text(LocaleText.FrameworkPackages).FontSize(12).Parent(mRootLayout);

            var verticalLayout = new VerticalLayout("box").Parent(mRootLayout);

            var searchView = EasyIMGUI
                .Horizontal()
                .Box()
                .Parent(verticalLayout);

            searchView.AddChild(EasyIMGUI.Label().Text("搜索:")
                .FontBold()
                .FontSize(12)
                .Width(40));

            searchView.AddChild(
                EasyIMGUI.TextField()
                    .Height(20)
                    .Self(search =>
                    {
                        search.Content
                            .Bind(key => { mControllerNode.SendCommand(new SearchCommand(key)); })
                            .AddTo(mDisposableList);
                    })
            );

            EasyIMGUI.Toolbar()
                .Menus(new List<string>()
                    {"all", PackageAccessRight.Public.ToString(), PackageAccessRight.Private.ToString()})
                .Parent(verticalLayout)
                .Self(self =>
                {
                    self.IndexProperty.Bind(value =>
                    {
                        PackageManagerState.AccessRightIndex.Value = value;
                        mControllerNode.SendCommand(new SearchCommand(PackageManagerState.SearchKey.Value));
                    }).AddTo(mDisposableList);
                });

            mCategoriesSelectorView = EasyIMGUI.Toolbar()
                .Parent(verticalLayout)
                .Self(self =>
                {
                    self.IndexProperty.Bind(value =>
                    {
                        PackageManagerState.CategoryIndex.Value = value;
                        mControllerNode.SendCommand(new SearchCommand(PackageManagerState.SearchKey.Value));
                    }).AddTo(mDisposableList);
                });

            new PackageListHeaderView()
                .Parent(verticalLayout);

            var packageList = new VerticalLayout("box")
                .Parent(verticalLayout);

            mRepositoryList = EasyIMGUI.Scroll()
                .Height(600)
                .Parent(packageList);

            PackageManagerState.Categories.Bind(value => { Categories = value; }).AddTo(mDisposableList);

            PackageManagerState.PackageRepositories
                .Bind(list => { this.PackageRepositories = list; }).AddTo(mDisposableList);
        }

        private IScrollLayout mRepositoryList = null;


        private void OnRefreshList(List<PackageRepository> packageRepositories)
        {
            mRepositoryList.Clear();
            mRepositoryList.AddChild(EasyIMGUI.Space().Pixel(2));

            foreach (var packageRepository in packageRepositories)
            {
                mRepositoryList
                    .AddChild(EasyIMGUI.Space().Pixel(2))
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
            mControllerNode.Recycle2Cache();
            mControllerNode = null;

            mDisposableList.Dispose();
            mDisposableList = null;
            mCategoriesSelectorView = null;
        }

        public void OnShow()
        {
            
        }

        public void OnHide()
        {
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