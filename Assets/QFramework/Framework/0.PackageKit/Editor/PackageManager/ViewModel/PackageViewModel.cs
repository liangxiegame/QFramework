using System.IO;
using System.Linq;
using BindKit.Commands;
using BindKit.ViewModels;
using UnityEditor;
using UnityEngine;

namespace QFramework.PackageKit
{
    public class PackageViewModel : ViewModelBase
    {

        
        public SimpleCommand<PackageRepository> Import
        {
            get
            {
                return new SimpleCommand<PackageRepository>(packageData =>
                {
                    PackageApplication.Container.Resolve<PackageKitWindow>().Close();
                    
                    InstallPackage.Do(packageData);
                });
            }
        }
        
        public SimpleCommand<PackageRepository> Update
        {
            get
            {
                return new SimpleCommand<PackageRepository>(packageData =>
                {

                    var path = Application.dataPath.Replace("Assets", packageData.installPath);

                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }

                    RenderEndCommandExecuter.PushCommand(() =>
                    {
                        AssetDatabase.Refresh();
                        
                        PackageApplication.Container.Resolve<PackageKitWindow>().Close();

                        InstallPackage.Do(packageData);
                    });
                    
                });
            }
        }
        
        public SimpleCommand<PackageRepository> Reimport
        {
            get
            {
                return new SimpleCommand<PackageRepository>(packageData =>
                {
                    var path = Application.dataPath.Replace("Assets", packageData.installPath);

                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }

                    RenderEndCommandExecuter.PushCommand(() =>
                    {
                        AssetDatabase.Refresh();

                        PackageApplication.Container.Resolve<PackageKitWindow>().Close();
                        
                        InstallPackage.Do(packageData);
                    });
                });
            }
        }

        public SimpleCommand<PackageData> OpenReadme
        {
            get
            {
                return new SimpleCommand<PackageData>(packageData =>
                {
                    ReadmeWindow.Init(packageData.readme, packageData.PackageVersions.First());
                });
            }
        }
        
        public SimpleCommand<PackageRepository> OpenDetail
        {
            get
            {
                return new SimpleCommand<PackageRepository>(repository =>
                {
                    Application.OpenURL("https://liangxiegame.com/qf/package/detail/" + repository.id);
                });
            }
        }
    }
}