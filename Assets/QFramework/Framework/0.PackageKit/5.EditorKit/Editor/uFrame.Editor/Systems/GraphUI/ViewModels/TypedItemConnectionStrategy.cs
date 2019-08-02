using UnityEngine;

namespace QF.GraphDesigner
{
    public class TypedItemConnectionStrategy : DefaultConnectionStrategy
    {
        public override Color ConnectionColor
        {
            get { return Color.white; }
        }

        public override void Remove(ConnectorViewModel output, ConnectorViewModel input)
        {
            var typedItem = output.DataObject as ITypedItem;
            if (typedItem != null)
            {
                typedItem.RemoveType();
            }
        }

        public override bool IsConnected(ConnectorViewModel output, ConnectorViewModel input)
        {
            if (output.DataObject == input.DataObject) return false;
            var typedItem = output.DataObject as ITypedItem;
            var classItem = input.DataObject as IClassTypeNode;
            if (typedItem != null && classItem != null)
            {
                if (typedItem.RelatedType == classItem.Identifier)
                {
                    return true;
                }
            }
            return false;
        }

        public override ConnectionViewModel Connect(DiagramViewModel diagramViewModel, ConnectorViewModel a, ConnectorViewModel b)
        {
            var typedItem = a.DataObject as GenericTypedChildItem;
            var clsType = b.DataObject as IClassTypeNode;
            if (clsType != null && typedItem != null)
            {
                if (a.Direction == ConnectorDirection.Output && b.Direction == ConnectorDirection.Input)
                    return CreateConnection(diagramViewModel, a, b, Apply);
            }
            return null;
            return base.Connect(diagramViewModel, a, b);
        }

        protected override void ApplyConnection(IGraphData graph, IConnectable output, IConnectable input)
        {
            //base.ApplyConnection(graph, output, input);
            var typedItem = output as ITypedItem;
            
            if (typedItem != null)
            {
                typedItem.RelatedType = input.Identifier;
            }
        }

        //public override void GetConnections(List<ConnectionViewModel> connections, ConnectorInfo info)
        //{
        //    base.GetConnections(connections, info);
        //    foreach (var item in info.Outputs.Where(p => p.ConnectorFor.DataObject is ITypedItem))
        //    {
        //        var referenceItem = item.ConnectorFor.DataObject as ITypedItem;
        //        if (referenceItem == null) continue;
        //        var sourceObject = referenceItem.RelatedNode();
        //        if (sourceObject == null) continue;
        //        foreach (var input in info.Inputs.Where(p => p.ConnectorFor.DataObject == sourceObject))
        //        {
        //            connections.Add(new ConnectionViewModel(info.DiagramViewModel)
        //            {
        //                Remove = Remove,
        //                Name = item.Name + "->" + input.Name,
        //                ConnectorA = item,
        //                ConnectorB = input,
        //                Color = new Color(0.3f, 0.4f, 0.75f)
        //            });
        //        }
        //    }
        //}

        public override void Remove(ConnectionViewModel connectionViewModel)
        {
            
            base.Remove(connectionViewModel);
            var obj = connectionViewModel.ConnectorA.ConnectorFor.DataObject as ITypedItem;
            if (obj != null)
            {
                obj.RelatedType = null;
            }
        }
    }
}