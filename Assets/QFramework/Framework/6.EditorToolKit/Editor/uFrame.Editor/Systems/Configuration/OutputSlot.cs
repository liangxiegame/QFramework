using System;

namespace QF.GraphDesigner
{
    public class OutputSlot : Slot
    {
        public OutputSlot(string name) : base(name)
        {
        }

        public OutputSlot(string name, Type sourceType, bool allowMultiple) : base(name, sourceType, allowMultiple)
        {
        }

        public OutputSlot(string name, bool allowMultiple, SectionVisibility visibility) : base(name, allowMultiple, visibility)
        {
        }
    }
}