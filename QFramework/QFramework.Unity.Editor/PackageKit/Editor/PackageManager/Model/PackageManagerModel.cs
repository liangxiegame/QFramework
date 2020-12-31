using System.Collections.Generic;
using UnityEditor;

namespace QFramework
{
    public interface IPackageManagerModel : IModel
    {
        List<PackageRepository> Repositories { get; set; }
    }

    class PackageManagerModel : Model<PackageKit>, IPackageManagerModel
    {
        public PackageManagerModel()
        {
            Repositories = PackageInfosRequestCache.Get().PackageRepositories;
        }

        public List<PackageRepository> Repositories { get; set; }

        public bool VersionCheck
        {
            get { return EditorPrefs.GetBool("QFRAMEWORK_VERSION_CHECK", true); }
            set { EditorPrefs.SetBool("QFRAMEWORK_VERSION_CHECK", value); }
        }
    }
}