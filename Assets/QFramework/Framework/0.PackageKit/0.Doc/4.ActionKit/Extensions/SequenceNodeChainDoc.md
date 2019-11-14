>作者：布鞋     邮箱：827922094@qq.com

## SequenceNodeChain
class in Actionkit / Inherits from:[ActionChain](www.baidu.com) 

## Description
链式序列节点，用于驱动序列节点执行。

## Construction
* public SequenceNodeChain()	默认构造函数，会对自身存储的序列节点初始化。

## Inherits Methods

*  public IActionChain Append(IAction node)  加入节点方法

| 参数 | 描述               |
| ---- | ------------------ |
| node | 需要加入链中的节点 |
* Begin 开始执行链，由Mono的协程以Time.deltaTime的时间执行
* public IDisposeEventRegister DisposeWhen(Func<bool> condition) bool值Func用于达成条件时销毁节点

| 参数      | 描述     |
| --------- | -------- |
| condition | 条件委托 |

* IDisposeEventRegister IDisposeEventRegister.OnFinished(Action onFinishedEvent) 设置执行结束回调

| 参数            | 描述         |
| --------------- | ------------ |
| onFinishedEvent | 执行结束回调 |

public void OnDisposed(System.Action onDisposedEvent)  设置销毁时回调

| 参数            | 描述       |
| --------------- | ---------- |
| onFinishedEvent | 销毁时回调 |

```
注释：下面的一段链式序列节点中添加了延迟节点、事件节点，将执行一次。在Begin之前我们还可以给他添加条件节点、可控事件节点、时间轴节点等
	this.Sequence()
		.Delay(1.0f)
		.Event(() => Log.I("Sequece:1.0s"))
		.Begin()
		.OnDisposed(() => Log.I("Sequece: dispose when sequence ended"));
```
