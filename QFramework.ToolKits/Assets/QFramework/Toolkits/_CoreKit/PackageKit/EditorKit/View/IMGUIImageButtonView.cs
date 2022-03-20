/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public class ImageButtonView : IMGUIAbstractView
    {
        private Texture2D mTexture2D { get; set; }

        private Action mOnClick { get; set; }

        public ImageButtonView(string texturePath, Action onClick)
        {
            mTexture2D = Resources.Load<Texture2D>(texturePath);
            mOnClick = onClick;

            //Style = new GUIStyle(GUI.skin.button);
        }

        protected override void OnGUI()
        {
            if (GUILayout.Button(mTexture2D, LayoutStyles))
            {
                mOnClick.Invoke();
            }
        }
    }
}