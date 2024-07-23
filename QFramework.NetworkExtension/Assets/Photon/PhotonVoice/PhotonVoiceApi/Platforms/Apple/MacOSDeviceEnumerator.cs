#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Photon.Voice.MacOS
{
    /// <summary>Enumerates microphones available on device.
    /// </summary>
    public class AudioInEnumerator : DeviceEnumeratorBase
    {
        const string lib_name = "AudioIn";
        [DllImport(lib_name)]
        private static extern IntPtr Photon_Audio_In_CreateMicEnumerator();
        [DllImport(lib_name)]
        private static extern void Photon_Audio_In_DestroyMicEnumerator(IntPtr handle);
        [DllImport(lib_name)]
        private static extern int Photon_Audio_In_MicEnumerator_Count(IntPtr handle);
        [DllImport(lib_name)]
        private static extern IntPtr Photon_Audio_In_MicEnumerator_NameAtIndex(IntPtr handle, int idx);
        [DllImport(lib_name)]
        private static extern int Photon_Audio_In_MicEnumerator_IDAtIndex(IntPtr handle, int idx);

        IntPtr handle;
        public AudioInEnumerator(ILogger logger) : base(logger)
        {
            Refresh();
        }

        /// <summary>Refreshes the microphones list.
        /// </summary>
        public override void Refresh()
        {
            Dispose();
            try
            {
                handle = Photon_Audio_In_CreateMicEnumerator();
                var count = Photon_Audio_In_MicEnumerator_Count(handle);
                devices = new List<DeviceInfo>();
                for (int i = 0; i < count; i++)
                {
                    devices.Add(new DeviceInfo(Photon_Audio_In_MicEnumerator_IDAtIndex(handle, i), Marshal.PtrToStringAuto(Photon_Audio_In_MicEnumerator_NameAtIndex(handle, i))));
                }
                Error = null;
            }
            catch (Exception e)
            {
                Error = e.ToString();
                if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    Error = "Exception in AudioInEnumerator.Refresh()";
                }
            }
        }

        /// <summary>Disposes enumerator.
        /// Call it to free native resources.
        /// </summary>
        public override void Dispose()
        {
            if (handle != IntPtr.Zero && Error == null)
            {
                Photon_Audio_In_DestroyMicEnumerator(handle);
                handle = IntPtr.Zero;
            }
        }
    }
}
#endif
