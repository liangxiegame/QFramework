using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using QFramework.CodeGen;
using Invert.Data;
using QF;
using QFramework;

namespace QFramework.CodeGen
{


    public class NodeConfig : NodeConfigBase
    {
        
        private Dictionary<Type, NodeGeneratorConfigBase> _typeGeneratorConfigs;

        public Dictionary<Type, NodeGeneratorConfigBase> TypeGeneratorConfigs
        {
            get { return _typeGeneratorConfigs ?? (_typeGeneratorConfigs = new Dictionary<Type, NodeGeneratorConfigBase>()); }
            set { _typeGeneratorConfigs = value; }
        }


        private List<Func<ConfigCodeGeneratorSettings, CodeGenerator>> _codeGenerators;


        

        public NodeConfig(IQFrameworkContainer container)
            : base(container)
        {

        }


        public class ConfigCodeGeneratorSettings
        {
        }
    }
}