using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QF.GraphDesigner
{

    public interface IKeyDown
    {
        bool KeyDown(bool control, bool alt, bool shift, KeyCode character);
    }

    public interface IKeyUp
    {
        bool KeyUp(bool control, bool alt, bool shift, KeyCode character);
    }

    public interface IMouseDown
    {
        void MouseDown(MouseEvent mouse);
    }

    public interface IMouseUp
    {
        void MouseUp(MouseEvent mouse);
    }
}
