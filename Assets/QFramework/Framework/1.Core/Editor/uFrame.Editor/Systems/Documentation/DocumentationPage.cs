using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{

    public abstract class DocumentationPage
    {
        public virtual decimal Order
        {
            get { return 0; }
        }

        public virtual bool ShowInNavigation
        {
            get { return true; }
        }
        public virtual IEnumerable<ScaffoldGraph> Scaffolds()
        {
            yield break;
        }
        private static WorkspaceService _workspaceService;
        public static WorkspaceService WorkspaceService
        {
            get { return _workspaceService ?? (_workspaceService = InvertApplication.Container.Resolve<WorkspaceService>()); }
            set { _workspaceService = value; }
        }

        private List<DocumentationPage> _childPages;
        private string _name;

        public virtual string Name
        {
            get
            {
                return AddSpacesToSentence(this.GetType().Name, true);
            }
     
        }
        string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
        public List<DocumentationPage> ChildPages
        {
            get { return _childPages ?? (_childPages = new List<DocumentationPage>()); }
            set { _childPages = value; }
        }

        public Action<IDocumentationBuilder> PageContent { get; set; }
        public virtual Type RelatedNodeType { get; set; }

        public virtual Type ParentPage
        {
            get { return null; }
        }


        public virtual void GetContent(IDocumentationBuilder _)
        {
            _.Title(Name);
        }

        public T DoNamedNodeStep<T>(IDocumentationBuilder builder, string requiredName, IGraphFilter requiredFilter = null,
            Action<IDocumentationBuilder> stepContent = null) where T : GenericNode
        {
            
            T existing = null;
            if (WorkspaceService.CurrentWorkspace == null || WorkspaceService.CurrentWorkspace.CurrentGraph == null)
            {

            }
            else
            {
                existing = WorkspaceService.Repository.All<T>().FirstOrDefault(p => p.Name == requiredName);
            }
            
            builder.ShowTutorialStep(new TutorialStep(string.Format("Create a '{0}' node with the name '{1}'", InvertApplication.Container.GetNodeConfig<T>().Name, requiredName), () =>
            {
                if (existing == null)
                {
                    if (requiredFilter != null)
                    {
                        if (WorkspaceService.CurrentWorkspace.CurrentGraph.CurrentFilter != requiredFilter)
                        {
                            return string.Format("Double-click on the '{0}' Node.", requiredFilter.Name);
                        }
                    }

                    return "Node not created yet";
                }
                return null;
            })
            {
                StepContent = _ =>
                {
                    var nodeTypeName = typeof(T).Name.Replace("Node", "");

                    _.Paragraph("In this step you need to create {0}.", nodeTypeName);
                    _.Paragraph("To create any kind of node you need to right click on an empty space on the graph. Context menu will appear. It will contain different 'Add' commands. Each 'Add' command" +
                                "allows you to add a new node to the graph. The types of nodes you can create may be different based on the context. For example, you can create elements only inside of subsystems, and " +
                                "you can only create views inside of elements.");

                    if (requiredFilter != null)
                    {
                        if (WorkspaceService.CurrentWorkspace.CurrentGraph.CurrentFilter != requiredFilter)
                        {
                            _.Paragraph("First of all you need to get into {0} node.", requiredFilter.Name);
                            
                            _.Paragraph(
                                "At any time you can tell where you are, by refering to filter bar, which is located under opened tabs bar. It will show your current position in the hierarchy.");
                            
                            _.ImageByUrl("http://i.imgur.com/bPMlmOq.png");
                            
                            _.Paragraph(
                                "There are several ways you can navigate in the graph. First of all, you may switch graphs them selves. Each graph is represented as a graph node and is a filter on it's own. " +
                                "You can double-click the headers of certain nodes. If it is possible, such node will become a current filter. This will show the graph filtered by this node. " +
                                "If you double-click the header of the current filter, it will bring bring you back.");
                            
                            _.Paragraph("If {0} node is a graph node, just open the corresponding graph and select root in the filter bar.",
                                requiredFilter.Name);
                            
                            _.Paragraph("If {0} node is a regular node, locate it and double-click it's header.",
                                requiredFilter.Name);
                        }
                        else
                        {
                            _.Paragraph("It seems like you are inside of {0}, which is correct. Now you should be able to right-click on empty space of the graph and select Add {1}.", requiredFilter.Name, nodeTypeName);
                        
                            _.Paragraph("Finally you need to rename newly created node. To rename your node you have to right-click on the header of the node and click rename." +
                               "Node title will become editable. Type in \"{0}\". Click anywhere else, to finish editing",requiredName);

                            _.ImageByUrl("http://i.imgur.com/PDUZhsU.png", "This picture shows how to rename a node");
                        }



                    }

                    if (stepContent != null)
                    {
                        stepContent(_);
                    }
                    _.Break();
                    _.ToggleContentByNode<T>(null);
                }
            });
            return existing;
        }

        public T DoNamedItemStep<T>(IDocumentationBuilder builder,
            string requiredName,
            IDiagramNode requiredNode,
            string singularItemTypeName,
            Action<IDocumentationBuilder> stepContent,
            string requiredNodeName = "UNSPECIFIED",
            string requiredSection = "UNSPECIFIED"
            ) where T : class, IDiagramNodeItem
        {
            T existing = requiredNode == null ?  (T) null : requiredNode.PersistedItems.OfType<T>().FirstOrDefault(p => p.Name == requiredName);
            var message = string.Format("Create {0} with the name '{1}' on '{2}' node", singularItemTypeName,
                requiredName, requiredNodeName);
            if (requiredNode != null)
            {
                message += string.Format(" on the '{0}' node", requiredNode.Name);
            }
            builder.ShowTutorialStep(new TutorialStep(message, () =>
            {
                if (existing == null)
                {
                    return "Item not created yet";
                }
                return null;
            })
            {
                StepContent = _ =>
                {

                    var nodeName = requiredNode == null ? requiredNodeName : requiredNode.Name;
                    var section = requiredSection;
                    _.Paragraph("In this step you will need to create and item called \"{2}\" in the \"{1}\" section of {0} node. Just like any other item, it will belong to a node. That is why you first need to locate the corresponding node: {0}. " +
                                "The next step is to locate the corresponding section. Most of the time section has a clear name. In this case, section is called \"{1}\"\n" +
                                "Almost every section will have a plus button to the right. By clicking this button, you introduce a new entry in the section. Most of time, clicking this button will instantly" +
                                " add a new entry in the section. However, some of them will show selection window, and you will need to select an item from the list.", nodeName, section,requiredName);

                    _.Paragraph("The final step is specifying name and optional type for the item. This is not always necessary." +
                                "To rename an item, you have to double click it's name. If it is not a reference item, the name will become editable. To finish editing, click anywhere else on the graph.\n"+
                                "Some items may allow you to specify type. To select type, click on the current type of the item. Type selection window will appear. Select desired type from the list.");

                    _.ImageByUrl("http://i.imgur.com/EzVOszn.png", "This image explain the section structure of the node.");
                    
                    _.Note("Not every item has a type. ");
                    _.Note("Some items represent a reference to another item. That is why you cannot modify it's name directly.");
                    
                    _.Break();
                    
                    _.ToggleContentByPage<T>(singularItemTypeName);
                    
                    if (stepContent != null)
                    {
                        stepContent(_);
                    }
                }
            });
            return existing;
        }


        
        public TGraphType DoGraphStep<TGraphType>(IDocumentationBuilder builder, string name = null, Action<IDocumentationBuilder> stepContent = null) where TGraphType : class,IGraphData
        {
            var currentGraph =
                (WorkspaceService.CurrentWorkspace == null)
                || (WorkspaceService.CurrentWorkspace.CurrentGraph == null)
                    ? null : (WorkspaceService.CurrentWorkspace.Graphs.OfType<UnityGraphData>().Select(p => p.Graph).OfType<TGraphType>().FirstOrDefault()) as TGraphType;

            builder.ShowTutorialStep(new TutorialStep(string.Format("Create a new {0} Graph with the name '{1}'", typeof(TGraphType).Name.Replace("Graph",""),name ?? "ANYTHING"), () =>
            {
                if (currentGraph == null)
                {
                    return "Graph hasn't been created yet.";
                }
                else if (!string.IsNullOrEmpty(name) && currentGraph.RootFilter.Name != name)
                {
                    return string.Format("Rename it to '{0}'", name);
                }
                return null;
            })
            {
                StepContent = b =>
                {
                    b.Paragraph("In this step you will learn a little bit about graphs." +
                                "Each graph lives physically as a file inside your project folder. Graphs are combined into a uFrame project, but they can live on their own and can be linked to different projects at once!");
 
                    b.Paragraph("Each graph can contain nodes of certain types. " +
                                "Graphs can also share nodes between each other as long as they belong to the same project." +
                                "Graphs are used to manage various sections of your project. ");

                    b.Paragraph(
                        "You can create graphs using uFrame Graph Designer Window. In the top left corner you will find graph selection box. Besides displaying all the graphs attached to the current project, it will show options to create new graphs of different types");

                    b.ImageByUrl("http://i.imgur.com/MqXxE6h.png","This picture shows how to create new graph");
                    
                    b.Note("When you create a graph, pay attention to what project is currently selected! Graph will be created and linked to the selected project automatically. Also, graph file will be created inside of the selected project folder.");

                    b.Paragraph("In this step we need to create a graph of type {0}",typeof(TGraphType).Name);

                    b.Paragraph("After you create a graph, it will contain only one node. This node represents graph itself and is often called Graph node.");

                    b.Paragraph("To rename your graph, you have to rename the corresponding graph node. For this, you have to right-click on the header of the graph node and click rename." +
                                "Node title will become editable. Type in the desired title. Click anywhere else, to finish editing");
         
                    b.ImageByUrl("http://i.imgur.com/PDUZhsU.png", "This picture shows how to rename a graph");

                    b.Note("When user renames graph node, uFrame automatically updates graph file too!");

                    if (stepContent != null)
                    {
                        stepContent(b);
                    }
                }
            });
            return currentGraph;
        }

        public Workspace DoCreateNewProjectStep(IDocumentationBuilder builder,string projectName, Action<IDocumentationBuilder> stepContent = null)
        {
            var currentProject = WorkspaceService.CurrentWorkspace;
            builder.ShowTutorialStep(new TutorialStep("Create a project and open it in Graph Designer", () =>
            {
            
                if (currentProject == null)
                {
                    return "Project hasn't been created yet.";
                }
                if (currentProject.Name != projectName)
                {
                    return string.Format("Make sure that \"{0}\" project is created\n" +
                                         "Make sure that Graph Designer window is opened and selected project is \"{0}\"", projectName);
                }
                return null;
            })
            {
                StepContent = _ =>
                {
                   
                    if (stepContent != null)
                    {
                        stepContent(_);
                    }
                    
                    _.Paragraph("You need to create a new project to begin this journey.");
                    _.Paragraph("Navigate to the project window and create a folder. Pick up the name to correspond your project: ");

                    _.ImageByUrl("http://i.imgur.com/gV6dWD0.png","This picture shows how to create new folder for uFrame project");

                    _.Paragraph("From here right click on the newly created folder and choose uFrame->New Project");
                    _.Paragraph("This will create a new project repository for you. You will find it inside of the folder." +
                                "The project repository will hold references to all of your graphs.");

                    _.ImageByUrl("http://i.imgur.com/yv6J4Ag.png","This picture shows how to create new uFrame project.");

                    _.Paragraph("Change your project name by renaming the project repository file. You can pick up the same name, you have chosen for the folder:");


                    _.ImageByUrl("http://i.imgur.com/YTf2p2G.png", "This picture shows how to change uFrame project name.");

                    _.Break();
                    
                    _.Paragraph("If you haven't already, open the graph designer by clicking on Window->uFrame Designer in the Unity menu.");
                    _.ImageByUrl("http://i.imgur.com/80Pou12.png", "This picture shows how to open uFrame Graph Designer");

                    _.Paragraph("Finally you need to select your project in graph designer.");
                    _.ImageByUrl("http://i.imgur.com/K7rf60I.png", "This picture shows how to select uFrame project in uFrame Graph Designer");
                 
                },
            });
            return currentProject;
        }
        public TutorialStep SaveAndCompile(GraphNode node)
        {
            return new TutorialStep("Save & Compile the project.", () =>
            {
                if (InvertApplication.FindType(node.FullName) == null)
                {
                    return string.Format("Expected generated types are not found. Make sure that:\n\n" +
                                         "* You clicked 'Save and Compile' button\n" +
                                         "* Generation is finished\n" +
                                         "* Unity console does not contain compilation errors\n");
                }
                return null;
            });
        }
        public string EnsureCodeInEditableFile(GraphNode elementNode, string filenameSearchText, string codeSearchText)
        {
            var firstOrDefault = elementNode.GetCodeGeneratorsForNode(InvertApplication.Container.Resolve<IGraphConfiguration>())
                .FirstOrDefault(p => !p.AlwaysRegenerate && (p.Filename.Contains(filenameSearchText) || p.Filename == filenameSearchText));
            if (firstOrDefault == null || !File.Exists(firstOrDefault.FullPathName) || !File.ReadAllText(firstOrDefault.FullPathName).Contains(codeSearchText))
            {
                return "File not found, or you haven't implemented it properly.";
            }
            return null;
        }
        public ConnectionData DoCreateConnectionStep(IDocumentationBuilder builder, IDiagramNodeItem output, IDiagramNodeItem input, string outputName = "" , string inputName = "", string outputItemName = "", string inputItemName = "", Action<IDocumentationBuilder> stepContent = null)
        {
     
//            var inputName = input == null ? "A" :input.Name;
//            var outputName = output == null ? "B" : output.Name;
//
//            if (input != null && input != input.Node)
//            {
//                inputName = input.Node.Name + "'s " + input.Name + " input slot";
//            }
//            if (output != null && output != output.Node)
//            {
//                outputName = output.Node.Name + "'s " + output.Name + " output slot";
//            }
            builder.ShowTutorialStep( new TutorialStep(string.Format("Create a connection from {0} to {1}.", outputName, inputName), () =>
            {
                var existing = output == null || input == null ? null :
                    WorkspaceService.Repository.All<ConnectionData>().FirstOrDefault(p => p.OutputIdentifier == output.Identifier && p.InputIdentifier == input.Identifier);
                if (existing == null)
                {
                    var typedItem = output as ITypedItem;
                    if (typedItem != null && input != null)
                    {
                        if (typedItem.RelatedType == input.Identifier)
                        {
                            return null;
                        }
                    }
                    return "The connection hasen't been created yet.";
                }
                return null;
            })
            {
                StepContent = _ =>
                {
                        _.Paragraph("In this step you are going to create a connection from {0} to {1}.\n" +
                                    "First of all you need to locate {2}. If it is a node, expand it. When you place your mouse pointer over the {2}, input and output connectors will be shown." +
                                    "Then you need to locate {3}. If it is a node, expand it. When you place your mouse pointer over the {3}, input and output connectors will be shown." +
                                    "Both nodes and items MAY have inputs and outputs", outputName, inputName, outputItemName, inputItemName);

                        _.ImageByUrl("http://i.imgur.com/YTSnk19.png",
                            "This picture shows input and output connectors of a node");
                        _.ImageByUrl("http://i.imgur.com/1y5mvRb.png",
                            "This picture shows input and output connectors of a node item");

                        _.Paragraph(
                            "Left-click on the {0}, hold left mouse button and drag it to {1}. Then release left mouse button. This should create a connection between {0} and {1}",
                            outputName, inputName);

                        if (stepContent != null) stepContent(_);
                }
            });
            return null;
        }

    }
}