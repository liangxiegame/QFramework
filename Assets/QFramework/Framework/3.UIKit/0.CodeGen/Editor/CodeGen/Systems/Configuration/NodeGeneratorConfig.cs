using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace QFramework.CodeGen
{
   
    public class NodeGeneratorConfig<TNode> : NodeGeneratorConfigBase
        where TNode : GenericNode
    {

        public Func<TNode, bool> Condition { get; set; } 

        public ConfigProperty<TNode, IEnumerable<string>> Namespaces
        {
            get { return _namespaces; }
            set { _namespaces = value; }
        }

        private ConfigProperty<TNode, IEnumerable<string>> _namespaces;

        public ConfigProperty<TNode, CodeTypeDeclaration> DesignerDeclaration
        {
            get { return _designerDeclaration; }
            set { _designerDeclaration = value; }
        }

        private ConfigProperty<TNode, CodeTypeDeclaration> _designerDeclaration;

        public ConfigProperty<TNode, CodeTypeDeclaration> Declaration
        {
            get { return _declaration; }
            set { _declaration = value; }
        }

        private ConfigProperty<TNode, CodeTypeDeclaration> _declaration;


        private ConfigProperty<TNode, CodeTypeReference> _baseType;

        public ConfigProperty<TNode, CodeTypeReference> BaseType
        {
            get { return _baseType; }
            set { _baseType = value; }
        }

        private ConfigProperty<TNode, string> _className = new ConfigProperty<TNode,string>(_=>_.Name);

        public ConfigProperty<TNode, string> ClassName
        {
            get { return _className; }
            set { _className = value; }
        }
    }
}