using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using QF.GraphDesigner;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Commands
{
    public class SelectPackageCommand : IBackgroundCommand
    {
        public string Id { get; set; }
        public BackgroundWorker Worker { get; set; }
        public string Title { get; set; }
    }
}
