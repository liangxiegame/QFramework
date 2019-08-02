using System;
using System.Collections.Generic;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class GraphDesignerPlugin : DiagramPlugin,
        IConnectionEvents,
        ICommandExecuted,
        IQueryPossibleConnections,
        IShowConnectionMenu
    {
        public override decimal LoadPriority
        {
            get { return -100; }
        }

        public override bool Required
        {
            get { return true; }
        }

        public override void Initialize(QFrameworkContainer container)
        {
            var typeContainer = InvertGraphEditor.TypesContainer;
            // Drawers
            container.Register<DiagramViewModel, DiagramViewModel>();
            container.RegisterDrawer<PropertyFieldViewModel, PropertyFieldDrawer>();
            container.Register<SectionHeaderDrawer, SectionHeaderDrawer>();
            container.RegisterItemDrawer<GenericItemHeaderViewModel, GenericChildItemHeaderDrawer>();

            container.RegisterDrawer<InspectorViewModel, InspectorDrawer>();
            container.RegisterDrawer<SectionHeaderViewModel, SectionHeaderDrawer>();
            container.RegisterDrawer<ConnectorViewModel, ConnectorDrawer>();
            container.RegisterDrawer<ConnectionViewModel, ConnectionDrawer>();
            container.RegisterDrawer<InputOutputViewModel, SlotDrawer>();

            container.RegisterDrawer<DiagramViewModel, DiagramDrawer>();

            container.RegisterInstance<IConnectionStrategy>(new InputOutputStrategy(), "InputOutputStrategy");
            container.RegisterConnectable<GenericInheritableNode, GenericInheritableNode>();
            container.RegisterInstance<IConnectionStrategy>(new TypedItemConnectionStrategy(),
                "TypedConnectionStrategy");

            container.AddNode<EnumNode>("Enum")
                .AddCodeTemplate<EnumNode, EnumNodeGenerator>();
            container.AddItem<EnumChildItem>();

            typeContainer.RegisterInstance(
                new GraphTypeInfo() {Type = typeof(int), Group = "", Label = "int", IsPrimitive = true}, "int");
            typeContainer.RegisterInstance(
                new GraphTypeInfo()
                    {Type = typeof(string), Group = "", Label = "string", IsPrimitive = true}, "string");
            typeContainer.RegisterInstance(
                new GraphTypeInfo() {Type = typeof(decimal), Group = "", Label = "decimal", IsPrimitive = true},
                "decimal");
            typeContainer.RegisterInstance(
                new GraphTypeInfo() {Type = typeof(float), Group = "", Label = "float", IsPrimitive = true}, "float");
            typeContainer.RegisterInstance(
                new GraphTypeInfo() {Type = typeof(bool), Group = "", Label = "bool", IsPrimitive = true}, "bool");
            typeContainer.RegisterInstance(
                new GraphTypeInfo() {Type = typeof(char), Group = "", Label = "char", IsPrimitive = true}, "char");
            typeContainer.RegisterInstance(
                new GraphTypeInfo() {Type = typeof(DateTime), Group = "", Label = "date", IsPrimitive = true}, "date");
            typeContainer.RegisterInstance(
                new GraphTypeInfo() {Type = typeof(Vector2), Group = "", Label = "Vector2", IsPrimitive = true},
                "Vector2");
            typeContainer.RegisterInstance(
                new GraphTypeInfo() {Type = typeof(Vector3), Group = "", Label = "Vector3", IsPrimitive = true},
                "Vector3");

            container.Register<DesignerGeneratorFactory, RegisteredTemplateGeneratorsFactory>("TemplateGenerators");

#if UNITY_EDITOR
            typeContainer.RegisterInstance(
                new GraphTypeInfo() {Type = typeof(Quaternion), Group = "", Label = "Quaternion", IsPrimitive = true},
                "Quaternion");
            //container.Register<DesignerGeneratorFactory, Invert.uFrame.CodeGen.ClassNodeGenerators.SimpleClassNodeCodeFactory>("ClassNodeData");  

#endif
            // Register the container itself
            container.RegisterInstance<IQFrameworkContainer>(container);
            container.RegisterInstance<QFrameworkContainer>(container);

            container.AddNode<TypeReferenceNode, TypeReferenceNodeViewModel, TypeReferenceNodeDrawer>("Type Reference");
            container.AddNode<NoteNode, NoteNodeViewModel, NoteNodeDrawer>("Note");
        }

        public override void Loaded(QFrameworkContainer container)
        {
            InvertGraphEditor.DesignerPluginLoaded();
        }

        public void ConnectionApplying(IGraphData graph, IConnectable output, IConnectable input)
        {

        }

        public void ConnectionApplied(IGraphData g, IConnectable output, IConnectable input)
        {
            //#if UNITY_EDITOR
            //var projectService = InvertApplication.Container.Resolve<WorkspaceService>();
            //foreach (var graph in projectService.CurrentWorkspace.Graphs)
            //{
            //    if (graph.Identifier == g.Identifier)
            //    {
            //        UnityEditor.EditorUtility.SetDirty(graph as UnityEngine.Object);
            //    }
            //}
            //#endif
        }

        public void CreateConnectionMenu(ConnectionHandler viewModel, DiagramViewModel diagramViewModel,
            MouseEvent mouseEvent)
        {

        }


        public void QueryPossibleConnections(SelectionMenu menu, DiagramViewModel diagramViewModel,
            ConnectorViewModel startConnector,
            Vector2 mousePosition)
        {


            var currentGraph = InvertApplication.Container.Resolve<WorkspaceService>().CurrentWorkspace.CurrentGraph;
            var allowedFilterNodes = FilterExtensions.AllowedFilterNodes[currentGraph.CurrentFilter.GetType()];
            foreach (var item in allowedFilterNodes)
            {
                if (item.IsInterface) continue;
                if (item.IsAbstract) continue;

                var node = Activator.CreateInstance(item) as IDiagramNode;
                node.Repository = diagramViewModel.CurrentRepository;
                node.GraphId = currentGraph.Identifier;

                var vm = InvertGraphEditor.Container.GetNodeViewModel(node, diagramViewModel) as DiagramNodeViewModel;


                if (vm == null) continue;
                vm.IsCollapsed = false;
                var connectors = new List<ConnectorViewModel>();
                vm.GetConnectors(connectors);

                var config = InvertGraphEditor.Container.Resolve<NodeConfigBase>(item.Name);
                var name = config == null ? item.Name : config.Name;
                foreach (var connector in connectors)
                {
                    foreach (var strategy in InvertGraphEditor.ConnectionStrategies)
                    {
                        var connection = strategy.Connect(diagramViewModel, startConnector, connector);
                        if (connection == null) continue;
                        var node1 = node;
                        var message = string.Format("Create {0}", name);
                        //if (!string.IsNullOrEmpty(connector.Name))
                        //{
                        //    message += string.Format(" and connect to {0}", connector.Name);
                        //}
                        var value = new KeyValuePair<IDiagramNode, ConnectionViewModel>(node1, connection);

                        var qaItem = new SelectionMenuItem("Connect", message, () =>
                        {
                            diagramViewModel.AddNode(value.Key, mousePosition);
                            connection.Apply(value.Value as ConnectionViewModel);
                            value.Key.IsSelected = true;
                            value.Key.IsEditing = true;
                            value.Key.Name = "";
                        });
                        menu.AddItem(qaItem);
                    }
                }
            }
        }

        public void CommandExecuted(ICommand command)
        {
            //InvertGraphEditor.DesignerWindow.RefreshContent();
        }

        public void Show(DiagramViewModel diagramViewModel, ConnectorViewModel startConnector, Vector2 position)
        {
            var selectionMenu = new SelectionMenu();
            Signal<IQueryPossibleConnections>(_ =>
                _.QueryPossibleConnections(selectionMenu, diagramViewModel, startConnector, position));
            Signal((IShowSelectionMenu _) => _.ShowSelectionMenu(selectionMenu, position));
        }
    }
}