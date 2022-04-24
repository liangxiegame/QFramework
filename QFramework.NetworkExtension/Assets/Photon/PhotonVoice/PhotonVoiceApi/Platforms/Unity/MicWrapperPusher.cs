using UnityEngine;
using System;
using System.Linq;

namespace Photon.Voice.Unity
{
    public class MicWrapperPusher : IAudioPusher<float>
    {
        private AudioSource audioSource;
        private AudioClip mic;
        private string device;
        private ILogger logger;

        private AudioOutCapture audioOutCapture;

        private int sampleRate;
        private int channels;

        private bool destroyGameObjectOnStop;

        public MicWrapperPusher(string device, AudioSource aS, int suggestedFrequency, ILogger lg, bool destroyOnStop = true)
        {
            try
            {
                this.logger = lg;
                this.device = device;
                this.audioSource = aS;
                this.destroyGameObjectOnStop = destroyOnStop;
                if (UnityMicrophone.devices.Length < 1)
                {
                    this.Error = "No microphones found (Microphone.devices is empty)";
                    this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(device) && !UnityMicrophone.devices.Contains(device))
                {
                    this.logger.LogError("[PV] MicWrapperPusher: \"{0}\" is not a valid Unity microphone device, falling back to default one", device);
                    device = UnityMicrophone.devices[0];
                }
                this.sampleRate = AudioSettings.outputSampleRate;
                switch (AudioSettings.speakerMode)
                {
                    case AudioSpeakerMode.Mono: this.channels = 1; break;
                    case AudioSpeakerMode.Stereo: this.channels = 2; break;
                    default:
                        this.Error = string.Concat("Only Mono and Stereo project speaker mode supported. Current mode is ", AudioSettings.speakerMode);
                        this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
                        return;
                }
                int minFreq;
                int maxFreq;
                this.logger.LogInfo("[PV] MicWrapperPusher: initializing microphone '{0}', suggested frequency = {1}).", device, suggestedFrequency);
                UnityMicrophone.GetDeviceCaps(device, out minFreq, out maxFreq);
                int frequency = suggestedFrequency;
                //        minFreq = maxFreq = 44100; // test like android client
                if (suggestedFrequency < minFreq || maxFreq != 0 && suggestedFrequency > maxFreq)
                {
                    this.logger.LogWarning("[PV] MicWrapperPusher does not support suggested frequency {0} (min: {1}, max: {2}). Setting to {2}",
                        suggestedFrequency, minFreq, maxFreq);
                    frequency = maxFreq;
                }
                if (!this.audioSource.enabled)
                {
                    this.logger.LogWarning("[PV] MicWrapperPusher: AudioSource component disabled, enabling it.");
                    this.audioSource.enabled = true;
                }
                if (!this.audioSource.gameObject.activeSelf)
                {
                    this.logger.LogWarning("[PV] MicWrapperPusher: AudioSource GameObject inactive, activating it.");
                    this.audioSource.gameObject.SetActive(true);
                }
                if (!this.audioSource.gameObject.activeInHierarchy)
                {
                    this.Error = "AudioSource GameObject is not active in hierarchy, audio input can't work.";
                    this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
                    return;
                }
                this.audioOutCapture = this.audioSource.gameObject.GetComponent<AudioOutCapture>();
                if (ReferenceEquals(null, this.audioOutCapture) || !this.audioOutCapture)
                {
                    this.audioOutCapture = this.audioSource.gameObject.AddComponent<AudioOutCapture>();
                }
                if (!this.audioOutCapture.enabled)
                {
                    this.logger.LogWarning("[PV] MicWrapperPusher: AudioOutCapture component disabled, enabling it.");
                    this.audioOutCapture.enabled = true;
                }
                this.mic = UnityMicrophone.Start(device, true, 1, frequency);
                this.audioSource.mute = true;
                this.audioSource.volume = 0f;
                this.audioSource.clip = this.mic;
                this.audioSource.loop = true;
                this.audioSource.Play();
                this.logger.LogInfo("[PV] MicWrapperPusher: microphone '{0}' initialized, frequency = in:{1}|out:{2}, channels = in:{3}|out:{4}.", device, this.mic.frequency, this.SamplingRate, this.mic.channels, this.Channels);
            }
            catch (Exception e)
            {
                this.Error = e.ToString();
                if (this.Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    this.Error = "Exception in MicWrapperPusher constructor";
                }
                this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
            }
        }

        public MicWrapperPusher(string device, GameObject gO, int suggestedFrequency, ILogger lg, bool destroyOnStop = true)
        {
            try
            {
                this.logger = lg;
                this.device = device;
                this.destroyGameObjectOnStop = destroyOnStop;
                if (UnityMicrophone.devices.Length < 1)
                {
                    this.Error = "No microphones found (Microphone.devices is empty)";
                    this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(device) && !UnityMicrophone.devices.Contains(device))
                {
                    this.logger.LogError("[PV] MicWrapperPusher: \"{0}\" is not a valid Unity microphone device, falling back to default one", device);
                    device = UnityMicrophone.devices[0];
                }
                this.sampleRate = AudioSettings.outputSampleRate;
                switch (AudioSettings.speakerMode)
                {
                    case AudioSpeakerMode.Mono: this.channels = 1; break;
                    case AudioSpeakerMode.Stereo: this.channels = 2; break;
                    default:
                        this.Error = string.Concat("Only Mono and Stereo project speaker mode supported. Current mode is ", AudioSettings.speakerMode);
                        this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
                        return;
                }
                int minFreq;
                int maxFreq;
                this.logger.LogInfo("[PV] MicWrapperPusher: initializing microphone '{0}', suggested frequency = {1}).", device, suggestedFrequency);
                UnityMicrophone.GetDeviceCaps(device, out minFreq, out maxFreq);
                int frequency = suggestedFrequency;
                //        minFreq = maxFreq = 44100; // test like android client
                if (suggestedFrequency < minFreq || maxFreq != 0 && suggestedFrequency > maxFreq)
                {
                    this.logger.LogWarning("[PV] MicWrapperPusher does not support suggested frequency {0} (min: {1}, max: {2}). Setting to {2}",
                        suggestedFrequency, minFreq, maxFreq);
                    frequency = maxFreq;
                }
                if (!gO || gO == null)
                {
                    this.logger.LogWarning("[PV] MicWrapperPusher: AudioSource GameObject is destroyed or null. Creating a new one.");
                    gO = new GameObject("[PV] MicWrapperPusher: AudioSource + AudioOutCapture");
                    this.audioSource = gO.AddComponent<AudioSource>();
                    this.audioOutCapture = this.audioSource.gameObject.AddComponent<AudioOutCapture>();
                }
                else
                {
                    if (!gO.activeSelf)
                    {
                        this.logger.LogWarning("[PV] MicWrapperPusher: AudioSource GameObject inactive, activating it.");
                        gO.SetActive(true);
                    }
                    if (!gO.activeInHierarchy)
                    {
                        this.Error = "AudioSource GameObject is not active in hierarchy, audio input can't work.";
                        this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
                        return;
                    }
                    this.audioSource = gO.GetComponent<AudioSource>();
                    if (ReferenceEquals(null, this.audioSource) || !this.audioSource)
                    {
                        this.audioSource = gO.AddComponent<AudioSource>();
                    }
                    if (!this.audioSource.enabled)
                    {
                        this.logger.LogWarning("[PV] MicWrapperPusher: AudioSource component disabled, enabling it.");
                        this.audioSource.enabled = true;
                    }
                    if (!this.audioSource.gameObject.activeSelf)
                    {
                        this.logger.LogWarning("[PV] MicWrapperPusher: AudioSource GameObject inactive, activating it.");
                        this.audioSource.gameObject.SetActive(true);
                    }
                    if (!this.audioSource.gameObject.activeInHierarchy)
                    {
                        this.Error = "AudioSource GameObject is not active in hierarchy, audio input can't work.";
                        this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
                        return;
                    }
                    this.audioOutCapture = this.audioSource.gameObject.GetComponent<AudioOutCapture>();
                    if (ReferenceEquals(null, this.audioOutCapture) || !this.audioOutCapture)
                    {
                        this.audioOutCapture = this.audioSource.gameObject.AddComponent<AudioOutCapture>();
                    }
                    if (!this.audioOutCapture.enabled)
                    {
                        this.logger.LogWarning("[PV] MicWrapperPusher: AudioOutCapture component disabled, enabling it.");
                        this.audioOutCapture.enabled = true;
                    }
                }
                this.mic = UnityMicrophone.Start(device, true, 1, frequency);
                this.audioSource.mute = true;
                this.audioSource.volume = 0f;
                this.audioSource.clip = this.mic;
                this.audioSource.loop = true;
                this.audioSource.Play();
                this.logger.LogInfo("[PV] MicWrapperPusher: microphone '{0}' initialized, frequency = in:{1}|out:{2}, channels = in:{3}|out:{4}.", device, this.mic.frequency, this.SamplingRate, this.mic.channels, this.Channels);
            }
            catch (Exception e)
            {
                this.Error = e.ToString();
                if (this.Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    this.Error = "Exception in MicWrapperPusher constructor";
                }
                this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
            }
        }

        public MicWrapperPusher(string device, Transform parentTransform, int suggestedFrequency, ILogger lg, bool destroyOnStop = true)
        {
            try
            {
                this.logger = lg;
                this.device = device;
                this.destroyGameObjectOnStop = destroyOnStop;
                if (UnityMicrophone.devices.Length < 1)
                {
                    this.Error = "No microphones found (Microphone.devices is empty)";
                    this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(device) && !UnityMicrophone.devices.Contains(device))
                {
                    this.logger.LogError("[PV] MicWrapperPusher: \"{0}\" is not a valid Unity microphone device, falling back to default one", device);
                    device = UnityMicrophone.devices[0];
                }
                this.sampleRate = AudioSettings.outputSampleRate;
                switch (AudioSettings.speakerMode)
                {
                    case AudioSpeakerMode.Mono: this.channels = 1; break;
                    case AudioSpeakerMode.Stereo: this.channels = 2; break;
                    default:
                        this.Error = string.Concat("Only Mono and Stereo project speaker mode supported. Current mode is ", AudioSettings.speakerMode);
                        this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
                        return;
                }
                int minFreq;
                int maxFreq;
                this.logger.LogInfo("[PV] MicWrapperPusher: initializing microphone '{0}', suggested frequency = {1}).", device, suggestedFrequency);
                UnityMicrophone.GetDeviceCaps(device, out minFreq, out maxFreq);
                int frequency = suggestedFrequency;
                //        minFreq = maxFreq = 44100; // test like android client
                if (suggestedFrequency < minFreq || maxFreq != 0 && suggestedFrequency > maxFreq)
                {
                    this.logger.LogWarning("[PV] MicWrapperPusher does not support suggested frequency {0} (min: {1}, max: {2}). Setting to {2}",
                        suggestedFrequency, minFreq, maxFreq);
                    frequency = maxFreq;
                }
                GameObject gO = new GameObject("[PV] MicWrapperPusher: AudioSource + AudioOutCapture");
                if (ReferenceEquals(null, parentTransform) || !parentTransform)
                {
                    this.logger.LogWarning("[PV] MicWrapperPusher: Parent transform passed is destroyed or null. Creating AudioSource GameObject at root.");
                }
                else
                {
                    gO.transform.SetParent(parentTransform, false);
                    if (!gO.activeSelf)
                    {
                        this.logger.LogWarning("[PV] MicWrapperPusher: AudioSource GameObject inactive, activating it.");
                        gO.gameObject.SetActive(true);
                    }
                    if (!gO.activeInHierarchy)
                    {
                        this.Error = "AudioSource GameObject is not active in hierarchy, audio input can't work.";
                        this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
                        return;
                    }
                }
                this.audioSource = gO.AddComponent<AudioSource>();
                this.audioOutCapture = this.audioSource.gameObject.AddComponent<AudioOutCapture>();
                this.mic = UnityMicrophone.Start(device, true, 1, frequency);
                this.audioSource.mute = true;
                this.audioSource.volume = 0f;
                this.audioSource.clip = this.mic;
                this.audioSource.loop = true;
                this.audioSource.Play();
                this.logger.LogInfo("[PV] MicWrapperPusher: microphone '{0}' initialized, frequency = in:{1}|out:{2}, channels = in:{3}|out:{4}.", device, this.mic.frequency, this.SamplingRate, this.mic.channels, this.Channels);
            }
            catch (Exception e)
            {
                this.Error = e.ToString();
                if (this.Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    this.Error = "Exception in MicWrapperPusher constructor";
                }
                this.logger.LogError("[PV] MicWrapperPusher: {0}", this.Error);
            }
        }

        private float[] frame2 = new float[0];

        private void AudioOutCaptureOnOnAudioFrame(float[] frame, int channelsNumber)
        {
            if (channelsNumber != this.Channels)
            {
                this.logger.LogWarning("[PV] MicWrapperPusher: channels number mismatch; expected:{0} got:{1}.", this.Channels, channelsNumber);
            }
            if (this.frame2.Length != frame.Length)
            {
                this.frame2 = new float[frame.Length];
            }
            Array.Copy(frame, this.frame2, frame.Length);
            this.pushCallback(frame);
            Array.Clear(frame, 0, frame.Length);
        }

        private Action<float[]> pushCallback;


        public void SetCallback(Action<float[]> callback, ObjectFactory<float[], int> bufferFactory)
        {
            this.pushCallback = callback;
            this.audioOutCapture.OnAudioFrame += this.AudioOutCaptureOnOnAudioFrame;
        }

        public void Dispose()
        {
            if (this.pushCallback != null && this.audioOutCapture != null)
            {
                this.audioOutCapture.OnAudioFrame -= this.AudioOutCaptureOnOnAudioFrame;
            }
            UnityMicrophone.End(this.device);
            if (this.destroyGameObjectOnStop && this.audioSource != null)
            {
                UnityEngine.Object.Destroy(this.audioSource.gameObject);
            }
        }

        public int SamplingRate { get { return this.Error == null ? this.sampleRate : 0; } }
        public int Channels { get { return this.Error == null ? this.channels : 0; } }
        public string Error { get; private set; }

    }
}