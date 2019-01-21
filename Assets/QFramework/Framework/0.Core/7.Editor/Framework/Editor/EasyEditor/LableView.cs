using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public sealed class LabelView : GUIView
    {
        public GUIStyle Style  = new GUIStyle();

        public string Text = string.Empty;
        
        public FontStyle FontStyle 
        {
            set { Style.fontStyle = value; }
        }

        public int FontSize
        {
            set { Style.fontSize = value; }
        }

        public Color FontColor
        {
            set { Style.normal.textColor = value; }
        }

        public LabelView(string text, float width, float height)
        {
            Text = text;
            Width = width;
            Height = height;
        }

        public override void OnGUI()
        {            
            if (Visible) EditorGUILayout.LabelField(Text,Style,mLayoutOptions.ToArray());
        }
    }
}