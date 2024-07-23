/****************************************************************************
 * 2022.4 凉鞋的MacBook Pro
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public partial class UIGameSubElement
	{
		[SerializeField] public UnityEngine.UI.Text Text3;

		public void Clear()
		{
			Text3 = null;
		}

		public override string ComponentName
		{
			get { return "UIGameSubElement";}
		}
	}
}
