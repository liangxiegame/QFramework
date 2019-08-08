using System;
using System.Reflection;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class NodeInputConfig : GraphItemConfiguration
    {
        //public bool AllowMultiple { get; set; }
        public ConfigProperty<IDiagramNodeItem, string> Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private ConfigProperty<IDiagramNodeItem, string> _name;

        public ConnectorStyle Style { get; set; }

        public Color Tint { get; set; }

        public NodeInputConfig NameConfig(ConfigProperty<IDiagramNodeItem, string> name)
        {
            Name = name;
            return this;
        }

        public NodeInputConfig NameConfig(string literal)
        {
            Name = new ConfigProperty<IDiagramNodeItem, string>(literal);
            return this;
        }

        public NodeInputConfig NameConfig(Func<IDiagramNodeItem, string> selector)
        {
            Name = new ConfigProperty<IDiagramNodeItem, string>(selector);
            return this;
        }
        //public string Name { get; set; }
        public string OutputName { get; set; }
        public bool IsAlias { get; set; }
        public Func<IDiagramNodeItem, IDiagramNodeItem, bool> Validator { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public Slot AttributeInfo { get; set; }

        public IDiagramNodeItem GetDataObject(GenericNode node)
        {
            if (IsAlias) return node;
            if (PropertyInfo != null)
            {
                var result = PropertyInfo.GetValue(node, null) as GenericSlot;
                if (result == null)
                {
                    var slot = Activator.CreateInstance((Type)PropertyInfo.PropertyType) as GenericSlot;
                    slot.Node = node;
                    slot.Name = AttributeInfo.Name;
                    PropertyInfo.SetValue(node, slot, null);
                    return slot;
                }
                return result;
            }
            return node.GetConnectionReference(ReferenceType);
        }

    }
}