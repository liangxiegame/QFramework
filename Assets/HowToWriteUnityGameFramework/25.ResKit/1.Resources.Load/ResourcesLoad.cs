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
		private List<Object> mLoadedAssets = new List<Object>();
		
		private void Start()
		{
			var testTexture = Resources.Load<Texture2D>("TestTexture");
			// do sth

			mLoadedAssets.Add(testTexture);
			
			testTexture = Resources.Load<Texture2D>("TestTexture");
			// do sth

			mLoadedAssets.Add(testTexture);
		}

		private void OnDestroy()
		{
			mLoadedAssets.ForEach(Resources.UnloadAsset);
			mLoadedAssets.Clear();
			mLoadedAssets = null;
		}
	}
}