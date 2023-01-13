







![LOGO](LOGO.png)



[![](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/liangxiegame/QFramework/blob/master/LICENSE)
[![Build Status](https://travis-ci.org/liangxiegame/QFramework.svg?branch=master)](https://travis-ci.org/liangxiegame/QFramework)

# QFramework 简介 Intro

[中文]()|[English](README_EN.md)

[QFramework](https://github.com/liangxiegame/QFramework) 是提供一套简单、强大、易上手、符合 SOLID 原则、支持领域驱动设计（DDD）、事件驱动、数据驱动、分层、MVC 、CQRS、模块化、易扩展的架构，她的源码只有不到 800 行，你可以完全把她放在你的笔记应用里保存。

## 提供的架构图

![](http://processon.com/chart_image/5c270aa6e4b007ba5d5029dc.png)

## 举个例子（一图胜千言😂）

![](https://file.liangxiegame.com/6bf42306-0b2a-4417-bbcf-354af0132596.png)

## 各种情况的示意图

![](http://processon.com/chart_image/5cbb1edce4b0bab90960a4f6.png)

## 架构使用规范

**QFramework系统设计架构分为四层及其规则：**

* 表现层：ViewController层。IController接口，负责接收输入和状态变化时的表现，一般情况下，MonoBehaviour 均为表现层
    * 可以获取System
    * 可以获取Model
    * 可以发送Command
    * 可以监听Event
* 系统层：System层。ISystem接口，帮助IController承担一部分逻辑，在多个表现层共享的逻辑，比如计时系统、商城系统、成就系统等
    * 可以获取System
    * 可以获取Model
    * 可以监听Event
    * 可以发送Event
* 数据层：Model层。IModel接口，负责数据的定义、数据的增删查改方法的提供
    * 可以获取Utility
    * 可以发送Event
* 工具层：Utility层。IUtility接口，负责提供基础设施，比如存储方法、序列化方法、网络连接方法、蓝牙方法、SDK、框架继承等。啥都干不了，可以集成第三方库，或者封装API
* 除了四个层级，还有一个核心概念——Command
    * 可以获取System
    * 可以获取Model
    * 可以发送Event
    * 可以发送Command
* 层级规则：
    * IController 更改 ISystem、IModel 的状态必须用Command
    * ISystem、IModel状态发生变更后通知IController必须用事件或BindableProperty
    * IController可以获取ISystem、IModel对象来进行数据查询
    * ICommand不能有状态
    * 上层可以直接获取下层，下层不能获取上层对象
    * 下层向上层通信用事件
    * 上层向下层通信用方法调用（只是做查询，状态变更用Command），IController的交互逻辑为特别情况，只能用Command

（照抄自：[学生课堂笔记1](https://github.com/Haogehaojiu/FrameworkDesign)）

### 运行环境

* Unity 2018.4.x ~ 2021.x

## 安装

* QFramework.cs 
    * 直接复制[此代码](QFramework.cs)到自己项目中的任意脚本中
* QFramework.cs 与 官方示例
    * [点此下载 unitypackage](./QFramework.cs.Examples.unitypackage)

* QFramework.ToolKits
    * [点此下载 unitypackage](./QFramework.Toolkits.unitypackage)
* QFramework.ToolKitsPro
    * 从 [AssetStore](http://u3d.as/SJ9) 安装



## 资源

| **版本** |                                      |                                                          |
| ----------------------  | ---------------------------------------- | ------------------------------------------------------------ |
| QFramework.cs | QFramework 本体架构的实现 | [文件](QFramework.cs) |
| QFramework.cs  示例 | QFramework.cs 与官方示例： CounterApp、《点点点》、FlappyBird、CubeMaster、ShootingEditor2D、贪吃蛇等 | [点此下载 unitypackage](./QFramework.cs.Examples.unitypackage) |
| QFramework.Toolkits | QFramework  集成 CoreKit/UIKit/ActionKit/ResKit/PackageKit/AudioKit 等全部官方工具（已包含 QFramework.cs 和 示例) | [点此下载 unitypackage](./QFramework.Toolkits.unitypackage) |
| QFramework.Toolkits.Demo.WuZiQi | 使用 QFramework.Toolkits 开发的五子棋 Demo（需要安装好  QFramework.Toolkits） | [点此下载 unitypackage](./QFramework.Toolkits.Demo.WuZiQi.unitypackage) |
| QFramework.Toolkits.Demo.Saolei | 使用 QFramework.Toolkits 开发的扫雷 Demo（需要安装好  QFramework.Toolkits） | [点此下载 unitypackage](./QFramework.Toolkits.Demo.SaoLei.unitypackage) |
| QFramework.ToolKitsPro | 在 ToolKits 基础上集成更多好用的工具的版本（已包含 QFramework.Toolkits） | [AssetStore](http://u3d.as/SJ9) |
| **群友案例** |  |  |
| 赛车游戏《Crazy Car》 | 群友 [TastSong](https://github.com/TastSong) 使用 QF 进行重构的开源赛车游戏 | [游戏主页(Github](https://github.com/TastSong/CrazyCar)) |
| **社区** |  |  |
| QQ 群:623597263        | 交流群 | [点击加群](http://shang.qq.com/wpa/qunwpa?idkey=706b8eef0fff3fe4be9ce27c8702ad7d8cc1bceabe3b7c0430ec9559b3a9ce66) |
| github issue | github 社区 | [地址](https://github.com/liangxiegame/QFramework/issues/new) |
| gitee issue | gitee 社区（国内访问快） | [地址](https://gitee.com/liangxiegame/QFramework/issues) |
| **教程** |  |  |
| 《框架搭建 决定版》    | 教程 QFramework  的核心架构是怎么演化过来的？ | [课程主页](https://learn.u3d.cn/tutorial/framework_design)\|[学生课堂笔记1](https://github.com/Haogehaojiu/FrameworkDesign)\|[学生课堂笔记2](https://github.com/Haogehaojiu/ShootingEditor2D) |
| **产品案例** | 如果用了 qf 并且想要在如下列表中登记，可以在 github/gitee 的 issue 里发帖子,也可以加 qq 群，也可以用邮箱联系凉鞋 liangxiegame@163.com |  |
| 独立游戏《当火车鸣笛三秒》 | 部分使用 QF 制作的独立游戏 | [Steam](https://store.steampowered.com/app/1563700/_/)\|[TapTap](https://www.taptap.cn/app/208258) |
| 独立游戏《你好茄子》 | 部分使用 QF 制作的独立游戏 | [游戏主页(Steam)](https://store.steampowered.com/app/2091640/Hi_Eggplant/) |
| 独立游戏《第一座山》 | 部分使用 QF 制作的独立游戏 | [游戏主页(Steam)](https://store.steampowered.com/app/2149980/The_First_Mountain/) |
| 独立游戏《推灭泡泡姆》 | ‍QF 群友，大学生团队制作的独立游戏，终于等到上架啦，亲自游玩过，很好玩，大家多多支持呀~（P.S 使用 QF.cs 作为架构开发的哦~） | [游戏主页(TapTap)](https://www.taptap.com/app/233228) |
| 独立游戏《鬼山之下》   | 部分使用 QF 制作的独立游戏 | [游戏主页(Steam)](https://store.steampowered.com/app/1517160/_/) |
| 手机游戏《谐音梗挑战》 | 部分使用 QF 制作的独立游戏 | [游戏主页(TapTap)](https://www.taptap.com/app/201075)        |
| **官方工具**（独立版本，不互相依赖) |                                                              |                                                              |
| SingletonKit              | 易上手功能强大的单例工具，由 QF 官方维护                            | [github](https://github.com/liangxiegame/SingletonKit)\|[gitee](https://gitee.com/liangxiegame/SingletonKit) |
| ExtensionKit | 易上手功能强大的 C#/UnityAPI 的静态扩展 ，由 QF 官方维护 | [github](https://github.com/liangxiegame/ExtensionKit)\|[gitee](https://gitee.com/liangxiegame/ExtensionKit) |
| IOCKit | 易上手功能强大的 IOC 容器 ，由 QF 官方维护 | [github](https://github.com/liangxiegame/IOCKit)\|[gitee](https://gitee.com/liangxiegame/IOCKit) |
| TableKit | 一套类似表格的数据结构（List<List\<T\>>)，兼顾查询效率和联合强大的查询功能，由 QF 官方维护 | [github](https://github.com/liangxiegame/TableKit)\|[gitee](https://gitee.com/liangxiegame/TableKit) |
| PoolKit | 对象池工具，由 QF 官方维护 | [github](https://github.com/liangxiegame/PoolKit)\|[gitee](https://gitee.com/liangxiegame/PoolKit) |
| LogKit | 日志工具，由 QF 官方维护 | [github](https://github.com/liangxiegame/LogKit)\|[gitee](https://gitee.com/liangxiegame/LogKit) |
| ActionKit | 动作序列工具，由 QF 官方维护 | [github](https://github.com/liangxiegame/ActionKit)\|[gitee](https://gitee.com/liangxiegame/ActionKit) |
| ResKit | 资源管理工具，由 QF 官方维护 | [github](https://github.com/liangxiegame/ResKit)\|[gitee](https://gitee.com/liangxiegame/ResKit) |
| UIKit | UIKit 是一套 UI/View 开发解决方案，由 QF 官方维护 | [github](https://github.com/liangxiegame/UIKit)\|[gitee](https://gitee.com/liangxiegame/UIKit) |
| AudioKit | 一套音频管理工具，由 QF 官方维护 | [github](https://github.com/liangxiegame/AudioKit)\|[gitee](https://gitee.com/liangxiegame/AudioKit) |
| PackageKit | 一套包管理工具，可以通过 PackageKit 安装旧版本的 QFramework，以及大量的解决方案。 | [github](https://github.com/liangxiegame/PackageKit)\|[gitee](https://gitee.com/liangxiegame/PackageKit) |
| **其他相关教程** |  |  |
| 《独立游戏体验计划》（猫叔） | 独立游戏制作体验教程，有用到 QFramework.cs | [b 站](https://space.bilibili.com/656352) |
| 《原创独立游戏制作》（凉鞋） | 原创独立游戏制作教程，有用到 QFramework.cs | [b 站](https://space.bilibili.com/60450548/channel/collectiondetail?sid=125221) |



## Star 趋势（如果项目有帮到您欢迎点赞）

[![Stargazers over time](https://starchart.cc/liangxiegame/QFramework.svg)](https://starchart.cc/liangxiegame/QFramework)

### 贡献者

* [蓝色孤舟 gdtdftdqtd](https://github.com/gdtdftdqtd)
* [h3166179](https://github.com/h3166179)
* [葫芦 WangEdgar](https://github.com/WangEdgar)
* [凉鞋 liangxiegame](https://github.com/liangxiegame)




### 优秀的 Unity 库、框架

- [ET](https://github.com/egametang/ET)：ET Unity3D Client And C# Server Framework
- [IFramework（OnClick）](https://github.com/OnClick9927/IFramework) Simple Unity Tools
- [JEngine](https://github.com/JasonXuDeveloper/JEngine)  使Unity开发的游戏支持热更新的解决方案。
- [TinaX Framework](https://tinax.corala.space/) “开箱即用”的Unity独立游戏开发工具

### 代码规范完全遵循:

[QCSharpStyleGuide](https://github.com/liangxiegame/QCSharpStyleGuide)


### 赞助 Donate:

* 如果觉得不错可以在 [这里 Asset Store](http://u3d.as/SJ9) 给个 5 星哦~ give 5 star
* 或者给此仓库一个小小的 Star~ star this repository
* 以上这些都会转化成我们的动力,提供更好的技术服务! 
