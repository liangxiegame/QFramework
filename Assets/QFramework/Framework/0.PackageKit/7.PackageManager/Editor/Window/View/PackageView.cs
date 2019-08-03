using System.Linq;
using EGO.Framework;
using QF.Action;
using QF.Extensions;
using UnityEngine;

namespace QF.Editor
{
    public class PackageView : HorizontalLayout
    {
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

        private PackageData mPackageData;
        public PackageView(PackageData packageData) : base(null)
        {

            this.mPackageData = packageData;
            
            Refresh();
        }

        protected override void OnRefresh()
        {
            Clear();
            
            new EGO.Framework.SpaceView(2).AddTo(this);
            
            new LabelView(mPackageData.Name)
                .FontBold()
                .Width(150)
                .AddTo(this);

            new LabelView(mPackageData.Version)
                .TextMiddleCenter()
                .Width(80)
                .AddTo(this);

            var installedPackage = InstalledPackageVersions.FindVersionByName(mPackageData.Name);

            new LabelView(installedPackage != null ? installedPackage.Version : " ")
                .TextMiddleCenter()
                .Width(80)
                .AddTo(this);

            new LabelView(mPackageData.AccessRight.ToString())
                .TextMiddleCenter()
                .Width(80)
                .AddTo(this);

            if (mPackageData.DocUrl.IsNotNullAndEmpty())
            {
                new ButtonView(LocaleText.Doc, () => { }).AddTo(this);
            }
            else
            {
                new EGO.Framework.SpaceView(40).AddTo(this);
            }


            if (installedPackage == null)
            {
                new ButtonView(LocaleText.Import, () =>
                    {
                        EditorActionKit.ExecuteNode(new InstallPackage(mPackageData));

                        PackageApplication.Container.Resolve<PackageKitWindow>().Close();
                    })
                    .Width(90)
                    .AddTo(this);
            }

            else if (installedPackage != null && mPackageData.VersionNumber > installedPackage.VersionNumber)
            {
                new ButtonView(LocaleText.Update, () =>
                    {
                        var path = Application.dataPath.Replace("Assets", mPackageData.InstallPath);

                        path.DeleteDirIfExists();

                        EditorActionKit.ExecuteNode(new InstallPackage(mPackageData));

                        PackageApplication.Container.Resolve<PackageKitWindow>().Close();
                    })
                    .Width(90)
                    .AddTo(this);
            }
            else if (installedPackage.IsNotNull() &&
                     mPackageData.VersionNumber == installedPackage.VersionNumber)
            {
                new ButtonView(LocaleText.Reimport, () =>
                    {
                        var path = Application.dataPath.Replace("Assets", mPackageData.InstallPath);

                        path.DeleteDirIfExists();

                        EditorActionKit.ExecuteNode(new InstallPackage(mPackageData));
                        PackageApplication.Container.Resolve<PackageKitWindow>().Close();
                    })
                    .Width(90)
                    .AddTo(this);
            }
            else if (installedPackage != null)
            {
                new EGO.Framework.SpaceView(94).AddTo(this);
            }

            new ButtonView(LocaleText.ReleaseNotes,
                    () => { ReadmeWindow.Init(mPackageData.readme, mPackageData.PackageVersions.First()); }).Width(100)
                .AddTo(this);
        }
    }
}