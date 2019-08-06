/****************************************************************************
 * Copyright (c) 2017 ~ 2018.5 liangxie
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

namespace QFramework
{
    using UnityEngine;
    
	public static class UIRectTransformExtension
    {
        public static Vector2 GetLocalPosInRect(this RectTransform selfRectTrans, Camera camera = null)
        {
            Vector2 retLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRectTrans, Input.mousePosition,
                camera, out retLocalPos);
            return retLocalPos;
        }

        public static bool InRect(this RectTransform selfRectTrans, Camera camera = null)
        {
            if (null == camera)
                camera = UIManager.Instance.RootCanvas.worldCamera;
 
            return RectTransformUtility.RectangleContainsScreenPoint(selfRectTrans, Input.mousePosition, camera);
        }
 
        public static Vector2 ToScreenPoint(this RectTransform selfRectTrans)
        {
            return RectTransformUtility.WorldToScreenPoint(UIManager.Instance.RootCanvas.worldCamera, selfRectTrans.position);
        }
 
        public static Vector2 ToScreenPoint(this RectTransform selfRectTrans, Vector2 worldPos)
        {
            return RectTransformUtility.WorldToScreenPoint(UIManager.Instance.RootCanvas.worldCamera, worldPos);
        }
 
        public static bool InRootTransRect(this RectTransform selfRectTrans, RectTransform rootTrans, Camera camera = null)
        {
            if (null == camera)
                camera = UIManager.Instance.RootCanvas.worldCamera;
            return RectTransformUtility.RectangleContainsScreenPoint(rootTrans, selfRectTrans.ToScreenPoint(), camera);
        }
 
        public static Vector2 ConvertWorldPosToLocalPosInSelf(this RectTransform selfRectTrans, Vector2 worldPos)
        {
            var screenPos = RectTransformUtility.WorldToScreenPoint(QUICameraUtil.UICamera, worldPos);
            Vector2 retLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRectTrans, screenPos, QUICameraUtil.UICamera,
                out retLocalPos);
            return retLocalPos;
        }
	}
}