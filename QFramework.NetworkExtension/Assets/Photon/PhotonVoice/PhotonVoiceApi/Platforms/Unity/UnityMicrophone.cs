namespace Photon.Voice.Unity
{
    #if UNITY_WEBGL
    using System;
    #endif
    using UnityEngine;

    /// <summary>A wrapper around UnityEngine.Microphone to be able to safely use Microphone and compile for WebGL.</summary>
    public static class UnityMicrophone
    {
        #if UNITY_WEBGL
        private static readonly string[] _devices = new string[0];
        #endif

        public static string[] devices
        {
            get
            {
                #if UNITY_WEBGL
                return _devices;
                #else
                return Microphone.devices;
                #endif
            }
        }

        public static void End(string deviceName)
        {
            #if UNITY_WEBGL
            throw new NotImplementedException("Unity Microphone not supported on WebGL");
            #else
            Microphone.End(deviceName);
            #endif
        }
        
        public static void GetDeviceCaps(string deviceName, out int minFreq, out int maxFreq)
        {
            #if UNITY_WEBGL
            throw new NotImplementedException("Unity Microphone not supported on WebGL");
            #else
            Microphone.GetDeviceCaps(deviceName, out minFreq, out maxFreq);
            #endif
        }

        public static int GetPosition(string deviceName)
        {
            #if UNITY_WEBGL
            throw new NotImplementedException("Unity Microphone not supported on WebGL");
            #else
            return Microphone.GetPosition(deviceName);
            #endif
        }

        public static bool IsRecording(string deviceName)
        {
            #if UNITY_WEBGL
            return false;
            #else
            return Microphone.IsRecording(deviceName);
            #endif
        }

        public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency)
        {
            #if UNITY_WEBGL
            throw new NotImplementedException("Unity Microphone not supported on WebGL");
            #else
            return Microphone.Start(deviceName, loop, lengthSec, frequency);
            #endif
        }
    }
}