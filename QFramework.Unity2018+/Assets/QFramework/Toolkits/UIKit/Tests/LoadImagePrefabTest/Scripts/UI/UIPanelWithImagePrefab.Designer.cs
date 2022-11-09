using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:45b531f7-1b13-45f8-a05a-17d3bc9cca3b
	public partial class UIPanelWithImagePrefab
	{
		public const string Name = "UIPanelWithImagePrefab";
		
		
		private UIPanelWithImagePrefabData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIPanelWithImagePrefabData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIPanelWithImagePrefabData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIPanelWithImagePrefabData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
