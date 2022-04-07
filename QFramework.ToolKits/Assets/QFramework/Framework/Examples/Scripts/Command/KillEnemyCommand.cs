namespace QFramework.PointGame
{
    public class KillEnemyCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var gameModel = this.GetModel<IGameModel>();
            
            gameModel.KillCount.Value++;
            
            if (UnityEngine.Random.Range(0,10) < 3)
            {
                gameModel.Gold.Value += UnityEngine.Random.Range(1, 3);
            }

            this.SendEvent<OnEnemyKillEvent>();

            if (gameModel.KillCount.Value == 10)
            {
                this.SendEvent<GamePassEvent>();
            }
        }
    }
}