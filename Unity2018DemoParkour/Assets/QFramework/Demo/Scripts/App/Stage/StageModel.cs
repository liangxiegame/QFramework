/****************************************************************************
 * Copyright (c) 2018.3 ~ 7 liangxie
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

using QF;
using UnityEngine;

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 关卡数据生成器
	/// </summary>
	public class StageModel : Singleton<StageModel>
	{
		private StageModel()
		{
			InitModel();
			ResetModel();
		}

		public int Theme
		{
			get { return PlatformRunner.Interface.GetModel<IGameModel>().CurTheme.Value; }
		}

		public bool Switching = false; // 是否正在转换


		public float ThemeLength
		{
			get
			{
				switch (Theme)
				{
					case 1:
						return 27.0f;
					case 2:
						return 28.0f;
					case 3:
						return 28.0f;
					case 4:
						return 28.0f;
				}

				return 0;
			}
		}

		/// <summary>
		/// 生成关卡数据返回给StageCtrl 
		/// </summary>
		public StageData GetStageData()
		{
			switch (Theme)
			{
				case 1:
					return Stage1DataGenerator.Instance.GetStageData();
				case 2:
					return Stage2DataGenerator.Instance.GetStageData();
				case 3:
					return Stage3DataGenerator.Instance.GetStageData();
				case 4:
					return Stage4DataGenerator.Instance.GetStageData();
				default:
					break;
			}

			return null;
		}

		/// <summary>
		/// 获取名字
		/// </summary>
		/// <returns>The stage name.</returns>
		public string GetStageName()
		{
			// 主题区分
			switch (Theme)
			{
				case 1:
					return Stage1DataGenerator.Instance.GetStageName();
				case 2:
					return Stage2DataGenerator.Instance.GetStageName();
				case 3:
					return Stage3DataGenerator.Instance.GetStageName();
				case 4:
					return Stage4DataGenerator.Instance.GetStageName();
				default:
					break;
			}

			return null;
		}

		/// <summary>
		/// 解析位置数据
		/// </summary>
		/// <param name="trans">Trans.</param>
		public void ParseStagePattern(Transform trans)
		{
			switch (Theme)
			{
				case 1:
					Stage1DataGenerator.Instance.ParseStagePattern(trans);
					break;
				case 2:
					Stage2DataGenerator.Instance.ParseStagePattern(trans);
					break;
				case 3:
					Stage3DataGenerator.Instance.ParseStagePattern(trans);
					break;
				case 4:
					Stage4DataGenerator.Instance.ParseStagePattern(trans);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// 初始化StageModel
		/// </summary>
		public void InitModel()
		{

		}

		/// <summary>
		/// 重置模型
		/// </summary>
		public void ResetModel()
		{

		}
	}
}