using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;

namespace QFramework.GraphDesigner
{
    public class GenericNodeChildItem : DiagramNodeItem
    {

        public override string FullLabel
        {
            get { return Name; }
        }

        public override string Label
        {
            get { return Name; }
        }

        public override void Remove(IDiagramNode diagramNode)
        {

        }



        public override void OnConnectedToInput(IConnectable input)
        {
            base.OnConnectedToInput(input);
            
        }

        //private List<string> _connectedGraphItemIds = new List<string>();

        //public IEnumerable<IGraphItem> ConnectedGraphItems
        //{
        //    get
        //    {
        //        foreach (var item in Node.Project.NodeItems)
        //        {
        //            if (ConnectedGraphItemIds.Contains(item.Identifier))
        //                yield return item;

        //            foreach (var child in item.ContainedItems)
        //            {
        //                if (ConnectedGraphItemIds.Contains(child.Identifier))
        //                {
        //                    yield return child;
        //                }
        //            }
        //        }
        //    }
        //}

        //public List<string> ConnectedGraphItemIds
        //{
        //    get { return _connectedGraphItemIds; }
        //    set { _connectedGraphItemIds = value; }
        //}



    }
}