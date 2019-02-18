using QFramework.GraphDesigner;
using UnityEngine;

namespace QFramework.GraphDesigner.Unity
{
    internal interface IDrawProblem
    {
        void DrawProblemInspector(Rect bounds, Problem problem);
    }
}