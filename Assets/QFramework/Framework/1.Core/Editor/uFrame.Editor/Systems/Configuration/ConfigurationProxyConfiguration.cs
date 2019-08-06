using System;
using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class ConfigurationProxyConfiguration : GraphItemConfiguration
    {
        public Func<GenericNode, IEnumerable<GraphItemConfiguration>> ConfigSelector { get; set; }
    }
}