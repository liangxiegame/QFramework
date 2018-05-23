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


using UnityEngine.UI;

namespace QFramework
{
	using UnityEngine;

	public enum UIMarkType
	{
		DefaultUnityElement,
		Element,
		Component
	}
	
	/// <summary>
	/// UI的标记
	/// </summary>
	public class QUIMark : MonoBehaviour, IUIMark
	{
		public UIMarkType MarkType = UIMarkType.DefaultUnityElement;

		public Transform Transform
		{
			get { return transform; }
		}

		public string CustomComponentName;

		public UIMarkType GetUIMarkType()
		{
			return MarkType;
		}

		public virtual string ComponentName
		{
			get
			{
				if (MarkType == UIMarkType.DefaultUnityElement)
				{
					if (null != GetComponent("SkeletonAnimation"))
						return "SkeletonAnimation";
					if (null != GetComponent<ScrollRect>())
						return "ScrollRect";
					if (null != GetComponent<InputField>())
						return "InputField";
					if (null != GetComponent<Text>())
						return "Text";
					if (null != GetComponent<Button>())
						return "Button";
					if (null != GetComponent<RawImage>())
						return "RawImage";
					if (null != GetComponent<Toggle>())
						return "Toggle";
					if (null != GetComponent<Slider>())
						return "Slider";
					if (null != GetComponent<Scrollbar>())
						return "Scrollbar";
					if (null != GetComponent<Image>())
						return "Image";
					if (null != GetComponent<ToggleGroup>())
						return "ToggleGroup";
					if (null != GetComponent<Animator>())
						return "Animator";
					if (null != GetComponent<Canvas>())
						return "Canvas";
					if (null != GetComponent<RectTransform>())
						return "RectTransform";

					return "Transform";
				}

				return CustomComponentName;
			}
		}
	}
}