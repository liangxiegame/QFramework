/****************************************************************************
 * 2019.4 vin129的MacBook Pro
 ****************************************************************************/

namespace QFramework.Example
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class TestView
	{
		public const string NAME = "TestView";

		[SerializeField] public Button Button1;
		[SerializeField] public Button Button2;

		protected override void ClearUIComponents()
		{
			Button1 = null;
			Button2 = null;
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
