using System.Collections.Generic;
using Invert.uFrame.Editor;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class MouseEvent
    {
        private Stack<IInputHandler> _inputHandlers;

        public MouseEvent()
        {
            ModifierKeyStates = new ModifierKeyState();
        }

        public IInputHandler CurrentHandler
        {
            get
            {
                if (InputHandlers.Count < 1)
                    return DefaultHandler;
                return InputHandlers.Peek();
            }
        }

        public IInputHandler DefaultHandler { get; set; }

        public Stack<IInputHandler> InputHandlers
        {
            get { return _inputHandlers ?? (_inputHandlers = new Stack<IInputHandler>()); }
            set { _inputHandlers = value; }
        }

        public bool IsMouseDown { get; set; }

        public Vector2 LastMousePosition { get; set; }

        public ModifierKeyState ModifierKeyStates { get; set; }

        public int MouseButton { get; set; }

        public Vector2 MouseDownPosition { get; set; }

        public Vector2 MousePosition { get; set; }

        public Vector2 MousePositionDelta { get; set; }

        public Vector2 MouseUpPosition { get; set; }

        public Vector2 ContextScroll { get; set; }

        public bool NoBubble { get; set; }
        public Vector2 MousePositionDeltaSnapped { get; set; }

        public MouseEvent(ModifierKeyState modifierKeyStates)
        {
            ModifierKeyStates = modifierKeyStates;
        }

        public MouseEvent(ModifierKeyState modifierKeyStates, IInputHandler defaultHandler)
        {
            ModifierKeyStates = modifierKeyStates;
            DefaultHandler = defaultHandler;
        }

        public void Begin(IInputHandler handler)
        {
            if (handler != null)
            {
                InputHandlers.Push(handler);
            }
        }

        public void Cancel()
        {
            if (this.InputHandlers.Count > 0)
                this.InputHandlers.Pop();
        }
    }
}