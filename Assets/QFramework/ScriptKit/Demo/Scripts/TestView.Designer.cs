/****************************************************************************
 * 2019.4 XAVIER
 ****************************************************************************/

namespace QFramework.Example
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class TestView
	{
		public const string NAME = "TestView";

		[SerializeField] public Button Button;

		protected override void ClearUIComponents()
		{
			Button = null;
			mData = null;
		}

		private TestViewData mPrivateData = null;

		public TestViewData mData
		{
			get { return mPrivateData ?? (mPrivateData = new TestViewData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
