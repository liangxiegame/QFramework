# Unity 游戏框架搭建 (十八) 静态扩展 + 泛型实现transform的链式编程

本篇文章介绍如何实现如下代码的链式编程:

``` csharp
			this.Position(Vector3.one)			
				.LocalScale(1.0f)				
				.Rotation(Quaternion.identity); 
```

以上代码中,this为MonoBehaviour类型的对象。

#### 如何实现?

通过上篇文章介绍的return this + 静态扩展很容易做到,实现代码如下所示:
``` csharp
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
``` csharp
			this.Position(Vector3.one)			
				.LocalScale(1.0f)				
				.Rotation(Quaternion.identity)
				.DoSomething(); 
```

以上代码中,this为MonoBehaviour类型的对象。

如何解决这个问题呢?答案是引入泛型。

#### 引入泛型
实现代码如下所示:
``` csharp
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
实现很简单，只是把之前代码中的MonoBehaivour改成泛型T,然后增加一个约束: where T : MonoBehaviour。表示这个泛型一定要继承自MonoBehaviour。这样之前例子中的this可以使用MonoBehaviour之外的方法实现链式编程了。

#### 进一步完善
不只是自己实现的MonoBehaviour脚本像UGUI的Image等都要支持transform的链式编程。那么就要找到transfom到底是哪个类？最后找到了如下代码。
``` csharp
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
最终定位到,transform是Component的属性器。
所以可以将之前的实现改为如下代码:
``` csharp
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
通过此种方式实现Graphfic,Component等类，最后可以实现如下方式的链式编程。
``` csharp
			Image image = null;

			image.LocalPosition(Vector3.back)
				.ColorAlpha(0.0f)
				.Hide();
```

当然，去查看一个属性到底是哪个类实现的这个过程也是一个很好的学习方式 : ) ，有很多类都可以实现链式编程，不过剩下的要靠大家自己了，当然也可以参考QFramework里的实现方式，不过QFramework也只是对笔者常用的类进行了实现。

OK,本篇介绍到这里。

#### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework&游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](https://ws3.sinaimg.cn/large/006tKfTcgy1frybwa41ogj30by0byt9i.jpg)

### 如果有帮助到您:

如果觉得本篇教程或者 QFramework 对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

- 给 QFramework 一个 Star:https://github.com/liangxiegame/QFramework
- 下载 Asset Store 上的 QFramework 给个五星(如果有评论小的真是感激不尽):http://u3d.as/SJ9
- 购买 gitchat 话题并给 5 星好评: http://gitbook.cn/gitchat/activity/5abc3f43bad4f418fb78ab77 (6 元，会员免费)
- 购买同名的蛮牛视频课程并给 5 星好评:http://edu.manew.com/course/431 (目前定价 29.8 元，之后会涨价)
- 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design( 29.9 元，内容会在 2018 年 10 月份完结)

笔者在这里保证 QFramework、入门教程、文档和此框架搭建系列的专栏永远免费开源。以上捐助产品的内容对于使用 QFramework 的使用来讲都不是必须的，所以大家不用担心，各位使用 QFramework 或者 阅读此专栏 已经是对笔者团队最大的支持了。