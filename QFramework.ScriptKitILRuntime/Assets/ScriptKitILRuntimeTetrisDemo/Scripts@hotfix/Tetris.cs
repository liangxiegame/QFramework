using UniRx;

namespace QFramework.ILKitDemo.Tetris
{
    public class Tetris : ILArchitecture<Tetris>
    {
        protected override void OnSystemConfig(ILRuntimeIOCContainer systemLayer)
        {
        }

        protected override void OnModelConfig(ILRuntimeIOCContainer modelLayer)
        {
            modelLayer.RegisterInstance<IGameModel>(new GameModel());
        }

        protected override void OnUtilityConfig(ILRuntimeIOCContainer utilityLayer)
        {
            utilityLayer.RegisterInstance<IStorage>(new PlayerPrefsStorage());
        }

        protected override void OnLaunch()
        {
            GetModel<IGameModel>()
                .Load();

            Observable.OnceApplicationQuit()
                .Subscribe(_ =>
                {
                    GetModel<IGameModel>()
                        .Save();
                });
        }
    }
}