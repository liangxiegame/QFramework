using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:3381426a-864d-437a-8d4d-65b79984658f
	public partial class UIMultiPanel
	{
		public const string Name = "UIMultiPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Text PageIndex;
		
		private UIMultiPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			PageIndex = null;
			
			mData = null;
		}
		
		public UIMultiPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIMultiPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIMultiPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
