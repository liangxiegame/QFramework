### UI 模块简介:

自动生成UI脚本，标记自动绑定组件，提供便捷管理。

帮助拓展UI控件，帮助自定义UI编辑器面板。

#### 什么是 UI 模块 ?

* 游戏界面管理

* UI组件扩展

* UI交互帮助

* 提供方便的UI编辑操作


  #### 这个 UI 模块有什么亮点？

  基于QUIManager与CreatUICode便捷迅速地完成界面的开发。

### UI模块特征

* ##### CodeGenerator

  配合QUIMark生成UICode并对Mark的组件自动绑定。

* ##### UIManager


  管理与组织界面，如创建、显示隐藏界面，划分了UI层级。


### UICode 的生命周期

```csharp
Init(IUIData uiData = null);

Show();
    
OnShow();
    
IUIBehaviour.Close(bool destroyed = true);

OnClose();

OnDestroy();
    
OnBeforeDestroy();   
```




### 创建你的第一个QFramework UI 界面

创建一个Panel并命名，建议以UI为前缀。并添加一些元素（例如Button），并为其添加**标记**。标记方式为添加**QUIMark Component**

将你编辑好的 Panel 以 **Prefab** 方式存放到Resources 或 标记为AssetBundle

右击 preafb,选择 ** QFramework-Create UICode **，会生成对应脚本。

### 开始编写你的第一个QUI脚本

使用 **Create UICode** 会生成两份有关的脚本文件

``` csharp
namespace QFramework.UIExample
{
	//界面相关的PanelCode
    public class UIMenuPanelData : IUIData
    {
        // 这里存放Panel相关数据
    }
    public partial class UIMenuPanel : QUIBehaviour
    {
        protected override void InitUI(IUIData uiData = null)
        {
            mData = uiData as UIMenuPanelData;
            //please add init code here
        }

        protected override void ProcessMsg(int eventId, QMsg msg)
        {
            throw new System.NotImplementedException();
            // 这里处理消息
        }
        
        protected override void RegisterUIEvent()
        {
			//这里可以进行UI事件的注册
        }   
        UIMenuPanelData mData = null;
    }
}

```

```csharp
namespace QFramework.UIExample
{
	//收集被标记的组件
	public partial class UIMenuPanel
	{
		[SerializeField] public Button BtnPlay;
		[SerializeField] public Button BtnSetting;
	}
}
```



### QUICode使用流程

* 使用 **UIMgr.OpenPanel** 创建你的UI界面

  ```csharp
  internal static T OpenPanel<T>(int canvasLevel = UILevel.Common, IUIData uiData = null, string assetBundleName = null,string prefabName = null) where T : QUIBehaviour
  ```

  UILevel:来区分UIPanel的层级，例如UILeve.PopUI，UILeve.Guide，会放置在 UIRoot 中对应区域

  UIData:传递Panel初始化的数据 (UIData 会自动生成模板)

  assetBundleName/prefabName：Panel所相对应的路径

* 关闭界面

  ```csharp
  CloseSelf();
  ```

  可编写关闭响应

  ```csharp
  protected override void OnClose()
  {
      UnRegisterAllEvent();
      base.OnClose();
  }
  ```

* 显示与隐藏

  ```csharp
   Show();
   Hide();
  ```

  编写相关响应

  ```csharp
  protected override void OnShow()
  {
      base.OnShow();
  }
  protected override void OnHide()
  {
      base.OnHide();
  }
  ```

* 消息（事件）的注册与发送

  注册你的事件

  ```csharp
  protected void RegisterEvent<T>(T eventId) where T : IConvertible
  ```

  发送

  ```csharp
  protected void SendEvent<T>(T eventId) where T : IConvertible
  ```

  响应

  ```csharp
  protected virtual void ProcessMsg (int eventId,QMsg msg) {}
  ```

  UI事件的ID段为3000～5999

  ```csharp
  	public class QMsgSpan
  	{
  		public const int Count = 3000;
  	}
  	public partial class QMgrID
  	{
  		public const int Framework = 0;
  		public const int UI = Framework + QMsgSpan.Count; // 3000
  		public const int Audio = UI + QMsgSpan.Count; // 6000
      }
  ```

### 用 QFramework 的方式启动你的界面

为你的场景添加 **UIRoot**，位置在 Aseets/QFramework/Framework/3.UI/Resources/UIRoot。

启动你的第一个界面

```csharp
namespace QFramework.UIExample
{
	public class Example : MonoBehaviour
	{
		private void Awake()
		{
            UIMgr.OpenPanel<UIMenuPanel>(prefabName: "Resources/UIMenuPanel");
		}
	}
}
```
