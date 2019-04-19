>作者：布鞋     邮箱：827922094@qq.com

## RepeatNodeChain
class in Actionkit / Inherits from:[ActionChain](www.baidu.com) 

## Description
重复链式节点，用于驱动```重复节点(RepeatNode)```一次或多次执行```序列节点(SequenceNode)```。

## Construction

* public RepeatNodeChain(int repeatCount)

| 参数        | 描述                         |
| ----------- | ---------------------------- |
| repeatCount | 重复次数，默认为-1暨无限重复 |

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
注释：下面的整段序列在按下键盘S或者this的MonoBehaviour失效时会主动或被动停止重复，主动停止(暨按下S)时输出"结束"this.Repeat()

this.Repeat
    .Delay(0.5f)
    .Event(() => { Debug.Log("0.5s"); })
    .Begin()
    .DisposeWhen(() => Input.GetKeyDown(KeyCode.S))
    .OnDisposed(() => { Debug.Log("结束"); });
```
