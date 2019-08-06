namespace QF.GraphDesigner
{

    public class ScaffoldNode<TData> where TData : GenericNode
    {

#if !SERVER
        public class Drawer : GenericNodeDrawer<TData, ViewModel>
        {


            public Drawer(ViewModel viewModel)
                : base(viewModel)
            {

            }
        }

        public class ItemDrawer : ScaffoldNodeChildItem<TData>.Drawer
        {
            public ItemDrawer(ScaffoldNodeChildItem<TData>.ViewModel viewModel)
                : base(viewModel)
            {
            }
        }
        //public class ScaffoldTypedItemDrawer : ScaffoldNodeTypedChildItem<TData>.Drawer
        //{
        //    public ScaffoldTypedItemDrawer(ScaffoldNodeTypedChildItem<TData>.ViewModel viewModel)
        //        : base(viewModel)
        //    {
        //    }
        //}


        public class ItemViewModel : ScaffoldNodeChildItem<TData>.ViewModel
        {
            public ItemViewModel(TData graphItemObject, DiagramNodeViewModel diagramViewModel)
                : base(graphItemObject, diagramViewModel)
            {
            }
        }

        //public class TypedItemViewModel : ScaffoldNodeTypedChildItem<TData>.ViewModel
        //{
        //    public TypedItemViewModel(TData graphItemObject, DiagramNodeViewModel diagramViewModel)
        //        : base(graphItemObject, diagramViewModel)
        //    {
        //    }
        //}

        public class ViewModel : GenericNodeViewModel<TData>
        {
            public ViewModel(TData graphItemObject, DiagramViewModel diagramViewModel)
                : base(graphItemObject, diagramViewModel)
            {
            }
        }
#endif
        
    }
}