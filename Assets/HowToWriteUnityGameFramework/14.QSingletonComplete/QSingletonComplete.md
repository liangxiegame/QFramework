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

![](https://ws4.sinaimg.cn/large/006tKfTcgy1frqa0ikhd4j30fi09k74o.jpg)

这样从头到尾都很！优！雅！

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