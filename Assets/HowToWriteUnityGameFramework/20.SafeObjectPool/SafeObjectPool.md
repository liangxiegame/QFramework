# Unity 游戏框架搭建 (二十) 更安全的对象池

上篇文章介绍了,只需通过实现 IObjectFactory 接口和继承 Pool 类，就可以很方便地实现一个SimpleObjectPool。SimpleObjectPool 可以满足大部分的对象池的需求。而笔者通常将 SimpleObjectPool 用于项目开发，原因是接入比较方便，适合在发现性能瓶颈时迅速接入，不需要更改瓶颈对象的内部代码，而且代码精简较容易掌控。

本篇内容会较多:)

#### 新的需求来了

当我们把对象池应用在框架开发中，我们就有了新的需求。
* 要保证使用时安全。
* 易用性。

现在让我们思考下 SimpleObjectPool 哪里不安全?

贴上 SimpleObjectPool 的源码:
``` csharp
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

首先不安全的地方是泛型 T,在上篇文章中我们说泛型是灵活的体现，但是在框架设计中未约束的泛型却有可能是未知的隐患。我们很有可能在写代码时把 SimpleObjectPool<Fish> 写成 SimpleObjectPool<Fit>，而如果恰好你的工程里有 Fit 类，再加上使用var来声明变量而不是具体的类型（笔者较喜欢用var），那么这个错误要过好久才能发现。

为了解决这个问题，我们要给泛型T加上约束。要求可被对象池管理的对象必须是某种类型。是什么类型呢？就是IPoolAble类型。

``` csharp
    public interface IPoolable
    {
        
    }
```

然后我们要给对象池类的泛型加上类型约束，本文的对象池我们叫SafeObjectPool。

``` csharp
    public class SafeObjectPool<T> : Pool<T> where T : IPoolable
```

OK，第一个安全问题解决了。

第二个安全问题来了,我们有可能将一个 IPoolable 对象回收两次。为了解决这个问题，我们可以在SafeObjectPool 维护一个已经分配过的对象容器来记录对象是否被回收过，也可以在 IPoolable 对象中增加是否被回收的标记。这两种方式笔者倾向于后者，维护一个容器的成本相比只是在对象上增加标记的成本来说高太多了。

我们在 IPoolable 接口上增加一个 bool 变量来表示对象是否被回收过。

``` csharp
    public interface IPoolAble
    {        
        bool IsRecycled { get; set; }
    }
```

接着在进行 Allocate 和 Recycle 时进行标记和拦截。
``` csharp
    public class SafeObjectPool<T> : Pool<T> where T : IPoolAble
    {
		  ...
        public override T Allocate()
        {
            T result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }

        public override bool Recycle(T t)
        {
            if (t == null || t.IsRecycled)
            {
                return false;
            }

            t.IsRecycled = true;
            mCacheStack.Push(t);

            return true;
        }
    }
```

OK,第二个安全问题解决了。接下来第三个不是安全问题，是职责问题。我们再次观察下上篇文章中的SimpleObjectPool
``` csharp
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

可以看到，对象回收时的重置操作是由构造函数传进来的 mResetMethod 来完成的。当然，上篇忘记说了，这也是灵活的体现：）通过将重置的控制权开放给开发者，这样在接入 SimpleObjectPool 时，不需要更改对象内部的代码。

在框架设计中我们要收敛一些了，重置的操作要由对象自己来完成，我们要在 IPoolable 接口增加一个接收重置事件的方法。

``` csharp
    public interface IPoolAble
    {
        void OnRecycled();
        
        bool IsRecycled { get; set; }
    }
```

当 SafeObjectPool 回收对象时来触发它。

``` csharp
    public class SafeObjectPool<T> : Pool<T> where T : IPoolAble
    {
		  ...
        public override bool Recycle(T t)
        {
            if (t == null || t.IsRecycled)
            {
                return false;
            }

            t.IsRecycled = true;
            t.OnRecycled();
            mCacheStack.Push(t);

            return true;
        }
    }

```

同样地，在 SimpleObjectPool 中，创建对象的控制权我们也开放了出去，在 SafeObjectPool 中我们要收回来。还记得上篇文章的 CustomObjectFactory 嘛?

``` csharp
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

CustomObjectFactory 不管要创建对象的构造方法是私有的还是公有的，只要开发者有办法搞出个对象就可以。现在我们要加上限制，大部分对象是 new 出来的。所以我们要设计一个可以 new 出对象的工厂。我们叫它 DefaultObjectFactory。

``` csharp
    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
        public T Create()
        {
            return new T();
        }
    }
```

注意下对泛型 T 的约束:)

接下来我们在构造 SafeObjectPool 时，创建一个 DefaultObjectFactory。

``` csharp
    public class SafeObjectPool<T> : Pool<T> where T : IPoolAble, new()
    {
        public SafeObjectPool()
        {
            mFactory = new DefaultObjectFactory<T>();
        }
		  ...
```

注意 SafeObjectPool 的泛型也要加上 new() 的约束。

这样安全的 SafeObjectPool 已经完成了。

我们先测试下:

``` csharp
		class Msg : IPoolAble
		{
			public void OnRecycled()
			{
				Log.I("OnRecycled");
			}
			
			public bool IsRecycled { get; set; }
		}
		
		private void Start()
		{
			var msgPool = new SafeObjectPool<Msg>();
			
			msgPool.Init(100,50); // max count:100 init count: 50
			
			Log.I("msgPool.CurCount:{0}", msgPool.CurCount);
			
			var fishOne = msgPool.Allocate();
			
			Log.I("msgPool.CurCount:{0}", msgPool.CurCount);
			
			msgPool.Recycle(fishOne);

			Log.I("msgPool.CurCount:{0}", msgPool.CurCount);
			
			for (int i = 0; i < 10; i++)
			{
				msgPool.Allocate();
			}
			
			Log.I("msgPool.CurCount:{0}", msgPool.CurCount);
		}
```

由于是框架级的对象池，例子将上文的 Fish 改成 Msg。

输出结果:
``` csharp
OnRecycled 
OnRecycled
... x50
msgPool.CurCount:50
msgPool.CurCount:49
OnRecycled
msgPool.CurCount:50
msgPool.CurCount:40
```

OK，测试结果没问题。不过，难道要让用户自己去维护 Msg 的对象池?

#### 改进:
以上只是保证了机制的安全，这还不够。
我们想要用户获取一个 Msg 对象应该像 new Msg() 一样自然。要做到这样，我们需要做一些工作。

首先，Msg 的对象池全局只有一个就够了，为了实现这个需求，我们会想到用单例，但是 SafeObjectPool 已经继承了 Pool 了，不能再继承 QSingleton 了。还记得以前介绍的 QSingletonProperty 嘛？是时候该登场了，代码如下所示。

``` csharp
    /// <summary>
    /// Object pool.
    /// </summary>
    public class SafeObjectPool<T> : Pool<T>, ISingleton where T : IPoolAble, new()
    {
        #region Singleton
        protected void OnSingletonInit()
        {
        }

        public SafeObjectPool()
        {
            mFactory = new DefaultObjectFactory<T>();
        }

        public static SafeObjectPool<T> Instance
        {
            get { return QSingletonProperty<SafeObjectPool<T>>.Instance; }
        }

        public void Dispose()
        {
            QSingletonProperty<SafeObjectPool<T>>.Dispose();
        }
        #endregion
```

注意，构造方法的访问权限改成了 protected.

我们现在不想让用户通过 SafeObjectPool 来 Allocate 和 Recycle 池对象了，那么 Allocate 和 Recycle 的控制权就要交给池对象来管理。

由于控制权交给池对象管理这个需求不是必须的，所以我们要再提供一个接口
``` csharp
    public interface IPoolType
    {
        void Recycle2Cache();
    }
```

为什么只有一个 Recycle2Cache,没有 Allocate 相关的方法呢？
因为在池对象创建之前我们没有任何池对象，只能用静态方法创建。这就需要池对象提供一个静态的 Allocate 了。使用方法如下所示。

``` csharp
		class Msg : IPoolAble,IPoolType
		{
			#region IPoolAble 实现

			public void OnRecycled()
			{
				Log.I("OnRecycled");
			}
			
			public bool IsRecycled { get; set; }

			#endregion
		
			
			#region IPoolType 实现

			public static Msg Allocate()
			{
				return SafeObjectPool<Msg>.Instance.Allocate();
			}
			
			public void Recycle2Cache()
			{
				SafeObjectPool<Msg>.Instance.Recycle(this);
			}
			
			#endregion
		}
```

贴上测试代码:

``` csharp
			SafeObjectPool<Msg>.Instance.Init(100, 50);			
			
			Log.I("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount);
			
			var fishOne = Msg.Allocate();
			
			Log.I("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount);
			
			fishOne.Recycle2Cache();

			Log.I("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount);
			
			for (int i = 0; i < 10; i++)
			{
				Msg.Allocate();
			}
			
			Log.I("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount);
```

测试结果:
``` csharp
OnRecycled 
OnRecycled
... x50
msgPool.CurCount:50
msgPool.CurCount:49
OnRecycled
msgPool.CurCount:50
msgPool.CurCount:40
```

测试结果一致，现在贴上 SafeObejctPool 的全部代码。这篇文章内容好多，写得我都快吐了- -。

``` csharp
   using System;

    /// <summary>
    /// I cache type.
    /// </summary>
    public interface IPoolType
    {
        void Recycle2Cache();
    }

    /// <summary>
    /// I pool able.
    /// </summary>
    public interface IPoolAble
    {
        void OnRecycled();
        
        bool IsRecycled { get; set; }
    }

    /// <summary>
    /// Count observer able.
    /// </summary>
    public interface ICountObserveAble
    {
        int CurCount { get; }
    }

    /// <summary>
    /// Object pool.
    /// </summary>
    public class SafeObjectPool<T> : Pool<T>, ISingleton where T : IPoolAble, new()
    {
        #region Singleton
        public void OnSingletonInit()
        {
        }

        protected SafeObjectPool()
        {
            mFactory = new DefaultObjectFactory<T>();
        }

        public static SafeObjectPool<T> Instance
        {
            get { return QSingletonProperty<SafeObjectPool<T>>.Instance; }
        }

        public void Dispose()
        {
            QSingletonProperty<SafeObjectPool<T>>.Dispose();
        }
        #endregion


        /// <summary>
        /// Init the specified maxCount and initCount.
        /// </summary>
        /// <param name="maxCount">Max Cache count.</param>
        /// <param name="initCount">Init Cache count.</param>
        public void Init(int maxCount, int initCount)
        {
            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);

                mMaxCount = maxCount;
            }

            if (CurCount < initCount)
            {
                for (int i = CurCount; i < initCount; ++i)
                {
                    Recycle(mFactory.Create());
                }
            }
        }

        /// <summary>
        /// Gets or sets the max cache count.
        /// </summary>
        /// <value>The max cache count.</value>
        public int MaxCacheCount
        {
            get { return mMaxCount; }
            set
            {
                mMaxCount = value;

                if (mCacheStack != null)
                {
                    if (mMaxCount > 0)
                    {
                        if (mMaxCount < mCacheStack.Count)
                        {
                            int removeCount = mMaxCount - mCacheStack.Count;
                            while (removeCount > 0)
                            {
                                mCacheStack.Pop();
                                --removeCount;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allocate T instance.
        /// </summary>
        public override T Allocate()
        {
            T result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }

        /// <summary>
        /// Recycle the T instance
        /// </summary>
        /// <param name="t">T.</param>
        public override bool Recycle(T t)
        {
            if (t == null || t.IsRecycled)
            {
                return false;
            }

            if (mMaxCount > 0)
            {
                if (mCacheStack.Count >= mMaxCount)
                {
                    t.OnRecycled();
                    return false;
                }
            }

            t.IsRecycled = true;
            t.OnRecycled();
            mCacheStack.Push(t);

            return true;
        }
    }
```

代码实现很简单，但是要考虑很多。

#### 总结:
* SimpleObjectPool 适合用于项目开发，渐进式，更灵活。
* SafeObjectPool 适合用于库级开发，更多限制，要求开发者一开始就想好，更安全。

OK，今天就到这里。


#### 相关链接:

附: [我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

QFramework&游戏框架搭建QQ交流群: 623597263

微信公众号:liangxiegame

![](https://ws3.sinaimg.cn/large/006tKfTcgy1frqsbrq6psj30by0byt9i.jpg)

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