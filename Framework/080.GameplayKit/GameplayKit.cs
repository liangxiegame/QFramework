using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
	[MonoSingletonPath("QFramework/GameplayKit")]
	public class GameplayKit : MonoBehaviour, ISingleton
	{
		GameTime Time = new GameTime();

		static IQFrameworkContainer mContainer = new QFrameworkContainer();

		/// <summary>
		/// 发送到 UI 的事件
		/// </summary>
		public static ITypeEventSystem Game2UIEventSystem = new TypeEventSystem();

		/// <summary>
		/// 游戏内部使用的事件系统
		/// </summary>
		public static ITypeEventSystem GameEventSystem = new TypeEventSystem();

		/// <summary>
		/// 注册游戏模式
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static void RegisterGameMode<T>() where T : IGameMode, new()
		{
			mContainer.RegisterInstance<IGameMode>(new T());
		}

		/// <summary>
		/// 注册游戏状态
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static void RegisterGameState<T>() where T : IGameState, new()
		{
			mContainer.RegisterInstance<IGameState>(new T());
		}

		static HashSet<IGameplayKitObject> mObjects = new HashSet<IGameplayKitObject>();

		public static bool IsGameStart { get; private set; }

		/// <summary>
		/// 注册游戏对象
		/// </summary>
		/// <param name="obj"></param>
		public static void RegisterGameplayKitObject(IGameplayKitObject obj)
		{
			mObjects.Add(obj);

			// 动态加入进来的 Object 就走这个流程
			if (IsGameStart)
			{
				obj.GameStart();
			}
		}

		public static void UnregisterGameplayKitObject(IGameplayKitObject obj)
		{
			mObjects.Remove(obj);
		}

		public static void StartGame(Action onStartDone = null)
		{
			mGameplayKit.StartCoroutine(mGameplayKit.DoStartGame(onStartDone));
		}

		IEnumerator DoStartGame(Action onStartDone = null)
		{
			yield return null;

			foreach (var gameplayKitObject in mObjects)
			{
				gameplayKitObject.GameStart();
			}

			mGameplayKit.Time.Start();

			IsGameStart = true;

			if (onStartDone != null)
			{
				onStartDone();
			}
		}

		public static void StopGame(Action onStopDone = null)
		{
			mGameplayKit.Time.Stop();

			mGameplayKit.StartCoroutine(mGameplayKit.DoStopGame(onStopDone));
		}

		public static void Clear()
		{
			mContainer.Clear();
			Game2UIEventSystem.Clear();
			GameEventSystem.Clear();
			mObjects.Clear();
		}

		IEnumerator DoStopGame(Action onStopDone)
		{
			yield return null;

			foreach (var gameplayKitObject in mObjects)
			{
				gameplayKitObject.GameStop();
			}

			IsGameStart = false;

			if (onStopDone != null)
			{
				onStopDone();
			}
		}

		#region DEPRECATED

		#endregion

		private static GameplayKit mGameplayKit
		{
			get { return MonoSingletonProperty<GameplayKit>.Instance; }
		}

		public static void RegisterGameplayComponent(GameplayComponent gameplayComponent)
		{
			mGameplayKit.mGameplayComponents.Add(gameplayComponent);
		}

		private GameTimer mTimer = new GameTimer();

		public static float TimeScale
		{
			get { return mGameplayKit.mTimer.timeScale; }
		}

		private static QueueNode mQueueNode = new QueueNode();

		private static AsyncNode mAsyncNode = new AsyncNode();

		private void Update()
		{
			// RaiseTickPlayerInputManagers(tickArgs);
			// RaiseTickControllers(tickArgs);
			// RaiseTickActors(tickArgs);
			// RaiseTickPlayerCameraManagers(tickArgs);
			// RaiseTickPlayerHUDManagers(tickArgs);
			// RaiseTickGameMode(tickArgs);
			if (IsGameStart)
			{
				var time = Time.Time;
				var delteTime = Time.DeltaTime;
				mOnUpdateEvents(time, delteTime);
				mControllerOnUpdateEvents(time, delteTime);
				mQueueNode.Execute(delteTime);
				mAsyncNode.Execute(delteTime);
				Time.EndUpdate();
			}
		}

		private void FixedUpdate()
		{
			if (IsGameStart)
			{
				var time = Time.Time;
				var fixedDelteTime = Time.FixedDeltaTime;
				mOnFixedUpdateEvents(time, fixedDelteTime);
				Time.EndFixedUpdate();
			}
		}

		private Action<float, float> mControllerOnUpdateEvents = (time, dt) => { };

		public void RegisterControllerOnUpdateEvent(Action<float, float> onUpdateEvent)
		{
			mControllerOnUpdateEvents += onUpdateEvent;
		}

		public void UnregisterControllerOnUpdateEvent(Action<float, float> onUpdateEvent)
		{
			mControllerOnUpdateEvents -= onUpdateEvent;
		}

		private static Action<float, float> mOnUpdateEvents = (time, dt) => { };

		public static void RegisterOnUpdateEvent(Action<float, float> onUpdateEvent)
		{
			mOnUpdateEvents += onUpdateEvent;
		}

		public void UnregisterOnUpdateEvent(Action<float, float> onUpdateEvent)
		{
			mOnUpdateEvents -= onUpdateEvent;
		}

		private static Action<float, float> mOnFixedUpdateEvents = (time, dt) => { };

		public static void RegisterOnFixedUpdateEvent(Action<float, float> onFixedUpdate)
		{
			mOnFixedUpdateEvents += onFixedUpdate;
		}

		public static void UnregisterOnFixedUpdateEvent(Action<float, float> onFixedUpdate)
		{
			mOnFixedUpdateEvents -= onFixedUpdate;
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

		public static void RegisterController<T>(T controller) where T : Controller
		{
			mContainer.RegisterInstance(controller);
		}

		public static void RegisterController<TController>() where TController : Controller, new()
		{
			mContainer.RegisterInstance(new TController());
		}

		public static T GetController<T>() where T : Controller
		{
			return mContainer.Resolve<T>();
		}

		protected static bool mOnApplicationQuit = false;

		protected virtual void OnApplicationQuit()
		{
			mOnApplicationQuit = true;
		}

		public static bool IsApplicationQuit
		{
			get { return mOnApplicationQuit; }
		}

		public static void PushAction(GameplayKitAction action)
		{
			mQueueNode.Enqueue(action);
		}

		public static void ExecuteAction(GameplayKitAction action)
		{
			mAsyncNode.Add(action);
		}
	}
}