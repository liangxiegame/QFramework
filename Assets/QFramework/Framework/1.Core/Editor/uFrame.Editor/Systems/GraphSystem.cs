using System;
using System.Linq;
using QF.GraphDesigner;
using Invert.Data;
using QF;

public class GraphSystem : DiagramPlugin
    , IContextMenuQuery
    , IToolbarQuery
    , IExecuteCommand<CreateGraphMenuCommand>
    , IExecuteCommand<CreateGraphCommand>
    , IExecuteCommand<AddGraphToWorkspace>
    , IDataRecordInserted
    , IDataRecordRemoved
    , IDataRecordPropertyChanged
    , ICommandExecuted
{
    public override void Loaded(QFrameworkContainer container)
    {
        base.Loaded(container);
        WorkspaceService = container.Resolve<WorkspaceService>();
    }

    public WorkspaceService WorkspaceService { get; set; }

    public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, params object[] obj)
    {
        if (obj is CreateGraphMenuCommand)
        {
            var config = WorkspaceService.CurrentConfiguration;
            foreach (var item in config.GraphTypes)
            {
                ui.AddCommand(new ContextMenuItem()
                {
                    Title = item.Title ?? item.GraphType.Name,
                    Command = new CreateGraphCommand()
                    {
                        GraphType = item.GraphType,
                        Name = "New" + item.GraphType.Name
                    }
                });
            }
            
        }
        var diagram = obj.FirstOrDefault() as DiagramViewModel;
        if (diagram != null)
        {
     
            ui.AddCommand(new ContextMenuItem()
            {
                Title = "Delete This Graph",
                Group = "Z",
                Command =  new LambdaFileSyncCommand("Delete Graph", () =>
                {
                    Container.Resolve<IRepository>().Remove(diagram.DataObject as IDataRecord);
                })
            });
        }
    }

    public void QueryToolbarCommands(ToolbarUI ui)
    {
        //ui.AddCommand(new ToolbarItem()
        //{
        //    Title = "Create Graph",
        //    Position = ToolbarPosition.BottomRight,
        //    Command = new CreateGraphMenuCommand(),
        //    Order = -1
        //});

        //ui.AddCommand(new ToolbarItem()
        //{
        //    Title = "Import Graph",
        //    Position = ToolbarPosition.BottomRight,
        //    Command = new AddGraphToWorkspace()
        //});
    }

    public void Execute(CreateGraphMenuCommand command)
    {
        Signal<IShowContextMenu>(_ => _.Show(null, command));
    }

    public void Execute(AddGraphToWorkspace command)
    {
        var workspaceService = Container.Resolve<WorkspaceService>();
        var repo = Container.Resolve<IRepository>();
        var workspaceGraphs = workspaceService.CurrentWorkspace.Graphs.Select(p => p.Identifier).ToArray();
        var importableGraphs = repo.AllOf<IGraphData>().Where(p => !workspaceGraphs.Contains(p.Identifier));
        InvertGraphEditor.WindowManager.InitItemWindow(importableGraphs, _ =>
        {
            InvertApplication.Execute(new LambdaCommand("Add Graph", () =>
            {
                workspaceService.CurrentWorkspace.AddGraph(_);
            }));
      
        });
    }

    public void Execute(CreateGraphCommand command)
    {
        var workspaceService = Container.Resolve<WorkspaceService>();
        var repo = Container.Resolve<IRepository>();
        var graph = Activator.CreateInstance(command.GraphType) as IGraphData;
        repo.Add(graph);
        graph.Name = command.Name;
        workspaceService.CurrentWorkspace.AddGraph(graph);
        workspaceService.CurrentWorkspace.CurrentGraphId = graph.Identifier;
        InvertApplication.SignalEvent<INotify>(_ => _.Notify(command.Name + " graph has been created!", NotificationIcon.Info));
    }

    public void RecordInserted(IDataRecord record)
    {
        if (record is WorkspaceGraph || record is FilterStackItem || record is FilterItem || record is Workspace || record is UndoItem || record is RedoItem) return;
        if (WorkspaceService != null && WorkspaceService.CurrentWorkspace != null &&
            WorkspaceService.CurrentWorkspace.CurrentGraph != null)
        {
            var diagramItem = record as IDiagramNodeItem;
            if (diagramItem != null)
            {
                if (diagramItem.Graph != null)
                diagramItem.Graph.IsDirty = true;
            }
            else
            {
                WorkspaceService.CurrentWorkspace.CurrentGraph.IsDirty = true;    
            }
            
            //InvertApplication.Log(record.GetType().Name);
        }
    }

    public void RecordRemoved(IDataRecord record)
    {
        if (record is WorkspaceGraph || record is FilterStackItem || record is FilterItem || record is Workspace || record is UndoItem || record is RedoItem) return;
        if (WorkspaceService != null && WorkspaceService.CurrentWorkspace != null &&
            WorkspaceService.CurrentWorkspace.CurrentGraph != null)
        {
            var diagramItem = record as IDiagramNodeItem;
            if (diagramItem != null)
            {
                if (diagramItem.Graph != null)
                diagramItem.Graph.IsDirty = true;
            }
            else
            {
                WorkspaceService.CurrentWorkspace.CurrentGraph.IsDirty = true;
            }
        }
    }

    public void PropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
    {
        if (name == "IsDirty") return;
        if (record is WorkspaceGraph || record is FilterStackItem || record is FilterItem || record is Workspace || record is UndoItem || record is RedoItem) return;
        if (WorkspaceService != null && WorkspaceService.CurrentWorkspace != null &&
            WorkspaceService.CurrentWorkspace.CurrentGraph != null)
        {
            if (record is IGraphData) return;

            var diagramItem = record as IDiagramNodeItem;
            if (diagramItem != null)
            {
                diagramItem.Graph.IsDirty = true;
            }
            else
            {
                WorkspaceService.CurrentWorkspace.CurrentGraph.IsDirty = true;
            }

            WorkspaceService.CurrentWorkspace.CurrentGraph.IsDirty = true;
            //InvertApplication.Log(name + " " + record.GetType().Name);
        }
    }

    public void CommandExecuted(ICommand command)
    {
        
    }

}