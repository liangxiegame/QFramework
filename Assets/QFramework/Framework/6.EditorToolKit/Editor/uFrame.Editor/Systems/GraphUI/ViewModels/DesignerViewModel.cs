using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class DesignerViewModel : ViewModel<Workspace>
    {
        private MVVM.ObservableCollection<TabViewModel> _tabs;
 
        private WorkspaceService _workspaceService;

        public override void DataObjectChanged()
        {
            base.DataObjectChanged();
            //Tabs = Data.OpenGraphs;
        }

        public WorkspaceService WorkspaceService
        {
            get { return _workspaceService ?? (_workspaceService = InvertGraphEditor.Container.Resolve<WorkspaceService>()); }
        }
        public override object DataObject
        {
            get { return WorkspaceService.CurrentWorkspace; }
            set { base.DataObject = value; }
        }

        public TabViewModel CurrentTab { get; set; }

        public IEnumerable<IGraphData> Tabs
        {
            get { return Data.Graphs; }
        }

        public void OpenTab(IGraphData graphData, string[] path = null)
        {
            Data.CurrentGraph = graphData;
        }


    }
}