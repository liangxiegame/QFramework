# Unity 游戏框架搭建 (十) QFramework v0.0.2小结

从框架搭建系列的第一篇文章开始到现在有四个多月时间了,这段时间对自己来说有很多的收获,好多小伙伴和前辈不管是在评论区还是私下里给出的建议非常有参考性,在此先谢过各位。

说到是一篇小节,先列出框架的概要图。
![](https://ws2.sinaimg.cn/large/006tKfTcgy1frothawkluj30m20r975s.jpg)

目前,图中除了UI模块和未支持的部分,都有相应的文章来介绍。

设计模式:

* [Unity 游戏框架搭建 (二) 单例的模板](http://liangxiegame.com/unity-you-xi-kuang-jia-da-jian-er-dan-li-de-mo-ban/)
* [Unity 游戏框架搭建 (三) MonoBehaviour单例的模板](http://liangxiegame.com/unity-you-xi-kuang-jia-da-jian-san-monobehaviourdan-li-de-mo-ban/)

事件:

* [Unity 游戏框架搭建 (五) 简易消息机制](http://liangxiegame.com/unity-you-xi-kuang-jia-da-jian-wu-jian-yi-xiao-xi-ji-zhi/)

AI:

* [Unity 游戏框架搭建 (四) 简易有限状态机](http://liangxiegame.com/unity-you-xi-kuang-jia-da-jian-er-dan-li-de-mo-ban-2/)

模块化:

* [Unity 游戏框架搭建 (七) 减少加班利器-QApp类](http://liangxiegame.com/untitled-2/)

调试:

* [Unity 游戏框架搭建 (八) 减少加班利器-QLog](http://liangxiegame.com/unity-you-xi-kuang-jia-da-jian-ba-jian-shao-jia-ban-li-qi-qlog/)

* [Unity 游戏框架搭建 (九) 减少加班利器-QConsole](http://liangxiegame.com/unity-you-xi-kuang-jia-da-jian-jiu-jian-shao-jia-ban-li-qi-qconsole/)

UI模块还没有写对应的文章来介绍。因为没有找到一种自己认为满意的方式,目前暂时使用UGUIManager或者NGUIManager来配合UILayer应付UI的开发。

#### QFramework的基因:

我个人意愿是想把Framework打造成,让使用的人觉得所有框架中出现的概念要非常清晰,没有任何模糊的概念,出现的概念已经达成共识的概念,没有任何生僻概念,使用门槛尽很低:)。

#### QFramework和框架搭建系列的接下来要做的事情:

1. Unity中比较强大(但是坑多)的概念就属AssetBundle了吧,还没有认真研究过,打算近期花些时间指定一个比较满意的方案引入到框架里。

2. 框架中很多工具的实现都是基于字典+字符串的形式实现的,但是看了好多其他框架用的都是字典+enum转unsigned int方式实现,这部分的话要考虑下大换血。

4. UI模块:很少有哪个项目不使用UI的,所以UI这部分应该多下些功夫,以目前的UGUIManager和NGUIManager结合UILayer肯定是不够的,还需要一些辅助工具来加快UI的开发。

4. 一键打包,这部分每个项目的差异化太大了,不过可以先为QFramework制定一个标准。

5. 网络、数据持久化。数据持久化部分可以考虑为SQLite封装一套易用的API,网络的话需要花些时间研究下,因为本人只开发过弱联网游戏,所以这部分还不太了解。所以这部分最后花时间研究。

以上的这些内容需要十篇左右的文章来介绍吧。对了,目前QFramework框架算是稳定一些了,欢迎大家入坑。

#### 欢迎讨论!

#### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework&游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](http://liangxiegame.com/content/images/2017/06/qrcode_for_gh_32f0f3669ac8_430.jpg)

### 如果有帮助到您:

如果觉得本篇教程对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

- 给 QFramework 一个 Star
  - 地址: https://github.com/liangxiegame/QFramework
- 给 Asset Store 上的 QFramework 并给个五星(需要先下载)
  - 地址: http://u3d.as/SJ9
- 购买 gitchat 话题《Unity 游戏框架搭建：我所理解的框架》
  - 价格: 6 元，会员免费
  - 地址:  http://gitbook.cn/gitchat/activity/5abc3f43bad4f418fb78ab77
- 购买 gitchat 话题《Unity 游戏框架搭建：资源管理神器 ResKit》
  - 价格: 6 元，会员免费
  - 地址:  http://gitbook.cn/gitchat/activity/5b29df073104f252297a779c
- 购买同名的蛮牛视频课程录播课程:
  - 价格 ~~19.2 元~~ 29.8 元
  - 地址: http://edu.manew.com/course/431 
- 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design( 29.9 元，内容会在 2018 年 10 月份完结)