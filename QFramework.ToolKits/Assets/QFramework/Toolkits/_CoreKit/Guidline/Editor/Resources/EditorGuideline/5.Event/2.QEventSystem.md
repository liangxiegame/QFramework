# QFramework 使用指南 2020（二十四）：事件篇（2）基于枚举的事件系统—QEventSystem
在上一篇，我们介绍了 基于类型的事件系统— TypeEventSystem

在此篇，我们来介绍下基于枚举的事件系统——QEventSystem

## QEventSystem 简介
QEventSystem 是 UI Kit 中内置的事件系统，它是基于枚举的，废话不多说，我们先看下它的基本使用。

### 基本使用

消息注册与处理：
``` csharp
using UnityEngine;

namespace QFramework
{
	#region 事件定义

	public enum TestEvent
	{
		Start,
		TestOne,
		End,
	}

	public enum TestEventB
	{
		Start = TestEvent.End, // 为了保证每个消息 Id 唯一，需要头尾相接
		TestB,
		End,
	}

	#endregion 事件定义

	public class EventReceiverExample : MonoBehaviour
	{
		void Start()
		{
			QEventSystem.RegisterEvent(TestEvent.TestOne, OnEvent);
		}

		void OnEvent(int key, params object[] obj)
		{
			switch (key)
			{
				case (int) TestEvent.TestOne:
					obj[0].LogInfo();
					break;
			}
		}

		private void OnDestroy()
		{
			QEventSystem.UnRegisterEvent(TestEvent.TestOne, OnEvent);
		}
	}
}
```

消息发送：
``` csharp
using UnityEngine;

namespace QFramework
{
    public class EventSenderExample : MonoBehaviour
    {
        private void Update()
        {
            QEventSystem.SendEvent(TestEvent.TestOne, "Hello World!");
        }
    }
}
```

运行之后，输出结果如下:
![image.png](http://file.liangxiegame.com/6ec604a2-0934-49b4-8fcb-60a542989d46.png)

非常简单。

这就是基本使用。

### QEventSystem 优势
* 性能高于 TypeEventSystem
* 与其他的基于枚举的消息机制想必，不需要做 enum 转 int 操作，其他的事件机制需要这样写: QEventSysten.Send((ushort)TestEvent.One)。

基于以上的优势，QFramewwork 在初期就选择了 QEventSystem 作为 Manager Of Managers 架构的基础事件工具。

当然相比于 TypeEventSystem，QEventSystem 的劣势就很明显了，就是它虽然比其他的基于枚举的消息写法简介，但是没有 TypeEventSystem 简介，使用枚举的消息机制的一个问题，就是必须要按照头尾相接的方式定义消息。

不过 QEventSystem 笔者并不打算把它替换下来，原因是 QF 的 Manager Of Managers 架构 和 UI Kit 都依赖 QEventSystem，而 Manager Of Managers 架构其实对新手还是比较友好的，非常容易上手。

### 笔者建议
* 如果还没有使用 UI Kit 中的 消息机制，并且不打算使用 Manager Of Managers 架构，笔者建议使用 TypeEventSystem 作为消息通信的工具
* 如果打算使用 Manager Of Managers 架构，或者 UI Kit 中的消息机制，那么就用 UI Kit 提供的消息机制就好，具体怎么用会在下一篇进行介绍
* 全局的 QEventSystem 与 UI Kit、Manager Of Managers 的 QEventSystem 不是同一个示例，所以在某些情况下如果用到 全局 QEventSystem 那么不要担心与 UI Kit 冲突的问题。不过这里要注意一下 全局的 QEventSystem 事件是发送不到 Manager Of Mananger 和 UI Kit 中的。

而 Manager Of Managers 架构笔者会在 下一个章节进行介绍。

文中的 QEventSystem 所在的目录如下:
![image.png](http://file.liangxiegame.com/215a787e-ab63-4952-b445-8cd12222c995.png)

是一个独立的文件，感兴趣的童鞋拿走不谢。

此篇的内容就这些。