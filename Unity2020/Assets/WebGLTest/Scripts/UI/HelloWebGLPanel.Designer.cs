using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f531f363-7e6e-4062-8b22-5a05b9f68b22
	public partial class HelloWebGLPanel
	{
		public const string Name = "HelloWebGLPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Button;
		
		private HelloWebGLPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Button = null;
			
			mData = null;
		}
		
		public HelloWebGLPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		HelloWebGLPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new HelloWebGLPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
