/****************************************************************************
 * 2019.1 LIANGXIE
 ****************************************************************************/

namespace QFramework.Example
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class UIMenuPanel
	{
		public const string NAME = "UIMenuPanel";

		[SerializeField] public Image ImageBg;
		[SerializeField] public Button BtnPlay;
		[SerializeField] public Button BtnSetting;

		protected override void ClearUIComponents()
		{
			ImageBg = null;
			BtnPlay = null;
			BtnSetting = null;
			mData = null;
		}

		private UIMenuPanelData mPrivateData = null;

		public UIMenuPanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new UIMenuPanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
