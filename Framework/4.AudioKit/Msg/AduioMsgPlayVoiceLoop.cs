using UnityEngine.Events;

namespace QFramework
{
    public class AduioMsgPlayVoiceLoop : QMsg
    {
        public string VoiceName;
        public UnityAction OnVoiceBeganCallback;
        public UnityAction OnVoiceEndedCallback;
    }
}