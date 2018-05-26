# Unity 游戏框架搭建 (二) 单例的模板

上一篇文章中说到的manager of managers,其中每个manager都是单例的实现,当然也可以使用静态类实现,但是相比于静态类的实现,单例的实现更为通用,可以适用大多数情况。

#### 如何设计这个单例的模板?

先分析下需求,当设计一个manager时候,我们希望整个程序只有一个该manager对象实例,一般马上能想到的实现是这样的:

```c#
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
如果一个游戏需要10个各种各样的manager,那么以上这些代码要复制粘贴好多遍。重复的代码太多!!!想要把重复的代码抽离出来,怎么办?答案是引入泛型。实现如下:

```C#
using System;  
using System.Collections.Generic;  
using System.Text;  
using System.Reflection;

namespace QFramework 
{  
    public abstract class QSingleton<T> where T : QSingleton<T>
    {
        protected static T instance = null;

        protected QSingleton()
        {
        }

        public static T Instance()
        {
            if (instance == null)
            {
                // 如何new 一个T???
            }

            return instance;
        }
    }
}
```
为了可以被继承,静态实例和构造方法都使用protect修饰符。以上的问题很显而易见,那就是不能new一个泛型(3月9日补充:并不是不能new一个泛型,参考:[new一个泛型的实例，编译失败了，为什么?-CSDN论坛-CSDN.NET-中国最大的IT技术社区](http://bbs.csdn.net/topics/390911693)),(4月5日补充:有同学说可以new一个泛型的实例,不过要求改泛型提供了public的构造函数,好吧,这里不用new的原因是,无法显示调用private的构造函数)。因为泛型本身不是一个类型,那该怎么办呢?答案是使用反射。实现如下:

```C#
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
namespace QFramework 
{  
    public abstract class QSingleton<T> where T : QSingleton<T>
    {
        protected static T instance = null;

        protected QSingleton()
        {
        }

        public static T Instance()
        {
            if (instance == null)
            {
                // 先获取所有非public的构造方法
                ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                // 从ctors中获取无参的构造方法
                ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
                if (ctor == null)
                    throw new Exception("Non-public ctor() not found!");
                // 调用构造方法
                instance = ctor.Invoke(null) as T;
            }

            return instance;
        }
    }
}
```

以上就是最终实现了。这个实现是在任何C#程序中都是通用的。其测试用例如下所示:

```
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
#### 总结:
这个单例的模板是平时用得比较顺手的工具了,其实现是在其他的框架中发现的,拿来直接用了。反射的部分可能会耗一些性能,但是只会执行一次。在Unity中可能会需要继承MonoBehaviour的单例,因为很多游戏可能会只创建一个GameObject,用来获取MonoBehaviour的生命周期,这些内容会再下一讲中介绍:)。

#### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework&游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](http://liangxiegame.com/content/images/2017/06/qrcode_for_gh_32f0f3669ac8_430.jpg)

### 如果有帮助到您:

如果觉得本篇教程或者 QFramework 对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

- 给 QFramework 一个 Star:https://github.com/liangxiegame/QFramework
- 下载 Asset Store 上的 QFramework 给个五星(如果有评论小的真是感激不尽):http://u3d.as/SJ9
- 购买 gitchat 话题并给 5 星好评: http://gitbook.cn/gitchat/activity/5abc3f43bad4f418fb78ab77 (6 元，会员免费)
- 购买同名的蛮牛视频课程并给 5 星好评:http://edu.manew.com/course/431 (目前定价 29.8 元)
- 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design( 29.9 元，内容会在 2018 年 10 月份完结)

笔者在这里保证 QFramework、入门教程、文档和此框架搭建系列的专栏永远免费开源。以上捐助产品的内容对于使用 QFramework 的使用来讲都不是必须的，所以大家不用担心，各位使用 QFramework 或者 阅读此专栏 已经是对笔者团队最大的支持了。