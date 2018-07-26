/****************************************************************************
 * 2018.7 iMac03010154
 ****************************************************************************/

namespace QFramework.UIExample
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class UIGamePanel
	{
		public const string NAME = "UIGamePanel";

		[SerializeField] public Text gameText;
		[SerializeField] public Button backBtn;

		protected override void ClearUIComponents()
		{
			gameText = null;
			backBtn = null;
		}

		private UIGamePanelData mPrivateData = null;

		public UIGamePanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new UIGamePanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
