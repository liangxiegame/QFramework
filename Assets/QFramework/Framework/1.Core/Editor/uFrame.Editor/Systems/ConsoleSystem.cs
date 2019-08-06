using System;
using System.Linq;
using QF.GraphDesigner;
using Invert.Data;
using QF;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity.WindowsPlugin
{
    
    public class ConsoleSystem : DiagramPlugin, IDataRecordInserted
    {

        private static ConsoleViewModel _consoleViewModel;

        public static IRepository Repository { get; set; }

        public override void Initialize(QFrameworkContainer container)
        {
            base.Initialize(container);
            container.RegisterDrawer<ConsoleViewModel,ConsoleDrawer>();
        }

        public override void Loaded(QFrameworkContainer container)
        {
            base.Loaded(container);
            Repository = container.Resolve<IRepository>();
        }

        public static ConsoleViewModel ConsoleViewModel
        {
            get { return _consoleViewModel ?? (_consoleViewModel = new ConsoleViewModel()
            {
                Messages = Repository.AllOf<LogMessage>().ToList()
            }); }
            set { _consoleViewModel = value; }
        }

        public void RecordInserted(IDataRecord record)
        {
            var item = record as LogMessage;
            if (item != null)
            {
                ConsoleViewModel.Messages.Add(item);
            }
        }


     //  [MenuItem("uFrame Dev/Console")]
        public static void ShowConsole()
        {
            InvertApplication.SignalEvent<IOpenWindow>(_=>_.OpenWindow(ConsoleViewModel,WindowType.Normal,null,new Vector2(400,600)));
        }

    }



}