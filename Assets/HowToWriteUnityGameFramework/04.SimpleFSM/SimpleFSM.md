# Unity 游戏框架搭建 (四) 简易有限状态机

#### 为什么用有限状态机?

之前做过一款跑酷游戏,跑酷角色有很多状态:跑、跳、二段跳、死亡等等。一开始是使用if/switch来切换状态,但是每次角色添加一个状态（提前没规划好),所有状态处理相关的代码就会指数级增长,那样就会嗅出代码的坏味道了。在这种处理状态并且状态数量不是特别多的情况下,自然就想到了引入状态机。

优点:

* 使代码整洁,状态容易扩展和管理。
* 可复用。
* 还没想到.....
  缺点:
* 也没想到......

#### 什么是有限状态机?
解释不清楚,看了下百度百科。反正是一种数据结构,一个解决问题的工具。
从百度百科可以看到,有限状态机最最最基础的概念有两个:状态和转移。
从刚才跑酷的例子来讲,跑、跳、二段跳等这些就是角色的状态。

如图所示:
![](http://liangxiegame.com/content/images/2016/05/-----2016-05-08---3-10-32.png)
主角从跑状态切换到跳状态,从跳状态切换到二段跳状态,这里的切换就是指状态的转移。状态的转移是有条件的,比如主角从跑状态不可以直接切换到二段跳状态。但是可以从二段跳状态切换到跑状态。

另外,一个基本的状态有:进入状态、退出状态、接收输入、转移状态等动作。但是仅仅作为跑酷的角色的状态管理来说,只需要转移状态就足够了。有兴趣的同学可以自行扩展。
#### 如何实现?
恰好之前看到过一个还算简易的实现(简易就是指我能看得懂- -,希望大家也是),原版是用lua实现的,我的跑酷游戏是用C# 实现的,所以直接贴出C#代码。

```csharp
namespace QFramework
{
	using System.Collections.Generic;

	/// <summary>
	/// 教程地址:http://liangxiegame.com/post/4/
	/// </summary>
	public class QFSMLite
	{
		/// <summary>
		/// FSM callfunc.
		/// </summary>
		public delegate void FSMCallfunc(params object[] param);

		/// <summary>
		/// QFSM state.
		/// </summary>
		class QFSMState
		{
			private string mName;

			public QFSMState(string name)
			{
				mName = name;
			}

			/// <summary>
			/// The translation dict.
			/// </summary>
			public readonly Dictionary<string, QFSMTranslation> TranslationDict = new Dictionary<string, QFSMTranslation>();
		}

		/// <summary>
		/// Translation 
		/// </summary>
		public class QFSMTranslation
		{
			public string FromState;
			public string Name;
			public string ToState;
			public FSMCallfunc OnTranslationCallback; // 回调函数

			public QFSMTranslation(string fromState, string name, string toState, FSMCallfunc onTranslationCallback)
			{
				FromState = fromState;
				ToState = toState;
				Name = name;
				OnTranslationCallback = onTranslationCallback;
			}
		}

		public string State { get; private set; }

		/// <summary>
		/// The m state dict.
		/// </summary>
		private readonly Dictionary<string, QFSMState> mStateDict = new Dictionary<string, QFSMState>();

		/// <summary>
		/// Adds the state.
		/// </summary>
		/// <param name="name">Name.</param>
		public void AddState(string name)
		{
			mStateDict[name] = new QFSMState(name);
		}

		/// <summary>
		/// Adds the translation.
		/// </summary>
		/// <param name="fromState">From state.</param>
		/// <param name="name">Name.</param>
		/// <param name="toState">To state.</param>
		/// <param name="callfunc">Callfunc.</param>
		public void AddTranslation(string fromState, string name, string toState, FSMCallfunc callfunc)
		{
			mStateDict[fromState].TranslationDict[name] = new QFSMTranslation(fromState, name, toState, callfunc);
		}

		/// <summary>
		/// Start the specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		public void Start(string name)
		{
			State = name;
		}

		/// <summary>
		/// Handles the event.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="param">Parameter.</param>
		public void HandleEvent(string name, params object[] param)
		{
			if (State != null && mStateDict[State].TranslationDict.ContainsKey(name))
			{
				QFSMTranslation tempTranslation = mStateDict[State].TranslationDict[name];
				tempTranslation.OnTranslationCallback(param);
				State = tempTranslation.ToState;
			}
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			mStateDict.Clear();
		}
	}
}
```

测试代码(需自行修改):

```csharp
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
```

就这些,想要进一步扩展的话,可以给FSMState类添加EnterCallback和ExitCallback等委托,然后在FSM的HandleEvent方法中进行调用。当时对跑酷的项目来说够用了,接没继续扩展了,我好懒- -,懒的借口是:没有最好的设计,只有最适合的设计,233333。

#### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework&游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](https://ws3.sinaimg.cn/large/006tKfTcgy1fros9vo6tcj30by0byt9i.jpg)

### 如果有帮助到您:

如果觉得本篇教程或者 QFramework 对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

- 给 QFramework 一个 Star:https://github.com/liangxiegame/QFramework
- 下载 Asset Store 上的 QFramework 给个五星(如果有评论小的真是感激不尽):http://u3d.as/SJ9
- 购买 gitchat 话题并给 5 星好评: http://gitbook.cn/gitchat/activity/5abc3f43bad4f418fb78ab77 (6 元，会员免费)
- 购买同名的蛮牛视频课程并给 5 星好评:http://edu.manew.com/course/431 (目前定价 29.8元，之后会涨价)
- 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design( 29.9 元，内容会在 2018 年 10 月份完结)

笔者在这里保证 QFramework、入门教程、文档和此框架搭建系列的专栏永远免费开源。以上捐助产品的内容对于使用 QFramework 的使用来讲都不是必须的，所以大家不用担心，各位使用 QFramework 或者 阅读此专栏 已经是对笔者团队最大的支持了。