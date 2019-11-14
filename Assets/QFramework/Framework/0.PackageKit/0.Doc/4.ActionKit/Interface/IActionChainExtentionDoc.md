>作者：布鞋     邮箱：827922094@qq.com

## IActionChainExtention

class in ActionKit

## Description

链式节点的MonoBehaviour扩展，使继承MonoBehaviour的类型能通过```类型扩展```的方式快速执行各类链式节点。

## Static Methods

* public static IActionChain Repeat<T>(this T selfbehaviour, int count = -1) where T : MonoBehaviour      重复链式节点扩展

| 参数  | 描述     |
| ----- | -------- |
| count | 延迟时间 |

* public static IActionChain Sequence<T>(this T selfbehaviour) where T : MonoBehaviour      序列链式节点扩展
*  public static IActionChain OnlyBegin(this IActionChain selfChain, Action<OnlyBeginAction> onBegin)   可控事件节点扩展

| 参数    | 描述         |
| ------- | ------------ |
| onBegin | 接收节点委托 |
*  public static IActionChain Delay(this IActionChain senfChain, float seconds) 延迟节点扩展
*  public static IActionChain Wait(this IActionChain senfChain, float seconds)  延迟节点扩展

| 参数    | 描述     |
| ------- | -------- |
| seconds | 延迟时间 |

* public static IActionChain Event(this IActionChain selfChain,params System.Action[] onEvents)  事件节点扩展

| 参数     | 描述               |
| -------- | ------------------ |
| onEvents | 多个需要回调的事件 |

* public static IActionChain Until(this IActionChain selfChain, Func<bool> condition)   条件节点扩展

| 参数      | 描述           |
| --------- | -------------- |
| condition | 返回条件的委托 |

``` 
注释:下面是链式序列节点的扩展使用方式，得益于c#方法扩展可以快速的加入各类型节点
this.Sequence()
    .Delay(1.0f)
    .Event(() => Log.I("Sequece:1.0s"))
    .Until(()=> Input.GetKeyDown(KeyCode.O))
    .Begin()
    .OnDisposed(() => Log.I("Sequece: dispose when sequence ended"));
```

