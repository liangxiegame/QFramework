using System;

namespace QFramework
{

	public class Game : IDisposable
	{
		public Game()
		{
			InitGameMode();
		}
		
		public GameMode GameMode { get; protected set; }
		
		public TypeEventSystem EventSystem = new TypeEventSystem();
		public IQFrameworkContainer Container = new QFrameworkContainer();


		protected virtual void InitGameMode()
		{
			GameMode = new GameMode();
		}
		
	
		public void Dispose()
		{
			if (EventSystem != null) EventSystem.Dispose();
		}
	}

	public class GameTime
	{
		public readonly System.Diagnostics.Stopwatch TimeWatch = new System.Diagnostics.Stopwatch();
		public readonly System.Diagnostics.Stopwatch UpdateWatch = new System.Diagnostics.Stopwatch();
		public readonly System.Diagnostics.Stopwatch FixedUpdateWatch = new System.Diagnostics.Stopwatch();

		public void Start()
		{
			TimeWatch.Start();
			UpdateWatch.Start();
			FixedUpdateWatch.Start();
		}

		public float Time
		{
			get { return TimeWatch.ElapsedMilliseconds / 1000f; }
		}

		public float DeltaTime
		{
			get { return UpdateWatch.ElapsedMilliseconds / 1000f; }
		}

		public float FixedDeltaTime
		{
			get { return FixedUpdateWatch.ElapsedMilliseconds / 1000f; }
		}

		public void EndUpdate()
		{
			UpdateWatch.Reset();
		}

		public void EndFixedUpdate()
		{
			FixedUpdateWatch.Reset();
		}

		public void Stop()
		{
			UpdateWatch.Stop();
			FixedUpdateWatch.Stop();
		}
	}
}