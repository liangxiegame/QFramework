using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Photon.Voice.Unity
{
    public class AudioInEnumerator : DeviceEnumeratorBase
    {
        public AudioInEnumerator(ILogger logger) : base (logger)
        {
            Refresh();
        }

        public override void Refresh()
        {
            var unityDevs = UnityMicrophone.devices;
            devices = new List<DeviceInfo>();
            for (int i = 0; i < unityDevs.Length; i++)
            {
                var d = unityDevs[i];
                devices.Add(new DeviceInfo(d));
            }
        }

#if UNITY_WEBGL
        public override bool IsSupported => false;
        
        public override string Error { get { return "Current platform " + Application.platform + " is not supported by AudioInEnumerator."; } }
#else
        public override string Error { get { return null; } }
        #endif

        public override void Dispose()
        {
        }
    }

#if PHOTON_VOICE_VIDEO_ENABLE
    public class VideoInEnumerator : DeviceEnumeratorBase
    {
        public VideoInEnumerator(ILogger logger) : base(logger)
        {
            Refresh();
        }

        public override void Refresh()
        {
            var unityDevs = UnityEngine.WebCamTexture.devices;
            devices = new List<DeviceInfo>();
            for (int i = 0; i < unityDevs.Length; i++)
            {
                var d = unityDevs[i];
                devices.Add(new DeviceInfo(d.name));
            }
        }

        public override string Error { get { return null; } }

        public override void Dispose()
        {
        }
    }
#endif
}