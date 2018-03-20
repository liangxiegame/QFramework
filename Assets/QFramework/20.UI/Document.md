### UI 模块简介:

提供了对UI控件、事件的拓展，提供了以Panel为单位的一套UI模版生成、管理工具。以及包含了对UnityEditorUI编辑工具

#### 什么 UI 模块 ?

* 游戏界面管理

* UI组件扩展

* UI交互帮助

* 提供方便的UI编辑操作


  #### 这个 UI 模块有什么亮点？

  基于QUIManager与CreatUICode便捷迅速地完成界面的开发。

### 创建你的第一个QFarmUI界面

创建一个Panel并命名，建议以UI为前缀。并添加一些元素（例如Button），并为其添加**标记**。标记方式为添加**QUIMark Component**

将你编辑好的Panel以**Prefab**方式存放到Resources 或 标记为AssetBundle

右击 preafb,选择 **QFramework-Create UICode **，会生成对应脚本。

### 开始编写你的第一个QFram脚本

``` csharp
namespace QFramework.UIExample

{

    public class UIMenuPanelData : IUIData

    {
        // TODO: Query
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
        }
        
        protected override void RegisterUIEvent()
        {
            BtnPlay.onClick.AddListener(() =>
            {
                Log.I("on btn play clicked");
                UIMgr.OpenPanel<UISelectionPanel>(UILevel.Common, prefabName: "Resources/UISectionPanel");
                CloseSelf();
            });
            BtnSetting.onClick.AddListener(() =>
            {
                Log.I("on btn setting clicked");
                UIMgr.OpenPanel<UISettingPanel>(UILevel.PopUI, prefabName: "Resources/UISettingPanel");
            });
        }   
        UIMenuPanelData mData = null;
    }
}

```

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

### 管理你的界面

```csharp
		internal static T OpenPanel<T>(int canvasLevel = UILevel.Common, IUIData uiData = null, string assetBundleName = null,
			string prefabName = null) where T : QUIBehaviour
			
			
		internal static void ClosePanel<T>() where T : QUIBehaviour
```

使用**UIMgr**的**OpenPanel**与**ClosePanel**来开关你的界面。

```csharp
public class UILevel
	{
		public const int Bg                 = -2;  //背景层UI
		public const int AnimationUnderPage = -1; //动画层
		public const int Common             = 0; //普通层UI
		public const int AnimationOnPage    = 1; // 动画层
		public const int PopUI              = 2; //弹出层UI
		public const int Guide              = 3; //新手引导层
		public const int Const              = 4; //持续存在层UI
		public const int Toast              = 5; //对话框层UI
		public const int Forward            = 6; //最高UI层用来放置UI特效和模型
	}
```

请对你的UIPanel进行分层，他们将显示在QUIManager对应的层级中。