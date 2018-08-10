# Unity 游戏框架搭建 (二十二)  简易引用计数器

引用计数是一个很好用的技术概念，不要被这个名字吓到了。首先来讲讲引用计数是干嘛的。

#### 引用计数使用场景
有一间黑色的屋子，里边有一盏灯。当第一个人进屋的时候灯会打开，之后的人进来则不用再次打开了，因为已经开过了。当屋子里的所有人离开的时候，灯则会关闭。

我们先定义灯的对象模型:
``` csharp
	class Light
	{
		public void Open()
		{
			Log.I("灯打开了");
		}

		public void Close()
		{
			Log.I("灯关闭了");
		}
	}
```
很简单就是两个方法而已。

再定义屋子的类,屋子应该持有一个Light的对象，并且要记录人们的进出。当有人进入，进入后当前房间只有一个人的时候，要把灯打开。当最后一个人离开的时候灯要关闭。

代码如下:
``` C#
	class Room
	{
		private Light mLight = new Light();

		private int mPeopleCount = 0;
		
		public void EnterPeople()
		{
			if (mPeopleCount == 0)
			{
				mLight.Open();
			}

			++mPeopleCount;
			
			Log.I("一个人走进房间,房间里当前有{0}个人",mPeopleCount);
		}

		public void LeavePeople()
		{
			--mPeopleCount;
			
			if (mPeopleCount == 0)
			{
				mLight.Close();
			}

			Log.I("一个人走出房间,房间里当前有{0}个人", mPeopleCount);
		}
	}
```
很简单，我们来看下测试代码。

``` C#
			var room = new Room();
			room.EnterPeople();
			room.EnterPeople();
			room.EnterPeople();
			
			room.LeavePeople();
			room.LeavePeople();
			room.LeavePeople();
			
			room.EnterPeople();
```

看下输出的结果:
``` csharp
灯打开了
一个人走进房间,房间里当前有1个人
一个人走进房间,房间里当前有2个人
一个人走进房间,房间里当前有3个人
一个人走出房间,房间里当前有2个人
一个人走出房间,房间里当前有1个人
一个人走出房间,房间里当前有0个人
灯关闭了
灯打开了
一个人走进房间,房间里当前有1个人
```

OK.以上就是引用计数这项计数的使用场景的所有代码。
测试的代码比较整齐，很容易算出来当前有多少个人在屋子里，所以看不出来引用计数的作用。但是在日常开发中，我们可能会把EnterPeople和LeavePeople散落在工程的各个位置。这样就很难统计，这时候引用计数的作用就很明显了，它可以自动帮助你判断什么时候进行开灯关灯操作，而你不用写开关灯的一行代码。

这个例子比较接近生活，假如笔者再换个例子，我们把Light对象换成资源对象，其开灯对应加载资源，关灯对应卸载资源。而屋子则是对应资源管理器,EnterPeople对应申请资源对象，LeavePeople对应归还资源对象。这样你只管在各个界面中申请各个资源，只要在界面关闭的时候归还各个资源对象就可以不用关心资源的加载和卸载了，可以减轻大脑的负荷。

#### 简易计数器实现:

计数器的接口很简单,代码如下:
``` C#
    public interface IRefCounter
    {
        int RefCount { get; }

        void Retain(object refOwner = null);
        void Release(object refOwner = null);
    }
```
Retain是增加引用计数(RefCount+1),Release减去一个引用计数(RefCount—)。
在接下来的具体实现中，当RefCount降为0时我们需要捕获一个事件,这个事件叫OnZeroRef。
代码如下:

``` C#
    public class SimpleRC : IRefCounter
    {
        public SimpleRC()
        {
            RefCount = 0;
        }

        public int RefCount { get; private set; }

        public void Retain(object refOwner = null)
        {
            ++RefCount;
        }

        public void Release(object refOwner = null)
        {
            --RefCount;
            if (RefCount == 0)
            {
                OnZeroRef();
            }
        }

        protected virtual void OnZeroRef()
        {
        }
    }
```

以上就是简易引用计数器的所有实现了。

接下来我们用这个引用计数器，重构灯的使用场景的代码。
``` C#
	class Light
	{
		public void Open()
		{
			Log.I("灯打开了");
		}

		public void Close()
		{
			Log.I("灯关闭了");
		}
	}

	class Room : SimpleRC
	{
		private Light mLight = new Light();
		
		public void EnterPeople()
		{
			if (RefCount == 0)
			{
				mLight.Open();
			}

			Retain();
			
			Log.I("一个人走进房间,房间里当前有{0}个人",RefCount);
		}

		public void LeavePeople()
		{
			// 当前还没走出，所以输出的时候先减1
			Log.I("一个人走出房间,房间里当前有{0}个人", RefCount - 1);

			// 这里才真正的走出了
			Release();
		}

		protected override void OnZeroRef()
		{
			mLight.Close();
		}
	}
```

测试代码和之前的一样，我们看下测试结果:
``` C#
灯打开了
一个人走进房间,房间里当前有1个人
一个人走进房间,房间里当前有2个人
一个人走进房间,房间里当前有3个人
一个人走出房间,房间里当前有2个人
一个人走出房间,房间里当前有1个人
一个人走出房间,房间里当前有0个人
灯关闭了
灯打开了
一个人走进房间,房间里当前有1个人
```

好了，今天就到这里

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
* 购买 gitchat 话题[《命名的力量：变量》][5]
	* 价格: 12 元
	* 地址: http://gitbook.cn/gitchat/activity/5b29df073104f252297a779c
* 购买同名的蛮牛视频课程录播课程: 
	* 价格 49.2 元
	* 地址: http://edu.manew.com/course/431
* 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design
	* 价格  49.2 元，内容会在 2018 年 10 月份完结

[1]:	https://github.com/liangxiegame/QFramework
[2]:	https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/%0A
[3]:	http://liangxiegame.com/
[4]:	https://github.com/liangxiegame/QFramework
[5]:	%20http://gitbook.cn/gitchat/activity/5b29df073104f252297a779c

[image-1]:	https://ws4.sinaimg.cn/large/006tKfTcgy1fryc5skygwj30by0byt9i.jpg