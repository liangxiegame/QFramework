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

![F2DC34F-821A-46DA-9283-0641A795A4C](/Users/vin129/Library/Containers/com.tencent.qq/Data/Library/Application Support/QQ/Users/515019721/QQ/Temp.db/DF2DC34F-821A-46DA-9283-0641A795A4C6.png)

![B759EFC-4204-4117-A092-5C791968963](/Users/vin129/Library/Containers/com.tencent.qq/Data/Library/Application Support/QQ/Users/515019721/QQ/Temp.db/CB759EFC-4204-4117-A092-5C791968963F.png)

将你编辑好的Panel以**Prefab**方式存放到Resources 或 标记为AssetBundle

右击 preafb,选择 **QFramework-Create UICode **，会生成对应脚本。

![4021508-768C-42BC-B44F-B705AF24E1C](/Users/vin129/Library/Containers/com.tencent.qq/Data/Library/Application Support/QQ/Users/515019721/QQ/Temp.db/94021508-768C-42BC-B44F-B705AF24E1C5.png)

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

![5CF9EB1-BDD7-40DF-A29A-A1791FBA198](/Users/vin129/Library/Containers/com.tencent.qq/Data/Library/Application Support/QQ/Users/515019721/QQ/Temp.db/C5CF9EB1-BDD7-40DF-A29A-A1791FBA1989.png)



开启你的第一个界面

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

