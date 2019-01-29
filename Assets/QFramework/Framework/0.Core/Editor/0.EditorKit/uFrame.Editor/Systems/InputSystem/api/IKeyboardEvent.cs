using UnityEngine;

namespace QFramework.GraphDesigner
{
    public interface IKeyboardEvent
    {
        bool KeyEvent(KeyCode keyCode, ModifierKeyState state);
    }
}