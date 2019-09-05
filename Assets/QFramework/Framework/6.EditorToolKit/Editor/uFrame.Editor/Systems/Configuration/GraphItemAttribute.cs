using System;

namespace QF.GraphDesigner
{
    public class GraphItemAttribute : Attribute
    {
        private bool _isNewRow = true;

        public bool IsNewRow
        {
            get { return _isNewRow; }
            set { _isNewRow = value; }
        }

        public int OrderIndex { get; set; }
    }
}