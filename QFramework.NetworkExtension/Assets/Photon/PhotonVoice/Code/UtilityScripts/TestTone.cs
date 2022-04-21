// --------------------------------------------------------------------------------
// <copyright file="TestTone.cs" company="Exit Games GmbH">
//   Part of: Photon Voice Utilities for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// This MonoBehaviour is a sample demo of how to use AudioSource.Factory 
// by implementing IAudioReader.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------

using System;
using UnityEngine;

namespace Photon.Voice.Unity.UtilityScripts
{
    [RequireComponent(typeof(Recorder))]
    public class TestTone : MonoBehaviour
    {
        private void Start()
        {
            Recorder rec = this.gameObject.GetComponent<Recorder>();
            rec.SourceType = Recorder.InputSourceType.Factory;
            rec.InputFactory = () =>
            {
                return new ToneAudioReader();
            };
        }
    }

    // IAudioReader implementation sample. Provides constant tone signal.
    // See also MicWrapper and AudioClipWrapper
    // Because of current resamplig algorithm, the tone is distorted if SamplingRate not equals encoder sampling rate.
    class ToneAudioReader : IAudioReader<float>
    {
        private double k;
        private long timeSamples;

        public ToneAudioReader()
        {
            this.k = 2 * Math.PI * 440 / this.SamplingRate;
        }
        public int Channels { get { return 2; } }

        public int SamplingRate { get { return 24000; } }

        public string Error
        {
            get
            {
                return null;
            }
        }

        public void Dispose()
        {
        }

        public bool Read(float[] buf)
        {
            var bufSamples = buf.Length / this.Channels;
            var t = (long)(AudioSettings.dspTime * this.SamplingRate);

            var deltaTimeSamples = t - this.timeSamples;
            if (Math.Abs(deltaTimeSamples) > this.SamplingRate / 4) // when started or Read has not been called for a while
            {
                Debug.LogWarningFormat("ToneAudioReader sample time is out: {0} / {1}", this.timeSamples, t);
                deltaTimeSamples = bufSamples;
                this.timeSamples = t - bufSamples;
            }

            if (deltaTimeSamples < bufSamples)
            {
                return false;
            }
            int x = 0;
            for (int i = 0; i < bufSamples; i++)
            {
                var v = (float)Math.Sin(this.timeSamples++ * this.k) * 0.2f;
                for (int j = 0; j < this.Channels; j++)
                    buf[x++] = v;
            }
            return true;
        }
    }
}

