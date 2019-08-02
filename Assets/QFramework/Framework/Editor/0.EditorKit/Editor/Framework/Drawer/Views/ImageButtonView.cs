using System;
using UnityEngine;

namespace EGO.Framework
{
    public class ImageButtonView : View
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
            if (GUILayout.Button(mTexture2D,LayoutStyles))
            {
                mOnClick.Invoke();
            }           
        }
    }
}