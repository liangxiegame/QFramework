using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.PlatformRunner
{
	public partial class UIGamePanel
	{
		[SerializeField] public Button BtnPause;
		[SerializeField] public UGUINumberLabel Time;
		[SerializeField] public UGUINumberLabel MeterNumber;
		
		protected override void ClearUIComponents()
		{
			BtnPause = null;
			Time = null;
			MeterNumber = null;
		}
	}
}
