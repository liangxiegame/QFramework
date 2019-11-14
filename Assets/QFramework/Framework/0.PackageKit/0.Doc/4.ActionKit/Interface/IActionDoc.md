>作者：布鞋     邮箱：827922094@qq.com

## IAction
interface in Actionkit / Implemented from:[IDisposable](www.baidu.com)

## Description
执行节点的基础接口

## Properties
* Disposed		节点是否销毁
* Finished		节点是否完成

## Public Methods
*  bool Execute(float delta)    执行节点

| 参数  | 描述           |
| ----- | -------------- |
| delta | 执行一次的时间 |

* void Reset()  重置节点
* void Finish()  结束节点
