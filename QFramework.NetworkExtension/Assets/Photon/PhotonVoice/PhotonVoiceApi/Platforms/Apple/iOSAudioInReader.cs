#if (UNITY_IOS && !UNITY_EDITOR) || __IOS__
using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace Photon.Voice.IOS
{
    public class AudioInReader : IAudioReader<float>, IResettable
    {
        const string lib_name = "__Internal";
        [DllImport(lib_name)]
        private static extern IntPtr Photon_Audio_In_CreateReader(int sessionCategory, int sessionMode, int sessionCategoryOptions);
        [DllImport(lib_name)]
        private static extern void Photon_Audio_In_Reset(IntPtr handler);
        [DllImport(lib_name)]
        private static extern void Photon_Audio_In_Destroy(IntPtr handler);
        [DllImport(lib_name)]
        private static extern bool Photon_Audio_In_Read(IntPtr handle, float[] buf, int len);

        IntPtr audioIn;
        private bool initializationFinished;

        public AudioInReader(AudioSessionParameters sessParam, ILogger logger)
        {
            // initialization in a separate thread to avoid 0.5 - 1 sec. pauses in main thread execution
            var t = new Thread(() =>
            {
                lock (this)
                {
                    try
                    {
                        var audioIn = Photon_Audio_In_CreateReader((int)sessParam.Category, (int)sessParam.Mode, sessParam.CategoryOptionsToInt());
                        lock (this)
                        {
                            this.audioIn = audioIn;
                        }
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
                    finally
                    {
                        initializationFinished = true;
                    }
                }
            });
            Util.SetThreadName(t, "[PV] IOSAudioInReaderCtr");
            t.Start();
        }
        public int Channels { get { return 1; } }

        public int SamplingRate { get { return 48000; } }

        public string Error { get; private set; }

        public void Reset()
        {
            lock (this)
             {
                if (audioIn != IntPtr.Zero)
                {
                    Photon_Audio_In_Reset(audioIn);
                }
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                while (!initializationFinished) // should never happen because of lock if the thread in constructor started before Dispose() call
                {
                    Thread.Sleep(1);
                }

                if (audioIn != IntPtr.Zero)
                {
                    Photon_Audio_In_Destroy(audioIn);
                    audioIn = IntPtr.Zero;
                }
            }
        }

        public bool Read(float[] buf)
        {
            return audioIn != IntPtr.Zero && Photon_Audio_In_Read(audioIn, buf, buf.Length);
        }
    }
}
#endif