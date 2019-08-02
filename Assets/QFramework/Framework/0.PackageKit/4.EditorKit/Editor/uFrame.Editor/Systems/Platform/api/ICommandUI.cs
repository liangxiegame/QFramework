using System.Collections.Generic;
using QFramework.GraphDesigner;
using Invert.Data;
using UnityEditor;

namespace QFramework.GraphDesigner
{
    public interface ICommandUI
    {
        void AddCommand(ICommand command);
        void Go();
        
        void GoBottom();
    }
}