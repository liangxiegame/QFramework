using QF.GraphDesigner.Unity.KoinoniaSystem.Classes;
using UnityEngine;

namespace QF.GraphDesigner.Unity.KoinoniaSystem
{
    public interface IDrawPackageControlPanel
    {
        void DrawControlPanel(Rect bounds, UFramePackage package);
    }
}