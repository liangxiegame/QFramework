#if UNITY_EDITOR
using System.Collections.Generic;

namespace QFramework
{
    internal class PackageManagerModel : AbstractModel
    {
        internal PackageManagerModel() => Repositories = PackageInfosRequestCache.Get().PackageRepositories;

        public List<PackageRepository> Repositories { get; set; }

        protected override void OnInit()
        {
            
        }
    }
}
#endif