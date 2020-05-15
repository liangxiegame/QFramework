using QFramework.CodeGen;

namespace QFramework.CodeGen
{
    public interface IGraphConfiguration : IItem
    {
        string CodeOutputPath { get;  }
        string Namespace { get; set; }
        bool IsCurrent { get; set; }
        string FullPath { get;  }
    }
}