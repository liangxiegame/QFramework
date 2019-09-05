using System.Collections.Generic;
using QF.GraphDesigner.Unity.KoinoniaSystem.Classes;
using QF.GraphDesigner.Unity.KoinoniaSystem.Data;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Services
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