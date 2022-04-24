namespace QFramework.Example
{
    // 消息 ID 定义
    public static class EnemyEvent
    {
        public enum SkillEvent
        {
            Start = MgrID.Enemy,
            Play,
            Stop,
            End,
        }
        
        public enum MovementEvent
        {
            Start = SkillEvent.End,
            Arrived,
            End
        }
    }
    
    // 消息体 (需要携带参数的时候要用消息体)
    public class EnemySkillPlay : QMsg
    {
        public string EnemyId { get; set; }

        public string SkillName { get; set; }

        public EnemySkillPlay() : base((int) EnemyEvent.SkillEvent.Play)
        {

        }
    }
}