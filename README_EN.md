![LOGO](https://file.liangxiegame.com/67ca2c27-d711-40b2-96f3-d2f6071e3f3c.png)

[![](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/liangxiegame/QFramework/blob/master/LICENSE)
[![Build Status](https://travis-ci.org/liangxiegame/QFramework.svg?branch=master)](https://travis-ci.org/liangxiegame/QFramework)


# QFramework Intro

[中文](README)|[English](README_EN.md)

[QFramework](https://github.com/liangxiegame/QFramework)  is a framework. she support solid pricinple、domain design driven、event-driven、data-driven、layered、mvc、cqrs、modulization、extendable、scalable architecture. Simple but powerful! she only has 800 lines of code.can save to a note-taking app. 

## Architecture diagram

![](http://processon.com/chart_image/5c270aa6e4b007ba5d5029dc.png)

## For Example（😂）

![](https://file.liangxiegame.com/5fcdf6d1-0605-4ae6-b4bf-12e661eb2f1e.png)

## Schematic diagram of various situations

![](http://processon.com/chart_image/5cbb1edce4b0bab90960a4f6.png)

## Architecture Rule

**QFramework System Design Architecture has 4 layers：**

* Presentation Layer：ViewController Layer. Using IController interface，recive input from user and state changed event from model. In unity MonoBehaviour is on presentation layer
    * Can get System
    * Can get Model
    * Can send Command
    * Can listen Event
* System Layer：Using ISystem interface. share IController's part of responsibility. Sharing logic shared across multiple presentation layers，suchas time system、shop system、archivement system.
    * Can get System
    * Can get Model
    * Can listen Event
    * Can send Event
* Model Layer：Using IModel interface.Responsible for data definition, data addition, deletion, query and modification methods.
    * Can get Utility
    * Can send Event
* Utility Layer：Using IUtility interface.Responsible for providing infrastructure, such as storage method, serialization method, network connection method, Bluetooth method, SDK, framework inheritance, etc. Nothing can be done. You can integrate third-party libraries or encapsulate APIs
* In addition to the four layers, there is a core concept - command
    * Can get System
    * Can get Model
    * Can send Event
    * Can send Command
* Layer Rule：
    * IController change ISystem、IModel's state by Command
    * Notify icontroller after the change of ISystem and IModel must use event or bindableproperty
    * IController can get ISystem、IModel for data query
    * ICommand cannot have state
    * The upper layer can directly obtain the lower layer, and the lower layer cannot obtain the upper object
    * Events for lower layer to upper layer communication
    * The communication between the upper layer and the lower layer is called by method (only for query and command for state change). The interaction logic of IController is special, and command can only be used

（照抄自：[学生课堂笔记1](https://github.com/Haogehaojiu/FrameworkDesign)）

### Environment

* Unity 2018.4.x ~ 2021.x

## Install

* QFramework.cs
    * copy [this code](QFramework.cs) to your project

* QFramework.cs With Examples
    * [downlowd unitypackage](./QFramework.cs.Examples.unitypackage)
* QFramework.ToolKits
    * [downlowd unitypackage](./QFramework.Toolkits.unitypackage)
* QFramework.ToolkitsPro
    * install by [Asset Store](http://u3d.as/SJ9) 




## Resources

| **Version**                                 |                                                              |                                                              |
| ------------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| QFramework.cs                               | Implementation of qframework ontology architecture           | [code file](./QFramework.cs)                                 |
| QFramework.cs With Examples                 | QFramework.cs and  Examples：CounterApp、Point Point Point、CubeMaster、FlappyBird、ShootingEditor2D、SnakeGame  etc (QFramework.cs included) | [downlowd unitypackage](./QFramework.cs.Examples.unitypackage) |
| QFramework.ToolKits                         | QFramework.cs  with UIKit/ActionKit/ResKit/PackageKit/AudioKit (QFramework.cs and examples included) | [downlowd unity package](./QFramework.Toolkits.unitypackage) |
| QFramework.Toolkits.Demo.WuZiQi             | Gobang Demo by QFramework.Toolkits（Need Install QFramework.Toolkits） | [download unitypackage](./QFramework.Toolkits.Demo.WuZiQi.unitypackage) |
| QFramework.Toolkits.Demo.Saolei             | Mine clearance Demo by QFramework.Toolkits（Need Install QFramework.Toolkits） | [download unitypackage](./QFramework.Toolkits.Demo.SaoLei.unitypackage) |
| QFramework.ToolKitsPro                      | More Powerful Tools version based on QFramework.ToolKits (QFramework.Toolkits included) | [AssetStore](http://u3d.as/SJ9)                              |
| **Community**                               |                                                              |                                                              |
| github issue                                | github community                                             | [address](https://github.com/liangxiegame/QFramework/issues/new) |
| gitee issue                                 | gitee community                                              | [address](https://gitee.com/liangxiegame/QFramework/issues)  |
| **ShowCase**                                | email me or publish on github's issue. My email: liangxiegame@163.com |                                                              |
| 《When The Train Buzzes For Three Seconds》 |                                                              | [Steam](https://store.steampowered.com/app/1563700/_/)\|[TapTap](https://www.taptap.cn/app/208258) |
| 《The First Mountain》                      |                                                              | [Steam](https://store.steampowered.com/app/2149980/The_First_Mountain/) |
| 《Hi Eggplant》                             |                                                              | [Steam](https://store.steampowered.com/app/2091640/Hi_Eggplant/) |
| 《Under The Ghost Mountain》                |                                                              | [Steam](https://store.steampowered.com/app/1517160/_/)       |
|                                             |                                                              |                                                              |
| **Official Toolkits**                       |                                                              |                                                              |
| SingletonKit                                |                                                              | [github](https://github.com/liangxiegame/SingletonKit)\|[gitee](https://gitee.com/liangxiegame/SingletonKit) |
| ExtensionKit                                |                                                              | [github](https://github.com/liangxiegame/ExtensionKit)\|[gitee](https://gitee.com/liangxiegame/ExtensionKit) |
| IOCKit                                      |                                                              | [github](https://github.com/liangxiegame/IOCKit)\|[gitee](https://gitee.com/liangxiegame/IOCKit) |
| TableKit                                    |                                                              | [github](https://github.com/liangxiegame/TableKit)\|[gitee](https://gitee.com/liangxiegame/TableKit) |
| PoolKit                                     |                                                              | [github](https://github.com/liangxiegame/PoolKit)\|[gitee](https://gitee.com/liangxiegame/PoolKit) |
| LogKit                                      |                                                              | [github](https://github.com/liangxiegame/LogKit)\|[gitee](https://gitee.com/liangxiegame/LogKit) |
| ActionKit                                   |                                                              | [github](https://github.com/liangxiegame/ActionKit)\|[gitee](https://gitee.com/liangxiegame/ActionKit) |
| ResKit                                      |                                                              | [github](https://github.com/liangxiegame/ResKit)\|[gitee](https://gitee.com/liangxiegame/ResKit) |
| UIKit                                       |                                                              | [github](https://github.com/liangxiegame/UIKit)\|[gitee](https://gitee.com/liangxiegame/UIKit) |
| AudioKit                                    |                                                              | [github](https://github.com/liangxiegame/AudioKit)\|[gitee](https://gitee.com/liangxiegame/AudioKit) |
| PackageKit                                  |                                                              | [github](https://github.com/liangxiegame/PackageKit)\|[gitee](https://gitee.com/liangxiegame/PackageKit) |
|                                             |                                                              |                                                              |
|                                             |                                                              |                                                              |



## Star Trends

[![Stargazers over time](https://starchart.cc/liangxiegame/QFramework.svg)](https://starchart.cc/liangxiegame/QFramework)

### Contributors

* [蓝色孤舟 gdtdftdqtd](https://github.com/gdtdftdqtd)
* [h3166179](https://github.com/h3166179)
* [葫芦 WangEdgar](https://github.com/WangEdgar)
* [凉鞋 liangxiegame](https://github.com/liangxiegame)




### Other Awesome Framework

- [ET](https://github.com/egametang/ET)：ET Unity3D Client And C# Server Framework
- [IFramework（OnClick）](https://github.com/OnClick9927/IFramework) Simple Unity Tools
- [JEngine](https://github.com/JasonXuDeveloper/JEngine)  The solution that allows unity games update in runtime.
- [TinaX Framework](https://tinax.corala.space/) “开箱即用”的Unity独立游戏开发工具

### Code Style:

[QCSharpStyleGuide](https://github.com/liangxiegame/QCSharpStyleGuide)


### Donate:

* 如果觉得不错可以在 [Asset Store](http://u3d.as/SJ9) 给个 5 星哦~ give 5 star
* 或者给此仓库一个小小的  Star~ star this repository
* 以上这些都会转化成我们的动力,提供更好的技术服务! 