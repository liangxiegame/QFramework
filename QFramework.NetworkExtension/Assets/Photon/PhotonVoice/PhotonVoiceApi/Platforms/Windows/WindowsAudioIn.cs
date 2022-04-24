#if PHOTON_VOICE_WINDOWS || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Photon.Voice.Windows
{
#pragma warning disable 0414
    public class MonoPInvokeCallbackAttribute : System.Attribute
    {
        private Type type;
        public MonoPInvokeCallbackAttribute(Type t) { type = t; }
    }
#pragma warning restore 0414

    public class WindowsAudioInPusher : IAudioPusher<short>
    {
        enum SystemMode
        {
            SINGLE_CHANNEL_AEC = 0,
            OPTIBEAM_ARRAY_ONLY = 2,
            OPTIBEAM_ARRAY_AND_AEC = 4,
            SINGLE_CHANNEL_NSAGC = 5,
        }

        [DllImport("AudioIn")]
        private static extern IntPtr Photon_Audio_In_Create(int instanceID, SystemMode systemMode, int micDevIdx, int spkDevIdx, Action<int, IntPtr, int> callback, bool featrModeOn, bool noiseSup, bool agc, bool cntrClip);

        [DllImport("AudioIn")]
        private static extern void Photon_Audio_In_Destroy(IntPtr handler);

        private delegate void CallbackDelegate(int instanceID, IntPtr buf, int len);

        IntPtr handle;
        int instanceID;
        Action<short[]> pushCallback;
        ObjectFactory<short[], int> bufferFactory;

        public WindowsAudioInPusher(int deviceID, ILogger logger)
        {
            try
            {
                lock (instancePerHandle)
                {
                    // use default playback device
                    handle = Photon_Audio_In_Create(instanceCnt, SystemMode.SINGLE_CHANNEL_AEC, deviceID, -1, nativePushCallback, true, true, true, true); // defaults in original ms sample: false, true, false, false
                    this.instanceID = instanceCnt;
                    instancePerHandle.Add(instanceCnt++, this);
                }
            }
            catch (Exception e)
            {
                Error = e.ToString();
                if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    Error = "Exception in WindowsAudioInPusher constructor";
                }
                logger.LogError("[PV] WindowsAudioInPusher: " + Error);
            }
        }

        // IL2CPP does not support marshaling delegates that point to instance methods to native code.
        // Using static method and per instance table.
        static int instanceCnt;
        private static Dictionary<int, WindowsAudioInPusher> instancePerHandle = new Dictionary<int, WindowsAudioInPusher>();
        [MonoPInvokeCallbackAttribute(typeof(CallbackDelegate))]
        private static void nativePushCallback(int instanceID, IntPtr buf, int len)
        {
            WindowsAudioInPusher instance;
            bool ok;
            lock (instancePerHandle)
            {
                ok = instancePerHandle.TryGetValue(instanceID, out instance);
            }
            if (ok)
            {
                instance.push(buf, len);
            }
        }

        // Supposed to be called once at voice initialization.
        // Otherwise recreate native object (instead of adding 'set callback' method to native interface)
        public void SetCallback(Action<short[]> callback, ObjectFactory<short[], int> bufferFactory)
        {
            this.bufferFactory = bufferFactory;            
            this.pushCallback = callback;
        }

        private void push(IntPtr buf, int lenBytes)
        {
            if (pushCallback != null)
            {
                var len = lenBytes / sizeof(short);
                var bufManaged = this.bufferFactory.New(len);
                Marshal.Copy(buf, bufManaged, 0, len);
                pushCallback(bufManaged);
            }
        }

        public int Channels { get { return 1; } }

        // Hardcoded in AudioInAec.cpp
        // Supported sample rates: 8000, 11025, 16000, 22050
        // https://docs.microsoft.com/en-us/windows/win32/medfound/voicecapturedmo?redirectedfrom=MSDN (Voice Capture DSP)
        public int SamplingRate { get { return 16000; } }

        public string Error { get; private set; }

        public void Dispose()
        {
            lock (instancePerHandle)
            {
                instancePerHandle.Remove(instanceID);
            }
            if (handle != IntPtr.Zero)
            {
                Photon_Audio_In_Destroy(handle);
                handle = IntPtr.Zero;
            }
            // TODO: Remove this from instancePerHandle
        }
    }
}
#endif