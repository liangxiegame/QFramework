#if (UNITY_IOS && !UNITY_EDITOR) || __IOS__

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Photon.Voice.IOS
{
    public class AudioInChangeNotifier : IAudioInChangeNotifier
    {
        public bool IsSupported => true;

        const string lib_name = "__Internal";
        [DllImport(lib_name)]
        private static extern IntPtr Photon_Audio_In_CreateChangeNotifier(int instanceID, Action<int> callback);
        [DllImport(lib_name)]
        private static extern IntPtr Photon_Audio_In_DestroyChangeNotifier(IntPtr handle);

        private delegate void CallbackDelegate(int instanceID);

        IntPtr handle;
        int instanceID;
        Action callback;

        public AudioInChangeNotifier(Action callback, ILogger logger)
        {
            this.callback = callback;
            var handle = Photon_Audio_In_CreateChangeNotifier(instanceCnt, nativeCallback);
            lock (instancePerHandle)
            {
                this.handle = handle;
                this.instanceID = instanceCnt;
                instancePerHandle.Add(instanceCnt++, this);
            }
        }

        public class MonoPInvokeCallbackAttribute : System.Attribute
        {
            private Type type;
            public MonoPInvokeCallbackAttribute(Type t) { type = t; }
        }

        // IL2CPP does not support marshaling delegates that point to instance methods to native code.
        // Using static method and per instance table.
        static int instanceCnt;
        private static Dictionary<int, AudioInChangeNotifier> instancePerHandle = new Dictionary<int, AudioInChangeNotifier>();
        [MonoPInvokeCallbackAttribute(typeof(CallbackDelegate))]
        private static void nativeCallback(int instanceID)
        {
            AudioInChangeNotifier instance;
            bool ok;
            lock (instancePerHandle)
            {
                ok = instancePerHandle.TryGetValue(instanceID, out instance);
            }
            if (ok)
            {
                instance.callback();
            }
        }

        /// <summary>If not null, the enumerator is in invalid state.</summary>
        public string Error { get; private set; }

        /// <summary>Disposes enumerator.
        /// Call it to free native resources.
        /// </summary>
        public void Dispose()
        {
            lock (instancePerHandle)
            {
                instancePerHandle.Remove(instanceID);
            }
            if (handle != IntPtr.Zero)
            {
                Photon_Audio_In_DestroyChangeNotifier(handle);
                handle = IntPtr.Zero;
            }
        }
    }
}
#endif
