namespace QFramework.PointGame
{
    public class PointGame : Architecture<PointGame>
    {
        protected override void Init()
        {
            RegisterSystem<IScoreSystem>(new ScoreSystem());
            RegisterSystem<ICountDownSystem>(new CountDownSystem());
            RegisterSystem<IAchievementSystem>(new AchievementSystem());

            RegisterModel<IGameModel>(new GameModel());

            RegisterUtility<IStorage>(new PlayerPrefsStorage());
        }
    }
}
