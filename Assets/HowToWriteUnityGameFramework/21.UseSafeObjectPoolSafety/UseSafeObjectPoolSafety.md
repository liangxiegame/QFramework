# Unity 游戏框架搭建 (二十一)  使用对象池时的一些细节

上篇文章使用SafeObjectPool实现了一个简单的Msg类。代码如下:

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

这个类虽然只是用来做SafeObjectExample的实例类的，但是还是有改进的空间。

在Msg的使用场景中，我们只用到了Msg类的Allocate和Recycle2Cache方法。而OnRecycled和IsRecycle这两个API用户都用不到,或者说用户用了会对Msg的管理造成风险，所以我们要想办法使用户不能访问到这两个API。

这个是可以做到的，就是把OnRecycled和IsRecycled使用接口的显示实现。

代码如下:
``` csharp
		class Msg : IPoolAble,IPoolType
		{
			#region IPoolAble 实现

			void IPoolAble.OnRecycled()
			{
				Log.I("OnRecycled");
			}
			
			bool IPoolAble.IsRecycled { get; set; }

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

这样，创建出来的 Msg对象不能直接访问OnRecycled和IsRecycled这两个API了，如果硬是要访问也可以，就要将Msg对象转成IPoolAble接口就可以访问了了。

* 最近在看《Framework Design Guidlines》，里边说IPoolAble这样的命名有问题，应该是IPoolable,嗯…,要去重构了...

关于接口的显示实现是C#的语法细节，随便贴上一篇给大家参考:
[C# 接口的隐式与显示实现 - Ben—Zhang - 博客园][1]:http://www.cnblogs.com/ben-zhang/archive/2012/12/18/2823455.html

OK,今天就到这里

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
* 购买 gitchat 话题[《命名的力量：变量》][6]
	* 价格: 12 元
	* 地址: http://gitbook.cn/gitchat/activity/5b29df073104f252297a779c
* 购买同名的蛮牛视频课程录播课程: 
	* 价格 49.2 元
	* 地址: http://edu.manew.com/course/431
* 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design
	* 价格  49.2 元，内容会在 2018 年 10 月份完结

[1]:	http://www.cnblogs.com/ben-zhang/archive/2012/12/18/2823455.html
[2]:	https://github.com/liangxiegame/QFramework
[3]:	https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/%0A
[4]:	http://liangxiegame.com/
[5]:	https://github.com/liangxiegame/QFramework
[6]:	%20http://gitbook.cn/gitchat/activity/5b29df073104f252297a779c

[image-1]:	https://ws4.sinaimg.cn/large/006tKfTcgy1fryc5skygwj30by0byt9i.jpg