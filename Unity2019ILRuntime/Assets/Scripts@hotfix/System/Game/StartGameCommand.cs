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

            tetrisPanel.LogoName.Hide();
            tetrisPanel.MenuUI.Hide();
            tetrisPanel.DoBeforeEnteringPlay();

            tetrisPanel.ShowGameUI(model.Score.Value, model.HighScore.Value);

            SendCommand(new CameraZoomInCommand(tetrisPanel.MainCamera));

            tetrisPanel.gameManager.StartGame();
        }
    }
}