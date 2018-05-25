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
using UnityEngine;
using UnityEngine.Assertions;
//using UnityEngine.PlaymodeTests;
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
            Debug.Log(testTexture);
			testTexture = mResourcesLoader.LoadSync<Texture2D>("TestTexture");
			// do sth
            Debug.Log(testTexture);
        }

		private void OnDestroy()
		{
			mResourcesLoader.ReleaseAllRes();
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
            Debug.Log(testTexture);
		}

		private void OnDestroy()
		{
			mResourcesLoader.ReleaseAllRes();
			mResourcesLoader = null;
		}
	}

	public class ResourcesLoader
	{
		private List<Res> mLoadedAssets = new List<Res>();

		public T LoadSync<T>(string resName) where T : Object
		{
			var retRes = mLoadedAssets.Find(asset => asset.Name == resName);

			if (retRes == null)
			{
				retRes = ResourcesManager.GetRes(resName);
				retRes.Retain();

				mLoadedAssets.Add(retRes);
				// 容错处理
			}

			return retRes.Asset as T;
		}

		public void ReleaseAllRes()
		{
			mLoadedAssets.ForEach(res => res.Release());
			mLoadedAssets.Clear();			
		}
	}

	public class ResourcesManager : QSingleton<ResourcesManager>
	{
		private static List<Res> mLoadedAssets = new List<Res>();

		public static Res GetRes(string resName)
		{
			var retRes = mLoadedAssets.Find(loadedAsset => loadedAsset.Name == resName);

			if (retRes == null)
			{
				retRes = new Res(resName);
				retRes.LoadSync();

				mLoadedAssets.Add(retRes);
			}

			return retRes;
		}

		public static void RemoveRes(Res res)
		{
			mLoadedAssets.Remove(res);
		}
	}

	public class Res : SimpleRC
	{
		public Res(string resName)
		{
			Name = resName;
		}

		public Object Asset { get; protected set; }

		public string Name { get; protected set; }

		public void LoadSync()
		{
			Asset = Resources.Load(Name);
		}

		protected override void OnZeroRef()
		{
			Resources.UnloadAsset(Asset);
			Asset = null;

			// 从管理器上移除自己
			ResourcesManager.RemoveRes(this);
		}

        //[Test]
		public static void TestRes()
		{
			var res = new Res("TestTexture");
			res.LoadSync();
			Assert.AreEqual(res.Asset.name, "TestTexture");
			res.Retain();
			res.Release();
			Assert.IsNull(res.Asset);
		}
	}
}