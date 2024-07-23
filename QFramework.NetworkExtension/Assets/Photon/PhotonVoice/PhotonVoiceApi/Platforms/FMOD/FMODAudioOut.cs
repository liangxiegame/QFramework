#if PHOTON_VOICE_FMOD_ENABLE
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMODLib = FMOD;

namespace Photon.Voice.FMOD
{
    // Plays back input audio via FMOD Sound
    public class AudioOut<T> :  AudioOutDelayControl<T>
    {
        protected readonly int sizeofT = Marshal.SizeOf(default(T));

        FMODLib.System coreSystem;
        FMODLib.Sound sound;
        FMODLib.Channel channel;
        FMODLib.SOUND_FORMAT soundFormat;

        public FMODLib.Sound Sound { get { return sound; } }
        public FMODLib.Channel Channel { get { return channel; } }


        public AudioOut(FMODLib.System coreSystem, PlayDelayConfig playDelayConfig, ILogger logger, string logPrefix, bool debugInfo)
            : base(false, playDelayConfig, logger, "[PV] [FMOD] AudioOut" + (logPrefix == "" ? "" : " ") + logPrefix + " ", debugInfo)

        {
            if (sizeofT == 2)// (typeof(T) == typeof(short)) // sometimes T is Int16 even if short passed, checking is more reliable
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
                logger.LogError(logPrefix + Error);
                return;
            }
            this.coreSystem = coreSystem;
        }

        override public void OutCreate(int samplingRate, int channels, int bufferSamples)
        {
            FMODLib.RESULT res;
            FMODLib.CREATESOUNDEXINFO exinfo = new FMODLib.CREATESOUNDEXINFO();
            exinfo.cbsize = Marshal.SizeOf(exinfo);
            exinfo.numchannels = channels;
            exinfo.format = soundFormat;
            exinfo.defaultfrequency = samplingRate;
            exinfo.length = (uint)(bufferSamples * channels * sizeofT);

            FMODLib.MODE soundMode = FMODLib.MODE.OPENUSER | FMODLib.MODE.LOOP_NORMAL;
            res = coreSystem.createSound("Photon AudioOut", soundMode, ref exinfo, out sound);
            if (res != FMODLib.RESULT.OK)
            {
                Error = "failed to createSound: " + res;
                logger.LogError(logPrefix + Error);
                return;
            }

            logger.LogInfo(logPrefix + "Sound Created" + sound.handle);
        }

        override public void OutStart()
        {
            FMODLib.ChannelGroup master;
            coreSystem.getMasterChannelGroup(out master);
            FMODLib.RESULT res = coreSystem.playSound(sound, master, false, out channel);
            if (res != FMODLib.RESULT.OK)
            {
                Error = "failed to playSound: " + res;
                logger.LogError(logPrefix + Error);
                return;
            }
        }

        override public int OutPos 
        { 
            get 
            {
                channel.getPosition(out uint pos, FMODLib.TIMEUNIT.PCMBYTES);
                return (int)(pos / channels / sizeofT); 
            } 
        }

        override public void OutWrite(T[] frame, int offsetSamples)
        {
            if (Error != null)
            {
                return;
            }
            FMODLib.RESULT res;

            IntPtr ptr1, ptr2;
            uint len1, len2;
            res = sound.@lock((uint)(offsetSamples * sizeofT * channels), (uint)(frame.Length * sizeofT), out ptr1, out ptr2, out len1, out len2);
            if (res != FMODLib.RESULT.OK)
            {
                Error = "failed to lock sound buffer: " + res;
                logger.LogError(logPrefix + Error);
                return;
            }

            int len1T = (int)len1 / sizeofT;
            int len2T = (int)len2 / sizeofT;
            if (soundFormat == FMODLib.SOUND_FORMAT.PCM16)
            {
                Marshal.Copy(frame as short[], 0, ptr1, len1T);
                if (ptr2 != IntPtr.Zero)
                {
                    Marshal.Copy(frame as short[], len1T, ptr2, len2T);
                }
            }
            else if (soundFormat == FMODLib.SOUND_FORMAT.PCMFLOAT)
            {
                Marshal.Copy(frame as float[], 0, ptr1, len1T);
                if (ptr2 != IntPtr.Zero)
                {
                    Marshal.Copy(frame as float[], len1T, ptr2, len2T);
                }
            }

            res = sound.unlock(ptr1, ptr2, len1, len2);
            if (res != FMODLib.RESULT.OK)
            {
                Error = "failed to unlock sound buffer: " + res;
                logger.LogError(logPrefix + Error);
                return;
            }
        }


        override public void Stop()
        {
            base.Stop();
            sound.release();
        }

        public string Error { get; private set; }

    }

    // Plays back input audio via FMOD Programmer Instrument
    // Provide an event with looped Programmer Instrument. AudioOutEvent<T> creates a Sound, assigns it to the Event and fires it on eaxh Start() call
    public class AudioOutEvent<T> : AudioOut<T>
    {
        FMODLib.Studio.EventInstance fmodEvent;
        public AudioOutEvent(FMODLib.System coreSystem, FMODLib.Studio.EventInstance fmodEvent, PlayDelayConfig playDelayConfig, ILogger logger, string logPrefix, bool debugInfo)
            : base(coreSystem, playDelayConfig, logger, "(Event)" + (logPrefix == "" ? "" : " ") + logPrefix, debugInfo)
        {
            this.fmodEvent = fmodEvent;
        }

        override public int OutPos
        {
            get
            {
                if (fmodEvent.handle == IntPtr.Zero)
                {
                    return 0;
                }
                else
                {
                    fmodEvent.getTimelinePosition(out int position);
                    return (int)(position * (long)this.frequency / 1000 % this.bufferSamples);
                }
            }
        }

        static int instCnt = 0;
        static Dictionary<int, AudioOutEvent<T>> instTable = new Dictionary<int, AudioOutEvent<T>>();

        override public void OutStart()
        {
            fmodEvent.setCallback(FMODEventCallback);
            IntPtr ud;
            lock (instTable)
            {
                instTable[instCnt] = this;
                ud = new IntPtr(instCnt);
                instCnt++;
           }

            fmodEvent.setUserData(ud);
            
            fmodEvent.start();
            logger.LogInfo(logPrefix + "Event Started");
        }

        [AOT.MonoPInvokeCallback(typeof(FMODLib.Studio.EVENT_CALLBACK))]
        static FMODLib.RESULT FMODEventCallback(FMODLib.Studio.EVENT_CALLBACK_TYPE type, IntPtr instance, IntPtr parameterPtr)
        {
            var evDummy = new FMODLib.Studio.EventInstance();
            evDummy.handle = instance;
            evDummy.getUserData(out IntPtr userdata);
            AudioOutEvent<T> audioOut;
            lock (instTable)
            {
                if (!instTable.TryGetValue(userdata.ToInt32(), out audioOut))
                {
                    // should not happen becase we deregister callback before removing the instance from the table
                    return FMODLib.RESULT.ERR_NOTREADY;
                }
            }
            return audioOut.fmodEventCallback(type, instance, parameterPtr);
        }
        FMODLib.RESULT fmodEventCallback(FMODLib.Studio.EVENT_CALLBACK_TYPE type, IntPtr instance, IntPtr parameterPtr)
        {
            logger.LogInfo(logPrefix + "EventCallback " + type);            
            switch (type)
            {
                case FMODLib.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                    {
                        var parameter = Marshal.PtrToStructure<FMODLib.Studio.PROGRAMMER_SOUND_PROPERTIES>(parameterPtr);
                        parameter.sound = Sound.handle;
                        parameter.subsoundIndex = -1;
                        Marshal.StructureToPtr(parameter, parameterPtr, false);
                        logger.LogInfo(logPrefix + "Sound Assigned to Event Parameter");
                    }
                    break;
                case FMODLib.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                    {
                    	// sound is released in Stop()
                    	
                        //var parameter = Marshal.PtrToStructure<FMODLib.Studio.PROGRAMMER_SOUND_PROPERTIES>(parameterPtr);
                        //var sound = new FMODLib.Sound();
                        //sound.handle = parameter.sound;
                        //sound.release();
                    }
                    break;
                case FMODLib.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                    break;
            }
            return FMODLib.RESULT.OK;
        }

        override public void Stop()
        {
            base.Stop();
            fmodEvent.setCallback(null);
            lock (instTable)
            {
                foreach (var i in instTable)
                {
                    if (i.Value == this)
                    {
                        instTable.Remove(i.Key);
                        break;
                    }
                }
            }
        }
    }
}
#endif
