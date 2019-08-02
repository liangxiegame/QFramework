using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Invert.Data;
using QF.GraphDesigner.Unity.WindowsPlugin;
using QF;
using MessageType = QF.GraphDesigner.Unity.WindowsPlugin.MessageType;

namespace QF.GraphDesigner.Unity.LogSystem
{
    public class LogSystem : DiagramPlugin, ILogEvents, IExecuteCommand<InfiniteLoopCommand>//, ICommandProgressEvent
    {
        public static IRepository Repository { get; set; }

        public override void Loaded(QFrameworkContainer container)
        {
            base.Loaded(container);
            Repository = container.Resolve<IRepository>();
        }

        public void Log(string message, WindowsPlugin.MessageType type)
        {
            var msg = new LogMessage();
            msg.Message = message;
            msg.MessageType = type;

            Repository.Add(msg);
            //Repository.Add(msg);
            //Repository.Commit();       
        }

        public void Log<T>(T message) where T : LogMessage, new()
        {
            var msg = new T();
            Repository.Add(msg);
            //Repository.Commit();    
        }


      //  [MenuItem("uFrame Dev/Multithreading/Start Infinite Loop")]
        public static void RunInfiniteLoop()
        {
            Task = InvertApplication.ExecuteInBackground(new InfiniteLoopCommand());
        }

    //    [MenuItem("uFrame Dev/Multithreading/Stop Infinite Loop")]
        public static void StopInfiniteLoop()
        {
            Task.Cancel();
        }

        public static BackgroundTask Task { get; set; }

        public void Execute(InfiniteLoopCommand command)
        {
            while (true)
            {
                InvertApplication.SignalEvent<ILogEvents>(
                    i => i.Log(string.Format("Generated from infinite loop, {0}", DateTime.Now), MessageType.Info));
                Thread.Sleep(1000);
            }
        }

        public void Progress(ICommand command, string message, float progress)
        {
            Log(message,MessageType.Info);
        }
    }
}
