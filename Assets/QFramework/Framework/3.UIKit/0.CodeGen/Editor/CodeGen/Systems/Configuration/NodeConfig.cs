using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QFramework.CodeGen;
using Invert.Data;
using QF;
using QFramework;

namespace QFramework.CodeGen
{


    public class NodeConfig<TNode> : NodeConfigBase where TNode : GenericNode, IConnectable
    {
        
        private Dictionary<Type, NodeGeneratorConfigBase> _typeGeneratorConfigs;

        public Dictionary<Type, NodeGeneratorConfigBase> TypeGeneratorConfigs
        {
            get { return _typeGeneratorConfigs ?? (_typeGeneratorConfigs = new Dictionary<Type, NodeGeneratorConfigBase>()); }
            set { _typeGeneratorConfigs = value; }
        }


        private List<Func<ConfigCodeGeneratorSettings, CodeGenerator>> _codeGenerators;

        public List<NodeValidator<TNode>> Validators
        {
            get { return _validators ?? (_validators = new List<NodeValidator<TNode>>()); }
            set { _validators = value; }
        }

        public IEnumerable<NodeValidator<TNode>> Validate(TNode node)
        {
            return Validators.Where(p => p.Validate != null && p.Validate(node));
        }




        public override NodeColor GetColor(IGraphItem obj)
        {
            if (NodeColor == null)
            {
                return CodeGen.NodeColor.Gray;
            }
            return NodeColor.Literal;
        }


        public ConfigProperty<TNode, NodeColor> NodeColor
        {
            get { return _nodeColor; }
            set { _nodeColor = value; }
        }

        private ConfigProperty<TNode, NodeColor> _nodeColor;
        private List<NodeValidator<TNode>> _validators;
        

        public NodeConfig(IQFrameworkContainer container)
            : base(container)
        {

        }


        public class ConfigCodeGeneratorSettings
        {
            public TNode Data { get; set; }

        }

        public override bool IsValid(GenericNode node)
        {
            return !Validate(node as TNode).Any();
        }
    }
}