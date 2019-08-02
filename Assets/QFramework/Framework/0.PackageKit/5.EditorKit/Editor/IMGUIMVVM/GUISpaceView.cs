using UnityEngine;

namespace QF
{
    public class GUISpaceView :GUIView
    {
        private int mSpacePixels;

        public GUISpaceView(int spacePixels)
        {
            mSpacePixels = spacePixels;
        }
        
        public override void OnGUI()
        {
            if (Visible) return;
            base.OnGUI();
            GUILayout.Space(mSpacePixels);   
        }


        #region 协助重构的工具

        private static GUISpaceView mS;

        public static GUISpaceView S(int spacePixels)
        {
            mS = mS ?? (mS = new GUISpaceView(spacePixels));
            mS.mSpacePixels = spacePixels;
            return mS;
        }

        #endregion
    }
}