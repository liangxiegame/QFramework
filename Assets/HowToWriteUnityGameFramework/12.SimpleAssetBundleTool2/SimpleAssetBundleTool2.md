# Unity 游戏框架搭建 (十二) 简易AssetBundle打包工具(二)

上篇文章中实现了基本的打包功能,在这篇我们来解决不同平台打AB包的问题。

本篇文章的核心api还是:
``` csharp
BuildPipeline.BuildAssetBundles (outPath, 0, EditorUserBuildSettings.activeBuildTarget);  
```

在第三个参数中,只要传入不同平台 BuildTarget就可以了。目前只考虑Android和iOS平台。

#### 区分iOS、Android平台

很简单,只要在上篇文章的QABEditor类中将原来的BuildAssetBundle方法分为BuildAssetBundleiOS和BuildAssetBundleAndroid即可。代码如下所示。
``` csharp
public class QABEditor
	{
		[MenuItem("QFramework/AB/Build iOS")]
		public static void BuildABiOS()
		{
			string outputPath = QPath.ABBuildOutPutDir (RuntimePlatform.IPhonePlayer);

			QIO.CreateDirIfNotExists (outputPath);

			QABBuilder.BuildAssetBundles (BuildTarget.iOS);

			AssetDatabase.Refresh ();
		}

		[MenuItem("QFramework/AB/Build Android")]
		public static void BuildABAndroid()
		{
			string outputPath = QPath.ABBuildOutPutDir (RuntimePlatform.Android);
				
			QIO.CreateDirIfNotExists (outputPath);

			QABBuilder.BuildAssetBundles (BuildTarget.Android);

			AssetDatabase.Refresh ();

		}
}
```
大家觉得代码中有几个类有些陌生。下面我来一一介绍下。

#### QPath.ABBuildOutPutDir(build target)

QPath这个类在我的框架中是用来指定固定的路径用的,因为路径的代码全是字符串,不能让字符串暴露在各处都是,这样会影响代码的可读性。统一管理起来比较方便修改。ABBuildOutPutDir这个API的实现如下所示,就不多说了。
``` csharp
	/// <summary>
	/// 所有的路径常量都在这里
	/// </summary>
	public class QPath 
	{
		/// <summary>
		/// 资源输出的路径
		/// </summary>
		public static string ABBuildOutPutDir(RuntimePlatform platform) {
			string retDirPath = null;
			switch (platform) {
			case RuntimePlatform.Android:
				retDirPath = Application.streamingAssetsPath + "/QAB/Android";
				break;
			case RuntimePlatform.IPhonePlayer:
				retDirPath = Application.streamingAssetsPath + "/QAB/iOS";
				break;
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
				retDirPath = Application.streamingAssetsPath + "/QAB/Windows";
				break;
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.OSXEditor:
				retDirPath = Application.streamingAssetsPath + "/QAB/OSX";
				break;
			}

			return retDirPath;
		}

		/// <summary>
		/// 打包之前的源资源文件
		/// </summary>
		public static string SrcABDir  {
			get {
				return Application.dataPath + "/QArt/QAB";
			}
		}
    }
}
```
#### QIO.CreateDirIfNotExists (outputPath)

QIO这个类是用来封装C#的System.IO和一些文件操作相关的API。CreateDirIfNotExists这个命名非常的傻瓜,会点英文就应该可以理解了。下面贴出实现代码,
``` csharp
using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// 各种文件的读写复制操作,主要是对System.IO的一些封装
/// </summary>
namespace QFramework {
	
	public class QIO {

		/// <summary>
		/// 创建新的文件夹,如果存在则不创建
		/// </summary>
		public static void CreateDirIfNotExists(string dirFullPath)
		{
			if (!Directory.Exists (dirFullPath)) {
				Directory.CreateDirectory (dirFullPath);
			}
		}
	}
}
```

#### QABBuilder
QABBuilder只是封装了本文的核心API
``` csharp
BuildPipeline.BuildAssetBundles (outPath, 0, EditorUserBuildSettings.activeBuildTarget);  
```
封装的原因是打AB包成功后,要对AB包进行一些处理,比如计算包尺寸,计算哈希或者md5值。主要是为了以后的热更新做准备的。看下QABBuilder核心实现.

``` csharp
	public class QABBuilder
	{
		public static string overloadedDevelopmentServerURL = "";


		public static void BuildAssetBundles(BuildTarget buildTarget)
		{
			string outputPath = Path.Combine(QPlatform.ABundlesOutputPath,  QPlatform.GetPlatformName());

			if (Directory.Exists (outputPath)) {
				Directory.Delete (outputPath,true);
			}
			Directory.CreateDirectory (outputPath);

			BuildPipeline.BuildAssetBundles(outputPath,BuildAssetBundleOptions.None,buildTarget);

			GenerateVersionConfig (outputPath);
			if(Directory.Exists(Application.streamingAssetsPath+"/QAB")){
				Directory.Delete (Application.streamingAssetsPath+"/QAB",true);
			}
			Directory.CreateDirectory (Application.streamingAssetsPath+"/QAB");
			FileUtil.ReplaceDirectory (QPlatform.ABundlesOutputPath,Application.streamingAssetsPath+"/QAB");
			AssetDatabase.Refresh ();
		}
    }
}
```

#### 使用方式

按这里

![](https://ws1.sinaimg.cn/large/006tKfTcgy1frotpdnmeqj30io07egmz.jpg)

结果看这里(创建了iOS文件夹)

![](https://ws1.sinaimg.cn/large/006tNc79gy1fqisnf9h8wj30e00fmglj.jpg)

介绍完毕,睡觉了!

#### 欢迎讨论!

附:[我的框架地址](https://github.com/liangxiegame/QFramework)

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)

微信公众号:liangxiegame

![](https://ws3.sinaimg.cn/large/006tKfTcgy1frotphkbcwj30by0byt9i.jpg)

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