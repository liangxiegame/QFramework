using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:6c4e7fdf-0e37-48af-8148-ee9990dca762
	public partial class UIStackPanel1
	{
		public const string Name = "UIStackPanel1";
		
		
		private UIStackPanel1Data mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIStackPanel1Data Data
		{
			get
			{
				return mData;
			}
		}
		
		UIStackPanel1Data mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIStackPanel1Data());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
