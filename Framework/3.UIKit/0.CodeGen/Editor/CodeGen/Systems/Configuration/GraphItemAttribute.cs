using System;

namespace QFramework.CodeGen
{
    public class GraphItemAttribute : Attribute
    {
        private bool _isNewRow = true;

        public int OrderIndex { get; set; }
    }
}