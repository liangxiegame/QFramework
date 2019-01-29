using System;
using System.Collections.Generic;

namespace QFramework.GraphDesigner
{
    public class SelectTypeCommand : Command
    {

        public bool AllowNone { get; set; }
        public bool PrimitiveOnly { get; set; }
        public bool IncludePrimitives { get; set; }
        public Action OnSelectionFinished { get; set; }
        public Predicate<ITypeInfo> Filter { get; set; }
        public ITypedItem Item { get; set; }
    }
}