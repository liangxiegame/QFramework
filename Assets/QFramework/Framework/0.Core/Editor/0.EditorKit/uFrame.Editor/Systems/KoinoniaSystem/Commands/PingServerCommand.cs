using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using QFramework.GraphDesigner;

namespace QFramework.GraphDesigner.Unity.KoinoniaSystem.Commands
{
    public class PingServerCommand : IBackgroundCommand
    {
        public BackgroundWorker Worker { get; set; }
        public string Server { get; set; }
        public string Title { get; set; }
    }
}
