using System;

namespace QF.GraphDesigner
{
    public class Section : GraphItemAttribute
    {
        public string Name { get; set; }
        public SectionVisibility Visibility { get; set; }

        public Section(string name, SectionVisibility visibility)
        {
            Name = name;
            Visibility = visibility;
        }

        public Type AddCommandType { get; set; }
    }
}