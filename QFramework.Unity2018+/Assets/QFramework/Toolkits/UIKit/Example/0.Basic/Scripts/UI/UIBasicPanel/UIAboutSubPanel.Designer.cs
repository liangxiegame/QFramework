/****************************************************************************
 * 2022.7 LIANGXIEWIN
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public partial class UIAboutSubPanel
	{
		[SerializeField] public UnityEngine.UI.Button BtnClose;

		public void Clear()
		{
			BtnClose = null;
		}

		public override string ComponentName
		{
			get { return "UIAboutSubPanel";}
		}
	}
}
