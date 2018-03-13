
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.PlatformRunner
{
	public class UIMenuPanelComponents : MonoBehaviour, IUIComponents
	{
		[SerializeField] public Button BtnPlay;
		[SerializeField] public Button BtnSetting;

		public void Clear()
		{
			BtnPlay = null;
			BtnSetting = null;
		}

	}
}
