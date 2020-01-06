namespace QFramework.PackageKit
{
    public class PackageView : HorizontalLayout
    {
        public PackageView(PackageData packageData)
        {
            new SpaceView(2).AddTo(this);

            new LabelView(packageData.Name).FontBold().Width(200).AddTo(this);

            new LabelView(packageData.version).TextMiddleCenter().Width(80).AddTo(this);

            var installedPackage = InstalledPackageVersions.FindVersionByName(packageData.Name);

            new LabelView(installedPackage != null ? installedPackage.Version : " ").TextMiddleCenter().Width(100)
                .AddTo(this);

            new LabelView(packageData.AccessRight.ToString()).TextMiddleLeft().Width(50).AddTo(this);

            // 数据绑定
            var bindingSet = BindKit.CreateBindingSet(this, new PackageViewModel());
            
            if (installedPackage == null)
            {
                bindingSet.Bind(new ButtonView(LocaleText.Import).Width(90).AddTo(this))
                    .For(v => v.OnClick)
                    .To(vm => vm.Import)
                    .CommandParameter(packageData);
            }
            else if (packageData.VersionNumber > installedPackage.VersionNumber)
            {
                bindingSet.Bind(new ButtonView(LocaleText.Update).Width(90).AddTo(this))
                    .For(v => v.OnClick)
                    .To(vm => vm.Update)
                    .CommandParameter(packageData);
            }
            else if (packageData.VersionNumber == installedPackage.VersionNumber)
            {
                bindingSet.Bind(new ButtonView(LocaleText.Reimport).Width(90).AddTo(this))
                    .For(v => v.OnClick)
                    .To(vm => vm.Reimport)
                    .CommandParameter(packageData);

            }
            else if (packageData.VersionNumber < installedPackage.VersionNumber)
            {
                new SpaceView(94).AddTo(this);
            }
            
            bindingSet.Bind(new ButtonView(LocaleText.ReleaseNotes).Width(100)
                    .AddTo(this))
                .For(v => v.OnClick)
                .To(vm => vm.OpenReadme)
                .CommandParameter(packageData);

            new LabelView(packageData.Author)
                .TextMiddleLeft()
                .FontBold().Width(100)
                .AddTo(this);

            
            bindingSet.Build();
        }
        
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
    }
}