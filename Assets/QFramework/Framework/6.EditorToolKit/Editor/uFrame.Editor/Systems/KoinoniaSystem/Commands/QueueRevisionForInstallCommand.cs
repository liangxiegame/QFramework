using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using QF.GraphDesigner.Unity.KoinoniaSystem.Data;
using QF.GraphDesigner;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Commands
{
    public class QueueRevisionForInstallCommand : IBackgroundCommand
    {
        public BackgroundWorker Worker { get; set; }
        public UFramePackageDescriptor PackageDescriptor { get; set; }
        public UFramePackageRevisionDescriptor RevisionDescriptor { get; set; }
        public string Title { get; set; }
    }
}
