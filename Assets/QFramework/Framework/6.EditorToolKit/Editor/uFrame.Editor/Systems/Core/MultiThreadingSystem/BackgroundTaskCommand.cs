using System;

namespace QF.GraphDesigner
{
    public class BackgroundTaskCommand : ICommand
    {
        public string Title { get;  set; }
        public Action<IBackgroundCommand> Action { get; set; }
        public BackgroundTask Task { get; set; }
        public IBackgroundCommand Command { get; set; }
    }
}