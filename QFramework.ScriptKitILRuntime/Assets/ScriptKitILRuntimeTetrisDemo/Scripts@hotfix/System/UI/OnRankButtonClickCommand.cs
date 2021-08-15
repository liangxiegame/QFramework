namespace QFramework.ILKitDemo.Tetris
{
    public class OnRankButtonClickCommand : ILCommand<Tetris>
    {

        
        public override void Execute()
        {
            var gameModel = GetModel<IGameModel>();
            
            ILUIKit.GetPanel<UITetrisPanel>()
                .ShowRankUI(gameModel.Score.Value, gameModel.HighScore.Value, gameModel.NumbersGame.Value);
        }
    }
}