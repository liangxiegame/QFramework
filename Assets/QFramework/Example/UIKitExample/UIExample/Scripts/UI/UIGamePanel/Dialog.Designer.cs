/****************************************************************************
 * 2019.7 LIANGXIE
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public partial class Dialog
	{
		[SerializeField] public Image Image;

		public void Clear()
		{
			Image = null;
		}

		public override string ComponentName
		{
			get { return "Dialog";}
		}
	}
}
