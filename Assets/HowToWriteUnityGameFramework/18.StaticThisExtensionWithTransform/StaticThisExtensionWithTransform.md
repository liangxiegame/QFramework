# Unity 游戏框架搭建 (一) 概述

为了重构手头的一款项目,翻出来当时未接触Unity时候收藏的视频[《Unity项目架构设计与开发管理》](http://v.qq.com/boke/page/d/0/u/d016340mkcu.html),对于我这种初学者来说全是干货。简单的总结了一下,以后慢慢提炼。

关于Unity的架构有如下几种常用的方式。

#### 1.EmptyGO:

  在Hierarchy上创建一个空的GameObject,然后挂上所有与GameObject无关的逻辑控制的脚本。使用GameObject.Find()访问对象数据。

缺点:逻辑代码散落在各处,不适合大型项目。

#### 2.Simple GameManager:

  所有与GameObject无关的逻辑都放在一个单例中。
缺点:单一文件过于庞大。
#### 3.Manager Of Managers:

将不同的功能单独管理。如下:

* MainManager: 作为入口管理器。 
* EventManager: 消息管理。 
* GUIManager: 图形视图管理。 
* AudioManager: 音效管理。 
* PoolManager: GameObject管理（减少动态开辟内存消耗,减少GC)。

#### 实现一个简单的PoolManager<br>


``` csharp
// 存储动可服用的GameObject。
private List<GameObject> dormantObjects = new List<GameObject>();  
// 在dormantObjects获取与go类型相同的GameObject,如果没有则new一个。
public GameObject Spawn(GameObject go)  
{
     GameObject temp = null;
     if (dormantObjects.Count > 0)
     {
          foreach (GameObject dob in dormantObjects)
          {
               if (dob.name == go.name)
               {
                    // Find an available GameObject
                    temp = dob;
                    dormantObjects.Remove(temp);
                    return temp;
               }
          }
     }
     // Now Instantiate a new GameObject.
     temp = GameObject.Instantialte(go) as GameObject;
     temp.name = go.name;
     return temp;
}

// 将用完的GameObject放入dormantObjects中
public void Despawn(GameObject go)  
{
     go.transform.parent = PoolManager.transform;
     go.SetActive(false);
     dormantObject.Add(go);
     Trim();
}

//FIFO 如果dormantObjects大于最大个数则将之前的GameObject都推出来。
public void Trim()  
{
     while (dormantObjects.Count > Capacity)
     {
          GameObject dob = dormantObjects[0];
          dormantObjects.RemoveAt(0);
          Destroy(dob);
     }
}
```

##### 缺点:
* 不能管理prefabs。
* 没有进行分类。

更好的实现方式是将一个PoolManager分成:

* 若干个 SpawnPool。
  * 每个SpawnPool分成PrefabPool和PoolManager。
    * PrefabPool负责Prefab的加载和卸载。
    * PoolManager与之前的PoolMananger功能一样,负责GameObject的Spawn、Despawn和Trim。

##### 要注意的是:
* 每个SpawnPool是EmeptyGO。
* 每个PoolManager管理两个List (Active,Deactive)。

讲了一堆,最后告诉有一个NB的插件叫PoolManager- -。

* LevelManager: 关卡管理。
  推荐插件:MadLevelManager。
  GameManager: 游戏管理。
    [C#程序员整理的Unity 3D笔记（十二）：Unity3D之单体模式实现GameManager](http://www.tuicool.com/articles/u6NN7v)

* SaveManager: 配置管理。

* 实现Resume,功能玩到一半数据临时存储。
    推荐SaveManager插件。可以Load、Save均采用二进制(快!!!)
    所有C#类型都可以做Serialize。
    数据混淆,截屏操作。
    	MenuManager 菜单管理。

#### 4.将View和Model之间增加一个媒介层。

MVCS:StrangeIOC插件。

MVVM:uFrame插件。

#### 5. ECS(Entity Component Based  System)

Unity是基于ECS,比较适合GamePlay模块使用。
还有比较有名的[Entitas-CSharp](https://github.com/sschmid/Entitas-CSharp)

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
- 购买同名的蛮牛视频课程并给 5 星好评:http://edu.manew.com/course/431 (目前定价 19 元，之后会涨价,课程会在 2018 年 6 月初结课)
- 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design( 29.9 元，内容会在 2018 年 10 月份完结)

笔者在这里保证 QFramework、入门教程、文档和此框架搭建系列的专栏永远免费开源。以上捐助产品的内容对于使用 QFramework 的使用来讲都不是必须的，所以大家不用担心，各位使用 QFramework 或者 阅读此专栏 已经是对笔者团队最大的支持了。