using UnityEngine;
using UnityEngine.UI;

namespace QFramework.PlatformRunner
{
	public partial class UIPropShop
	{
		[SerializeField] public UGUINumberLabel BtnBuy;

		public void Clear()
		{
			BtnBuy = null;
		}

	}
}
