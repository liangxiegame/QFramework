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

        
        public SimpleCommand<PackageData> Import
        {
            get
            {
                return new SimpleCommand<PackageData>(packageData =>
                {
                    PackageApplication.Container.Resolve<PackageKitWindow>().Close();
                    
                    InstallPackage.Do(packageData);
                });
            }
        }
        
        public SimpleCommand<PackageData> Update
        {
            get
            {
                return new SimpleCommand<PackageData>(packageData =>
                {

                    var path = Application.dataPath.Replace("Assets", packageData.InstallPath);

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
        
        public SimpleCommand<PackageData> Reimport
        {
            get
            {
                return new SimpleCommand<PackageData>(packageData =>
                {
                    var path = Application.dataPath.Replace("Assets", packageData.InstallPath);

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
    }
}