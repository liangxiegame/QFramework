using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:e3b8305d-9402-4df9-a57b-6d0366339dab
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
