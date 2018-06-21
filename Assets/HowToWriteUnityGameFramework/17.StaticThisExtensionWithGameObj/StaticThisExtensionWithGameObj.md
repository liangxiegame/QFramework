# Unity 游戏框架搭建 (十七) 静态扩展GameObject实现链式编程

本篇本来是作为原来优雅的QChain的第一篇的内容,但是QChain流产了，所以收录到了游戏框架搭建系列。本篇介绍如何实现GameObject的链式编程。

链式编程的实现技术之一是C#的静态扩展。静态扩展可以做到无需继承GameObject就可以为GameObject的对象添加成员方法。其实这么说不太严谨，但是看起来就是这样:)

#### C# 静态扩展快速入门

首先我们要实现给GameObject添加一个DestroySelf方法。使用方式如下:

```  csharp
		gameObject.DestroySelf();
```

贴上具体实现代码 :

``` csharp
    using System;
    using UnityEngine;

    /// <summary>
    /// GameObject's Util/This Extension
    /// </summary>
    public static class GameObjectExtension
    {
		  ...  		
        public static void DestroySelf(this GameObject selfObj)
        {
            GameObject.Destroy(selfObj);
        }
		  ...
    }
```

代码非常简单。

以上代码要注意的是:
1. 静态扩展方法必须在静态类中实现。
2. 第一个参数前要加this关键字。

当然也可以用这种方式使用:
``` csharp
	GameObjectExtension.DestroySelf(gameObject);
```

这样写的意义不大，不如直接用Object/GameObject.Destroy(gameObject);不过也有可以使用的情形，就是当导出给脚本层使用的时候。这里不多说。
初步入门就介绍到这里。下面实现链式编程。

#### GameObject实现链式编程

链式编程实现方式多种多样。但是对于GameObject来说有一种最简单并且最合适的方法，就是静态扩展 + 返回this的方式。

为什么呢？链式编程如果可以使用继承实现的话有很多种玩法，只不过GameObject是sealed class,不能被继承。所以只能通过静态扩展 + 返回this的方式。~~这也是为什么会把这篇文章作为第一篇的原因。~~

先看下如何使用。
``` csharp
			gameObject.Show()   // active = true
				.Layer(0) // layer = 0 
				.Name("Example");	// name = "Example"
```

接下来贴出实现:
``` csharp
    using System;
    using UnityEngine;

    /// <summary>
    /// GameObject's Util/This Extension
    /// </summary>
    public static class GameObjectExtension
    {
        public static GameObject Show(this GameObject selfObj)
        {
            selfObj.SetActive(true);
            return selfObj;
        }

        public static GameObject Hide(this GameObject selfObj)
        {
            selfObj.SetActive(false);
            return selfObj;
        }

        public static GameObject Name(this GameObject selfObj,string name)
        {
            selfObj.name = name;
            return selfObj;
        }

        public static GameObject Layer(this GameObject selfObj, int layer)
        {
            selfObj.layer = layer;
            return selfObj;
        }

        public static void DestroySelf(this GameObject selfObj)
        {
            GameObject.Destroy(selfObj);
        }
		  ...
}
```

可以看到新增的几个静态方法与DestroySelf不同的是,多了个return selfObj，就是调用方法时返回自己，这样可以接着调用自己的方法。原理很简单。

#### 目前GameObject链式编程的优劣:

* 优点:代码紧凑，写起来很爽快，以自己的习惯设计接口，会提高开发效率。
* 缺点:性能会损耗一丢丢，调试不方便，出异常时候会发现堆栈信息超级长，别人看了会误认为Unity升级又加了API😂。不过DoTween,UniRx都在这么用… 

执行效率 vs 开发效率 + 低bug率，就看各位怎么权衡啦。

OK,本篇就介绍到这里。

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