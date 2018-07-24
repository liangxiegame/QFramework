/****************************************************************************
 * Copyright (c) 2017 liangxie
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

namespace QFramework
{
	using UnityEngine.Events;

	public static class UnityActionExtension
	{
		public static void Example()
		{
			UnityAction action = () => { };
			UnityAction<int> actionWithInt = num => { };
			UnityAction<int, string> actionWithIntString = (num, str) => { };

			action.InvokeGracefully();
			actionWithInt.InvokeGracefully(1);
			actionWithIntString.InvokeGracefully(1, "str");		
		}
		
		/// <summary>
		/// Call action
		/// </summary>
		/// <param name="selfAction"></param>
		/// <returns> call succeed</returns>
		public static bool InvokeGracefully(this UnityAction selfAction)
		{
			if (null != selfAction)
			{
				selfAction();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Call action
		/// </summary>
		/// <param name="selfAction"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool InvokeGracefully<T>(this UnityAction<T> selfAction, T t)
		{
			if (null != selfAction)
			{
				selfAction(t);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Call action
		/// </summary>
		/// <param name="selfAction"></param>
		/// <returns> call succeed</returns>
		public static bool InvokeGracefully<T, K>(this UnityAction<T, K> selfAction, T t, K k)
		{
			if (null != selfAction)
			{
				selfAction(t, k);
				return true;
			}
			return false;
		}
	}
}