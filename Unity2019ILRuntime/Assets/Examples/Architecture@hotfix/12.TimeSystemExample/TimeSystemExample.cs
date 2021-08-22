namespace QFramework.Example
{
    public class TimeSystemExample : ILArchitecture<TimeSystemExample>
    {
        protected override void OnSystemConfig(ILRuntimeIOCContainer systemLayer)
        {
            // 注册时间系统
            systemLayer.RegisterInstance<ITimeSystem>(new TimeSystem());
        }

        protected override void OnModelConfig(ILRuntimeIOCContainer modelLayer)
        {
        }

        protected override void OnUtilityConfig(ILRuntimeIOCContainer utilityLayer)
        {
        }

        protected override void OnLaunch()
        {
        }
    }
}