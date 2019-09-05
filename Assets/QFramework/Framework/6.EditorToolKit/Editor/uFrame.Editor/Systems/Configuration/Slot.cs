using System;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class Slot : GraphItemAttribute
    {
        
        public string Name { get; set; }
        public Type SourceType { get; set; }
        public bool AllowMultiple { get; set; }
        public SectionVisibility Visibility { get; set; }

        public ConnectorStyle Style { get; set; }

        public Color Tint { get; set; }

        public Slot(string name)
        {
            Name = name;
        }

        public Slot(string name, bool allowMultiple, SectionVisibility visibility)
        {
            Name = name;
            AllowMultiple = allowMultiple;
            Visibility = visibility;
        }

        public Slot(string name, Type sourceType, bool allowMultiple)
        {
            Name = name;
            SourceType = sourceType;
            AllowMultiple = allowMultiple;
        }

        public Slot(string name, Type sourceType, bool allowMultiple, SectionVisibility visibility)
        {
            Name = name;
            SourceType = sourceType;
            AllowMultiple = allowMultiple;
            Visibility = visibility;
        }
    }
}