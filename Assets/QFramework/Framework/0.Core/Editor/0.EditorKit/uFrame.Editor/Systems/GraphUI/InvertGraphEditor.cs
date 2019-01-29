using Invert.Common;
using Invert.uFrame;
using Invert.uFrame.Editor;
using Invert.uFrame.Editor.ViewModels;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using QFramework.GraphDesigner;
using Invert.Data;
using QFramework;
using UnityEngine;
namespace QFramework.GraphDesigner
{
    public static class InvertGraphEditor
    {
        public const string CURRENT_VERSION = "1.601";
        public const double CURRENT_VERSION_NUMBER = 1.601;
        public const bool REQUIRE_UPGRADE = true;


        private static IAssetManager _assetManager;



        private static IConnectionStrategy[] _connectionStrategies;





        private static IGraphEditorSettings _settings;

        private static QFrameworkContainer _TypesContainer;

        private static IWindowManager _windowManager;
        private static MouseEvent _currentMouseEvent;
        private static IGraphWindow _designerWindow;
        private static IStyleProvider _styleProvider;
        private static IQFrameworkContainer _uiContainer;

        public static IPlatformOperations Platform { get; set; }
        public static IPlatformPreferences Prefs { get; set; }

        public static IStyleProvider StyleProvider
        {
            get { return _styleProvider ?? (_styleProvider = Container.Resolve<IStyleProvider>()); }
            set { _styleProvider = value; }
        }



        public static IAssetManager AssetManager
        {
            get { return _assetManager ?? (_assetManager = Container.Resolve<IAssetManager>()); }
            set { _assetManager = value; }
        }

        public static IConnectionStrategy[] ConnectionStrategies
        {
            get { return _connectionStrategies ?? (_connectionStrategies = Container.ResolveAll<IConnectionStrategy>().ToArray()); }
            set { _connectionStrategies = value; }
        }

        public static IQFrameworkContainer Container
        {
            get { return InvertApplication.Container; }
        }
        public static IQFrameworkContainer UIContainer
        {
            get { return _uiContainer ?? (_uiContainer = new QFrameworkContainer()); }
        }


        public static DiagramViewModel CurrentDiagramViewModel
        {
            get
            {
                if (DesignerWindow == null) return null;
                return DesignerWindow.DiagramViewModel;
            }
        }

        //public static MouseEvent CurrentMouseEvent
        //{
        //    // TODO
        //    get { return _currentMouseEvent; }
        //    set { _currentMouseEvent = value; }
        //}

        //public static IProjectRepository CurrentProject
        //{
        //    get { return _currentProject; }
        //    set
        //    {
        //        _currentProject = value;
        //        if (value != null)
        //        {
        //            foreach (var diagram in _currentProject.Graphs)
        //            {
        //                diagram.SetProject(value);
        //            }
        //        }
        //    }
        //}

        public static IGraphWindow DesignerWindow
        {
            get { return _designerWindow; }
            set
            {
                _designerWindow = value;
                //Container.Inject(_designerWindow);
            }
        }

        public static IKeyBinding[] KeyBindings { get; set; }


        public static IGraphEditorSettings Settings
        {
            get { return _settings ?? (_settings = Container.Resolve<IGraphEditorSettings>()); }
            set { _settings = value; }
        }

        public static QFrameworkContainer TypesContainer
        {
            get
            {
                if (_TypesContainer != null) return _TypesContainer;
                _TypesContainer = new QFrameworkContainer();
                InitializeTypesContainer(_TypesContainer);
                return _TypesContainer;
            }
            set { _TypesContainer = value; }
        }

        public static IWindowManager WindowManager
        {
            get { return _windowManager ?? (_windowManager = Container.Resolve<IWindowManager>()); }
        }

        public static IPlatformDrawer PlatformDrawer { get; set; }

        public static IQFrameworkContainer Connectable<TSource, TTarget>(this IQFrameworkContainer container, bool oneToMany = true)
            where TSource : class, IConnectable
            where TTarget : class, IConnectable
        {
            return Connectable<TSource, TTarget>(container, Color.white, oneToMany);
        }

        public static IQFrameworkContainer Connectable<TSource, TTarget>(this IQFrameworkContainer container, Color color, bool oneToMany = true)
            where TSource : class, IConnectable
            where TTarget : class, IConnectable
        {
            container.RegisterConnectable<TSource, TTarget>();  //if (oneToMany)
            //container.RegisterInstance<IConnectionStrategy>(new CustomInputOutputStrategy<TSource, TTarget>(color), typeof(TSource).Name + "_" + typeof(TTarget).Name + "Connection");
            //else
            //{
            //    container.RegisterInstance<IConnectionStrategy>(new OneToOneConnectionStrategy<TSource, TTarget>(), typeof(TSource).Name + "_" + typeof(TTarget).Name + "Connection");
            //}
            return container;
        }
        //public static IQFrameworkContainer AddNodeFlag<TSource>(this IQFrameworkContainer container, string flag) where TSource : class, IDiagramNodeItem
        //{
        //    container.RegisterInstance<IDiagramNodeCommand>(new GraphItemFlagCommand<TSource>(flag, flag), typeof(TSource).Name + flag + "FlagCommand");
        //    return container;
        //}
        //public static IQFrameworkContainer AddItemFlag<TSource>(this IQFrameworkContainer container, string flag, Color color) where TSource : class, IDiagramNodeItem
        //{
        //    var command = new GraphItemFlagCommand<TSource>(flag, flag) { Color = color };

        //    container.RegisterInstance<IDiagramNodeItemCommand>(command, typeof(TSource).Name + flag + "FlagCommand");
        //    container.RegisterInstance<IFlagCommand>(command, flag);
        //    return container;
        //}
        //public static IEnumerable<IEditorCommand> CreateCommandsFor<T>()
        //{
        //    var commands = Container.ResolveAll<T>();

        //    return Enumerable.Where(Commands, p => typeof(T).IsAssignableFrom(p.For));
        //}
        //public static TCommandUI CreateCommandUIWithCommands<TCommandUI>(params IEditorCommand[] actions) where TCommandUI : class,ICommandUI
        //{
        //    var ui = Container.Resolve<TCommandUI>() as ICommandUI;
        //    ui.Handler = DesignerWindow;
        //    foreach (var action in actions)
        //    {
        //        if (action.CanExecute(DesignerWindow) == null)
        //            ui.AddCommand(action);
        //    }
        //    return (TCommandUI)ui;
        //}
        //public static ToolbarUI CreateToolbarUI()
        //{
        //    var ui = Container.Resolve<ToolbarUI>();

        //    ui.Handler = DesignerWindow;

        //    var commands = Container.ResolveAll<IToolbarCommand>();
        //    InvertApplication.SignalEvent<IToolbarQuery>(_=>_.QueryToolbarCommands(ui));
        //    foreach (var command in commands)
        //    {
        //        ui.AddCommand(command);
        //    }
        //    return ui;
        //}


        public static void DesignerPluginLoaded()
        {

            Settings = Container.Resolve<IGraphEditorSettings>();
            AssetManager = Container.Resolve<IAssetManager>();
            OrganizeFilters();
            //var commandKeyBindings = new List<IKeyBinding>();
            //foreach (var item in Container.Instances)
            //{
            //    if (typeof(IEditorCommand).IsAssignableFrom(item.Key))
            //    {
            //        if (item.Instance != null)
            //        {
            //            var command = item.Instance as IEditorCommand;
            //            if (command != null)
            //            {
            //                var keyBinding = command.GetKeyBinding();
            //                if (keyBinding != null)
            //                    commandKeyBindings.Add(keyBinding);
            //            }
            //        }
            //    }
            //}

            ConnectionStrategies = Container.ResolveAll<IConnectionStrategy>().ToArray();

            KeyBindings = Container.ResolveAll<IKeyBinding>().ToArray();
        }

        //public static void ExecuteCommand(IEditorCommand action)
        //{
        //    ExecuteCommand(DesignerWindow, action);
        //}

        //public static void ExecuteCommand(Action<DiagramViewModel> action, bool recordUndo = false)
        //{
        //    ExecuteCommand(DesignerWindow, new SimpleEditorCommand<DiagramViewModel>(action), recordUndo);
        //}


        //private static void ExecuteCommand(this ICommandHandler handler, IEditorCommand command, bool recordUndo = true)
        //{
        //    var objs = handler.ContextObjects.ToArray();
        //    if (recordUndo && DesignerWindow != null && DesignerWindow.DiagramViewModel != null)
        //    {
        //        // TODO 2.0 Record Undo
        //        //DesignerWindow.DiagramViewModel.CurrentRepository.RecordUndo(DesignerWindow.DiagramViewModel.GraphData, command.Name);
        //    }

        //    command.Execute(handler);


        //    if (recordUndo && DesignerWindow != null && DesignerWindow.DiagramViewModel != null)
        //    {
        //        DesignerWindow.DiagramViewModel.CurrentRepository.MarkDirty(DesignerWindow.DiagramViewModel.GraphData);
        //    }
        //    Container.Resolve<IRepository>().Commit();
        //    //CurrentProject.MarkDirty(CurrentProject.CurrentGraph);
        //}

        public static IEnumerable<OutputGenerator> GetAllCodeGenerators(IGraphConfiguration graphConfiguration, IDataRecord[] items, bool includeDisabled = false)
        {

            // Grab all the code generators
            var graphItemGenerators = Container.ResolveAll<DesignerGeneratorFactory>().ToArray();


            foreach (var outputGenerator in GetAllCodeGeneratorsForItems(graphConfiguration, graphItemGenerators, items,includeDisabled))
                yield return outputGenerator;
        }

        private static IEnumerable<OutputGenerator> GetAllCodeGeneratorsForItems(IGraphConfiguration graphConfiguration,
            DesignerGeneratorFactory[] graphItemGenerators, IDataRecord[] items, bool includeDisabled = false)
        {
            foreach (var graphItemGenerator in graphItemGenerators)
            {
                DesignerGeneratorFactory generator = graphItemGenerator;
                // If its a generator for the entire diagram
                foreach (var item in items)
                {
                    if (generator.DiagramItemType.IsAssignableFrom(item.GetType()))
                    {
                        var codeGenerators = generator.GetGenerators(graphConfiguration, item);
                        foreach (var codeGenerator in codeGenerators)
                        {
                            if ( !includeDisabled && !codeGenerator.IsValid()) continue;
                            // TODO Had to remove this?
                            //if (!codeGenerator.IsEnabled(prsteroject)) continue;

                            codeGenerator.AssetDirectory = graphConfiguration.CodeOutputPath;
                            //codeGenerator.Settings = settings;
                            if (codeGenerator.ObjectData == null)
                                codeGenerator.ObjectData = item;
                            codeGenerator.GeneratorFor = graphItemGenerator.DiagramItemType;
                            yield return codeGenerator;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get all of the output generators for a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<OutputGenerator> GetCodeGeneratorsForNode(this IDataRecord node, IGraphConfiguration config)
        {

            return GetAllCodeGenerators(config, new[] { node }).Where(p => p.ObjectData == node);
        }

        /// <summary>
        /// Get all of the output generators that are generated only the first time, AKA: Editable Files
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<OutputGenerator> GetAllEditableFilesForNode(this IDataRecord node, IGraphConfiguration config)
        {
            return GetAllCodeGenerators(config, new[] { node }).Where(p => p.ObjectData == node && !p.AlwaysRegenerate);
        }

        /// <summary>
        /// This method gets every method, property, or constructor generated for a node, pass in an item filter to filter only to specific items
        /// that are set up as iterators on templates.
        /// 
        /// For Example:
        /// You can get only the members of a class that have been added since last save and compile by comparing with with the change tracking data.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="itemFilter"></param>
        /// <returns></returns>
        public static IEnumerable<TemplateMemberResult> GetEditableOutputMembers(this IDiagramNodeItem node, IGraphConfiguration config, Predicate<IDiagramNodeItem> itemFilter = null)
        {

            foreach (var item in GetAllEditableFilesForNode(node, config).OfType<ITemplateClassGenerator>())
            {
                var gen = new CodeFileGenerator()
                {
                    Generators = new[] { item as OutputGenerator },
                    SystemPath = string.Empty
                };
                gen.Namespace = new CodeNamespace();
                gen.Unit = new CodeCompileUnit();
                gen.Unit.Namespaces.Add(gen.Namespace);
                item.ItemFilter = itemFilter;
                item.Initialize(gen);
                foreach (var result in item.Results)
                {
                    yield return result;
                }
            }
        }
        public static IEnumerable<IClassTemplate> GetTemplates(this IDiagramNodeItem node, IGraphConfiguration config, Predicate<IDiagramNodeItem> itemFilter = null)
        {

            foreach (var item in node.GetCodeGeneratorsForNode(config).OfType<ITemplateClassGenerator>())
            {
                yield return item.Template;
            }
        }
        /// <summary>
        /// Grab all of the output generators that are always regenerated on a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<OutputGenerator> GetAllDesignerFilesForNode(this IDiagramNodeItem node, IGraphConfiguration config)
        {
            return GetAllCodeGenerators(config, new IDataRecord[] { node }).Where(p => p.ObjectData == node && p.AlwaysRegenerate);
        }

        /// <summary>
        /// Get all of the code generators for a repository IE: (Graph, or Project)
        /// </summary>
        /// <param name="settings">Obsolete pass null</param>
        /// <param name="project">Project or graph</param>
        /// <param name="generator"></param>
        /// <param name="diagramItemGenerator"></param>
        /// <param name="includeDisabled"></param>
        /// <returns></returns>
        private static IEnumerable<OutputGenerator> GetCodeGeneratorsForNodes(GeneratorSettings settings, IRepository project,
            DesignerGeneratorFactory generator, DesignerGeneratorFactory diagramItemGenerator, bool includeDisabled = false)
        {
            yield break;
            // TODO 2.0 IMPORANT: Figure out code generators

            //var diagrams = project.AllOf<IGraphData>();


            //foreach (var diagram in diagrams)
            //{
            //    if (diagram.Precompiled) continue;
            //    if (diagram.Settings.CodeGenDisabled && !includeDisabled) continue;
            //    var items = diagram.AllGraphItems.OfType<IDiagramNodeItem>().Where(p => p.GetType() == generator.DiagramItemType);

            //    foreach (var item in items)
            //    {
            //        if (item.Precompiled) continue;
            //        var codeGenerators = generator.GetGenerators(settings, null, project, item);
            //        foreach (var codeGenerator in codeGenerators)
            //        {
            //            // TODO had to remove this?
            //            //if (!codeGenerator.IsEnabled(project)) continue;
            //            codeGenerator.AssetDirectory = diagram.Project.SystemDirectory;
            //            codeGenerator.Settings = settings;
            //            if (codeGenerator.ObjectData == null)
            //                codeGenerator.ObjectData = item;
            //            codeGenerator.GeneratorFor = diagramItemGenerator.DiagramItemType;
            //            yield return codeGenerator;
            //        }
            //    }
            //}
        }

        //public static IEnumerable<CodeFileGenerator> GetAllFileGenerators(GeneratorSettings settings)
        //{
        //    return GetAllFileGenerators(settings, CurrentProject);
        //}
        /// <summary>
        /// Get all of the merged generators for a project, this will merge any output generators with the same filename into a combined "CodeFileGenerator".
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CodeFileGenerator> GetAllFileGenerators(IGraphConfiguration config, IDataRecord[] items, bool includeDisabled = false)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (items == null) throw new ArgumentNullException("items");

            var codeGenerators = GetAllCodeGenerators(config, items, includeDisabled).ToArray();
            var groups = codeGenerators.GroupBy(p => Path.Combine(config.CodeOutputPath, p.Filename)).Distinct();
            foreach (var @group in groups)
            {
                var generator = new CodeFileGenerator(config.Namespace)
                {
                    AssetPath = @group.Key.Replace("\\", "/"),
#if UNITY_EDITOR
                    //SystemPath = Path.Combine(config.CodeOutputSystemPath, @group.Key.Substring(7)).Replace("\\", "/"),
                    SystemPath = @group.Key,
#endif
                    Generators = @group.ToArray()
                };
                yield return generator;
            }
        }


        public static GraphItemViewModel GetNodeViewModel(this IQFrameworkContainer container, IGraphItem item, DiagramViewModel diagram)
        {
            var vm = InvertApplication.Container.ResolveRelation<ViewModel>(item.GetType(), item, diagram) as
                           GraphItemViewModel;
            return vm;
        }
        public static IEnumerable<Type> GetAllowedFilterItems(Type filterType)
        {
            return Container.RelationshipMappings.Where(
                p => p.Key.Item1 == filterType && p.Key.Item2 == typeof(IDiagramNodeItem)).Select(p => p.Value);
        }

        public static IEnumerable<Type> GetAllowedFilterNodes(Type filterType)
        {
            return Container.RelationshipMappings.Where(
                p => p.Key.Item1 == filterType && p.Key.Item2 == typeof(IDiagramNode)).Select(p => p.Value);
        }



        public static bool IsFilter(Type type)
        {
            return FilterExtensions.AllowedFilterNodes.ContainsKey(type);
        }

        public static void OrganizeFilters()
        {
            var filterTypes = Container.RelationshipMappings.Where(
               p => typeof(IGraphFilter).IsAssignableFrom(p.Key.Item1) && p.Key.Item2 == typeof(IDiagramNode));
            var filterTypeItems = Container.RelationshipMappings.Where(
                p => typeof(IGraphFilter).IsAssignableFrom(p.Key.Item1) && p.Key.Item2 == typeof(IDiagramNodeItem));

            foreach (var filterMapping in filterTypes)
            {
                if (!FilterExtensions.AllowedFilterNodes.ContainsKey(filterMapping.Key.Item1))
                {
                    FilterExtensions.AllowedFilterNodes.Add(filterMapping.Key.Item1, new List<Type>());
                }
                FilterExtensions.AllowedFilterNodes[filterMapping.Key.Item1].Add(filterMapping.Value);
            }

            foreach (var filterMapping in filterTypeItems)
            {
                if (!FilterExtensions.AllowedFilterItems.ContainsKey(filterMapping.Key.Item1))
                {
                    FilterExtensions.AllowedFilterItems.Add(filterMapping.Key.Item1, new List<Type>());
                }
                FilterExtensions.AllowedFilterItems[filterMapping.Key.Item1].Add(filterMapping.Value);
            }
        }

        public static IQFrameworkContainer RegisterChildGraphItem<TModel, TViewModel>(this IQFrameworkContainer container)
        {
            container.RegisterRelation<TModel, ItemViewModel, TViewModel>();
            return container;
        }

        public static IQFrameworkContainer RegisterCodeTemplate<TFor, TTemplateType>(this IQFrameworkContainer container)
        {
            container.RegisterRelation<TFor, CodeGenerator, TTemplateType>();
            return container;
        }

        public static void RegisterFilterItem<TFilterData, TAllowedItem>()
        {
            Container.RegisterRelation<TFilterData, IDiagramNodeItem, TAllowedItem>();
        }

        public static void RegisterFilterItem<TFilterData, TAllowedItem>(this IQFrameworkContainer container)
        {
            container.RegisterRelation<TFilterData, IDiagramNodeItem, TAllowedItem>();
        }
        public static IQFrameworkContainer RegisterDataViewModel<TModel, TViewModel>(this IQFrameworkContainer container)
        {
            container.RegisterRelation<TModel, ViewModel, TViewModel>();
            return container;
        }
        public static IQFrameworkContainer RegisterDataChildViewModel<TModel, TViewModel>(this IQFrameworkContainer container)
        {
            container.RegisterRelation<TModel, ItemViewModel, TViewModel>();
            return container;
        }

        public static IQFrameworkContainer RegisterConnectionStrategy<TConnectionStrategy>(this IQFrameworkContainer container)
            where TConnectionStrategy : IConnectionStrategy, new()
        {
            container.RegisterInstance<IConnectionStrategy>(new TConnectionStrategy(), typeof(TConnectionStrategy).Name);
            return container;
        }




        //public static IQFrameworkContainer ConnectionStrategy<TSource, TTarget>(this IQFrameworkContainer container, Color connectionColor,
        //    Func<TSource, TTarget, bool> isConnected, Action<TSource, TTarget> apply, Action<TSource, TTarget> remove) where TSource : class, IConnectable where TTarget : class, IConnectable
        //{
        //    container.RegisterInstance<IConnectionStrategy>(new CustomConnectionStrategy<TSource, TTarget>(connectionColor,isConnected,apply,remove), typeof(TSource).Name + "_" + typeof(TTarget).Name + "CustomConnection");
        //    return container;
        //}
   

        public static IQFrameworkContainer RegisterGraphItem<TModel, TViewModel>(this IQFrameworkContainer container)
        {
            container.RegisterRelation<TModel, ViewModel, TViewModel>();
            return container;
        }

        public static IDrawer CreateDrawer(this IQFrameworkContainer container, ViewModel viewModel)
        {
            return CreateDrawer<IDrawer>(container, viewModel);
        }

        public static Dictionary<Type, Type> _drawers;
        public static IDrawer CreateDrawer<TDrawerBase>(this IQFrameworkContainer container, ViewModel viewModel) where TDrawerBase : IDrawer
        {
            if (_drawers != null)
            {

            }
            if (viewModel == null)
            {
                InvertApplication.LogError("Data is null.");
                return null;
            }
            var drawer = container.ResolveRelation<TDrawerBase>(viewModel.GetType(), viewModel);
            if (drawer == null)
            {
                InvertApplication.Log(String.Format("Couldn't Create drawer for {0}.", viewModel.GetType()));
            }
            return drawer;
        }


        public static GraphItemViewModel CreateViewModel(this IQFrameworkContainer container, object data)
        {
            return container.ResolveRelation<ViewModel>(data.GetType(), data, null) as GraphItemViewModel;
        }
        private static void InitializeTypesContainer(QFrameworkContainer container)
        {

        }

    }
}