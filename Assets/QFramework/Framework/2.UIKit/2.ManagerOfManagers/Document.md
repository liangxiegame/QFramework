### Event 模块简介:

在游戏开发中，我们经常要用到事件模块。

QFramework 为此提供了 QEventSystem。



它的使用非常简单。



注册事件

``` csharp
using UnityEngine;

namespace QFramework
{
	public enum TestEvent
	{
		TestOne
	}

	public class EventGet : MonoBehaviour 
	{
		void Start () 
		{
			QEventSystem.RegisterEvent(TestEvent.TestOne,GetEvent);
		}

		void GetEvent(int key, params object[] obj)
		{
			switch (key)
			{
				case (int)TestEvent.TestOne:
					this.LogInfo(obj[0].ToString());
					break;
			}
		}
	}
}
```

当鼠标左键按下后，发送事件

``` csharp
using UnityEngine;
using UniRx;

namespace QFramework
{
    public class EventTest : MonoBehaviour
    {       
        void Start()
        {
            Observable.EveryUpdate()
				.Where(x => Input.GetMouseButtonDown(0))
                .Subscribe(_ => QEventSystem.SendEvent(TestEvent.TestOne,"Hello World!"));
        }
    }
}
```



当程序运行后，每次点击鼠标就会输出 "Hello World!"



示例地址: [Assets/QFramework/Example/UIKitExample/EventExample/](https://github.com/liangxiegame/QFramework/tree/master/Assets/QFramework/Example/UIKitExample/EventExample)



