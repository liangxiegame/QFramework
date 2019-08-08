using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using QF.GraphDesigner;
using QF.GraphDesigner.Systems;
using QF;

namespace QF.GraphDesigner
{
    public class WorkspaceService : RepoService,
        IRemoveWorkspace,
        IContextMenuQuery,
        IToolbarQuery,
        IExecuteCommand<SelectWorkspaceCommand>,
        IExecuteCommand<SelectGraphCommand>,
        IExecuteCommand<OpenWorkspaceCommand>,
        IExecuteCommand<CreateWorkspaceCommand>,
        IExecuteCommand<RemoveWorkspaceCommand>,
        IExecuteCommand<ConfigureWorkspaceCommand>
    {
        public IEnumerable<Workspace> Workspaces
        {
            get
            {
                if (Repository == null) yield break;
                foreach (var item in Repository.AllOf<Workspace>()) yield return item;
            }
        }


        public void RemoveWorkspace(string name)
        {
            RemoveWorkspace(Workspaces.FirstOrDefault(p => p.Name == name));
        }

        public void RemoveWorkspace(Workspace workspace)
        {
            Repository.Remove(workspace);
        }

        public Workspace CurrentWorkspace { get; set; }
        public override void Loaded(QFrameworkContainer container)
        {
            base.Loaded(container);
            if (CurrentWorkspace == null && InvertGraphEditor.Prefs != null)
            {
                CurrentWorkspace = Workspaces.FirstOrDefault(p => p.Identifier == InvertGraphEditor.Prefs.GetString("LastLoadedWorkspace", string.Empty));
            }
            Configurations = container.ResolveAll<WorkspaceConfiguration>().ToDictionary(p => p.WorkspaceType);

        }

        public override decimal LoadPriority
        {
            get { return -5;  }
        }

        public WorkspaceConfiguration CurrentConfiguration
        {
            get
            {
                if (Configurations == null || CurrentWorkspace == null) return null;
                if (!Configurations.ContainsKey(CurrentWorkspace.GetType())) return null;
                return Configurations[CurrentWorkspace.GetType()];
            }
        }
        public Dictionary<Type, WorkspaceConfiguration> Configurations { get; set; }


        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, params object[] obj)
        {
           
        }

        public void Execute(SelectWorkspaceCommand command)
        {
            Signal<IShowContextMenu>(_ => _.Show(null, command));
        }

        public void Execute(SelectGraphCommand command)
        {
            Signal<IShowContextMenu>(_ => _.Show(null, command));
        }

        public void QueryToolbarCommands(ToolbarUI ui)
        {

        
        }

        public void Execute(OpenWorkspaceCommand command)
        {
            if (command.Workspace == CurrentWorkspace) return;
            if (command.Workspace == null) return;

            CurrentWorkspace = command.Workspace;
             InvertGraphEditor.Prefs.SetString("LastLoadedWorkspace", command.Workspace.Identifier);
            Signal<IWorkspaceChanged>(_ => _.WorkspaceChanged(CurrentWorkspace));
        }

        public void Execute(CreateWorkspaceCommand command)
        {
            var workspace = Activator.CreateInstance(command.WorkspaceType) as Workspace;

            if (workspace == null)
            {
                throw new Exception("Workspace cannot be created! If you are using custom workspace type, make sure it derives from Workspace class.");
            }
            workspace.Name = command.Name;
            command.Result = workspace;
            Repository.Add(workspace);
            Execute(new OpenWorkspaceCommand()
            {
                Workspace = workspace
            });

            InvertApplication.SignalEvent<INotify>(_ => _.Notify(command.Name + " workspace has been created!", NotificationIcon.Info));

        }

        public void Execute(RemoveWorkspaceCommand command)
        {
            Repository.Remove(command.Workspace);
        }

        public void Execute(ConfigureWorkspaceCommand command)
        {
            command.Workspace.Name = command.Name;
            Signal<INotify>(_=>_.Notify("Workspace configured!",NotificationIcon.Info));
        }
    }
}
