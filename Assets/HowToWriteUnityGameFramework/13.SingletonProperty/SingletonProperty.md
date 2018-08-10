# Unity 游戏框架搭建 (十三) 无需继承的单例的模板

之前的文章中介绍的[Unity 游戏框架搭建 (二) 单例的模板][1]和[Unity 游戏框架搭建 (三) MonoBehaviour单例的模板][2]有一些问题。

#### 存在的问题:

* 只要继承了单例的模板就无法再继承其他的类。

虽然单例继承其他类是比较脏的设计,但是难免会遇到不得不继承的时候。没有最好的设计，只有最合适的设计。

#### 解决方案:

* 首先实现单例的类从使用方式上应该不变,还是
```csharp
 XXX.Instance.ABCFunc()
```

之前的单利的模板代码如下所示:
```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

/// <summary>
/// 1.泛型
/// 2.反射
/// 3.抽象类
/// 4.命名空间
/// </summary>
namespace QFramework {
	public abstract class QSingleton<T> where T : QSingleton<T>
	{
		protected static T mInstance = null;

		protected QSingleton()
		{
		}

		public static T Instance
		{

			get {
				if (mInstance == null) {
					// 先获取所有非public的构造方法
					ConstructorInfo[] ctors = typeof(T).GetConstructors (BindingFlags.Instance | BindingFlags.NonPublic);
					// 从ctors中获取无参的构造方法
					ConstructorInfo ctor = Array.Find (ctors, c => c.GetParameters ().Length == 0);
					if (ctor == null)
						throw new Exception ("Non-public ctor() not found!");
					// 调用构造方法
					mInstance = ctor.Invoke (null) as T;
				}

				return mInstance;
			}
		}

		public void Dispose()
		{
			mInstance = null;
		}
	}
}
```


按照以前的方式,如果想实现一个单例的代码应该是这样的:

```csharp
using QFramework;  
// 1.需要继承QSingleton。
// 2.需要实现非public的构造方法。
public class XXXManager : QSingleton<XXXManager> {  
    private XXXManager() {
        // to do ...
    }
}


public static void main(string[] args)  
{
    XXXManager.Instance().xxxyyyzzz();
}
```

如果我想XXXManager继承一个BaseManager代码就变成这样了

```csharp
using QFramework;  
// 1.需要继承QSingleton。
// 2.需要实现非public的构造方法。
public class XXXManager : BaseManager {  
    private XXXManager() {
        // to do ...
    }
}


public static void main(string[] args)  
{
    XXXManager.Instance().xxxyyyzzz();
}
```

这样这个类就不是单例了,怎么办?

答案是通过C#的属性。

```csharp
using QFramework;  
// 1.需要继承QSingleton。
// 2.需要实现非public的构造方法。
public class XXXManager : BaseManager {  
    private XXXManager() {
        // to do ...
    }
    public static XXXManager Instance { 
        get {
            return QSingletonComponent<XXXManager>.Instance;
        }
    }
}


public static void main(string[] args)  
{
    XXXManager.Instance().xxxyyyzzz();
}
```

好了,又看到陌生的东西了,QSingletonComponent是什么?

和之前的单例的模板很相似,贴上代码自己品吧...

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

/// <summary>
///	组合方式实现单例子
/// </summary>
namespace QFramework {

	/// <summary>
	/// class是引用类型
	/// </summary>
	public class QSingletonComponent<T> where T : class
	{
		protected static T mInstance = null;

		public static T Instance
		{

			get {
				if (mInstance == null) {
					// 先获取所有非public的构造方法
					ConstructorInfo[] ctors = typeof(T).GetConstructors (BindingFlags.Instance | BindingFlags.NonPublic);
					// 从ctors中获取无参的构造方法
					ConstructorInfo ctor = Array.Find (ctors, c => c.GetParameters ().Length == 0);
					if (ctor == null)
						throw new Exception ("Non-public ctor() not found!");
					// 调用构造方法
					mInstance = ctor.Invoke (null) as T;
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

缺点是:相比于QSingleton,QSingletonComponent在使用时候多了一次函数调用,不过做中小型项目应该可以应付了。

介绍完毕,睡觉了。。。

#### 欢迎讨论!

## 相关链接:
[我的框架地址][3]:https://github.com/liangxiegame/QFramework

[教程源码][4]:https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework &游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记][5] http://liangxiegame.com/

微信公众号:liangxiegame

![][image-1]

## 如果有帮助到您:
如果觉得本篇教程对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

* 给 [QFramework][6] 一个 Star
	* 地址: https://github.com/liangxiegame/QFramework
* 给 Asset Store 上的 QFramework 并给个五星(需要先下载)
	* 地址: http://u3d.as/SJ9
* 购买 gitchat 话题[《命名的力量：变量》][7]
	* 价格: 12 元
	* 地址: http://gitbook.cn/gitchat/activity/5b29df073104f252297a779c
* 购买同名的蛮牛视频课程录播课程: 
	* 价格 49.2 元
	* 地址: http://edu.manew.com/course/431
* 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design
	* 价格  49.2 元，内容会在 2018 年 10 月份完结

[1]:	http://liangxiegame.com/unity-you-xi-kuang-jia-da-jian-er-dan-li-de-mo-ban/
[2]:	http://liangxiegame.com/unity-you-xi-kuang-jia-da-jian-san-monobehaviourdan-li-de-mo-ban/
[3]:	https://github.com/liangxiegame/QFramework
[4]:	https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/%0A
[5]:	http://liangxiegame.com/
[6]:	https://github.com/liangxiegame/QFramework
[7]:	%20http://gitbook.cn/gitchat/activity/5b29df073104f252297a779c

[image-1]:	https://ws4.sinaimg.cn/large/006tKfTcgy1fryc5skygwj30by0byt9i.jpg