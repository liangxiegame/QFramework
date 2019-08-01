using UnityEditor;
using UnityEngine;

namespace EGO.Framework
{
    public class ColorView : View
    {
        public ColorView(Color color)
        {
            Color = new Property<Color>(color);            
        }

        public Property<Color> Color { get; }

        protected override void OnGUI()
        {
            Color.Value = EditorGUILayout.ColorField(Color.Value,LayoutStyles);
        }
    }
}