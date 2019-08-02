using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using QF.GraphDesigner;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class InputSystem : DiagramPlugin, IDesignerWindowEvents
    {
        public void AfterDrawGraph(Rect diagramRect)
        {
            //var rect = new Rect(diagramRect);
            //InvertGraphEditor.PlatformDrawer.DrawLabel(rect, string.Format("KEYS: {0}", _fpsShown), CachedStyles.WizardSubBoxTitleStyle);
        }

        public void BeforeDrawGraph(Rect diagramRect)
        {
            
        }

        public void AfterDrawDesignerWindow(Rect windowRect)
        {
            
        }

        public void DrawComplete()
        {
            
        }
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public bool Shift { get; set; }

        public void ProcessInput()
        {
            var e = Event.current;

            if (!e.isKey) return;
            if (e.type == EventType.KeyUp)
            {
                Signal<IKeyUp>(_ =>
                {
                    if (_.KeyUp(Control, Alt, Shift, e.keyCode))
                    {
                        e.Use();
                    }
                }
               );
            }

            if (e.keyCode == KeyCode.LeftControl || e.keyCode == KeyCode.LeftCommand ||
                       e.keyCode == KeyCode.RightControl || e.keyCode == KeyCode.RightCommand)
            {
                Control = e.type == EventType.KeyDown;
            }
            if ( e.keyCode == KeyCode.LeftAlt || e.keyCode == KeyCode.RightAlt)
            {
                Alt = e.type == EventType.KeyDown;
            }
            if (e.keyCode == KeyCode.LeftShift || e.keyCode == KeyCode.RightShift)
            {
                Shift = e.type == EventType.KeyDown;
            }
            if (e.type == EventType.KeyDown)
            {
                Signal<IKeyDown>(_ =>
                {
                    if (_.KeyDown(Control, Alt, Shift, e.keyCode))
                    {
                        e.Use();
                    }
                }
                );
            }
        }
    }

}
