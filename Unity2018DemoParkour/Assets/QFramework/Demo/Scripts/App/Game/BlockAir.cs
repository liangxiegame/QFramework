/****************************************************************************
 * Copyright (c) 2018.3 liangxie
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

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 空中的Block
	/// </summary>
	public class BlockAir : MonoBehaviour
	{

		public BoxCollider2D  box2d;
		public EdgeCollider2D edge2d;

		void Awake()
		{
			box2d = GetComponent<BoxCollider2D>();
			edge2d = GetComponent<EdgeCollider2D>();
		}

		/// <summary>
		/// 重置
		/// </summary>
		public void ResetBox()
		{
			box2d.enabled = false;
			ResetBlock();
		}

		/// <summary>
		/// 重置Block
		/// </summary>
		public void ResetBlock()
		{
			if (PropModel.Instance.prop_big_on)
			{
				edge2d.offset = new Vector2(0, 3.3f);
			}
			else
			{
				edge2d.offset = new Vector2(0, 2.4f);
			}
		}
	}
}