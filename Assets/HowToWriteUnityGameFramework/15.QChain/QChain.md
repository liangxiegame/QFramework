# Unity 游戏框架搭建 (十五) 优雅的 ActionKit(QChain)

加班加了三个月终于喘了口气，博客很久没有更新了，这段期间框架加了很多Feature，大部分不太稳定，这些Feature中实现起来比较简单而且用的比较稳定的就是链式编程支持了。

#### 什么是链式编程?

我想大家应该都接触过DOTween，用起来是这样的。
``` csharp
	transform.DOMove(Vector3.one, 0.5f)
				.SetEase(Ease.InBack)
				.OnKill(() => Debug.Log("on killed"))
				.OnComplete(() => Debug.Log("on completed"));
```
像以上.XXX().YYY().ZZZ()这种写法就叫链式编程了。

#### QChain是什么?

QFramework中有零零散散支持了链式写法，打算整理出来作为一个独立的库进行过维护。目前的使用方式如下:
``` csharp
			this.Show()
				.LocalIdentity() // 归一化
				.LocalPosition(Vector3.back)
				.LocalPositionX(1.0f)
				.Sequence() // 开始序列
				.Delay(1.0f)
				.Event(() => Log.I("frame event"))
				.Until(() => count == 2)
				.Event(() => Log.I("count is 2"))
				.Begin() // 执行序列
				.DisposeWhen(() => count == 3)
				.OnDisposed(() => Log.I("On Disposed"));

			this.Repeat()
				.Delay(1.0f)
				.Event(() => count++)
				.Begin()
				.DisposeWhenGameObjDestroyed();

			this.Repeat(5)
				.Event(() => Log.I(" Hello workd"))
				.Begin()
				.DisposeWhenFinished(); // 默认是这个
			
			this.Sequence()
				.Delay(1.0f)
				.Event(() => Log.I("delay one second"))
				.Delay(1.0f)
				.Event(() => Log.E("delay two second"))
				.Begin();
```

#### 为什么要用QChain

前段时间在给公司写一个蓝牙的插件,比较麻烦的是蓝牙管理类的状态同步和当状态改变时通知其他对象的问题。但是有了QChain，蓝牙连接的代码可以这样写:
``` csharp
			this.Sequence()
				.Event(() => PTBluetooth.Initialize(true, false))
				.Until(() => PTBluetooth.IsInitialized)
				.Until(() => PTBluetooth.IsOpened)
				.Event(() => PTBluetooth.ScanPeripheral((address, name, rssi, adInfo) => name.Contains("device")))
				.Until(() => PTBluetooth.ScannedDevices.Count >= 1)
				.Event(() => PTBluetooth.ConnectToPeripheral(PTBluetooth.ScannedDevices[0].Address))
				.Begin()
				.DisposeWhen(()=>
				{
					if (PTBluetooth.IsInitialized && !PTBluetooth.IsOpened)
					{
						// TODO: 这里处理初始化失败逻辑
						return true;
					}
					
					// ... 其他失败逻辑处理
					return false;
				});
```
这样写的好处是，逻辑不会分散到处都是。相比于协程,生命周期更好进行管理(不用管理协程对象)，可作为协程的替代方案。还有其他的好处随着本系列的更新逐个讨论。

#### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework&游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](https://ws2.sinaimg.cn/large/006tKfTcgy1frqsk953swj30by0byt9i.jpg)

### 如果有帮助到您:

如果觉得本篇教程对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

- 购买 gitchat 话题《Unity 游戏框架搭建：资源管理 与 ResKit 精讲》
  - 价格: 6 元，会员免费
  - 地址:  http://gitbook.cn/gitchat/activity/5b29df073104f252297a779c
- 给 QFramework 一个 Star
  - 地址: https://github.com/liangxiegame/QFramework
- 给 Asset Store 上的 QFramework 并给个五星(需要先下载)
  - 地址: http://u3d.as/SJ9
- 购买同名的蛮牛视频课程录播课程:
  - 价格 ~~19.2 元~~ 29.8 元
  - 地址: http://edu.manew.com/course/431 
- 购买 gitchat 话题《Unity 游戏框架搭建：我所理解的框架》
  - 价格: 6 元，会员免费
  - 地址:  http://gitbook.cn/gitchat/activity/5abc3f43bad4f418fb78ab77
- 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design( 29.9 元，内容会在 2018 年 10 月份完结)