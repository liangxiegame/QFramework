using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using QF;

namespace QF.GraphDesigner
{
    public class MultiThreadingSystem : CorePlugin, IExecuteCommand<BackgroundTaskCommand>
    {

        public override bool Enabled
        {
            get { return true; }
            set { }
        }

        public override void Loaded(QFrameworkContainer container)
        {
        }

        public void Execute(BackgroundTaskCommand command)
        {
            BackgroundWorker worker = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            InvertApplication.Log("Creating background task");
            worker.DoWork += (sender, args) =>
            {
               
                InvertApplication.Log("Executing background task");
                var bgCommand = args.Argument as BackgroundTaskCommand;
               
                if (bgCommand != null)
                {
                    bgCommand.Command.Worker = sender as BackgroundWorker;
                    bgCommand.Action(bgCommand.Command);
                }
                   

              

            };
            worker.ProgressChanged += (sender, args) =>
            {
                InvertApplication.Log("PROGRESS");
                InvertApplication.SignalEvent<ICommandProgressEvent>(_=>_.Progress(null,args.UserState.ToString(),args.ProgressPercentage));
            };
            command.Task = new BackgroundTask(worker);
            worker.RunWorkerAsync(command);
       
        }



    }
}




