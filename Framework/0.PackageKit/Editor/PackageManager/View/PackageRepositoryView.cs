namespace QFramework.PackageKit
{
    public class PackageRepositoryView : HorizontalLayout
    {
        public PackageRepositoryView(PackageRepository packageRepository)
        {
            new SpaceView(2).AddTo(this);

            new LabelView(packageRepository.name).FontBold().Width(200).AddTo(this);

            new LabelView(packageRepository.latestVersion).TextMiddleCenter().Width(80).AddTo(this);

            var installedPackage = InstalledPackageVersions.FindVersionByName(packageRepository.name);

            new LabelView(installedPackage != null ? installedPackage.Version : " ").TextMiddleCenter().Width(100)
                .AddTo(this);

            new LabelView(packageRepository.accessRight).TextMiddleLeft().Width(50).AddTo(this);

            // 数据绑定
            var bindingSet = BindKit.CreateBindingSet(this, new PackageViewModel());
            
            if (installedPackage == null)
            {
                bindingSet.Bind(new ButtonView(LocaleText.Import).Width(90).AddTo(this))
                    .For(v => v.OnClick)
                    .To(vm => vm.Import)
                    .CommandParameter(packageRepository);
            }
            else if (packageRepository.VersionNumber > installedPackage.VersionNumber)
            {
                bindingSet.Bind(new ButtonView(LocaleText.Update).Width(90).AddTo(this))
                    .For(v => v.OnClick)
                    .To(vm => vm.Update)
                    .CommandParameter(packageRepository);
            }
            else if (packageRepository.VersionNumber == installedPackage.VersionNumber)
            {
                bindingSet.Bind(new ButtonView(LocaleText.Reimport).Width(90).AddTo(this))
                    .For(v => v.OnClick)
                    .To(vm => vm.Reimport)
                    .CommandParameter(packageRepository);

            }
            else if (packageRepository.VersionNumber < installedPackage.VersionNumber)
            {
                new SpaceView(94).AddTo(this);
            }
            
            bindingSet.Bind(new ButtonView(LocaleText.ReleaseNotes).Width(100)
                    .AddTo(this))
                .For(v => v.OnClick)
                .To(vm => vm.OpenDetail)
                .CommandParameter(packageRepository);

            new LabelView(packageRepository.author)
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
                get { return Language.IsChinese ? "详情" : "Detail"; }
            }
        }
    }
}