### UI 模块简介:

提供了对UI控件、事件的拓展，提供了以Panel为单位的一套UI模版生成、管理工具。以及包含了对UnityEditorUI编辑工具

#### 什么 UI 模块 ?

* 游戏界面管理

* UI组件扩展

* UI交互帮助

* 提供方便的UI编辑操作


  #### 这个 UI 模块有什么亮点？

  基于QUIManager与CreatUICode便捷迅速地完成界面的开发。

### 创建你的第一个QFramUI界面

创建一个Panel并命名，建议以UI为前缀。并添加一些元素（例如Button），并为其添加**标记**。标记方式为添加**QUIMark Component**

将你编辑好的Panel以**Prefab**方式存放到Resources 或 标记为AssetBundle

右击 preafb,选择 **QFramework-Create UICode **，会生成对应脚本。

### 开始编写你的第一个QUI脚本

使用**Create UICode**会生成两份有关的脚本文件

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

* 使用**UIMgr.OpenPanel**创建你的UI界面

  ```csharp
  internal static T OpenPanel<T>(int canvasLevel = UILevel.Common, IUIData uiData = null, string assetBundleName = null,string prefabName = null) where T : QUIBehaviour
  ```

  UILeve:来区分UIPanel的层级，例如UILeve.PopUI，UILeve.Guide，会放置在QUIManager中对应区域

  UIData:传递Panel初始化的数据

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

  ​

  ​


### 用QFarm的方式启动你的界面

为你的场景添加**QUIManager**，位置在 Aseets/QFramework/20.UI/Resources/QUIManager。

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

### 
