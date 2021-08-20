using System.Collections.Generic;
using UnityEditor;

namespace QFramework
{
    public interface IPackageManagerModel : IModel
    {
        List<PackageRepository> Repositories { get; set; }
    }

    class PackageManagerModel : AbstractModel, IPackageManagerModel
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

        protected override void OnInit()
        {
            
        }
    }
}