using System;
using System.ComponentModel;

namespace QFramework.CodeGen
{
    [Browsable(false)]
    public class ReferenceSection : Section
    {
        private bool _editable = true;

        public Type ReferenceType { get; set; }
        public bool AllowDuplicates { get; set; }
        public bool Automatic { get; set; }
        public bool HasPredefinedOptions { get; set; }
        public bool Editable
        {
            get { return _editable; }
            set { _editable = value; }
        }

        public ReferenceSection(string name, SectionVisibility visibility, bool allowDuplicates, bool automatic, bool editable = true) : base(name, visibility)
        {
            AllowDuplicates = allowDuplicates;
            Automatic = automatic;
            Editable = editable;
        }
    }
}