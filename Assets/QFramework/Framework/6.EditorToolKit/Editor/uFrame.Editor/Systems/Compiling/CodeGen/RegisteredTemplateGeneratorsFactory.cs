using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using Invert.Data;

namespace QF.GraphDesigner
{
    public class RegisteredTemplateGeneratorsFactory : DesignerGeneratorFactory<IDataRecord>
    {
        private static Dictionary<Type, List<Type>> _registeredTemplates = new Dictionary<Type, List<Type>>();

        protected static Dictionary<Type, List<Type>> RegisteredTemplates
        {
            get { return _registeredTemplates; }
            set { _registeredTemplates = value; }
        }

        public static void RegisterTemplate<TFor, TTemplate>()
            where TTemplate : class, IClassTemplate<TFor>, new()
            where TFor : class, IDataRecord
        {
            var type = typeof(TemplateClassGenerator<TFor, TTemplate>);
            List<Type> list;
            if (!RegisteredTemplates.TryGetValue(typeof(TFor), out list))
            {
                RegisteredTemplates.Add(typeof(TFor), list = new List<Type>());
            }
            if (!list.Contains(type))
                list.Add(type);
        }
        
        public static void UnRegisterTemplate<TFor>()
            where TFor : class, IDataRecord
        {
            List<Type> list;
            if (RegisteredTemplates.TryGetValue(typeof(TFor), out list))
            {
                RegisteredTemplates.Remove(typeof(TFor));
            }
        }
        
        public override IEnumerable<OutputGenerator> CreateGenerators(IGraphConfiguration graphConfig, IDataRecord item)
        {

            foreach (var template in RegisteredTemplates)
            {
                if (template.Key.IsAssignableFrom(item.GetType()))
                {
                    foreach (var templateType in template.Value)
                    {
                        foreach (var t in CreateTemplateGenerators(graphConfig, item, templateType))
                        {
                            yield return t;
                        }
                    }
                }
            }


        }

        private IEnumerable<OutputGenerator> CreateTemplateGenerators(IGraphConfiguration config, IDataRecord graphItem, Type templateType)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (graphItem == null) throw new ArgumentNullException("graphItem");
            if (templateType == null) throw new ArgumentNullException("templateType");

            var templateClassType = templateType.GetGenericArguments()[1];
            var templateAttribute = templateClassType.GetCustomAttributes(typeof(TemplateClass), true)
                .OfType<TemplateClass>()
                .FirstOrDefault();
            if (templateAttribute == null)
            {
                InvertApplication.Log(string.Format("ClassTemplate attribute not found on {0} ", templateClassType.Name));
                yield break;
            }


            if (templateAttribute.Location == TemplateLocation.DesignerFile || templateAttribute.Location == TemplateLocation.Both)
            {
                var template = Activator.CreateInstance(templateType) as CodeGenerator;
                template.ObjectData = graphItem;
                template.IsDesignerFile = true;

                //template.AssetDirectory = graphItem.Graph.Project.SystemDirectory;
                template.AssetDirectory = config.CodeOutputPath;
                yield return template;
          


            }
            if (templateAttribute.Location == TemplateLocation.EditableFile || templateAttribute.Location == TemplateLocation.Both)
            {
                var template = Activator.CreateInstance(templateType) as CodeGenerator;
                template.ObjectData = graphItem;
                template.IsDesignerFile = false;
                template.AssetDirectory = config.CodeOutputPath;

                yield return template;
        

            }
        }
    }
    public static class NodeConfigTemplateExtensions
    {
        public static void AddCodeTemplate<TNode,TGeneratorTemplate>(this NodeConfig<TNode> nodeConfig ) where TGeneratorTemplate : class, IClassTemplate<TNode>, new() where TNode : GenericNode
        {
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<TNode, TGeneratorTemplate>();
        
        }
    }

}