/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
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

namespace QFramework
{
	/// <summary>
	/// 教程地址:http://liangxiegame.com/post/4/
	/// </summary>
	public class SimpleFSMExample : MonoBehaviour
	{
		/// <summary>
		/// 管理玩家的状态机
		/// </summary>
		private QFSMLite mPlayerFsm;

		/// <summary>
		/// 闲置状态
		/// </summary>
		private const string STATE_IDLE = "idle";

		/// <summary>
		/// 跑的状态
		/// </summary>
		private const string STATE_RUN = "run";

		/// <summary>
		/// 一段跳的状态
		/// </summary>
		private const string STATE_JUMP = "jump";

		/// <summary>
		/// 二段跳
		/// </summary>
		private const string STATE_DOUBLE_JUMP = "double_jump";

		/// <summary>
		/// 挂彩
		/// </summary>
		private const string STATE_DIE = "die";

		/// <summary>
		/// 用户点击屏幕事件
		/// </summary>
		private const string EVENT_TOUCH_DOWN = "touch_down";


		/// <summary>
		/// 玩家从空中着陆(一般是碰撞器触发)
		/// </summary>
		private const string EVENT_LAND = "land";

		private void Start()
		{
			mPlayerFsm = new QFSMLite();

			// 添加状态
			mPlayerFsm.AddState(STATE_DIE);
			mPlayerFsm.AddState(STATE_RUN);
			mPlayerFsm.AddState(STATE_JUMP);
			mPlayerFsm.AddState(STATE_DOUBLE_JUMP);
			mPlayerFsm.AddState(STATE_DIE);

			// 添加跳转
			mPlayerFsm.AddTranslation(STATE_RUN, EVENT_TOUCH_DOWN, STATE_JUMP, JumpThePlayer);
			mPlayerFsm.AddTranslation(STATE_JUMP, EVENT_TOUCH_DOWN, STATE_DOUBLE_JUMP, DoubleJumpThePlayer);
			mPlayerFsm.AddTranslation(STATE_JUMP, EVENT_LAND, STATE_RUN, RunThePlayer);
			mPlayerFsm.AddTranslation(STATE_DOUBLE_JUMP, EVENT_LAND, STATE_RUN, RunThePlayer);

			// 启动状态机
			mPlayerFsm.Start(STATE_RUN);
		}

		private void OnGUI()
		{
			if (GUI.Button(new Rect(0, 0, 200, 100), "用户:输入跳跃"))
			{
				mPlayerFsm.HandleEvent(EVENT_TOUCH_DOWN);
			}
						
			if (GUI.Button(new Rect(0, 100, 200, 100), "碰撞器:输入着陆"))
			{
				mPlayerFsm.HandleEvent(EVENT_LAND);
			}
			
		}

		void JumpThePlayer(object[] args)
		{
			Log.I("让Player跳跃");
		}

		void DoubleJumpThePlayer(object[] args)
		{
			Log.I("让Player二段跳");
		}

		void RunThePlayer(object[] args)
		{
			Log.I("让Player跑");
		}
	}
}