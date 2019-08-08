using QF;
using UnityEngine;

namespace QF.GraphDesigner
{
    public static class DrawerExtensions
    {
        public static IQFrameworkContainer RegisterGraphItem<TModel, TViewModel, TDrawer>(this IQFrameworkContainer container)
        {
            container.RegisterDataViewModel<TModel, TViewModel>();
            container.RegisterDrawer<TViewModel, TDrawer>();
            return container;
        }

        public static void RegisterItemDrawer<TViewModel, TDrawer>(this IQFrameworkContainer container)
        {
            container.RegisterRelation<TViewModel, IDrawer, TDrawer>();
        }
        public static void RegisterDrawer<TViewModel, TDrawer>(this IQFrameworkContainer container)
        {
            container.RegisterRelation<TViewModel, IDrawer, TDrawer>();
        }

        public static IQFrameworkContainer RegisterChildGraphItem<TModel, TViewModel, TDrawer>(this IQFrameworkContainer container)
        {
            container.RegisterRelation<TModel, ItemViewModel, TViewModel>();
            container.RegisterItemDrawer<TViewModel, TDrawer>();
            return container;
        }

        public static NodeConfig<TNodeData> AddNode<TNodeData, TNodeViewModel, TNodeDrawer>(this IQFrameworkContainer container, string name) where TNodeData : GenericNode, IConnectable
        {

            container.AddItem<TNodeData>();
            container.RegisterGraphItem<TNodeData, TNodeViewModel, TNodeDrawer>();
            var config = container.GetNodeConfig<TNodeData>();
            config.Name = name;
            return config;
        }
        public static NodeConfig<TNodeData> AddNode<TNodeData>(this IQFrameworkContainer container, string tag = null)
            where TNodeData : GenericNode
        {
            var config = container.AddNode<TNodeData, ScaffoldNode<TNodeData>.ViewModel, ScaffoldNode<TNodeData>.Drawer>(tag);
            return config;
        }


        public static IQFrameworkContainer AddItem<TNodeData, TNodeViewModel, TNodeDrawer>(this IQFrameworkContainer container) where TNodeData : IDiagramNodeItem
        {
            container.RegisterChildGraphItem<TNodeData, TNodeViewModel, TNodeDrawer>();
            return container;
        }
        public static IQFrameworkContainer AddItem<TNodeData>(this IQFrameworkContainer container) where TNodeData : IDiagramNodeItem
        {
            container.RegisterChildGraphItem<TNodeData, ScaffoldNodeChildItem<TNodeData>.ViewModel, ScaffoldNodeChildItem<TNodeData>.Drawer>();
            return container;
        }
        public static IQFrameworkContainer AddTypeItem<TNodeData>(this IQFrameworkContainer container) where TNodeData : ITypedItem
        {
            container.AddItem<TNodeData>();
            container.RegisterChildGraphItem<TNodeData,
                ScaffoldNodeTypedChildItem<TNodeData>.ViewModel,
                ScaffoldNodeTypedChildItem<TNodeData>.Drawer>();
            return container;
        }
        public static IQFrameworkContainer AddTypeItem<TNodeData, TViewModel, TDrawer>(this IQFrameworkContainer container) where TNodeData : ITypedItem
        {
            container.AddItem<TNodeData>();
            container.RegisterChildGraphItem<TNodeData,
                TViewModel,
                TDrawer>();
            return container;
        }
        public static NodeConfig<TGraphNode> AddGraph<TGraphType, TGraphNode>(this IQFrameworkContainer container, string name)
            where TGraphType : IGraphData
            where TGraphNode : GenericNode, new()
        {

            container.Register<IGraphData, TGraphType>(name);
            return AddNode<TGraphNode>(container, name);
        }
        public static IQFrameworkContainer RegisterGraphItem<TModel>(this QFrameworkContainer container) where TModel : GenericNode
        {
            container.RegisterGraphItem<TModel, ScaffoldNode<TModel>.ViewModel, ScaffoldNode<TModel>.Drawer>();
            //RegisterDrawer();
            return container;
        }
    }
}