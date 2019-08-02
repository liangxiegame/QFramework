using QFramework.GraphDesigner.Unity.KoinoniaSystem.Classes;
using UnityEngine;

namespace QFramework.GraphDesigner.Unity.KoinoniaSystem
{
    public interface IDrawPackageControlPanel
    {
        void DrawControlPanel(Rect bounds, UFramePackage package);
    }
}