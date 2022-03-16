using UnityEngine;

namespace QFramework.Example
{

	#region 事件定义

	public class GameStartEvent
	{

	}

	public class GameOverEvent
	{
		// 可以携带参数
		public int Score;
	}

	public interface ISkillEvent
	{

	}

	// 支持继承
	public class PlayerSkillAEvent : ISkillEvent
	{

	}

	public class PlayerSkillBEvent : ISkillEvent
	{

	}

	#endregion


	public class TypeEventSystemExample : MonoBehaviour
	{
		private void Start()
		{
			TypeEventSystem.Global.Register<GameStartEvent>(OnGameStartEvent);
			TypeEventSystem.Global.Register<GameOverEvent>(OnGameOverEvent);
			TypeEventSystem.Global.Register<ISkillEvent>(OnSkillEvent);


			TypeEventSystem.Global.Send<GameStartEvent>();
			TypeEventSystem.Global.Send(new GameOverEvent()
			{
				Score = 100
			});

			// 要把事件发送给父类
			TypeEventSystem.Global.Send<ISkillEvent>(new PlayerSkillAEvent());
			TypeEventSystem.Global.Send<ISkillEvent>(new PlayerSkillBEvent());
		}

		void OnGameStartEvent(GameStartEvent gameStartEvent)
		{
			Debug.Log("游戏开始了");
		}

		void OnGameOverEvent(GameOverEvent gameOverEvent)
		{
			Debug.LogFormat("游戏结束，分数:{0}", gameOverEvent.Score);
		}

		void OnSkillEvent(ISkillEvent skillEvent)
		{
			if (skillEvent is PlayerSkillAEvent)
			{
				Debug.Log("A 技能释放");
			}
			else if (skillEvent is PlayerSkillBEvent)
			{
				Debug.Log("B 技能释放");
			}
		}

		private void OnDestroy()
		{
			TypeEventSystem.Global.UnRegister<GameStartEvent>(OnGameStartEvent);
			TypeEventSystem.Global.UnRegister<GameOverEvent>(OnGameOverEvent);
			TypeEventSystem.Global.UnRegister<ISkillEvent>(OnSkillEvent);
		}
	}
}