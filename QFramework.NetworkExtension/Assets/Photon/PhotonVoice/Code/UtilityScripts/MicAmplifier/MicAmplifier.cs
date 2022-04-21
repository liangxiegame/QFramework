using UnityEngine;

namespace Photon.Voice.Unity.UtilityScripts
{
    [RequireComponent(typeof(Recorder))]
    public class MicAmplifier : VoiceComponent
    {
        [SerializeField]
        private float boostValue;

        [SerializeField]
        private float amplificationFactor = 1f;

        public float AmplificationFactor
        {
            get { return this.amplificationFactor; }
            set
            {
                if (this.amplificationFactor.Equals(value))
                {
                    return;
                }
                this.amplificationFactor = value;
                if (this.floatProcessor != null)
                {
                    this.floatProcessor.AmplificationFactor = this.amplificationFactor;
                }
                if (this.shortProcessor != null)
                {
                    this.shortProcessor.AmplificationFactor = (short)this.amplificationFactor;
                }
            }
        }

        public float BoostValue
        {
            get { return this.boostValue; }
            set
            {
                if (this.boostValue.Equals(value))
                {
                    return;
                }
                this.boostValue = value;
                if (this.floatProcessor != null)
                {
                    this.floatProcessor.BoostValue = this.boostValue;
                }
                if (this.shortProcessor != null)
                {
                    this.shortProcessor.BoostValue = (short)this.boostValue;
                }
            }
        }

        private MicAmplifierFloat floatProcessor;
        private MicAmplifierShort shortProcessor;

        private void OnEnable()
        {
            if (this.floatProcessor != null)
            {
                this.floatProcessor.Disabled = false;
            }
            if (this.shortProcessor != null)
            {
                this.shortProcessor.Disabled = false;
            }
        }

        private void OnDisable()
        {
            if (this.floatProcessor != null)
            {
                this.floatProcessor.Disabled = true;
            }
            if (this.shortProcessor != null)
            {
                this.shortProcessor.Disabled = true;
            }
        }

        // Message sent by Recorder
        private void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
        {
            if (p.Voice is LocalVoiceAudioFloat)
            {
                LocalVoiceAudioFloat v = p.Voice as LocalVoiceAudioFloat;
                this.floatProcessor = new MicAmplifierFloat(this.AmplificationFactor, this.BoostValue);
                v.AddPostProcessor(this.floatProcessor);
            }
            else if (p.Voice is LocalVoiceAudioShort)
            {
                LocalVoiceAudioShort v = p.Voice as LocalVoiceAudioShort;
                this.shortProcessor = new MicAmplifierShort((short)this.AmplificationFactor, (short)this.BoostValue);
                //this.shortProcessor = new SimpleAmplifierShortProcessor((short)(this.AmplificationFactor* short.MaxValue), (short)(this.boostValue * short.MaxValue));
                v.AddPostProcessor(this.shortProcessor);
            }
            else if (this.Logger.IsErrorEnabled)
            {
                this.Logger.LogError("LocalVoice object has unexpected value/type: {0}", p.Voice == null ? "null" : p.Voice.GetType().ToString());
            }
        }
    }
}