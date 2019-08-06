using System;
using System.ComponentModel;

namespace QF.GraphDesigner
{
    [Browsable(false)]
    public class ReferenceSection : Section
    {
        private bool _editable = true;

        public ReferenceSection(string name, SectionVisibility visibility, bool allowDuplicates, bool automatic, Type referenceType, bool editable = true)
            : base(name, visibility)
        {
            AllowDuplicates = allowDuplicates;
            Automatic = automatic;
            ReferenceType = referenceType;
            Editable = editable;
        }

        public Type ReferenceType { get; set; }
        public bool AllowDuplicates { get; set; }
        public bool Automatic { get; set; }
        public bool HasPredefinedOptions { get; set; }
        public bool Editable
        {
            get { return _editable; }
            set { _editable = value; }
        }

        public ReferenceSection(string name, SectionVisibility visibility, bool allowDuplicates) : base(name, visibility)
        {
            AllowDuplicates = allowDuplicates;
        }

        public ReferenceSection(string name, SectionVisibility visibility, bool allowDuplicates, bool automatic, bool editable = true) : base(name, visibility)
        {
            AllowDuplicates = allowDuplicates;
            Automatic = automatic;
            Editable = editable;
        }
    }
}