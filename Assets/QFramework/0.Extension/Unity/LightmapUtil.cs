/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using UnityEngine;

	public static class LightmapUtil 
	{
		public static void SetAmbientLightHTMLStringColor(string htmlStringColor)
		{
			RenderSettings.ambientLight = ColorUtil.HtmlStringToColor(htmlStringColor);
		}
	}
}