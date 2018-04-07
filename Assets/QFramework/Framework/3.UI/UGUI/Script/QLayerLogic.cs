/****************************************************************************
 * Copyright (c) 2018 vin129
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

namespace QFramework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class QLayerLogic : MonoBehaviour
    {
        [SerializeField]Transform mBgTrans;
        [SerializeField]Transform mAnimationUnderPageTrans;
        [SerializeField]Transform mCommonTrans;
        [SerializeField]Transform mAnimationOnPageTrans;
        [SerializeField]Transform mPopUITrans;
        [SerializeField]Transform mConstTrans;
        [SerializeField]Transform mToastTrans;
        [SerializeField]Transform mForwardTrans;


        public void SetLayer(int uiLevel, IUIBehaviour ui) {
            switch (uiLevel)
            {
                case UILevel.Bg:
                    ui.Transform.SetParent(mBgTrans);
                    break;
                case UILevel.AnimationUnderPage:
                    ui.Transform.SetParent(mAnimationUnderPageTrans);
                    break;
                case UILevel.Common:
                    ui.Transform.SetParent(mCommonTrans);
                    break;
                case UILevel.AnimationOnPage:
                    ui.Transform.SetParent(mAnimationOnPageTrans);
                    break;
                case UILevel.PopUI:
                    ui.Transform.SetParent(mPopUITrans);
                    break;
                case UILevel.Const:
                    ui.Transform.SetParent(mConstTrans);
                    break;
                case UILevel.Toast:
                    ui.Transform.SetParent(mToastTrans);
                    break;
                case UILevel.Forward:
                    ui.Transform.SetParent(mForwardTrans);
                    break;
            }
            var uiGoRectTrans = ui.Transform as RectTransform;

            uiGoRectTrans.offsetMin = Vector2.zero;
            uiGoRectTrans.offsetMax = Vector2.zero;
            uiGoRectTrans.anchoredPosition3D = Vector3.zero;
            uiGoRectTrans.anchorMin = Vector2.zero;
            uiGoRectTrans.anchorMax = Vector2.one;

            ui.Transform.LocalScaleIdentity();
        }
    }
}
