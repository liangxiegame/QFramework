#if PHOTON_VOICE_FMOD_ENABLE
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMODLib = FMOD;

namespace Photon.Voice.FMOD
{
    public class AudioInReader<T> : IAudioReader<T>
    {
        readonly int sizeofT = Marshal.SizeOf(default(T));

        const int BUF_LENGTH_MS = 2000;

        const string LOG_PREFIX = "[PV] [FMOD] AudioIn: ";

        private int device;
        ILogger logger;

        FMODLib.System coreSystem;                
        FMODLib.Sound sound;
        readonly FMODLib.SOUND_FORMAT soundFormat;
        public bool isRecording;
        int samplingRate;
        int channels;
        int bufLengthSamples;
        

        public AudioInReader(FMODLib.System coreSystem, int device, int suggestedFrequency, ILogger logger)
        {
            if (sizeofT == 2)// (typeof(T) == typeof(short)) // sometimes T is Int16 even if short passed, checking size is more reliable
            {
                soundFormat = FMODLib.SOUND_FORMAT.PCM16;
            }
            else if (sizeofT == 4)// (typeof(T) == typeof(float))
            {
                soundFormat = FMODLib.SOUND_FORMAT.PCMFLOAT;
            }
            else
            {
                Error = "only float and short buffers are supported: " + typeof(T);
                logger.LogError(LOG_PREFIX + Error);
                return;
            }

            try
            {
                if (device == -1) // default device
                {
                    device = 0;
                }
                FMODLib.RESULT res;

                this.coreSystem = coreSystem;
                this.device = device;
                this.logger = logger;

                // use given frequency, fmod will resample
                this.samplingRate = suggestedFrequency;

                // set this.channels to driver's value
                res = this.coreSystem.getRecordDriverInfo(device, out string name, 1, out Guid guid, out int systemrate, out FMODLib.SPEAKERMODE speakermode, out this.channels, out FMODLib.DRIVER_STATE state);
                if (res != FMODLib.RESULT.OK)
                {
                    Error = "failed to getRecordDriverInfo: " + res;
                    logger.LogError(LOG_PREFIX + Error);
                    return;
                }

                FMODLib.CREATESOUNDEXINFO exinfo = new FMODLib.CREATESOUNDEXINFO();
                exinfo.cbsize = Marshal.SizeOf(exinfo);
                exinfo.numchannels = channels;
                exinfo.format = soundFormat;
                exinfo.defaultfrequency = samplingRate;
                bufLengthSamples = samplingRate * BUF_LENGTH_MS / 1000;
                exinfo.length = (uint)(bufLengthSamples * channels * sizeofT);
                
                FMODLib.MODE soundMode = FMODLib.MODE.OPENUSER | FMODLib.MODE.LOOP_NORMAL;
                res = this.coreSystem.createSound("Photon AudioIn", soundMode, ref exinfo, out sound);

                if (res != FMODLib.RESULT.OK)
                {
                    Error = "failed to createSound: " + res;
                    logger.LogError(LOG_PREFIX + Error);
                    return;
                }

                res = this.coreSystem.recordStart(0, sound, true);

                if (res != FMODLib.RESULT.OK)
                {
                    Error = "failed to startrecord: " + res;
                    logger.LogError(LOG_PREFIX + Error);
                    return;
                }
                else
                {
                    isRecording = true;
                }

                //test play
                //this.coreSystem.playSound(sound, channelGroup, false, out channel);

                logger.LogInfo("[PV] [FMOD] Mic: microphone '{0}' initialized, frequency = {1}, channels = {2}.", device, samplingRate, channels);
            }
            catch (Exception e)
            {
                Error = e.ToString();
                if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    Error = "Exception in [FMOD] Mic constructor";
                }
                logger.LogError(LOG_PREFIX + Error);
            }
        }

        public int SamplingRate { get { return Error == null ? this.samplingRate : 0; } }
        public int Channels { get { return Error == null ? this.channels : 0; } }
        public string Error { get; private set; }

        public void Dispose()
        {
            this.coreSystem.recordStop(device);
            sound.release();
        }

        private uint micPrevPos;
        private int micLoopCnt;
        private uint readAbsPos; // pos in sample (sample size in bytes = sizeofT * channels)

        public bool Read(T[] readBuf)
        {
            if (Error != null)
            {
                return false;
            }
            uint micPos;
            FMODLib.RESULT res = this.coreSystem.getRecordPosition(0, out micPos);
            if (res != FMODLib.RESULT.OK)
            {
                Error = "failed to getRecordPosition: " + res;
                logger.LogError(LOG_PREFIX + Error);
                return false;
            }

            // loop detection
            if (micPos < micPrevPos)
            {
                micLoopCnt++;
            }
            micPrevPos = micPos;

            var micAbsPos = micLoopCnt * bufLengthSamples + micPos;

            var nextReadPos = this.readAbsPos + readBuf.Length / channels;
            if (nextReadPos < micAbsPos)
            {
                IntPtr ptr1, ptr2;
                uint len1, len2;
                res = sound.@lock((uint)(this.readAbsPos % bufLengthSamples * sizeofT * channels), (uint)(readBuf.Length * sizeofT), out ptr1, out ptr2, out len1, out len2);
                if (res != FMODLib.RESULT.OK)
                {
                    Error = "failed to lock sound buffer: " + res;
                    logger.LogError(LOG_PREFIX + Error);
                    return false;
                }

                int len1T = (int)len1 / sizeofT;
                int len2T = (int)len2 / sizeofT;
                if (soundFormat == FMODLib.SOUND_FORMAT.PCM16)
                {
                    Marshal.Copy(ptr1, readBuf as short[], 0, len1T);
                    if (ptr2 != IntPtr.Zero)
                    {
                        Marshal.Copy(ptr2, readBuf as short[], len1T, len2T);
                    }
                }
                else if (soundFormat == FMODLib.SOUND_FORMAT.PCMFLOAT)
                {
                    Marshal.Copy(ptr1, readBuf as float[], 0, len1T);
                    if (ptr2 != IntPtr.Zero)
                    {
                        Marshal.Copy(ptr2, readBuf as float[], len1T, len2T);
                    }
                }

                res = sound.unlock(ptr1, ptr2, len1, len2);
                if (res != FMODLib.RESULT.OK)
                {
                    Error = "failed to unlock sound buffer: " + res;
                    logger.LogError(LOG_PREFIX + Error);
                    return false;
                }

                this.readAbsPos = (uint)nextReadPos;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
#endif