using System.Collections.Generic;
using QF.GraphDesigner;
using Invert.Data;
using UnityEditor;

namespace QF.GraphDesigner
{
    public interface ICommandUI
    {
        void AddCommand(ICommand command);
        void Go();
        
        void GoBottom();
    }
}