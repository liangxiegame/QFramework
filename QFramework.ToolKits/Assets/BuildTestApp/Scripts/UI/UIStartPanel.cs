using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIStartPanelData : UIPanelData
	{
	}
	public partial class UIStartPanel : UIPanel
	{
		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			throw new System.NotImplementedException();
		}
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIStartPanelData ?? new UIStartPanelData();
			// please add init code here
			
			BtnStart.onClick.AddListener(() =>
			{
				UIKit.OpenPanelAsync<UIGameInfoPanel>().ToAction().Start(this, CloseSelf);
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
