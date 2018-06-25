# Unity 游戏框架搭建 (三) QFramework 快速入门

## 前言

QFramework 开发至今已经三年了，到目前为止，还没有进行一次完整的介绍。原因是，在过去，QFramework 在飞速迭代中，API 的变动比较大。

一直到今天，QFramework 目前的版本(v0.0.7) 已经趋于稳定，不会有太大的变动，这就是写本篇文章的契机。

本篇主要介绍 QFramework 的三大核心模块，分别是:

* 资源管理模块 Res Kit
* UI 框架 UI Kit
* 执行节点系统 Action Kit

对应 QFramework 中的目录结构如下:

![](https://ws1.sinaimg.cn/large/006tNc79gy1fsjuraz57qj30b8084glk.jpg)

本文使用的 QFramework 版本为 v0.0.7 版本，下载地址在文章尾部给出。

### Res Kit 快速入门

ResKit 中用户最频繁使用的 API 只有 ResLoader 这一个类。字如其意，ResLoader 职责就是提供资源加载服务。

比如，加载 Resources 目录下的资源，代码如下：

``` csharp
// allocate a loader when initialize a panel or a monobehavour
var loader = ResLoader.Allocate();

// load someth in a panel or a monobehaviour
var smobjPrefab = loader.LoadSync<GameObject>("Resources/smobj");

var bgTexture = loader.LoadSync<Texture2D>("Resources/Bg");

var gameObjPrefab = loader.LoadSync("Resources/gameObj") as GameObject;


// resycle this panel/monobehaivour loaded res when destroyed 
loader.Recycle2Cache();
loader = null;
```

代码中，出现了 ResLoader.Allocate() 这个 API。Allocate 中文意思为"申请"，意思是说从一个对象池容器中申请一个 ResLoader 对象。我们直接可以理解为 new ResLoader()。

与其对仗的是代码片段中的 loader.Recycle2Cache() 这个 API，意思是，将这个 ResLoader 对象回收。

loader.Recycle2Cache() 做了两件事情：

* 对 ResLoader 里加载过的资源进行一次清空操作。
* 回收此 ResLoader 对象到对象池容器当中。大家可以直接理解为销毁对象操作就好了。

另外，还有 ResLoader.LoadSync 这个 API，从字面上理解，就是进行同步加载。类型可以自己输入到泛型中，比如 ResLoader.LoadSync<GameObject>("Resources/SomeObj");

传入的参数，是要加载资源的路径。如果前缀加上 "Resources/" 的话就加载 Resources 目录下的资源。如果不加的话就默认加载 AssetBundle 里的资源。说到这里，如何加载 AssetBundle 里的资源呢? 

要使用 AssetBundle ，就要先进行 AssetBundle 的制作。

在 Res Kit 中 AssetBundle 的制作非常简单。

准备:
* 鼠标右击某个资源或文件夹，然后选择 @ResKit Mark AssetBundle。这步操作是标记操作。
* 快捷键 Command/Ctrl + Shift + R 弹出资源构建面板，确保勾选 Simulation Mode 复选框。

这样一个 AssetBundle 的准备就完成了。



在代码中加载 AssetBundle 资源:

``` csharp 
// init res mgr before load asset bundle
ResMgr.Init();

// allocate a loader when initialize a panel or a monobehavour
var loader = ResLoader.Allocate<ResLoader>();

// load someth in a panel or a monobehaviour
var smObjPrefab = loader.LoadSync<GameObject>("smObj");

var bgTexture = loader.LoadSync<Texture2D>("Bg");

var logoTexture = loader.LoadSync<Texture2D>("hometextures","logo");

// resycle this panel/monobehaivour loaded res when destroyed 
loader.Recycle2Cache();

loader = null;
```

代码和 加载 Resources 资源的代码非常相近。

可以发现在代码最开始进行了一次 ResMgr.Init() 操作。

这步操作一般是在游戏启动的时候调用一次即可，ResMgr.Init 主要是解析 AssetBundle 和 Asset 的配置表。而这个配置则是在制作 AssetBundle 的时候自动生成的，这里不多说，在后边的精讲文章中会进行讲解。

加载的 API 还是 LoadSync,只不过是传入的资源路径没有了 Resources/ 前缀。这个 API 支持只传入 AssetName，不必要传入 AssetBundleName。如果出现了资源重名，传入对应的 AssetBundleName 即可。

除了支持加载 Resources 目录下的资源和 AssetBundle 资源，还支持 各种路径资源和网络资源。

OK，Res Kit 的快速入门介绍到这里。

### UI Kit 快速入门

UI Kit 是 QFramework 中的 UI 开发套件。它集成了 UI 管理，可积累的 UI 组件机制和一套简单的逻辑表现分离的架构规则。

如何制作一个 UI 界面 ?

第一步:将 UIRoot 拖入 Hierarchy 中

![](https://ws2.sinaimg.cn/large/006tNc79gy1fsn6kemcguj30e808i3yg.jpg)



可以看到，Design 是专门用来拼界面用的，当该场景运行时，Design 节点下的所有东西都会被隐藏。

第二步: 拼好任意一个界面,对在代码中想要获取的控件添加 UIMark 脚本。
![](https://ws1.sinaimg.cn/large/006tNc79gy1fsn6okau4qj318o10gjt6.jpg)

第三步:将该 Panel 制作成 prefab 放到任意位置 (推荐放在 Assets/Art/UIPrefab 中),右击该 prefab,选择 @ResKit - Create UI Code 来生成 UI 代码。

![](https://ws2.sinaimg.cn/large/006tNc79gy1fsn6snj32aj30py0v8jst.jpg)

生成代码位置如下:

![](https://ws3.sinaimg.cn/large/006tNc79gy1fsn6va6dkvj30di0gs74g.jpg)

第四步 编写 UI 脚本

``` csharp
/****************************************************************************
 * 2018.6 凉鞋的MacBook Pro (2)
 ****************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIHomePanelData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UIHomePanel : UIPanel
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as UIHomePanelData ?? new UIHomePanelData();
			//please add init code here
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
			BtnStartGame.onClick.AddListener(() =>
			{
				Log.E("开始按钮点击");
			});
		}

		protected override void OnShow()
		{
			base.OnShow();
		}

		protected override void OnHide()
		{
			base.OnHide();
		}

		protected override void OnClose()
		{
			base.OnClose();
		}

		void ShowLog(string content)
		{
			Debug.Log("[ UIHomePanel:]" + content);
		}

		UIHomePanelData mData = null;
	}
}
```

第五步 测试该界面

* 在场景中创建 GameObject 命名为 TestUIHomePanel。
* 挂上 UIPanelTester 脚本,填入 Panel 名字。

![](https://ws2.sinaimg.cn/large/006tNc79gy1fsn725dtjvj30me08adg1.jpg)

* 将该 UI 的 prefab 标记为 AssetBundle。
* 运行。

则可以看到该界面。

当然用代码打开该页面也比较简单。
``` csharp
UIMgr.OpenPanel<UIHomePanel>();
```

OK,UIKit 的快速入门写到这里。

### ActionKit 快速入门

**ActionKit** 的设计初衷是为了解决异步逻辑的管理问题，异步逻辑在日常开发中往往比较难以管理，而且代码的风格差异很大。诸如 "播放一段音效并获取播放完成的事件","当 xxx 为 true 时触发"，包括我们常用的 Tween 动画都是异步逻辑，以上的异步逻辑都可以用 **ExecuteNode** 来封装他们。由此设计出了 **NodeSystem**，灵感来自于 **cocos2d** 的 **CCAction**。

ActionKit 有两个基础概念 Node 和 Action:
* Action 为动作指令。
* Node 为 Action 的容器。

### 基础 Node

#### 1.延时节点: DelayAction

通过 **this**(MonoBehaviour) 触发延时回调。

``` csharp
this.Delay(1.0f, () =>
{
	Log.I("延时 1s");
});
```

通过申请 **DelayNode** 对象，使用 **this**(MonoBehaviour) 触发延时回调。

``` csharp
var delay2s = DelayAction.Allocate(2.0f, () => { Log.I("延时 2s"); });
this.ExecuteNode(delay2s);
```

使用 **Update** 驱动延时回调。

``` csharp
private DelayAction mDelay3s = DelayAction.Allocate(3.0f, () => { Log.I("延时 3s"); });

private void Update()
{
	if (mDelay3s != null && !mDelay3s.Finished && mDelay3s.Execute(Time.deltaTime))
	{
		Log.I("Delay3s 执行完成");
	}
}
```

**FeatureId:CEDN001**

#### 2.事件节点: EventAction

​	字如其意,**EventAction**,也就是分发事件。也许单独使用并不会发挥它的价值，但是在 **容器节点** 里他是不可或缺的。

通过申请 **EventAction** 对象，使用 **this**(MonoBehaviour) 触发事件执行。

``` csharp
var eventAction = EventAction.Allocate(() => { Log.I("event 1 called"); }, () => { Log.I("event 2 called"); });
this.ExecuteNode(eventAction);
```

使用 **Update** 驱动回调。

``` csharp
private EventAction mEventAction2 = EventAction.Allocate(() => { Log.I("event 3 called"); }, () => { Log.I("event 4 called"); });

private void Update()
{
	if (mEventAction2 != null && !mEventAction2.Finished && mEventAction2.Execute(Time.deltaTime))
	{
		Log.I("eventAction2 执行完成");
	}
}
```

**FeatureId:CEEN001**

### 基础容器

#### 1.Sequence

**SequenceNode** 字如其意就是序列节点，是一种 **容器节点** 可以将孩子节点按顺序依次执行，每次执行完一个节点再进行下一个节点。

通过 **this**(MonoBehaviour) 触发延时回调。

``` csharp
this.Sequence()
	.Delay(1.0f)
	.Event(() => Log.I("Sequence1 延时了 1s"))
	.Begin()
	.DisposeWhenFinished() // Default is DisposeWhenGameObjDestroyed
	.OnDisposed(() => { Log.I("Sequence1 destroyed"); });
```

通过申请 **SequenceNode** 对象，使用 **this**(MonoBehaviour) 触发节点执行。

``` csharp
	var sequenceNode2 = SequenceNode.Allocate(DelayAction.Allocate(1.5f));
	sequenceNode2.Append(EventAction.Allocate(() => Log.I("Sequence2 延时 1.5s")));
	sequenceNode2.Append(DelayAction.Allocate(0.5f));
	sequenceNode2.Append(EventAction.Allocate(() => Log.I("Sequence2 延时 2.0s")));

	this.ExecuteNode(sequenceNode2);

	/* 这种方式需要自己手动进行销毁
	sequenceNode2.Dispose();
	sequenceNode2 = null;
	*/

	// 或者 OnDestroy 触发时进行销毁
	sequenceNode2.AddTo(this);
```

使用 **Update** 驱动执行。

``` csharp
private SequenceNode mSequenceNode3 = SequenceNode.Allocate(
			DelayAction.Allocate(3.0f),
			EventAction.Allocate(() => { Log.I("Sequence3 延时 3.0f"); }));

private void Update()
{
    if (mSequenceNode3 != null 
        && !mSequenceNode3.Finished 
        && mSequenceNode3.Execute(Time.deltaTime))
	{
		Log.I("SequenceNode3 执行完成");
	}
}

private void OnDestroy()
{
	mSequenceNode3.Dispose();
	mSequenceNode3 = null;
}
```

OK，ActionKit 快速入门到这里

### 优势

Res Kit:

* 简单易用。

* 对每个 ResLoader 中的资源进行引用计数管理，不会造成重复加载，重复卸载等问题，减少大脑负荷。
* Simulation Mode 模式不用每次都需要 Build AssetBundle,加快开发，降低时间成本。
* ResLoader 支持只传入 AssetName,当资源目录结构发生改变时不用再更改代码，减少人力成本。
* 除了可以管理 Resources 和 AssetBundles 资源，还支持加载 网络资源 和 本地资源，接口可自定义，比较灵活。
* 其他的优势同市面上常用的 AssetBundleManager。

UI Kit:

* 简单易用。

* 代码生成，节省时间，避免了 transform.Find("XXX").GetComponent<YYY> 这个 API 的大量编写。
* UIComponent 可将 UI 空间进行复用和模块化，方便团队积累 UI 控件。
* 一套推荐使用的 表现和数据分离的结构(非MVC)。
* 其他的优势同市面上常用的 UIManager。

Action Kit:

* 简单易用。

* 更清晰的方式编写复杂的异步逻辑。
* 可扩展并组织所有的异步操作 Tween、Animation、网络请求 等等。
* 可自组成状态机。
* 本身是一套轻量级的行为树。
* 链式支持。

在后续的文章中，会对文件结构、模块结构、各个模块的快速入门及推荐使用方式等几个角度对 QFramework 进行介绍。

OK，今天先到这里。

### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework &游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](https://ws2.sinaimg.cn/large/006tKfTcgy1fr1ywcobcwj30by0byt9i.jpg)

### 如果有帮助到您:

如果觉得本篇教程对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

- 购买 gitchat 话题《Unity 游戏框架搭建：资源管理神器 ResKit》
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