/****************************************************************************
 * Copyright (c) 2017 liangxie
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

namespace QFramework
{
	public static class QUICameraUtil 
	{
		public static Camera UICamera
		{
			get { return UIKit.Root.Camera; }
		}
		
		public static void SetPerspectiveMode()
		{
			UICamera.orthographic = false;
		}

		public static void SetOrthographicMode()
		{
			UICamera.orthographic = true;
		}

		public static Texture2D CaptureCamera(Rect rect)
		{
			var camera = UICamera;
			RenderTexture rt = new RenderTexture(Screen.width,Screen.height,0);
			camera.targetTexture = rt;
			camera.Render();

			RenderTexture.active = rt;

			Texture2D screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
			screenShot.ReadPixels(rect,0,0);
			screenShot.Apply();

			camera.targetTexture = null;
			RenderTexture.active = null;
			rt.Release();
			Object.Destroy(rt);

			return screenShot;
		}
	}
}
