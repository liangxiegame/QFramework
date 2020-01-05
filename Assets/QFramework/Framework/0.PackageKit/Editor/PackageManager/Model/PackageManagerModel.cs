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

        public List<PackageRepository> Repositories = new List<PackageRepository>();

        public bool VersionCheck
        {
            get { return EditorPrefs.GetBool("QFRAMEWORK_VERSION_CHECK", true); }
            set { EditorPrefs.SetBool("QFRAMEWORK_VERSION_CHECK", value); }
        }


        public IEnumerable<PackageData> SelectedPackageType
        {
            get { return PackageDatas.OrderBy(p => p.Name); }
        }
    }
}