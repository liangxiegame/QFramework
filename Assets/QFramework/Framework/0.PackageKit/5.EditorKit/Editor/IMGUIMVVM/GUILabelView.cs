using UnityEngine;

namespace QF
{
    public class GUILabelView :GUIView
    {
        public string Text = string.Empty;

        public GUILabelView(string text)
        {
            Text = text;
        }

        public override void OnGUI()
        {            
            if (Visible) GUILayout.Label(Text);
        }


        #region 协助重构的工具

        private static GUILabelView mS;

        public static GUILabelView S(string text)
        {
            mS = mS ?? (mS = new GUILabelView(text));
            mS.Text = text;
            return mS;
        }

        #endregion
    }
}