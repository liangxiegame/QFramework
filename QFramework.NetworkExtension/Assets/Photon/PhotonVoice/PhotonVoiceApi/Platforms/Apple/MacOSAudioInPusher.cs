#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Photon.Voice.MacOS
{
    public class MonoPInvokeCallbackAttribute : System.Attribute
    {
        private Type type;
        public MonoPInvokeCallbackAttribute(Type t) { type = t; }
    }

    public class AudioInPusher : IAudioPusher<float>
    {
        const string lib_name = "AudioIn";
        [DllImport(lib_name)]
        private static extern IntPtr Photon_Audio_In_CreatePusher(int instanceID, int deviceID, Action<int, IntPtr, int> pushCallback);
        [DllImport(lib_name)]
        private static extern void Photon_Audio_In_Destroy(IntPtr handler);

        private delegate void CallbackDelegate(int instanceID, IntPtr buf, int len);

        public AudioInPusher(int deviceID, ILogger logger)
        {
            this.deviceID = deviceID;
            try
            {
                handle = Photon_Audio_In_CreatePusher(instanceCnt, deviceID, nativePushCallback);
                instancePerHandle.Add(instanceCnt++, this);
            }
            catch (Exception e)
            {
                Error = e.ToString();
                if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    Error = "Exception in AudioInPusher constructor";
                }
                logger.LogError("[PV] AudioInPusher: " + Error);
            }
        }

        private int deviceID;
        // IL2CPP does not support marshaling delegates that point to instance methods to native code.
        // Using static method and per instance table.
        static int instanceCnt;
        private static Dictionary<int, AudioInPusher> instancePerHandle = new Dictionary<int, AudioInPusher>();
        [MonoPInvokeCallbackAttribute(typeof(CallbackDelegate))]
        private static void nativePushCallback(int instanceID, IntPtr buf, int len)
        {
            AudioInPusher instance;
            if (instancePerHandle.TryGetValue(instanceID, out instance))
            {
                instance.push(buf, len);
            }
        }

        IntPtr handle;
        Action<float[]> pushCallback;
        ObjectFactory<float[], int> bufferFactory;

        // Supposed to be called once at voice initialization.
        // Otherwise recreate native object (instead of adding 'set callback' method to native interface)
        public void SetCallback(Action<float[]> callback, ObjectFactory<float[], int> bufferFactory)
        {
            this.bufferFactory = bufferFactory;
            this.pushCallback = callback;
        }
        private void push(IntPtr buf, int len)
        {            
            if (this.pushCallback != null)
            {
                var bufManaged = bufferFactory.New(len);
                Marshal.Copy(buf, bufManaged, 0, len);
                pushCallback(bufManaged);
            }
        }

        public int Channels { get { return 1; } }

		public int SamplingRate { get { return 44100; } }

        public string Error { get; private set; }

        public void Dispose()
        {
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