using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace Photon.Voice.Unity
{
    // Plays back input audio via Unity AudioSource
    // May consume audio packets in thread other than Unity's main thread
    public class UnityAudioOut : AudioOutDelayControl<float>
    {
        protected readonly AudioSource source;
        protected AudioClip clip;

        public UnityAudioOut(AudioSource audioSource, PlayDelayConfig playDelayConfig, ILogger logger, string logPrefix, bool debugInfo)
            : base(true, playDelayConfig, logger, "[PV] [Unity] AudioOut" + (logPrefix == "" ? "" : " " + logPrefix), debugInfo)
        {
            this.source = audioSource;
        }

        override public int OutPos { get { return source.timeSamples; } }

        override public void OutCreate(int frequency, int channels, int bufferSamples)
        {
            this.source.loop = true;
            // using streaming clip leads to too long delays
            this.clip = AudioClip.Create("UnityAudioOut", bufferSamples, channels, frequency, false);
            this.source.clip = clip;
        }

        override public void OutStart()
        {
            this.source.Play();
        }

        override public void OutWrite(float[] data, int offsetSamples)
        {
            clip.SetData(data, offsetSamples);
        }

        override public void Stop()
        {
            base.Stop();
            this.source.Stop();
            if (this.source != null)
            {
                this.source.clip = null;
                clip = null;
            }
        }
    }
}