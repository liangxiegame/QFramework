using UnityEngine;

namespace QFramework
{
    public interface ILabel : IMGUIView,IHasText<ILabel>
    {
    
    }
    
    internal class Label : View,ILabel
    {
        public string Content { get; set; }

        public Label()
        {
            mStyleProperty = new GUIStyleProperty(() => new GUIStyle(GUI.skin.label));
        }

        protected override void OnGUI()
        {
            GUILayout.Label(Content, Style.Value, LayoutStyles);
        }

        public ILabel Text(string labelText)
        {
            Content = labelText;
            return this;
        }
    }
}