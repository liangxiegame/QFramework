using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace QFramework.PackageKit
{
  public class PackageManagerModel
    {
        public PackageManagerModel()
        {
            PackageDatas = PackageInfosRequestCache.Get().PackageDatas;
        }

        public List<PackageData> PackageDatas = new List<PackageData>();

        public List<string> Categories = new List<string>()
        {
            "Framework", "Plugin", "UIKitComponent", "Shader", "AppOrTemplate"
        };

        public int CategoryIndex
        {
            get { return EditorPrefs.GetInt("PM_TOOLBAR_INDEX", 0); }
            set { EditorPrefs.SetInt("PM_TOOLBAR_INDEX", value); }
        }

        public bool VersionCheck
        {
            get { return EditorPrefs.GetBool("QFRAMEWORK_VERSION_CHECK", true); }
            set { EditorPrefs.SetBool("QFRAMEWORK_VERSION_CHECK", value); }
        }


        public IEnumerable<PackageData> SelectedPackageType
        {
            get
            {
                switch (CategoryIndex)
                {
                    case 0:
                        return PackageDatas.Where(packageData => packageData.Type == PackageType.FrameworkModule)
                            .OrderBy(p => p.Name);
                    case 1:
                        return PackageDatas.Where(packageData => packageData.Type == PackageType.Plugin)
                            .OrderBy(p => p.Name);
                    case 2:
                        return PackageDatas.Where(packageData => packageData.Type == PackageType.UIKitComponent)
                            .OrderBy(p => p.Name);
                    case 3:
                        return PackageDatas.Where(packageData => packageData.Type == PackageType.Shader)
                            .OrderBy(p => p.Name);
                    case 4:
                        return PackageDatas.Where(packageData =>
                            packageData.Type == PackageType.AppOrGameDemoOrTemplate).OrderBy(p => p.Name);
                    case 5:
                        return PackageDatas.Where(packageData =>
                            packageData.AccessRight == PackageAccessRight.Private).OrderBy(p => p.Name);
                    case 6:
                        return PackageDatas.Where(packageData => packageData.Type == PackageType.Master)
                            .OrderBy(p => p.Name);
                    default:
                        return PackageDatas.Where(packageData => packageData.Type == PackageType.FrameworkModule)
                            .OrderBy(p => p.Name);
                }
            }
        }
    }
}