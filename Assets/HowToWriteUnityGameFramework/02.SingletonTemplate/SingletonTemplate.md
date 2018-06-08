# Unity 游戏框架搭建 2018 (二) 单例的模板与最佳实践

## 背景

很多开发者或者有经验的老手都会建议尽量不要用单例模式，这是有原因的。

单例模式是设计模式中最简单的也是大家通常最先接触的一种设计模式。在框架的设计中一些管理类或者系统类多多少少都会用到单例模式，比如 QFramework 中的 UIMgr，ResMgr 都是单例。当然在平时的游戏开发过程中也会用到单例模式，比如数据管理类，角色管理类等等，以上这些都是非常常见的使用单例的应用场景。

那么今天笔者想好好聊聊单例的使用上要注意的问题，希望大家对单例有更立体的认识，并介绍 QFramework 中单例套件的使用和实现细节。

本篇文章分为四个主要内容:

1. 单例的简介
2. 几种单例的模板实现。
3. 单例的利弊分析。
4. 单例的最佳实践:如何设计一个令人愉快的 API?

## 单例模式简介

可能说有的朋友不太了解单例，笔者先对单例做一个简单的介绍。

### 定义
>保证一个类仅有一个实例，并提供一个访问它的全局访问点。

定义比较简洁而且不难理解。

再引用一个比较有意思的例子
>俺有6个漂亮的老婆，她们的老公都是我，我就是我们家里的老公 Singleton，她们只要说道“老公”，都是指的同一个人，那就是我(刚才做了个梦啦，哪有这么好的事)。-《泡妞与设计模式》

这个例子非常形象地介绍了我们日常开发中使用单例类的情景，不管在哪里都可以获得同一个并且唯一的单例类的实例。 

关于单例模式的简介就到这里，实现的细节和对模式更详尽的介绍网上到处都是，这里不再浪费篇幅。

## 单例的模板

上一篇文章中说到的 Manager of Managers 架构,其中每个 Manager 在 QFramework 中都是由单例实现，当然也可以使用静态类实现，但是相比于静态类的实现,单例更为合适。

### 如何设计这个单例的模板?

先分析下需求,当设计一个 Manager 时候,我们希望整个程序只有一个该 Manager 类的实例,一般马上能想到的实现是这样的:

``` csharp
public class XXXManager 
{
    private static XXXManager instance = null;

    private XXXManager
    {
        // to do ...
    }

    public static XXXManager()
    {
        if (instance == null)
        {
            instance = new XXXManager();
        }

        return instance;
    }
}
```

如果一个游戏需要10个各种各样的 manager,那么以上这些代码要复制粘贴好多遍。重复的代码太多!!! 想要把重复的代码抽离出来,怎么办?



答案是引入泛型。



实现如下:

``` csharp
namespace QFramework 
{  
    public abstract class Singleton<T> where T : Singleton<T>
    {
        protected static T mInstance = null;

        protected Singleton()
        {
        }

        public static T Instance
        {
            get 
            {
                if (mInstance == null)
                {
                    // 如何 new 一个T???
                }

                return mInstance;
            }
        }
    }
}
```

为了可以被继承,静态实例和构造方法都使用了 protect 修饰符。以上的问题很显而易见,那就是不能 new 一个泛型(2016 年 3月9日补充:并不是不能new一个泛型,参考:new 一个泛型的实例，编译失败了，为什么?-CSDN论坛-CSDN.NET-中国最大的IT技术社区),(2016 年 4月5日补充:有同学说可以new一个泛型的实例,不过要求改泛型提供了 public 的构造函数,好吧,这里不用new的原因是,无法显示调用 private 的构造函数)。因为泛型本身不是一个类型,那该怎么办呢?答案是使用反射。

这部分以后可能会复用，所以抽出了 SingletonCreator.cs，专门用来通过反射创建私有构造示例。

实现如下:

SingletonCreator.cs

``` csharp
namespace QFramework
{
    using System;
    using System.Reflection;

    public static class SingletonCreator
    {
        public static T CreateSingleton<T>() where T : class, ISingleton
        {
            // 获取私有构造函数
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            
            // 获取无参构造函数
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

            if (ctor == null)
            {
                throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
            }

            // 通过构造函数，常见实例
            var retInstance = ctor.Invoke(null) as T;
            retInstance.OnSingletonInit();

            return retInstance;
        }
    }
}
```

希望在单例类的内部获得初始化事件所以定制了 ISingleton 接口用来接收单例初始化事件。

ISingleton.cs

``` csharp
namespace QFramework
{    
    public interface ISingleton
    {        
        void OnSingletonInit();
    }
}
```

Singleton.cs

``` csharp
namespace QFramework
{
	public abstract class Singleton<T> : ISingleton where T : Singleton<T>
	{
		protected static T mInstance;

		static object mLock = new object();

		protected Singleton()
		{
		}

		public static T Instance
		{
			get
			{
				lock (mLock)
				{
					if (mInstance == null)
					{
						mInstance = SingletonCreator.CreateSingleton<T>();
					}
				}

				return mInstance;
			}
		}

		public virtual void Dispose()
		{
			mInstance = null;
		}

		public virtual void OnSingletonInit()
		{
		}
	}
}
```

以上就是最终实现了，并且加上了线程锁,而且实现了一个用来接收初始化事件的接口 ISingleton。这个实现是在任何 C# 程序中都是通用的。其测试用例如下所示:

``` csharp
using QFramework;  
// 1.需要继承 Singleton。
// 2.需要实现非 public 的构造方法。
public class XXXManager : Singleton<XXXManager> 
{  
    private XXXManager() 
    {
        // to do ...
    }
}


public static void main(string[] args)  
{
    XXXManager.Instance.xxxyyyzzz();
}
```

### 小结:
这个单例的模板是平时用得比较顺手的工具了，其实现是在其他的框架中发现的，拿来直接用了。反射的部分可能会耗一些性能，但是第一次调用只会执行一次，所以放心。在 Unity 中可能会需要继承 MonoBehaviour 的单例,因为很多游戏可能会只创建一个 GameObject,用来获取 MonoBehaviour 的生命周期,这些内容会再下一节中介绍:)。


## MonoBehaviour 单例的模板

上一小节讲述了如何设计 C# 单例的模板。也随之抛出了问题: 
* 如何设计接收 MonoBehaviour 生命周期的单例的模板?

### 如何设计?

先分析下需求:
* 约束脚本实例对象的个数。
* 约束 GameObject 的个数。
* 接收 MonoBehaviour 生命周期。
* 销毁单例和对应的 GameObject。

首先，第一点,约束脚本实例对象的个数,这个在上一篇中已经实现了。 但是第二点,约束 GameObject 的个数,这个需求,还没有思路,只好在游戏运行时判断有多少个 GameObject 已经挂上了该脚本,然后如果个数大于1抛出错误即可。 第三点,通过继承 MonoBehaviour 实现,只要覆写相应的回调方法即可。 第四点,在脚本销毁时,把静态实例置空。 完整的代码就如下所示:
``` csharp
using UnityEngine;

/// <summary>
/// 需要使用Unity生命周期的单例模式
/// </summary>
namespace QFramework 
{  
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T mInstance = null;

        public static T Instance()
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<T>();

                if (FindObjectsOfType<T>().Length > 1)
                {
                    Debug.LogError("More than 1!");
                    return instance;
                }

                if (instance == null)
                {
                    string instanceName = typeof(T).Name;
                    Debug.Log ("Instance Name: " + instanceName); 
                    GameObject instanceGO = GameObject.Find(instanceName);

                    if (instanceGO == null)
                        instanceGO = new GameObject(instanceName);
                        
                    instance = instanceGO.AddComponent<T>();
                    DontDestroyOnLoad(instanceGO);  //保证实例不会被释放
                    Debug.Log ("Add New Singleton " + mInstance.name + " in Game!");
                }
                else
                {
                    Debug.Log("Already exist: " + mInstance.name);
                }
            }

            return mInstance;
        }


        protected virtual void OnDestroy()
        {
            mInstance = null;
        }
    }
}
```

这样一个独立的 MonoSingleton 就实现了。

### 小结:
目前已经实现了两种单例的模板,一种是需要接收 MonoBehaviour 生命周期的,一种是不需要接收生命周期的 C# 单例的模板,可以配合着使用。虽然不是本人实现的,但是用起来可是超级爽快,2333。

## Singleton Property

文章写到这，我们已经实现了 C# 单例的模板和 MonoBehaviour 单例的模板，这两个模板已经可以满足大多数实现单例的需求了。但是偶尔还是会遇到比较奇葩的需求的。

比如这样的需求:

* 单例要继承其他的类，比如 Model.cs 等等。

虽然单例继承其他类是比较脏的设计,但是难免会遇到不得不继承的时候。没有最好的设计，只有最合适的设计。

  

  解决方案:

* 首先要保证实现单例的类从使用方式上应该不变,还是

``` csharp
XXX.Instance.ABCFunc();
```
之前的单例的模板代码如下所示:
``` csharp
namespace QFramework
{
	public abstract class Singleton<T> : ISingleton where T : Singleton<T>
	{
		protected static T mInstance;

		static object mLock = new object();

		protected Singleton()
		{
		}

		public static T Instance
		{
			get
			{
				lock (mLock)
				{
					if (mInstance == null)
					{
						mInstance = SingletonCreator.CreateSingleton<T>();
					}
				}

				return mInstance;
			}
		}

		public virtual void Dispose()
		{
			mInstance = null;
		}

		public virtual void OnSingletonInit()
		{
		}
	}
}
```

按照以前的方式,如果想实现一个单例的代码应该是这样的:

``` csharp
using QFramework;  
// 1.需要继承QSingleton。
// 2.需要实现非public的构造方法。
public class XXXManager : QSingleton<XXXManager> 
{  
    private XXXManager() 
    {
        // to do ...
    }
}

public static void main(string[] args)  
{
    XXXManager.Instance().xxxyyyzzz();
}
```

如果我想 XXXManager 继承一个 BaseManager 代码就变成这样了
``` csharp
using QFramework;  
// 1.需要继承QSingleton。
// 2.需要实现非public的构造方法。
public class XXXManager : BaseManager 
{  
    private XXXManager() 
    {
        // to do ...
    }
}
```

这样这个类就不是单例了,怎么办?
答案是通过 C# 的属性器。
``` csharp
using QFramework;  
// 1.需要继承QSingleton。
// 2.需要实现非public的构造方法。
public class XXXManager : BaseManager,ISingleton
{  
    private XXXManager() 
    {
    	// 不建议在这里初始化代码
    }
    
    void ISingleton.OnSingletonInit()
    {
        // to do ...
    }
    
    public static XXXManager Instance 
    { 
        get 
        {
            return SingletonProperty<XXXManager>.Instance;
        }
    }
}

public static void main(string[] args)  
{
    XXXManager.Instance.xxxyyyzzz();
}
```
好了,又看到陌生的东西了,SingletonProperty 是什么?
和之前的单例的模板很相似,贴上代码自己品吧...

``` csharp
namespace QFramework
{
	public static class SingletonProperty<T> where T : class, ISingleton
	{
		private static T mInstance;
		private static readonly object mLock = new object();

		public static T Instance
		{
			get
			{
				lock (mLock)
				{
					if (mInstance == null)
					{
						mInstance = SingletonCreator.CreateSingleton<T>();
					}
				}

				return mInstance;
			}
		}

		public static void Dispose()
		{
			mInstance = null;
		}
	}
}
```

这样无法继承的问题就解决啦。
缺点是:相比于 Singleton，SingletonProperty 在使用时候多了一次函数调用，而且还要再实现个 getter,不过问题解决啦,。

## 单例的利弊

在介绍单例的最佳实践之前，我们要先分析下单例的利弊。



首先我们先从定义上入手。

> 保证一个类仅有一个实例，并提供一个访问它的全局访问点。

就两句话：

1. 保证一个类仅有一个实例。
2. 提供一个访问它的全局访问点。



保证一个类仅有一个实例,这个是对单例的一个需求。但是这句话没有告诉你，这个实例什么时候应该去创建。而笔者所知到的创建方式一般是有两种，第一种是在程序编译后马上创建，一般实现方式是在声明静态成员变量的时候去 new 一个实例，实现如下。

``` csharp
public class Test
{
    public static readonly Test Instance = new Test();
}
```

这种方式最简单，也最容易实现。

第二种则第一次获取实例时去创建，实现如下:

``` csharp
public class Test
{
    public static Test mInstance;
    
    public static Test Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new Test();
            }
            
            return mInstance;
        }
    }
}
```

这种单例实现也比较常见，被称为懒单例模式，乘坐懒的原因是用到的时候再去创建，这样可以减缓内存和 CPU 压力。造成的风险则是，声明周期不可控。



所以说第一个利弊是懒加载的利弊。

* 懒加载
  * 优点:减少内存和 CPU  压力。
  * 缺点:声明周期不可控。

懒加载是可用不可用的，在 Unity 开发中一般用单例的模板时候都是用懒加载的方式的。



其他的还有 全局唯一 和 全局访问。

全局唯一这个没什么好说的，单例的存在就是为了保证全局唯一，只有个优点吧。



提供全局访问。提供全局访问这个功能，优点是方便获取单例实例。缺点就很明显了，在文章的开始，笔者说

> 很多开发者或者有经验的老手都会建议尽量不要用单例模式，这是有原因的。



这个原因就是因为全局访问。一个实例的全局访问会有很多风险，当然静态类也是可以全局访问的。但是静态类一般我们用作工具或者 Helper，所以没什么问题。但是单例本身是一个实例，是一个对象。所以对象有的时候是有声明周期的，并且有时候还有上下文(缓存的数据、状态)。而有时候还须有一定特定的顺序去使用 API。这些都是非常有可能的。 所以说要设计一个好的单例类，好的管理类。是对开发者要求是非常高的。不过在这里笔者提醒一下，不是说要把单例类设计得非常好才是完全正确的。有的时候，我们来不及花精力去设计，考虑周全，但是可以完成工作，完成任务，这样最起码是对得起公司付的工资的，而且功能完成了，等不忙的时候可以回来再思考的嘛，罗马不是一天建成的，但是罗马可以通过一点一点迭代完成。具体要求高在哪里，主要是符合设计模式的六大设计原则就好。

接下来笔者就贴出一个笔者认为比较严格的单例类设计。



原则上是，保留单例优点的同时，去削弱使用它的风险。

目前来看，单例使用的风险主要是全局访问，所以削弱全局访问就好了。笔者所分享的方式是，对外提供的 API 都用静态 API。Instance 变量不对外提供，外部访问只能通过静态的 API。而内部则维护一个私有的单例实例。

代码如下：

``` csharp
using System;
using QFramework;
using UnityEngine;

/// <summary>
/// 职责:
/// 1. 用户数据管理
/// 2. 玩家数据管理
/// 3. Manager  容器: List/Dictionary  增删改查
/// </summary>
///
///
public class PlayerData
{
    public string Username;
    
    public int Level;
    
    public string Carrer;
}


[QMonoSingletonPath("[Game]/PlayerDataMgr")]
public class PlayerDataMgr : MonoBehaviour,ISingleton
{
    private static PlayerDataMgr mInstance
    {
        get { return MonoSingletonProperty<PlayerDataMgr>.Instance; }
    }
   
    /// <summary>
    /// 对外阉割
    /// </summary>
    void ISingleton.OnSingletonInit()
    {
        mPlayerData = new PlayerData();
        // 从本地加载的一个操作

    }
    
    #region public 对外提供的 API

    public static void SavePlayerData()
    {
        mInstance.Save();
    }
    
    public static PlayerData GetPlayerData()
    {
        return mInstance.mPlayerData;
    }
    
    #endregion

    
    private PlayerData mPlayerData;

    
    private void Save()
    {
        // 保存到本地
    }
}
```



使用上非常干净简洁:

``` csharp
public class TestMonoSingletonA : MonoBehaviour {

	// Use this for initialization
	private void Start()
	{
		var playerData = PlayerDataMgr.GetPlayerData();

		playerData.Level++;

		PlayerDataMgr.SavePlayerData();		
	}

	// Update is called once per frame
	void Update () {
		
	}
}
```

### 命名小建议 

到这里还要补充一下，笔者呢不太喜欢 Instance 这个命名。在命名上，很多书籍都建议用业务命名而不是用技术概念来命名。

比如 PlayerDataSaver 是业务命名，但是 SerializeHelper 则是技术命名，本质上他们两个都可以做数据存储相关的任务。但是 PlayerDataSaver,更适合人类阅读。

Instance 是技术概念命名，而不是业务命名。尽量不要让技术概念的命名出现在 UI/逻辑层。只可以在框架层或者插件曾出现是允许的。

以上这些是笔者的自己的观点，不是标准的原则，大家看看就好。



今天的内容就这些，谢谢阅读~

#### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework&游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](https://ws3.sinaimg.cn/large/006tNc79gy1fs3dpkrblsj30by0byt9i.jpg)

### 如果有帮助到您:

如果觉得本篇教程或者 QFramework 对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

- 给 QFramework 一个 Star:https://github.com/liangxiegame/QFramework
- 下载 Asset Store 上的 QFramework 给个五星(如果有评论小的真是感激不尽):http://u3d.as/SJ9
- 购买 gitchat 话题并给 5 星好评: http://gitbook.cn/gitchat/activity/5abc3f43bad4f418fb78ab77 (6 元，会员免费)
- 购买同名的蛮牛视频课程并给 5 星好评:http://edu.manew.com/course/431 (目前定价 ~~19~~ 29.8 元)
- 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design( 29.9 元，内容会在 2018 年 10 月份完结)

笔者在这里保证 QFramework、入门教程、文档和此框架搭建系列的专栏永远免费开源。以上捐助产品的内容对于使用 QFramework 的使用来讲都不是必须的，所以大家不用担心，各位使用 QFramework 或者 阅读此专栏 已经是对笔者团队最大的支持了。