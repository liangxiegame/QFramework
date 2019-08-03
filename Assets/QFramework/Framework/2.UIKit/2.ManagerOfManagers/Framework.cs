/****************************************************************************
 * Copyright (c) 2017 ~ 2018.5 liangxie
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
using QF.Extensions;

namespace QFramework
{
	using UnityEngine.Events;
	
	/// <inheritdoc />
	/// <summary>
	/// 全局唯一继承于MonoBehaviour的单例类，保证其他公共模块都以App的生命周期为准
	/// 这个东西很基类，没什么用。概念也不太清晰
	/// </summary>
	[QMonoSingletonPath("[Framework]/QFramework")]
	public class Framework : QMgrBehaviour, ISingleton
	{
		/// <summary>
		/// 组合的方式实现单例的模板
		/// </summary>
		/// <value>The instance.</value>
        public static Framework Instance
		{
            get { return MonoSingletonProperty<Framework>.Instance; }
		}
		
		public override int ManagerId
		{
			get { return QMgrID.Framework; }
		}

		public void OnSingletonInit()
		{
		}

		public void Dispose()
		{
		}

        private Framework()
		{
		}

		#region 全局生命周期回调

		public UnityAction OnUpdateEvent = delegate { };
		public UnityAction OnFixedUpdateEvent = delegate { };
		public UnityAction OnLateUpdateEvent = delegate { };
		public UnityAction OnGUIEvent = delegate { };
		public UnityAction OnDestroyEvent = delegate { };
		public UnityAction OnApplicationQuitEvent = delegate { };

		private void Update()
		{
			OnUpdateEvent.InvokeGracefully();
		}

		private void FixedUpdate()
		{
			OnFixedUpdateEvent.InvokeGracefully();
		}

		private void LateUpdate()
		{
			OnLateUpdateEvent.InvokeGracefully();
		}

		private void OnGUI()
		{
			OnGUIEvent.InvokeGracefully();
		}

		protected override void OnDestroy()
		{
			OnDestroyEvent.InvokeGracefully();
		}

		private void OnApplicationQuit()
		{
			OnApplicationQuitEvent.InvokeGracefully();
		}

		#endregion
	}
}