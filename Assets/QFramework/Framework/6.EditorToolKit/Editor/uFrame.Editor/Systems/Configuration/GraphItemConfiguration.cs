using System;

namespace QF.GraphDesigner
{
    public class GraphItemConfiguration 
    {
        public int OrderIndex { get; set; }
        public Type ReferenceType { get; set; }
        public Type SourceType { get; set; }

  
        public SectionVisibility Visibility { get; set; }
        public bool IsInput { get; set; }
        public bool IsOutput { get; set; }


    }
}