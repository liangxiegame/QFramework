namespace QFramework.PlatformRunner
{
    public class PlatformRunner : Architecture<PlatformRunner>
    {
        protected override void Init()
        {
            RegisterModel<IPlayerModel>(new PlayerModel());
            RegisterModel<IGameModel>(new GameModel());
        }
    }
}