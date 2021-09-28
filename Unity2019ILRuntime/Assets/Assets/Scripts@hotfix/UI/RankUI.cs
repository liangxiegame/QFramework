namespace QFramework.ILKitDemo.Tetris
{
	public partial class RankUI : ILViewController<Tetris>
	{
		protected override void OnStart()
		{
			// Code Here
			DestroyButton.onClick.AddListener(() =>
				SendCommand(new OnDestoryButtonClickCommand()));
		}

		protected override void OnDestroy()
		{
			// Destory Code Here
		}

		public void WithData(int score, int highScore, int numbersGame)
		{
			RankUIScoreText.text = score.ToString();
			RankUIHighScoreText.text = highScore.ToString();
			NumbersGameText.text = numbersGame.ToString();
		}
	}
}
