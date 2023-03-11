using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIPanelWithImagePrefabData : UIPanelData
	{
	}
	public partial class UIPanelWithImagePrefab : UIPanel
	{
		private ResLoader mResLoader = ResLoader.Allocate();
		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			throw new System.NotImplementedException();
		}
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIPanelWithImagePrefabData ?? new UIPanelWithImagePrefabData();
			// please add init code here

			mResLoader.LoadSync<GameObject>("ImagePrefabA").InstantiateWithParent(this.transform);
			mResLoader.LoadSync<GameObject>("ImagePrefabB").InstantiateWithParent(this.transform);
			mResLoader.LoadSync<GameObject>("ImagePrefabC").InstantiateWithParent(this.transform);


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
			
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}
