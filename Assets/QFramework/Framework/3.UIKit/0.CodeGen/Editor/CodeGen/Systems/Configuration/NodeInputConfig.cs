using System;
using System.Reflection;
using UnityEngine;

namespace QFramework.CodeGen
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


        public Color Tint { get; set; }


        //public string Name { get; set; }

        public bool IsAlias { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

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
                    PropertyInfo.SetValue(node, slot, null);
                    return slot;
                }
                return result;
            }
            return node.GetConnectionReference(ReferenceType);
        }

    }
}