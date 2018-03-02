
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
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
