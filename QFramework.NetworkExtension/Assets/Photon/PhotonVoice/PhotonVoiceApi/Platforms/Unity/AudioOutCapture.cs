using UnityEngine;
using System;

namespace Photon.Voice.Unity
{
    public class AudioOutCapture : MonoBehaviour
    {
        public event Action<float[], int> OnAudioFrame;

        void OnAudioFilterRead(float[] frame, int channels)
        {
            if (OnAudioFrame != null)
            {
                OnAudioFrame(frame, channels);
            }
        }
    }
}