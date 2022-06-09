# CodeGenKit

## 1.基本使用

先在场景中，随便创建一些有父子结构的 GameObject，如下所示：
![image.png](http://file.liangxiegame.com/e9830e5c-94af-410d-ada5-cb7f5be69a2d.png)

给 GameRoot 挂上 ViewController，快捷键 （Alt + V），如下图所示：
![image.png](http://file.liangxiegame.com/c0f22baf-b1df-4dc5-bb4e-3791c7d4dfe0.png)

二是填写 刚刚添加的组件信息:
![image.png](http://file.liangxiegame.com/3f3e817c-4cd4-4a2a-9c51-d5f1f5e17279.png)

填写 生成的脚本名 和 要在哪个目录生成脚本？

之后，为 Player GameObject 挂上 Bind 组件（快捷键，alt + b)，如下图所示：
![Screenshot 2019-08-21 23.43.42.png](http://file.liangxiegame.com/376194ee-2e9e-4a2c-a64e-98f1ec4daa45.png)

Player 挂上的组件如下所示:
![image.png](http://file.liangxiegame.com/0954bc64-018a-4da7-99fd-e5bc5ac9bb51.png)


接下来点击图中的 生成代码按钮 或者是 GameRoot 的 ViewController 的 生成代码按钮。

点击之后等待编译，结果如下:

脚本目录:
![image.png](http://file.liangxiegame.com/6f8d8f31-8dbe-4a97-b4f1-d51d3ed04fc7.png)

GameRoot 脚本：
![image.png](http://file.liangxiegame.com/56658074-f4dd-42fc-9cd6-628c7338c137.png)

GameRoot 自动得到了 Player 的引用。

在 GameRoot.cs 中可以直接访问到 Player，如下图所示:
![image.png](http://file.liangxiegame.com/669bab80-d2cb-40cc-95d0-394984754d37.png)


## 2.嵌套


让 ViewController 成为另一个 ViewController 的子节点非常简单，只需要在子节点挂上 ViewController 的同时，再挂上一个 Bind。

如下图所示：
![image.png](http://file.liangxiegame.com/a139a109-44fc-407f-a29b-d2cf8b2e7029.png)

然后，先生成子节点的 ViewController 代码，再生成 父节点的 ViewController 代码即可。

结果如下所示:
![image.png](http://file.liangxiegame.com/27a53981-3257-4453-8389-a32c791b13cc.png)

这样，嵌套绑定就实现了。

当前的组件结构如下:
* GameRoot(ViewController)
    * Player1(ViewController)
        * PlayerAnimation(Bind)

嵌套绑定的实现非常简单，不过这里要提醒大家，在一般情况下，笔者很少在项目中使用嵌套绑定，一般只使用简单的 ViewController 和 Bind 就可以满足大部分需求了。

## 3.生成 Prefab

我们先看下 ViewController 的面板，如下图所示：

![image.png](http://file.liangxiegame.com/2fc2cd1a-d58a-400c-b051-8bd5072d6fcc.png)

Player 1 是，已经生成过脚本的 ViewController。

要想生成 Prefab，很简单，只需要把 生成 Prefab 勾上，并且把要生成的目录填好即可。

然后点击，生成代码，就能看到 Prefab 在目录中生成成功。

如下图所示:

![image.png](http://file.liangxiegame.com/8de6a2ee-9208-46e5-9cdb-25a9615b9acd.png)

OK，生成好了。

## 4. 命名空间怎么设置？

ctrl + e 可以看到如下面板：

![image-20220609165048935](https://file.liangxiegame.com/7339eb1e-5ea8-453e-903e-aab4b7e34b45.png)

直接填写 ViewController 命名空间即可