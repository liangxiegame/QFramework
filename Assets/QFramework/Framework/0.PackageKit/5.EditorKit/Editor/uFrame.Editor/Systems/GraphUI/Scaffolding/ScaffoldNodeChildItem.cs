namespace QF.GraphDesigner
{
    public class ScaffoldNodeChildItem<TData> where TData : IDiagramNodeItem
    {
#if !SERVER
        public class Drawer : ItemDrawer
        {
            public Drawer(ViewModel viewModel)
                : base(viewModel)
            {
            }
        }

        public class ViewModel : GenericItemViewModel<TData>
        {
            public ViewModel(TData graphItemObject, DiagramNodeViewModel diagramViewModel)
                : base(graphItemObject, diagramViewModel)
            {
            }
        }
#endif
    }

}