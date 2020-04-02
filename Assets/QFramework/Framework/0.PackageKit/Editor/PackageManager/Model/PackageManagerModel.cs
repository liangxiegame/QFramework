using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace QFramework.PackageKit
{
    public class PackageManagerModel
    {

        public List<PackageRepository> Repositories = new List<PackageRepository>();

        public bool VersionCheck
        {
            get { return EditorPrefs.GetBool("QFRAMEWORK_VERSION_CHECK", true); }
            set { EditorPrefs.SetBool("QFRAMEWORK_VERSION_CHECK", value); }
        }


        public IEnumerable<PackageRepository> SelectedPackageType
        {
            get { return Repositories.OrderBy(p => p.name); }
        }
    }
}