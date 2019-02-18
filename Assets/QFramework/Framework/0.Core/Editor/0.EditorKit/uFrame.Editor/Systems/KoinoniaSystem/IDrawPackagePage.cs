using QFramework.GraphDesigner.Unity.KoinoniaSystem.Data;
using UnityEngine;

namespace QFramework.GraphDesigner.Unity.KoinoniaSystem
{
    public interface IDrawPackagePage
    {
        void DrawPackagePage(Rect bounds, UFramePackageDescriptor package);
    }
}