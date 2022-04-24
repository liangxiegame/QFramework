using UnityEngine;
using System;
using System.Linq;

namespace Photon.Voice.Unity
{
    // Wraps UnityEngine.Microphone with Voice.IAudioStream interface.
    public class MicWrapper : IAudioReader<float>
    {
        private AudioClip mic;
        private string device;
		ILogger logger;

		public MicWrapper(string device, int suggestedFrequency, ILogger logger)
        {
            try
            {
                this.device = device;
				this.logger = logger;
                if (UnityMicrophone.devices.Length < 1)
                {
                    Error = "No microphones found (UnityMicrophone.devices is empty)";
                    logger.LogError("[PV] MicWrapper: " + Error);
                    return;
                }
                if (!string.IsNullOrEmpty(device) && !UnityMicrophone.devices.Contains(device))
                {
                    logger.LogError(string.Format("[PV] MicWrapper: \"{0}\" is not a valid Unity microphone device, falling back to default one", device));
                    device = null;
                }
                int minFreq;
                int maxFreq;
                logger.LogInfo("[PV] MicWrapper: initializing microphone '{0}', suggested frequency = {1}).", device, suggestedFrequency);
                UnityMicrophone.GetDeviceCaps(device, out minFreq, out maxFreq);
                var frequency = suggestedFrequency;
                //        minFreq = maxFreq = 44100; // test like android client
                if (suggestedFrequency < minFreq || maxFreq != 0 && suggestedFrequency > maxFreq)
                {
                    logger.LogWarning("[PV] MicWrapper does not support suggested frequency {0} (min: {1}, max: {2}). Setting to {2}",
                        suggestedFrequency, minFreq, maxFreq);
                    frequency = maxFreq;
                }
                this.mic = UnityMicrophone.Start(device, true, 1, frequency);
                logger.LogInfo("[PV] MicWrapper: microphone '{0}' initialized, frequency = {1}, channels = {2}.", device, this.mic.frequency, this.mic.channels);
            }
            catch (Exception e)
            {
                Error = e.ToString();
                if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    Error = "Exception in MicWrapper constructor";
                }
                logger.LogError("[PV] MicWrapper: " + Error);
            }
        }

        public int SamplingRate { get { return Error == null ? this.mic.frequency : 0; } }
        public int Channels { get { return Error == null ? this.mic.channels : 0; } }
        public string Error { get; private set; }

        public void Dispose()
        {
            UnityMicrophone.End(this.device);
        }

        private int micPrevPos;
        private int micLoopCnt;
        private int readAbsPos;

        public bool Read(float[] buffer)
        {
            if (Error != null)
            {
                return false;
            }
            int micPos = UnityMicrophone.GetPosition(this.device);
            // loop detection
            if (micPos < micPrevPos)
            {
                micLoopCnt++;
            }
            micPrevPos = micPos;

            var micAbsPos = micLoopCnt * this.mic.samples + micPos;

			if (mic.channels == 0)
			{
				Error = "Number of channels is 0 in Read()";
				logger.LogError("[PV] MicWrapper: " + Error);
				return false;
			}
			var bufferSamplesCount = buffer.Length / mic.channels;

			var nextReadPos = this.readAbsPos + bufferSamplesCount;
            if (nextReadPos < micAbsPos)
            {
                this.mic.GetData(buffer, this.readAbsPos % this.mic.samples);
                this.readAbsPos = nextReadPos;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}