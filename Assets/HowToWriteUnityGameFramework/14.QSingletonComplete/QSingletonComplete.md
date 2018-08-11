# Unity 游戏框架搭建 (十四) 优雅的QSignleton(零) QuickStart

好久不见 ！之前想着让各位直接用 QFramework，但是后来想想，如果正在进行的项目直接使用 QFramework，这样风险太高了，要改的代码太多，所以打算陆续独立出来一些工具和模块,允许各位一个模块一个模块的进行更换，减少更换带来的风险。

#### QSingleton:

  之前有几篇文章介绍过单例模板在 Unity 中的几种实现。之后又参考了其他的单例库的实现，借鉴(chao)了它们的优点,借鉴了哪里有声明原作者。

#### 快速开始:

  实现一个继承MonoBehaviour的单例类

```csharp
namespace QFramework.Example
{
	[QMonoSingletonPath("[Audio]/AudioManager")]
	public class AudioManager : ManagerBase,ISingleton
	{
		public static AudioManager Instance
		{
			get { return QMonoSingletonProperty<AudioManager>.Instance; }
		}
		
		public void OnSingletonInit()
		{
			
		}

		public void Dispose()
		{
			QMonoSingletonProperty<AudioManager>.Dispose();
		}


		public void PlaySound(string soundName)
		{
			
		}

		public void StopSound(string soundName)
		{
			
		}
	}
}
```

  结果如下:

![][image-1]

这样从头到尾都很！优！雅！

## 相关链接:
[我的框架地址][1]:https://github.com/liangxiegame/QFramework

[教程源码][2]:https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework &游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记][3] http://liangxiegame.com/

微信公众号:liangxiegame

![][image-2]

## 如果有帮助到您:
如果觉得本篇教程对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

* 给 [QFramework][4] 一个 Star
	* 地址: https://github.com/liangxiegame/QFramework
* 给 Asset Store 上的 QFramework 并给个五星(需要先下载)
	* 地址: http://u3d.as/SJ9
* 购买 gitchat 话题:[《命名的力量：变量》][5]
	* 价格: 12 元
	* 地址: [https://gitbook.cn/gitchat/activity/5b65904096290075f5829388 ][6]
* 购买同名的蛮牛视频课程录播课程: 
	* 价格 49.2 元
	* 地址: [http://edu.manew.com/course/431][7]
* 购买同名电子书:[https://www.kancloud.cn/liangxiegame/unity_framework_design][8]
	* 价格  49.2 元，内容会在 2018 年 10 月份完结

[1]:	https://github.com/liangxiegame/QFramework
[2]:	https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/%0A
[3]:	http://liangxiegame.com/
[4]:	https://github.com/liangxiegame/QFramework
[5]:	https://gitbook.cn/gitchat/activity/5b65904096290075f5829388
[6]:	https://gitbook.cn/gitchat/activity/5b65904096290075f5829388 "https://gitbook.cn/gitchat/activity/5b65904096290075f5829388"
[7]:	http://edu.manew.com/course/431
[8]:	https://www.kancloud.cn/liangxiegame/unity_framework_design

[image-1]:	https://ws4.sinaimg.cn/large/006tKfTcgy1frqa0ikhd4j30fi09k74o.jpg
[image-2]:	https://ws4.sinaimg.cn/large/006tKfTcgy1fryc5skygwj30by0byt9i.jpg