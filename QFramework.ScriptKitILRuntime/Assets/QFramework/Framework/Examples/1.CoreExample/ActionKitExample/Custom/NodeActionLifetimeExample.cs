using UnityEngine;

namespace QFramework.Example.ActionKit
{
	public class ActionLifetimeAction : ActionKitAction
	{
		protected override void OnReset()
		{
			Debug.Log("重置了,配合 RepeatNode 或者 Finish() 后再被执行的时候触发");
			mExecuteTime = 0;
		}

		protected override void OnBegin()
		{
			Debug.Log("开始了");
		}

		/// <summary>
		/// 已经执行了的次数
		/// </summary>
		private int mExecuteTime = 0;

		/// <summary>
		/// finished
		/// </summary>
		protected override void OnExecute(float dt)
		{
			Debug.Log("执行了，每帧被调用一次 ");
			mExecuteTime++;

			if (mExecuteTime >= 5)
			{
				// 自动会触发 Finish
				Finished = true;
			}

		}

		protected override void OnEnd()
		{
			Debug.Log("结束了，调用 Finish() 时触发");
		}

		protected override void OnDispose()
		{
			Debug.Log("销毁了,调用 Dispose() 时触发");
		}
	}

	public class NodeActionLifetimeExample : MonoBehaviour
	{

		void Start()
		{
			var lifetimeAction = new ActionLifetimeAction();
			
			this.Repeat(1)
				.Append(lifetimeAction)
				.Begin()
				.DisposeWhen(() => lifetimeAction.Finished);
		}
	}
}