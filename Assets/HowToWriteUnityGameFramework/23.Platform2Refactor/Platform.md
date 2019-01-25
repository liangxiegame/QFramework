# Unity 游戏框架搭建 (二十三)  重构小工具 Platform


在日常开发中，我们经常遇到或者写出这样的代码

```cs
var sTrAngeNamingVariable = "a variable";

#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
		sTrAngeNamingVariable = "a!value";
#else
		sTrAngeNamingVariable = "other value";
#endif
```
宏本身没有什么问题。但是 MonoDevelop IDE  上，只要写了宏判断，后边的代码的排版就会出问题。这是第一点。

第二点是，当我们发现 sTrAngeNamingVariable 的命名很不规范的时候，要对此变量进行重命名。一般的 IDE 都会支持变量/类/方法的重命名。

借助 IDE 的重命名功能，代码会变成如下:
```cs
var strangeVariableName = "a variable";

#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
	strangeVariableName = "a!value";
#else
	sTrAngeNamingVariable = "other value";
#endif
```

else 代码块里的变量重命名没有成功。当宏判断散落在各处时，很难发现这种错误。直到打包/AssetBundle/切换平台时，问题才会暴露(笔者也是被坑了很多次T.T)。

从这里得出的结论 : 当进行重构时，宏相关的代码会对重构造成风险，也不利于维护。在这里笔者设计出了 Platform。首先看下怎么使用:
```cs
var strangeVariableName = "a variable";

if (Platform.IsiOS || Platform.IsAndroid || Platform.IsEditor)
{
	sTrAngeNamingVariable = "a!value";
}
else
{
	sTrAngeNamingVariable = "other value";
}
```
代码很简单，就是把 宏 换成了 Platform 而已。
这时候我们再进行下重命名。重命名后代码如下:
```cs
if (Platform.IsiOS || Platform.IsAndroid || Platform.IsEditor)
{
	strangeNamingVariable = "a!value";
}
else
{
	strangeNamingVariable = "other value";
}
```
重命名问题解决了。
Platform 的代码很简单，贴出简单看下就可以了。
```cs
namespace QFramework
{
	public class Platform
	{
		public static bool IsAndroid
		{
			get
			{
				bool retValue = false;
#if UNITY_ANDROID
                retValue = true;    
#endif
				return retValue;
			}
		}
         
		public static bool IsEditor
		{
			get
			{
				bool retValue = false;
#if UNITY_EDITOR
				retValue = true;    
#endif
				return retValue;
			}
		}
        
		public static bool IsiOS
		{
			get
			{
				bool retValue = false;
#if UNITY_IOS
				retValue = true;    
#endif
				return retValue;
			}
		}
	}
}
```

只是简单的把宏封装了一下。

但是  Platform 不是万能的，有一些事项还是要注意一下。

* 在有 Platform 的条件语句块里，不能使用平台特有的  API ，如果要使用这种 API 还是建议封装一下，平台特有的 API 或者 宏 最好不要出现在 UI 或者 逻辑层代码中。
* Platform.cs 这个类不能打成 dll。
* 其他的大家来补充:)，目前上线了几个项目，都没什么问题。

当笔者在接手一个项目的时候，优先会把所有宏相关的判断，能换的全换成  Platform ，不能换的，比如使用了平台特有 API 的都会简单封装下，然后再进行一些小部分的重命名，以熟悉一些代码的逻辑。

有一些宏判断比较棘手，比如:
```cs
if ("1" == "2" || "2" == "3")
{
	// do sth
#if UNITY_EDITOR
}
else
{
	// do sth	
#else
	// do sth
#endif
}
```
如果遇到这种代码，
首先先揍一顿写这种代码的人吧。哈哈，开玩笑的。
体谅一下吧，也许是版本太急了，谁都不想写出这样的代码，都有苦衷，都不容易，最起码是没有 bug 的。

解决办法是有的，就是复制一遍代码。第一份代码删掉 # if 代码块的代码，把宏换成对应的  Platform API，第二份代码删掉  # else 代码块的代码，然后一样把宏换成对应的 Platform API。 剩下的就容易解决了。

17 年年底的时候看了 《重构》 这本书，这里还是推荐大家看一下吧，有点枯燥，但是收获很多。

Hard Code 是难免的，追求代码质量的道路是没有终点的，让代码产生价值才是我们游戏开发者要做的。

今天就到这里。

## 相关链接:
[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/%0A):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework &游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/) http://liangxiegame.com/

微信公众号:liangxiegame

![][https://s3.us-west-2.amazonaws.com/secure.notion-static.com/347c0821-ec03-4318-ac06-99e0536ca558/qrcode_for_gh_b4b5e104e2c0_344.jpg?AWSAccessKeyId=ASIAT73L2G45K5WJNQ6Z&Expires=1548463576&Signature=SEMV8zKjXwGhFDXCw%2BUu4H4hydE%3D&x-amz-security-token=FQoGZXIvYXdzEKj%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaDGRp6XONd1Z%2FowgXeCK3AySON6Pbdmpe5YKIoJBVQ1JraMcix%2FWWoZJID8UoKuQtiRPTcXHFqOEOv%2FycYCrWz3p2hSBN3Ew8NHRweWLZsJ%2FOHTKD%2BTd%2BejV2S37h7EGkKJQs5IX3Q9mAkHOFhVISiEj1EN5qxLYmP9eygzsZ%2BUxq46Qr0Rm1eEVEr6I4czAgWotEZsWa%2BBFaiy5D2%2F0ODnbqBcrDMdpcBKP6KtTwgu%2BvL2iDtCeoMivVllC3YeAgiSQCGRb%2FO2Aja1uUo4tEL0styO4VbA13tFeiCX1Nm6kpoYf%2Bfbic5hOalwUgum5iTIl%2BEsCwAHQO6foclvCrOyqYOie1X3IG9TA5EjkJZ9rgVWHZfeQL6ho3EAVgi91Xlc0Nmd88NVr1Y0Jl8AQJiakttQLdqRYLerOxn120k75VeX%2FtlSAAUDtzxTs6vgTGtqoX8ra7DxHpuNaJ33oN1VMoWJbggUruEhi6qnBixYzOxYbYtAGASeQvBC1F94%2BGPx79R2AR%2Fb29qDZ8oQLSQ2tQG3CyYffJtmck%2Fs43TGQgAukUOj0qrW5NC5m9ZGrZcAHKrcibAWdbctf%2B4YJC6LRq%2BkCGYNYo0f2o4gU%3D]

## 如果有帮助到您:
如果觉得本篇教程对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

* 给 [QFramework][4] 一个 Star
	* 地址: https://github.com/liangxiegame/QFramework
* 加入 siki A 计划，可以免费观看笔者的全部课程
	* 价格: 12 元
	* 地址: [http://www.sikiedu.com/user/52592]
