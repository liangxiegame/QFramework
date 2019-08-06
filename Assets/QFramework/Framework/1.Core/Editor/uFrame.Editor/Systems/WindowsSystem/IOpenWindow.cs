using UnityEngine;

namespace QF.GraphDesigner.Unity.WindowsPlugin
{
    public interface IOpenWindow
    {
        void OpenWindow<T>(T viewModel, WindowType type = WindowType.Normal, Vector2? position = null,
            Vector2? size = null, Vector2? minSize = null, Vector2? maxSize = null) where T : GraphItemViewModel;
    }
}