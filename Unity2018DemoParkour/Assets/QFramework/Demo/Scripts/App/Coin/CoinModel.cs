/****************************************************************************
 * Copyright (c) 2018.7 liangxie
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

using System.Collections.Generic;
using QF;

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 金币模型,定义金币的价格
	/// </summary>
	public class CoinModel : Singleton<CoinModel>
	{

		private CoinModel()
		{
		}

		public class CoinDataDefine
		{
			public int rmb;
			public int coins;

			public CoinDataDefine(int coins, int rmb)
			{
				this.coins = coins;
				this.rmb = rmb;
			}
		}

		public Dictionary<int, CoinDataDefine> CoinDefines = new Dictionary<int, CoinDataDefine>()
		{
			{1, new CoinDataDefine(400, 6)},
			{2, new CoinDataDefine(900, 11)},
			{3, new CoinDataDefine(1800, 20)},
			{4, new CoinDataDefine(3000, 32)},
		};
	}
}