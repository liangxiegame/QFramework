using System.ComponentModel;
using QFramework.GraphDesigner;
using Invert.Data;

namespace QFramework.GraphDesigner
{
    public class ValidateDatabaseCommand : Command, IBackgroundCommand
    {
        public IRepository Repository { get; set; }
        public BackgroundTask Task { get; set; }
        public BackgroundWorker Worker { get; set; }
        public IGraphConfiguration GraphConfiguration { get; set; }
        public string FullPath { get; set; }
    }
}