using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		private static PuppetObjectTable mPuppetTable = new PuppetObjectTable();

		public static IEnumerable<IPuppet> GetPuppets<T>() where T : IPuppet
		{
			return mPuppetTable.TypeIndex.Get(typeof(T));
		}

		public static T GetPuppet<T>() where T : IPuppet
		{
			return (T) mPuppetTable.TypeIndex.Get(typeof(T)).FirstOrDefault();
		}


		public static void RegisterPuppet(IPuppet monoPuppet)
		{
			mPuppetTable.Add(monoPuppet);
		}

		public static void UnregisterPuppet(IPuppet monoPuppet)
		{
			mPuppetTable.Remove(monoPuppet);
		}

		public static void RegisterController(Controller controller)
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

		public static void ExecuteCommand(GameplayKitAction action)
		{
			action.Execute();
		}
	}

	public class PuppetObjectTable : Table<IPuppet>
	{
		public TableIndex<Type, IPuppet> TypeIndex = new TableIndex<Type, IPuppet>(p => p.GetType());
		
		protected override void OnAdd(IPuppet item)
		{
			TypeIndex.Add(item);
		}

		protected override void OnRemove(IPuppet item)
		{
			TypeIndex.Remove(item);
		}

		protected override void OnClear()
		{
			TypeIndex.Clear();
		}

		public override IEnumerator<IPuppet> GetEnumerator()
		{
			return null;
		}

		protected override void OnDispose()
		{
			TypeIndex.Dispose();
		}
	}
}