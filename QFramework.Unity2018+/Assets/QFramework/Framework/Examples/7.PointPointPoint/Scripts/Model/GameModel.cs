namespace QFramework.PointGame
{
    public interface IGameModel : IModel
    {
        BindableProperty<int> KillCount { get; }

        BindableProperty<int> Gold { get; }

        BindableProperty<int> Score { get; }

        BindableProperty<int> BestScore { get; }
        
        BindableProperty<int> Life { get; } 

    }

    public class GameModel : AbstractModel, IGameModel
    {
        public BindableProperty<int> KillCount { get; } = new BindableProperty<int>()
        {
            Value = 0
        };

        public BindableProperty<int> Gold { get; } = new BindableProperty<int>()
        {
            Value = 0
        };

        public BindableProperty<int> Score { get; } = new BindableProperty<int>()
        {
            Value = 0
        };

        public BindableProperty<int> BestScore { get; } = new BindableProperty<int>()
        {
            Value = 0
        };

        public BindableProperty<int> Life { get; } = new BindableProperty<int>();

        protected override void OnInit()
        {
            var storage = this.GetUtility<IStorage>();

            BestScore.Value = storage.LoadInt(nameof(BestScore), 0);
            BestScore.Register(v => storage.SaveInt(nameof(BestScore), v));
            
            Life.Value = storage.LoadInt(nameof(Life), 3); 
            Life.Register(v => storage.SaveInt(nameof(Life), v)); 
          
            Gold.Value = storage.LoadInt(nameof(Gold), 0); 
            Gold.Register((v) => storage.SaveInt(nameof(Gold), v)); 
        }
    }
}