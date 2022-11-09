using QFramework;

namespace SnakeGame
{
    internal interface ISnakeSystem : ISystem
    {
        void CreateSnake(int w, int h);
    }
    public class SnakeSystem : AbstractSystem, ISnakeSystem
    {
        private ISnake mCurSnake;
        private int mBodyCount;
        public int mSecondIndex;

        private DelayTask mAutoMoveTask;
        private SnakeMoveEvent mMoveEvent;

        protected override void OnInit()
        {
            this.RegisterEvent<GameOverEvent>(OnGameOver);
            this.RegisterEvent<GameInitEndEvent>(OnGameInitEnd);
            this.RegisterEvent<DirInputEvent>(OnInputDir);
            this.RegisterEvent<EatFoodEvent>(OnFoodEat);
        }
        void ISnakeSystem.CreateSnake(int x, int y)
        {
            mCurSnake = new Snake();
            Bigger(x, y);
        }
        private void OnFoodEat(EatFoodEvent e) =>
            Bigger(e.x, e.y);
        private void OnGameInitEnd(GameInitEndEvent e) =>
            mAutoMoveTask = this.GetSystem<ITimeSystem>().AddDelayTask(0.3f, AutoMove, true);
        private void OnInputDir(DirInputEvent e) =>
            mCurSnake.GetMoveDir(e.hor, e.ver);
        private void OnGameOver(GameOverEvent e) =>
            mAutoMoveTask.StopTask();
        private void Bigger(int x, int y)
        {
            mCurSnake.Bigger(mBodyCount++);
            this.SendEvent(new SnakeBiggerEvent() { x = x, y = y, dir = mCurSnake.NextMoveDir });
        }
        private void AutoMove()
        {
            mMoveEvent.lastIndex = mCurSnake.TailIndex;
            mMoveEvent.headIndex = mCurSnake.HeadIndex;
            mMoveEvent.nextMove = mCurSnake.NextMoveDir;
            mCurSnake.Move();
            this.SendEvent(mMoveEvent);
        }
    }
}