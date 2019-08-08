using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using QF.GraphDesigner.Unity.KoinoniaSystem.Classes;
using QF.GraphDesigner;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Commands
{
    public class QueueRevisionForUninstallCommand : IBackgroundCommand
    {
        public BackgroundWorker Worker { get; set; }
        public UFramePackage Package { get; set; }
        public string Title { get; set; }
    }

}
