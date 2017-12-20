/****************************************************************************
 * Copyright (c) 2017 liangxie
 ****************************************************************************/

namespace QFramework
{
    using UnityEngine;

    public class GUIHorizontalView : EditorView
    {
        public override void OnGUI()
        {
            if (Visible) GUILayout.BeginHorizontal();
            base.OnGUI();
            if (Visible) GUILayout.EndHorizontal();
        }

        #region 重构工具

        

        #endregion
    }
}