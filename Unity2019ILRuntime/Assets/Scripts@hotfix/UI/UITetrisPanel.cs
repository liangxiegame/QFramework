using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QFramework.ILKitDemo.Tetris
{
    public class UITetrisPanelData : ILUIData
    {
    }

    public partial class UITetrisPanel : ILUIPanelController<Tetris>
    {
        public GameModel Model { get; set; }

        public GameManager gameManager;


        public void DoBeforeEnteringPlay()
        {
            ShowGameUI(Model.Score.Value, Model.HighScore.Value);

            SendCommand(new CameraZoomInCommand(MainCamera));

            gameManager.StartGame();
        }

        public void DoBeforeLeavingPlay()
        {
            HideGameUI();
            ShowRestartButton();
            gameManager.PauseGame();
        }


        public void UpdateGameUI(int score, int highScore)
        {
            GameUI.GetILComponent<GameUI>().ScoreText.text = score.ToString();
            GameUI.GetILComponent<GameUI>().HighScoreText.text = highScore.ToString();
        }

        public void ShowGameUI(int score = 0, int highScore = 0)
        {
            GameUI.Show()
                .GetILComponent<GameUI>()
                .WithData(score, highScore);
        }

        public void HideGameUI()
        {
            (GameUI.transform as RectTransform).AnchorPosY(160.4f);
            GameUI.Hide();
        }

        public void ShowRestartButton()
        {
            GameOverUI.GetILComponent<GameOverUI>().RestartButton.Show();
        }

        public void ShowGameOverUI(int score = 0)
        {
            GameOverUI.Show();
            GameOverUI.GetILComponent<GameOverUI>()
                .GameOverUIText.text = score.ToString();
        }

        public void HideGameOverUI()
        {
            GameOverUI.gameObject.SetActive(false);
        }

        public void OnHomeButtonClick()
        {
            SendCommand<PlayCursorSoundCommand>();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void SetIsAudioOn(bool isActive)
        {
            SettingUI.GetILComponent<SettingUI>().Mute.gameObject.SetActive(!isActive);
        }

        public void OnSettingUIClick()
        {
            SendCommand<PlayCursorSoundCommand>();
            SettingUI.gameObject.SetActive(false);
        }

        public void ShowRankUI(int score, int highScore, int numbersGame)
        {
            RankUI.Show()
                .GetILComponent<RankUI>()
                .WithData(score, highScore, numbersGame);
        }

        public void OnRankUIClick()
        {
            RankUI.gameObject.SetActive(false);
        }

        protected override void OnOpen(ILUIData uiData = null)
        {
            mData = uiData as UITetrisPanelData ?? new UITetrisPanelData();


            Model = new GameModel();


            RankUI.GetComponent<Button>().onClick.AddListener(OnRankUIClick);
            SettingUI.GetComponent<Button>().onClick.AddListener(OnSettingUIClick);


            SetIsAudioOn(AudioKit.Settings.IsOn.Value);
            AudioKit.Settings.IsOn.Bind(SetIsAudioOn);

            gameManager = new GameManager(transform);

            SendCommand(new EnteringMenuCommand());
        }

        protected override void OnClose()
        {
        }
    }
}