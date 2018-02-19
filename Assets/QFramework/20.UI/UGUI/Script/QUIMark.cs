/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

using UnityEngine.UI;

namespace QFramework
{
	using UnityEngine;

	/// <summary>
	/// UI的标记
	/// </summary>
	public class QUIMark : MonoBehaviour, IUIMark
	{
		public Transform Transform
		{
			get { return transform; }
		}

		public virtual string ComponentName
		{
			get
			{
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
				if (null != GetComponent<RectTransform>())
					return "RectTransform";
				return "Transform";
			}
		}
	}
}