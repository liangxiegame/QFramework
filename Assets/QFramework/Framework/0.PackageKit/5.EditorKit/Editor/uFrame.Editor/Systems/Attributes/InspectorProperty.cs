using System;
using UnityEngine;

namespace QF.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InspectorProperty : Attribute
    {
        public InspectorType InspectorType { get; set; }

        public Type CustomDrawerType { get; set; }

        public string InspectorTip { get; set; }

        public InspectorProperty(Type customDrawerType)
        {
            CustomDrawerType = customDrawerType;
        }

        public InspectorProperty()
        {
            InspectorType = InspectorType.Auto;
        }     
        
        public InspectorProperty(string tip)
        {
            InspectorTip = tip;
        }

        public InspectorProperty(string tip, InspectorType inspectorType)
        {
            InspectorType = inspectorType;
            InspectorTip = tip;
        }        
        
        public InspectorProperty(InspectorType inspectorType)
        {
            InspectorType = inspectorType;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NodeFlag : Attribute
    {
        public NodeColor Color { get; set; }
        public string Name { get; set; }

        public NodeFlag(string name)
        {
            Name = name;
        }

        public NodeFlag(string name, NodeColor color)
        {
            Color = color;
            Name = name;
        }
    }
}