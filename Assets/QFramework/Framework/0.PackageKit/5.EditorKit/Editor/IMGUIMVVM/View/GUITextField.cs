
namespace QF
{
    using UnityEngine;

    /// <summary>
    /// 这个输入有些问题了
    /// </summary>
    public class GUITextField : GUIView
    {
        public string Text { get; set; }

        public Rect Rect { get; set; }

        public GUITextField(string text, Rect rect)
        {
            Text = text;
            Rect = rect;
        }

        public override void OnGUI()
        {
            if (Visible) Text = GUI.TextField(Rect, Text, (int) Rect.width);
        }
    }
}