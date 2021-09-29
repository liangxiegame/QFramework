using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:99e91740-9950-4440-9972-fa37945ef93b
	public partial class UIABCPanel
	{
		public const string Name = "UIABCPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Button;
		
		private UIABCPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Button = null;
			
			mData = null;
		}
		
		public UIABCPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIABCPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIABCPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
