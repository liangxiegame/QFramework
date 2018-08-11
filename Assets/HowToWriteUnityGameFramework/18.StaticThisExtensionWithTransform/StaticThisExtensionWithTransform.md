# Unity 游戏框架搭建 (十八) 静态扩展 + 泛型实现transform的链式编程

本篇文章介绍如何实现如下代码的链式编程:

```cs
			this.Position(Vector3.one)			
				.LocalScale(1.0f)				
				.Rotation(Quaternion.identity); 
```

以上代码中,this为 MonoBehaviour 类型的对象。

## 如何实现?

通过上篇文章介绍的return this + 静态扩展很容易做到,实现代码如下所示:
```cs
		public static MonoBehaviour Position(this MonoBehaviour selfBehaviour, Vector3 position) 
		{
			selfBehaviour.transform.position = position;
			return selfBehaviour;
		}
		
		public static MonoBehaviour LocalScale(this MonoBehaviour selfBehaviour, float xyz)
		{
			selfBehaviour.transform.localScale = Vector3.one * xyz;
			return selfBehaviour;
		}
		
		public static MonoBehaviour Rotation(this MonoBehaviour selfBehaviour, Quaternion rotation)
		{
			selfBehaviour.transform.rotation = rotation;
			return selfBehaviour;
		}
```

很容易实现对吧?但是这样有个问题，由于静态扩展方法返回的是MonoBehaviour类而不是this所属的类型，所以接下来链式方法中只能使用MonoBehaviour的方法。不能像如下方式使用。
```cs
			this.Position(Vector3.one)			
				.LocalScale(1.0f)				
				.Rotation(Quaternion.identity)
				.DoSomething(); 
```

以上代码中,this为MonoBehaviour类型的对象。

如何解决这个问题呢?答案是引入泛型。

## 引入泛型
实现代码如下所示:
```cs
		public static T Position<T>(this T selfBehaviour, Vector3 position) where T : MonoBehaviour
		{
			selfBehaviour.transform.position = position;
			return selfBehaviour;
		}
		
		public static T LocalScale<T>(this T selfBehaviour, float xyz) where T : MonoBehaviour
		{
			selfBehaviour.transform.localScale = Vector3.one * xyz;
			return selfBehaviour;
		}
		
		public static T Rotation<T>(this T selfBehaviour, Quaternion rotation) where T : MonoBehaviour
		{
			selfBehaviour.transform.rotation = rotation;
			return selfBehaviour;
		}
```
实现很简单，只是把之前代码中的 MonoBehaivour 改成泛型 T,然后增加一个约束: where T : MonoBehaviour。表示这个泛型一定要继承自 MonoBehaviour。这样之前例子中的this可以使用MonoBehaviour 之外的方法实现链式编程了。

## 进一步完善
不只是自己实现的 MonoBehaviour 脚本像 UGUI 的 Image 等都要支持 transform 的链式编程。那么就要找到 transfom 到底是哪个类？最后找到了如下代码。
```cs
namespace UnityEngine
{
  /// <summary>
  ///   <para>Base class for everything attached to GameObjects.</para>
  /// </summary>
  [RequiredByNativeCode]
  public class Component : Object
  {
    /// <summary>
    ///   <para>The Transform attached to this GameObject.</para>
    /// </summary>
    public extern Transform transform { [MethodImpl(MethodImplOptions.InternalCall)] get; }
...
```
最终定位到,transform 是 Component 的属性器。
所以可以将之前的实现改为如下代码:
```cs
		public static T Position<T>(this T selfComponent, Vector3 position) where T : Component
		{
			selfComponent.transform.position = position;
			return selfComponent;
		}
		
		public static T LocalScale<T>(this T selfComponent, float xyz) where T : Component
		{
			selfComponent.transform.localScale = Vector3.one * xyz;
			return selfComponent;
		}
		
		public static T Rotation<T>(this T selfComponent, Quaternion rotation) where T : Component
		{
			selfComponent.transform.rotation = rotation;
			return selfComponent;
		}
```
通过此种方式实现 Graphfic,Component 等类，最后可以实现如下方式的链式编程。
```cs
			Image image = null;

			image.LocalPosition(Vector3.back)
				.ColorAlpha(0.0f)
				.Hide();
```

当然，去查看一个属性到底是哪个类实现的这个过程也是一个很好的学习方式 : ) ，有很多类都可以实现链式编程，不过剩下的要靠大家自己了，当然也可以参考QFramework里的实现方式，不过QFramework也只是对笔者常用的类进行了实现。

OK,本篇介绍到这里。

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

[image-1]:	https://ws4.sinaimg.cn/large/006tKfTcgy1fryc5skygwj30by0byt9i.jpg