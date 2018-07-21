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

namespace QFramework.Example
{
	using System.Collections;
	using UnityEngine.UI;
	using UnityEngine;
	
	public class ImageLoaderExample : MonoBehaviour
	{

		ResLoader mResLoader = ResLoader.Allocate();

		// Use this for initialization
		private IEnumerator Start()
		{
			ResMgr.Init();

			// net image
			mResLoader.Add2Load(
				"http://qframework.io/content/images/2017/07/-----2017-07-01-12-14-56.png".ToNetImageResName(),
				delegate(bool b, IRes res)
				{
					Log.E(b);
					if (b)
					{
						var texture2D = res.Asset as Texture2D;
						transform.Find("Image").GetComponent<Image>().sprite = Sprite.Create(texture2D,
							new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one * 0.5f);
					}
				});

			mResLoader.LoadAsync();

			yield return new WaitForSeconds(5.0f);

			mResLoader.Recycle2Cache();
			mResLoader = null;


			yield return new WaitForSeconds(1.0f);

			mResLoader = ResLoader.Allocate();

			// local image
			string localImageUrl = "file://" + Application.persistentDataPath + "/Workspaces/lM1wmsLQtfzRQc6fsdEU.jpg";

			mResLoader.Add2Load(localImageUrl.ToLocalImageResName(),
				delegate(bool b, IRes res)
				{
					Log.E(b);
					if (b)
					{
						var texture2D = res.Asset as Texture2D;
						transform.Find("Image").GetComponent<Image>().sprite = Sprite.Create(texture2D,
							new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one * 0.5f);
					}
				});
			mResLoader.LoadAsync();
			
			
			yield return new WaitForSeconds(5.0f);
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}