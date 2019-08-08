using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QF.Json;
using QF.GraphDesigner;


namespace QF.GraphDesigner
{
    public abstract class NodeConfigBase : GraphItemConfiguration
    {
        public bool AllowMultipleInputs { get; set; }
        public bool AllowMultipleOutputs { get; set; }

        protected NodeConfigBase(IQFrameworkContainer container)
        {
            Container = container;
        }

        public string Name
        {
            get { return _name ?? NodeType.Name; }
            set { _name = value; }
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
                        IsOutput =  false,
                        OrderIndex = proxy.OrderIndex,
                        Visibility = proxy.Visibility,
                        ConfigSelector =
                            (node) => ((IEnumerable) item.Key.GetValue(node, null)).Cast<GraphItemConfiguration>()
                    };
                    GraphItemConfigurations.Add(c);
                    continue;
                }
                var slot = item.Value as Slot;
                if (slot != null)
                {
                    var result = CreateSlotConfiguration(item.Key, slot);
                    result.OrderIndex = item.Value.OrderIndex;
                    GraphItemConfigurations.Add(result);
                    Slots.Add(item.Key, slot);
                    if (result.IsOutput) {
                        //Debug.Log(string.Format("Registering output {0} : {1}", result.ReferenceType, result.SourceType.Name));
                        // TODO ??

                        Container.RegisterConnectable(result.ReferenceType, result.SourceType);

                    }
                    else
                    {
                       // Debug.Log(string.Format("Registering input {0} : {1}", result.SourceType.Name, result.ReferenceType));

                        Container.RegisterConnectable(result.SourceType, result.ReferenceType);

                    }
                    
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

            SerializedProperties = NodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p=>p.GetCustomAttributes(typeof(JsonProperty), true).Any()).ToArray();
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
                                              sectionConfig.SourceType.GetGenericParameter() ?? property.PropertyType.GetGenericParameter();
                sectionConfig.IsEditable = referenceSection.Editable;
                sectionConfig.HasPredefinedOptions = referenceSection.HasPredefinedOptions;
                
                if (sectionConfig.ReferenceType == null)
                {
                    throw new Exception(string.Format("Reference Section on property {0} doesn't have a valid ReferenceType.", property.Name));
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
                        return node.Repository.AllOf<IGraphItem>().Where(p => referenceSection.ReferenceType.IsAssignableFrom(p.GetType()));
                    };

                }

            }
            return sectionConfig;
        }

        private GraphItemConfiguration CreateSlotConfiguration(PropertyInfo property, Slot slot)
        {
            var config = new NodeInputConfig()
            {
                Name = new ConfigProperty<IDiagramNodeItem, string>(slot.Name),
                IsInput = slot is InputSlot,
                IsOutput = slot is OutputSlot,
                Visibility = slot.Visibility,
                ReferenceType = property.PropertyType,
                SourceType = slot.SourceType ?? property.PropertyType.GetGenericParameter(),
                PropertyInfo = property,
                AttributeInfo = slot,
                Tint = slot.Tint,
                Style = slot.Style
            };

            return config;
        }

        public KeyValuePair<PropertyInfo, GraphItemAttribute>[] AllAttributes { get; set; }

        public PropertyInfo[] SerializedProperties { get; set; }
        private string _name;




        private List<string> _tags;
        private Type _nodeType;
        private Dictionary<PropertyInfo, Slot> _slots;
        private List<GraphItemConfiguration> _graphItemConfigurations;
       // private List<Func<OutputGenerator>> _outputGenerators;
        private bool _allowAddingInMenu = true;

        public IEnumerable<NodeConfigSectionBase> Sections
        {
            get { return GraphItemConfigurations.OfType<NodeConfigSectionBase>(); }
        } 

        public Dictionary<PropertyInfo, Slot> Slots
        {
            get { return _slots ?? (_slots = new Dictionary<PropertyInfo, Slot>()); }
            set { _slots = value; }
        }

        public IEnumerable<KeyValuePair<PropertyInfo, Slot>> InputSlots
        {
            get { return Slots.Where(p => p.Value is InputSlot); }
        }
        public IEnumerable<KeyValuePair<PropertyInfo, Slot>> OutputSlots
        {
            get { return Slots.Where(p => p.Value is OutputSlot); }
        } 

        public IEnumerable<NodeInputConfig> Inputs
        {
            get { return GraphItemConfigurations.OfType<NodeInputConfig>().Where(p => p.IsInput); }
      
        }
        public IEnumerable<NodeInputConfig> Outputs
        {
            get { return GraphItemConfigurations.OfType<NodeInputConfig>().Where(p => p.IsOutput); }

        }


        public IQFrameworkContainer Container { get; set; }

        public List<string> Tags
        {
            get { return _tags ?? (_tags = new List<string>()); }
            set { _tags = value; }
        }

        public abstract bool IsValid(GenericNode node);

        //public List<Func<OutputGenerator>> OutputGenerators
        //{
        //    get { return _outputGenerators ?? (_outputGenerators = new List<Func<OutputGenerator>>()); }
        //    set { _outputGenerators = value; }
        //}

        public bool AllowAddingInMenu
        {
            get { return _allowAddingInMenu; }
            set { _allowAddingInMenu = value; }
        }

        //public void AddOutputGenerator(Func<OutputGenerator> action)
        //{
        //    OutputGenerators.Add(action);
        //}

        public abstract NodeColor GetColor(IGraphItem obj);
    }


}