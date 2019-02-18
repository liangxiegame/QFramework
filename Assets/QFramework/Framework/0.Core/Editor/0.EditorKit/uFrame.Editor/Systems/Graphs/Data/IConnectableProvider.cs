using System.Collections.Generic;
using Invert.Data;

namespace QFramework.GraphDesigner
{
    public interface IConnectableProvider : IDataRecord
    {
        IEnumerable<IConnectable> Connectables { get; }
    }
}