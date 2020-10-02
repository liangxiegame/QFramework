using System.Collections.Generic;
using Invert.Data;

namespace QFramework.CodeGen
{
    public interface IConnectableProvider : IDataRecord
    {
        IEnumerable<IConnectable> Connectables { get; }
    }
}