using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QF.GraphDesigner.Unity.WindowsPlugin
{
 
    public class WindowsSystem : DiagramPlugin, IOpenWindow
    {

        public Vector2 DefaultSize
        {
            get { return new Vector2(400, 400); }
        }

        public void OpenWindow<T>(T viewModel, WindowType type = WindowType.Normal, Vector2? position = null, Vector2? size = null,
            Vector2? minSize = null, Vector2? maxSize = null) where T : GraphItemViewModel
        {
            var window = ScriptableObject.CreateInstance<UnityWindowDrawer>();
            window.ViewModelObject = viewModel;

            var finalSize = size ?? DefaultSize;
            var finalPosition = position ?? new Vector2((Screen.currentResolution.width - finalSize.x) / 2, (Screen.currentResolution.height - finalSize.y) / 2);

            switch (type)
            {
                case WindowType.Normal:
                    window.Show();
                    break;
                case WindowType.Popup:
                    window.ShowPopup();
                    break;
                case WindowType.FocusPopup:
                    window.ShowAsDropDown(new Rect(finalPosition.x, finalPosition.y,1f,1f),finalSize );
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }

            window.minSize = minSize ?? finalSize;
            window.maxSize = maxSize ?? finalSize;

            window.position = new Rect(finalSize.x, finalSize.y, finalPosition.x, finalPosition.y);
            window.Focus();
            window.Repaint();

        }
    }
}
