namespace QFramework.ILKitDemo.Tetris
{
    public class OnRestartButtonClickCommand : ILCommand<Tetris>
    {
        public override void Execute()
        {
            var tetrisPanel = GetPanel<UITetrisPanel>();
            
            tetrisPanel.HideGameOverUI();
            
            var model = GetModel<IGameModel>();
            
            model.Restart();
            
            tetrisPanel.gameManager.StartGame();
            
            tetrisPanel.UpdateGameUI(0, model.HighScore.Value);
        }
    }
}