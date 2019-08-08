using System;

namespace QF.GraphDesigner
{
    public class InputSlot : Slot
    {
        public InputSlot(string name) : base(name)
        {
        }

        public InputSlot(string name, Type sourceType, bool allowMultiple) : base(name, sourceType, allowMultiple)
        {
        }

        public InputSlot(string name, bool allowMultiple, SectionVisibility visibility) : base(name, allowMultiple, visibility)
        {
        }
    }
}