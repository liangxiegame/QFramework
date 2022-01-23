



[![](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/liangxiegame/QFramework/blob/master/LICENSE)
[![Build Status](https://travis-ci.org/liangxiegame/QFramework.svg?branch=master)](https://travis-ci.org/liangxiegame/QFramework)

# QFramework 简介 Intro

[中文]()|[English](README_EN.md)

[QFramework](https://github.com/liangxiegame/QFramework) 是提供一套简单、强大、易上手、符合 SOLID 原则、支持领域驱动设计（DDD）、事件驱动、数据驱动、分层、MVC 、CQRS、模块化、易扩展的架构，她的源码只有不到 800 行，你可以完全把她放在你的笔记应用里保存。

## 提供的架构图

![](http://processon.com/chart_image/5c270aa6e4b007ba5d5029dc.png)

## 举个例子（一图胜千言😂）

![](Example.png)

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

* PackageManager
    * add from package git url：https://github.com/liangxiegame/QFramework.git 
    * 或者国内镜像仓库：https://gitee.com/liangxiegame/QFramework.git
* 或者直接复制[此代码](QFramework.cs)到自己项目中的任意脚本中



## 资源

| **版本** |                                      |                                                          |
| ----------------------  | ---------------------------------------- | ------------------------------------------------------------ |
| QFramework.cs | QFramework 本体架构的实现 |  |
| QFrameworkWith Toolkits | QFramework  集成 UIKit/ActionKit/ResKit/PackageKit/AudioKit 等全部官方工具 | [从国内服务器下载](https://file.liangxiegame.com/Frameworkv0_14_22_a4b5a851_aff3_4f11_beb5_6d87a600c554.unitypackage)\|[AssetStore](http://u3d.as/SJ9) |
| **示例/Demo** |  |  |
| Example 示例 | 包含 CounterApp、《点点点》小游戏等 QF 使用示例 | [github](https://github.com/liangxiegame/QFramework.Example)\|[gitee](https://gitee.com/liangxiegame/QFramework.Example) |
| ShootingEditor2D | 包含一个关卡编辑器的开源射击游戏 | [github](https://github.com/liangxiegame/ShootingEditor2D)\|[gitee](https://gitee.com/liangxiegame/ShootingEditor2D) |
| **群友案例** |  |  |
| 赛车游戏《Crazy Car》 | 群友 [TastSong](https://github.com/TastSong) 使用 QF 进行重构的开源赛车游戏 | [游戏主页(Github](https://github.com/TastSong/CrazyCar)) |
| **社区** |  |  |
| QQ 群:623597263        | 交流群 | [点击加群](http://shang.qq.com/wpa/qunwpa?idkey=706b8eef0fff3fe4be9ce27c8702ad7d8cc1bceabe3b7c0430ec9559b3a9ce66) |
| github issue | github 社区 | [地址](https://github.com/liangxiegame/QFramework/issues/new) |
| gitee issue | gitee 社区（国内访问快） | [地址](https://gitee.com/liangxiegame/QFramework/issues) |
| **教程** |  |  |
| 《框架搭建 决定版》    | 教程 QFramework  的核心架构是怎么演化过来的？ | [课程主页](https://learn.u3d.cn/tutorial/framework_design)\|[学生课堂笔记1](https://github.com/Haogehaojiu/FrameworkDesign)\|[学生课堂笔记2](https://github.com/Haogehaojiu/ShootingEditor2D) |
| **产品案例** |  |  |
| 独立游戏《鬼山之下》   | 使用 QF 制作的独立游戏           | [游戏主页(Steam)](https://store.steampowered.com/app/1517160/_/) |
| 手机游戏《谐音梗挑战》 | 使用 QF 制作的手机游戏          | [游戏主页(TapTap)](https://www.taptap.com/app/201075)        |
| **官方工具**  |                                                              |                                                              |
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



## Star 趋势（如果项目有帮到您欢迎点赞）

[![Stargazers over time](https://starchart.cc/liangxiegame/QFramework.svg)](https://starchart.cc/liangxiegame/QFramework)

### 核心成员

* [h3166179](https://github.com/h3166179)
* [王二](https://github.com/so-sos-so) [so-sos-so](https://github.com/so-sos-so)

* [凉鞋 liangxieq](https://github.com/liangxieq)




### 优秀的 Unity 库、框架

- [ET](https://github.com/egametang/ET)：ET Unity3D Client And C# Server Framework
- [IFramework（OnClick）](https://github.com/OnClick9927/IFramework) Simple Unity Tools
- [TinaX Framework](https://tinax.corala.space/) “开箱即用”的Unity独立游戏开发工具
- [JEngine](https://github.com/JasonXuDeveloper/JEngine)  一个基于XAsset&ILRuntime，精简好用的热更框架

### 代码规范完全遵循:

[QCSharpStyleGuide](https://github.com/liangxiegame/QCSharpStyleGuide)


### 赞助 Donate:

* 如果觉得不错可以在 [这里 Asset Store](http://u3d.as/SJ9) 给个 5 星哦~ give 5 star
* 或者给此仓库一个小小的 Star~ star this repository
* 以上这些都会转化成我们的动力,提供更好的技术服务! 
