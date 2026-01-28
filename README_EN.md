![LOGO](LOGO.png)

[![](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/liangxiegame/QFramework/blob/master/LICENSE)


# QFramework Intro

[中文](README)|[English](README_EN.md)

[QFramework](https://github.com/liangxiegame/QFramework)  is a framework. she support solid pricinple、domain design driven、event-driven、data-driven、layered、mvc、cqrs、modulization、extendable、scalable architecture. Simple but powerful! she only has 1000 lines of code.can save to a note-taking app. 

## Architecture diagram

![image.png](https://file.liangxiegame.com/5e9f1682-1907-47a2-a23a-2d5a4ba2e7a4.png)

## For Example

![](https://file.liangxiegame.com/dd678daa-6bca-46ea-8d8e-adcb5208ddfb.png)

## Schematic diagram of various situations

![image-20260124142617406](https://file.liangxiegame.com/9fbc9dc9-cbac-49ba-b6f8-328c2e063d9c.png)

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
| github issue                                | github community                                             | [link](https://github.com/liangxiegame/QFramework/issues/new) |
| gitee issue                                 | gitee community                                              | [link](https://gitee.com/liangxiegame/QFramework/issues)     |
| discord                                     |                                                              | [link](https://discord.gg/PHqHX5v5SE)                        |
| **ShowCase**                                | email me or publish on github's issue. My email: liangxiegame@163.com |                                                              |
| 《Fools, Maniacs and Liars》                |                                                              | [Steam](https://store.steampowered.com/app/1741170/_/)       |
| 《Skulker》                                 |                                                              | [Steam](https://store.steampowered.com/app/1731000/)         |
| 《ScaleBox》                                |                                                              | [Steam](https://store.steampowered.com/app/3528380/ScaleBox/) |
| 《MyRose》                                  |                                                              | [Steam](https://store.steampowered.com/app/3246640/_/)       |
| 《蚀界档案》                                |                                                              | [TapTap](https://www.taptap.cn/app/222365) \|[Steam](https://store.steampowered.com/app/1976540/_/) |
| 《Box Bakery》                              |                                                              | [Steam](https://store.steampowered.com/app/2942950/Box_Bakery/) |
| 《X-teroids》                               |                                                              | [Steam](https://store.steampowered.com/app/3342540/Xteroids/) |
| 《The last day of Han dynasty》             |                                                              | [Steam](https://store.steampowered.com/app/2078910/_/)       |
| 《Hi Eggplant：The Birth Of Sprites》       |                                                              | [Steam](https://store.steampowered.com/app/2375290/_/)       |
| 《When The Train Buzzes For Three Seconds》 |                                                              | [Steam](https://store.steampowered.com/app/1563700/_/)\|[TapTap](https://www.taptap.cn/app/208258) |
| 《Hi Eggplant》                             |                                                              | [Steam](https://store.steampowered.com/app/2091640/Hi_Eggplant/) |
| 《The First Mountain》                      |                                                              | [Steam](https://store.steampowered.com/app/2149980/The_First_Mountain/) |
| 《Hi Eggplant》                             |                                                              | [Steam](https://store.steampowered.com/app/2091640/Hi_Eggplant/) |
| 《Under The Ghost Mountain》                |                                                              | [Steam](https://store.steampowered.com/app/1517160/_/)       |



## Star Trends

[![Stargazers over time](https://starchart.cc/liangxiegame/QFramework.svg)](https://starchart.cc/liangxiegame/QFramework)


### Author

* [凉鞋 liangxiegame](https://github.com/liangxiegame)

### Contributors

* [京产肠饭]( https://gitee.com/JingChanChangFan/hk_-unity-tools)

* [猫叔(一只皮皮虾)]( https://space.bilibili.com/656352/)

* [TastSong]( https://github.com/TastSong)

* [misakiMeiii](https://github.com/misakiMeiii)

* [soso](https://github.com/so-sos-so)

* [蓝色孤舟 gdtdftdqtd](https://github.com/gdtdftdqtd)

* [h3166179](https://github.com/h3166179)

* [葫芦 WangEdgar](https://github.com/WangEdgar)

* New一天

* 幽飞冷凝雪～冷


### Other Awesome Framework

- [ET](https://github.com/egametang/ET)：ET Unity3D Client And C# Server Framework
- [JEngine](https://github.com/JasonXuDeveloper/JEngine)  The solution that allows unity games update in runtime.
- [TinaX Framework](https://tinax.corala.space/) “开箱即用”的Unity独立游戏开发工具

### Code Style:

[QCSharpStyleGuide](https://github.com/liangxiegame/QCSharpStyleGuide)


### Donate:

* 如果觉得不错可以在 [Asset Store](http://u3d.as/SJ9) 给个 5 星哦~ give 5 star
* 或者给此仓库一个小小的  Star~ star this repository
* 以上这些都会转化成我们的动力,提供更好的技术服务! 

### Credits:

Thanks for Licenses Supporting by JetBrains Company

<p><a href="https://www.jetbrains.com/?from=QFramework ">
<img src="https://file.liangxiegame.com/2bf40802-c296-4bdc-bc8a-718000503771.png" alt="JetBrains的Logo" width="20%" height="20%"></a></p>