using UnityEngine;

namespace QFramework.GraphDesigner.Unity
{
    public interface IKeyboardEvent
    {
        bool KeyEvent(KeyCode keyCode, ModifierKeyState state);
    }
}