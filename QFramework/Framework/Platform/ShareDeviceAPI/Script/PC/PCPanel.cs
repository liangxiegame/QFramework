/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

namespace QFramework
{
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UnityEngine.UI;
	using System;
	using System.Collections;
	
	/// <summary>
	/// 其实是FlexiSocket的Client端
	/// </summary>
	public class PCPanel : MonoBehaviour
	{
		private InputField mAddressInputField;
		private Button mBtnConnect;

		private IEnumerator Start()
		{
			mAddressInputField = transform.Find("AddressInputField").GetComponent<InputField>();
			mBtnConnect = transform.Find("BtnConnect").GetComponent<Button>();

			bool connected = false;
			
			mBtnConnect.OnClick(delegate
			{
				PCClient.Instance.ConnectToMobile(mAddressInputField.text, delegate(bool success, Exception exception)
				{
					if (success)
					{
						Log.i("connected");
						// 连接成功后进入指定场景
						connected = true;
					}
					else
					{
						Log.i(exception.ToString());
					}
				});
			});

			while (!connected)
			{
				yield return 0;
			}

			SceneManager.LoadScene(2);
		}
	}
}