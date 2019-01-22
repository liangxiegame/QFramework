# Unity 游戏框架搭建 (十九) 简易对象池

在Unity中我们经常会用到对象池，使用对象池无非就是解决两个问题:

* 一是减少new时候寻址造成的消耗，该消耗的原因是内存碎片。
* 二是减少Object.Instantiate时内部进行序列化和反序列化而造成的CPU消耗。

想进一步了解对象池模式优化原理的同学可以参阅: [对象池模式:http://gpp.tkchu.me/object-pool.html][1]，本篇主要讲如何实现一个精简并且灵活的对象池。

## 设计：

首先我们要弄清楚本篇对象池的几个概念，否则直接上代码大家会一头雾水。
从字面上理解对象池，池的意思就是容器。我们可以从池中获取一个对象(一条鱼)，也可以向池中放入一个对象(一条鱼)。获取的操作我们叫Allocate(分配),而放入一个对象我们叫Recycle(回收)(ps:也有很多习惯叫Spawn和Despawn的,这个看自己习惯了)。所以我们可以定义池的接口为如下:
```cs
    public interface IPool<T>
    {
        T Allocate();

        bool Recycle(T obj);
    }
```
为什么要用泛型呢？因为笔者开头说过，本篇主要讲如何实现一个精简并且灵活的对象池。这个灵活很大一部分是通过泛型体现的。

前面有说过，池是容器的意思，在C#中可以是List,Queue或者Stack甚至是数组。所以对象池本身要维护一个容器。本篇我们选取Stack来作为池容器，原因是当我们在Allocate和Recycle时并不关心缓存的存储的顺序，只要求缓存对象的地址是连续的。代码如下所示:
```cs
    using System.Collections.Generic;

    public abstract class Pool<T> : IPool<T>
    {
		  ...
        protected Stack<T> mCacheStack = new Stack<T>();
	      ...
    }
```

Pool是个抽象类，为什么呢? 因为笔者开头说过，本篇主要讲如何实现一个精简并且灵活的对象池。这个灵活很大一部分是通过抽象类体现的。

现在对象的存取和缓存接口都设计好了，那么这些对象是从哪里来的呢？我们分析下，创建对象我们知道有两种方式，反射构造方法和new一个对象。对象池的一个重要功能就是缓存，要想实现缓存就要求对象可以在对象池内部进行创建。所以我们要抽象出一个对象的工厂，代码如下所示:
```cs
    public interface IObjectFactory<T>
    {
        T Create();
    }
```
那么大家要问为什么要用工厂? 因为笔者开头说过，本篇主要讲如何实现一个精简并且灵活的对象池。这个灵活很大一部分是通过工厂体现的。

OK，现在对象的创建，存取，缓存的接口都设计好了。下面放出Pool的全部代码。
```cs
    using System.Collections.Generic;

    public abstract class Pool<T> : IPool<T>
    {
        #region ICountObserverable
        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>The current count.</value>
        public int CurCount
        {
            get { return mCacheStack.Count; }
        }
        #endregion
        
        protected IObjectFactory<T> mFactory;

        protected Stack<T> mCacheStack = new Stack<T>();

        /// <summary>
        /// default is 5
        /// </summary>
        protected int mMaxCount = 5;

        public virtual T Allocate()
        {
            return mCacheStack.Count == 0
                ? mFactory.Create()
                : mCacheStack.Pop();
        }

        public abstract bool Recycle(T obj);
    }
```

代码不多，设计阶段基本就这样。下面介绍如何实现一个简易的对象池。

## 对象池实现

首先要实现一个对象的创建器,代码如下所示:
```cs
    using System;

    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        public CustomObjectFactory(Func<T> factoryMethod)
        {
            mFactoryMethod = factoryMethod;
        }
        
        protected Func<T> mFactoryMethod;

        public T Create()
        {
            return mFactoryMethod();
        }
    }
```
比较简单，只是维护了一个返回值为T的委托(如果说得有误请指正)。
对象池实现:
```cs
    using System;

    /// <summary>
    /// unsafe but fast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleObjectPool<T> : Pool<T>
    {
        readonly Action<T> mResetMethod;

        public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null,int initCount = 0)
        {
            mFactory = new CustomObjectFactory<T>(factoryMethod);
            mResetMethod = resetMethod;

            for (int i = 0; i < initCount; i++)
            {
                mCacheStack.Push(mFactory.Create());
            }
        }

        public override bool Recycle(T obj)
        {
            mResetMethod.InvokeGracefully(obj);
            mCacheStack.Push(obj);
            return true;
        }
    }
```
实现也很简单，这里不多说了。

## 如何使用?
```cs
			var fishPool = new SimpleObjectPool<Fish>(() => new Fish(), null, 100);

			Log.I("fishPool.CurCount:{0}", fishPool.CurCount);

			var fishOne = fishPool.Allocate();
			
			Log.I("fishPool.CurCount:{0}", fishPool.CurCount);

			fishPool.Recycle(fishOne);
			
			Log.I("fishPool.CurCount:{0}", fishPool.CurCount);

			for (int i = 0; i < 10; i++)
			{
				fishPool.Allocate();
			}
			
			Log.I("fishPool.CurCount:{0}", fishPool.CurCount);
```
运行结果:

```csharp
fishPool.CurCount:100
fishPool.CurCount:99
fishPool.CurCount:100
fishPool.CurCount:90
```

OK，本篇就介绍到这里

## 相关链接:
[我的框架地址][2]:https://github.com/liangxiegame/QFramework

[教程源码][3]:https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework &游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记][4] http://liangxiegame.com/

微信公众号:liangxiegame

![][image-1]

## 如果有帮助到您:
如果觉得本篇教程对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

* 给 [QFramework][5] 一个 Star
	* 地址: https://github.com/liangxiegame/QFramework
* 给 Asset Store 上的 QFramework 并给个五星(需要先下载)
	* 地址: http://u3d.as/SJ9
* 购买 gitchat 话题:[《命名的力量：变量》][6]
	* 价格: 12 元
	* 地址: [https://gitbook.cn/gitchat/activity/5b65904096290075f5829388 ][7]
* 购买同名的蛮牛视频课程录播课程: 
	* 价格 49.2 元
	* 地址: [http://edu.manew.com/course/431][8]
* 购买同名电子书:[https://www.kancloud.cn/liangxiegame/unity_framework_design][9]
	* 价格  49.2 元，内容会在 2018 年 10 月份完结

[1]:	http://gpp.tkchu.me/object-pool.html
[2]:	https://github.com/liangxiegame/QFramework
[3]:	https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/%0A
[4]:	http://liangxiegame.com/
[5]:	https://github.com/liangxiegame/QFramework
[6]:	https://gitbook.cn/gitchat/activity/5b65904096290075f5829388
[7]:	https://gitbook.cn/gitchat/activity/5b65904096290075f5829388 "https://gitbook.cn/gitchat/activity/5b65904096290075f5829388"
[8]:	http://edu.manew.com/course/431
[9]:	https://www.kancloud.cn/liangxiegame/unity_framework_design

[image-1]:	https://ws4.sinaimg.cn/large/006tKfTcgy1fryc5skygwj30by0byt9i.jpg