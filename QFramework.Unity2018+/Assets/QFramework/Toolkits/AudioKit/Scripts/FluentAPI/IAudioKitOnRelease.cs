using System;

namespace QFramework
{
    public interface IAudioKitOnRelease
    {
        void OnRelease(Action onRelease);
    }
    
    public static class AudioKitOnReleaseExtensions
    {
        public static void OnRelease(this IAudioKitOnRelease self,Action onRelease)
        {
            self?.OnRelease(onRelease);
        }
    }
}