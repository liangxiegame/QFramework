# Unity 游戏框架搭建 (十一) 简易AssetBundle打包工具(一)

最近在看Unity官方的 AssetBundle（以下简称 AB )的教程,也照着做了一遍,不过做出来的 AssetBundleManager 的 API 设计得有些不太习惯。目前想到了一个可行的解决方案。AB 相关的内容有点多,所以为了良好的阅读体验,就把教程分为几个小文章，一次写一个点。 

#### 1. AssetBundle设置:

首先要确定一个专门打资源包用的目录,我的框架定的目录是
QArt/QAB,并存放了一些Prefab资源,如下所示。
![](https://ws3.sinaimg.cn/large/006tKfTcgy1frotmiittpj30be07ajrm.jpg)

然后选定TestAB目录,将Inspector窗口的设置为如下图所示:

![](https://ws1.sinaimg.cn/large/006tKfTcgy1frotmjgzywj313b0dfabi.jpg)

一级名字为testab,二级扩展名为unity3d。
这样AB就设置好了。

#### 2. 制作编辑器工具。

这里打包的核心API只有一个,就是

```
BuildPipeline.BuildAssetBundles (outPath, 0, EditorUserBuildSettings.activeBuildTarget);
```

贴上编辑器工具代码:

```csharp
using UnityEditor;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace QFramework.Editor {

    public class QABEditor
    {
        [MenuItem("QFramework/AB/Build")]
        public static void BuildAssetBundle()
        {
            // AB包输出路径
            string outPath = Application.streamingAssetsPath + "/QAB";

            // 检查路径是否存在
            CheckDirAndCreate (outPath);

            BuildPipeline.BuildAssetBundles (outPath, 0, EditorUserBuildSettings.activeBuildTarget);

            // 刚创建的文件夹和目录能马上再Project视窗中出现
            AssetDatabase.Refresh ();
        }

        /// <summary>
        /// 判断路径是否存在,不存在则创建
        /// </summary>
        public static void CheckDirAndCreate(string dirPath)
        {
            if (!Directory.Exists (dirPath)) {
                Directory.CreateDirectory (dirPath);
            }
        }
     }
}
```

**这个脚本要放在Editor目录下!!!**

**这个脚本要放在Editor目录下!!!**

**这个脚本要放在Editor目录下!!!**

#### 使用方法:

点击QFramework/AB/Build

![](https://ws4.sinaimg.cn/large/006tKfTcgy1frotmw3h0aj307902m74f.jpg)

之后,生成的AB包如下所示:

![](https://ws2.sinaimg.cn/large/006tKfTcgy1frotmyvgktj30fg03ojrf.jpg)

AB包就打好了,接下来开始测试AB包的使用。

#### 3.测试:

代码很简单,如下所示,一些常识性的问题就不介绍了。

```csharp
using UnityEngine;
using System.Collections;
using System.IO;

namespace QFramework.Example {
    public class TestABEditor : MonoBehaviour {

        // Use this for initialization
        IEnumerator Start () {

            WWW www = new WWW ("file:///" + Application.streamingAssetsPath + Path.DirectorySeparatorChar + "QAssetBundle" + Path.DirectorySeparatorChar + "testab.unity3d");

            yield return www;

            if (string.IsNullOrEmpty (www.error)) {
                var go = www.assetBundle.LoadAsset<GameObject> ("Canvas");

                Instantiate (go);
            }
            else {
                Debug.LogError (www.error);
            }

        }

        // Update is called once per frame
        void Update () {

        }
    }
}
```

##### 运行结果:

![](https://ws3.sinaimg.cn/large/006tKfTcgy1frotn1tljkj30gi09u0sr.jpg)

最初版的打包工具就做好了,接下来到了吐槽的时刻了。

#### 4.存在的问题:

1. 不支持多平台,只能打当前PlayerSettings所设置的平台的AB包,有点麻烦。
2. 需要手动设置AB包的名字和扩展名,这个问题完全可以交给代码实现。
3. 看不到整个工程究竟有哪些设置了AB包的名字,哪些没设置,有些无关的资源误操作设置了AB包名,要排查这种资源要花些时间。
4. 测试的代码中暴露的字符串太多了,其中包括AB包名,AB包路径,要加载的资源名字,这些都可以集中管理或者生成代码。
5. 欢迎补充

这些问题在此后的文章中一步一步解决,希望大家多给些建议。

#### 欢迎讨论!

#### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework &游戏框架搭建 QQ 交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](https://ws4.sinaimg.cn/large/006tKfTcgy1frotn48eioj30by0byt9i.jpg)

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