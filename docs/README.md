# QFramework 简介
  QFramework 是一套 **渐进式** 的 **快速** 开发框架。目标是作为无框架经验的公司、独立开发者、以及 Unity3D 初学者们的 **第一套框架**。框架内部积累了多个项目的在各个技术方向的解决方案。学习成本低，接入成本低，重构成本低，二次开发成本低，文档内容丰富(提供使用方式以及原理、开发文档)。

#### 特性
* UI
* Action
* ResKit
* Core


#### 快速开始
Unity API 扩展:
``` C#
	gameObject
	// 1. gameObject.SetActive(true)
	.Show()
	// 2. gameObject.SetActive(false)
	.Hide()
	// 3. gameObject.name = "Yeah" (这是UnityEngine.Object的API)
	.Name("Yeah")
	// 4. gameObject.layer = 10
	.Layer(0)
	// 5. gameObject.layer = LayerMask.NameToLayer("Default);
	.Layer("Default")
	// 6. Destroy(gameObject) (这是UnityEngine.Object的API)
	.DestroySelf();
```






**如在使用中遇到问题请提交 [这里](https://github.com/liangxiegame/QFramework/issues/new)，我们团队会在一天内快速回复并着手解决。**

本文档部分文字参考自:http://catlib.io
