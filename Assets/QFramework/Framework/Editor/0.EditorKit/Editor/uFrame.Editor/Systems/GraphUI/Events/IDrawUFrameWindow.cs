using UnityEngine;

namespace QFramework.GraphDesigner
{
    public interface IDrawUFrameWindow
    {
        void Draw(float width, float height, Vector2 scrollPosition, float scale);
    }
}