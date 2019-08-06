using System;
using System.Collections.Generic;
using QF.GraphDesigner;

[Obsolete]
public interface IConnectionDataProvider
{
    IEnumerable<ConnectionData> Connections { get; }
}