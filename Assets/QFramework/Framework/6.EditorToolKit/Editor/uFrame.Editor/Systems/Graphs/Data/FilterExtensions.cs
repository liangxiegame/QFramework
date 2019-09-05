using System;
using System.Collections.Generic;
using System.Linq;
using QF;

namespace QF.GraphDesigner
{
    public static class FilterExtensions
    {
        public static void RegisterConnectable<TOutput, TInput>(this IQFrameworkContainer container)
        {
            container.RegisterInstance<RegisteredConnection>(new RegisteredConnection() { TInputType = typeof(TInput), TOutputType = typeof(TOutput) }, typeof(TOutput).Name + typeof(TInput).Name);

        }
        public static void RegisterConnectable(this IQFrameworkContainer container, Type outputType, Type inputType)
        {
            container.RegisterInstance<RegisteredConnection>(new RegisteredConnection() { TInputType = inputType, TOutputType = outputType }, outputType.Name + inputType.Name);

        }
        //public static IEnumerable<IDiagramNode> GetContainingNodesInProject(this IGraphFilter filter, IProjectRepository repository)
        //{
        //    return GetContainerNodesInProjectInternal(filter, repository).Distinct();
        //}

        //private static IEnumerable<IDiagramNode> GetContainerNodesInProjectInternal(IGraphFilter filter, IProjectRepository repository)
        //{
        //    foreach (var item in repository.Graphs)
        //    {
        //        var positionData = item.PositionData;

        //        FilterLocations locations;
        //        if (positionData.Positions.TryGetValue(filter.Identifier, out locations))
        //        {
        //            foreach (var node in repository.NodeItems)
        //            {
        //                if (node == filter) continue;
        //                if (locations.Keys.Contains(node.Identifier))
        //                {
        //                    yield return node;
        //                }
        //            }
        //        }
        //    }
        //}


        //public static IEnumerable<IDiagramNode> GetParentNodes(this IDiagramNode node)
        //{
        //    foreach (var item in node.Project.PositionData.Positions)
        //    {
        //        if (item.Value.Keys.Contains(node.Identifier))
        //        {
        //            yield return node.Project.NodeItems.FirstOrDefault(p => p.Identifier == item.Key);
        //        }
        //    }
        //}
        //public static IEnumerable<IDiagramNode> GetContainingNodesResursive(this IDiagramFilter filter, INodeRepository repository)
        //{
        //    foreach (var item in filter.GetContainingNodes(repository))
        //    {
        //        yield return item;
        //        if (item is IDiagramFilter)
        //        {
        //            var result = GetContainingNodesResursive(item as IDiagramFilter, repository);
        //            foreach (var subItem in result)
        //                yield return subItem;

        //        }
        //    }
        //}
        public static void RegisterFilterNode<TFilterData, TAllowedItem>(this IQFrameworkContainer container)
        {
            if (!FilterExtensions.AllowedFilterNodes.ContainsKey(typeof(TFilterData)))
            {
                FilterExtensions.AllowedFilterNodes.Add(typeof(TFilterData), new List<Type>());
            }
            FilterExtensions.AllowedFilterNodes[typeof(TFilterData)].Add(typeof(TAllowedItem));
        }
        public static void RegisterFilterNode(this IQFrameworkContainer container, Type filter, Type tnode)
        {
            if (!FilterExtensions.AllowedFilterNodes.ContainsKey(filter))
            {
                FilterExtensions.AllowedFilterNodes.Add(filter, new List<Type>());
            }
            FilterExtensions.AllowedFilterNodes[filter].Add(tnode);
        }
        public static void RegisterFilterNode<TFilterData, TAllowedItem>()
        {
            if (!FilterExtensions.AllowedFilterNodes.ContainsKey(typeof(TFilterData)))
            {
                FilterExtensions.AllowedFilterNodes.Add(typeof(TFilterData), new List<Type>());
            }
            FilterExtensions.AllowedFilterNodes[typeof(TFilterData)].Add(typeof(TAllowedItem));
        }

        public static IEnumerable<IGraphItem> AllGraphItems(this IGraphFilter filter)
        {
            foreach (var item in filter.FilterNodes)
            {
                yield return item;
                foreach (var child in item.GraphItems)
                {
                    yield return child;
                }
            }
        }

        public static bool IsAllowed(this IGraphFilter filter, object item, Type t)
        {

            if (filter == item) return true;

            if (!AllowedFilterNodes.ContainsKey(filter.GetType())) return false;

            foreach (var x in AllowedFilterNodes[filter.GetType()])
            {
                if (t.IsAssignableFrom(x)) return true;
            }
            return false;
            // return InvertGraphEditor.AllowedFilterNodes[filter.GetType()].Contains(t);
        }

        public static bool IsItemAllowed(this IGraphFilter filter, object item, Type t)
        {
            return true;
            //return uFrameEditor.AllowedFilterItems[filter.GetType()].Contains(t);
        }

        private static Dictionary<Type, List<Type>> _allowedFilterItems;

        private static Dictionary<Type, List<Type>> _allowedFilterNodes;

        public static Dictionary<Type, List<Type>> AllowedFilterItems
        {
            get { return _allowedFilterItems ?? (_allowedFilterItems = new Dictionary<Type, List<Type>>()); }
            set { _allowedFilterItems = value; }
        }

        public static Dictionary<Type, List<Type>> AllowedFilterNodes
        {
            get { return _allowedFilterNodes ?? (_allowedFilterNodes = new Dictionary<Type, List<Type>>()); }
            set { _allowedFilterNodes = value; }
        }
    }
}