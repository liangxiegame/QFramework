using UnityEngine;

namespace QFramework.Example.ActionKit
{
	public class ActionKitFSMExample : MonoBehaviour
	{
		/// <summary>
		/// 走状态
		/// </summary>
		class WalkState : ActionKitFSMState<ActionKitFSMExample>
		{
			public WalkState(ActionKitFSMExample target) : base(target)
			{
			}

			protected override void OnEnter()
			{
				Debug.Log("进入 走 状态");
			}

			protected override void OnExit()
			{
				Debug.Log("退出 走 状态");
			}
		}


		/// <summary>
		/// 跑状态
		/// </summary>
		class RunState : ActionKitFSMState<ActionKitFSMExample>
		{
			public RunState(ActionKitFSMExample target) : base(target)
			{
			}
			
			protected override void OnEnter()
			{
				Debug.Log("进入 跑 状态");
			}

			protected override void OnExit()
			{
				Debug.Log("退出 跑 状态");
			}
		}


		/// <summary>
		/// 空格键按下 走=>跑
		/// </summary>
		class SpaceKeyDown : ActionKitFSMTransition<WalkState,RunState>
		{
			
		}

		/// <summary>
		/// 空格键按起 跑=>走
		/// </summary>
		class SpaceKeyUp : ActionKitFSMTransition<RunState,WalkState>
		{
			
		}
		
		
		/// <summary>
		/// 状态机
		/// </summary>
		ActionKitFSM mFsm = new ActionKitFSM();
		
		void Awake()
		{
			var walkState = new WalkState(this);
			var runState = new RunState(this);
			
			mFsm.AddState(walkState);
			mFsm.AddState(runState);
			
			mFsm.AddTransition(new SpaceKeyDown());
			mFsm.AddTransition(new SpaceKeyUp());
			
			mFsm.StartState<WalkState>();
		}


		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				mFsm.HandleEvent<SpaceKeyDown>();
			}

			if (Input.GetKeyUp(KeyCode.Space))
			{
				mFsm.HandleEvent<SpaceKeyUp>();
			}
		}
	}
}