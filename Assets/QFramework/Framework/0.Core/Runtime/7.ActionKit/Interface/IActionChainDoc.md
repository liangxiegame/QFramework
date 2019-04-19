>作者：布鞋     邮箱：827922094@qq.com

## IActionChain

interface in ActionKit / Implemented in : IAction

## Properties

* Executer  当前驱动链式的MonoBehaviour

## Description

链式节点的通用接口

## Public Methods

- IActionChain Append(IAction node);

| 参数 | 描述         |
| ---- | ------------ |
| node | 任意类型节点 |

* IDisposeWhen Begin()   链式节点的入口
