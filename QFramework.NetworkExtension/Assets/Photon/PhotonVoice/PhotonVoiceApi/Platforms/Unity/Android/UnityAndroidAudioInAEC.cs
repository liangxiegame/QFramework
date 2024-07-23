using System;
using UnityEngine;

namespace Photon.Voice.Unity
{
    public class AndroidAudioInParameters
    {
        public bool EnableAEC = false;
        public bool EnableAGC = false;
        public bool EnableNS = false;
    }

    // depends on Unity's AndroidJavaProxy
    public class AndroidAudioInAEC : Voice.IAudioPusher<short>, IResettable
    {
        class DataCallback : AndroidJavaProxy
        {
            Action<short[]> callback;
            IntPtr javaBuf;
            public DataCallback() : base("com.exitgames.photon.audioinaec.AudioInAEC$DataCallback") { }
            public void SetCallback(Action<short[]> callback, IntPtr javaBuf)
            {
                this.callback = callback;
                this.javaBuf = javaBuf;
            }
            public void OnData()
            {
                if (callback != null)
                {
                    //TODO: copy to LocalVoiceFramed.PushDataBufferPool element instead
                    var buf = AndroidJNI.FromShortArray(javaBuf);
                    cntFrame++;
                    cntShort += buf.Length;
                    this.callback(buf);
                }
            }
            public void OnStop()
            {
                AndroidJNI.DeleteGlobalRef(javaBuf);
            }
            int cntFrame;
            int cntShort;

        }

        AndroidJavaObject audioIn;
        IntPtr javaBuf;
        Voice.ILogger logger;
        int audioInSampleRate = 0;

        public AndroidAudioInAEC(Voice.ILogger logger, bool enableAEC = false, bool enableAGC = false, bool enableNS = false)
        {
            // true means to use a route-dependent value which is usually the sample rate of the source
            // otherwise, 44100 Hz requested
            // On Android 4.4.4 (probably on all < 6.0), auto does not work: java.lang.IllegalArgumentException: 0Hz is not a supported sample rate.
            const bool SAMPLE_RATE_AUTO = false; 
            
            // 44100Hz is currently the only rate that is guaranteed to work on all devices 
            // used for GetMinBufferSize call even if SAMPLE_RATE_AUTO = true
            const int SAMPLE_RATE_44100 = 44100;
            const int SAMPLE_RATE_UNSPECIFIED = 0;
            const int SAMPLE_RATE_REQUEST = SAMPLE_RATE_AUTO ? SAMPLE_RATE_UNSPECIFIED : SAMPLE_RATE_44100;

            this.logger = logger;
            try
            {
                this.callback = new DataCallback();
                audioIn = new AndroidJavaObject("com.exitgames.photon.audioinaec.AudioInAEC");
                //bool aecAvailable = audioIn.Call<bool>("AECIsAvailable");
                int minBufSize = audioIn.Call<int>("GetMinBufferSize", SAMPLE_RATE_44100, Channels);
                logger.LogInfo("[PV] AndroidAudioInAEC: AndroidJavaObject created: aec: {0}/{1}, agc: {2}/{3}, ns: {4}/{5} minBufSize: {6}", 
                    enableAEC, audioIn.Call<bool>("AECIsAvailable"),
                    enableAGC, audioIn.Call<bool>("AGCIsAvailable"),
                    enableNS, audioIn.Call<bool>("NSIsAvailable"),
                    minBufSize);

                AndroidJavaClass app = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = app.GetStatic<AndroidJavaObject>("currentActivity");
                // Set buffer IntPtr reference separately via pure jni call, pass other values and start capture via AndroidJavaObject helper

                var ok = audioIn.Call<bool>("Start", activity, this.callback, SAMPLE_RATE_REQUEST, Channels, minBufSize * 4, enableAEC, enableAGC, enableNS);
                if (ok)
                {
                    audioInSampleRate = audioIn.Call<int>("GetSampleRate");
                    logger.LogInfo("[PV] AndroidAudioInAEC: AndroidJavaObject started: {0}, sampling rate: {1}, channels: {2}, record buffer size: {3}", ok, SamplingRate, Channels, minBufSize * 4);
                }
                else
                {
                    Error = "[PV] AndroidAudioInAEC constructor: calling Start java method failure";
                    logger.LogError("[PV] AndroidAudioInAEC: {0}", Error);
                }                
            }
            catch (Exception e)
            {
                Error = e.ToString();
                if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    Error = "Exception in AndroidAudioInAEC constructor";
                }
                logger.LogError("[PV] AndroidAudioInAEC: {0}", Error);
            }
        }

        // Supposed to be called once at voice initialization.
        // Otherwise recreate native object (instead of adding 'set callback' method to java interface)
        public void SetCallback(Action<short[]> callback, ObjectFactory<short[], int> bufferFactory)
        {
            if (Error == null)
            {
                var voiceFrameSize = bufferFactory.Info;
                // setting to voice FrameSize lets to avoid framing procedure
                javaBuf = AndroidJNI.NewGlobalRef(AndroidJNI.NewShortArray(voiceFrameSize));
                this.callback.SetCallback(callback, javaBuf);
                var meth = AndroidJNI.GetMethodID(audioIn.GetRawClass(), "SetBuffer", "([S)Z");
                bool ok = AndroidJNI.CallBooleanMethod(audioIn.GetRawObject(), meth, new jvalue[] { new jvalue() { l = javaBuf } });
                if (!ok)
                {
                    Error = "AndroidAudioInAEC.SetCallback(): calling SetBuffer java method failure";
                }
            }
            if (Error != null)
            {
                logger.LogError("[PV] AndroidAudioInAEC: {0}", Error);
            }
        }

        DataCallback callback;

        public int Channels { get { return 1; } }

        public int SamplingRate { get { return audioInSampleRate; } }

        public string Error { get; private set; }

        public void Reset()
        {
            if (audioIn != null)
            {
                audioIn.Call("Reset");
            }
        }

        public void Dispose()
        {
            if (audioIn != null)
            {
                audioIn.Call<bool>("Stop");
            }
        }
    }
}