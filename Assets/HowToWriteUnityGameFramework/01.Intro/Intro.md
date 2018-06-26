# Unity 游戏框架搭建 2018 (一) 架构、框架与 QFramework 简介


### 约定

还记得上版本的第二十四篇的约定嘛？现在出来履行啦~

### 为什么要重制?

之前写的专栏都是按照心情写的，在最初的时候笔者什么都不懂，而且文章的发布是按照很随性的一个顺序。结果就是说，大家都看完了，都还对框架没有一个感觉，感觉很乱。而现在，经过两年多的摸索，笔者已经对框架的体系有了一个了解，所以希望再版一次此系列的专栏。

### 为什么不在原来的文章里直接修改呢?

在上一轮的专栏第二十四篇里有讲过过:虽然以前的内容过时了，但是这些专栏对笔者有很重要的意义，它们记录了笔者成长的一个经历，在评论区有着大家的支持和鼓励也有一些很有意义的问答，所以笔者舍不得破坏掉这些宝贵的回忆。

### 更新哪些内容?

这次的重制更新的内容围绕如下几点:

1. 在原有内容的基础上补充更多内容。
2. 语句不通顺、不太容易理解、有歧义的地方、不够严谨的地方进行修改优化。
3. 顺序调整：会按照从易到难、由浅入深、由常用到不常用这三个角度进行内容上的排版，以提高阅读体验，使只是掌握更容易更充分。
4. QFramework 的介绍与原理，重点是 UI 和 资源管理两个模块的介绍与原理。

整理后的内容结构如下:

* 理论与方法论:包含架构与框架搭建、重构、命名、测试、设计模式等内容。
* 资源管理神器: ResKit (重点)
* UI 框架: UIKit (重点)
* QFramework 最佳实践与 Demo
* 归纳和总结。

希望这次可以大家展现一个比较清晰的框架知识体系。


此次专栏重制的背景就写到这里。接下来开始正文。

## 架构与框架初识

### 什么是架构?

架构是一个约定，一个规则，一个大家都懂得遵守的共识。那这是什么样的约定、什么样的规则、什么样的共识呢？

我以包为例，我经常出差，双肩背包里装了不少东西。笔记本电脑、电源、2 个上网卡、鼠标、USB 线、一盒大的名片、一盒小的名片、口香糖、Mini-DisplayPort 转 VGA 接口、U 盘、几根笔、小螺丝刀、洗漱用品、干净衣服、袜子、香水、老婆给我带的抹脸膏（她嫌我最近累，脸有点黄）、钱包、Token 卡、耳机、纸巾、USB 线、U 盘等。这个包有很多格子，最外面的格子我放常用的，比如笔、纸、一盒小的名片等；中间的格子一般放的是衣服、袜子、洗漱用品、香水等；靠背的那个大格子放了笔记本电脑，和笔记本电脑相近的小格子放的是两个上网卡、Mini-DisplayPort 转 VGA 接口、大盒名片、记事本，和笔记本电脑相近的大格子放的是电源、鼠标、口香糖等。

我闭着眼睛都可以将我的东西从包里掏出来，闭着眼睛都可以将东西塞到包里！但是，非常不幸的是，一旦我老婆整理过我的包，那我就很惨了，老是因为找不到东西而变得抓狂！更不幸的，要是我那个不到两岁的“小可爱”翻过，就更不得了了。

这个包就是我放所有物品的“架构”，每一个东西放置的位置就是我的“约定、规则、共识”。倘若我老婆也知道我的“架构”、我的“约定、规则、共识”，那么不管她怎么动我的包，我都照样能够轻易的拿东西或者放东西。进一步，如果我的同事也知道我的“架构”，知道我的“约定、规则、共识”，那么他们什么时候动我的包，我也毫无所知！——道法自然 《10 年感触：架构是什么？——消灭架构！》


## 什么是框架?
框架（framework）是一个框子--指其约束性，也是一个架子--指其支撑性。——360 百科

### 小结
本小节对框架和架构概念做了简单的认识，得出了以下两个结论：

* 架构是“约定、规则、共识”
* 框架具有约束性和支撑性

到这里，大家应该对这两个概念有点感觉了。但是还是会有很多疑问，比如“如何去做架构？”、“框架的约束性和支撑性分别指的什么怎么体现的？”等等。这些在后续的专栏中详细讲解。关于架构与框架的初识就介绍到这里。

## QFramework 简介

两年前，笔者毕业半年，刚从 cocos2d 转 Unity 不到两个月，当时所在的公司有一套游戏开发框架。笔者用它做了两个月的项目，使用框架做项目的时候并没有去思考框架是什么，只是开始的时候觉得很新鲜，而且越用越顺手，尝到了它的甜头。

后来笔者接到了一个跑酷游戏项目，于是就把工作辞掉了，决定出来全职做这个项目。辞职后，公司的框架由于保密协议就不可以用了。项目就只能从零开始开发，那么结果就是在跑酷项目的开发的过程中各种中水土不服。

于是，笔者就开始了市面上开源框架的选型，折腾了几天，发现要么上手太难，要么学习成本很高文档不齐全，有的框架光是理解概念就要很久，对于像笔者一样刚毕业的初学者来说，市面上的开源框架真的很不友好。

从那时候笔者就决定要 为自己，开发一套符合自己使用习惯的框架，也就是现在的 QFramework。

### 为什么叫 QFramework

笔者在做 cocos2dx 的时候，市面上有个叫 Quick-Cocos2d-x 的开源框架，用两个词形容就是简单、强大。

而笔者一直坚信好的工具就应该简单。

QFramework 的目标是要做到像 Quick-Cocos2d-x 一样 “简单、强大”。当时笔者纠结过很多名字，比如 QuickEngine，QuickUnity 等等。Q 代表 Quick，并且 Q 这个字母给人感觉灵活有弹性，所以最终确定为 QFramework。

### QFramework 的目标

记得在此系列上一轮的第十篇中有如下一段话:
> 笔者意愿是想把 QFramework 打造成,让使用的人觉得所有框架中出现的概念要非常清晰,没有任何模糊的概念,出现的概念已经达成共识的概念,没有任何生僻概念,使用门槛尽很低:)。

这个 flag 是 2016 年立的，目前从用户的反馈来看完成得还不错。

QFramework 群里有人形容 QFramework 三个词:简单、粗暴、还有点小精致，笔者觉得形容地非常地贴切。

目前主要三个模块，UIKit,ResKit,ActionKit。目前还有一个模块 EditorKit 正在开发中。

看到这里大家可能对 ActionKit 有些陌生，它的前身就是 QChain，负责所有的异步逻辑，包含 UniRx 和  Promise 还有一套轻量级的行为树。之后会用一个非常详细的文章介绍它。




### QFramework 快速入门:

要介绍 QFramework 只要附上三段代码就够了:

**1.Action Kit**

* chainning style(Driven by MonoBehaviour or Update)

``` csharp
this.Sequence()
	.Delay(1.0f)
	.Event(()=>Log.I("Delayed 1 second"))
	.Until(()=>something is done)
	.Begin();
```

* object oriented style

``` csharp
var sequenceNode = new SequenceNode();

sequenceNode.Append(DelayAction.Allocate(1.0f));
sequenceNode.Append(EventAction.Allocate(()=>Log.I("Delayed 1 second"));
sequenceNode.Append(UntilAction.Allocate(()=>something is true));

this.ExecuteNode(sequenceNode);
```

**2.Res Kit**
``` csharp
// allocate a loader when initialize a panel or a monobehavour
var loader = ResLoader.Allocate<ResLoader>();

// load someth in a panel or a monobehaviour
loader.LoadSync<GameObject>("Resources/smobj");

loader.LoadSync<Texture2D>("Resources/Bg");

// load by asset bundle's assetName
loader.LoadSync<Texture2D>("HomeBg");

// load by asset bundle name and assetName
loader.LoadSync<Texture2D>("home","HomeBg");


// resycle this panel/monobehaivour's loaded res when destroyed 
loader.Recycle2Cache()
loader = null
```

**3.UI Kit**
``` csharp
// open a panel from assetBundle
UIMgr.OpenPanel<UIMainPanel>();

// load a panel from specified Resources
UIMgr.OpenPanel<UIMainPanel>(prefabName:"Resources/UIMainPanel");

// load a panel from specield assetName
UIMgr.OpenPanel<UIMainPanel>(prefabName:"UIMainPanel1");
```

QFramework 介绍到这里。


## Unity 里的常用架构

### 一份六一礼物
今天是六一，作为礼物先送上一段对笔者影响最大的一段话:

> 在写一个项目的时候，不要短视地说我就把这个项目做完了，就是交一个差上线了就完了，我们希望每写一个游戏的时候，我们都积累一些东西，把写的每一行代码，都当成是一个可以收藏的，甚至是可以传递下去的这样的一个资产。有了这样一个思想，可能我们在写代码的时候，整个的思维模式会完全不一样。——刘钢《Unity 项目架构和开发管理》

以上这段话来自刘钢老师的讲座[《Unity项目架构设计与开发管理》](http://v.qq.com/boke/page/d/0/u/d016340mkcu.html) 的结尾。

### Unity 项目架构设计与开发管理

笔者比较幸运，在学习 Unity 之后不久就看了刘钢老师的这个视频，笔者当时很受启发。而视频中所提出的 Manager Of Managers 很好地为 QFramework 指明了方向。视频讲得通俗易懂，里边很多内容都值得反复咀嚼，笔者之后花了很长时间去消化里边的内容。直到今天，笔者再看一遍视频还是会有很多收获的，希望大家看完之后也有所收获有所启发。

视频中比较精彩的部分是从一个什么架构都没有的项目一点一点演化到 MVVM 和 StrangeIOC 架构。

关于Unity的架构有如下几种常用的方式简单总结如下:

### 1.EmptyGO:

  在 Hierarchy 上创建一个空的 GameObject,然后挂上所有与 GameObject 无关的逻辑控制的脚本。使用GameObject.Find() 访问对象数据。

缺点:逻辑代码散落在各处,不适合大型项目。

### 2.Simple GameManager:

  所有与 GameObject 无关的逻辑都放在一个单例中。
缺点:单一文件过于庞大。

### 3.Manager Of Managers:

将不同的功能单独管理。如下:

* MainManager: 作为入口管理器。 
* EventManager: 消息管理。 
* GUIManager: 图形视图管理。 
* AudioManager: 音效管理。 
* PoolManager: GameObject管理（减少动态开辟内存消耗,减少GC)。
* LevelManager: 关卡管理。
* GameManager: 游戏管理。
* SaveManager: 配置&存储管理。   
* MenuManager 菜单管理。

### 4.将 View 和 Model 之间增加一个媒介层。

MVCS:StrangeIOC 插件。

MVVM:uFrame 插件。

### 5. ECS (Entity Component Based  System)

Unity 是基于 ECS,比较适合 GamePlay 模块使用。
还有比较有名的 [Entitas-CSharp](https://github.com/sschmid/Entitas-CSharp)


### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework & 游戏框架搭建 QQ 交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](https://ws2.sinaimg.cn/large/006tKfTcgy1fr1ywcobcwj30by0byt9i.jpg)

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