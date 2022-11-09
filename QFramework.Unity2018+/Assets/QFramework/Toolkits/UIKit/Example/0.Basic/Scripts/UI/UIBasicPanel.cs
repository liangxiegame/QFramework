using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIBasicPanelData : UIPanelData
	{
	}
	public partial class UIBasicPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIBasicPanelData ?? new UIBasicPanelData();
			
			BtnStart.onClick.AddListener(() =>
			{
				Debug.Log("开始游戏");
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			
			Debug.Log("UIBasicPanel OnOpen");
		}
		
		protected override void OnShow()
		{
			Debug.Log("UIBasicPanel OnShow");
		}
		
		protected override void OnHide()
		{
			Debug.Log("UIBasicPanel OnHide");
		}
		
		protected override void OnClose()
		{
			Debug.Log("UIBasicPanel OnClose");
		}
	}
}
