# Unity 游戏框架搭建 (七) 减少加班利器-QApp类

本来这周想介绍一些框架中自认为比较好用的小工具的,但是发现很多小工具都依赖一个类----App。

App 类的职责:

1.接收 Unity 的生命周期事件。

2.做为游戏的入口。

3.一些框架级别的组件初始化。

本文只介绍App的职责2:做为游戏的入口。

#### Why?
在我小时候做项目的时候,每次改一点点代码(或者不止一点点),要看下结果就要启动游戏->Loading界面->点击各种按钮->跳转到目标界面看结果或者Log之类的。一天如果10次这种行为会浪费很多时间,如果按照时薪算的话那就是......很多钱(捂嘴)。
流程图是这样的:
![](https://ws4.sinaimg.cn/large/006tNc79ly1fsb2wkfk24j30gj080jrj.jpg)

#### 为什么会出现这种问题呢?
1.模块间的耦合度太高了。下一个模块要依赖前一个模块的一些数据或者逻辑。
2.或者有可能是这个模块设计得太大了,界面太多,也会发生这种情况。

#### 解决方案:
针对问题1:在模块的入口提供一个测试的接口,用来写这个模块的资源加载或者数据初始化的逻辑,...什么!?...你们项目就一个模块...来来来我们好好聊聊.....
针对问题2:在模块的入口提供一个测试接口,写跳转到目标界面的相关代码。
流程图是这样的:
![](https://ws4.sinaimg.cn/large/006tKfTcgy1frost3hq0ij30gz0bfaaf.jpg)
虽然很low但是勉强解决了问题。

#### 阶段的划分
资源加载乱七八糟的代码和最好能一步跳转到目标界面的代码,需要在出包或者跑完整游戏流程的时候失效。
如何做到?答案是阶段的划分。
我的框架里分为如下几个阶段:
1.开发阶段: 不断的编码->验证结果->编码->验证结果->blablabla。
2.出包/真机阶段: 这个阶段跑跑完整流程,在真机上跑跑,给QA测测。
3.发布阶段:  上线了,yeah!。

对应的枚举:
```csharp
public enum AppMode 
{
	Developing,
	QA,
	Release
}
```
很明显,乱七八糟的代码是要在开发阶段有效，但是在QA或者Release版本中无效的。那么只要在游戏的入口处判断当前在什么阶段就好了。
开始编码:

```csharp
/// <summary>
/// 全局唯一继承于MonoBehaviour的单例类，保证其他公共模块都以App的生命周期为准
/// </summary>
public class App : QMonoSingleton<App>
{
	public AppMode mode = AppMode.Developing;

	private App() {}

	void Awake()
	{
		// 确保不被销毁
		DontDestroyOnLoad(gameObject);

		mInstance = this;

		// 进入欢迎界面
		Application.targetFrameRate = 60;
	}
		
	void  Start()
    {
		CoroutineMgr.Instance ().StartCoroutine (ApplicationDidFinishLaunching());
	}

	/// <summary>
	/// 进入游戏
	/// </summary>
	IEnumerator ApplicationDidFinishLaunching()
	{
		// 配置文件加载 类似PlayerPrefs
		QSetting.Load();

		// 日志输出 
		QLog.Instance ();

		yield return GameManager.Instance ().Init ();

		// 进入测试逻辑
		if (App.Instance().mode == AppMode.Developing) 
		{
		
            // 测试资源加载
            ResMgr.Instance ().LoadRes ("TestRes",delegate(string resName, Object resObj) 
		{
 				if (null != resObj) 
				{
                    GameObject.Instantiate(resObj);
              }
                // 进入目标界面等等
 
            });
            yield return null;
		// 进入正常游戏逻辑
		} 
		else 
		{
			yield return GameManager.Instance ().Launch ();
		}
			
		yield return null;
	}


```

首先App是Mono单例,要接收Unity的生命周期.
然后要维护一个AppMode类型的变量,便于区分。
之后在,ApplicationDidFinishLaunching中有这么一段代码
```csharp
		// 进入测试逻辑
		if (App.Instance().mode == AppMode.Developing) 
		{
			// 测试资源加载
			ResMgr.Instance ().LoadRes ("TestRes",delegate(string resName, Object resObj) 
			{
				if (null != resObj) 
				{
					GameObject.Instantiate(resObj);
				}
				// 进入目标界面等等
			});

			yield return null;

		// 进入正常游戏逻辑
		} 
		else 
		{
			yield return GameManager.Instance ().Launch ();
		}
```
在这段代码中做了阶段的区分。所有的逻辑都可以写在这里。这样基本的需求就满足啦。

####还有一个问题:
假如一个游戏的业务逻辑分为模块A,B,C,D,E,分为5个不同的人来开发,那App是一个mono单例,除非不提交App代码,否则每次都要解决冲突,同样很浪费时间。怎么办? 答案是通过多态来解决,先定义一个ITestEntry接口,只定义一个方法。
```csharp
	/// <summary>
	/// 测试入口
	/// </summary>
	public interface ITestEntry 
	{
		/// <summary>
		/// 启动
		/// </summary>
		IEnumerator Launch();
	}
```
然后每个模块分别实现ITestEntry接口,例如AModuleTestEntry,BModuleTestEntry等等。
看下项目中的实现:
```csharp
/// <summary>
/// AR模块测试入口
/// </summary>
public class ARSceneTestEntry :MonoBehaviour,ITestEntry 
{
	public IEnumerator Launch() 
	{
		Debug.LogWarning ("进入AR场景开始");

		yield return GameObject.Find ("ARScene").GetComponent<ARScene> ().Launch ();

		yield return null;
	}
}

```

App类中阶段区分的代码要改成这样:
```csharp
		// 进入测试逻辑
		if (App.Instance().mode == AppMode.Developing) {
		
			yield return GetComponent<ITestEntry> ().Launch ();

		// 进入正常游戏逻辑
		} else {
			yield return GameManager.Instance ().Launch ();

		}
```

因为Launch方法的返回类型是IEnumerator,所以很好控制跳转的时间。
看下在Unity中是什么样的:
![](https://ws1.sinaimg.cn/large/006tKfTcgy1frostv3etsj30lt0abq4i.jpg)

每个模块都要有个App的GameObject，原因是因为,框架的其他的组件依赖于App,也想过把依赖的部分抽离出来,那样的话可能命名为QMonoLifeCircleReceiver和ModuleEntry之类的,这样遵循了单一职责原则。不过孰优孰略各有千秋。我觉得叫App更直观一些,因为入口、组件初始化、启动某个模块应该是通常放在一起更人性化,还有一些ApplicationDidEnterBackground之类的事件还是模仿iOS的AppDelegate人性化一些。

如果要跑完整流程,那么把模块的App GameObject关掉就好了。要注意一点是:在整个游戏的入口场景要有个App GameObject放在上面,并且AppMode要为Release或者QA。这样才能正常地跑起来。

OK就这样....
#### 对未来的一些畅想:

1. 最近在想着如何为项目引入自动化测试,有一个思路是这样的,界面的所有输入包括点击事件等都包装成一个命令或者一个消息。测试的时候只要不断地自动发送消息或者命令就好了。当然只是个畅想。 那和这个有毛关系呢,有啊!界面跳转的时候可以发命令或者消息就够了啊,这样还很方便。 但实际上有很多问题,包括模块的最上层如何拿到一些界面组件的权限比如按钮等等。处理命令或者消息的话那么所有的输入都要经过一层过滤。。。。额。。想想好麻烦。。。以后吧。。。以后吧。。

2. 框架的很多组件都是基于字典实现的。字典真好用,23333。以后还是想办法能改的都改成List吧。


#### 欢迎讨论!

#### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework&游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](https://ws1.sinaimg.cn/large/006tKfTcgy1frosusbd6oj30by0byt9i.jpg)

### 如果有帮助到您:

如果觉得本篇教程对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

- 购买 gitchat 话题《Unity 游戏框架搭建：资源管理 与 ResKit 精讲》
  - 价格: 6 元，会员免费
  - 地址:  http://gitbook.cn/gitchat/activity/5b29df073104f252297a779c
- 给 QFramework 一个 Star
  - 地址: https://github.com/liangxiegame/QFramework
- 给 Asset Store 上的 QFramework 并给个五星(需要先下载)
  - 地址: http://u3d.as/SJ9
- 购买同名的蛮牛视频课程录播课程:
  - 价格 ~~19.2 元~~ 29.8 元
  - 地址: http://edu.manew.com/course/431 
- 购买 gitchat 话题《Unity 游戏框架搭建：我所理解的框架》
  - 价格: 6 元，会员免费
  - 地址:  http://gitbook.cn/gitchat/activity/5abc3f43bad4f418fb78ab77
- 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design( 29.9 元，内容会在 2018 年 10 月份完结)