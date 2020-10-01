using QFramework.PackageKit.Command;
using QFramework.PackageKit.Model;

namespace QFramework.PackageKit
{
    public class PackageRepositoryView : HorizontalLayout
    {
        ControllerNode<PackageKit> mControllerNode = ControllerNode<PackageKit>.Allocate();
        public PackageRepositoryView(PackageRepository packageRepository)
        {
            new SpaceView(2).AddTo(this);

            new LabelView(packageRepository.name).FontBold().Width(200).AddTo(this);

            new LabelView(packageRepository.latestVersion).TextMiddleCenter().Width(80).AddTo(this);

            var installedPackage = mControllerNode.GetModel<IInstalledPackageVersionsConfigModel>()
                .GetByName(packageRepository.name);

            new LabelView(installedPackage != null ? installedPackage.Version : " ").TextMiddleCenter().Width(100)
                .AddTo(this);

            new LabelView(packageRepository.accessRight).TextMiddleLeft().Width(50).AddTo(this);
            
            
            if (installedPackage == null)
            {
                new ButtonView(LocaleText.Import).Width(90).AddTo(this)
                    .OnClick.AddListener(() =>
                    {
                        mControllerNode.SendCommand(new ImportPackageCommand(packageRepository));
                    });
            }
            else if (packageRepository.VersionNumber > installedPackage.VersionNumber)
            {
                new ButtonView(LocaleText.Update).Width(90).AddTo(this)
                    .OnClick.AddListener(() =>
                    {
                        mControllerNode.SendCommand(new UpdatePackageCommand(packageRepository));
                    });
            }
            else if (packageRepository.VersionNumber == installedPackage.VersionNumber)
            {
                new ButtonView(LocaleText.Reimport).Width(90).AddTo(this)
                    .OnClick.AddListener(() =>
                    {
                        mControllerNode.SendCommand(new ReimportPackageCommand(packageRepository));
                    });

            }
            else if (packageRepository.VersionNumber < installedPackage.VersionNumber)
            {
                new SpaceView(94).AddTo(this);
            }

            new ButtonView(LocaleText.ReleaseNotes)
                .Width(100)
                .AddTo(this)
                .OnClick.AddListener(() =>
                {
                    mControllerNode.SendCommand(new OpenDetailCommand(packageRepository));
                });

            new LabelView(packageRepository.author)
                .TextMiddleLeft()
                .FontBold().Width(100)
                .AddTo(this);
        }

        protected override void OnDisposed()
        {
            mControllerNode.Recycle2Cache();
            mControllerNode = null;
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