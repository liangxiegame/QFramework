using UnityEditor;
using UnityEngine;

namespace EGO.Framework
{
    public class PopupView : View
    {
        public Property<int> IndexProperty { get; }

        public string[] MenuArray { get; }

        public PopupView(int initValue, string[] menuArray)
        {
            MenuArray = menuArray;
            IndexProperty = new Property<int>(initValue);
            IndexProperty.Value = initValue;

            Style = new GUIStyle(EditorStyles.popup);
        }

        protected override void OnGUI()
        {
            IndexProperty.Value = EditorGUILayout.Popup(IndexProperty.Value, MenuArray, LayoutStyles);
        }
    }
}