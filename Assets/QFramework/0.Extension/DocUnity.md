### Unity Extensions :

#### 1.Enable,Disable

``` csharp
var component = gameObject.GetComponent<MonoBehaviour>();

component.Enable(); // component.enabled = true
component.Disable(); // component.enabled = false
```

#### 2.CaptrueCamera:

``` csharp
var screenshotTexture2D = Camera.main.CaptureCamera(new Rect(0, 0, Screen.width, Screen.height));
```

#### 3.Color:

``` csharp
var color = "#C5563CFF".HtmlStringToColor();
```

#### 4.GameObject:

``` csharp
var boxCollider = gameObject.AddComponent<BoxCollider>();

gameObject.Show(); // gameObject.SetActive(true)
this.Show(); // this.gameObject.SetActive(true)
boxCollider.Show(); // boxCollider.gameObject.SetActive(true)
transform.Show(); // transform.gameObject.SetActive(true)

gameObject.Hide(); // gameObject.SetActive(false)
this.Hide(); // this.gameObject.SetActive(false)
boxCollider.Hide(); // boxCollider.gameObject.SetActive(false)
transform.Hide(); // transform.gameObject.SetActive(false)

this.DestroyGameObj();
boxCollider.DestroyGameObj();
transform.DestroyGameObj();

this.DestroyGameObjGracefully();
boxCollider.DestroyGameObjGracefully();
transform.DestroyGameObjGracefully();

this.DestroyGameObjAfterDelay(1.0f);
boxCollider.DestroyGameObjAfterDelay(1.0f);
transform.DestroyGameObjAfterDelay(1.0f);

this.DestroyGameObjAfterDelayGracefully(1.0f);
boxCollider.DestroyGameObjAfterDelayGracefully(1.0f);
transform.DestroyGameObjAfterDelayGracefully(1.0f);

gameObject.Layer(0);
this.Layer(0);
boxCollider.Layer(0);
transform.Layer(0);

gameObject.Layer("Default");
this.Layer("Default");
boxCollider.Layer("Default");
transform.Layer("Default");
```

#### 5.Graphic

``` csharp
var image = gameObject.AddComponent<Image>();
var rawImage = gameObject.AddComponent<RawImage>();

// image.color = new Color(image.color.r,image.color.g,image.color.b,1.0f);
image.ColorAlpha(1.0f);
rawImage.ColorAlpha(1.0f);
```

#### 6.Image

``` csharp
var image1 = gameObject.AddComponent<Image>();

image1.FillAmount(0.0f); // image1.fillAmount = 0.0f;
```

#### 7.Object

``` csharp
gameObject.Instantiate()
    .Name("ExtensionExample")
	.DestroySelf();

gameObject.Instantiate()
	.DestroySelfGracefully();

gameObject.Instantiate()
	.DestroySelfAfterDelay(1.0f);

gameObject.Instantiate()
	.DestroySelfAfterDelayGracefully(1.0f);

gameObject
	.ApplySelfTo(selfObj => Debug.Log(selfObj.name))
	.Name("TestObj")
	.ApplySelfTo(selfObj => Debug.Log(selfObj.name))
	.Name("ExtensionExample")
	.DontDestroyOnLoad();
```

#### 8.Transform

``` csharp
transform
	.Parent(null)
	.LocalIdentity()
	.LocalPositionIdentity()
	.LocalRotationIdentity()
	.LocalScaleIdentity()
	.LocalPosition(Vector3.zero)
	.LocalPosition(0, 0, 0)
	.LocalPosition(0, 0)
	.LocalPositionX(0)
	.LocalPositionY(0)
	.LocalPositionZ(0)
	.LocalRotation(Quaternion.identity)
	.LocalScale(Vector3.one)
	.LocalScaleX(1.0f)
	.LocalScaleY(1.0f)
	.Identity()
	.PositionIdentity()
	.RotationIdentity()
	.Position(Vector3.zero)
	.PositionX(0)
	.PositionY(0)
	.PositionZ(0)
	.Rotation(Quaternion.identity)
	.DestroyAllChild()
	.AsLastSibling()
	.AsFirstSibling()
	.SiblingIndex(0);

this
	.Parent(null)
	.LocalIdentity()
	.LocalPositionIdentity()
	.LocalRotationIdentity()
	.LocalScaleIdentity()
	.LocalPosition(Vector3.zero)
	.LocalPosition(0, 0, 0)
	.LocalPosition(0, 0)
	.LocalPositionX(0)
	.LocalPositionY(0)
	.LocalPositionZ(0)
	.LocalRotation(Quaternion.identity)
	.LocalScale(Vector3.one)
	.LocalScaleX(1.0f)
	.LocalScaleY(1.0f)
	.Identity()
	.PositionIdentity()
	.RotationIdentity()
	.Position(Vector3.zero)
	.PositionX(0)
	.PositionY(0)
	.PositionZ(0)
	.Rotation(Quaternion.identity)
	.DestroyAllChild()
	.AsLastSibling()
	.AsFirstSibling()
	.SiblingIndex(0);
```

#### 9.UnityAction

``` csharp
UnityAction action = () => { };
UnityAction<int> actionWithInt = num => { };
UnityAction<int, string> actionWithIntString = (num, str) => { };

action.InvokeGracefully();
actionWithInt.InvokeGracefully(1);
actionWithIntString.InvokeGracefully(1, "str");
```

#### FeatureId:EXUN001