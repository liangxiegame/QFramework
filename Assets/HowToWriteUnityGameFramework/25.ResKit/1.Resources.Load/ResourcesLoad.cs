/****************************************************************************
 * Copyright (c) 2018 liangxie
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.ResKitLearn
{
	public class ResourcesLoad : MonoBehaviour
	{
		[SerializeField] private RawImage mImage;

		// Use this for initialization
		private IEnumerator Start()
		{
			gameObject.AddComponent<XXXPanel>();
			
			yield return new WaitForSeconds(1.0f);

			var testTexture = Resources.Load<Texture2D>("TestTexture");
			mImage.texture = testTexture;
			
			yield return new WaitForSeconds(1.0f);

			Resources.UnloadAsset(testTexture);
		}
	}

	public class XXXPanel : MonoBehaviour
	{
		ResourcesLoader mResourcesLoader = new ResourcesLoader();
		
		private void Start()
		{
			var testTexture = mResourcesLoader.LoadSync<Texture2D>("TestTexture");
			// do sth
			
			testTexture = mResourcesLoader.LoadSync<Texture2D>("TestTexture");
			// do sth
		}

		private void OnDestroy()
		{
			mResourcesLoader.UnloadAll();
			mResourcesLoader = null;
		}
	}
	
	public class YYYPanel : MonoBehaviour
	{
		ResourcesLoader mResourcesLoader = new ResourcesLoader();
		
		private void Start()
		{
			var testTexture = mResourcesLoader.LoadSync<Texture2D>("TestTexture");
			// do sth
			
			testTexture = mResourcesLoader.LoadSync<Texture2D>("TestTexture");
			// do sth
		}

		private void OnDestroy()
		{
			mResourcesLoader.UnloadAll();
			mResourcesLoader = null;
		}
	}

	public class ResourcesLoader
	{
		private List<Object> mLoadedAssets = new List<Object>();

		public T LoadSync<T>(string resName) where T : Object
		{
			var retRes = mLoadedAssets.Find(asset => asset.name == resName);

			if (retRes == null)
			{
				retRes = ResourcesManager.Instance.GetRes(resName);
				
				mLoadedAssets.Add(retRes);
				// 容错处理
			}

			return retRes as T;
		}

		public void UnloadAll()
		{
			mLoadedAssets.ForEach(Resources.UnloadAsset);
			mLoadedAssets.Clear();
			mLoadedAssets = null;
		}
	}


	public class ResourcesManager : QSingleton<ResourcesManager>
	{
		private static List<Object> mLoadedAssets = new List<Object>();

		public Object GetRes(string resName)
		{
			var retRes = mLoadedAssets.Find(loadedAsset => loadedAsset.name == resName);

			if (retRes == null)
			{
				retRes = Resources.Load(resName);
			}

			return retRes;
		}
	}
}