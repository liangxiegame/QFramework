using UnityEngine;

namespace EGO.Framework
{
    public class HorizontalLayout : Layout
    {
        public string HorizontalStyle { get; set; }

        public HorizontalLayout(string horizontalStyle =null)
        {
            HorizontalStyle = horizontalStyle;
        }
        protected override void OnGUIBegin()
        {
            if (string.IsNullOrEmpty(HorizontalStyle))
            {
                GUILayout.BeginHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal(HorizontalStyle);
            }
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndHorizontal();
        }
    }
}