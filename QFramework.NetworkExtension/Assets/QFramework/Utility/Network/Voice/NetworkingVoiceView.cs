using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

namespace QFramework.NetworkExtension
{
    [RequireComponent(typeof(PhotonVoiceView))]
    public class NetworkingVoiceView : MonoBehaviour
    {
        private PhotonVoiceView voiceView;
        private Speaker speaker;
        private AudioSource mAudio;
        public GameObject VoiceObj;
        public void Awake()
        {
            voiceView = gameObject.GetComponent<PhotonVoiceView>();
        }

        private void Start()
        {
            VoiceObj = this.transform.Find("PlayerController").gameObject;
            VoiceObj.AddComponent<Speaker>();

            speaker = VoiceObj.GetComponent<Speaker>();

            voiceView.SpeakerInUse = speaker;
            voiceView.UsePrimaryRecorder = true;
            voiceView.SetupDebugSpeaker = true;

            mAudio = VoiceObj.GetComponent<AudioSource>();
            mAudio.spatialBlend = 1;//3D音效
            mAudio.minDistance = 1;
            mAudio.maxDistance = 6;
            mAudio.rolloffMode = AudioRolloffMode.Linear;
        }

        public void SetSpeaker(bool isOn)
        {
            if (isOn)
            {
                speaker.StartPlayback();
            }
            else
            {
                speaker.StopPlayback();
            }
        }

        public AudioSource GetAudioSource
        {
            get { return mAudio; }
        }
    }
}