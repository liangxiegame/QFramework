using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:e2d08fe3-1678-415f-b88f-dad810ad76bf
	public partial class UIGameInfoPanel
	{
		public const string Name = "UIGameInfoPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnBack;
		
		private UIGameInfoPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnBack = null;
			
			mData = null;
		}
		
		public UIGameInfoPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGameInfoPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGameInfoPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
