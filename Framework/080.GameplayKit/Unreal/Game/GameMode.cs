namespace QFramework
{
    /// <summary>
    /// “游戏”的概念分为两类。Game Mode 和 Game State 是游戏的定义，包括游戏规则和获胜条件等内容。
    /// 在联网游戏中它仅存在于服务器上，在单机游戏中是存储在本地的。
    /// 它通常不应有太多在游戏过程中会发生变化的数据，也绝对不应有客户端需要了解的临时数据。
    /// </summary>
    public class GameMode : IGameMode
    {
        public GameState GameState { get; protected set; }

        public PlayerState PlayerState { get; protected set; }

        public GameMode()
        {
            InitGameState();
            InitPlayerState();
        }
        
        protected virtual void InitGameState()
        {
            GameState = new GameState();
        }

        protected virtual void InitPlayerState()
        {
            PlayerState = new PlayerState();
        }
    }
}