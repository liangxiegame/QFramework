using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.PlatformRunner
{
	public partial class UIPausePanel
	{
		[SerializeField] public Button BtnReplay;
		[SerializeField] public Button BtnContinue;
		[SerializeField] public Toggle BtnSound;
		[SerializeField] public Button BtnHome;

		protected override void ClearUIComponents()
		{
			BtnReplay = null;
			BtnContinue = null;
			BtnSound = null;
			BtnHome = null;
		}
	}
}
