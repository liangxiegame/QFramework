#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System;
using System.Runtime.InteropServices;

namespace Photon.Voice.MacOS
{
    public class AudioInReader : IAudioReader<float>
    {
        const string lib_name = "AudioIn";
        [DllImport(lib_name)]
        private static extern IntPtr Photon_Audio_In_CreateReader(int deviceID);
        [DllImport(lib_name)]
        private static extern void Photon_Audio_In_Destroy(IntPtr handler);
        [DllImport(lib_name)]
        private static extern bool Photon_Audio_In_Read(IntPtr handle, float[] buf, int len);

        IntPtr audioIn;

        public AudioInReader(int deviceID, ILogger logger)
        {
            try
            {
                audioIn = Photon_Audio_In_CreateReader(deviceID);
            }
            catch (Exception e)
            {
                Error = e.ToString();
                if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    Error = "Exception in AudioInReader constructor";
                }
                logger.LogError("[PV] AudioInReader: " + Error);
            }
        }
        public int Channels { get { return 1; } }

        public int SamplingRate { get { return 44100; } }

        public string Error { get; private set; }

        public void Dispose()
        {
            if (audioIn != IntPtr.Zero)
            {
                Photon_Audio_In_Destroy(audioIn);
                audioIn = IntPtr.Zero;
            }
        }

        public bool Read(float[] buf)
        {
            return audioIn != IntPtr.Zero && Photon_Audio_In_Read(audioIn, buf, buf.Length);
        }
    }
}
#endif
