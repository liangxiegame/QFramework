using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.PlatformRunner
{
	public partial class UILoadingStagePanel
	{
		[SerializeField] public RawImage SwitchTreeImage;

		protected override void ClearUIComponents()
		{
			SwitchTreeImage = null;
		}
	}
}
