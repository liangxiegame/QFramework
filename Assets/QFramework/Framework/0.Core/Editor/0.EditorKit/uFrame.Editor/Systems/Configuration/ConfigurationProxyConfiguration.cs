using System;
using System.Collections.Generic;

namespace QFramework.GraphDesigner
{
    public class ConfigurationProxyConfiguration : GraphItemConfiguration
    {
        public Func<GenericNode, IEnumerable<GraphItemConfiguration>> ConfigSelector { get; set; }
    }
}