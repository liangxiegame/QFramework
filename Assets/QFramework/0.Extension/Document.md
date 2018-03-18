### Extensions 模块简介:

the Extensions Module is wrapper for Unity and .Net's API

简单来说都是对 .Net 和  Unity 的 API 进行了一层封装

#### QuickStart:

``` csharp
// traditional style
var playerPrefab = Resources.Load<GameObject>("playerPrefab");
var playerObj = Instantiate(playerPrefab);
playerObj.transform.SetParent(null);
playerObj.transform.localRotation = Quaternion.identity;
playerObj.transform.localPosition = Vector3.left;
playerObj.transform.localScale = Vector3.one;
playerObj.layer = 1;
playerObj.layer = LayerMask.GetMask("Default");

Debug.Log("playerPrefab instantiated");


// Extension's Style,same as above 
Resources.Load<GameObject>("playerPrefab")
	.Instantiate()
	.transform
	.Parent(null)
	.LocalRotationIdentity()
	.LocalPosition(Vector3.left)
	.LocalScaleIdentity()
	.Layer(1)
	.Layer("Default")
	.ApplySelfTo(_ => { Debug.Log("playerPrefab instantiated"); });
```
