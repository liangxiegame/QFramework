using QFramework;

namespace SnakeGame
{
    public class SnakeGame : Architecture<SnakeGame>
    {
        protected override void Init()
        {
            RegisterSystem<IGridNodeSystem>(new GridNodeSystem());
            RegisterSystem<IGridCreateSystem>(new GridCreateSystem());
            RegisterSystem<ISnakeCreateSystem>(new SnakeCreateSystem());

            RegisterSystem<ITimeSystem>(new TimeSystem());
            RegisterSystem<IInputSystem>(new InputSystem());
            RegisterSystem<ISnakeSystem>(new SnakeSystem());
        }
    }
}