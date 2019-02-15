/****************************************************************************
 * 2019.1 LIANGXIE
 ****************************************************************************/

namespace QFramework.Example
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class UISectionPanel
	{
		public const string NAME = "UISectionPanel";

		[SerializeField] public Button SectionBtn;
		[SerializeField] public Button backBtn;
		[SerializeField] public Button settingBtn;

		protected override void ClearUIComponents()
		{
			SectionBtn = null;
			backBtn = null;
			settingBtn = null;
			mData = null;
		}

		private UISectionPanelData mPrivateData = null;

		public UISectionPanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new UISectionPanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
