



[![](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/liangxiegame/QFramework/blob/master/LICENSE)
[![Build Status](https://travis-ci.org/liangxiegame/QFramework.svg?branch=master)](https://travis-ci.org/liangxiegame/QFramework)

# QFramework 简介 Intro

[中文]()|[English](README_EN.md)

[QFramework](https://github.com/liangxiegame/QFramework) 是提供一套简单、强大、易上手、符合 SOLID 原则、支持领域驱动设计（DDD）、事件驱动、数据驱动、分层、MVC 、CQRS、模块化、易扩展的架构，她的源码只有不到 800 行，你可以完全把她放在你的笔记应用里保存。

## 提供的架构图

![](http://processon.com/chart_image/5c270aa6e4b007ba5d5029dc.png)

## 示意图

![](http://processon.com/chart_image/5cbb1edce4b0bab90960a4f6.png)



### 运行环境

* Unity 2018.4.x ~ 2021.x

## 安装

* 从 [Asset Store](http://u3d.as/SJ9) 下载
* PackageManager
    * add from package git url：https://github.com/liangxiegame/QFramework.git 
    * 或者国内镜像仓库：https://gitee.com/liangxiegame/QFramework.git
* 或者直接复制[此代码](QFramework.cs)到自己项目中的任意脚本中
* OpenUPM(TODO)



## 资源

| 名称                     | 描述                                     | 地址                                                         |
| ----------------------  | ---------------------------------------- | ------------------------------------------------------------ |
| Example 示例 | 包含 CounterApp、《点点点》小游戏等 QF 使用示例 | [github](https://github.com/liangxiegame/QFramework.Example)\|[gitee](https://gitee.com/liangxiegame/QFramework.Example) |
| ------**社区**------ |  |  |
| QQ 群:623597263        | 交流群 | [点击加群](http://shang.qq.com/wpa/qunwpa?idkey=706b8eef0fff3fe4be9ce27c8702ad7d8cc1bceabe3b7c0430ec9559b3a9ce66) |
| github issue | github 社区 | [地址](https://github.com/liangxiegame/QFramework/issues/new) |
| gitee issue | gitee 社区（国内访问快） | [地址](https://gitee.com/liangxiegame/QFramework/issues) |
| ------**教程**------ |  |  |
| 教程《框架搭建 决定版》    | 教程 QFramework  的核心架构是怎么演化过来的？ | [课程主页](https://learn.u3d.cn/tutorial/framework_design)   |
| ------**案例**------ |  |  |
| 独立游戏《鬼山之下》   | 使用 QF 制作的独立游戏           | [游戏主页(Steam)](https://store.steampowered.com/app/1517160/_/) |
| 手机游戏《谐音梗挑战》 | 使用 QF 制作的手机游戏          | [游戏主页(TapTap)](https://www.taptap.com/app/201075)        |
| ------**群友案例**------ |  |  |
| 赛车游戏《Crazy Car》 | 群友使用 QF 进行重构的开源赛车游戏 | [游戏主页(Github](https://github.com/TastSong/CrazyCar))     |



## Star 趋势（如果项目有帮到您欢迎点赞）

[![Stargazers over time](https://starchart.cc/liangxiegame/QFramework.svg)](https://starchart.cc/liangxiegame/QFramework)

### 核心成员

* [h3166179](https://github.com/h3166179)
* [王二](https://github.com/so-sos-so) [so-sos-so](https://github.com/so-sos-so)

* [凉鞋 liangxieq](https://github.com/liangxieq)




### 优秀的 Unity 库、框架

- [ET](https://github.com/egametang/ET)：ET Unity3D Client And C# Server Framework
- [IFramework（OnClick）](https://github.com/OnClick9927/IFramework) Simple Unity Tools
- [JEngine](https://github.com/JasonXuDeveloper/JEngine)  一个基于XAsset&ILRuntime，精简好用的热更框架

### 代码规范完全遵循:

[QCSharpStyleGuide](https://github.com/liangxiegame/QCSharpStyleGuide)


### 赞助 Donate:

* 如果觉得不错可以在 [这里 Asset Store](http://u3d.as/SJ9) 给个 5 星哦~ give 5 star
* 或者给此仓库一个小小的 Star~ star this repository
* 以上这些都会转化成我们的动力,提供更好的技术服务! 


## TODO:

以下内容待整理内容



#### 快速开始 QuickStart:

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
var loader = ResLoader.Allocate();

// load someth in a panel or a monobehaviour
loader.LoadSync<GameObject>("resources://smobj");

loader.LoadSync<Texture2D>("resources://Bg");

// load by asset bundle's assetName
loader.LoadSync<Texture2D>("HomeBg");

// load by asset bundle name and assetName
loader.LoadSync<Texture2D>("home","HomeBg");


// resycle this panel/monobehaivour's loaded res when destroyed 
loader.Recycle2Cache();
loader = null;
```

**3.UI Kit**

``` csharp
// open a panel from assetBundle
UIKit.OpenPanel<UIMainPanel>();

// load a panel from specified Resources
UIKit.OpenPanel<UIMainPanel>(prefabName:"Resources/UIMainPanel");

// load a panel from specield assetName
UIKit.OpenPanel<UIMainPanel>(prefabName:"UIMainPanel1");
```

### 技术支持 Tech Support：
* [文档 Document:http://qf.liangxiegame.com/qf/community](http://qf.liangxiegame.com/qf/community)
* **社区:https://qframework.cn**
* [awesome_qframework](https://github.com/liangxiegame/awesome-qframework)  

#### 下载地址 Download:
* 最新版本:https://github.com/liangxiegame/QFramework/releases
* 

### 可选的包含项目 Include Projects:
* [UniRx](https://github.com/neuecc/UniRx)
* [Json.net](https://github.com/JamesNK/Newtonsoft.Json)

#### 参考 Reference:
* [MultyFramework](https://github.com/OnClick9927/MultyFramework)
* [IFramework_GUICanvas](https://github.com/OnClick9927/IFramework_GUICanvas)
* [IFramework](https://github.com/OnClick9927/IFramework)
* [Loxodon Framework](https://github.com/cocowolf/loxodon-framework)
* [BDFramework](https://github.com/yimengfan/BDFramework.Core)
* [HGFramework: Unity3D客户端框架](https://github.com/zhutaorun/HGFramework)
* [Qarth: Framework For Game Develop With Unity3d](https://github.com/SnowCold/Qarth)
* [GameFramework:A game framework based on Unity 5.3 and later versions](https://github.com/EllanJiang/GameFramework)
* [cocos2d/cocos2d-x](https://github.com/cocos2d/cocos2d-x)
* [ResetCore.Unity](https://github.com/vgvgvvv/ResetCore.Unity)
* [UnityUGUIImageShaderPack](https://github.com/zhangmaker/UnityUGUIImageShaderPack)
* [FishManShaderTutorial](https://github.com/JiepengTan/FishManShaderTutorial)



## 功能列表

| 模块名称  | 包含功能                  | 描述                                                         |
| --------- | ------------------------- | ------------------------------------------------------------ |
| Core      | Architecture              | 一套全栈通用的系统设计架构，QF 的本身是用这套架构设计，同时这套架构可以应用与项目开发，为 QF 的主要提供架构，QF 的文档系统（React）、QF 的插件系统（.Net Core）均用此架构开发。 |
|           | CodeGen                   | 代码生成库，一套链式代码生成模板。                           |
|           | Singleton                 | 一套单例模板工具                                             |
|           | IOC                       | 依赖注入/控制反转容器                                        |
|           | Event                     | 事件机制实现，包含枚举事件 和 类型事件                       |
|           | Pool                      | 各种对象池提供，包含 List、Dictionary 对象池                 |
|           | FSM                       | 一套基于类型的状态机实现                                     |
|           | Disposable                | 销毁模式 和 对应扩展方法实现                                 |
|           | Factory                   | 对象的创建模式封装                                           |
|           | RefCounter                | 引用计数器实现                                               |
|           | CSharpExtensions          | 大量的方便易用的扩展实现                                     |
|           | Table                     | 可以建立索引的表格数据结构实现                               |
|           | EasyIMGUI                 | 方便易用的、面向对象的（组合模式）的 IMGUI（OnGUI）绘制库    |
|           | Utility                   | 各种静态方法封装                                             |
|           | ManagerOfManagers（弃用） | Manager Of Managers 架构的实现                               |
| ActionKit | 之后写                    |                                                              |
| ResKit    | SimulationModel           | 真机和编辑器模拟资源加载双模式，让开发阶段与真机阶段自如切换 |
|           | ResPathLoad               | 从Resources目录与沙盒目录以及从网络中加载资源                |
|           | LoadSprite                | 加载Sprite或精灵图集                                         |
|           | LoadScene                 | 在AssetBundle中同步与异步加载Scene场景                       |
|           | ResAssetManager           | 标记的AssetBunlde资源管理，快速定位                          |
|           | LoadASync                 | 异步加载与异步队列加载                                       |
|           | CustomRes                 | Reskit功能自定义拓展                                         |
|           | CustomLoadConfig          | AssetBundle自定义配置表生成                                  |
| UIKit     | 之后写                    |                                                              |
| AudioKit  | 之后写                    |                                                              |

