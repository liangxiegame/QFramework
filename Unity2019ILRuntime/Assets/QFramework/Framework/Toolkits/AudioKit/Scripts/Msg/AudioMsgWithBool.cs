namespace QFramework
{
    public class AudioMsgWithBool : QMsg
    {
        public bool on;

        public AudioMsgWithBool(ushort eventId, bool on) : base(eventId)
        {
            this.on = on;
        }
    }
}