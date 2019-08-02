using QF.GraphDesigner.Unity.KoinoniaSystem.Data;
using UnityEngine;

namespace QF.GraphDesigner.Unity.KoinoniaSystem
{
    public interface IDrawPackagePage
    {
        void DrawPackagePage(Rect bounds, UFramePackageDescriptor package);
    }
}