using UnityEngine;
using QFramework;

namespace QFramework.ILKitDemo.Tetris
{
	public partial class SettingUI : ILViewController<Tetris>
	{
		protected override void OnStart()
		{
			AudioButton.onClick.AddListener(() => SendCommand<OnAudioButtonClickCommand>());
		}

		protected override void OnDestroy()
		{
			
		}
	}
}
