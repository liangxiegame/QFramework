using UnityEngine;

namespace QFramework.GraphDesigner
{
    public interface IDesignerWindowEvents
    {
        void AfterDrawGraph(Rect diagramRect);

        void BeforeDrawGraph(Rect diagramRect);

        void AfterDrawDesignerWindow(Rect windowRect);

        void DrawComplete();

        void ProcessInput();
    }
}