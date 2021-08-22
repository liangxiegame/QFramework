using DG.Tweening;
using UnityEngine;

namespace QFramework.ILKitDemo.Tetris
{
    public partial class GameUI : ILViewController<Tetris>
    {
        protected override void OnStart()
        {
            PauseButton.onClick.AddListener(() => SendCommand(new PauseGameCommand()));
        }

        protected override void OnDestroy()
        {
            // Destory Code Here
        }

        public void WithData(int score, int highScore)
        {
            ScoreText.text = score.ToString();
            HighScoreText.text = highScore.ToString();
            (transform as RectTransform).DOAnchorPosY(-160.3f, 0.5f);
        }
    }
}