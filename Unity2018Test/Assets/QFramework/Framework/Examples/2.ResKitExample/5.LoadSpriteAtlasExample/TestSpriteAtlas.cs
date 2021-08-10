/****************************************************************************
 * Copyright (c) 2018.12 liangxie
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
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace QFramework
{
	/// <inheritdoc />
	/// <summary>
	/// 参考:http://www.cnblogs.com/TheChenLin/p/9763710.html
	/// </summary>
	public class TestSpriteAtlas : MonoBehaviour
	{
		[SerializeField] private Image mImage;

		// Use this for initialization
		private IEnumerator Start()
		{
			var loader = ResLoader.Allocate();

			ResMgr.Init();

#if UNITY_2017_1_OR_NEWER
			var spriteAtlas = loader.LoadSync<SpriteAtlas>("spriteatlas");
			var square = spriteAtlas.GetSprite("Square");
			Log.I(spriteAtlas.spriteCount);

			mImage.sprite = square;
#endif

			yield return new WaitForSeconds(5.0f);

			loader.Recycle2Cache();
			loader = null;
		}
	}
}