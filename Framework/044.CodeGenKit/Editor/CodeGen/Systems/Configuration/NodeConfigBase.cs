using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace QFramework.CodeGen
{
    public abstract class NodeConfigBase : GraphItemConfiguration
    {
        protected NodeConfigBase(IQFrameworkContainer container)
        {
            Container = container;
        }

        public Type NodeType
        {
            get { return _nodeType; }
            set
            {
                _nodeType = value;
                LoadByRefelection();
            }
        }

        public List<GraphItemConfiguration> GraphItemConfigurations
        {
            get { return _graphItemConfigurations ?? (_graphItemConfigurations = new List<GraphItemConfiguration>()); }
            set { _graphItemConfigurations = value; }
        }


        private void LoadByRefelection()
        {
            AllAttributes = NodeType.GetPropertiesWithAttributeByType<GraphItemAttribute>().ToArray();
            foreach (var item in AllAttributes)
            {
                var proxy = item.Value as ConfigProxy;
                if (proxy != null)
                {
                    var c = new ConfigurationProxyConfiguration()
                    {
                        ReferenceType = typeof(GraphItemConfiguration),
                        SourceType = typeof(GraphItemConfiguration),
                        IsInput = false,
                        IsOutput = false,
                        OrderIndex = proxy.OrderIndex,
                        Visibility = proxy.Visibility,
                        ConfigSelector =
                            (node) => ((IEnumerable) item.Key.GetValue(node, null)).Cast<GraphItemConfiguration>()
                    };
                    GraphItemConfigurations.Add(c);
                    continue;
                }


                var section = item.Value as Section;
                if (section != null)
                {
                    var property1 = item.Key;
                    var section1 = section;
                    var result = CreateSectionConfiguration(property1, section1);
                    result.OrderIndex = section.OrderIndex;
                    GraphItemConfigurations.Add(result);
                }
            }
        }

        private GraphItemConfiguration CreateSectionConfiguration(PropertyInfo property, Section section)
        {
            var sectionConfig = new NodeConfigSectionBase
            {
                Name = section.Name,
                IsProxy = section is ProxySection,
                Visibility = section.Visibility,
                SourceType = property.PropertyType.GetGenericParameter(),
                AddCommandType = section.AddCommandType
            };

            var referenceSection = section as ReferenceSection;
            if (referenceSection != null)
            {
                sectionConfig.AllowDuplicates = referenceSection.AllowDuplicates;
                sectionConfig.AllowAdding = !referenceSection.Automatic;
                sectionConfig.ReferenceType = referenceSection.ReferenceType ??
                                              sectionConfig.SourceType.GetGenericParameter() ??
                                              property.PropertyType.GetGenericParameter();
                sectionConfig.IsEditable = referenceSection.Editable;
                sectionConfig.HasPredefinedOptions = referenceSection.HasPredefinedOptions;

                if (sectionConfig.ReferenceType == null)
                {
                    throw new Exception(string.Format(
                        "Reference Section on property {0} doesn't have a valid ReferenceType.", property.Name));
                }

                //sectionConfig.GenericSelector = (node) =>
                //{

                //};
            }

            if (sectionConfig.IsProxy || referenceSection == null)
            {
                var property1 = property;

                if (sectionConfig.IsProxy)
                    sectionConfig.AllowAdding = false;

                sectionConfig.GenericSelector = (node) =>
                {
                    var enumerator = property1.GetValue(node, null) as IEnumerable;
                    if (enumerator == null) return null;
                    return enumerator.Cast<IGraphItem>();
                };
            }
            else if (referenceSection != null)
            {
                var possibleSelectorProperty = NodeType.GetProperty("Possible" + property.Name);
                if (possibleSelectorProperty != null)
                {
                    sectionConfig.GenericSelector = (node) =>
                    {
                        var propertyN = NodeType.GetProperty("Possible" + property.Name);
                        var enumerator = propertyN.GetValue(node, null) as IEnumerable;

                        if (enumerator == null) return null;
                        return enumerator.OfType<IGraphItem>();
                    };
                }
                else
                {
                    sectionConfig.GenericSelector = (node) =>
                    {
                        return node.Repository.AllOf<IGraphItem>().Where(p =>
                            referenceSection.ReferenceType.IsInstanceOfType(p));
                    };
                }
            }

            return sectionConfig;
        }

        public KeyValuePair<PropertyInfo, GraphItemAttribute>[] AllAttributes { get; set; }

        private string _name;


        private List<string> _tags;
        private Type _nodeType;

        private List<GraphItemConfiguration> _graphItemConfigurations;


        public IQFrameworkContainer Container { get; set; }
    }
}