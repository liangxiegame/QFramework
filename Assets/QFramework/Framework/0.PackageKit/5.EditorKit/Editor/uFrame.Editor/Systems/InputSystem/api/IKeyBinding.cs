using System;
using QF.GraphDesigner;
using UnityEngine;
namespace QF.GraphDesigner
{ 
    public interface IKeyBinding
    {
        bool RequireShift { get; set; }
        bool RequireAlt { get; set; }
        bool RequireControl { get; set; }
        KeyCode Key { get; set; }


        string Name { get; set; }
        Type CommandType { get; }
	    ICommand Command { get; }
    }
}