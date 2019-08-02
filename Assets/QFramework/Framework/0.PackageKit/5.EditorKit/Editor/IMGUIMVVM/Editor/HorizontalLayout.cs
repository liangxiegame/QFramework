using RSG.Utils;
using QF;
using UnityEngine;

namespace UnityEditorUI
{
    /// <summary>
    /// Lays widgets out horizontally.
    /// </summary>
    internal class HorizontalLayout : AbstractLayout
    {
        private string mText;
        public HorizontalLayout(ILayout parent,string text = null) :
            base(parent)
        {
            Argument.NotNull(() => parent);
            mText = text;
        }

        public override void OnGUI()
        {
            if (!enabled)
            {
                return;
            }

            if (!string.IsNullOrEmpty(mText))
            {
                GUILayout.BeginHorizontal(mText);
            }
            else
            {
                GUILayout.BeginHorizontal();
            }
            base.OnGUI();
            GUILayout.EndHorizontal();
        }
    }
}
