using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public interface IKeyboardEvent
    {
        bool KeyEvent(KeyCode keyCode, ModifierKeyState state);
    }
}