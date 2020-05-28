namespace QFramework
{
    /// <summary>
    /// GameState 包含游戏的状态，其中可以包括联网玩家列表、得分、棋类游戏中棋子的位置，或者在开放世界场景中完成的任务列表。
    /// 游戏状态存在于服务器和所有客户端上，可以自由复制以保持所有机器处于最新状态。
    /// </summary>
    public class GameState : IGameState
    {
        public bool IsStarting { get; set; }
    }
}