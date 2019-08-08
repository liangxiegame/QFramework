using UnityEngine;

namespace QF.GraphDesigner
{
    public interface IShowSelectionMenu
    {
        void ShowSelectionMenu(SelectionMenu menu, Vector2? position = null, bool useWindow = false);
    }
}