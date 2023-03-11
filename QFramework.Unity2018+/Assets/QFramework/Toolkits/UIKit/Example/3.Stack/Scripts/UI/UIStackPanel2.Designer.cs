using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:41781e95-be2b-4e2a-89fd-17abb779e25a
	public partial class UIStackPanel2
	{
		public const string Name = "UIStackPanel2";
		
		
		private UIStackPanel2Data mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIStackPanel2Data Data
		{
			get
			{
				return mData;
			}
		}
		
		UIStackPanel2Data mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIStackPanel2Data());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
