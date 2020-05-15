using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
	[MonoSingletonPath("GameplayKit")]
	public class GameplayKit : MonoBehaviour, ISingleton
	{
		private static GameplayKit mGameplayKit
		{
			get { return MonoSingletonProperty<GameplayKit>.Instance; }
		}

		public static void RegisterGameplayComponent(GameplayComponent gameplayComponent)
		{
			mGameplayKit.mGameplayComponents.Add(gameplayComponent);
		}

		public static void UnRegisterGameplayComponent(GameplayComponent gameplayComponent)
		{
			mGameplayKit.mGameplayComponents.Remove(gameplayComponent);
		}

		private GameTimer mTimer = new GameTimer();

		public static float TimeScale
		{
			get { return mGameplayKit.mTimer.timeScale; }
		}

		void Update()
		{
			//update global timer
			mTimer.UpdateTime();

			float deltaTime = mTimer.deltaTime;
			float gameTime = mTimer.gameTime;

			mGameplayComponents.ForEach(e => e.UpdateAI(gameTime, deltaTime));

			// 第三层：更新请求层
			mGameplayComponents.ForEach(e => e.UpdateReqeust(gameTime, deltaTime));
			// 第四层：更新行为层（怎么做）- How to do （决策的执行，播放动画等等)
			// 第五层：更新运动层
			mGameplayComponents.ForEach(e => e.UpdateBehavior(gameTime, deltaTime));

			mTimer.UpdateTime();
		}

		//--------------------------------------------------------------------------------------
		private List<GameplayComponent> mGameplayComponents = new List<GameplayComponent>();

		public static void Init()
		{
			mGameplayKit.mTimer.Init();
		}

		void ISingleton.OnSingletonInit()
		{
		}
	}
}