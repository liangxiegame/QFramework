using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:83b8348d-4028-4ae9-942c-2255cb590d89
	public partial class UIGamePanel
	{
		public const string Name = "UIGamePanel";
		
		[SerializeField]
		public UnityEngine.UI.Text gameText;
		[SerializeField]
		public UnityEngine.UI.Button backBtn;
		[SerializeField]
		public UnityEngine.UI.Text Text2;
		[SerializeField]
		public UIGameSubElement UIGameSubElement;
		
		private UIGamePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			gameText = null;
			backBtn = null;
			Text2 = null;
			UIGameSubElement = null;
			
			mData = null;
		}
		
		public UIGamePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGamePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGamePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
