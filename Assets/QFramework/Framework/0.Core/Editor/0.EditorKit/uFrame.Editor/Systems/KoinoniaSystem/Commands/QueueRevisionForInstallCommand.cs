using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using QFramework.GraphDesigner;
using QFramework.GraphDesigner.Unity.KoinoniaSystem.Data;

namespace QFramework.GraphDesigner.Unity.KoinoniaSystem.Commands
{
    public class QueueRevisionForInstallCommand : IBackgroundCommand
    {
        public BackgroundWorker Worker { get; set; }
        public UFramePackageDescriptor PackageDescriptor { get; set; }
        public UFramePackageRevisionDescriptor RevisionDescriptor { get; set; }
        public string Title { get; set; }
    }
}
