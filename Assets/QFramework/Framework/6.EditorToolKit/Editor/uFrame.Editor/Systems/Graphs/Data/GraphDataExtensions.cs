using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using Invert.Data;
using UnityEngine;

namespace QF.GraphDesigner
{
    public static class GraphDataExtensions
    {
        public static FilterItem ShowInFilter(this IGraphFilter filter, IDiagramNode node, Vector2 position, bool collapsed = false)
        {
            var filterItem = new FilterItem()
            {
                FilterId = filter.Identifier,
                NodeId = node.Identifier,
                Position = position,
                Collapsed = collapsed
            };
            filter.Repository.Add(filterItem);
            var filterNode = filter as IDiagramNode;
            if (filterNode != null)
            {
                filterNode.NodeAddedInFilter(node);
            }
            return filterItem;
        }
        public static void HideInFilter(this IGraphFilter filter, IDiagramNode node)
        {
            filter.Repository.RemoveAll<FilterItem>(p => p.FilterId == filter.Identifier && p.NodeId == node.Identifier);
        }
        public static IEnumerable<IDiagramNode> GetImportableItems(this IGraphFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");
            var items = filter.FilterNodes.Select(p=>p.Identifier).ToArray();

            return
                filter.GetAllowedDiagramItems()
                    .Where(p => !items.Contains(p.Identifier))
                    .ToArray();
        }

        public static IEnumerable<IDiagramNode> GetAllowedDiagramItems(this IGraphFilter filter)
        {

            return filter.Repository.AllOf<IDiagramNode>().Where(p => filter.IsAllowed(p, p.GetType()));
        }

        public static IGraphFilter Container(this IDiagramNode node)
        {
            foreach (var item in node.Repository.All<FilterItem>())
            {
                if (item.NodeId == node.Identifier)
                {
                    return item.Filter;
                }
            }
            return null;
        }



        public static IEnumerable<IGraphFilter> FilterPath(this IDiagramNode node)
        {
            return FilterPathInternal(node).Reverse();
        }

        private static IEnumerable<IGraphFilter> FilterPathInternal(IDiagramNode node)
        {
            var container = node.Container();
            while (container != null)
            {
                yield return container;
                var filterNode = container as IDiagramNode;
                if (filterNode != null)
                {
                    container = filterNode.Container();
                    if (container == filterNode)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }

            }
        }
        public static IEnumerable<IGraphFilter> GetFilterPath(this IGraphData t)
        {
            return t.FilterStack.Reverse();
        }

        //public static IEnumerable<IDiagramNode> FilterItems(this IGraphData designerData, INodeRepository repository)
        //{
        //    return designerData.CurrentFilter.FilterItems(repository);
        //}



    

        public static IDiagramNode RelatedNode(this ITypedItem item)
        {
            var gt = item as GenericTypedChildItem;
            if (gt != null)
            {
                return gt.RelatedTypeNode as IDiagramNode;
            }
            
            return item.Repository.GetById<IDiagramNode>(item.RelatedType);
        }

       

    }
}