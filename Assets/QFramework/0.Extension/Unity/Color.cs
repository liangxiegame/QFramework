/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using UnityEngine;

	public static class ColorUtil 
	{
		/// <summary>
		/// #C5563CFF -> 197.0f / 255,86.0f / 255,60.0f / 255
		/// </summary>
		/// <param name="htmlString"></param>
		/// <returns></returns>
		public static Color HtmlStringToColor(string htmlString)
		{
			Color retColor;
			bool parseSucceed = ColorUtility.TryParseHtmlString(htmlString, out retColor);

			if (parseSucceed)
			{
				return retColor;
			}

			return Color.black;
		}

		/// <summary>
		/// unity's color always new a color
		/// </summary>
		public static Color White = Color.white;
	}
}