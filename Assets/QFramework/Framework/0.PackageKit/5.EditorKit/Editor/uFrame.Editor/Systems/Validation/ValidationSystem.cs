using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF.GraphDesigner;
using Invert.Data;
using QF;
using JetBrains.Annotations;

namespace QF.GraphDesigner
{
    public class ValidationSystem : DiagramPlugin, 
        IDataRecordPropertyChanged, 
        //ICommandProgressEvent,
        IExecuteCommand<ValidateDatabaseCommand>,
        IDataRecordInserted,
        IDataRecordRemoved,
        ICommandExecuted,
        IQueryErrors
    {
        public void QueryErrors(List<ErrorInfo> errorInfo)
        {
            foreach (var item in ErrorNodes)
            {
                errorInfo.AddRange(item.Errors);
            }
        }
        public bool shouldRestart = false;
        private List<IDiagramNode> _itemsToValidate;
        private List<ErrorInfo> _errorInfo;
        private List<IDiagramNode> _errorNodes;

        public override void Loaded(QFrameworkContainer container)
        {
            base.Loaded(container);

            //Signal<ITaskHandler>(_ => _.BeginBackgroundTask(ValidateDatabase()));
          
        }

        public List<IDiagramNode> ItemsToValidate
        {
            get { return _itemsToValidate ?? (_itemsToValidate = new List<IDiagramNode>()); }
            set { _itemsToValidate = value; }
        }

        public IEnumerator ValidateDatabase()
        {
            ErrorNodes.Clear();
            var ws = Container.Resolve<DatabaseService>();
            if (ws != null && ws.CurrentConfiguration != null && ws.CurrentConfiguration.Database != null)
            foreach (var item in ws.CurrentConfiguration.Database.AllOf<IDiagramNode>())
            {
                yield return new TaskProgress("Validating " + item.Name, 95f);
                ValidateNode(item); 
                
            }
        } 
        public IEnumerator ValidateGraph()
        {
            var ws = Container.Resolve<WorkspaceService>();
            if (ws == null || ws.CurrentWorkspace == null || ws.CurrentWorkspace.CurrentGraph == null) yield break;
            var items =  ws.CurrentWorkspace.CurrentGraph.NodeItems.ToArray();
            var total = 100f / items.Length;
            for (int index = 0; index < items.Length; index++)
            {
                var item = items[index];
                yield return new TaskProgress("Validating " + item.Name, index * total);
                ValidateNode(item);
            }

        }
        public void PropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
        {
            if (name == "Selected") return;
            QueueValidate(record);
        }

 
        public List<IDiagramNode> ErrorNodes
        {
            get { return _errorNodes ?? (_errorNodes = new List<IDiagramNode>()); }
            set { _errorNodes = value; }
        }

        private void ValidateNode(IDiagramNode node)
        {
            ErrorNodes.Remove(node);
            var list = new List<ErrorInfo>();
            node.Validate(list);
            node.Errors = list.ToArray();
            if (node.Errors.Length > 0)
            {
                ErrorNodes.Add(node);
            }
            Signal<INodeValidated>(_=>_.NodeValidated(node));
        }


        public BackgroundTask ValidationTask { get; set; }

        [InspectorProperty]
        public bool ConstantValidation
        {
            get { return InvertGraphEditor.Prefs.GetBool("uFrame_ConstantValidation",false); }
            set { InvertGraphEditor.Prefs.SetBool("uFrame_ConstantValidation",value); }
        }

        public void Progress(ICommand command, string message, float progress)
        {
            InvertApplication.Log(message);
        }

        public void Execute(ValidateDatabaseCommand command)
        {
           //InvertApplication.Log("YUP");
            var list = new List<ErrorInfo>();
            var repo = new TypeDatabase(new JsonRepositoryFactory(command.FullPath));
            var items = repo.AllOf<IDiagramNode>();
            foreach (IDiagramNode t in items)
            {
                if (command.Worker.CancellationPending) return;
                var item = t;
                var item1 = item;
                command.Worker.ReportProgress(1, item1.Name);
                item.Validate(list);
            }
        }


        public void CommandExecuted(ICommand command)
        {
            if (command is SaveCommand)
            {
                Signal<ITaskHandler>(_ => _.BeginBackgroundTask(ValidateGraph()));
                return;
            }
            if (!ConstantValidation) return;
            if (command is SaveAndCompileCommand) return;
            if (ShouldRevalidate)
            Signal<ITaskHandler>(_ => _.BeginBackgroundTask(ValidateGraph()));

            ItemsToValidate.Clear();
            ShouldRevalidate = false;
        }

        public void RecordInserted(IDataRecord record)
        {
            QueueValidate(record);
        }

        private void QueueValidate(IDataRecord record)
        {
          
            var node = record as IDiagramNode;
            if (node != null)
            {
                ItemsToValidate.Add(node); ShouldRevalidate = true;
            }
            else
            {
                var nodeItem = record as IDiagramNodeItem;
                if (nodeItem != null)
                {
                    ShouldRevalidate = true;
                    node = nodeItem.Node;
                    if (node != null)
                    ItemsToValidate.Add(node);
                }
            }
                
        }

        public bool ShouldRevalidate { get; set; }
        public void RecordRemoved(IDataRecord record)
        {
            var node = record as IDiagramNode;
            if (node != null)
            {
                ErrorNodes.Remove(node);
            }
        }

        public IEnumerator ValidateNodes(IDiagramNode[] items)
        {
     
            var total = 100f / items.Length;
            for (int index = 0; index < items.Length; index++)
            {
                var item = items[index];
                yield return new TaskProgress("Validating " + item.Name, index * total);
                ValidateNode(item);
            }
        }
    }
}
