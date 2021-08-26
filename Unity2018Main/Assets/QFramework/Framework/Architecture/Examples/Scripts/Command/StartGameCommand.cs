namespace QFramework.PointGame
{
    public class StartGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            // 重置数据
            var gameModel = this.GetModel<IGameModel>();
          
            gameModel.KillCount.Value = 0;
            gameModel.Score.Value = 0;
            
            this.SendEvent<GameStartEvent>();
        }
    }
}