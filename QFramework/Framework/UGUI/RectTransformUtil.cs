/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
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
	using UnityEngine;

	public static class RectTransformUtil
	{
        public static UnityEngine.Vector2 GetLocalPosInRect(this RectTransform selfRectTrans,Camera camera)
		{
			UnityEngine.Vector2 retLocalPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRectTrans,Input.mousePosition,
                                                                    camera, out retLocalPos);
			return retLocalPos;
		}

        public static UnityEngine.Vector2 ConvertWorldPosToLocalPosInSelf(this RectTransform selfRectTrans, UnityEngine.Vector2 worldPos,Camera camera)
		{
            var screenPos = RectTransformUtility.WorldToScreenPoint(camera, worldPos);
			UnityEngine.Vector2 retLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRectTrans, screenPos, camera,
				out retLocalPos);
			return retLocalPos;
		}

        public static bool InRect(this RectTransform selfRectTrans,Camera camera)
		{			
			return RectTransformUtility.RectangleContainsScreenPoint(selfRectTrans, Input.mousePosition,camera);
		}
		
		public static UnityEngine.Vector2 GetWorldposInRect(this RectTransform selfRectTrans)
		{
			UnityEngine.Vector2 retWorldPos = UnityEngine.Vector2.down;
			return retWorldPos;
		}

		public static bool InRootTransRect(this RectTransform selfRectTrans, RectTransform rootTrans,Camera camera = null)
		{
			//if (null == camera)
				//camera = QUIManager.Instance.RootCanvas.worldCamera;
			return RectTransformUtility.RectangleContainsScreenPoint(rootTrans, selfRectTrans.ToScreenPoint(QUIManager.Instance.RootCanvas.worldCamera), camera);
		}

		public static UnityEngine.Vector2 GetPosInRootTrans(this RectTransform selfRectTransform, Transform rootTrans)
		{
			return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, selfRectTransform).center;
		}
		
        public static UnityEngine.Vector2 ToScreenPoint(this RectTransform selfRectTrans,Camera camera)
		{
            return RectTransformUtility.WorldToScreenPoint(camera, selfRectTrans.position);
		}
		
        public static UnityEngine.Vector2 ToScreenPoint(this RectTransform selfRectTrans,UnityEngine.Vector2 worldPos,Camera camera)
		{
            return RectTransformUtility.WorldToScreenPoint(camera, worldPos);
		}

		public static void SetAnchorPosX(this RectTransform selfRectTrans, float anchorPosX)
		{
			var anchorPos = selfRectTrans.anchoredPosition;
			anchorPos.x = anchorPosX;
			selfRectTrans.anchoredPosition = anchorPos;
		}
		
		public static void SetSizeWidth(this RectTransform selfRectTrans, float sizeWidth)
		{
			var sizeDelta = selfRectTrans.sizeDelta;
			sizeDelta.x = sizeWidth;
			selfRectTrans.sizeDelta = sizeDelta;
		}

		public static void SetSizeHeight(this RectTransform selfRectTrans, float sizeHeight)
		{
			var sizeDelta = selfRectTrans.sizeDelta;
			sizeDelta.y = sizeHeight;
			selfRectTrans.sizeDelta = sizeDelta;
		}
	}
}