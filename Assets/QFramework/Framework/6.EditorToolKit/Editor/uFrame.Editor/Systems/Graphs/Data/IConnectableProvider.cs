using System.Collections.Generic;
using Invert.Data;

namespace QF.GraphDesigner
{
    public interface IConnectableProvider : IDataRecord
    {
        IEnumerable<IConnectable> Connectables { get; }
    }
}