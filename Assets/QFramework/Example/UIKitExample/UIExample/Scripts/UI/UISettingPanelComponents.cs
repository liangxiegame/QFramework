/****************************************************************************
 * 2018.5 凉鞋的MacBook Pro (2)
 ****************************************************************************/

namespace QFramework.UIExample
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class UISettingPanel	{
		public const string NAME = "UISettingPanel";
		[SerializeField] public Button eventBtn;
		[SerializeField] public Button backBtn;

		protected override void ClearUIComponents()
		{
			eventBtn = null;
			backBtn = null;
		}
	}
}
