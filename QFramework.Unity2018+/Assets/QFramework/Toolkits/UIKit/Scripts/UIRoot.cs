/****************************************************************************
 * Copyright (c) 2017 ~ 2020.1 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    [MonoSingletonPath("UIRoot")]
    public class UIRoot : MonoBehaviour,ISingleton
    {
        public Camera UICamera;
        public Canvas Canvas;
        public CanvasScaler CanvasScaler;
        public GraphicRaycaster GraphicRaycaster;

        public RectTransform Bg;
        public RectTransform Common;
        public RectTransform PopUI;
        public RectTransform CanvasPanel;
        
        private static UIRoot mInstance;

        public static UIRoot Instance
        {
            get
            {
                if (!mInstance)
                {
                    mInstance = FindObjectOfType<UIRoot>();
                }

                if (!mInstance)
                {
                    Instantiate(Resources.Load<GameObject>("UIRoot"));
                    mInstance = MonoSingletonProperty<UIRoot>.Instance;
                    mInstance.name = "UIRoot";
                    DontDestroyOnLoad(mInstance);
                }

                return mInstance;
            }
        }


        public Camera Camera
        {
            get { return UICamera; }
        }

        public void SetResolution(int width, int height, float matchOnWidthOrHeight)
        {
            CanvasScaler.referenceResolution = new Vector2(width, height);
            CanvasScaler.matchWidthOrHeight = matchOnWidthOrHeight;
        }

        public Vector2 GetResolution()
        {
            return CanvasScaler.referenceResolution;
        }

        public float GetMatchOrWidthOrHeight()
        {
            return CanvasScaler.matchWidthOrHeight;
        }

        public void ScreenSpaceOverlayRenderMode()
        {
            Canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;
            UICamera.gameObject.SetActive(false);
        }

        public void ScreenSpaceCameraRenderMode()
        {
            Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            UICamera.gameObject.SetActive(true);
            Canvas.worldCamera = UICamera;
        }


        public void SetLevelOfPanel(UILevel level, IPanel panel)
        {

            var canvas = panel.Transform.GetComponent<Canvas>();

            if (canvas)
            {
                panel.Transform.SetParent(CanvasPanel);
            }
            else
            {
                switch (level)
                {
                    case UILevel.Bg:
                        panel.Transform.SetParent(Bg);
                        break;
                    case UILevel.Common:
                        panel.Transform.SetParent(Common);
                        break;
                    case UILevel.PopUI:
                        panel.Transform.SetParent(PopUI);
                        break;
                }
            }

            if (panel.Info != null && panel.Info.Level != level)
            {
                panel.Info.Level = level;
            }
        }

        public void OnSingletonInit()
        {
            
        }
    }
}