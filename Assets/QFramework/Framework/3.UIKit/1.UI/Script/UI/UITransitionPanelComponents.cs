/****************************************************************************
 * 2018.9 凉鞋的MacBook Pro (2)
 ****************************************************************************/

namespace QFramework
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class UITransitionPanel
	{
		public const string NAME = "UITransitionPanel";


		protected override void ClearUIComponents()
		{
			mData = null;
		}

		private UITransitionPanelData mPrivateData = null;

		public UITransitionPanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new UITransitionPanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
