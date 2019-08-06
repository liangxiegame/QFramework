using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class ElementsDesigner : EditorWindow
    {
        public static ElementsDesigner Instance { get; set; }

        [MenuItem("Window/uFrame/Graph Window #&u", false, 1)]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (ElementsDesigner) GetWindow(typeof(ElementsDesigner));
            window.title = "uFrame";
            window.wantsMouseMove = true;
            window.Show();
            window.Repaint();
            Instance = window;
        }

        public void InfoBox(string message, MessageType type = MessageType.Info)
        {
            EditorGUI.HelpBox(new Rect(15, 30, 300, 30), message, type);
        }

        public bool IsFocused { get; set; }

        public void OnFocus()
        {
            IsFocused = true;
        }

        public void OnGUI()
        {
            if (InvertGraphEditor.Container != null)
            {
                InvertApplication.SignalEvent<IDrawDesignerWindow>(_ =>
                    _.DrawDesigner(position.width, position.height));
            }
        }

        public void OnLostFocus()
        {
            InvertApplication.SignalEvent<IDesignerWindowLostFocus>(_ => _.DesignerWindowLostFocus());

            IsFocused = false;
        }

        public void Update()
        {
            Instance = this;
            InvertApplication.SignalEvent<IUpdate>(_ => _.Update());
            Repaint();
        }
    }
}