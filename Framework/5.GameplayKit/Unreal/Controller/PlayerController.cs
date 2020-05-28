namespace QFramework
{
    /// <summary>
    /// PlayerController(玩家控制器) 是Pawn和控制Pawn的人类玩家之间的界面。玩家控制器基本上代表人类玩家的意愿。
    /// </summary>
    public class PlayerController : Controller
    {
        public PlayerController()
        {
            InitPlayerInputManager();
            InitPlayerHUDManager();
            InitPlayerHUDManager();
        }

        public PlayerInputManager PlayerInputManager { get; protected set; }

        public PlayerHUDManager PlayerHudManager { get; protected set; }

        public PlayerCameraManager PlayerCameraManager { get; protected set; }


        protected virtual void InitPlayerInputManager()
        {
            PlayerInputManager = new PlayerInputManager();
        }

        protected virtual void InitPlayerHUDManager()
        {
            PlayerHudManager = new PlayerHUDManager();
        }

        protected virtual void InitPlayerCameraManager()
        {
            PlayerCameraManager = new PlayerCameraManager();
        }
    }
}