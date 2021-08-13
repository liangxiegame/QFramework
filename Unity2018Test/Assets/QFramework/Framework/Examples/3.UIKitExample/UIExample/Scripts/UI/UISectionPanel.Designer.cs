using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:18d09b6d-fe4e-4a3f-adc5-eae7c748c4c4
	public partial class UISectionPanel
	{
		public const string Name = "UISectionPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button SectionBtn;
		[SerializeField]
		public UnityEngine.UI.Button BackBtn;
		[SerializeField]
		public UnityEngine.UI.Button SettingBtn;
		
		private UISectionPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SectionBtn = null;
			BackBtn = null;
			SettingBtn = null;
			
			mData = null;
		}
		
		public UISectionPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UISectionPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UISectionPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
