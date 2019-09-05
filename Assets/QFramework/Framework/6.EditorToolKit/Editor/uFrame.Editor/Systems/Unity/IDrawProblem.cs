using QF.GraphDesigner;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    internal interface IDrawProblem
    {
        void DrawProblemInspector(Rect bounds, Problem problem);
    }
}