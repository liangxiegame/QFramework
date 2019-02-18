/****************************************************************************
 * 2019.1 LIANGXIE
 ****************************************************************************/

namespace QFramework.Example
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class UISettingPanel
	{
		public const string NAME = "UISettingPanel";

		[SerializeField] public Button eventBtn;
		[SerializeField] public Button backBtn;

		protected override void ClearUIComponents()
		{
			eventBtn = null;
			backBtn = null;
			mData = null;
		}

		private UISettingPanelData mPrivateData = null;

		public UISettingPanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new UISettingPanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
