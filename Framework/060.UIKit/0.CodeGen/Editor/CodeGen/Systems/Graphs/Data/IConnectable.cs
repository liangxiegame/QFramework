using System.Collections.Generic;
using UnityEngine;

namespace QFramework.CodeGen
{
    public interface IConnectable : IGraphItem
    {
       
        IGraphData Graph { get; }

        bool AllowInputs { get; }
        bool AllowOutputs { get; }

        string InputDescription { get; }
        string OutputDescription { get; }
        bool CanOutputTo(IConnectable input);
        bool CanInputFrom(IConnectable output);



    }
}