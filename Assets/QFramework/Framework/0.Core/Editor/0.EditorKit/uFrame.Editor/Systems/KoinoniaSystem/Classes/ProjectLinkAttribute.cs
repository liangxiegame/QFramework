using System;

namespace QFramework.GraphDesigner.Unity.KoinoniaSystem.Classes
{
    public class ProjectLinkAttribute : Attribute
    {
        public string PackageId { get; set; }
        public string Version { get; set; }

        public ProjectLinkAttribute(string packageId, string version)
        {
            PackageId = packageId;
            Version = version;
        }
    }
}