using UnityEngine;

namespace QF.GraphDesigner
{
    public interface IInspectorPropertyDrawer
    {
        void Refresh(IPlatformDrawer platform, Vector2 position, PropertyFieldDrawer viewModel);
        void Draw(IPlatformDrawer platform, float scale, PropertyFieldDrawer viewModel);
    }
}