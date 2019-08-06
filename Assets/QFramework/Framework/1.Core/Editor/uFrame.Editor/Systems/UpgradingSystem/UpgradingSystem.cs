using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using QF.GraphDesigner.Unity;
using Invert.Data;
using QF.Json;
using QF;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class UpgradingSystem : DiagramPlugin
        , IExecuteCommand<Import16Command>
        , IExecuteCommand<FixTypes>
        , IToolbarQuery
        
    {
        public void Execute(Import16Command command)
        {
            Repository = InvertGraphEditor.Container.Resolve<IRepository>();
        }

        public void QueryToolbarCommands(ToolbarUI ui)
        {

        }
        public IRepository Repository { get; set; }

        private void ImportGraph(UnityGraphData unityGraphData)
        {
            Debug.Log(string.Format("Importing {0}", unityGraphData.name));
            Debug.Log(unityGraphData._jsonData);
            var json = JSON.Parse(unityGraphData._jsonData);
            ImportData(json as JSONClass);
        }

        public void ImportData(JSONClass node)
        {
            var typeName = string.Empty;
            if (node["_CLRType"] != null)
            {
                typeName = node["_CLRType"].Value;
            }
            else if (node["Type"] != null)
            {
                typeName = node["Type"].Value;
            }
            var type = InvertApplication.FindType(typeName) ?? Type.GetType(typeName);
            if (type == null && typeName.StartsWith("ConnectionData"))
            {
                type = typeof(ConnectionData);
            }

            if (type != null)
            {
                var result = ImportType(type, node);

                if (result is IGraphData)
                {
                    var item = InvertApplication.Container.Resolve<WorkspaceService>();
                    if (item.CurrentWorkspace != null)
                        item.CurrentWorkspace.AddGraph(result as IGraphData);
                    CurrentGraph = result as InvertGraph;
                    CurrentGraph.RootFilterId = node["RootNode"]["Identifier"].Value;
                    Debug.Log("Set Root filter id to " + CurrentGraph.RootFilterId);

                }
                if (result is GraphNode)
                {
                    CurrentNode = result as GraphNode;
                    CurrentNode.GraphId = CurrentGraph.Identifier;

                }
                if (result is DiagramNodeItem)
                {
                    ((IDiagramNodeItem)result).NodeId = CurrentNode.Identifier;
                }
                if (result is ITypedItem)
                {
                    // TODO Find type and replace it will fullname
                    ((ITypedItem)result).RelatedType = node["ItemType"].Value;
                }

                foreach (KeyValuePair<string, JSONNode> child in node)
                {
                    var array = child.Value as JSONArray;
                    if (array != null)
                    {

                        foreach (var item in array.Childs.OfType<JSONClass>())
                        {
                            ImportData(item);
                        }


                    }
                    var cls = child.Value as JSONClass;
                    if (cls != null)
                    {
                        if (child.Key == "FilterState") continue;
                        if (child.Key == "Settings") continue;
                        if (child.Key == "Changes") continue;
                        if (child.Key == "PositionData")
                        {
                            ImportPositionData(cls);
                        }
                        else
                        {
                            if (child.Key == "RootNode")
                            {
                                InvertApplication.Log("Importing ROOT NODE");
                            }
                            ImportData(cls);
                        }

                    }
                }
            }


        }

        private void ImportPositionData(JSONClass positionData)
        {
            foreach (KeyValuePair<string, JSONNode> item in positionData)
            {
                if (item.Key == "_CLRType") continue;
                var filterId = item.Key;
                foreach (KeyValuePair<string, JSONNode> positionItem in item.Value.AsObject)
                {
                    
                    var filterItem = new FilterItem();
                    filterItem.FilterId = filterId;
                    filterItem.NodeId = positionItem.Key;

                    var x = positionItem.Value["x"].AsInt;
                    var y = positionItem.Value["y"].AsInt;
                    InvertApplication.Log("Importing position ");
                    filterItem.Position = new Vector2(x, y);
                    filterItem.Collapsed = true;

                    Repository.Add(filterItem);
                }


            }
        }

        public GraphNode CurrentNode { get; set; }

        public InvertGraph CurrentGraph { get; set; }

        public IDataRecord ImportType(Type type, JSONClass cls)
        {
            var node = InvertJsonExtensions.DeserializeObject(type, cls) as IDataRecord;
            if (node != null)
                Repository.Add(node);
            return node;
        }

        public void Execute(FixTypes command)
        {
                // 用来升级的系统
//            foreach (var item in Container.Resolve<IRepository>().AllOf<SequenceItemNode>())
//            {
//                item.VariableName = item.VariableNameProvider.GetNewVariableName(item.GetType().Name);
//            }
        }
    }
}