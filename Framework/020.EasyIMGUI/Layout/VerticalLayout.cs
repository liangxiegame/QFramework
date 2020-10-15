using UnityEngine;

namespace QFramework
{
    public interface IVerticalLayout : IMGUILayout
    {
    
    }
    
    public class VerticalLayout : Layout,IVerticalLayout
    {
        public string VerticalStyle { get; set; }

        public VerticalLayout(string verticalStyle = null)
        {
            VerticalStyle = verticalStyle;
        }

        protected override void OnGUIBegin()
        {
            if (string.IsNullOrEmpty(VerticalStyle))
            {
                GUILayout.BeginVertical(LayoutStyles);
            }
            else
            {
                GUILayout.BeginVertical(VerticalStyle, LayoutStyles);
            }
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndVertical();
        }
    }

}