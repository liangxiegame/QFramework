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
using System.Collections;

namespace QFramework.PlatformRunner
{
	public class Coins : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}

		public Vector2[]   coinList;
		public Transform[] childs;
		public int         count;

		private void Awake()
		{
			count = transform.childCount;

			coinList = new Vector2[count];
			childs = new Transform[count];
		}

		private void Start()
		{
			StartCoroutine(ParsePos());
		}

		private IEnumerator ParsePos()
		{
			for (var i = 0; i < count; i++)
			{
				coinList[i] = transform.GetChild(i).localPosition;
				childs[i] = transform.GetChild(i);

				yield return new WaitForSeconds(0.01f);
			}

			yield return 0;
		}

		/// <summary>
		/// 重置金币的位置
		/// </summary>
		public void ResetPos()
		{
			if (this.gameObject.activeInHierarchy)
			{
				StartCoroutine(ResetCoin());
			}
		}

		/// <summary>
		/// 重置金币位置
		/// </summary>
		private IEnumerator ResetCoin()
		{
			for (var i = 0; i < count; i++)
			{
				if (childs[i] != null)
				{
					childs[i].gameObject.SetActive(true);
					childs[i].GetComponent<MagnetiteEffect>().enabled = false;
					childs[i].localPosition = coinList[i];

					yield return new WaitForSeconds(0.02f);
				}
			}

			yield return 0;
		}

		/// <summary>
		/// 吸铁石
		/// </summary>
		public void MagnetiteOn()
		{
			for (var i = 0; i < count; i++)
			{
				childs[i].GetComponent<MagnetiteEffect>().enabled = true;
			}
		}
	}
}