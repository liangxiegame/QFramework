# ResKit 模块简介:

## 加载 Resources 目录下的资源

``` csharp
// allocate a loader when initialize a panel or a monobehavour
var loader = ResLoader.Allocate<ResLoader>();

// load someth in a panel or a monobehaviour
loader.LoadSync<GameObject>("Resources/smobj")

loader.LoadSync<Texture2D>("Resources/Bg")

// resycle this panel/monobehaivour loaded res when destroyed 
loader.Recycle2Cache()
loader = null
```

## 加载网络资源

## 加载 AssetBundle 资源 

#### 准备

* 鼠标右键某个资源 Assets/ResKit/Mark AssetBundle
* Command/Ctrl + Shift + R 弹出资源面板，点击 build

#### 加载代码 
``` csharp
// init res mgr before load asset bundle
ResMgr.Init();
// allocate a loader when initialize a panel or a monobehavour
var loader = ResLoader.Allocate<ResLoader>();

// load someth in a panel or a monobehaviour
loader.LoadSync<GameObject>("smobj")

loader.LoadSync<Texture2D>("Bg")

// resycle this panel/monobehaivour loaded res when destroyed 
loader.Recycle2Cache()
loader = null
```
