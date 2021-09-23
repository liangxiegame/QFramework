/****************************************************************************
 * Copyright 2019.1 ~ 2020.10 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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


using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace QFramework
{
    [DisplayName("UIKit 设置")]
    [PackageKitRenderOrder(3)]
    [PackageKitGroup("QFramework")]
    public class UIKitSettingView : VerticalLayout, IPackageKitView
    {
        private UIKitSettingData mUiKitSettingData;

        public UIKitSettingView()
        {
            mUiKitSettingData = UIKitSettingData.Load();
        }

        public IQFrameworkContainer Container { get; set; }

        public int RenderOrder
        {
            get { return 0; }
        }

        public bool Ignore { get; private set; }

        public bool Enabled
        {
            get { return true; }
        }

        private IVerticalLayout mRootLayout = null;

        private UIKitSettingViewModel mViewModel;

        public void Init(IQFrameworkContainer container)
        {
            mViewModel = new UIKitSettingViewModel();

            EasyIMGUI.Label().Text(LocaleText.UIKitSettings).FontSize(12).Parent(this);

            mRootLayout = EasyIMGUI.Vertical().Box().Parent(this);

            mRootLayout.AddChild(EasyIMGUI.Space().Pixel(6));

            // 命名空间
            var nameSpaceLayout = new HorizontalLayout()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.Namespace)
                .FontSize(12)
                .FontBold()
                .Width(200)
                .Parent(nameSpaceLayout);

            EasyIMGUI.TextField().Text(mUiKitSettingData.Namespace)
                .Parent(nameSpaceLayout)
                .Content.Bind(content => mUiKitSettingData.Namespace = content);

            // UI 生成的目录
            EasyIMGUI.Space().Pixel(6)
                .Parent(mRootLayout);

            var uiScriptGenerateDirLayout = new HorizontalLayout()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.UIScriptGenerateDir)
                .FontSize(12)
                .FontBold()
                .Width(200)
                .Parent(uiScriptGenerateDirLayout);

            EasyIMGUI.TextField().Text(mUiKitSettingData.UIScriptDir)
                .Parent(uiScriptGenerateDirLayout)
                .Content.Bind(content => mUiKitSettingData.UIScriptDir = content);

            mRootLayout.AddChild(EasyIMGUI.Space().Pixel(6));

            var uiPanelPrefabDir = new HorizontalLayout()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.UIPanelPrefabDir)
                .FontSize(12)
                .FontBold()
                .Width(200)
                .Parent(uiPanelPrefabDir);

            EasyIMGUI.TextField().Text(mUiKitSettingData.UIPrefabDir)
                .Parent(uiPanelPrefabDir)
                .Content.Bind(content => mUiKitSettingData.UIPrefabDir = content);

            mRootLayout.AddChild(EasyIMGUI.Space().Pixel(6));

            // UI 生成的目录
            EasyIMGUI.Space().Pixel(6)
                .Parent(mRootLayout);

            var viewControllerScriptGenerateDirLayout = new HorizontalLayout()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.ViewControllerScriptGenerateDir)
                .FontSize(12)
                .FontBold()
                .Width(200)
                .Parent(viewControllerScriptGenerateDirLayout);

            EasyIMGUI.TextField().Text(mUiKitSettingData.DefaultViewControllerScriptDir)
                .Parent(viewControllerScriptGenerateDirLayout)
                .Content.Bind(content => mUiKitSettingData.DefaultViewControllerScriptDir = content);


            mRootLayout.AddChild(EasyIMGUI.Space().Pixel(6));

            var viewControllerPrefabDir = new HorizontalLayout()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.ViewControllerPrefabGenerateDir)
                .FontSize(12)
                .FontBold()
                .Width(220)
                .Parent(viewControllerPrefabDir);

            EasyIMGUI.TextField().Text(mUiKitSettingData.DefaultViewControllerPrefabDir)
                .Parent(viewControllerPrefabDir)
                .Content.Bind(content => mUiKitSettingData.DefaultViewControllerPrefabDir = content);

            mRootLayout.AddChild(EasyIMGUI.Space().Pixel(6));

            // 保存数据
            EasyIMGUI.Button()
                .Text(LocaleText.Apply)
                .OnClick(() => { mUiKitSettingData.Save(); })
                .Parent(mRootLayout);

            EasyIMGUI.TextField().Text(mViewModel.PanelNameToCreate)
                .Parent(mRootLayout)
                .Self(text => { text.Content.Bind(txt => mViewModel.PanelNameToCreate = txt); });

            // 创建 UI 界面 按钮的绑定
            EasyIMGUI.Button()
                .Text(LocaleText.CreateUIPanel)
                .Parent(mRootLayout)
                .Self(btn => btn.OnClick(() => { mViewModel.OnCreateUIPanelClick(); }));
        }

        public void OnUpdate()
        {
        }
        

        void IPackageKitView.OnGUI()
        {
            this.DrawGUI();
        }

        public void OnDispose()
        {
        }

        public new void OnShow()
        {
            
        }

        public new void OnHide()
        {
            
        }

        class LocaleText
        {
            public static string Namespace
            {
                get { return Language.IsChinese ? " 默认命名空间:" : " Namespace:"; }
            }

            public static string UIScriptGenerateDir
            {
                get { return Language.IsChinese ? " UI 脚本生成路径:" : " UI Scripts Generate Dir:"; }
            }

            public static string UIPanelPrefabDir
            {
                get { return Language.IsChinese ? " UIPanel Prefab 路径:" : " UIPanel Prefab Dir:"; }
            }

            public static string ViewControllerScriptGenerateDir
            {
                get { return Language.IsChinese ? " ViewController 脚本生成路径:" : " Default ViewController Generate Dir:"; }
            }

            public static string ViewControllerPrefabGenerateDir
            {
                get
                {
                    return Language.IsChinese
                        ? " ViewController Prefab 生成路径:"
                        : " Default ViewController Prefab Dir:";
                }
            }

            public static string Apply
            {
                get { return Language.IsChinese ? "保存" : "Apply"; }
            }

            public static string UIKitSettings
            {
                get { return Language.IsChinese ? "UI Kit 设置" : "UI Kit Settings"; }
            }

            public static string CreateUIPanel
            {
                get { return Language.IsChinese ? "创建 UI Panel" : "Create UI Panel"; }
            }
        }
    }
}