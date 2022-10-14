# 1. 介绍
# 01. 简介

大家好，我是 QFramework 的作者 凉鞋，QFramework 从第一次代码提交到现在快 7 年了（2015 年 12 月 \~ 2022 年 10 月）了，而经过了 7 年时间的打磨，我们终于迎来了 v1.0 版本。

此教程，将收录于 QFramework 的官方文档，发布于 qframework.cn，同时也会包含在 QFramework.Toolkits 的编辑器内置文档中。

## QFramework 简介

QFramework 是一套渐进式、快速开发框架，适用于任何类型的游戏及应用项目。

QFramework 包含一套 开发架构 和 大量的工具集。

QFramework 特性速览：

*   开发架构（QFramework.cs）
  *   简单、易上手、强大
  *   MVC
  *   IOC、分层支持
  *   CQRS 支持
  *   符合 SOLID原则
  *   可以使用 DDD 的方式设计项目
  *   不到 1000 行代码
*   工具集（QFramework.Toolkits）
  *   UIKit 界面\&View快速开发&管理解决方案
    *   UI、GameObject 的代码生成&自动赋值
    *   界面管理
    *   层级管理
    *   界面堆栈
    *   默认使用 ResKit 方式管理界面资源
    *   可自定义界面的加载、卸载方式
    *   Manager Of Manager 架构集成（不推荐使用）
  *   ResKit 资源快速开发&管理解决方案
    *   AssetBundle 提供模拟模式，开发阶段无需打包即可加载资源
    *   资源名称代码生成支持
    *   同一个 API 可加载 AssetBundle、Resources、网络 和 自定义来源的资源
    *   提供一套引用计数的资源管理模型
  *   AudioKit 音频管理解决方案
    *   提供背景音乐、人声、音效 三种音频播放 API
    *   音量控制
    *   默认使用 ResKit 方式管理音频资源
    *   可自定义音频的加载、卸载方式
  *   CoreKit 提供大量的代码工具
    *   ActionKit：动作序列执行系统
    *   CodeGenKit：代码生成 & 自动序列化赋值工具
    *   EventKit：提供基于类、字符串、枚举以及信号类型的事件工具集
    *   FluentAPI：对大量的 Unity 和 C# 常用的 API 提供了静态扩展的封装（链式 API）
    *   IOCKit：提供依赖注入容器
    *   LocaleKit：本地化&多语言工具集
    *   LogKit：日志工具集
    *   PackageKit：包管理工具，由此可更新框架和对应的插件模块。
    *   PoolKit：对象池工具集，提供对象池的基础上，也提供 ListPool 和 Dictionary Pool 等工具。
    *   SingletonKit：单例工具集
    *   TableKit：提供表格类数据结构的工具集

QFramework 的设计哲学是从每个细节上提升开发效率。

同时 QFramework 还包含丰富的生态。

*QFrameowrk.Toolkits 内置编辑器*

![image.png](https://file.liangxiegame.com/d15a75ba-8d6d-4d77-b096-a93c559d29b9.png)

*资源*

| **版本**                          |                                                                                                   |                                                                                                                                                                                                                            |
| ------------------------------- | ------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| QFramework.cs                   | QFramework 本体架构的实现                                                                                |                                                                                                                                                                                                                            |
| QFramework.cs  示例               | QFramework.cs 与官方示例： CounterApp、《点点点》、FlappyBird、CubeMaster、ShootingEditor2D、贪吃蛇等                 | [github](https://github.com/liangxiegame/QFramework/blob/master/QFramework.cs.Examples.unitypackage)\|[gitee](https://gitee.com/liangxiegame/QFramework/blob/master/QFramework.cs.Examples.unitypackage)                   |
| QFramework.Toolkits             | QFramework  集成 CoreKit/UIKit/ActionKit/ResKit/PackageKit/AudioKit 等全部官方工具（已包含 QFramework.cs 和 示例) | [github](https://github.com/liangxiegame/QFramework/blob/master/QFramework.Toolkits.unitypackage)\|[gitee](https://gitee.com/liangxiegame/QFramework/blob/master/QFramework.Toolkits.unitypackage)                         |
| QFramework.Toolkits.Demo.WuZiQi | 使用 QFramework.Toolkits 开发的五子棋 Demo（需要安装好  QFramework.Toolkits）                                    | [github](https://github.com/liangxiegame/QFramework/blob/master/QFramework.Toolkits.Demo.WuZiQi.unitypackage)\|[gitee](https://gitee.com/liangxiegame/QFramework/blob/master/QFramework.Toolkits.Demo.WuZiQi.unitypackage) |
| QFramework.Toolkits.Demo.Saolei | 使用 QFramework.Toolkits 开发的扫雷 Demo（需要安装好  QFramework.Toolkits）                                     | [github](https://github.com/liangxiegame/QFramework/blob/master/QFramework.Toolkits.Demo.SaoLei.unitypackage)\|[gitee](https://gitee.com/liangxiegame/QFramework/blob/master/QFramework.Toolkits.Demo.SaoLei.unitypackage) |
| QFramework.ToolKitsPro          | 在 ToolKits 基础上集成更多好用的工具的版本（已包含 QFramework.Toolkits）                                               | [AssetStore](http://u3d.as/SJ9)                                                                                                                                                                                            |
| **群友案例**                        |                                                                                                   |                                                                                                                                                                                                                            |
| 赛车游戏《Crazy Car》                 | 群友 [TastSong](https://github.com/TastSong) 使用 QF 进行重构的开源赛车游戏                                      | [游戏主页(Github](https://github.com/TastSong/CrazyCar))                                                                                                                                                                       |
| **社区**                          |                                                                                                   |                                                                                                                                                                                                                            |
| QQ 群:623597263                  | 交流群                                                                                               | [点击加群](http://shang.qq.com/wpa/qunwpa?idkey=706b8eef0fff3fe4be9ce27c8702ad7d8cc1bceabe3b7c0430ec9559b3a9ce66)                                                                                                              |
| github issue                    | github 社区                                                                                         | [地址](https://github.com/liangxiegame/QFramework/issues/new)                                                                                                                                                                |
| gitee issue                     | gitee 社区（国内访问快）                                                                                   | [地址](https://gitee.com/liangxiegame/QFramework/issues)                                                                                                                                                                     |
| **教程**                          |                                                                                                   |                                                                                                                                                                                                                            |
| 《框架搭建 决定版》                      | 教程 QFramework  的核心架构是怎么演化过来的？                                                                     | [课程主页](https://learn.u3d.cn/tutorial/framework_design)\|[学生课堂笔记1](https://github.com/Haogehaojiu/FrameworkDesign)\|[学生课堂笔记2](https://github.com/Haogehaojiu/ShootingEditor2D)                                              |
| **产品案例**                        |                                                                                                   |                                                                                                                                                                                                                            |
| 独立游戏《鬼山之下》                      | 使用 QF 制作的独立游戏                                                                                     | [游戏主页(Steam)](https://store.steampowered.com/app/1517160/_/)                                                                                                                                                               |
| 手机游戏《谐音梗挑战》                     | 使用 QF 制作的手机游戏                                                                                     | [游戏主页(TapTap)](https://www.taptap.com/app/201075)                                                                                                                                                                          |
| 独立游戏《推灭泡泡姆》                     | ‍QF 群友，大学生团队制作的独立游戏，终于等到上架啦，亲自游玩过，很好玩，大家多多支~（P.S 使用 QF.cs 作为架构开发的哦~）                              | [游戏主页(TapTap)](https://www.taptap.com/app/233228)                                                                                                                                                                          |
| **官方工具**（独立版本，不互相依赖)            |                                                                                                   |                                                                                                                                                                                                                            |
| SingletonKit                    | 易上手功能强大的单例工具，由 QF 官方维护                                                                            | [github](https://github.com/liangxiegame/SingletonKit)\|[gitee](https://gitee.com/liangxiegame/SingletonKit)                                                                                                               |
| ExtensionKit                    | 易上手功能强大的 C#/UnityAPI 的静态扩展 ，由 QF 官方维护                                                             | [github](https://github.com/liangxiegame/ExtensionKit)\|[gitee](https://gitee.com/liangxiegame/ExtensionKit)                                                                                                               |
| IOCKit                          | 易上手功能强大的 IOC 容器 ，由 QF 官方维护                                                                        | [github](https://github.com/liangxiegame/IOCKit)\|[gitee](https://gitee.com/liangxiegame/IOCKit)                                                                                                                           |
| TableKit                        | 一套类似表格的数据结构（List\<List\<T>>)，兼顾查询效率和联合强大的查询功能，由 QF 官方维护                                           | [github](https://github.com/liangxiegame/TableKit)\|[gitee](https://gitee.com/liangxiegame/TableKit)                                                                                                                       |
| PoolKit                         | 对象池工具，由 QF 官方维护                                                                                   | [github](https://github.com/liangxiegame/PoolKit)\|[gitee](https://gitee.com/liangxiegame/PoolKit)                                                                                                                         |
| LogKit                          | 日志工具，由 QF 官方维护                                                                                    | [github](https://github.com/liangxiegame/LogKit)\|[gitee](https://gitee.com/liangxiegame/LogKit)                                                                                                                           |
| ActionKit                       | 动作序列工具，由 QF 官方维护                                                                                  | [github](https://github.com/liangxiegame/ActionKit)\|[gitee](https://gitee.com/liangxiegame/ActionKit)                                                                                                                     |
| ResKit                          | 资源管理工具，由 QF 官方维护                                                                                  | [github](https://github.com/liangxiegame/ResKit)\|[gitee](https://gitee.com/liangxiegame/ResKit)                                                                                                                           |
| UIKit                           | UIKit 是一套 UI/View 开发解决方案，由 QF 官方维护                                                                | [github](https://github.com/liangxiegame/UIKit)\|[gitee](https://gitee.com/liangxiegame/UIKit)                                                                                                                             |
| AudioKit                        | 一套音频管理工具，由 QF 官方维护                                                                                | [github](https://github.com/liangxiegame/AudioKit)\|[gitee](https://gitee.com/liangxiegame/AudioKit)                                                                                                                       |
| PackageKit                      | 一套包管理工具，可以通过 PackageKit 安装旧版本的 QFramework，以及大量的解决方案。                                              | [github](https://github.com/liangxiegame/PackageKit)\|[gitee](https://gitee.com/liangxiegame/PackageKit)                                                                                                                   |
| **其他相关教程**                      |                                                                                                   |                                                                                                                                                                                                                            |
| 《独立游戏体验计划》（猫叔）                  | 独立游戏制作体验教程，有用到 QFramework.cs                                                                      | [b 站](https://space.bilibili.com/656352)                                                                                                                                                                                   |
| 《原创独立游戏制作》（凉鞋）                  | 原创独立游戏制作教程，有用到 QFramework.cs                                                                      | [b 站](https://space.bilibili.com/60450548/channel/collectiondetail?sid=125221)                                                                                                                                             |

**典型的 QFramework.cs 架构代码**

```csharp
namespace QFramework.Exmaple
{
    public class CounterAppController : MonoBehaviour , IController
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // Model
        private ICounterAppModel mModel;

        void Start()
        {
            // 获取模型
            mModel = this.GetModel<ICounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<IncreaseCountCommand>();
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand(new DecreaseCountCommand(/* 这里可以传参（如果有） */));
            });

            // 表现逻辑
            mModel.Count.RegisterWithInitValue(newCount => // -+
            {
                UpdateView();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
          
            mModel = null;
        }
    }
}

```

**典型的 QFramework.Toolkits 代码**

```csharp
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace liangxiegame
{
    public partial class UIGamePanel : UIPanel
    {
        private ResLoader mResLoader;
        
        protected override void OnInit(IUIData uiData = null)
        {
            mResLoader = ResLoader.Allocate();
            
            mResLoader.LoadSync<GameObject>("GameplayRoot")
                .Instantiate()
                .Identity()
                .GetComponent<GameplayRoot>()
                .InitGameplayRoot();
            
            
            BtnPause.onClick.AddListener(() =>
            {
                AudioKit.PlaySound("btn_click");
                
                ActionKit.Sequence()
                    .Callback(() => BtnPause.interactable = false)
                    .Callback(() => BtnPause.PlayBtnFadeAnimation())
                    .Delay(0.3f)
                    .Callback(() => UIKit.OpenPanel<UIPausePanel>())
                    .Start(this);
            });
        }

        protected override void OnClose()
        {
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }
    }
}
```

## 大量的示例

### 小游戏《点点点》

![b5966b31-f004-4b5f-a38d-25753fb2eb8f.gif](https://file.liangxiegame.com/5a10aa95-4c93-4dae-acec-667a113c30ca.gif)

### 小游戏《FlappyBird》

![430b7f31-508d-4569-aa51-b75d5553b8c4.gif](https://file.liangxiegame.com/9845122b-93d9-4106-a027-2d7c129a096a.gif)

作者：王二 soso <https://github.com/so-sos-so>

### 小游戏《Cube Master》

![b1334ef2-f6d4-4a9c-a5c4-b6cd6508595c.gif](https://file.liangxiegame.com/f51abab0-9dc9-478b-b1f1-67f2cd588477.gif) 作者：王二 soso <https://github.com/so-sos-so>

### 简易关卡编辑器2D

![c57c20cf-5ee6-4346-8be8-8ad1ea2d63b9.gif](https://file.liangxiegame.com/6492498b-6c22-478d-8785-9f43453c34db.gif)

![ea2cb545-4b5b-4d02-b494-dde4afa4e190.gif](https://file.liangxiegame.com/34b775c6-6a49-4141-9b9a-1377a6c15673.gif)

### 小游戏《贪吃蛇》

![fb907355-c06c-4bde-8ca3-5638ba9b3ef7.gif](https://file.liangxiegame.com/ac70d14e-ea89-445d-899e-06f18f11f8d1.gif)

作者：一只皮皮虾 <https://gitee.com/PantyNeko/>

以上的示例都是由 QFramework.cs 制作而成的官方示例。

另外还有群友制作的开源游戏

### CrazyCar

Unity制作的联机赛车游戏，后台为SpringBoot + Mybatis；游戏采用QFramework框架，支持KCP和WebSocket网络(商用级)

![Login.jpg](https://file.liangxiegame.com/0ab6cb1d-2374-4aa2-b27d-f04eb72792cd.png)

![Setting.png](https://file.liangxiegame.com/a113dcba-9ba8-4a40-b000-be3b61719ecc.png)

![Homepage.png](https://file.liangxiegame.com/9075c10d-6d21-411c-b1a4-7f92a08f9bfa.png)

![Avatar.png](https://file.liangxiegame.com/32b48b5b-cdcc-433e-b1b2-4b1333211a70.png) ![Profile.png](https://file.liangxiegame.com/bda476e4-0ede-4fd9-a5bb-e993bce8a786.png)

![Equip.png](https://file.liangxiegame.com/158b0ce0-6e67-47c5-81b5-cee6388dd99c.png)

![Rank.png](https://file.liangxiegame.com/2bd0ef1f-d639-48e8-8c48-320995d20de4.png)

![TimeTrial.png](https://file.liangxiegame.com/aa337718-b868-41d2-bc6b-2ef51c157481.png) ![Match.png](https://file.liangxiegame.com/06157781-3271-438c-bf3f-613e6ec00fb0.png)

作者: TastSone  <https://github.com/TastSong>

项目地址: <https://github.com/TastSong/CrazyCar>

## 案例《五子棋》

![2f4dacbd-e59b-43af-b7be-44220fac664e.png](https://file.liangxiegame.com/a76bc24a-1828-46f2-94c5-8bd24884f932.png)

源码地址:

*   github <https://github.com/liangxiegame/QFramework>
*   gitee <https://gitee.com/liangxiegame/QFramework>

![image.png](https://file.liangxiegame.com/3abceb70-2d17-4457-aff1-ef8a6ef4bd66.png)

## 案例《扫雷》

作者：Joker

![172348\_4d54744e\_5161625.webp](https://file.liangxiegame.com/c6116b7e-a08b-4c13-ae64-7053be3c503c.png)

源码地址:

*   github <https://github.com/liangxiegame/QFramework>
*   gitee <https://gitee.com/liangxiegame/QFramework>

![image.png](https://file.liangxiegame.com/6482d4eb-5af9-4932-a2f8-2164cb22e931.png)

## 本教程简介

在上一版官方教程《QFramework 使用指南 2020》写完之后，经过两年（2022 年），QFramework 改进了很多工具的使用体验，同时又新增了一套非常简单且强大的开发架构，这样就迎来了 QFramework 第一个正式版本 QFramework v1，这样就导致导致 QFramework 的推荐使用的 API 发生了一些变化，虽然旧版本的 API 还能用，但是按照《QFramework 使用指南 2020》写的很多代码会报很多警告，这会让很多初学者感到疑惑，所以笔者打算在《QFramework 使用指南 2020》的基础上，重制一套新的 QFramework 使用教程，名字叫做《QFramework v1.0 使用指南》。

教程分为架构篇和工具集篇，架构篇着重介绍 QFramework.cs 这套架构入门以及使用规范，工具篇着重介绍 QFramework 中的大量的工具集的使用。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 02.Roadmap-路线图

**将来也许**
* sLua、toLua、xLua、ILRuntime 支持
* Pro 版本推进
  * Architecture Designer 使用可视化设计架构，一键生成架构代码
* ResKit 支持自定义目录方案
* UIKit 支持多个 Canvas Root 和 摄像机方案
* CodeGenKit 同步
* 插件平台的插件整理
* ResKit 重构（支持热更）
* UIKit 重构
* AudioKit 重构

**v1.0.x**（当前）
* QFramework.Toolkits 收录四个示例
* 编辑器使用体验改进
* QFramework Pro v0.5 Architecture Designer 发布

**v0.16.x**（已完成）
*《QFramework v1.0 使用指南》 完成
* 编辑器文档 支持 gif 动画 和 简单的 C# 代码高亮
* 示例完善

**v0.15.x**（已完成）
* 文档整理 & 在编辑器内部内置

**v0.14.x（已完成）**
* Asset Store 兼容 & 减少第三方依赖

**v0.11.x（已完成）**
* 打 dll 优化旧设备的编译速度

**v0.10.x（已完成）**
* ILRuntime 支持（只完成一部分，后续再支持）

**v0.9.x（已完成）**
* 单元测试覆盖
* PackageKit、Framework、Extensions 的示例全部覆盖
* 3 ~ 5 个 Demo 发布

**v0.2.x ~ v0.8.x（已完成）**
* PackageManager 独立成 PackageKit
* 剥离掉第三方插件，最为扩展插件支持
* 插件平台发布：https://liangxiegame.com/qf/package
* 命名空间从 QF 改回 QFramework
* 大量 Bug 修复、大量示例编写
* 五子棋 Demo 发布：Demo：五子棋
* QFramework 使用指南 2020 完结：QFramework 使用指南 2020

**v0.1.x（已完成)**
* UniRx、Zenject、uFrame、JsonDotnet、CatLib 集成和增强
* IOC 增加 IOC 部分
* 框架自动更新机制 => PackageManager
* 命名空间从 QFramework 改成 QF

**v0.0.x（已完成**
* 框架搭建 2017 的工具集收录
* 框架搭建 2018 的 ResKit 和 UI Kit 模块实现
* ActionKit 模块实现
* Manager Of Managers 支持
* 框架自动更新机制

* Pro 版本（开源收费版本）
  * CoreKitPro（未开始）
  * LuaKit-轻量级 Lua 脚本方案，由 MoonSharp 魔改而来(未开始)
  * CodeGenKitPro 代码生成库
  * ActionKitPro(未开始)
  * ResKitPro(未开始)
  * UIKitPro
  * LuaKitPro（基于 xLua)(未开始)
  * ILRuntimeKitPro（基于 ILRuntime 的热更框架）(未开始)
  * DocKit-可视化编程写文档，一键生成类图功能（进行中）
  * DialogueKit-对话编辑器（未开始）
  * InventoryKit-背包系统（未开始）
  * ArchitectureKit-架构设计器（未开始）
  * QuestKit-任务编辑器（未开始）
  * 存档-未开始）


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 2. 架构篇：QFramework.cs
# 01. QFramework 架构简介

QFramework 架构是一套简单、强大、易上手的系统设计架构。

这套架构的特性如下：
* 基于 MVC
* 分层
* (可选)CQRS 支持
* (可选)事件驱动
* (可选)数据驱动
* (可选)IOC 模块化
* (可选)领域驱动设计（DDD）支持
* 符合 SOLID 原则
* 源码不到 1000 行

## 提供的架构图
![image.png](https://file.liangxiegame.com/5e9f1682-1907-47a2-a23a-2d5a4ba2e7a4.png)
## 举个例子（一图胜千言😂）
![](https://file.liangxiegame.com/6bf42306-0b2a-4417-bbcf-354af0132596.png)

这两张图现在大家可能还看不太懂，没关系，我们过一遍快速入门就懂了。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 02. QFramework 的 MVC
QFramework 基于 MVC 的开发模式

所以我们先从最熟知的 MVC 架构开始着手 QFramework 的学习。

我们先做一个非常简单的计数器应用。


首先我们使用 UGUI 创建一个最简单的界面，如下图所示：

![image.png](https://file.liangxiegame.com/902ef543-64b1-45ad-ae45-ea180ebec133.png)

场景结构如下所示：

![image.png](https://file.liangxiegame.com/1db2c50c-cb8b-4b1c-92f6-432ff4083105.png)

复制完之后，我们创建一个脚本叫做 CounterAppController，代码如下：

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    // Controller
    public class CounterAppController : MonoBehaviour
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // Model
        private int mCount = 0;

        void Start()
        {
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                mCount++;
                // 表现逻辑
                UpdateView();        
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                mCount--;
                // 表现逻辑
                UpdateView();
            });
            
            UpdateView();
        }
        
        void UpdateView()
        {
            mCountText.text = mCount.ToString();
        }
    }
}
```

代码很简单，这是一个非常简易的 MVC 的实现。

我们将此脚本挂在 Canvas 节点上，运行 Unity 结果如下：


![282fcc3c-96fa-46e1-b4c6-7f4528b04271.gif](https://file.liangxiegame.com/1b934e4f-8f72-44c2-800a-a97f1e707950.gif)

非常简单。

此时我们还没有导入我们的 QFramework，不着急，我们先看看代码中所介绍的概念。

首先是 Model、View、Controller

Model 的代码如下:
```csharp
// Model
private int mCount = 0;
```

非常简单，只有一个成员变量，但是在这里它其实并不算是一个 Model，他只是要在 View 中显示的一个数据而已，具体为什么不是 Model 我们在后边再说。

View 的代码如下:
```csharp
// View
private Button mBtnAdd;
private Button mBtnSub;
private Text mCountText;
```

View 的代码也很简单，View 在 QFramework 的 MVC 定义里就是提供关键组件的引用，比如这三个组件是要在 Controller 代码里要用到的。而其他的例如 Canvas Scaler 等这些组件目前 Controller 不需要，所以就不用声明。

Controller 的代码，如下：

```csharp
void Start()
{
    ...
      
    // 监听输入
    mBtnAdd.onClick.AddListener(() =>
    {
        // 交互逻辑
        mCount++;
        // 表现逻辑
        UpdateView();        
    });
            
    mBtnSub.onClick.AddListener(() =>
    {
        // 交互逻辑
        mCount--;
        // 表现逻辑
        UpdateView();
    });
            
    UpdateView();
}
        
void UpdateView()
{
    mCountText.text = mCount.ToString();
}
```

以上就是 Controller 的代码。

好了，我们回头再看下完整代码。


```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    // Controller
    public class CounterAppController : MonoBehaviour
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // Model
        private int mCount = 0;

        void Start()
        {
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                mCount++;
                // 表现逻辑
                UpdateView();        
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                mCount--;
                // 表现逻辑
                UpdateView();
            });
            
            UpdateView();
        }
        
        void UpdateView()
        {
            mCountText.text = mCount.ToString();
        }
    }
}
```

目前像计数器这样的逻辑，以上的代码完全没有问题。

但是我们要用发展的眼光看待问题。

假如这是一个初创项目，那么接下来很有可能需要添加大量的业务逻辑。

其中很有可能让 mCount 在多个 Controller 中使用，甚至需要针对 mCount 这个数据写一些其他逻辑，比如增加 mCount 则增加 5 个分数，或者 mCount 需要存储等，总之 mCount 在未来可能会发展成一个需要共享的数据，而 mCount 目前只属于 CounterAppController，显然在未来这是不够用的。

我们就需要让 mCount 成员变量变成一个共享的数据，最快的做法是吧 mCount 变量变成静态变量或者单例，但是这样虽然写起来很快，但是在后期维护额度时候会产生很多的问题。

而 QFramework 架构提供了 Model 的概念。

我们来使用一下。

我们先导入 QFramework 架构。

导入 QFramework 的方式非常简单，只需要复制 QFramework.cs 的代码到 Unity 工程中即可。

QFramework.cs 地址：
* Gitee: https://gitee.com/liangxiegame/QFramework/blob/master/QFramework.cs
* Github: https://github.com/liangxiegame/QFramework/blob/master/QFramework.cs

导入之后，我们将 CounterAppController 的代码改成如下：

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public class CounterAppModel : AbstractModel
    {
        public int Count;
        
        protected override void OnInit()
        {
            Count = 0;
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 Model
            this.RegisterModel(new CounterAppModel());
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private CounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<CounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 6. 交互逻辑
                mModel.Count++;
                // 表现逻辑
                UpdateView();        
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 7. 交互逻辑
                mModel.Count--;
                // 表现逻辑
                UpdateView();
            });
            
            UpdateView();
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.指定架构
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}
```

好了，代码引入了两个新的概念，一个是 Architecture，另一个是 Model。

Architecture 用于管理模块，或者说 Architecture 提供一整套架构的解决方案，而模块管理和提供 MVC 只是其功能的一小部分。

我们运行一下 Unity 结果如下：

![282fcc3c-96fa-46e1-b4c6-7f4528b04271.gif](https://file.liangxiegame.com/aa28ef15-11e9-49f2-9536-9db18b025a8f.gif)

运行正确。

好了，我们上手了 QFramework 提供的 MVC 架构。

这里要注意一点，Model 的引入是为了解决数据共享的问题，而不是说单只是为了让数据和表现分离，这一点是非常重要的一点。

数据共享分两种：空间上的共享和时间上的共享。

空间的共享很简单，就是多个点的代码需要访问 Model 里的数据。

时间上的共享就是存储功能，将上一次关闭 App 之前的数据存储到一个文件里，这次打开时获得上次关闭 App 之前的数据。

虽然我们上手了 MVC，但是这样的 MVC 还有很多问题，我们下一篇继续解决。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)




# 03. 引入 Command

我们回顾一下目前的代码，如下；
```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public class CounterAppModel : AbstractModel
    {
        public int Count;
        
        protected override void OnInit()
        {
            Count = 0;
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 Model
            this.RegisterModel(new CounterAppModel());
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private CounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<CounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 6. 交互逻辑
                mModel.Count++;
                // 表现逻辑
                UpdateView();        
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 7. 交互逻辑
                mModel.Count--;
                // 表现逻辑
                UpdateView();
            });
            
            UpdateView();
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}
```

现在，数据共享的问题通过 引入 Model 解决了。

这里再次强调一下，需要共享的数据放 Model 里，不需要共享的，能不放就不放。

虽然引入了 Model，但是这套代码随着项目规模的发展还是有很多的问题。

其中最严重也最常见的就是 Controller 会越来越臃肿。

我们简单分析一下为什么 Controller 会越来越臃肿，我们先看下监听用户输入部分的代码，如下：

```csharp
// 监听输入
mBtnAdd.onClick.AddListener(() =>
{
    // 交互逻辑
    mModel.Count++;
    // 表现逻辑
    UpdateView();        
});
            
mBtnSub.onClick.AddListener(() =>
{
    // 交互逻辑
    mModel.Count--;
    // 表现逻辑
    UpdateView();
});
```

在处理用户输入的代码中，笔者写了注释，交互逻辑 和 表现逻辑。

什么是交互逻辑 和 表现逻辑？

非常简单。

交互逻辑，就是从用户输入开始到数据变更的逻辑

顺序是 View->Controller->Model

表现逻辑，就是数据变更到在界面显示的逻辑

顺序是 Model->Controller->View

如下图所示：

![image.png](https://file.liangxiegame.com/0b4e1255-ee5d-4223-97a6-2e49cf68715c.png)

虽然交互逻辑和表现逻辑理解起来简单，但是它们非常重要，因为 QFramework 接下来的概念都是围绕这两个概念展开的。

View、Model 以及 Controller 的交互逻辑和表现逻辑形成了一个闭环。构成了完整的 MVC 闭环。


而 Controller 本身之所以臃肿，是因为，它负责了两种职责，即改变 Model 数据 的交互逻辑，以及 Model 数据变更之后更新到界面的表现逻辑。

而在一个有一定规模的项目中，表现逻辑和交互逻辑非常多。而一个 Controller 很容易就做到上千行代码。

而大部分的 MVC 方案，解决 Controller 臃肿用的是引入 Command 的方式，即引入命令模式，通过命令来分担 Controller 的交互逻辑的职责。

QFramework 也是使用了同样的方式解决 Controller 臃肿的问题。


我们将代码改成如下：

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public class CounterAppModel : AbstractModel
    {
        public int Count;
        
        protected override void OnInit()
        {
            Count = 0;
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 Model
            this.RegisterModel(new CounterAppModel());
        }
    }
    
    // 引入 Command
    public class IncreaseCountCommand : AbstractCommand // ++
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count++;
        }
    }
    
    public class DecreaseCountCommand : AbstractCommand // ++
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count--;
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private CounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<CounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<IncreaseCountCommand>();
                // 表现逻辑
                UpdateView();        
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<DecreaseCountCommand>();
                // 表现逻辑
                UpdateView();
            });
            
            UpdateView();
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}
```

代码很简单，我们用流程图表示如下：

![image.png](https://file.liangxiegame.com/6544f785-0389-46bc-813d-b9c77abdd336.png)

运行 Unity，结果如下：

![282fcc3c-96fa-46e1-b4c6-7f4528b04271.gif](https://file.liangxiegame.com/1b934e4f-8f72-44c2-800a-a97f1e707950.gif)


没有变化，运行正确。

大家可能会问，一个简单的数据加减操作，至于创建一个 Command 对象来承担么？看不出来好处呀，反而代码更多了。

如果整个项目只有一个简单的数据加减操作，那使用 Command 有点多此一举，但是一般的项目的交互逻辑，是非常复杂的，代码量也非常多，整个时候使用 Command 词汇发挥作用。

具体发挥什么作用，使用 Command 可以带来很多便利，比如：
* Command 可以复用，Command 也可以调用 Command
* Command 可以比较方便实现撤销功能，如果 App 或者 游戏需要的话
* 如果遵循一定规范，可以实现使用 Command 跑自动化测试。
* Command 可以定制 Command 队列，也可以让 Command 按照特定的方式执行
* 一个 Command 也可以封装成一个 Http 或者 TCP 里的一次数据请求
* Command 可以实现 Command 中间件模式
* 等等

OK，通过引入 Command，帮助分担了 Controller 的交互逻辑。使得 Controller 成为一个薄薄的一层，在需要修改 Model 的时候，Controller 只要调用一句简单的 Command 即可。

Command 最明显的好处就是：
* 就算代码再乱，也只是在一个 Command 对象里乱，而不会影响其他的对象。
* 讲方法封装成命令对象，可以实现对命令对象的组织、排序、延时等操作。

更多好处会随着大家的实践慢慢体会到。

当前的 MVC 流程如下：

![](https://file.liangxiegame.com/5ddfe754-110f-4417-8e29-d890e36d4a7a.png)

这篇内容就这些。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)


# 04. 引入 Event

我们看下当前的代码:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public class CounterAppModel : AbstractModel
    {
        public int Count;
        
        protected override void OnInit()
        {
            Count = 0;
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 Model
            this.RegisterModel(new CounterAppModel());
        }
    }
    
    // 引入 Command
    public class IncreaseCountCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count++;
        }
    }
    
    public class DecreaseCountCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count--;
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private CounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<CounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<IncreaseCountCommand>();
                // 表现逻辑
                UpdateView();        
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<DecreaseCountCommand>();
                // 表现逻辑
                UpdateView();
            });
            
            UpdateView();
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}

```


我们通过引入了 Command 来帮助 Controller 分担了一部分的交互逻辑。

但是表现逻辑的代码目前看起来并不是很智能。

表现逻辑的代码如下：

```csharp
// 监听输入
mBtnAdd.onClick.AddListener(() =>
{
    // 交互逻辑
    this.SendCommand<IncreaseCountCommand>();
    // 表现逻辑
    UpdateView();        
});
            
mBtnSub.onClick.AddListener(() =>
{
    // 交互逻辑
    this.SendCommand<DecreaseCountCommand>();
    // 表现逻辑
    UpdateView();
});
```

每次调用逻辑之后，表现逻辑部分都需要手动调用一次（UpdateView 方法）。

在一个项目中，表现逻辑的调用次数，至少会和交互逻辑的调用次数一样多。因为只要修改了数据，对应地就要把数据的biang在界面上表现出来。

而这部分嗲用表现逻辑的代码也会很多，所以我们引入一个事件机制来解决这个问题。

这个事件机制的使用其实是和 Command 一起使用的，这里有一个简单的小模式，如下图所示：

![](https://file.liangxiegame.com/60ccd370-7c2c-4792-8435-ff5427dc5a1b.png)

即通过 Command 修改数据，当数据发生修改后发送对应的数据变更事件。

这个是简化版本的 CQRS 原则，即 Command Query Responsibility Separiation，读写分离原则。

引入这项原则会很容易实现 事件驱动、数据驱动 架构。

在 QFramework 中，用法非常简单，代码如下:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public class CounterAppModel : AbstractModel
    {
        public int Count;
        
        protected override void OnInit()
        {
            Count = 0;
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 Model
            this.RegisterModel(new CounterAppModel());
        }
    }
    
    // 定义数据变更事件
    public struct CountChangeEvent // ++
    {
        
    }
    
    // 引入 Command
    public class IncreaseCountCommand : AbstractCommand 
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count++;
            this.SendEvent<CountChangeEvent>(); // ++
        }
    }
    
    public class DecreaseCountCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count--;
            this.SendEvent<CountChangeEvent>(); // ++
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private CounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<CounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<IncreaseCountCommand>();
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand(new DecreaseCountCommand(/* 这里可以传参（如果有） */));
            });
            
            UpdateView();
            
            // 表现逻辑
            this.RegisterEvent<CountChangeEvent>(e =>
            {
                UpdateView();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}
```


代码很简单。

流程图如下：

![image.png](https://file.liangxiegame.com/43474a6f-6a18-4d97-bdb9-9319a77481b9.png)

运行结果如下:

![282fcc3c-96fa-46e1-b4c6-7f4528b04271.gif](https://file.liangxiegame.com/1b934e4f-8f72-44c2-800a-a97f1e707950.gif)

引入事件机制 和 CQRS 原则之后，我们的表现逻辑的代码变少了很多。

由原来的两次主动调用

``` csharp
// 监听输入
mBtnAdd.onClick.AddListener(() =>
{
    // 交互逻辑
    this.SendCommand<IncreaseCountCommand>(); // 没有参数构造的命令支持泛型
    // 表现逻辑
    UpdateView();
});
            
mBtnSub.onClick.AddListener(() =>
{
    // 交互逻辑
    this.SendCommand(new DecreaseCountCommand()); // 也支持直接传入对象（方便通过构造传参)
    // 表现逻辑
    UpdateView();
});
```

变成了一处监听事件，接收事件进行调用。

``` csharp
// 监听输入
mBtnAdd.onClick.AddListener(() =>
{
    // 交互逻辑
    this.SendCommand<IncreaseCountCommand>(); // 没有参数构造的命令支持泛型
});
            
mBtnSub.onClick.AddListener(() =>
{
    // 交互逻辑
    this.SendCommand(new DecreaseCountCommand()); // 也支持直接传入对象（方便通过构造传参)
});
            
UpdateView();
            
// 表现逻辑
this.RegisterEvent<CountChangeEvent>(e =>
{
    UpdateView();
}).UnRegisterWhenGameObjectDestroyed(gameObject);
```

这样减缓了很多交互逻辑。

OK，到此，我们算是用上了还算合格的 MVC 的实现，而 QFramework 所提供的概念中，最重要的概念已经接触到了，即 CQRS，通过 Command 去修改数据，数据发生修改后发送数据变更事件。

当前的示意图如下：

![](https://file.liangxiegame.com/d25c65e0-25ba-43ca-9060-69bd51efaf46.png)


学到这里，对于 QFramework 架构的使用算是真正的入门了。

不过接下来还有一些概念，我们下一篇继续。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 05. 引入 Utility

在这一篇，我们来支持 CounterApp 的存储功能。

其代码也非常简单，只需要修改一部分 Model 的代码即可，如下：

```csharp
    // 定义一个 Model 对象
    public class CounterAppModel : AbstractModel
    {
        private int mCount;

        public int Count
        {
            get => mCount;
            set
            {
                if (mCount != value)
                {
                    mCount = value;
                    PlayerPrefs.SetInt(nameof(Count),mCount);
                }
            }
        }

        protected override void OnInit()
        {
            Count = PlayerPrefs.GetInt(nameof(Count), mCount);
        }
    }
```

这样就支持了非常基本的数据存储功能。

当然还是有一些问题，如果时候未来我们需要存储的数据非常多的时候，Model 层就会充斥大量存储、加载相关的代码。

还有就是，我们以后如果不想使用 PlayperPrefs 了，想使用 EasySave 或者 SQLite 的时候，就会造成大量的修改工作量。

于是 QFramework 提供了一个 Utility 层，专门用来解决上述两个问题的，使用方法非常简单，如下：

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public class CounterAppModel : AbstractModel
    {
        private int mCount;

        public int Count
        {
            get => mCount;
            set
            {
                if (mCount != value)
                {
                    mCount = value;
                    PlayerPrefs.SetInt(nameof(Count),mCount);
                }
            }
        }

        protected override void OnInit()
        {
            var storage = this.GetUtility<Storage>();

            Count = storage.LoadInt(nameof(Count));

            // 可以通过 CounterApp.Interface 监听数据变更事件
            CounterApp.Interface.RegisterEvent<CountChangeEvent>(e =>
            {
                this.GetUtility<Storage>().SaveInt(nameof(Count), Count);
            });
        }
    }

    // 定义 utility 层
    public class Storage : IUtility
    {
        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key,value);
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 Model
            this.RegisterModel(new CounterAppModel());
            
            // 注册存储工具的对象
            this.RegisterUtility(new Storage());
        }
    }
    
    // 定义数据变更事件
    public struct CountChangeEvent // ++
    {
        
    }
    
    // 引入 Command
    public class IncreaseCountCommand : AbstractCommand 
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count++;
            this.SendEvent<CountChangeEvent>(); // ++
        }
    }
    
    public class DecreaseCountCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count--;
            this.SendEvent<CountChangeEvent>(); // ++
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private CounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<CounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<IncreaseCountCommand>();
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand(new DecreaseCountCommand(/* 这里可以传参（如果有） */));
            });
            
            UpdateView();
            
            // 表现逻辑
            this.RegisterEvent<CountChangeEvent>(e =>
            {
                UpdateView();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}

```

代码非常简单，我们运行下 Unity 看下结果：

![f55b6c09-f5a0-402f-bffa-5ceb0bc3d8fb.gif](https://file.liangxiegame.com/1c622976-b32a-4b62-92a3-d34b2c628e27.gif)

运行正确。

这样当我们，想要将 PlayerPrefs 方案替换成 EasySave 的时候，只需要对 Storage 里的代码进行修改即可。

最后给出流程图，如下：

![image.png](https://file.liangxiegame.com/f2329b2f-700a-4693-b22e-b1afc50c7364.png)

好了，这篇就介绍到这里。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 06. 引入 System
在这一篇，我们来引入最后一个基本概念 System。

首先我们来看下代码，如下:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public class CounterAppModel : AbstractModel
    {
        private int mCount;

        public int Count
        {
            get => mCount;
            set
            {
                if (mCount != value)
                {
                    mCount = value;
                    PlayerPrefs.SetInt(nameof(Count),mCount);
                }
            }
        }

        protected override void OnInit()
        {
            var storage = this.GetUtility<Storage>();

            Count = storage.LoadInt(nameof(Count));

            // 可以通过 CounterApp.Interface 监听数据变更事件
            CounterApp.Interface.RegisterEvent<CountChangeEvent>(e =>
            {
                this.GetUtility<Storage>().SaveInt(nameof(Count), Count);
            });
        }
    }

    // 定义 utility 层
    public class Storage : IUtility
    {
        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key,value);
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 Model
            this.RegisterModel(new CounterAppModel());
            
            // 注册存储工具的对象
            this.RegisterUtility(new Storage());
        }
    }
    
    // 定义数据变更事件
    public struct CountChangeEvent // ++
    {
        
    }
    
    // 引入 Command
    public class IncreaseCountCommand : AbstractCommand 
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count++;
            this.SendEvent<CountChangeEvent>(); // ++
        }
    }
    
    public class DecreaseCountCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count--;
            this.SendEvent<CountChangeEvent>(); // ++
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private CounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<CounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<IncreaseCountCommand>();
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand(new DecreaseCountCommand(/* 这里可以传参（如果有） */));
            });
            
            UpdateView();
            
            // 表现逻辑
            this.RegisterEvent<CountChangeEvent>(e =>
            {
                UpdateView();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}

```

这里我们假设一个功能，即策划提出了一个成就达成的功能，即 Count 到 10 的时候，触发一个点击达人成就，点击二十次 则触发一个 点击专家成就。

逻辑听起来很简单，我们直接在 IncreaseCountCommand 里编写即可，如下：

```csharp
    public class IncreaseCountCommand : AbstractCommand 
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<CounterAppModel>();
                
            model.Count++;
            this.SendEvent<CountChangeEvent>(); // ++

            if (model.Count == 10)
            {
                Debug.Log("触发 点击达人 成就");
            }
            else if (model.Count == 20)
            {
                Debug.Log("触发 点击专家 成就");
            }
        }
    }
```

代码很简单，我们运行测试一下。

运行之后，笔者点击了 20 次 + 号，结果如下：

![image.png](https://file.liangxiegame.com/826d5513-059e-41ba-8f5a-4fbea78dbde7.png)

这个功能很快就完成了。

但是这个时候策划说，希望再增加一个当点击 - 号到 -10 时，触发一个 点击菜鸟成就，然后策划还说，点击达人 和 点击专家 成就太容易达成了，需要分别改成 1000 次 和 2000 次。

而这次策划提出的需求，需要我们修改两处的代码，即 IncreaseCountCommand 里需要修改数值为 1000 和 2000，然后再 DecreaseCountCommand 增加一个判断逻辑。

一次提出的需求，结果造成了多处修改，这说明代码有问题。

首先像这种规则类的逻辑，比如分数统计或者成就统计等代码，不适合分散写在 Command 里，而适合统一写在一个对象里，而这种对象，在 QFramework 里有提供，就是 System 对象。

使用代码如下:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public class CounterAppModel : AbstractModel
    {
        private int mCount;

        public int Count
        {
            get => mCount;
            set
            {
                if (mCount != value)
                {
                    mCount = value;
                    PlayerPrefs.SetInt(nameof(Count),mCount);
                }
            }
        }

        protected override void OnInit()
        {
            var storage = this.GetUtility<Storage>();

            Count = storage.LoadInt(nameof(Count));

            // 可以通过 CounterApp.Interface 监听数据变更事件
            CounterApp.Interface.RegisterEvent<CountChangeEvent>(e =>
            {
                this.GetUtility<Storage>().SaveInt(nameof(Count), Count);
            });
        }
    }


    public class AchievementSystem : AbstractSystem // +
    {
        protected override void OnInit()
        {
            var model = this.GetModel<CounterAppModel>();

            this.RegisterEvent<CountChangeEvent>(e =>
            {
                if (model.Count == 10)
                {
                    Debug.Log("触发 点击达人 成就");
                }
                else if (model.Count == 20)
                {
                    Debug.Log("触发 点击专家 成就");
                } else if (model.Count == -10)
                {
                    Debug.Log("触发 点击菜鸟 成就");
                }
            });
        }
    }

    // 定义 utility 层
    public class Storage : IUtility
    {
        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key,value);
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 System 
            this.RegisterSystem(new AchievementSystem()); // +
             
            // 注册 Model
            this.RegisterModel(new CounterAppModel());
            
            // 注册存储工具的对象
            this.RegisterUtility(new Storage());
        }
    }
    
    // 定义数据变更事件
    public struct CountChangeEvent // ++
    {
        
    }
    
    // 引入 Command
    public class IncreaseCountCommand : AbstractCommand 
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<CounterAppModel>();
                
            model.Count++;
            this.SendEvent<CountChangeEvent>(); // ++
        }
    }
    
    public class DecreaseCountCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count--;
            this.SendEvent<CountChangeEvent>(); // ++
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private CounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<CounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<IncreaseCountCommand>();
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand(new DecreaseCountCommand(/* 这里可以传参（如果有） */));
            });
            
            UpdateView();
            
            // 表现逻辑
            this.RegisterEvent<CountChangeEvent>(e =>
            {
                UpdateView();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}

```

代码越来越多，但是不难。

运行游戏，笔者点击的结果如下：

![](https://file.liangxiegame.com/a1adc1e8-6bb9-49c1-ae74-e0e55673e865.png)

结果没问题。

好了，笔者写的成就系统非常简陋，实际上额度成就系统可以写得非常完善，比如可以再成就系统里进行存储加载等操作，而此文的成就系统仅仅是展示目的。

到此，我们就接触到了 QFramework 架构所提供的核心概念。

我们回顾一下第一篇的两张图，如下:

![](https://file.liangxiegame.com/39bdcf54-0240-46e0-8f68-9eb708505695.png)



![](https://file.liangxiegame.com/6bf42306-0b2a-4417-bbcf-354af0132596.png)

到此，大家应该能看懂这两张图了。

QFramework 总共分四个层级，即
* 表现层：IController
* 系统层：ISystem
* 数据层：IModel
* 工具层：IUtility

除了四个层级，还接触了为 Controller 的交互逻辑减负的 Command 和 为表现逻辑减负的 Event。

还有一个非常重要的 CQRS 原则的简易版本，Command->Model->State Changed Event。

到目前为止 QFramework 的基本用法我们过了一遍了。

从下一篇开始，我们开始介绍 QFramework 架构提供的剩余功能，这些功能是可选的。

这篇就到这里。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)





# 07. 使用 BindableProperty 优化事件

在这篇我们介绍一个新的概念 BindableProperty。

BindableProperty 是包含 数据 + 数据变更事件 的一个对象。

## BindableProperty 基本使用
简单的用法如下:

```csharp
var age = new BindableProperty<int>(10);

age.Register(newAge=>{
  
  Debug.Log(newAge)
}).UnRegisterWhenGameObjectDestoryed(gameObject);


age++;
age--;


// 输出结果
// 11
// 10
```

非常简单，就是当调用 age++ 和 age-- 的时候，就会触发数据变更事件。

BindableProperty 除了提供 Register 这个 API 之外，还提供了 RegisterWithInitValue API,意思是 注册时 先把当前值返回过来。

具体用法如下:

```csharp
var age = new BindableProperty<int>(5);

age.RegisterWithInitValue(newAge => {
  
  Debug.Log(newAge);
  
});

// 输出结果
// 5
```

这个 API 就是，没有任何变化的情况下，age 先返回一个当前的值，比较方便用于显示初始界面。

BindableProperty 是一个独立的工具，可以脱离 QFramework 架构使用，也就是说不用非要用 QFramework 的 MVC 才能用 BindableProperty，而是可以再自己项目中随意使用。

## 使用 BindableProperty 优化  CounterApp 的代码

我们直接优化即可，优化后代码如下:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public class CounterAppModel : AbstractModel
    {
        public BindableProperty<int> Count { get; } = new BindableProperty<int>();

        protected override void OnInit()
        {
            var storage = this.GetUtility<Storage>();
            
            // 设置初始值（不触发事件）
            Count.SetValueWithoutEvent(storage.LoadInt(nameof(Count)));

            // 当数据变更时 存储数据
            Count.Register(newCount =>
            {
                storage.SaveInt(nameof(Count),newCount);
            });
        }
    }


    public class AchievementSystem : AbstractSystem 
    {
        protected override void OnInit()
        {
            this.GetModel<CounterAppModel>() // -+
                .Count
                .Register(newCount =>
                {
                    if (newCount == 10)
                    {
                        Debug.Log("触发 点击达人 成就");
                    }
                    else if (newCount == 20)
                    {
                        Debug.Log("触发 点击专家 成就");
                    }
                    else if (newCount == -10)
                    {
                        Debug.Log("触发 点击菜鸟 成就");
                    }
                });
        }
    }

    // 定义 utility 层
    public class Storage : IUtility
    {
        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key,value);
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 System 
            this.RegisterSystem(new AchievementSystem()); // +
             
            // 注册 Model
            this.RegisterModel(new CounterAppModel());
            
            // 注册存储工具的对象
            this.RegisterUtility(new Storage());
        }
    }

    // 引入 Command
    public class IncreaseCountCommand : AbstractCommand 
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<CounterAppModel>();
                
            model.Count.Value++; // -+
        }
    }
    
    public class DecreaseCountCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<CounterAppModel>().Count.Value--; // -+
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private CounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<CounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<IncreaseCountCommand>();
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand(new DecreaseCountCommand(/* 这里可以传参（如果有） */));
            });

            // 表现逻辑
            mModel.Count.RegisterWithInitValue(newCount => // -+
            {
                UpdateView();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}

```


代码改动很多，重要的改动为：
* Model 中的 Count 和 mCount 改成了一个叫做 Count 的 BindableProperty
* 删掉了 CountChangeEvent 改用监听 BindableProperty
* Controller 在初始化中去掉一次 UpdateView 的主动调用

可以说代码量一下子少了很多。

我们看下运行结果：

![282fcc3c-96fa-46e1-b4c6-7f4528b04271.gif](https://file.liangxiegame.com/1b934e4f-8f72-44c2-800a-a97f1e707950.gif)


运行没问题。

由于我们的 Count 数据，是单个数据 + 事件变更的形式，所以用 BindableProperty 非常合适，可以少写很多代码。

一般情况下，像主角的金币、分数等数据非常适合用 BindableProperty 的方式实现。

好了 BindableProperty 我们就介绍到这里。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 08. 用接口设计模块（依赖倒置原则）

QFramework 本身支持依赖倒置原则，就是所有的模块访问和交互都可以通过接口来完成，代码如下:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public interface ICounterAppModel : IModel
    {
        BindableProperty<int> Count { get; }
    }
    public class CounterAppModel : AbstractModel,ICounterAppModel
    {
        public BindableProperty<int> Count { get; } = new BindableProperty<int>();

        protected override void OnInit()
        {
            var storage = this.GetUtility<IStorage>();
            
            // 设置初始值（不触发事件）
            Count.SetValueWithoutEvent(storage.LoadInt(nameof(Count)));

            // 当数据变更时 存储数据
            Count.Register(newCount =>
            {
                storage.SaveInt(nameof(Count),newCount);
            });
        }
    }

    public interface IAchievementSystem : ISystem
    {
        
    }

    public class AchievementSystem : AbstractSystem ,IAchievementSystem
    {
        protected override void OnInit()
        {
            this.GetModel<ICounterAppModel>() // -+
                .Count
                .Register(newCount =>
                {
                    if (newCount == 10)
                    {
                        Debug.Log("触发 点击达人 成就");
                    }
                    else if (newCount == 20)
                    {
                        Debug.Log("触发 点击专家 成就");
                    }
                    else if (newCount == -10)
                    {
                        Debug.Log("触发 点击菜鸟 成就");
                    }
                });
        }
    }

    public interface IStorage : IUtility
    {
        void SaveInt(string key, int value);
        int LoadInt(string key, int defaultValue = 0);
    }
    
    public class Storage : IStorage
    {
        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key,value);
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 System 
            this.RegisterSystem<IAchievementSystem>(new AchievementSystem()); 
             
            // 注册 Model
            this.RegisterModel<ICounterAppModel>(new CounterAppModel());
            
            // 注册存储工具的对象
            this.RegisterUtility<IStorage>(new Storage());
        }
    }

    // 引入 Command
    public class IncreaseCountCommand : AbstractCommand 
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<ICounterAppModel>();
                
            model.Count.Value++; // -+
        }
    }
    
    public class DecreaseCountCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<ICounterAppModel>().Count.Value--; // -+
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private ICounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<ICounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<IncreaseCountCommand>();
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand(new DecreaseCountCommand(/* 这里可以传参（如果有） */));
            });

            // 表现逻辑
            mModel.Count.RegisterWithInitValue(newCount => // -+
            {
                UpdateView();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}

```

代码不难。

所有的模块注册，模块获取 等代码都是通过接口完成，这一点符合 SOLID 原则中的 依赖倒置原则。

通过接口设计模块可以让我们更容易思考模块之间的交互和职责本身，而不是具体实现，在设计的时候可以减少很多的干扰。

当然面向接口的方式去做开发也有很多其他的好处，这当然是大家随着使用时长会慢慢体会的。

其中有一个重要的大一点，就是我们之前说的 Storage，如果想把存储的 API 从 PlayerPrefs 切换成 EasySave，那么我们就不需要去修改 Storage 对象，而是扩展一个 IStorage 接口即可，伪代码如下:
```csharp
    public class EasySaveStorage : IStorage
    {
        public void SaveInt(string key, int value)
        {
            // todo
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            // todo
            throw new System.NotImplementedException();
        }
    }
```

注册模块的伪代码如下:

```csharp
    // 定义一个架构（用于管理模块）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册成就系统
            this.RegisterSystem<IAchievementSystem>(new AchievementSystem());
            
            this.RegisterModel<ICounterAppModel>(new CounterAppModel());
            
            // 注册存储工具对象
            // this.RegisterUtility<IStorage>(new Storage());
            this.RegisterUtility<IStorage>(new EasySaveStorage());
        }
    }
```

这样，底层所有存储的代码都切换成了 EasySave 的存储，替换一套方案非常简单。

好了，用接口设计模块的功能就介绍完了。

这篇内容就这些。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 09. Query 介绍

Query 是 CQRS 中的 Q，也就是 Command Query Responsibility Saperation 中的 Query。

关于 Command 我们已经介绍了。

而 Query 是和 Command 对应的查询对象。

首先 Controller 中的表现逻辑更多是接收到数据变更事件之后，对 Model 或者 System 进行查询，而查询的时候，有的时候需要组合查询，比如多个 Model 一起查询，查询的数据可能还需要转换一下，这种查询的代码量比较多。尤其是像模拟警用或者非常重数据的项目，所以 QFramework 支持通过 Query 这样的一个概念，来解决这部分问题。

使用的方式也很简单，和 Command 用法一致，这里我们写一个小的 App， 叫做 QueryExampleApp 代码如下:

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class QueryExampleController : MonoBehaviour,IController
    {
        public class StudentModel : AbstractModel
        {

            public List<string> StudentNames = new List<string>()
            {
                "张三",
                "李四"
            };
            
            protected override void OnInit()
            {
                
            }
        }
        
        public class TeacherModel : AbstractModel
        {
            public List<string> TeacherNames = new List<string>()
            {
                "王五",
                "赵六"
            };
                
            protected override void OnInit()
            {
                
            }
        }

        // Architecture
        public class QueryExampleApp : Architecture<QueryExampleApp>
        {
            protected override void Init()
            {
                this.RegisterModel(new StudentModel());
                this.RegisterModel(new TeacherModel());
            }
        }
        
        
        /// <summary>
        /// 获取学校的全部人数
        /// </summary>
        public class SchoolAllPersonCountQuery : AbstractQuery<int>
        {
            protected override int OnDo()
            {
                return this.GetModel<StudentModel>().StudentNames.Count +
                       this.GetModel<TeacherModel>().TeacherNames.Count;
            }
        }

        private int mAllPersonCount = 0;

        private void OnGUI()
        {
            GUILayout.Label(mAllPersonCount.ToString());

            if (GUILayout.Button("查询学校总人数"))
            {
                mAllPersonCount = this.SendQuery(new SchoolAllPersonCountQuery());
            }
        }

        public IArchitecture GetArchitecture()
        {
            return QueryExampleApp.Interface;
        }
    }
}
```

代码不难。

运行之后，当按下查询按钮时结果如下：


![image.png](https://file.liangxiegame.com/1736bf69-b795-408a-8c09-6e413f57b0b1.png)


好了，这样 Query 的示例就写完了。

Query 是一个可选的概念，如果游戏中数据的查询逻辑并不是很重的话，直接在 Controller 的表现逻辑里写就可以了，但是查询数据比较重，或者项目规模非常大的话，最好是用 Query 来承担查询的逻辑。


Command 一般负责数据的 增 删 改，而 Query 负责数据的 查。


如果游戏需要从服务器同步数据，一般拉取服务器数据的请求可以写在 Query 中，而增删改服务器输的请求可以写在 Command 中。

好了，关于 Query 就介绍到这里。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 10. 架构规范 与 推荐用法

QFramework 架构提供了四个层级：
* 表现层：IController
* 系统层：ISystem
* 数据层：IModel
* 工具层：IUtility


除了四个层级，还提供了 Command、Query、Event、BindableProperty 等概念和工具。

这里有一套层级的规则，如下：

* 表现层：ViewController 层。IController接口，负责接收输入和状态变化时的表现，一般情况下，MonoBehaviour 均为表现层
  * 可以获取 System、Model
  * 可以发送 Command、Query
  * 可以监听 Event

Controller 的接口定义如下：

```csharp
#region Controller

public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetSystem, ICanGetModel,ICanRegisterEvent, ICanSendQuery
{
}

#endregion
```

* 系统层：System层。ISystem接口，帮助IController承担一部分逻辑，在多个表现层共享的逻辑，比如计时系统、商城系统、成就系统等
  * 可以获取 System、Model
  * 可以监听Event
  *  可以发送Event

System 的接口定义如下：

```csharp
#region System

public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetUtility,ICanRegisterEvent, ICanSendEvent, ICanGetSystem
{
    void Init();
}
```


* 数据层：Model层。IModel接口，负责数据的定义、数据的增删查改方法的提供
  * 可以获取 Utility
  * 可以发送 Event


Model 的接口定义如下：
```csharp
public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanGetUtility, ICanSendEvent
{
    void Init();
}
```


* 工具层：Utility层。IUtility接口，负责提供基础设施，比如存储方法、序列化方法、网络连接方法、蓝牙方法、SDK、框架继承等。啥都干不了，可以集成第三方库，或者封装API

Utility 的接口定义如下:
```csharp
#region Utility

public interface IUtility
{
}

#endregion
```


* Command：命令，负责数据的增删改。
  * 可以获取 System、Model
  * 可以发送 Event、Command

Command 的接口定义如下：

```csharp
public interface ICommand : IBelongToArchitecture, ICanSetArchitecture, ICanGetSystem, ICanGetModel, ICanGetUtility,ICanSendEvent, ICanSendCommand, ICanSendQuery
{
    void Execute();
}
```

* Query：查询、负责数据的查询
  * 可以获取 System、Model
  * 可以发送 Query

```csharp
public interface IQuery<TResult> : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem,ICanSendQuery
{
    TResult Do();
}
```


* 通用规则：
  - IController 更改 ISystem、IModel 的状态必须用Command
  - ISystem、IModel 状态发生变更后通知 IController 必须用事件或BindableProperty
  - IController可以获取ISystem、IModel对象来进行数据查询
  - ICommand、IQuery 不能有状态,
  - 上层可以直接获取下层，下层不能获取上层对象
  - 下层向上层通信用事件
  - 上层向下层通信用方法调用（只是做查询，状态变更用 Command），IController 的交互逻辑为特别情况，只能用 Command

通用规则是理想状态下的一套规则，但是落实的实际项目，很有可能需要对以上规则做一些修改。

修改的方式非常简单，比如我希望 IController 可以发送事件，那么我们只需要在 IController 接口上增加一个 ICanSendEvent 接口即可，代码如下:

```csharp
    #region Controller

    public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetSystem, ICanGetModel,
        ICanRegisterEvent, ICanSendQuery,
        ICanSendEvent // +
    {
    }

    #endregion
```

这样，就可以在 Controller 对象里，通过 this.SendEvent 来发送事件了。

如果是打算先了解或学习 QFramework 架构，那么我推荐就先按照 QFramework 默认的架构规范来做练习项目。

如果是打算马上用 QFramework 做项目，那么可以再保持原有开发习惯的基础上，一点点引入 QFramework 的概念，比如一开始用 BindableProperty 和 Architecture 来解决 Model 和数据更新的问题。

再慢慢开始用 Command 来解决交互逻辑臃肿的问题，以此类推，直到能完全掌握全部概念，最终能修改和定制 QFramework.cs 源码。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 11. 光速实现 EditorCounterApp 和 给主程看的开发模式

首先，我们来实现一个好玩的事情，就是在前边已经实现好的 CounterApp 的基础上，光速实现一个编辑器版本的 CounterApp。

代码非常简单，如下:

```csharp
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace QFramework.Example
{
    public class EditorCounterAppWindow : EditorWindow,IController
    {

        [MenuItem("QFramework/Example/EditorCounterAppWindow")]
        static void Open()
        {
            GetWindow<EditorCounterAppWindow>().Show();
        }
        
        private ICounterAppModel mCounterAppModel;

        private void OnEnable()
        {
            mCounterAppModel = this.GetModel<ICounterAppModel>();
        }

        private void OnDisable()
        {
            mCounterAppModel = null;
        }

        private void OnGUI()
        {
            if (GUILayout.Button("+"))
            {
                this.SendCommand<IncreaseCountCommand>();
            }
            
            GUILayout.Label(mCounterAppModel.Count.Value.ToString());


            if (GUILayout.Button("-"))
            {
                this.SendCommand<DecreaseCountCommand>();
            }
        }

        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }
    }
}

#endif
```

代码量不多，运行结果如下：

![image.png](https://file.liangxiegame.com/3b685522-d4ef-4648-ba3d-5726aaee7b62.png)


这样就非常快速地实现了 CounterApp 的 编辑器版本。

因为 QFramework 写的 App ，底层三层是可以复用的。

如图所示：

![image.png](https://file.liangxiegame.com/fc803d9e-2868-4b5b-af29-d39dd9e37891.png)

底层的三层 与 表现层 的通信方式有 Command、回调/事件、方法/Query。


我们可以把表现层类比成网页前端，而底层三层类比成服务器。

那么 Command、回调/事件、方法/Query 其实就是类似于 HTTP 或者 TCP 的接口或协议。

而接口或者协议只要做好约定，那么前端就不需要关心服务端的具体实现了，而服务端也不需要关心前端的具体实现。

这就做到了在分工时，将表现层和底层三层的工作分别给不同的人来负责。

而笔者曾经做过一个这样的项目。

在项目中笔者负责将 底层三层实现好，然后和服务器把数据和接口调好，数据的显示部分笔者用的一个快速写界面的方案，比如 xmllayout 或者 delight，这种方案写界面非常快，可以用来实现系统原型。

然后等数据和接口调好，系统原型实现好后，把界面、做场景流程、做表现的工作都分配给了初学者的同事们，同事们只要看实现的系统原型，就知道调用哪些 Command/Query、监听哪些事件、或调用哪些方法，这样就可以做好分工协作了。

用一张图表示如下：

![image.png](https://file.liangxiegame.com/430968f9-68a8-470a-8450-b70316a31419.png)


当然这只是其中一种的项目开发模式。

随着时间，初学者同事们用熟了这套架构之后，渐渐地也能自己写底层三层了，于是笔者就慢慢把底层的工作量也分出去了，自己就没啥事干了。

好了，这就是一次笔者曾经使用的一种开发模式的分享，而具体自己的开发模式，需要根据实际情况来制定，最简单的方式就是先按照原有的习惯的开发模式，然后逐渐掌握这套架构，掌握了之后慢慢改进之前的开发模式。

这篇就介绍到这里。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)


# 12. 纸上设计
由于 QFramework 支持 MVC、分层 和 CQRS，再加上提供了使用规范，那么 QFramework 就可以达到高度的标准化，达到高度的标准化那么就有了做一件事情的条件，就是纸上设计。

假如我们想实现一个主角吃金币，金币数量增加的功能，则可以按照如下顺序设计图。


首先需要确定数据结构。

我们可以用类图来确定，也可以用更简单的方式绘制。

![image.png](https://file.liangxiegame.com/fb76cf87-9758-4289-8b6b-29621beba7c4.png)



然后，我们需要确定表现层如何显示金币。

![image.png](https://file.liangxiegame.com/6f4dcfce-75a6-4e5d-bfc0-edc5f2e20f16.png)

接着，我们要开始设计 Command。

![image.png](https://file.liangxiegame.com/ed1b486b-4ea7-45e1-8af2-6c238952c83e.png)

然后，可以把如何触发，和如何更新的图都画好。

![image.png](https://file.liangxiegame.com/504fc46e-351a-4b56-874e-bbb9d2879642.png)

这样一个吃金币的功能思路就设计好了。

当然吃金币这个例子很简单。

不过笔者建议，如果 QFramework 架构用得不是很熟悉的时候，就用这种小功能来做一些纸上设计比较合适。

而当 QFramework 架构用得很熟的时候，可以在纸上设计一些更复杂的功能。

比如技能系统、强化道具系统、背包系统、任务系统等等。


我们看一下第一篇中的一张图。

![](https://file.liangxiegame.com/6bf42306-0b2a-4417-bbcf-354af0132596.png)

这张图，其实就是一张纸上设计图，即 当主角打死敌人后 触发分数变更、触发成就达成 的功能。

这种图加上吃金币图，是 QFramework 纸上设计中的 功能图。


除了功能图 ，还有 QFrameowrk 纸上设计的 架构图。

架构图的示例如下所示：

![image.png](https://file.liangxiegame.com/c1584a3b-f8be-49a1-897a-9f1b684864bf.png)


架构图只是罗列了每个模块都在哪个层级，并没有展示具体如何交互。

而功能则是展示了一个功能具体的逻辑控制流向。

在一般情况下，架构图 和 功能图 都不是必须的。

功能图在早期更多是帮助 QFramework 不熟悉的人梳理思路用的。

但是也有开发人员不在电脑旁的时候，而此时项目也比较紧，这个时候 纸上设计 就会排上用场了。

开发人员完全可以纸上把整个项目的功能思路都实现出来。

还有一种用法就是，开发人员拿到需求之后，集合全部开发人员开一次会议，在会议中边研读策划文档边和大家一起用纸上设计把整个项目的功能思路都实现出来，然后再把编码和具体实现的工作量分配给每个人，这也是一种用法。

总之 纸上设计 是非常有用的一个方法。

可能有人会问，纸上设计需要遵循什么格式吗？

答案是没有的。

如果习惯用 UML 类图，那就用 UML 类图绘制，如果习惯用方块、圆圈、棱角那就用方块、圆圈、棱角，如果习惯用纸笔，那就用纸笔。

总之怎么快怎么方便就怎么用。


纸上设计 除了方便功能实现，也方便在团队内沟通，比如一位开发人员如果对实现一个功能没有思路，那么就可以问主程或者 QFramework 高手，让高手用一张纸来梳理思路，这样开发人员拿到这张纸就可以去实现了。还可以每次让开发人员先在纸上设计好，然后把这张纸拿给主程或者 QFramework 高手，主程或 QFramework 高手验证完才可以进行编码实现，这样用也是可以的。

好了关于纸上设计的入门和一些拓展用法就介绍到这里。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)







# 13. Architecture 的好处


不管是 System、Model 还是 Utility，都会注册到 Architecture 中。

伪代码如下:
```csharp
namespace QFramework.PointGame
{
    public class PointGame : Architecture<PointGame>
    {
        protected override void Init()
        {
            RegisterSystem<IScoreSystem>(new ScoreSystem());
            RegisterSystem<ICountDownSystem>(new CountDownSystem());
            RegisterSystem<IAchievementSystem>(new AchievementSystem());

            RegisterModel<IGameModel>(new GameModel());

            RegisterUtility<IStorage>(new PlayerPrefsStorage());
        }
    }
}

```

大家可能会问，如果一个项目有非常多的 System、Model、Utility 全部注册到 Architecture，这样 Architecture 的代码量就变多了，会不会让项目变得难以管理？

答案是不会，Architecture 注册的模块越多，这套架构发挥的作用就越大。

因为 Architecture 本身就能很好地展示项目的结构，可以把 Architecture 本身当做一个架构图。

比如以上伪代码对应的架构图如下：

![image.png](https://file.liangxiegame.com/cc294f03-4171-4cb3-b774-b487688e51fb.png)
非常清晰。

而伪代码中只有 5 个注册模块，是非常少见的，一般情况下，项目都会注册十几个甚至几十个模块，也有上百个模块的时候。

而如果这些模块没有用 QFramework 而全部使用单例实现的话，项目就会变得很混乱。

而使用了 QFramework，我们就可以在 Architecture 中统一集中管理这些模块，是方便项目管理的。

这就是使用 Architecture 的优势。

这里，再贴出一下笔者曾经写的项目的 Architecture，代码如下:

```csharp
using IndieGame.Models;
using IndieGame.Utility;
using QFramework;
using UnityEngine;
using UTGM;

namespace IndieGame
{
    public class LiangxiesGame : Architecture<LiangxiesGame>
    {
        public static bool IsTestMode = true;

        public static void SetTestMode(bool testMode)
        {
            IsTestMode = testMode;
        }

        protected override void Init()
        {
            RegisterSystem<ISaveSystem>(new SaveSystem());
            RegisterSystem<IInputSystem>(new InputSystem());
            RegisterSystem<ILevelSystem>(new LevelSystem());
            RegisterSystem<IBookSystem>(new BookSystem());
            RegisterSystem<IMapSystem>(new MapSystem());
            RegisterSystem<IGameTimeSystem>(new GameTimeSystem());
            RegisterSystem<IRankSystem>(new RankSystem());
            RegisterSystem<IGameSystem>(new GameSystem());
            RegisterSystem<ILuaSystem>(new LuaSystem());
            RegisterSystem<IAchievementSystem>(new AchievementSystem());
            RegisterSystem<IEnemyRecycleSystem>(new EnemyRecycleSystem());
            RegisterSystem<IUISystem>(new UISystem());
            RegisterSystem<IHurtSystem>(new HurtSystem());
            RegisterSystem<ILevelUpSystem>(new LevelUpSystem());
            RegisterSystem<ILevelConfigSystem>(new LevelConfigSystem());
        
            RegisterModel<ICoinModel>(new CoinModel());
            RegisterModel<ISettingModel>(new SettingModel());
            RegisterModel<IBookModel>(new BookModel());
            RegisterModel<IMechanismModel>(new MechanismModel());
            RegisterModel<IPlayerModel>(new PlayerModel());
            
            RegisterUtility<IStorage>(new EasySaveStorage());

            Application.persistentDataPath.CreateDirIfNotExists();
        }
    }
}
```

System 层有什么、Model 层有什么、Utility 层有什么，一目了然。

好了，这篇就到这里。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 14. Command 拦截

QFramework 提供了拦截 Command 的 API。

我们尝试在 CounterApp 中实现一个 Command 日志。

代码很简单，如下:

```csharp
public class CounterApp : Architecture<CounterApp>
{
    protected override void Init()
    {
        // 注册 System 
        this.RegisterSystem<IAchievementSystem>(new AchievementSystem()); 
             
        // 注册 Model
        this.RegisterModel<ICounterAppModel>(new CounterAppModel());
            
        // 注册存储工具的对象
        this.RegisterUtility<IStorage>(new Storage());
    }

    protected override void ExecuteCommand(ICommand command)
    {
        Debug.Log("Before " + command.GetType().Name + "Execute");
        base.ExecuteCommand(command);
        Debug.Log("After " + command.GetType().Name + "Execute");
    }
}
```

只需要在 Architecture 中覆写 ExecuteCommand 即可。

运行之后，笔者随意点击了几次按钮，结果如下:

![image.png](https://file.liangxiegame.com/96bdc2f4-222d-4e91-a10e-dc2128e50fb4.png)

这样就实现了一个非常简单的 Command 日志功能。


## 有了 Command 拦截有什么用？

有了 Command 拦截功能，我们可以做非常多的事情，比如：
* Command 日志可以用来方便调试
* 可以实现 Command 中间件模式 可以写各种各样额度 Command 中间件，比如 Command 日志中间件
* 可以方便你先撤销功能
* 可以用 Command 做自动化测试
* 等等

好了这篇就介绍到这里。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 15. 内置工具：TypeEventSystem

QFramework 除了提供了一套架构之外，QFramework 还提供三个可以脱离架构使用的工具 TypeEventSystem、EasyEvent、BindableProperty、IOCContainer。

这些工具并不是有意提供，而是 QFramework 的架构在设计之初是通过这三个工具组合使用而成的。

在这一篇，我们来学习 TypeEventSystem 的使用。

## 基本使用

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class TypeEventSystemBasicExample : MonoBehaviour
    {
        public struct TestEventA
        {
            public int Age;
        }

        private void Start()
        {
            TypeEventSystem.Global.Register<TestEventA>(e =>
            {
                Debug.Log(e.Age);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        private void Update()
        {
            // 鼠标左键点击
            if (Input.GetMouseButtonDown(0))
            {
                TypeEventSystem.Global.Send(new TestEventA()
                {
                    Age = 18
                });
            }

            // 鼠标右键点击
            if (Input.GetMouseButtonDown(1))
            {
                TypeEventSystem.Global.Send<TestEventA>();
            }
        }
    }
}

// 输出结果
// 点击鼠标左键，则输出:
// 18
// 点击鼠标右键，则输出:
// 0
```

这就是 TypeEventSystem 的最基本用法。

## 事件继承支持
除了基本用法，TypeEventSystem 的事件还支持继承关系。

示例代码如下:
```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class TypeEventSystemInheritEventExample : MonoBehaviour
    {
        public interface IEventA
        {
            
        }
        
        public struct EventB : IEventA
        {
            
        }

        private void Start()
        {
            TypeEventSystem.Global.Register<IEventA>(e =>
            {
                Debug.Log(e.GetType().Name);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TypeEventSystem.Global.Send<IEventA>(new EventB());
                
                // 无效
                TypeEventSystem.Global.Send<EventB>();
            }
        }
    }
}


// 输出结果:
// 当按下鼠标左键时，输出:
// EventB
```

代码不难。

## TypeEventSystem 手动注销

如果想控制 TypeEventSystem 的注销，而不是自动注销也很简单，代码如下:

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class TypeEventSystemUnRegisterExample : MonoBehaviour
    {

        public struct EventA
        {
            
        }
        
        private void Start()
        {
            TypeEventSystem.Global.Register<EventA>(OnEventA);
        }

        void OnEventA(EventA e)
        {
            
        }

        private void OnDestroy()
        {
            TypeEventSystem.Global.UnRegister<EventA>(OnEventA);
        }
    }
}
```

代码也很简单。

## 接口事件

TypeEventSystem 还支持接口事件模式，示例代码如下:

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public struct InterfaceEventA
    {
            
    }

    public struct InterfaceEventB
    {
        
    }

    public class InterfaceEventModeExample : MonoBehaviour
        , IOnEvent<InterfaceEventA>
        , IOnEvent<InterfaceEventB>
    {
        public void OnEvent(InterfaceEventA e)
        {
            Debug.Log(e.GetType().Name);
        }
        
        public void OnEvent(InterfaceEventB e)
        {
            Debug.Log(e.GetType().Name);
        }

        private void Start()
        {
            this.RegisterEvent<InterfaceEventA>()
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<InterfaceEventB>();
        }

        private void OnDestroy()
        {
            this.UnRegisterEvent<InterfaceEventB>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TypeEventSystem.Global.Send<InterfaceEventA>();
                TypeEventSystem.Global.Send<InterfaceEventB>();
            }
        }
    }
}

// 输出结果
// 当按下鼠标左键时，输出:
// InterfaceEventA
// InterfaceEventB
```

代码很简单。

同样接口事件也支持事件之间的继承。

接口事件拥有更好的约束，只要完成实现接口，就可以通过 IDE 的代码生成少写很多代码，其灵感受 CorgiEngine、TopDownEngine 启发。

## 非 MonoBehavior 脚本如何自动销毁

```csharp
public class NoneMonoScript : IUnRegisterList
{
    public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();


    void Start()
    {
        TypeEventSystem.Global.Register<EasyEventExample.EventA>(a =>
        {
                    
        }).AddToUnregisterList(this);
    }

    void OnDestroy()
    {
        this.UnRegisterAll();
    }
}
```


## 小结
如果想手动注销，必须要创建一个用于接收事件的方法。

而用自动注销则直接用委托即可。

这两个各有优劣，按需使用。

另外，事件的定义最好使用 struct，因为 struct 的 gc 更少，可以获得更好的性能。

接口事件拥有更好的约束，也可以通过 IDE 的代码生成来提高开发效率。

总之 TypeEventSystem 是一个非常强大的事件工具。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 16. 内置工具：EasyEvent

TypeEventSystem 是基于 EasyEvent 实现的。

EasyEvent 也是一个可以脱离架构使用的工具。

这里我们来学习一下基本用法。

## 基本使用
代码如下:

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class EasyEventExample : MonoBehaviour
    {
        private EasyEvent mOnMouseLeftClickEvent = new EasyEvent();
        
        private EasyEvent<int> mOnValueChanged = new EasyEvent<int>();
        
        public class EventA : EasyEvent<int,int> { }

        private EventA mEventA = new EventA();

        private void Start()
        {
            mOnMouseLeftClickEvent.Register(() =>
            {
                Debug.Log("鼠标左键点击");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            mOnValueChanged.Register(value =>
            {

                Debug.Log($"值变更:{value}");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);


            mEventA.Register((a, b) =>
            {
                Debug.Log($"自定义事件:{a} {b}");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mOnMouseLeftClickEvent.Trigger();
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                mOnValueChanged.Trigger(10);
            }

            // 鼠标中键
            if (Input.GetMouseButtonDown(2))
            {
                mEventA.Trigger(1,2);
            }
        }
    }
}

// 输出结果：
// 按鼠标左键时，输出:
// 鼠标左键点击
// 按鼠标右键时，输出:
// 值变更:10
// 按鼠标中键时，输出:
// 自定义事件:1 2
```

基本使用非常简单。

EasyEvent 最多支持三个泛型。

## EasyEvent 的优势
EasyEvent 是 C# 委托和事件的替代。

EasyEvent 相比 C# 委托和事件，优势是可以自动注销。

相比 TypeEventSystem，优势是更轻量，大多数情况下不用声明事件类，而且性能更好（接近 C# 委托）。

缺点则是其携带的参数没有名字，需要自己定义名字。

在设计一些通用系统的时候，EasyEvent 会派上用场，比如背包系统、对话系统，TypeEventSystem 是一个非常好的例子。

在一个项目早期做原型验证时，EasyEvent 也会起非常大的作用，QFramework 架构中的事件，其实写起来有点繁琐，而在项目早期快速迭代原型是重点，此时用 EasyEvent 可以获得更快的开发效率，而使用 QFramework 架构中的事件在项目规模更大的时候会发挥很大的作用，它更方便协作更容易维护，也更容易标准化。

好了，关于 EasyEvent 的介绍就到这里。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 17. 内置工具：BindableProperty

在此篇介绍 BindableProperty。

BindableProperty 提供 数据 + 数据变更事件 的一个对象。

## 基本使用

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class BindablePropertyExample : MonoBehaviour
    {
        private BindableProperty<int> mSomeValue = new BindableProperty<int>(0);

        private BindableProperty<string> mName = new BindableProperty<string>("QFramework");
        
        void Start()
        {
            mSomeValue.Register(newValue =>
            {
                Debug.Log(newValue);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            mName.RegisterWithInitValue(newName =>
            {
                Debug.Log(mName);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                mSomeValue.Value++;
            }
        }
    }
}


// 输出结果
// QFramework
// 按下鼠标左键,输出:
// 1
// 按下鼠标左键,输出:
// 2
```

非常简单。


关于 BindableProperty，在之前写 CounterApp 的时候有介绍过，所以这篇就介绍到这里。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 18. 内置工具：IOCContainer

QFramework 架构的模块注册与获取是通过 IOCContainer 实现的。

IOC 的意思是控制反转，即控制反转容器。

其技术的本质很简单，本质就是一个字典，Key 是 Type，Value 是 Object，即：Dictionary<Type,object>。

QFramework 架构中的 IOCContainer 是一个非常简易版本的控制翻转容器，仅支持了注册对象为单例的模式。

一般情况下，其他的控制反转容器会有各种各样的对象注册模式，有的甚至会内置对象池和对象工厂，比如 Zenject。

不过，我们先不用理会那些，如果先上手使用了最简易的版本，其他版本会更容易上手。

我们看下 IOCContainer 的基本使用。

代码如下:

```csharp
using System;
using UnityEngine;

namespace QFramework.Example
{
    public class IOCContainerExample : MonoBehaviour
    {
        
        public class SomeService
        {
            public void Say()
            {
                Debug.Log("SomeService Say Hi");
            }
        }
        
        
        public interface INetworkService
        {
            void Connect();
        }
        
        public class NetworkService : INetworkService
        {
            public void Connect()
            {
                Debug.Log("NetworkService Connect Succeed");
            }
        }

        private void Start()
        {
            var container = new IOCContainer();
            
            container.Register(new SomeService());
            
            container.Register<INetworkService>(new NetworkService());
            
            
            container.Get<SomeService>().Say();
            container.Get<INetworkService>().Connect();
        }
    }
}

// 输出结果:
// SomeService Say Hi
// NetworkService Connect Succeed
```

非常简单。

但是对于很多初学者，IOCContainer 感觉不知道怎么用，也无法理解。

这里给一个简单的说法，使用 IOCContainer 更容易设计出符合依赖倒置原则的模块。

而 QFramework 架构的用接口设计模块的支持就是通过 IOCContainer 支持的，同样使用 IOCContainer 也更容易设计出分层的架构。

好了，关于 IOCContainer 就介绍到这里。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 19. 心中有架构

QFramework.cs 提供了 MVC、分层、CQRS、事件驱动、数据驱动等工具，除了这些工具，QFramework.cs 还提供了架构使用规范。

而当使用 QFramework 熟练到一定的程度之后，就可以达到心中有架构的境界。

如果达到这个境界，你就早已不是当年的你了（开玩笑）。

心中有架构的境界，具体是指可以不依赖 QFramework.cs 就可以再项目中实践 QFramework.cs 架构。

具体的示例如下:

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class ArchitectureInHeartExample : MonoBehaviour
    {

        #region Framework

        public interface ICommand
        {
            void Execute();
        }

        public class BindableProperty<T>
        {
            private T mValue = default;

            public T Value
            {
                get => mValue;
                set
                {
                    if (mValue != null && mValue.Equals(value)) return;
                    mValue = value;
                    OnValueChanged?.Invoke(mValue);
                }
            }

            public event Action<T> OnValueChanged = _ => { };
        }

        #endregion


        #region 定义 Model

        public static class CounterModel
        {
            public static BindableProperty<int> Counter = new BindableProperty<int>()
            {
                Value = 0
            };
        }
        
        #endregion

        #region 定义 Command
        public struct IncreaseCountCommand : ICommand
        {
            public void Execute()
            {
                CounterModel.Counter.Value++;
            }
        }
        
        public struct DecreaseCountCommand : ICommand
        {
            public void Execute()
            {
                CounterModel.Counter.Value--;
            }
        }
        #endregion


        private void OnGUI()
        {
            if (GUILayout.Button("+"))
            {
                new IncreaseCountCommand().Execute();
            }

            GUILayout.Label(CounterModel.Counter.Value.ToString());

            if (GUILayout.Button("-"))
            {
                new DecreaseCountCommand().Execute();
            }
        }
    }
}
```

上图是一个计数器应用的实现。

在这个实现里，没有使用 QFramework.cs 里的任何内容，但是也写出来了符合 QFramework.cs 架构规范的计数器应用实现。

当大家使用 QFramework.cs 到一定程度之后，在未来不使用 QFramework.cs ，也可以按照 QFramework.cs 架构规范来写项目，而到此时，对于大家来说有没有  QFramework.cs 就无所谓了，因为 QFramework.cs 的架构规范已经刻在大家的骨子里了。



当大家熟练使用 QFramework.cs 之后，有一天如果大家去研究 网页前端、服务器、App 开发，会发现它们的很多框架与 QFramework.cs 架构有共通之处，甚至说，通过 QFramework.cs 中积累的开发经验可以直接照搬到其他领域的开发中。

这是因为 QFramework.cs 最初的设计目的，就是为了糅合和简化大量其他领域的架构概念，比如 React 中的 Redux（Flux）、.Net Core 开发中的领域驱动设计、CQRS、仓储模式等、App 开发中的 MVC、MVP、MVVM 等。

我们简单看一下这些图，大家就清楚了。

首先是前端 React 中的 Redux 的工作流程，如下：
![bg2016091802.jpg](https://file.liangxiegame.com/8b854e1e-4772-4a79-b595-c0ded004a569.png)




其中 React Components 对应的是 QFramework.cs  中的 Controller。

Action + Reducers 对应的是 QFramework.cs 中的 Command

Store 对应的是 QFramework.cs 中的 Model。


接着是领域驱动设计：

![image.png](https://file.liangxiegame.com/f966558b-c616-46cd-9eee-4ba56de64b2c.png)


其中 Interface 对应的是 IController。

Application 对应的是 ISystem。

Domain 对应的是 Model。

Infrustracture 对应的是 Utility + 一部分 Model。


接着看下 CQRS，CQRS 一般是领域驱动设计包含的模式，如下图所示:

![v2-da8a89f95e09bb518ad8c770b1413e5e_720w.jpg](https://file.liangxiegame.com/5f45fb20-537e-4574-80ac-c8d6a2d7921e.png)



其中 User Interface 对应的是 IController。

Command 和 Query 对应的是 Command 和 Query。

Domain Model 和 Data 对应的是 Model

Event 对应的是 Event。

非常接近。

接着看下仓储模式:

![20150922190750314.png](https://file.liangxiegame.com/5bf7ddeb-702d-4e05-aa4a-b3751a7547eb.png)


仓储模式没有具体的图，而此图是从网上随便找的，很清晰地表达出了仓储模式的结构。

其中 IRepository 对应的是 IModel。

Repository 对应的是 AbstractModel。

IBookRepository 对应的是 ICounterModel。

BookRespository 对应的是 CounterModel。

使用 ICounterModel 和 CounterModel 举例不是很合适，因为 CounterModel 只有一个 Counter 数据。

更适合的举例是 IStudentModel，StudentModel ，因为 StudentModel 会维护一个 Student 的 List。

仓储模式的优势在于，可以让上层（System、Controller）专注于数据的增删改查功能，而不是具体的增删改查实现，因为在服务器端，数据都是存储在数据库中的，数据库有很多类型，比如 MySQL、MongoDB 等，而在服务器端开发时，很有可能在开发阶段用 SQLite 或者 MongoDB，而在生产环境用的是 MySQL、PostgreSQL，所以在静态类型语言中，仓储模式会和 ORM 一起配合，让开发者专注在数据的增删改查和数据之间的关联上，而不是具体的查询语句，这样能提高开发效率。


最后，MVC、MVP、MVVM 这里就不介绍了，其中 MVP 和 MVVM 的实现会用 BindableProperty，有的会用反射的形式实现。

而 QFramework.cs 中的 BindableProperty 和 MVC 分层，则是来自这些架构中。

好了，此篇的内容就说完了。

大家可能会问，为什么 QFramework.cs 要糅合这些架构概念？

因为在 2019 年左右，笔者刚好在业余时间研究了一年 React 开发，用 React 前端开发做了一些 SideProject，服务器则是用的 .Net Core，再加上之前笔者也有做 iOS、Android 等开发经验。而在当时，笔者突然发现这些领域的架构概念很多都是相通的，可能在这个领域叫这个，在另一个领域只是换了一个名字而已，于是就产生了可不可以把这些架构概念都糅合在一起，然后去掉繁琐的保留有用的部分，于是就开始了 QFramework.cs 的设计。

杂糅和简化这些概念的  QFramework.cs 有什么好处呢？

首先 QFramework.cs 是非常容易上手的架构，因为其中的 MVC 三层概念让大家会觉得非常亲切，所以上手成本并不是很高。

其次 QFramework.cs 是一个能提高大家技术水平的架构，在架构方面，天花板是领域驱动设计的实现，是架构师必研究的内容，如果 QFramework.cs 用熟悉了，再去研究领域驱动设计会容易得多，而领域驱动设计不管在项目中有没有使用，只要去研究就会对架构水平有很大的提升，而 QFramework.cs 算是简化版本的领域驱动设计的实现。

然后 QFramework.cs 可以用来做系统设计、可以做游戏、做项目、做插件都是很适合的，因为笔者自己的很多项目、插件、服务器都是用 QFramework.cs 架构来做的。

最后 QFramework.cs 本身是很强大的，易上手、简单、代码精简、可维护性强、开发效率高、可定制性强、扩展性强，因为 QFramework.cs 吸取了大量其他领域架构的优点，同时也经历过大量项目的打磨而成，总体的代码精简到了 900 行左右。


如果大家想在更进一步强化这些概念，最好的方式就是尝试去学习其他领域的架构，比如:
* React 与  RedU型
* Java/.Net Core 与 DDD 实现，CQRS、仓储模式
* App 开发中的 MVC、MVP、MVVM

好了，这篇内容就说到这里。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)










# 20. QFramework.cs 的更多内容

终于把想介绍的内容都讲完了。

如果想进一步了解和学习 QFramework.cs 可以看如下内容。

## 使用 QFramework.cs 的案例与项目

更多的案例可以到 QFramework github 主页查看。

地址:
* github: https://github.com/liangxiegame/QFramework
* gitee: https://gitee.com/liangxiegame/QFramework

国内童鞋建议打开 gitee 版本仓库。

在 Readme 中可以看到如下内容:

![image.png](https://file.liangxiegame.com/26d4141d-b622-4fed-8dd3-f87979ae5c25.png)

在这里可以安装 QFramework.cs 与官方示例。

点击之后，再点击下图中的下载按钮。

![image.png](https://file.liangxiegame.com/78c1b225-4cfb-4f0c-ab98-33049e2068f2.png)
就可以下载 QFramework.cs 官方示例了。


示例中，除了本教程包含的 CounterApp，还有很多其他示例，如下:

### 小游戏《点点点》

![b5966b31-f004-4b5f-a38d-25753fb2eb8f.gif](https://file.liangxiegame.com/5a10aa95-4c93-4dae-acec-667a113c30ca.gif)

### 小游戏《FlappyBird》

![430b7f31-508d-4569-aa51-b75d5553b8c4.gif](https://file.liangxiegame.com/9845122b-93d9-4106-a027-2d7c129a096a.gif)

作者：王二 soso https://github.com/so-sos-so

### 小游戏《Cube Master》

![b1334ef2-f6d4-4a9c-a5c4-b6cd6508595c.gif](https://file.liangxiegame.com/f51abab0-9dc9-478b-b1f1-67f2cd588477.gif)
作者：王二 soso https://github.com/so-sos-so

### 简易关卡编辑器2D
![c57c20cf-5ee6-4346-8be8-8ad1ea2d63b9.gif](https://file.liangxiegame.com/6492498b-6c22-478d-8785-9f43453c34db.gif)

![ea2cb545-4b5b-4d02-b494-dde4afa4e190.gif](https://file.liangxiegame.com/34b775c6-6a49-4141-9b9a-1377a6c15673.gif)


### 小游戏《贪吃蛇》

![fb907355-c06c-4bde-8ca3-5638ba9b3ef7.gif](https://file.liangxiegame.com/ac70d14e-ea89-445d-899e-06f18f11f8d1.gif)

作者：一只皮皮虾 https://gitee.com/PantyNeko/

以上的示例都是由 QFramework.cs 制作而成的官方示例。


另外还有群友制作的开源游戏

### CrazyCar

Unity制作的联机赛车游戏，后台为SpringBoot + Mybatis；游戏采用QFramework框架，支持KCP和WebSocket网络(商用级)


![Login.jpg](https://file.liangxiegame.com/0ab6cb1d-2374-4aa2-b27d-f04eb72792cd.png)

![Setting.png](https://file.liangxiegame.com/a113dcba-9ba8-4a40-b000-be3b61719ecc.png)

![Homepage.png](https://file.liangxiegame.com/9075c10d-6d21-411c-b1a4-7f92a08f9bfa.png)

![Avatar.png](https://file.liangxiegame.com/32b48b5b-cdcc-433e-b1b2-4b1333211a70.png) ![Profile.png](https://file.liangxiegame.com/bda476e4-0ede-4fd9-a5bb-e993bce8a786.png)


![Equip.png](https://file.liangxiegame.com/158b0ce0-6e67-47c5-81b5-cee6388dd99c.png)

![Rank.png](https://file.liangxiegame.com/2bd0ef1f-d639-48e8-8c48-320995d20de4.png)

![TimeTrial.png](https://file.liangxiegame.com/aa337718-b868-41d2-bc6b-2ef51c157481.png)
![Match.png](https://file.liangxiegame.com/06157781-3271-438c-bf3f-613e6ec00fb0.png)

作者: TastSone  https://github.com/TastSong

项目地址: https://github.com/TastSong/CrazyCar


## QFramework.cs 的架构如何演化出来的?

QFramework.cs 的架构当前的版本，是从 《框架搭建 决定版》中设计出来的，如果学习这门课程，可以对 QFramework.cs 的原理和理念理解得更深刻，更容易对 QFramework.cs 做修改和定制。

* 《框架搭建 决定版》B 站试听：https://www.bilibili.com/video/BV1wh411U7X6
* 《框架搭建 决定版》完整版
  *  Unity 官方中文课堂：https://learn.u3d.cn/tutorial/framework_design
  *  siki 学院：https://www.sikiedu.com/my/course/871
  *  GamePix 独立游戏学院：https://www.gamepixedu.com/my/course/2


另外 QFramework.Toolkits 和 QFramework.ToolkitsPro 里包含的工具很多都是由 QFramework.cs 设计的， 工具的源码本身也是不错的学习资料。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 3. 工具篇：QFramework.Toolkits
# 01. QFramework.Toolkits 简介

QFramework.Toolkits 是包含 QFramework.cs 和 大量工具集的解决方案。

在 QFramework v1.0 之前，QFramework.Toolkits 就是 QFramework 本身，而在 QFramework v1.0 开始，QFramework 拥有了自己的开发架构—QFramework.cs，于是原来的 QFramework 就变成了 QFramework.Toolkits。

QFramework.Toolkits 称为 QFramework 工具集，是一套**开箱即用的、渐进式**的**快速开发**框架。

目标是作为无框架经验的公司、独立开发者、以及 Unity3D 初学者们的**第一套框架**。框架内部积累了多个项目在各个技术方向的解决方案。学习成本低，接入成本低（侵入性低），重构成本低，二次开发成本低。文档内容丰富。

QFramework 工具集的设计哲学是追求极致的开发效率和开发体验。

**QFramework.Toolkits 特性一览**

* 工具集（QFramework.Toolkits v0.16）
    * UIKit 界面&View快速开发&管理解决方案
        * UI、GameObject 的代码生成&自动赋值
        * 界面管理
        * 层级管理
        * 界面堆栈
        * 默认使用 ResKit 方式管理界面资源
        * 可自定义界面的加载、卸载方式
        * Manager Of Manager 架构集成（不推荐使用）
    * ResKit 资源快速开发&管理解决方案
        * AssetBundle 提供模拟模式，开发阶段无需打包即可加载资源
        * 资源名称代码生成支持
        * 同一个 API 可加载 AssetBundle、Resources、网络 和 自定义来源的资源
        * 提供一套引用计数的资源管理模型
    * AudioKit 音频管理解决方案
        * 提供背景音乐、人声、音效 三种音频播放 API
        * 音量控制
        * 默认使用 ResKit 方式管理音频资源
        * 可自定义音频的加载、卸载方式
    * CoreKit 提供大量的代码工具
        * ActionKit：动作序列执行系统
        * CodeGenKit：代码生成 & 自动序列化赋值工具
        * EventKit：提供基于类、字符串、枚举以及信号类型的事件工具集
        * FluentAPI：对大量的 Unity 和 C# 常用的 API 提供了静态扩展的封装（链式 API）
        * IOCKit：提供依赖注入容器
        * LocaleKit：本地化&多语言工具集
        * LogKit：日志工具集
        * PackageKit：包管理工具，由此可更新框架和对应的插件模块。
        * PoolKit：对象池工具集，提供对象池的基础上，也提供 ListPool 和 Dictionary Pool 等工具。
        * SingletonKit：单例工具集
        * TableKit：提供表格类数据结构的工具集


**典型的 QFrameowrk.Toolkits 代码**

```csharp
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace liangxiegame
{
    public partial class UIGamePanel : UIPanel
    {
        private ResLoader mResLoader;
        
        protected override void OnInit(IUIData uiData = null)
        {
            mResLoader = ResLoader.Allocate();
            
            mResLoader.LoadSync<GameObject>("GameplayRoot")
                .Instantiate()
                .Identity()
                .GetComponent<GameplayRoot>()
                .InitGameplayRoot();
            
            
            BtnPause.onClick.AddListener(() =>
            {
                AudioKit.PlaySound("btn_click");
                
                ActionKit.Sequence()
                    .Callback(() => BtnPause.interactable = false)
                    .Callback(() => BtnPause.PlayBtnFadeAnimation())
                    .Delay(0.3f)
                    .Callback(() => UIKit.OpenPanel<UIPausePanel>())
                    .Start(this);
            });
        }

        protected override void OnClose()
        {
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }
    }
}
```

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 02. 下载与安装


## 如何下载&安装
QFramework.Toolkits 的最新 unitypackage 存放在  git 仓库里，如下图所示:

![image.png](https://file.liangxiegame.com/e5ff0b03-e593-4720-b077-1b0af817cdf0.png)

git 仓库地址:
* github： https://github.com/liangxiegame/QFramework
* gitee（国内镜像）： https://gitee.com/liangxiegame/QFramework

点击包文件后，再点击下载按钮。

![image.png](https://file.liangxiegame.com/4cfddfa3-a27a-4454-9cd9-8ebeef6ae8cc.png)

下载完成后导入到 Unity 工程即可。

## 如何更新

当有新版本时，在 QFramework 的编辑器面板内就可以升级，打开方式 ctrl + e 或 ctrl + shift + e，打开后如下图所示：

![image.png](https://file.liangxiegame.com/af6ae4cb-312b-413b-a92e-a57c27820a60.png)

当有新版本时，图中的按钮会显示为"更新"，点击就可以一键更新。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)

# 03. CodeGenKit 脚本生成

在这一篇，我们学习几乎每个项目都要用到并且从中受益的功能：自动生成脚本并绑定，简称脚本生成。

## 基本使用

我们先在场景中，随便创建一些有父子结构的 GameObject，如下所示：

![image.png](https://file.liangxiegame.com/ed37997b-614b-4fb1-baa8-c23d7748c67d.png)

接着给 Player 挂上 ViewController，快捷键 （Alt + V），如下图所示：

![image.png](https://file.liangxiegame.com/cfb5f767-120f-4e0f-a69b-bdef1b6e9c98.png)


然后填写 刚刚添加的组件信息:

![image.png](https://file.liangxiegame.com/a2bc2a07-02bf-46e3-ad65-36309c290bce.png)


在这里，可以填写命名空间，要生成的脚本名，以及脚本生成的目录，当然这里也可以直接将要生成的目录拖到大方块中。

如果拖拽了目录，就会自动填写脚本生成目录，如下图所示：

![image.png](https://file.liangxiegame.com/41f2abac-2fcf-4c03-8ba0-ab45f71859f3.png)

之后，我们可以给 Player GameObject 一个子节点挂上 Bind 组件（快捷键，alt + b)，如下所示

![image.png](https://file.liangxiegame.com/e818f0e5-6bfc-436b-8f61-20fb90da4bd6.png)



Weapon 挂上的组件如下所示:
![image.png](https://file.liangxiegame.com/04e7c9a4-0bc6-4257-9793-41531c3faa64.png)


接下来我们可以点击图中的 生成代码按钮 或者是 Player 上 ViewController 的 生成代码按钮，两者点击哪个都可以。

点击之后，就会生成代码，等待编译，结果如下:

脚本目录:
![image.png](https://file.liangxiegame.com/d3fc5522-6655-4318-8bec-7f4721753110.png)

我们在看下场景中的 Player 的 Inspector 如下图所示：

![image.png](https://file.liangxiegame.com/07c51906-6c1d-49be-bb9b-faef8ce999ae.png)


我们看到，Player 自动获得了 Weapon 的引用。

而且，在 Player.cs 中可以直接访问到 Weapon，如下图所示:

![image.png](https://file.liangxiegame.com/3a9f0ac1-c05c-4cdf-b442-c33fadb6897a.png)

## 增量生成
我们再看下目录：

![image.png](https://file.liangxiegame.com/47398560-791c-4e41-8586-6b76347f2758.png)

这里有两个文件 Player 和 Player.Designer。

其中 Player 是用来给大家写逻辑用的，所以 Player 只会生成一次。

而 Player.Designer 每次点击生成代码都会重新生成。

我们看下 Player.Designer 的代码，如下:

```csharp
// Generate Id:471bf5e6-b60b-42b8-b5c8-b070a963ab4a
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class Player
	{

		public Transform Weapon;

	}
}
```

代码中只有一个 Weapon 。

接着，我们再给 Player 的另一个子 GameObject 挂上 Bind 脚本，如下:

![image.png](https://file.liangxiegame.com/acde8a1e-2e6f-4bee-8aa9-02cec82f2808.png)

然后点击生成代码，操作如下:

![image.png](https://file.liangxiegame.com/991db32f-8212-4d7a-8176-0065cebad93f.png)


生成之后，结果如下:

Player 多了一个 Ground Check
![image.png](https://file.liangxiegame.com/d769f7e4-1e70-4dfc-9962-27d6b99998a4.png)

再看下  Player.Designer 的代码，如下:

```csharp
// Generate Id:f512c2ed-6243-4a89-897e-bdaaabe50d63
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class Player
	{

		public Transform Weapon;

		public Transform GroundCheck;

	}
}
```

这次多了一个 GroundCheck。

而 Player 代码则未发生任何变化。

所以每次生成代码，Player.cs 只会生成一次，Player.Designer.cs 每次都重新生成，所以大家放心在 Player.cs 里写代码。

## 类型选择
之前我们用 Bind 绑定的 GameObject 都是 Transform 类型的，这次我们尝试绑定一下其他类型。

我们给 Weapon GameObject 挂上一个 Sprite Renderer 如下所示:

![image.png](https://file.liangxiegame.com/913a4dcb-7e35-433c-a50a-454614ddf89d.png)


然后，我们点击 Bind 的类型，显示如下：

![image.png](https://file.liangxiegame.com/9ff5d52d-61bb-43b7-b4f0-5e9c118329e1.png)

也就是说 Bind 可以选择挂在此 GameObject 上的组件。

我们选择 Sprite Render 类型，如下:

![image.png](https://file.liangxiegame.com/720ec620-1ca4-42b7-afa8-ec94ee846d06.png)

然后点击生成代码，结果如下:

![image.png](https://file.liangxiegame.com/dd6a1012-6721-4c71-9291-de008a5b8614.png)


Player 引用的  Weapon 变成了 Sprite Renderer 类型。

Player.Designer.cs 的代码变成了如下:

```csharp
// Generate Id:de59e915-d1b6-40aa-a8e5-6fc4a8bf8e3e
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class Player
	{

		public UnityEngine.SpriteRenderer Weapon;

		public Transform GroundCheck;

	}
}
```

Weapon 从原来的 Transform 类型变成了 SpriteRenderer 类型。

这样我们在 Player.cs 就可以拿到 SpriteRenderer 类型的 Weapon 了，如下图所示：

![image.png](https://file.liangxiegame.com/534d8275-5d63-4307-89a8-378722f0bffc.png)

## 如何设置默认的 命名空间 和 脚本生成目录
很简单，打开 QFramework 编辑器面板，（快捷键 ctrl + e 或 ctrl + shift + e)

![image.png](https://file.liangxiegame.com/4322e7cc-8f5e-4e45-abbe-d63110d2e605.png)

在 CodeGenKit 设置里就可以更改默认的命名空间和默认的脚本生成位置。

当然在这里生成了，也还是可以在 ViewController Inspector 上进行设置。

我们先改下命名空间和脚本生成路径，如下:

![image.png](https://file.liangxiegame.com/72f7df2a-40cb-443c-a1f3-f4c5d5656a4b.png)

然后我们创建一个 GameObject 挂上 ViewController 组件，结果如下:

![image.png](https://file.liangxiegame.com/f461ade5-8cf6-4bfd-a94d-c86f523cf8e8.png)

这样默认的命名空间就生效了。

## ViewController 与 ViewController 嵌套
ViewController 与 ViewController 之间可以嵌套

我们在 Player 的 Weapon GameObject 再创建一个 WeaponEffect GameObject 如下:

![image.png](https://file.liangxiegame.com/e9ef6d43-7e8c-42ff-9593-76dced914c7a.png)

然后将 WeaponEffect 挂上 Bind 脚本，如下:

![image.png](https://file.liangxiegame.com/0eed4e49-2a89-4d36-af02-4e42647cfe3a.png)

接着给 Weapon 挂一个 ViewController 脚本，如下:

![image.png](https://file.liangxiegame.com/e0b90b3b-cf9a-4688-ab6d-c73c8feb9f72.png)

我们将脚本生成目录修改一下，修改成与 Player.cs 同一个目录，如下:

![image.png](https://file.liangxiegame.com/f7c52c1e-0437-48a3-b3e1-7c9d77a080bf.png)


点击生成代码，如下所示:

![image.png](https://file.liangxiegame.com/29e139ca-9fc4-4422-9d4c-7831ad6d75c6.png)

生成完了之后，我们再将 Weapon 上的 Bind 类型改成 Weapon，如下:

![image.png](https://file.liangxiegame.com/54a25732-61ea-4dd9-84dd-7bb80d66fd2d.png)

然后点击 Bind 上的生成代码，结果如下:

![image.png](https://file.liangxiegame.com/83beb081-fb7a-48df-85f5-5caf01cac1fb.png)

这样 ViewController 与 ViewController 嵌套绑定就实现了。

在 Player.cs 中可以按照如下的方式调用 Weapon 的子 GameObject 如下:

![image.png](https://file.liangxiegame.com/c29ba2f9-39b0-436a-8084-781edaf959fe.png)

当然可以再 Weapon.cs 中写 Weapon 自己的逻辑。


## 生成 Prefab
在 ViewController 或 生成脚本的 Inspector 上，有一个生成 prefab 的选项

![image.png](https://file.liangxiegame.com/f88d06e7-2b95-47fe-ac91-c446fc550447.png)

勾选后，如下所示：

![image.png](https://file.liangxiegame.com/0b9de93d-12c9-498f-b38c-c2682aa98287.png)

这里可以修改要生成的目录，笔者选择和脚本生成的目录一致，如下:

![image.png](https://file.liangxiegame.com/7628fcb6-c9de-4fe5-9f80-8967d745b3aa.png)

然后点击，生成代码，结果如下:

场景中的 Player 变成了 prefab
![image.png](https://file.liangxiegame.com/9e71ac1b-874e-47dd-b9ab-8d64e605f8a1.png)

生成目录中也有了 prefab

![image.png](https://file.liangxiegame.com/18caef79-77b1-41a6-a102-9d53683be04d.png)

## Why？
为什么要搞一个  CodeGenKit？

因为创建脚本目录、创建脚本文件、声明成员变量或者通过 transform.Find 获取子节点的引用、然后挂脚本、拖拽赋值，这些工作量非常多，而且很繁重，如果能够把这部分工作量通过代码生成并自动赋值的方式给优化掉，那么项目的开发效率就会得到及大地提升。

CodeGenKit 中的  ViewController 除了可以用于普通的 GameObject，还可以支持 NGUI 和 UGUI 等 UI 组件。

好了，关于脚本生成的功能介绍到这里。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 04. ActionKit 时序动作执行系统
AciontKit 是一个时序动作执行系统。

游戏中，动画的播放、延时、资源的异步加载、Tween 的执行、网络请求等，这些全部都是时序任务，而 ActionKit，可以把这些任务全部整合在一起，使用统一的 API，来对他们的执行进行**计划**。

OK，我们先看下 ActionKit的基本用法。

## 延时回调

示例代码如下:

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class DelayExample : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Start Time:" + Time.time);
            
            ActionKit.Delay(1.0f, () =>
            {
                Debug.Log("End Time:" + Time.time);
                
            }).Start(this);
        }
    }
}

// 输出结果
// Start Time: 0
// End Time: 1.00781
```

## 序列和完成回调
```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class SequenceAndCallback : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Sequence Start:" + Time.time);

            ActionKit.Sequence()
                .Callback(() => Debug.Log("Delay Start:" + Time.time))
                .Delay(1.0f)
                .Callback(() => Debug.Log("Delay Finish:" + Time.time))
                .Start(this, _ => { Debug.Log("Sequence Finish:" + Time.time); });
        }
    }
}
// 输出结果
// Sequence Start:0
// Delay Start:0
// Delay Finish:1.00537
// Sequence Finish:1.00537
```

## 帧延时

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class DelayFrameExample : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Delay Frame Start FrameCount:" + Time.frameCount);
            
            ActionKit.DelayFrame(1, () => { Debug.Log("Delay Frame Finish FrameCount:" + Time.frameCount); })
                .Start(this);


            ActionKit.Sequence()
                .DelayFrame(10)
                .Callback(() => Debug.Log("Sequence Delay FrameCount:" + Time.frameCount))
                .Start(this);

            // ActionKit.Sequence()
            //      .NextFrame()
            //      .Start(this);

            ActionKit.NextFrame(() => { }).Start(this);
        }
    }
}

// 输出结果
// Delay Frame Start FrameCount:1
// Delay Frame Finish FrameCount:2
// Sequence Delay FrameCount:11
```

## 条件执行

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class ConditionExample : MonoBehaviour
    {
        private void Start()
        {
            ActionKit.Sequence()
                .Callback(() => Debug.Log("Before Condition"))
                .Condition(() => Input.GetMouseButtonDown(0))
                .Callback(() => Debug.Log("Mouse Clicked"))
                .Start(this);
        }
    }
}

// 输出结果
// Before Condition
// 鼠标左键按下后
// Mouse Clicked
```

## 重复执行

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class RepeatExample : MonoBehaviour
    {
        private void Start()
        {
            ActionKit.Repeat()
                .Condition(() => Input.GetMouseButtonDown(0))
                .Callback(() => Debug.Log("Mouse Clicked"))
                .Start(this);


            ActionKit.Repeat(5)
                .Condition(() => Input.GetMouseButtonDown(1))
                .Callback(() => Debug.Log("Mouse right clicked"))
                .Start(this, () =>
                {
                    Debug.Log("Right click finished");
                });
        }
    }
}

// 输出结果
// 每次点击鼠标左键都会输出：Mouse Clicked 
// 点击鼠标右键，只会输出五次：Mouse right clicked，第五次输出  Right click finished
// 
```

## 并行执行
```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class ParallelExample : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Parallel Start:" + Time.time);

            ActionKit.Parallel()
                .Delay(1.0f, () => { Debug.Log(Time.time); })
                .Delay(2.0f, () => { Debug.Log(Time.time); })
                .Delay(3.0f, () => { Debug.Log(Time.time); })
                .Start(this, () =>
                {
                    Debug.Log("Parallel Finish:" + Time.time);
                });
        }
    }
}

// 输出结果
// Parallel Start:0
// 1.030884
// 2.025135
// 3.018883
// Parallel Finish:3.018883
```


## 更复杂的示例

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class ComplexExample : MonoBehaviour
    {
        private void Start()
        {
            ActionKit.Sequence()
                .Callback(() => Debug.Log("Sequence Start"))
                .Callback(() => Debug.Log("Parallel Start"))
                .Parallel(p =>
                {
                    p.Delay(1.0f, () => Debug.Log("Delay 1s Finished"))
                        .Delay(2.0f, () => Debug.Log("Delay 2s Finished"));
                })
                .Callback(() => Debug.Log("Parallel Finished"))
                .Callback(() => Debug.Log("Check Mouse Clicked"))
                .Sequence(s =>
                {
                    s.Condition(() => Input.GetMouseButton(0))
                        .Callback(() => Debug.Log("Mouse Clicked"));
                })
                .Start(this, () =>
                {
                    Debug.Log("Finish");
                    
                });
        }
    }
}

// Sequence Start
// Parallel Start
// Delay 1s Finished
// Delay 2s Finished
// Parallel Finished
// Check Mouse Clicked
// 此时按下鼠标左键
// Mouse Clicked
// Finish
```


## 自定义动作

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class CustomExample : MonoBehaviour
    {
        class SomeData
        {
            public int ExecuteCount = 0;
        }

        private void Start()
        {
            ActionKit.Custom(a =>
            {
                a
                    .OnStart(() => { Debug.Log("OnStart"); })
                    .OnExecute(dt =>
                    {
                        Debug.Log("OnExecute");

                        a.Finish();
                    })
                    .OnFinish(() => { Debug.Log("OnFinish"); });
            }).Start(this);
            
            // OnStart
            // OnExecute
            // OnFinish

            ActionKit.Custom<SomeData>(a =>
                {
                    a
                        .OnStart(() =>
                        {
                            a.Data = new SomeData()
                            {
                                ExecuteCount = 0
                            };
                        })
                        .OnExecute(dt =>
                        {
                            Debug.Log(a.Data.ExecuteCount);
                            a.Data.ExecuteCount++;

                            if (a.Data.ExecuteCount >= 5)
                            {
                                a.Finish();
                            }
                        }).OnFinish(() => { Debug.Log("Finished"); });
                })
                .Start(this);
            
            // 0
            // 1
            // 2
            // 3
            // 4
            // Finished

            // 还支持 Sequence、Repeat、Spawn 等
            // Also support sequence repeat spawn
            // ActionKit.Sequence()
            //     .Custom(c =>
            //     {
            //         c.OnStart(() => c.Finish());
            //     }).Start(this);
        }
    }
}
```


## 协程支持

```csharp
using System.Collections;
using UnityEngine;

namespace QFramework.Example
{
    public class CoroutineExample : MonoBehaviour
    {
        private void Start()
        {
            ActionKit.Coroutine(SomeCoroutine).Start(this);
            
            SomeCoroutine().ToAction().Start(this);

            ActionKit.Sequence()
                .Coroutine(SomeCoroutine)
                .Start(this);
        }

        IEnumerator SomeCoroutine()
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("Hello:" + Time.time);
        }
    }
}

// 输出结果
// Hello:1.002077
// Hello:1.002077
// Hello:1.002077
```


## 全局 Mono 生命周期
```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class GlobalMonoEventsExample : MonoBehaviour
    {
        void Start()
        {
            ActionKit.OnUpdate.Register(() =>
            {
                if (Time.frameCount % 30 == 0)
                {
                    Debug.Log("Update");
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            ActionKit.OnFixedUpdate.Register(() =>
            {
                // fixed update code here
                // 这里写 fixed update 相关代码
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            ActionKit.OnLateUpdate.Register(() =>
            {
                // late update code here
                // 这里写 late update 相关代码
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            ActionKit.OnGUI.Register(() =>
            {
                GUILayout.Label("See Example Code");
                GUILayout.Label("请查看示例代码");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            ActionKit.OnApplicationFocus.Register(focus =>
            {
                Debug.Log("focus:" + focus);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            ActionKit.OnApplicationPause.Register(pause =>
            {
                Debug.Log("pause:" + pause);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            ActionKit.OnApplicationQuit.Register(() =>
            {
                Debug.Log("quit");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }
}
```

## DOTween 集成

需要先提前装好 DOTween。

然后导入 Example 中的如下包。

![image.png](https://file.liangxiegame.com/63e3eba5-0dfc-4d53-af56-242d6a308124.png)

导入之后，就可以用 让 ActionKit 跑 DOTween 了，代码如下:

```csharp
using DG.Tweening;
using UnityEngine;

namespace QFramework.Example
{
    public class DOTweenExample : MonoBehaviour
    {
        private void Start()
        {
            // 使用 Custom 就可以方便接入
            // Just Use Custom 
            ActionKit.Custom(c =>
            {
                c.OnStart(() => { transform.DOLocalMove(Vector3.one, 0.5f).OnComplete(c.Finish); });
            }).Start(this);
            
            // 也可以自定义 IAction
            // Also implement with IAction
            DOTweenAction.Allocate(() => transform.DOLocalRotate(Vector3.one, 0.5f))
                .Start(this);
            
            // 使用 ToAction
            // Use ToAction
            DOVirtual.DelayedCall(2.0f, () => LogKit.I("2.0f")).ToAction().Start(this);

            // 链式 API 支持
            // fluent api support
            ActionKit.Sequence()
                .DOTween(() => transform.DOScale(Vector3.one, 0.5f))
                .Start(this);
        }
    }
    
  
}
```

## UniRx 集成
需要先提前装好 UniRx。

然后导入 Example 中的如下包。

![image.png](https://file.liangxiegame.com/9b687ee1-83ec-49f5-b315-5795cc72b3ce.png)


导入成功后，使用示例如下:

```csharp
using System;
using UniRx;
using UnityEngine;

namespace QFramework.Example
{
    public class UniRxExample : MonoBehaviour
    {
        void Start()
        {
            // 可以直接使用 Custom
            // directly use custom
            ActionKit.Custom(c =>
            {
                c.OnStart(() => { Observable.Timer(TimeSpan.FromSeconds(1.0f)).Subscribe(_ => c.Finish()); });
            }).Start(this, () => LogKit.I("1.0f"));

            // 使用 UniRxAction 不方便...
            // Use UniRxAction 
            UniRxAction<long>.Allocate(() => Observable.Timer(TimeSpan.FromSeconds(2.0f))).Start(this,()=>LogKit.I("2.0f"));


            // 使用 ToAction 方便易用
            // Use ToAction
            Observable.Timer(TimeSpan.FromSeconds(3.0f)).ToAction().Start(this, () => LogKit.I("3.0f"));

            ActionKit.Sequence()
                .UniRx(() => Observable.Timer(TimeSpan.FromSeconds(4.0f)))
                .Start(this, () => LogKit.I("4.0f"));
        }
    }
 
}
```

好了，关于 ActionKit 的介绍就到这里。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 05. ResKit 资源管理&开发解决方案

## Res Kit 简介
Res Kit，是资源管理&快速开发解决方案

**特性如下:**
* 可以使用一个 API 从  dataPath、Resources、StreammingAssetPath、PersistentDataPath、网络等地方加载资源。
* 基于引用计数，简化资源加载和卸载。
* 拥抱游戏开发流程中的不同阶段
  * 开发阶段不用打 AB 直接从 dataPath 加载。
  * 测试阶段支持只需打一次 AB 即可。
* 可选择生成资源名常量代码，减少拼写错误。
* 异步加载队列支持
* 对于 AssetBundle 资源，可以只通过资源名而不是 AssetBundle 名 + 资源名 加载资源，简化 API 使用。


## Res Kit 快速入门
我们知道，在一般情况下，有两种方式可以让我们实现动态加载资源：
* Resources
* AssetBundle

在 Res Kit 中，推荐使用 AssetBundle 的方式进行加载，因为 Res Kit 所封装的 AssetBundle 方式，比 Resources 的方式更好用。

除了 Res Kit 中的 AsseBundle 方式更易用外，AssetBundle 本身相比 Resources 有更多的优点，比如更小的包体，支持热更等。

废话不多说，我们看下 Res Kit 的基本使用。

Res Kit 在开发阶段，分为两步。
* 标记资源
* 写代码

在开始之前，我们要确保，当前的 Res Kit 环境为模拟模式。

按下快捷键 ctrl + e 或者 ctrl + shift + r ，我们可以看到如下面板:

![image.png](https://file.liangxiegame.com/d6d1ac25-4c60-4b42-81ec-51b1628b640a.png)

确保模拟模式勾选之后，我们就可以进入使用流程了。

### 1. 资源标记

在 Asset 目录下，只需对需要标记的文件或文件夹右键->@ResKit- AssetBundle Mark，如下所示：

![image.png](https://file.liangxiegame.com/2d793421-94cb-457f-80da-ee976f700f02.png)

标记完了，

标记成功后，我们可以看到如下结果：

1. 该资源标记的选项为勾选状态

![image.png](https://file.liangxiegame.com/1ced7efd-a328-4c5e-a76a-4a85020acdd2.png)

2. 该资源的 AssetLabel 中的名字如下
   ![image.png](https://file.liangxiegame.com/a7e20396-e553-4ead-8291-e4395fe53b30.png)

这样就标记成功了。

这里注意，一次标记就是一个 AssetBundle，如果想要让 AssetBundle 包含多个资源，可以将多个资源放到一个文件夹中，然后标记文件夹。


### 2.资源加载
接下来我们直接写资源加载的代码即可，代码如下，具体的代码含义，看注释即可。。

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class ResKitExample : MonoBehaviour
    {
        // 每个脚本都需要
        private ResLoader mResLoader = ResLoader.Allocate();

        private void Start()
        {
            // 项目启动只调用一次即可
            ResKit.Init();
            
            // 通过资源名 + 类型搜索并加载资源（更方便）
            var prefab = mResLoader.LoadSync<GameObject>("AssetObj");
            var gameObj = Instantiate(prefab);
            gameObj.name = "这是使用通过 AssetName 加载的对象";

            // 通过 AssetBundleName 和 资源名搜索并加载资源（更精确）
            prefab = mResLoader.LoadSync<GameObject>("assetobj_prefab", "AssetObj");
            gameObj = Instantiate(prefab);
            gameObj.name = "这是使用通过 AssetName  和 AssetBundle 加载的对象";
        }

        private void OnDestroy()
        {
            // 释放所有本脚本加载过的资源
            // 释放只是释放资源的引用
            // 当资源的引用数量为 0 时，会进行真正的资源卸载操作
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }
    }
}
```

将此脚本挂到任意 GameObject 上，运行后，结果如下:


![image.png](https://file.liangxiegame.com/04cd1727-b7ad-436d-988c-80b70c0fc106.png)

资源加载成功。

## 模拟模式与非模拟模式

### AssetBundle 的不便之处
在使用 Res Kit 之前，相信大家多多少少接触过 AssetBundle。 有的童鞋可能是在项目中用过 AssetBundle，有的童鞋可能只是简单学习过 AssetBundle。总之，AssetBundle 在不通过 Res Kit 使用之前，总结下来就两个字：麻烦。

AssetBundle 麻烦在哪里呢？

首先 AssetBundle，需要打包才能在运行时加载资源。而打包需要我们写编辑器扩展脚本，在编辑器扩展脚本中还要处理平台和路径相关的逻辑。

在运行时，还需要根据平台和路径去加载对应的 AssetBundle。

这些操作想想就比较头痛。

既然 AssetBundle 这么麻烦，我们为什么还要用 AssetBundle 呢？

因为 AssetBundle 可以给项目带来更好的性能，而且 AssetBundle 支持热更新。

有了这两个优势，AssetBundle 就成了很多项目的必然选择。

而 Res Kit 中，为了解决频繁打包的问题，引入了一个概念：模拟模式（Simulation Mode）。

#### 模拟模式（Simulation Mode）

**什么是模拟模式？**

顾名思义，就是模拟加载 AssetBundle 的模式，这里只是模拟，并没有真正去加载 AssetBundle，而是去加载 Application.dataPath 目录下的资源，也就是 Assets 目录下的资源。

**这样做有什么好处呢？**

好处就是每当有资源修改的时候，就不用再打 AB 包了，就可以在运行时加载到修改后的资源。

如果是非模拟模式下，每当有资源修改时，就需要再打一次 AB 包，才能加载到修改后的资源。

所以一个模拟模式，解决了频繁打 AB 包的问题，从而在开发阶段提高我们的开发效率。

那么在使用 Res Kit 的时候，模拟模式对应的阶段是开发阶段，那么非模拟模式对应的是什么阶段呢？

答案就是真机阶段。

### 开发阶段、真机阶段
开发阶段、真机阶段并不是 Unity 提供的概念，而是笔者在迭代 Res Kit 中提出的两个概念。

这两个概念很容易理解：
* 开发阶段：开发逻辑的阶段，需要编写大量的逻辑，大部分情况下都在 Unity Editor 环境下开发。
* 真机阶段：需要在真机上运行的阶段，这个阶段主要是做大量的测试或者真正发布了。

相信有点规模的项目都会分阶段出来的，比如开发阶段、测试阶段、生产阶段等等，大家理解起来应该不难。

接下来简单分析一下开发阶段、真机阶段的特点。

**开发阶段**
在开发阶段，开发者需要写大量的逻辑，而且资源的目录还没有稳定，一般在开发过程中会有很大的变化。
如果每次资源的修改都需要打 AB 包的话，会非常影响开发进度。

**真机阶段**
真机阶段，一般就是一个版本的逻辑都写完了，只需要做一些测试和 debug 工作。在这个阶段，资源目录都稳定了，不需要做很大的调整。

在真机阶段，每次打 App 包之前，只需要 Build 一次 AB 即可。

当然，在 Unity Editor 环境中，可以取消勾选模拟模式，这样在 Unity Editor 环境下可以加载真正的 AssetBundle 包。

在上一篇文章所说的，拥抱各个开发阶段指的就是为开发阶段、和真机阶段做了考虑。

此篇的内容就这些。

### 小结
* 开发阶段：
  *  模拟模式
* 真机阶段：
  * 每次打 App 包之前，打一次 AB 包。
  * 可以在 Unity Editor 环境下，取消勾选模拟模式，这时在运行时加载的资源则是真正的 AssetBundle 资源


## 如何打 AssetBundle（真机模式）


![image.png](https://file.liangxiegame.com/bcc21643-8c4a-4f6f-b3a9-db1ec3071119.png)

取消勾选模拟模式情况下，点击打 AB 包 即可。


## 异步加载
异步加载代码如下:
``` csharp
// 添加到加载队列
mResLoader.Add2Load("TestObj",(succeed,res)=>{
    if (succeed) 
    {
        res.Asset.As<GameObject>()
						.Instantiate();
    }
});

// 执行异步加载
mResLoader.LoadAsync();
```
与 LoadSync 不同的是，异步加载是分两步的，第一步是添加到加载队列，第二步是执行异步加载。

这样做是为了支持同时异步加载多个资源的。

## 异步加载
代码如下:
```csharp
using System.Collections;
using UnityEngine;

namespace QFramework.Example
{
    public class AsyncLoadExample : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return ResKit.InitAsync();

            var resLoader = ResLoader.Allocate();
            
            resLoader.Add2Load<GameObject>("AssetObj 1",(b, res) =>
            {
                if (b)
                {
                    res.Asset.As<GameObject>().Instantiate();
                }
            });

            // AssetBundleName + AssetName
            resLoader.Add2Load<GameObject>("assetobj 2_prefab","AssetObj 2",(b, res) =>
            {
                if (b)
                {
                    res.Asset.As<GameObject>().Instantiate();
                }
            });
            
            resLoader.Add2Load<GameObject>("AssetObj 3",(b, res) =>
            {
                if (b)
                {
                    res.Asset.As<GameObject>().Instantiate();
                }
            });

            resLoader.LoadAsync(() =>
            {
                // 加载成功 5 秒后回收
                ActionKit.Delay(5.0f, () =>
                {
                    resLoader.Recycle2Cache();

                }).Start(this);
            });
        }

    }
}
```

结果如下:

![image.png](https://file.liangxiegame.com/8ad406e4-f59c-43d2-bd4a-e7de57560958.png)


## 加载场景

注意：标记场景时要确保，一个场景是一个 AssetBundle。

```csharp
using UnityEngine;

namespace QFramework.Example
{
	public class LoadSceneExample : MonoBehaviour
	{
		private ResLoader mResLoader = null;

		void Start()
		{
			ResKit.Init();

			mResLoader = ResLoader.Allocate();

			// 同步加载
			mResLoader.LoadSceneSync("SceneRes");

			// 异步加载
			mResLoader.LoadSceneAsync("SceneRes");

			// 异步加载
			mResLoader.LoadSceneAsync("SceneRes", onStartLoading: operation =>
			{
				// 做一些加载操作
			});
		}

		private void OnDestroy()
		{
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}
```


## 加载 Resources 中的资源

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
	public class LoadResourcesResExample : MonoBehaviour
	{
		public RawImage RawImage;
		
		private ResLoader mResLoader = ResLoader.Allocate();
		
		private void Start()
		{
			//  加载 Resources 目录里的资源不用调用 ResKit.Init
			
			RawImage.texture = mResLoader.LoadSync<Texture2D>("resources://TestTexture");
		}

		private void OnDestroy()
		{
			Debug.Log("On Destroy ");
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}
```


## 关联对象管理

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class ResLoaderRelateUnloadAssetExample : MonoBehaviour
    {
        // Use this for initialization
        IEnumerator Start()
        {
            var image = transform.Find("Image").GetComponent<Image>();

            ResKit.Init();

            var resLoader = ResLoader.Allocate();
            
            var texture2D = resLoader.LoadSync<Texture2D>("TextureExample1");

            // create Sprite 扩展
            var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one * 0.5f);

            image.sprite = sprite;

            // 添加关联的 Sprite
            resLoader.AddObjectForDestroyWhenRecycle2Cache(sprite);

            yield return new WaitForSeconds(5.0f);
            
            // 当释放时 sprite 也会销毁
            resLoader.Recycle2Cache();
            resLoader = null;
        }
    }
}
```

## SpriteAtlas 加载

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace QFramework
{
	/// <inheritdoc />
	/// <summary>
	/// 参考:http://www.cnblogs.com/TheChenLin/p/9763710.html
	/// </summary>
	public class TestSpriteAtlas : MonoBehaviour
	{
		[SerializeField] private Image mImage;

		// Use this for initialization
		private IEnumerator Start()
		{
			var loader = ResLoader.Allocate();

			ResKit.Init();

			var spriteAtlas = loader.LoadSync<SpriteAtlas>("spriteatlas");
			var square = spriteAtlas.GetSprite("shop");
			
			loader.AddObjectForDestroyWhenRecycle2Cache(square);

			mImage.sprite = square;

			yield return new WaitForSeconds(5.0f);

			loader.Recycle2Cache();
			loader = null;
		}
	}
}
```

## 加载网络图片

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class NetImageExample : MonoBehaviour
    {
        ResLoader mResLoader = ResLoader.Allocate();

        // Use this for initialization
        void Start()
        {
            var image = transform.Find("Image").GetComponent<Image>();
            
            mResLoader.Add2Load<Texture2D>(
                "http://pic.616pic.com/ys_b_img/00/44/76/IUJ3YQSjx1.jpg".ToNetImageResName(),
                (b, res) =>
                {
                    if (b)
                    {
                        var texture = res.Asset as Texture2D;

                        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                            Vector2.one * 0.5f);
                        image.sprite = sprite;
                        mResLoader.AddObjectForDestroyWhenRecycle2Cache(sprite);
                    }
                });
            
            mResLoader.LoadAsync();
        }
        
        private void OnDestroy()
        {
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }
    }
}
```


## 从 PersistentDataPath 加载图片
```csharp
namespace QFramework.Example
{
	using System.Collections;
	using UnityEngine.UI;
	using UnityEngine;
	
	public class ImageLoaderExample : MonoBehaviour
	{
		private ResLoader mResLoader = null;

		private IEnumerator Start()
		{
			ResMgr.Init();
			
			mResLoader = ResLoader.Allocate();

			// local image
			var localImageUrl = "file://" + Application.persistentDataPath + "/Workspaces/lM1wmsLQtfzRQc6fsdEU.jpg";

			mResLoader.Add2Load(localImageUrl.ToLocalImageResName(),
				delegate(bool b, IRes res)
				{
					Debug.LogError(b);
					if (b)
					{
						var texture2D = res.Asset as Texture2D;
						transform.Find("Image").GetComponent<Image>().sprite = Sprite.Create(texture2D,
							new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one * 0.5f);
					}
				});
			
			mResLoader.LoadAsync();
			
			
			yield return new WaitForSeconds(5.0f);
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}
```


## 自定义 Res

ResKit 提供了 自定义 Res ，通过自定义 Res 可以非常方便地自定义 Res 的加载来源，比如 PersistentDataPath、StreamingAssetPath、AssetBundle 等，甚至是内存中的 GameObject 等资产，还可以集成 Addressables 或者其他的资源管理方案，ResKit 内置支持的 AssetBundle、Resources、网络图片加载、PersistentDataPath 图片加载都是通过自定义 Res 的方式扩展而来。

我们看下自定义 Res 的用法，如下:
```csharp
using UnityEngine;

namespace QFramework
{
    public class CustomResExample : MonoBehaviour
    {
        // 自定义的 Res
        public class MyRes : Res
        {
            public MyRes(string name)
            {
                mAssetName = name;
            }

            // 同步加载（自己实现）
            public override bool LoadSync()
            {
                // Asset = 加载的结果给 Asset 赋值 
                State = ResState.Ready;
                return true;
            }

            // 异步加载(自己实现)
            public override void LoadAsync()
            {
                // Asset = 加载的结果给 Asset 赋值 
                State = ResState.Ready;
            }
            

            // 释放资源（自己实现)
            protected override void OnReleaseRes()
            {
                // 卸载操作
                // Asset = null
                State = ResState.Waiting;
            }
        }

        // 自定义的 Res 创建器（包含识别功能）
        public class MyResCreator : IResCreator
        {
            // 识别
            public bool Match(ResSearchKeys resSearchKeys)
            {
                return resSearchKeys.AssetName.StartsWith("myres://");
            }

            // 创建
            public IRes Create(ResSearchKeys resSearchKeys)
            {
                return new MyRes(resSearchKeys.AssetName);
            }
        }

        void Start()
        {
            // 添加创建器
            ResFactory.AddResCreator<MyResCreator>();

            var resLoader = ResLoader.Allocate();

            var resSearchKeys = ResSearchKeys.Allocate("myres://hello_world");
            
            var myRes =  resLoader.LoadResSync(resSearchKeys);
            
            resSearchKeys.Recycle2Cache();
            
            Debug.Log(myRes.AssetName);
            Debug.Log(myRes.State);
        }
    }
}
```

非常简单。

## 代码生成

Res Kit 支持代码生成，生成按钮的位置如下所示:
![image.png](https://file.liangxiegame.com/e482f08e-2e8e-4b43-84bf-f32722cc5f5c.png)
点击生成代码即可，生成后结果如下。
![image.png](http://file.liangxiegame.com/0ea13581-4960-4bc8-bbf1-b49a03455271.png)

生成了 QAssets 代码文件，代码内容如下:

```csharp
namespace QAssetBundle
{
  
    public class Testobj_prefab
    {
        public const string BundleName = "testobj_prefab";
        public const string TESTOBJ = "testobj";
    }
    public class Testsprite_png
    {
        public const string BundleName = "testsprite_png";
        public const string TESTSPRITE = "testsprite";
    }
}

```

生成了代码，那么在写资源加载的代码的时候就会爽的飞起，如下图示:
![image.png](http://file.liangxiegame.com/7b8ae854-aafe-49d8-9318-5f7d1190c8cc.png)

图中，给出了资源名字的提示。

这样就不容易出现字符串的拼写错误了。

## ResLoader 推荐用法

ResLoader 的推荐用法，是一个需要加载的单元申请一个 ResLoader。

代码如下:

```csharp
using QF.Res;
using QF.Extensions;
using UnityEngine;

namespace QF.Example 
{
	public class TestResKit : MonoBehaviour 
	{
		/// <summary>
		/// 每一个需要加载资源的单元（脚本、界面）申请一个 ResLoader
		/// ResLoader 本身会记录该脚本加载过的资源
		/// </summary>
		/// <returns></returns>
		ResLoader mResLoader = ResLoader.Allocate ();
  
    ...
  
        void Destroy()
		{
			// 释放所有本脚本加载过的资源
			// 释放只是释放资源的引用
			// 当资源的引用数量为 0 时，会进行真正的资源卸载操作
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}
```

在以上代码中，TestResKit 是一个需要加载资源的单元。

**这个单元是什么意思呢？**

其实很简单，单元可以是 UIPanel （界面），或者任何需要加载资源服务的 MonoBehaviour。

## ResLoader 的职责

ResLoader 的职责字如其意，就是负责加载资源的，即资源加载器。

一个 ResLoader 会记录所有它加载过的资源。

这样它在释放资源的时候只需要根据加载记录，进行释放即可。

ResLoader 与 单元（Test 脚本）的示意图如下:
![image.png](http://file.liangxiegame.com/296b0166-bdea-47d5-ac87-4b55c91df16f.png)

这里我们要注意，ResLoader 不是进行真正的资源加载操作，而是进行资源的引用获取。

真正的资源加载是在 ResMgr 中完成，这个过程用户是无法感知的到的。

ResLoader 获取资源引用的过程如下:

1. 从 ResLoader 的引用记录中查询是否已经获取了引用，如果之前已经在 ResLoader 记录过资源引用则返回资源。否则执行 2.
2. 从 ResMgr 中查询是否已经有资源对象，如果有资源对象，返回资源，并在 ResLoader 中记录引用，同时对资源对象进行引用计数 +1 操作，否则执行 3.
3. 让 ResMgr 进行资源加载，同时创建资源对象，剩下的步骤同 2。

大致的访问资源的过程就是如此，不理解的童鞋不要紧，因为对使用上来说不重要。

我们只需要知道，建议每个需要加载的脚本申请一个 ResLoader，是为了更方便地让大家进行资源管理。

不管这个脚本加载过多少个东西，也不管别的脚本加载过多少，只需要各自脚本释放自己的 ResLoader 即可。

因为每个资源对象对集成了引用计数的。

## 申请 ResLoader 的消耗

几乎没有消耗，因为 ResLoader 是从对象池中申请的。


## WebGL 注意事项补充

在 WebGL 平台 ResKit 加载 AssetBundle 资源只支持异步加载。


异步初始化
```csharp
StartCoroutine(ResKit.InitAsync());
// 或者
ResKit.InitAsync().ToAction().StartGlobal();
```

异步加载资源
* 先 Add2Load
* 再调用 LoadAsync()


好了，ResKit 的功能就全部介绍完了。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)





# 06. UIKit 界面管理&快速开发解决方案

## UI Kit 简介

UI Kit 是一套界面管理&快速开发解决方案

UI Kit 的特性如下：

* 界面管理
* 层级管理
* 代码生成及组件自动绑定（底层用的 ViewController)


## UI Kit 基本使用

UI Kit 本身有一套推荐使用的工作流程，而此工作流程的设计是为了使每个界面只负责展示数据和监听用户输入，界面与界面之间互相独立，并且可独立测试。

下面我们将介绍如何制作一个游戏主页（UIBasicPanel）。

首先我们先创建一个场景：TestUIBasicPanel，如下图所示:

![image-20220725171613899](https://file.liangxiegame.com/1b23e2de-9af1-4ce7-b5fd-7f1af8f1688b.png)

在这里大家要注意一下，UI Kit 推荐每个界面创建一个对应的测试场景，要保证每个界面是可以独立测试的。



接着打开 TestUIBasicPanel 如下所示：

![image-20220725171641152](https://file.liangxiegame.com/0f2accd3-1836-4d38-9858-3cc7828dc72f.png)

我们拖出来一个 UIRoot prefab，如下所示:

![image-20220725171556290](https://file.liangxiegame.com/3804bb25-0112-4798-bee8-d1b9bf13f134.png)

这里非常清晰地可以看到 UI Kit 所支持的所有层级。

接着我们在 Design 层级下创建一个 Panel（右击 Design->UI->Panel) ，并命名为 UIBasicPanel，如下所示:

![image-20220725171752004](https://file.liangxiegame.com/6b626de9-223f-4d32-9031-285c1d537c75.png)

这里要说一点，Design 层级，顾名思义就是用来做设计的层级，什么是设计？就是拼界面，这个层级就是专门用来拼界面的，Design 层级会在运行的时候会自动隐藏掉自己以及所有的子节点。

OK，接下来，我们将 UIBasic 制作成 prefab，将其放到 Assets/Art/UIPrefabs 目录下，如果没有这个目录就自己手动创建一下。

放入后如下图所示:

![image-20220725172338703](https://file.liangxiegame.com/584225aa-eb8a-4a7d-a44e-32ecd6732aa2.png)

Assets/Art/UIPrefab 这个目录是怎么来的呢？它是 QFramework 约定的专门放置 UI 界面 prefab 的位置。而 Assets/Art 是框架推荐存放资源的位置，当然关于资源的存放位置只是推荐，而不是强制的。

但是 UI 界面的 prefab 必须放在 Assets/Art/UIPrefab 目录下，因为这个部分在代码生成的时候需要。

那么有的童鞋可能会问，Assets/Art/UIPrefab 这个路径可以不可以更改？

当然可以，更改的方式也很简单，就是打开包管理面板(QFramework/Preference ctrl + e)，打开后可以看到如下面板:

![image-20220725172030417](https://file.liangxiegame.com/146ffecf-e9b8-46fc-8e6e-1b95832256fd.png)

详细的设置方式在上边介绍了，这里就不多介绍了。

接下来需要将 UIHomePanel prefab 标记为 AssetBundle，如下图所示:

![image-20220725172438374](https://file.liangxiegame.com/f999ac89-5b91-453f-a865-1dea7045a1d3.png)

标记成功后。

会看到如下结果:

![image-20220725172140857](https://file.liangxiegame.com/3c313f23-2350-4e88-a7b3-88958e9e6219.png)

接着，我们在这里要确保一件事情，就是 Res Kit 需要保证当前环境是模拟环境（Simulation Mode），具体看面板中的如下选项是否是勾上即可。

![image-20220725172213241](https://file.liangxiegame.com/98e4043d-bd1b-4a29-9363-890f9f545b12.png)

确保勾上之后，我们就开始生成代码，具体操作如下所示（右键->@UI-Kit Create UI Code):

![image-20220725172505171](https://file.liangxiegame.com/de51f6ad-fef7-46dd-adee-c286b77511fe.png)

点击之后等待编译，编译结束后，我们看到如下结果:

**脚本生成成功**

![image-20220725172532517](https://file.liangxiegame.com/7f3959f1-66fc-44de-a8b1-97e31383dcdd.png)



**脚本自动挂载了 UIBasicPanel Prefab 上**

![image-20220725172550535](https://file.liangxiegame.com/b386fd53-1f84-4ede-a79c-a20533361fa1.png)

到此，代码生成部分就介绍完了。

接着，我们想办法让这个场景独立运行。

现在，我们直接运行场景，是不会加载任何界面的，如下所示:

![image-20220725172721472](https://file.liangxiegame.com/4ecc5084-4f8b-4ac3-a62b-d4b7a4c9967e.png)

如何让这个场景加载 UIBasicPanel 呢？

很简单，使用 UIPanelTester 如下所示:

![image-20220725172923702](https://file.liangxiegame.com/dd934412-abfd-4d92-9906-3e084a2b761d.png)

按照图中样子设置就好，然后运行场景。
结果如下:

![image-20220725173003435](https://file.liangxiegame.com/71acdc41-14a2-42c3-a9c7-6fef2af9757b.png)

图中成功加载了改界面。

这样，最基本的 UIBasicPanel 测试场景就算搭建完了，同时我们是完全按照 QFramework 推荐的工作流程完成的。

虽然步骤会稍微繁琐一点，但是用一段时间大家就会觉得这是值得的。

OK，接下来我们来介绍控件的自动绑定功能。

## 控件的自动绑定功能

我们在 UIBasicPanel 上添加一些按钮，并在每个按钮上挂上 Bind 脚本，如下所示:

![image-20220725173212119](https://file.liangxiegame.com/9f099faf-d488-491c-bb74-444816c48d6f.png)

接着 Apply UIBasicPanel，如下所示:

![image-20220725173259294](https://file.liangxiegame.com/5782a40b-f683-41a2-9db7-7481853aa6a8.png)

这里要注意，一定要选定 UIBasicPanel 再进行 Apply，千万别选成 UIRoot 了。

Apply 之后，再次生成一次代码，操作如下所示:

![image-20220725172505171](https://file.liangxiegame.com/de51f6ad-fef7-46dd-adee-c286b77511fe.png)

生成之后，结果如下:

![image-20220725191039907](https://file.liangxiegame.com/3f332d24-b7cc-403e-a8d0-e628bf186f9e.png)

接着，我们打开 UIHomePanel.cs 脚本，试着写一些代码:

```csharp
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIBasicPanelData : UIPanelData
	{
	}
	public partial class UIBasicPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIBasicPanelData ?? new UIBasicPanelData();
			
			BtnStart.onClick.AddListener(() =>
			{
				Debug.Log("开始游戏");
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}

```

代码很简单，主要是在 OnInit 的时候注册了 BtnStart 按钮。

接着我们运行场景，接着点击 BtnStart 按钮，得到结果如下:
![image.png](http://file.liangxiegame.com/70f2fbb8-1267-407a-960f-bb019f114a83.png)

这样控件自动绑定功能就介绍完了。



自动绑定的功能与 View Controller + Bind 是使用的是同一套机制。



## 打开、关闭界面

我们运行 UIBasicPanel 是通过 UIPanelTester 实现的。

UIPanelTester 是一个 UI 界面的测试器，它只能在编辑器环境下运行。

真正打开一个 UI 界面，是通过 UIKit.OpenPanel 这个 API 完成的。

只需要写如下代码即可:

```csharp
UIKit.OpenPanel<UIBasicPanel>();
```

代码非常简单。

而我们要关闭掉一个 UI 界面也比较容易，代码如下:

```csharp
UIKit.ClosePanel<UIBasicPanel>();
```

如果是在一个界面内部关掉自己的话，代码如下:

```csharp
this.CloseSelf(); // this 继承自 UIPanel 
```

OK，到此我们接触了 3 个 API：

* UIKit.OpenPanel\<T\>();
* UIKit.ClosePanel\<T\>();
* UIPanel.CloseSelf();

后边的两个没什么好讲的，很简单，但是第一个 API 比较重要，因为它有一些参数我们可以填。

## UIKit.OpenPanel

UIKit.OpenPanel 的参数定义及重载如下：

```csharp
public static T OpenPanel<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null,
            string assetBundleName = null,
            string prefabName = null) where T : UIPanel
{
	...
}

public static T OpenPanel<T>(IUIData uiData, PanelOpenType panelOpenType = PanelOpenType.Single,
            string assetBundleName = null,
            string prefabName = null) where T : UIPanel
{
	...
}

public static UIPanel OpenPanel(string panelName, UILevel level = UILevel.Common, string assetBundleName = null)
{
	...
}
```



所有参数如下：

* canvasLevel：界面在哪个层级打开
  * 默认值：Common
* uiData：打开时可以给界面传的初始数据
  * 默认值：null
* assetBundleName：界面资源所在的 assetBundle 名
  * 默认值：null
* prefabName：如果界面名字和 prefab 名字不同，则以这个参数为准去加载界面资源
  * 默认值：null

都有默认值，所以这四个参数都可以不用传。

不过这四个 API 在某种情况下非常实用。

下边举一些例子。

```csharp
// 在 Forward 层级打开
UIKit.OpenPanel<UIBasicPanel>(UILevel.Forward);

// 传递初始数据给 UIHomePanel
UIKit.OpenPanel<UIBasicPanel>(new UIHomePanelData()
{
    Coin = 10
});
            
// 从 UIHomePanelTest.prefab 加载界面 
UIKit.OpenPanel<UIBasicPanel>(prefabName: "UIBasicPanel");
```

都比较容易理解。

有的童鞋可能会问，我们给 UIHomePanel 传递的  UIHomePanelData，在哪里使用呢？

答案是在，OnInit 和 OnOpen 中，如下所示:

```csharp
namespace QFramework.Example
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    
    
    public class UIBasicPanelData : QFramework.UIPanelData
    {
        public int Coin;
    }
    
    public partial class UIBasicPanel : QFramework.UIPanel
    {
        protected override void OnInit(QFramework.IUIData uiData)
        {
            mData = uiData as UIBasicPanelData ?? new UIBasicPanelData();
            // please add init code here
            
            // 外边传进来的，第一次初始化的时候使用
            Debug.Log(mData.Coin);
        }
        
        protected override void OnOpen(QFramework.IUIData uiData)
        {
            // 每次 OpenPanel 的时候使用
            Debug.Log((uiData as UIBasicPanelData).Coin);
        }
        
        protected override void OnShow()
        {

        }
        
        protected override void OnHide()
        {
        }
        
        protected override void OnClose()
        {
        }
    }
}

```

为什么要这样做呢？

笔者认为，界面有两种显示数据的用法，一种是有的界面是需要从外边填充的，比如警告、弹框、或者道具信息页面等。另一种界面是需要自己获取数据并展示的，比如游戏中的主角金币、等级、经验值等。



如果界面的数据都从外边填充，那么这个界面会拥有更好的可复用性。



当然需要一个可复用性的界面还是需要一个普通界面就看大家的需求了，并不是说有可复用性的界面就是好的。

## 异步加载界面

```csharp
StartCoroutine(UIKit.OpenPanelAsync<UIHomePanel>());
// 或者
UIKit.OpenPanelAsync<UIHomePanel>().ToAction().Start(this);
```

在 WebGL 平台上, AssetBundle 加载资源只支持异步加载，所以为此提供了 UIKit 的异步加载支持。





## UIPanel 生命周期

我们先看下  UIBasicPanel 的代码，如下:

```csharp
namespace QFramework.Example
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    
    
    public class UIBasicPanelData : QFramework.UIPanelData
    {
    }
    
    public partial class UIBasicPanel : QFramework.UIPanel
    {        
        protected override void OnInit(QFramework.IUIData uiData)
        {
            mData = uiData as UIHomePanelData ?? new UIHomePanelData();
            // please add init code here
            
            
        }
        
        protected override void OnOpen(QFramework.IUIData uiData)
        {
        }
        
        protected override void OnShow()
        {

        }
        
        protected override void OnHide()
        {
        }
        
        protected override void OnClose()
        {
        }
    }
}
```

默认的生命周期函数如下:

* OnInit
* OnOpen
* OnShow
* OnHide
* OnClose

OnInit 则是在 UIPanel 所在的 prefab 初始化的时候进行调用的，在调用 UIKit.OpenPanel 时，只要在 UIKit 中没有对应的缓存界面时，就会调用一次 OnInit 这个周期。

OnOpen 就是每次 UIKit.OpenPanel 调用时，就会调用。

OnShow  实际上调用时机与 UIKit.OpenPanel 是一样的，只不过 OnShow 是最初版本遗留下拉的 API，所以就保留了。当然还有 UIMgr.ShowPanel 调用时，OnShow 会被调用

OnHide 则是在 UIKit.HidePanel 调用时，OnHide 会被调用。

最后 OnClose 就是在 UIKit.ClosePanel 调用时，就会触发，实际上 OnClose 相当于 OnDestory 这个周期。

大概就这些，其中 UIKit.OpenPanel 会触发资源的加载和初始化操作，而 UIKit.ClosePanel 则会触发卸载和销毁操作，只要记得这两点就好。

笔者基本上就只会用到 OnInit 和 OnClose 这些周期，偶尔会用一用 OnOpen。



OK，此篇的内容就这些。






## UIKit 剩下的常用 API

### UIKit.Root.SetResolution

参数定义如下：

![image.png](http://file.liangxiegame.com/bac63766-0f9a-4d9c-92fd-cb6b90324262.png)

对应 UIRoot 上的 Canvas Scaler 如下:

![image.png](http://file.liangxiegame.com/bc2c2122-c559-48bf-8b2f-ea4609826493.png)

大部分项目，用这个 API 做屏幕适配足够了。

### UIKit.Root.Camera

获取 UIRoot 的摄像机。

```csharp
var uiCamera = UIKit.Root.Camera;
```

### UIKit.Stack.Push、UIPanel.Back（Pop）

有的时候，UI 需要实现一个 UI 界面的堆栈，以便于支持返回上一页这样的操作。

这个时候就可以用 Push 和 UIPanel.Back 实现。

示例代码:

```csharp
UIKit.Stack.Push(this); // this 是 Panel
// UIHomePanel 需要确保是打开的状态，如果不打开会报错。
UIKit.Stack.Push<UIHomePanel>();
            
this.Back(); // 弹出 UIHomePanel
this.Back(); // 弹出 this
```

非常简单。



## UIPanel 自动生成工具

在此篇的最开始，笔者手动创建了一套围绕 UIBasicPanel 的测试、开发场景，其过程比较繁琐。

为了解决这个问题，笔者写了一个简单的 UIPanel 自动生成工具。

接下来看下它的基本使用流程。

### 基本使用

首先，快捷键 ctrl + e 打开 PackageKit 面板，如下:

![image-20220725213353833](https://file.liangxiegame.com/12dcb73a-c255-45f4-869e-a00700d3b3c1.png)

在上图中界面名字的输入框中输入 Game/UIGamePanel，然后点击创建 UI Panel，如下所示:

![image-20220725213736831](https://file.liangxiegame.com/19dfe911-4cdd-4432-a99c-00423b38d781.png)



输入之后可以看到即将生成文件的预览。

在这个面板中，我们还可以设置 分辨率与适配对齐，还有模块的目录，如果不想在更目录创建按照规范生成文件，也可以在其他子目录中创建。

我们点击 "创建 UI Panel" 这个按钮。



点击之后结果如下:



![image-20220725214053092](https://file.liangxiegame.com/daf92af1-1e66-4b86-bf4b-e331c272570b.png)



相关的 prefab，场景、脚本都生成好了，就连 AssetBundle 也都标记好了，如下:



![image-20220725214155564](https://file.liangxiegame.com/a0a6c3e3-c4b6-4602-8b92-a47506714a98.png)

这就是这个工具的一个用处，非常方便，解决了笔者大量的开发工作量。


在上一篇，我们了解了界面的打开和关闭相关的 API。

在这一篇，我们了解一下 UI Kit 中的 子界面/子控件—UI Element

## UI Element 简介

在前篇，我们了解到，一个 UIPanel 是可以自动绑定几个 子控件的（Bind）。但是当一个界面结构比较复杂的时候，不可能一个 UIPanel 管理数十个 Bind，这时候就需要对 Bind 进行一些打组操作。我们的 UIElement 就可以登场了。

## UIElement 基本使用

使用方式非常简单，就是将 Bind 中的 标记类型 改成 Element即可,如下所示。

![image.png](https://file.liangxiegame.com/47b78081-62cd-41e2-b96b-47383dc80e04.png)

![image.png](https://file.liangxiegame.com/b9533003-b30b-406e-b14c-4f8f777b1e95.png)

并且要给 生成类名 填写一个名字，这个名字决定生成的类的名字。这里填写了 UIAboutSubPanel。


之后进行 Apply 操作。

![image-20220728141929443](https://file.liangxiegame.com/46876a38-e980-49a8-bbc8-59e74f968f3d.png)



注意这里 Apply 的是 UIBasicPanel。

接着生成代码， 如下:

![image-20220728142010223](https://file.liangxiegame.com/800b53e4-0d6a-43f4-9aa1-ce3815d5fc87.png)


等待编译后，如下所示：



![image-20220728142048854](https://file.liangxiegame.com/3c05a2b9-f815-421b-b0b4-379a0477e401.png)





BtnClose 由 UIAboutSubPanel 管理了

![image-20220728142125763](https://file.liangxiegame.com/956e3e01-32a3-4582-a691-59e2d9e647de.png)

我们看下脚本目录:

![image-20220728142202239](https://file.liangxiegame.com/76847346-79ad-4003-84f7-6111152e457a.png)

目录生成了一个新的文件夹，是以父 Panel （UIBasicPanel）为名的。

打开 UIAboutSubPanel 脚本，代码如下所示:

```csharp
/****************************************************************************
 * 2022.7 LIANGXIEWIN
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public partial class SubPanel1 : UIElement
	{
		private void Awake()
		{
		}

		protected override void OnBeforeDestroy()
		{
		}
	}
}
```

再看下 UILoginView.Designer.cs 脚本，如下所示:

```csharp
/****************************************************************************
 * 2022.7 LIANGXIEWIN
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public partial class SubPanel1
	{
		[SerializeField] public UnityEngine.UI.Button BtnStart2;
		[SerializeField] public UnityEngine.UI.Button BtnStart3;

		public void Clear()
		{
			BtnStart2 = null;
			BtnStart3 = null;
		}

		public override string ComponentName
		{
			get { return "SubPanel1";}
		}
	}
}
```

结构与之前的 UIBasicPanel 非常相似。

接下来，就可以写一些与子模块相关的逻辑了，关于 UIElement 的基本使用就介绍到这里。




## 同一个类型的界面打开多个

```csharp
UIKit.OpenPanel<UIMultiPanel>(new UIMultiPanelData(), PanelOpenType.Multiple);
```





## 如何自定义界面加载方式?



继承 AbstractPanelLoaderPool 类，再实现一个 IPanelLoader 的类，参考代码如下:



```csharp
using System;
using UnityEngine;

namespace QFramework.Example
{
    public class CustomPanelLoaderExample : MonoBehaviour
    {
        public class ResourcesPanelLoaderPool : AbstractPanelLoaderPool
        {
            /// <summary>
            /// Load Panel from Resources
            /// </summary>
            public class ResourcesPanelLoader : IPanelLoader
            {
                private GameObject mPanelPrefab;

                public GameObject LoadPanelPrefab(PanelSearchKeys panelSearchKeys)
                {
                    mPanelPrefab = Resources.Load<GameObject>(panelSearchKeys.GameObjName);
                    return mPanelPrefab;
                }

                public void LoadPanelPrefabAsync(PanelSearchKeys panelSearchKeys, Action<GameObject> onPanelLoad)
                {
                    var request = Resources.LoadAsync<GameObject>(panelSearchKeys.GameObjName);

                    request.completed += operation => { onPanelLoad(request.asset as GameObject); };
                }

                public void Unload()
                {
                    mPanelPrefab = null;
                }
            }

            protected override IPanelLoader CreatePanelLoader()
            {
                return new ResourcesPanelLoader();
            }
        }

        void Start()
        {
            // 游戏启动时，设置一次
            UIKit.Config.PanelLoaderPool = new ResourcesPanelLoaderPool();
        }
    }
}
```



如果想要支持 其他方式加载界面则可以通过此方式定制。



另外，QFramework 中的 UIKit 默认使用 ResKit 的方式加载界面。



可以在 QFramework 源码中看到如下代码:



```csharp
using System;
using UnityEngine;

namespace QFramework
{
    public class UIKitWithResKitInit
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            UIKit.Config.PanelLoaderPool = new ResKitPanelLoaderPool();
        }
    }
    
    ...
}
```



如果想要使用自定义的方式加载界面，需要将以上代码注释掉。



好了，关于 UIKit 自定义加载界面就简单介绍到这里。



## UI Kit 小结

在这一章，UI Kit 的核心功能，我们都接触过了，如下：

* UIPanel/UIElement 代码生成
* UIKit 常用 API
  * UIKit.OpenPanel（Async）
  * UIKit.ClosePanel
  * UIKit.CloseSelf
  * UIKit.SetResolution
  * UIKit.Stack.Push、UIPanel.Back(Pop)
* UIPanel 生命周期
* UIPanel 测试场景生成工具

只要掌握了以上这些，基本上开发一些界面就没啥问题了。

关于 UIKit 就介绍到这里。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)













  
# 07. AudioKit 音频管理解决方案

## 基本使用

AudioKit 音频播放相关的功能如下:
* 播放背景音乐，同一时间只能播放一个音乐，播放别的音乐会直接卸载掉正在播放的音乐。
* 播放音效，同一时间可以播放多个音效，当多人说话时，也可以用来播放人声。
* 播放人声，与播放背景音乐一致，同一时间只能播放一个人声，用于播放一些旁白之类的声音非常适合。

对应的 API 调用方式如下:

```csharp
btnPlayGame.onClick.AddListener(() => { AudioKit.PlayMusic("resources://game_bg"); });

btnPlaySound.onClick.AddListener(() => { AudioKit.PlaySound("resources://game_bg"); });

btnPlayVoiceA.onClick.AddListener(() => { AudioKit.PlayVoice("resources://game_bg"); });
```


AudioKit 设置相关的功能如下:
* 背景音乐开关
* 音效开关
* 人声开关

调用示例如下：

```csharp
btnSoundOn.onClick.AddListener(() => { AudioKit.Settings.IsSoundOn.Value = true; });

btnSoundOff.onClick.AddListener(() => { AudioKit.Settings.IsSoundOn.Value = false; });

btnMusicOn.onClick.AddListener(() => { AudioKit.Settings.IsMusicOn.Value = true; });

btnMusicOff.onClick.AddListener(() => { AudioKit.Settings.IsMusicOn.Value = false; });

btnVoiceOn.onClick.AddListener(() => { AudioKit.Settings.IsVoiceOn.Value = true; });

btnVoiceOff.onClick.AddListener(() => { AudioKit.Settings.IsVoiceOn.Value = false; });
```
这是打开声音这个功能的使用方式。


调整音量大小的代码如下:

```csharp
AudioKit.Settings.MusicVolume.RegisterWithInitValue(v => musicVolumeSlider.value = v);
AudioKit.Settings.VoiceVolume.RegisterWithInitValue(v => voiceVolumeSlider.value = v);
AudioKit.Settings.SoundVolume.RegisterWithInitValue(v => soundVolumeSlider.value = v);
            
musicVolumeSlider.onValueChanged.AddListener(v => { AudioKit.Settings.MusicVolume.Value = v; });
voiceVolumeSlider.onValueChanged.AddListener(v => { AudioKit.Settings.VoiceVolume.Value = v; });
soundVolumeSlider.onValueChanged.AddListener(v => { AudioKit.Settings.SoundVolume.Value = v; });
```


## 如何自定义音频加载

与 UIKit 一样, AudioKit 也支持了自定义音频加载的方式。

参考代码如下:

```csharp
using System;
using UnityEngine;

namespace QFramework.Example
{
    public class CustomAudioLoaderExample : MonoBehaviour
    {
        /// <summary>
        /// 定义从 Resources 加载音频
        /// </summary>
        class ResourcesAudioLoaderPool : AbstractAudioLoaderPool
        {
            protected override IAudioLoader CreateLoader()
            {
                return new ResourcesAudioLoader();
            }
        }

        class ResourcesAudioLoader : IAudioLoader
        {
            private AudioClip mClip;
        
            public AudioClip Clip => mClip;

            public AudioClip LoadClip(AudioSearchKeys panelSearchKeys)
            {
                mClip = Resources.Load<AudioClip>(panelSearchKeys.AssetName);
                return mClip;
            }

            public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool,AudioClip> onLoad)
            {
                var resourceRequest = Resources.LoadAsync<AudioClip>(audioSearchKeys.AssetName);
                resourceRequest.completed += operation =>
                {
                    var clip = resourceRequest.asset as AudioClip;
                    onLoad(clip, clip);
                };
            }

            public void Unload()
            {
                Resources.UnloadAsset(mClip);
            }
        }
        
        
        void Start()
        {
            // 启动时需要调用一次
            AudioKit.Config.AudioLoaderPool = new ResourcesAudioLoaderPool();
        }
    }
}

```

由于 QFramework 中的 AudioKit 默认是通过 ResKit 加载，所以使用自定义加载方式时，请将项目中如下代码注释掉:

```csharp
    public class AudioKitWithResKitInit 
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            AudioKit.Config.AudioLoaderPool = new ResKitAudioLoaderPool();
        }
    }
```


关于 AudioKit 就介绍到这。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)



# 08. FluentAPI 链式 API

## FluentAPI 简介
FluentAPI 是 笔者积累的 Unity API 的一些链式封装。

基本使用非常简单，如下：
```csharp
// traditional style
var playerPrefab = Resources.Load<GameObject>("no prefab don't run");
var playerObj = Instantiate(playerPrefab);

playerObj.transform.SetParent(null);
playerObj.transform.localRotation = Quaternion.identity;
playerObj.transform.localPosition = Vector3.left;
playerObj.transform.localScale = Vector3.one;
playerObj.layer = 1;
playerObj.layer = LayerMask.GetMask("Default");

Debug.Log("playerPrefab instantiated");

// Extension's Style,same as above 
Resources.Load<GameObject>("playerPrefab")
    .Instantiate()
    .transform
    .Parent(null)
    .LocalRotationIdentity()
    .LocalPosition(Vector3.left)
    .LocalScaleIdentity()
    .Layer(1)
    .Layer("Default")
    .ApplySelfTo(_ => { Debug.Log("playerPrefab instantiated"); });
```

代码很简单。

FluentAPI 包含 100 多个常用 API 的链式封装，具体可以参考编辑器内文档。

![image.png](https://file.liangxiegame.com/67604baa-a9ca-4f03-8f7a-c1f88be322b7.png)

另外 链式 API 可以与 QFramework 的其他模块配合使用事半功倍，比如 ResKit 与 FluentAPI 结合，参考代码如下:

```csharp
mResLoader.LoadSync<GameObject>("mygameobj")
  .InstantiateWithParent(parent)
  .transform
  .LocalIdentity()
  .Name("MyGameObj")
  .Show();
```


链式 API 就介绍到这里。



## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 09. SingletonKit 单例模板套件

SingletonKit 是 QFramework 的第一个收集的工具，经过了 7 年的迭代，现在已经非常成熟了。

好久不见 ！之前想着让各位直接用 QFramework，但是后来想想，如果正在进行的项目直接使用QFramework，这样风险太高了，要改的代码太多，所以打算陆续独立出来一些工具和模块,允许各位一个模块一个模块的进行更换，减少更换带来的风险。

## QSingleton:

之前有几篇文章介绍过单例模板在 Unity 中的几种实现。之后又参考了其他的单例库的实现，借鉴(chao)了它们的优点,借鉴了哪里有声明原作者。

## 快速开始:

实现一个继承 MonoBehaviour 的单例类

```csharp
namespace QFramework.Example
{
	[MonoSingletonPath("[Audio]/AudioManager")]
	public class AudioManager : ManagerBase,ISingleton
	{
		public static AudioManager Instance
		{
			get { return QMonoSingletonProperty<AudioManager>.Instance; }
		}
		
		public void OnSingletonInit()
		{
			
		}

		public void Dispose()
		{
			QMonoSingletonProperty<AudioManager>.Dispose();
		}


		public void PlaySound(string soundName)
		{
			
		}

		public void StopSound(string soundName)
		{
			
		}
	}
}
```

结果如下:
![DraggedImage.png](https://upload-images.jianshu.io/upload_images/2296785-a0d55653522f9037.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
这样从头到尾都很！优！雅！


## C# 单例类

*   Singleton.cs

```csharp
public class GameDataManager : Singleton<GameDataManager>
{
    private static int mIndex = 0;

    private Class2Singleton() {}

    public override void OnSingletonInit()
    {
        mIndex++;
    }

    public void Log(string content)
    {
        Debug.Log(""GameDataManager"" + mIndex + "":"" + content);
    }
}

GameDataManager.Instance.Log(""Hello"");
// GameDataManager1:OnSingletonInit:Hello
GameDataManager.Instance.Log(""Hello"");
// GameDataManager1:OnSingletonInit:Hello
GameDataManager.Instance.Dispose();
```

只需简单继承QSingleton，并声明非public构造方法即可。如果有需要获取单例初始化的时机，则可以选择重载OnSingletonInit方法。

## 结果:
``` 
Hello World!
Hello World!
```


## Mono 单例

* MonoSingleton.cs
```csharp
public class GameManager : MonoSingleton<GameManager>
{
    public override void OnSingletonInit()
    {
        Debug.Log(name + "":"" + ""OnSingletonInit"");
    }

    private void Awake()
    {
        Debug.Log(name + "":"" + ""Awake"");
    }

    private void Start()
    {
        Debug.Log(name + "":"" + ""Start"");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
			
        Debug.Log(name + "":"" + ""OnDestroy"");
    }
}

var gameManager = GameManager.Instance;
// GameManager:OnSingletonInit
// GameManager:Awake
// GameManager:Start
// ---------------------
// GameManager:OnDestroy
```


## Mono 属性单例
代码如下:

* MonoSingletonProperty.cs
```csharp
public class GameManager : MonoBehaviour,ISingleton
{
    public static GameManager Instance
    {
        get { return MonoSingletonProperty<GameManager>.Instance; }
    }
		
    public void Dispose()
    {
    	MonoSingletonProperty<GameManager>.Dispose();
    }
		
    public void OnSingletonInit()
    {
    	Debug.Log(name + "":"" + ""OnSingletonInit"");
    }
    
    private void Awake()
    {
        Debug.Log(name + "":"" + ""Awake"");
    }
    
    private void Start()
    {
        Debug.Log(name + "":"" + ""Start"");
    }
    
    protected void OnDestroy()
    {
        Debug.Log(name + "":"" + ""OnDestroy"");
    }
}
var gameManager = GameManager.Instance;
// GameManager:OnSingletonInit
// GameManager:Awake
// GameManager:Start
// ---------------------
// GameManager:OnDestroy
```

## C# 属性单例

代码如下：

* SingletonProperty.cs
```csharp
public class GameDataManager : ISingleton
{
    public static GameDataManager Instance
    {
        get { return SingletonProperty<GameDataManager>.Instance; }
    }

    private GameDataManager() {}
		
    private static int mIndex = 0;

    public void OnSingletonInit()
    {
        mIndex++;
    }

    public void Dispose()
    {
        SingletonProperty<GameDataManager>.Dispose();
    }
		
    public void Log(string content)
    {
        Debug.Log(""GameDataManager"" + mIndex + "":"" + content);
    }
}
 
GameDataManager.Instance.Log(""Hello"");
// GameDataManager1:OnSingletonInit:Hello
 
GameDataManager.Instance.Log(""Hello"");
// GameDataManager1:OnSingletonInit:Hello
 
GameDataManager.Instance.Dispose();
```



## MonoSingletPath 重命名


代码如下：
MonoSingletonPath.cs：

```csharp
namespace QFramework.Example
{
	using UnityEngine;

	[MonoSingletonPath("[Example]/MonoSingeltonPath")]
	class ClassUseMonoSingletonPath : QMonoSingleton<ClassUseMonoSingletonPath>
	{
		
	}
	
	public class MonoSingletonPath : MonoBehaviour
	{
		private void Start()
		{
			var intance = ClassUseMonoSingletonPath.Instance;
		}
	}
}
```

## 结果:
![DraggedImage.png](https://upload-images.jianshu.io/upload_images/2296785-8bf380c8327ffbce.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)





## PersistentMonoSingleton

当场景里包含两个 PersistentMonoSingleton，保留先创建的

```csharp
public class GameManager : PersistentMonoSingleton<GameManager>
{
 
}
 
IEnumerator Start()
{
    var gameManager = GameManager.Instance;
 
    var newGameManager = new GameObject().AddComponent<GameManager>();
 
    yield return new WaitForEndOfFrame();
 
    Debug.Log(FindObjectOfTypes<GameManager>().Length);
    // 1
    Debug.Log(gameManager == null);
    // false
    Debug.Log(newGameManager == null);
    // true
}
```

## ReplaceableMonoSingleton

当场景里包含两个 ReplaceableMonoSingleton，保留最后创建的

```csharp
public class GameManager : ReplaceableMonoSingleton<GameManager>
{
 
}

IEnumerator Start()
{
    var gameManager = GameManager.Instance;
 
    var newGameManager = new GameObject().AddComponent<GameManager>();
 
    yield return new WaitForEndOfFrame();
 
    Debug.Log(FindObjectOfTypes<GameManager>().Length);
    // 1
    Debug.Log(gameManager == null);
    // true
    Debug.Log(newGameManager == null);
    // false
}
```


关于 SingletonKit 的介绍就到这。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.game




# 10. FSMKit 状态机

QFramework 内置了一个简易的状态机，基本使用如下:

## 链式

```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class IStateBasicUsageExample : MonoBehaviour
    {
        public enum States
        {
            A,
            B
        }

        public FSM<States> FSM = new FSM<States>();

        void Start()
        {
            FSM.State(States.A)
                .OnCondition(()=>FSM.CurrentStateId == States.B)
                .OnEnter(() =>
                {
                    Debug.Log("Enter A");
                })
                .OnUpdate(() =>
                {
                    
                })
                .OnFixedUpdate(() =>
                {
                    
                })
                .OnGUI(() =>
                {
                    GUILayout.Label("State A");
                    if (GUILayout.Button("To State B"))
                    {
                        FSM.ChangeState(States.B);
                    }
                })
                .OnExit(() =>
                {
                    Debug.Log("Enter B");

                });

            FSM.State(States.B)
                .OnCondition(() => FSM.CurrentStateId == States.A)
                .OnGUI(() =>
                {
                    GUILayout.Label("State B");
                    if (GUILayout.Button("To State A"))
                    {
                        FSM.ChangeState(States.A);
                    }
                });
            
            FSM.StartState(States.A);
        }

        private void Update()
        {
            FSM.Update();
        }

        private void FixedUpdate()
        {
            FSM.FixedUpdate();
        }

        private void OnGUI()
        {
            FSM.OnGUI();
        }

        private void OnDestroy()
        {
            FSM.Clear();
        }
    }
}
```

运行之后，结果如下:

![1](https://file.liangxiegame.com/c263fec3-02eb-4af6-bb84-a3310440cfa9.gif)

没啥问题。



## 类模式

链式适合在快速开发阶段，或者在状态非常少的阶段使用。



而如果状态较多，或者相应代码量较多的阶段，可以使用类模式，代码如下:



```csharp
using UnityEngine;

namespace QFramework.Example
{
    public class IStateClassExample : MonoBehaviour
    {

        public enum States
        {
            A,
            B,
            C
        }

        public FSM<States> FSM = new FSM<States>();

        public class StateA : AbstractState<States,IStateClassExample>
        {
            public StateA(FSM<States> fsm, IStateClassExample target) : base(fsm, target)
            {
            }

            protected override bool OnCondition()
            {
                return mFSM.CurrentStateId == States.B;
            }

            public override void OnGUI()
            {
                GUILayout.Label("State A");

                if (GUILayout.Button("To State B"))
                {
                    mFSM.ChangeState(States.B);
                }
            }
        }
        
        
        public class StateB: AbstractState<States,IStateClassExample>
        {
            public StateB(FSM<States> fsm, IStateClassExample target) : base(fsm, target)
            {
            }

            protected override bool OnCondition()
            {
                return mFSM.CurrentStateId == States.A;
            }

            public override void OnGUI()
            {
                GUILayout.Label("State B");

                if (GUILayout.Button("To State A"))
                {
                    mFSM.ChangeState(States.A);
                }
            }
        }

        private void Start()
        {
            FSM.AddState(States.A, new StateA(FSM, this));
            FSM.AddState(States.B, new StateB(FSM, this));

            // 支持和链式模式混用
            // FSM.State(States.C)
            //     .OnEnter(() =>
            //     {
            //
            //     });
            
            FSM.StartState(States.A);
        }

        private void OnGUI()
        {
            FSM.OnGUI();
        }

        private void OnDestroy()
        {
            FSM.Clear();
        }
    }
}
```



运行之后结果如下。



![1](https://file.liangxiegame.com/c263fec3-02eb-4af6-bb84-a3310440cfa9.gif)

关于状态机的介绍就到这里。


## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)

# 11. PoolKit 对象池套件


## SimpleObjectPool 简易对象池

```csharp
class Fish
{
             
}

var pool = new SimpleObjectPool<Fish>(() => new Fish(),initCount:50);
 
Debug.Log(pool.CurCount);
// 50 
var fish = pool.Allocate();
 
Debug.Log(pool.CurCount);
// 49
pool.Recycle(fish);

Debug.Log(pool.CurCount);
// 50


// ---- GameObject ----
var gameObjPool = new SimpleObjectPool<GameObject>(() =>
{
    var gameObj = new GameObject(""AGameObject"");
    // init gameObj code 

    // gameObjPrefab = Resources.Load<GameObject>(""somePath/someGameObj"");
                
    return gameObj;
}, (gameObj) =>
{
    // reset code here
});
```

## SafeObjectPool 安全对象池

```csharp
class Bullet :IPoolable,IPoolType
{
    public void OnRecycled()
    {
        Debug.Log(""回收了"");
    }
 
    public  bool IsRecycled { get; set; }
 
    public static Bullet Allocate()
    {
        return SafeObjectPool<Bullet>.Instance.Allocate();
    }
             
    public void Recycle2Cache()
    {
        SafeObjectPool<Bullet>.Instance.Recycle(this);
    }
}
 
SafeObjectPool<Bullet>.Instance.Init(50,25);
             
var bullet = Bullet.Allocate();
 
Debug.Log(SafeObjectPool<Bullet>.Instance.CurCount);
             
bullet.Recycle2Cache();
 
Debug.Log(SafeObjectPool<Bullet>.Instance.CurCount);
 
// can config object factory
// 可以配置对象工厂
SafeObjectPool<Bullet>.Instance.SetFactoryMethod(() =>
{
    // bullet can be mono behaviour
    return new Bullet();
});
             
SafeObjectPool<Bullet>.Instance.SetObjectFactory(new DefaultObjectFactory<Bullet>());
 
// can set
// 可以设置
// NonPublicObjectFactory: 可以通过调用私有构造来创建对象,can call private constructor to create object
// CustomObjectFactory: 自定义创建对象的方式,can create object by Func<T>
// DefaultObjectFactory: 通过 new 创建对象, can create object by new 
```

## 基本的数据结构封装 List、Dictionary

```csharp
var names = ListPool<string>.Get()
names.Add(""Hello"");

names.Release2Pool();
// or ListPool<string>.Release(names);
```


```csharp
var infos = DictionaryPool<string,string>.Get()
infos.Add(""name"",""liangxie"");

infos.Release2Pool();
// or DictionaryPool<string,string>.Release(names);
```

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
# 12. TableKit 表数据结构

在设计 UIKit、ResKit 等系统时，如果只使用默认的 List 和 Dictionary 来管理数据和对象需要做很多的封装。

因为本身 List 和 Dictionary 支持的查询方式比较单一，如果想做一些比较复杂的查询，比如联合查询，那么 List 和 Dictionary 的性能会比较差。

所以为此，笔者简单封装了一个 Table 数据结构。

使用示例如下:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public class TableKitExample : MonoBehaviour
    {
        public class Student
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public int Level { get; set; }
        }
        public class School : Table<Student>
        {
            public TableIndex<int, Student> AgeIndex = new TableIndex<int, Student>((student) => student.Age);
            public TableIndex<int, Student> LevelIndex = new TableIndex<int, Student>((student) => student.Level);
            
            protected override void OnAdd(Student item)
            {
                AgeIndex.Add(item);
                LevelIndex.Add(item);
            }

            protected override void OnRemove(Student item)
            {
                AgeIndex.Remove(item);
                LevelIndex.Remove(item);
            }

            protected override void OnClear()
            {
                AgeIndex.Clear();
                LevelIndex.Clear();
            }

            public override IEnumerator<Student> GetEnumerator()
            {
                return AgeIndex.Dictionary.Values.SelectMany(s=>s).GetEnumerator();
            }

            protected override void OnDispose()
            {
                AgeIndex.Dispose();
                LevelIndex.Dispose();
            }
        }


        private void Start()
        {
            var school = new School();
            school.Add(new Student(){Age = 1,Level = 2,Name = "liangxie"});
            school.Add(new Student(){Age = 2,Level = 2,Name = "ava"});
            school.Add(new Student(){Age = 3,Level = 2,Name = "abc"});
            school.Add(new Student(){Age = 3,Level = 3,Name = "efg"});
            
            foreach (var student in school.LevelIndex.Get(2).Where(s=>s.Age < 3))
            {
                Debug.Log(student.Age + ":" + student.Level + ":" + student.Name);
            }
        }
    }
}
// 1:2:liangxie
// 2:2:ava
```


TableKit 兼顾查询功能支持和性能，在功能和性能之间取得了一个平衡。

ResKit、UIKit 的数据管理全部由 TableKit 支持。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)


# 13. 其他事件工具

QFramework 除了支持了  TypeEventSystem、EasyEvent 还支持了 EnumEventSystem、StringEventSystem。


## EnumEventSystem

EnumEventSystem 前身是 老版本 QFramework 的 QEventSystem

``` csharp
using UnityEngine;

namespace QFramework
{
	public class EnumEventExample : MonoBehaviour
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
		
		void Start()
		{
			EnumEventSystem.Global.Register(TestEvent.TestOne, OnEvent);
		}

		void OnEvent(int key, params object[] obj)
		{
			switch (key)
			{
				case (int) TestEvent.TestOne:
					Debug.Log(obj[0]);
					break;
			}
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				EnumEventSystem.Global.Send(TestEvent.TestOne, "Hello World!");
			}
		}

		private void OnDestroy()
		{
			EnumEventSystem.Global.UnRegister(TestEvent.TestOne, OnEvent);
		}
	}
}
```


## StringEventSystem

StringEventSystem 的前身是，老版本的 MsgDispatcher

``` csharp
using UnityEngine;

namespace QFramework
{
	public class EnumEventExample : MonoBehaviour
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
		
		void Start()
		{
			EnumEventSystem.Global.Register(TestEvent.TestOne, OnEvent);
		}

		void OnEvent(int key, params object[] obj)
		{
			switch (key)
			{
				case (int) TestEvent.TestOne:
					Debug.Log(obj[0]);
					break;
			}
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				EnumEventSystem.Global.Send(TestEvent.TestOne, "Hello World!");
			}
		}

		private void OnDestroy()
		{
			EnumEventSystem.Global.UnRegister(TestEvent.TestOne, OnEvent);
		}
	}
}
// 输出结果
// 点击鼠标左键
// Hello World
```


## StringEventSystem

``` csharp
using UnityEngine;

namespace QFramework.Example
{
    public class StringEventSystemExample : MonoBehaviour
    {
        void Start()
        {
            StringEventSystem.Global.Register("TEST_ONE", () =>
            {
                Debug.Log("TEST_ONE");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            // 事件 + 参数
            StringEventSystem.Global.Register<int>("TEST_TWO", (count) =>
            {
                Debug.Log("TEST_TWO:" + count);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                StringEventSystem.Global.Send("TEST_ONE");
                StringEventSystem.Global.Send("TEST_TWO",10);
                
            }
        }
    }
}

// 输出结果
// 点击鼠标左键
// TEST_ONE
// TEST_TWO:10

```




## 对比

* TypeEventSystem：
  * 事件体定义简洁
  * 比较适合用于设计框架
  * 支持 struct 获得较好内存性能
  * 使用反射，CPU 性能相对比较差

* EasyEvent
  * 方便、易用、开发效率高
  * CPU 性能、内存性能较好，接近委托
  * 功能有限
  * 比较适合设计通用解决工具，比如通用背包、全局生命周期触发等
  * StringEventSystem、TypeEventSystem 的底层由 EasyEvent 实现

* EnumEventSystem
  * 使用枚举作为事件 id，比较适合和服务端的 protobuf 或带有消息 id 的长链接通信
  * 性能较好
  * 枚举用于定义消息体有维护成本

* StringEventSystem
  * 使用字符串作为事件 id，比较适合和其他脚本层通信，比如 Lua、ILRuntime、PlayMaker 等。
  * 性能一般


目前官方推荐使用 TypeEventSystem 和 EasyEvent 这两个工具。

如果要和网络通信则选择用 EnumEventSystem。

如果要和其他脚本层通信选择用 StringEventSystem。

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
    
    
    
        
# 14. 更多内容


## 案例《五子棋》

![2f4dacbd-e59b-43af-b7be-44220fac664e.png](https://file.liangxiegame.com/a76bc24a-1828-46f2-94c5-8bd24884f932.png)


源码地址:
* github https://github.com/liangxiegame/QFramework
* gitee https://gitee.com/liangxiegame/QFramework

![image.png](https://file.liangxiegame.com/3abceb70-2d17-4457-aff1-ef8a6ef4bd66.png)

## 案例《扫雷》

作者：Joker

![172348_4d54744e_5161625.webp](https://file.liangxiegame.com/c6116b7e-a08b-4c13-ae64-7053be3c503c.png)

源码地址:
* github https://github.com/liangxiegame/QFramework
* gitee https://gitee.com/liangxiegame/QFramework

![image.png](https://file.liangxiegame.com/6482d4eb-5af9-4932-a2f8-2164cb22e931.png)

## 更多内容

*   转载请注明地址：[liangxiegame.com](https://liangxiegame.com) （首发） 微信公众号：凉鞋的笔记
*   QFramework 主页：[qframework.cn](https://qframework.cn)
*   QFramework 交流群: 623597263
*   QFramework Github 地址: [https://github.com/liangxiegame/qframework](https://github.com/liangxiegame/qframework)
*   QFramework Gitee 地址：[https://gitee.com/liangxiegame/QFramework](https://gitee.com/liangxiegame/QFramework)
*   GamePix 独立游戏学院 & Unity 进阶小班地址：[https://www.gamepixedu.com/](https://www.gamepixedu.com/)
