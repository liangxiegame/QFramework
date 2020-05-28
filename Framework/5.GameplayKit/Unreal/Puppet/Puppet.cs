namespace QFramework
{
    /// <summary>
    /// 提线木偶
    /// Pawn 是可作为世界场景中“代理”的Actor。Pawn可被控制器所有，且可将其设置为易于接受输入，用于执行各种各样类似于玩家的任务。请注意，Pawn不被认定为具有人的特性。
    /// </summary>
    public class Puppet : IPuppet
    {
        public Puppet()
        {
        }

        public void Dispose()
        {

        }

        public Controller Controller { get; set; }
    }
}