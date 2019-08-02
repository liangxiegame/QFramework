using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class ProblemWindow : EditorWindow
    {
        private Vector2 _scrollPos;

        void OnGUI()
        {
            if (this.Problem == null)
            {
                this.Close();
                return;
            }

            var bounds = new Rect().WithSize(this.position.width, this.position.height).PadSides(5);
            InvertApplication.SignalEvent<IDrawProblem>(_=>_.DrawProblemInspector(bounds,Problem));
        
        }

        public Problem Problem { get; set; }
    }
}