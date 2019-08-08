using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class InputOutputStrategy : DefaultConnectionStrategy<IConnectable, IConnectable>
    {
        public override Color ConnectionColor
        {
            get { return Color.white; }
        }
        private static RegisteredConnection[] _connectionTypes;

        public static RegisteredConnection[] ConnectionTypes
        {
            get { return _connectionTypes ?? (_connectionTypes = InvertGraphEditor.Container.ResolveAll<RegisteredConnection>().ToArray()); }
        }


        public override void Remove(ConnectorViewModel output, ConnectorViewModel input)
        {
            
        }

        public override ConnectionViewModel Connect(DiagramViewModel diagramViewModel, ConnectorViewModel a, ConnectorViewModel b)
        {
             
            
            if (ConnectionTypes.Any(p => p.CanConnect(a.DataObject as IConnectable, b.DataObject as IConnectable)))
            {
                return base.Connect(diagramViewModel, a, b);
            }
            return null;
        }

    }

    //public class RegisteredConnectionStrategy : DefaultConnectionStrategy
    //{
    //    private static RegisteredConnection[] _connectionTypes;

    //    public static RegisteredConnection[] ConnectionTypes
    //    {
    //        get { return _connectionTypes ?? (_connectionTypes = InvertGraphEditor.Container.ResolveAll<RegisteredConnection>().ToArray()); }
    //    }

    //    public override Color ConnectionColor
    //    {
    //        get { return Color.white; }
    //    }

    //    public override ConnectionViewModel Connect(DiagramViewModel diagramViewModel, ConnectorViewModel a, ConnectorViewModel b)
    //    {
    //        if (ConnectionTypes.Any(p => p.CanConnect(a.DataObject.GetType(), b.DataObject.GetType())))
    //        {
    //            return CreateConnection(diagramViewModel, a, b, Apply);
    //        }
    //        return null;
    //    }
    //}
}