namespace QFramework.ILKitDemo.Tetris
{
	public partial class GameOverUI : ILViewController<Tetris>
	{
		protected override void OnStart()
		{
			RestartButton.onClick.AddListener(() =>
			{
				SendCommand<OnRestartButtonClickCommand>();
			});
		}

		protected override void OnDestroy()
		{
		}
	}
}
