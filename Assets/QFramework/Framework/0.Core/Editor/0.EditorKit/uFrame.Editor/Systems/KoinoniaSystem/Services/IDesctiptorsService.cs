using System.Collections.Generic;
using QFramework.GraphDesigner.Unity.KoinoniaSystem.Classes;
using QFramework.GraphDesigner.Unity.KoinoniaSystem.Data;

namespace QFramework.GraphDesigner.Unity.KoinoniaSystem.Services
{
    public interface IDesctiptorsService
    {
        UFramePackageDescriptor GetPackageDescriptorById(string id);
        UFramePackageDescriptor GetPackageDescriptorByRevision(UFramePackageRevisionDescriptor revision);
        UFramePackageDescriptor GetPackageDescriptorByPackage(UFramePackage revision);

        UFramePackageRevisionDescriptor GetRevisionById(string id);
        UFramePackageRevisionDescriptor GetRevisionDescriptorByPackageIdAndTag(string packageId, string tag);
        IEnumerable<UFramePackageRevisionDescriptor> GetRevisionsByProject(UFramePackageDescriptor package);

        IEnumerable<UFramePackageDescriptor> GetLatest();
        IEnumerable<UFramePackageDescriptor> Search();

    }
}