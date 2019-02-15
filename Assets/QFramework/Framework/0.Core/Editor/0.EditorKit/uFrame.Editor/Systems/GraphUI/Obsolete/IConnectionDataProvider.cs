using System;
using System.Collections.Generic;
using QFramework.GraphDesigner;

[Obsolete]
public interface IConnectionDataProvider
{
    IEnumerable<ConnectionData> Connections { get; }
}