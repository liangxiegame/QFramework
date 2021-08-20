using System;

namespace QFramework.PlatformRunner
{
    public class Framework : MonoSingleton<Framework>
    {
        public event Action OnApplicationQuitEvent = () => { };
        
        protected override void OnApplicationQuit()
        {
            OnApplicationQuitEvent.InvokeGracefully();
            
            base.OnApplicationQuit();
        }
    }
}