using DG.Tweening;
using UnityEngine;

namespace QFramework.ILKitDemo.Tetris
{
    public class StartGameCommand : ILCommand<Tetris>
    {
        public override void Execute()
        {
            var tetrisPanel = ILUIKit.GetPanel<UITetrisPanel>();
            var model = this.GetModel<IGameModel>();

            SendCommand<PlayCursorSoundCommand>();

            tetrisPanel.LogoName.DOAnchorPosY(160.4f, 0.5f)
                .OnComplete(() => tetrisPanel.LogoName.Hide());
            (tetrisPanel.MenuUI.transform as RectTransform)
                .DOAnchorPosY(-66.64f, 0.5f)
                .OnComplete(() => tetrisPanel.MenuUI.Hide());
            tetrisPanel.DoBeforeEnteringPlay();

            tetrisPanel.ShowGameUI(model.Score.Value, model.HighScore.Value);

            SendCommand(new CameraZoomInCommand(tetrisPanel.MainCamera));

            tetrisPanel.gameManager.StartGame();
        }
    }
}