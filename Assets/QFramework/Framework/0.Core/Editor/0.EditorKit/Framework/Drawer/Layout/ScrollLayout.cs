using UnityEngine;

namespace EGO.Framework
{
    public class ScrollLayout : Layout
    {
        Vector2 mScrollPos = Vector2.zero;

        protected override void OnGUIBegin()
        {
            mScrollPos =  GUILayout.BeginScrollView(mScrollPos,LayoutStyles);
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndScrollView();
        }
    }
}