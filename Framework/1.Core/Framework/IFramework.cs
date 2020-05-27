namespace QFramework
{
    public interface IFramework
    {
        IQFrameworkContainer FrameworkModuleContainer { get; }
        
        IQFrameworkContainer UtilityContainer { get; }
        
        
    }
}