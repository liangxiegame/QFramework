### Extensions 模块简介:

the Extensions Module is wrapper for Unity and .Net's API

简单来说都是对 .Net 和  Unity 的 API 进行了一层封装



#### QuickStart:

``` csharp
gameObject
	// 1. gameObject.SetActive(true)
	.Show()
	// 2. gameObject.SetActive(false)
	.Hide()
	// 3. gameObject.name = "Yeah" (this is UnityEngine.Object's API)
	.Name("Yeah")
	// 4. gameObject.layer = 10
	.Layer(0)
	// 5. gameObject.layer = LayerMask.NameToLayer("Default);
	.Layer("Default")
	// 6. Destroy(gameObject) (this is UnityEngine.Object's API)
	.DestroySelf();
			
this
	// 1. this.gameObject.Show()
	.Show()
	// 2. this.gameObject.Hide()
	.Hide()
	// 3. this.gameObject.Name("Yeah")
	.Name("Yeah")
	// 4. gameObject.layer = 10
	.Layer(0)
	// 5. gameObject.layer = LayerMask.NameToLayer("Default);
	.Layer("Default")
	// 6. Destroy(this.gameObject)
	.DestroyGameObj();
```

