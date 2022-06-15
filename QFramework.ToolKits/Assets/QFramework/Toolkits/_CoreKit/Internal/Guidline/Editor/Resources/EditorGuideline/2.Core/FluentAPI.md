# FluentAPI

## FluentAPI 简介
FluentAPI 是 笔者积累的 Unity API 的一些链式封装。

基本使用非常简单，如下：
``` csharp
// traditional style
var playerPrefab = Resources.Load<GameObject>("no prefab don't run");
var playerObj = Instantiate(playerPrefab);

playerObj.transform.SetParent(null);
playerObj.transform.localRotation = Quaternion.identity;
playerObj.transform.localPosition = Vector3.left;
playerObj.transform.localScale = Vector3.one;
playerObj.layer = 1;
playerObj.layer = LayerMask.GetMask("Default");

Debug.Log("playerPrefab instantiated");

// Extension's Style,same as above 
Resources.Load<GameObject>("playerPrefab")
    .Instantiate()
    .transform
    .Parent(null)
    .LocalRotationIdentity()
    .LocalPosition(Vector3.left)
    .LocalScaleIdentity()
    .Layer(1)
    .Layer("Default")
    .ApplySelfTo(_ => { Debug.Log("playerPrefab instantiated"); });
```

代码很简单。

这就是基本的使用了。

CSharpExtension 就是为了让 QF 在每一细节上节省时间。

其每个 API 的实现也非常简单，只需要看 CSharpExtension.cs 这个源码即可，它不依赖其他的模块，所以仅仅只想用 CSharpExtension 那么只要把文件放到自己的项目中即可。

CSharpExtension.cs 所在目录如下：

![image.png](http://file.liangxiegame.com/70cefa38-8c7c-4c9e-96f0-6db604bbd9df.png)

独立的插件地址如下:
* [QFramework.CSharpExtension](https://liangxiegame.com/qf/package/detail/0a9407ce-d395-43c6-b2b0-abfec3e50824)


## 相关文章
* [框架搭建 2017：17\. 静态扩展GameObject 实现链式编程](https://liangxiegame.com/zhuanlan/content/detail/81c0e32f-1028-4455-aad1-a82d15704027)
* [框架搭建 2017：18\. 静态扩展 + 泛型实现 transform 的链式编程](https://liangxiegame.com/zhuanlan/content/detail/032de6c3-9b7d-4bb8-9fa7-06344afe44dd)

OK，此篇内容就这些。
