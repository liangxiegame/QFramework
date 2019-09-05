using System.Linq;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Classes
{
    public abstract class UFramePackage : IUFramePackage
    {
        private string _id;
        private string _versionTag;

        public string Id
        {
            get
            {
                return _id ?? (_id = GetGuidFromAttribute());
            }
            set { _id = value; }
        }

        public string VersionTag
        {
            get { return _versionTag ?? (_versionTag = GetVersionTagFromAttribute()); }
            set { _versionTag = value; }
        }

        private string GetGuidFromAttribute()
        {
            var attribute = this.GetType().GetCustomAttributes(typeof(ProjectLinkAttribute), true).FirstOrDefault() as ProjectLinkAttribute;
            if (attribute != null)
            {
                return attribute.PackageId;
            }
            return string.Empty;
        }

        private string GetVersionTagFromAttribute()
        {
            var attribute = this.GetType().GetCustomAttributes(typeof(ProjectLinkAttribute), true).FirstOrDefault() as ProjectLinkAttribute;
            if (attribute != null)
            {
                return attribute.Version;
            }
            return null;
        }

        public virtual void OnLoaded()
        {
           
        }

        public virtual void Install()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Uninstall()
        {

        }

        public virtual bool NeedsInstall()
        {
            return false;
        }

        public virtual bool NeedsUpdate()
        {
            return false;
        }
    }
}
