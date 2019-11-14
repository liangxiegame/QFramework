>作者：布鞋     邮箱：827922094@qq.com

## ActionChain
class in Actionkit / Inherits from:[NodeAction](www.baidu.com) / Implemented from:[IActionChain](www.baidu.com),[IDisposeWhen](www.baidu.com)

## Description
链式节点基类，用于任意组合节点达到异步的链式执行效果。

## Properties
* Executer        	          当前执行链的```Mono```对象                       

## Public Methods
*  public abstract IActionChain Append(IAction node) 抽象加入节点方法

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
