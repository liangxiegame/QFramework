# 事件篇（1）基于类型的事件系统—TypeEventSystem
在上一篇，我们对 UI Kit 的介绍做了一个小结，在此篇

从此篇开始，我们介绍 QFramework 提供的事件系统，而第一个推荐使用的系统就是 TypeEventSystem。

## TypeEventSystem 基本使用
TypeEventSystem 是一个基于类型的事件系统，废话不多说，我们来看下它的基本使用。
``` csharp
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
// 输出结果
// 游戏开始了
// 游戏结束，分数:100
// A 技能释放
// B 技能释放
```

使用非常方便，能够做的事情也比较多。

TypeEventSystem 的诞生在小班教学的过程，其前身是 SimpleEventSystem，由于 SimpleEventSystem 需要依赖 UniRx，不太容易进行教学，所以就写了个 TypeEventSystem，TypeEventSystem 代码更少，就只有一个文件，不依赖 UniRx，所以就成为了 QFramework 首推的事件系统。

## TypeEventSystem 优势
* 如果自己组一套 MVC 结构的时候 TypeEventSystem 会有更大的优势，因为它是基于类型的，消息体就是一个类，所以支持继承。有了继承就比较容易实现一套 Command 机制了。
* 消息的定义不需要去计数了，不想基于枚举的的时间机制，需要把所有的消息定义都写到一个文件里，然后头尾相接。
* 消息体的定义更简洁，表达能力更强。

当然它是有一定的劣势的，比如它有一定的 Type 操作（反射），没有基于枚举的事件机制性能好，比较适合做宏观方面的事件通信。

不过笔者现在的所有项目中的消息机制，换成了 TypeEventSystem。

用下来发现，它真的很香。


由于非常轻量，就放到了 QFramework.cs 里了，PackageKit 的编辑器中的事件通信就用的 TypeEventSystem。

想集成到自己项目的童鞋，请拿走不谢。

此篇的内容就这些。