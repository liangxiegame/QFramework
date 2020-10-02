using System;
using QF;
using QFramework;


namespace QFramework.CodeGen
{
    public static class ConfigExtensions
    {


        public static NodeConfig<TNode> GetNodeConfig<TNode>(this IQFrameworkContainer container) where TNode : GenericNode, IConnectable
        {
            var config = GetNodeConfig(container, typeof(TNode)) as NodeConfig<TNode>;

            return config;
        }

        public static NodeConfigBase GetNodeConfig(this IQFrameworkContainer container, Type nodeType)
        {
            
            var config = container.Resolve<NodeConfigBase>(nodeType.Name);    
            if (config == null)
            {
                var nodeconfigType = typeof (NodeConfig<>).MakeGenericType(nodeType);
                var nodeConfig = Activator.CreateInstance(nodeconfigType,container) as NodeConfigBase;
                nodeConfig.NodeType = nodeType;
                container.RegisterInstance(nodeConfig, nodeType.Name);
                //nodeConfig.Section(string.Empty, _ => _.PersistedItems.OfType<GenericConnectionReference>(), false);
                return nodeConfig;
            }
            return config;
        }
        
    }
}