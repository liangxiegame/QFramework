# Unity 游戏框架搭建 (十六)  v0.0.1 架构调整

## 背景:
前段时间用Xamarin.OSX开发一些工具,遇到了两个问题。

* QFramework的大部分的类耦合了Unity的API，这样导致不能在其他CLR平台使用QFramework。
* QFramework定义了太多了命名空间，如果使用vs for mac或者MonoDevelop开发项目很不方便，每次都要先using命名空间IDE才会提供代码提示，当然用Rider就没有这个问题。


基于以上几点进行了一次架构调整。

## 目前架构：
为了提升开发效率，命名空间全部统一为QFramework,而不是像以前一样使用QFramework.Core.Lib,QFramework.Core.Utils这样的命名空间。
考虑了多方面因素，目前QFramwork文件结构调整为如下:	

* Core 
  * CSharp
  * Unity
* Framework

主要分Core层和Framework层，Core层专注于提升编程效率，比如Rx，Chaining，Utils等都在这一层实现。Framework层专注于对游戏项目提供基本的管理模块，比如Audio，UI，ReplaySystem，ResSystem等。Core层又分为CSharp模块和Unity模块，CSharp层可以在其他CLR平台上使用，而Unity模块是为了对Audio，UI，等Framework层的模块提供支持。

## 还有...

其他的从QFramework中独立出去的项目比如QSingleton和QChain等不再进行维护了，目前只专注于QFramework的开发。

## 相关链接:
[我的框架地址][1]:https://github.com/liangxiegame/QFramework

[教程源码][2]:https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework &游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记][3] http://liangxiegame.com/

微信公众号:liangxiegame

![][image-1]

## 如果有帮助到您:
如果觉得本篇教程对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

* 给 [QFramework][4] 一个 Star
	* 地址: https://github.com/liangxiegame/QFramework
* 给 Asset Store 上的 QFramework 并给个五星(需要先下载)
	* 地址: http://u3d.as/SJ9
* 购买 gitchat 话题:[《命名的力量：变量》][5]
	* 价格: 12 元
	* 地址: [https://gitbook.cn/gitchat/activity/5b65904096290075f5829388 ][6]
* 购买同名的蛮牛视频课程录播课程: 
	* 价格 49.2 元
	* 地址: [http://edu.manew.com/course/431][7]
* 购买同名电子书:[https://www.kancloud.cn/liangxiegame/unity_framework_design][8]
	* 价格  49.2 元，内容会在 2018 年 10 月份完结

[1]:	https://github.com/liangxiegame/QFramework
[2]:	https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/%0A
[3]:	http://liangxiegame.com/
[4]:	https://github.com/liangxiegame/QFramework
[5]:	https://gitbook.cn/gitchat/activity/5b65904096290075f5829388
[6]:	https://gitbook.cn/gitchat/activity/5b65904096290075f5829388 "https://gitbook.cn/gitchat/activity/5b65904096290075f5829388"
[7]:	http://edu.manew.com/course/431
[8]:	https://www.kancloud.cn/liangxiegame/unity_framework_design

[image-1]:	https://ws4.sinaimg.cn/large/006tKfTcgy1fryc5skygwj30by0byt9i.jpg