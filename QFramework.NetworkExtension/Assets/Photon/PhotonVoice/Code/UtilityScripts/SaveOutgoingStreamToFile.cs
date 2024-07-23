namespace Photon.Voice.Unity.UtilityScripts
{
    using UnityEngine;
    using System.IO;
    using CSCore;
    using CSCore.Codecs.WAV;

    [RequireComponent(typeof(Recorder))]
    [DisallowMultipleComponent]
    public class SaveOutgoingStreamToFile : VoiceComponent
    {
        private WaveWriter wavWriter;

        private void PhotonVoiceCreated(PhotonVoiceCreatedParams photonVoiceCreatedParams)
        {
            VoiceInfo voiceInfo = photonVoiceCreatedParams.Voice.Info;
            int bitsPerSample = 32;
            if (photonVoiceCreatedParams.Voice is LocalVoiceAudioShort)
            {
                bitsPerSample = 16;
            }
            string filePath = this.GetFilePath();
            this.wavWriter = new WaveWriter(filePath, new WaveFormat(voiceInfo.SamplingRate, bitsPerSample, voiceInfo.Channels));
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("Outgoing stream, output file path: {0}", filePath);
            }
            if (photonVoiceCreatedParams.Voice is LocalVoiceAudioFloat)
            {
                LocalVoiceAudioFloat localVoiceAudioFloat = photonVoiceCreatedParams.Voice as LocalVoiceAudioFloat;
                localVoiceAudioFloat.AddPreProcessor(new OutgoingStreamSaverFloat(this.wavWriter));
            } 
            else if (photonVoiceCreatedParams.Voice is LocalVoiceAudioShort)
            {
                LocalVoiceAudioShort localVoiceAudioShort = photonVoiceCreatedParams.Voice as LocalVoiceAudioShort;
                localVoiceAudioShort.AddPreProcessor(new OutgoingStreamSaverShort(this.wavWriter));
            }
        }

        private string GetFilePath()
        {
            string filename = string.Format("out_{0}_{1}.wav", System.DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-ffff"), Random.Range(0, 1000));
            return Path.Combine(Application.persistentDataPath, filename);
        }

        private void PhotonVoiceRemoved()
        {
            this.wavWriter.Dispose();
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("Recording stopped: Saving wav file.");
            }
        }

        class OutgoingStreamSaverFloat : IProcessor<float>
        {
            private WaveWriter wavWriter;

            public OutgoingStreamSaverFloat(WaveWriter waveWriter)
            {
                this.wavWriter = waveWriter;
            }

            public float[] Process(float[] buf)
            {
                this.wavWriter.WriteSamples(buf, 0, buf.Length);
                return buf;
            }

            public void Dispose()
            {
                if (!this.wavWriter.IsDisposed && !this.wavWriter.IsDisposing)
                {
                    this.wavWriter.Dispose();
                }
            }
        }

        class OutgoingStreamSaverShort : IProcessor<short>
        {
            private WaveWriter wavWriter;

            public OutgoingStreamSaverShort(WaveWriter waveWriter)
            {
                this.wavWriter = waveWriter;
            }

            public short[] Process(short[] buf)
            {
                for (int i = 0; i < buf.Length; i++)
                {
                    this.wavWriter.Write(buf[i]);
                }
                return buf;
            }

            public void Dispose()
            {
                if (!this.wavWriter.IsDisposed && !this.wavWriter.IsDisposing)
                {
                    this.wavWriter.Dispose();
                }
            }
        }
    }
}
