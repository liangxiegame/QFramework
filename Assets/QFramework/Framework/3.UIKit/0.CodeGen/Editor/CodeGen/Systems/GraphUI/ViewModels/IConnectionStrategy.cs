using System.Collections.Generic;
using UnityEngine;

namespace QFramework.CodeGen
{
    public interface IConnectionStrategy
    {
        ///// <summary>
        ///// Iterate through connectors and find decorate the connections list with any found connections.
        ///// </summary>
        ///// <param name="connections"></param>
        ///// <param name="info"></param>
        //void GetConnections(List<ConnectionViewModel> connections, ConnectorInfo info);

        bool IsConnected(ConnectorViewModel output, ConnectorViewModel input);

        Color ConnectionColor { get; }

        void Remove(ConnectorViewModel output, ConnectorViewModel input);
    }
}