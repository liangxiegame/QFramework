namespace QFramework
{
    public interface ISystem : ICanGetModel,ICanGetSystem,ICanGetUtility,ICanSendEvent,ICanSendCommand
    {
        
    }

    public class System<TConfig> : ISystem where TConfig : AbstractArchitectureConfig<ICommand, TConfig>, new()
    {
        
    }
}