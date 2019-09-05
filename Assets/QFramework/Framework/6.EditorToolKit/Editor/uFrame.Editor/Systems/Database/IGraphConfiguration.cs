using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public interface IGraphConfiguration : IItem
    {
        string CodeOutputPath { get;  }
        string Namespace { get; set; }
        bool IsCurrent { get; set; }
        string FullPath { get;  }
    }
}