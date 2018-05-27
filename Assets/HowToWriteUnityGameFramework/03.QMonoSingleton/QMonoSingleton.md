# Unity 游戏框架搭建 (三) MonoBehaviour单例的模板

上一篇文章讲述了如何设计C#单例的模板。也随之抛出了问题:
* 如何设计接收MonoBehaviour生命周期的单例的模板?
#### 如何设计?
先分析下需求:

* 约束脚本实例对象的个数。 
* 约束 GameObject 的个数。 
* 接收 MonoBehaviour 生命周期。 
* 销毁单例和对应的 GameObject。

首先，第一点,约束脚本实例对象的个数,这个在上一篇中已经实现了。
但是第二点,约束 GameObject 的个数,这个需求,还没有思路,只好在游戏运行时判断有多少个 GameObject 已经挂上了该脚本,然后如果个数大于 1 抛出错误即可。 
第三点,通过继承 MonoBehaviour 实现,只要覆写相应的回调方法即可。 第四点,在脚本销毁时,把静态实例置空。 完整的代码就如下所示:
```csharp
using UnityEngine;

/// <summary>
/// 需要使用Unity生命周期的单例模式
/// </summary>
namespace QFramework 
{  
    public abstract class QMonoSingleton<T> : MonoBehaviour where T : QMonoSingleton<T>
    {
        protected static T instance = null;

        public static T Instance()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (FindObjectsOfType<T>().Length > 1)
                {
                    QPrint.FrameworkError ("More than 1!");
                    return instance;
                }

                if (instance == null)
                {
                    string instanceName = typeof(T).Name;
                    QPrint.FrameworkLog ("Instance Name: " + instanceName); 
                    GameObject instanceGO = GameObject.Find(instanceName);

                    if (instanceGO == null)
                        instanceGO = new GameObject(instanceName);
                    instance = instanceGO.AddComponent<T>();
                    DontDestroyOnLoad(instanceGO);  //保证实例不会被释放
                    QPrint.FrameworkLog ("Add New Singleton " + instance.name + " in Game!");
                }
                else
                {
                    QPrint.FrameworkLog ("Already exist: " + instance.name);
                }
            }

            return instance;
        }


        protected virtual void OnDestroy()
        {
            instance = null;
        }
    }

}
```
#### 总结:
目前已经实现了两种单例的模板,一种是需要接收 Unity 的生命周期的,一种是不需要接收生命周期的,可以配合着使用。虽然不是本人实现的,但是用起来可是超级爽快,2333。

### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework &游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](https://ws2.sinaimg.cn/large/006tKfTcgy1fr1ywcobcwj30by0byt9i.jpg)

### 如果有帮助到您:

如果觉得本篇教程或者 QFramework 对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

- 给 QFramework 一个 Star:https://github.com/liangxiegame/QFramework
- 下载 Asset Store 上的 QFramework 给个五星(如果有评论小的真是感激不尽):http://u3d.as/SJ9
- 购买 gitchat 话题并给 5 星好评: http://gitbook.cn/gitchat/activity/5abc3f43bad4f418fb78ab77 (6 元，会员免费)
- 购买同名的蛮牛视频课程并给 5 星好评:http://edu.manew.com/course/431 (目前定价 29.8 元)
- 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design( 29.9 元，内容会在 2018 年 10 月份完结)

笔者在这里保证 QFramework、入门教程、文档和此框架搭建系列的专栏永远免费开源。以上捐助产品的内容对于使用 QFramework 的使用来讲都不是必须的，所以大家不用担心，各位使用 QFramework 或者 阅读此专栏 已经是对笔者团队最大的支持了。