namespace QFramework
{
    public class AudioMsgWithNode : QMsg
    {
        public IAction Node;

        public AudioMsgWithNode(IAction node) : base((int) AudioEvent.PlayNode)
        {
            Node = node;
        }
    }
}