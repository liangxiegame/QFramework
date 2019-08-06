using System.Collections.Generic;
using System.Linq;

namespace QF.GraphDesigner
{
    public class EnumNode : GenericNode, IClassTypeNode
    {
        public override bool AllowOutputs
        {
            get { return false; }
        }

        public string ClassName
        {
            get { return Name; }
        }

        [Section("Enum Items", SectionVisibility.Always)]
        public IEnumerable<EnumChildItem> Items
        {
            get { return PersistedItems.OfType<EnumChildItem>(); }
        }

        public override bool IsEnum
        {
            get { return true; }
        }
    }
}