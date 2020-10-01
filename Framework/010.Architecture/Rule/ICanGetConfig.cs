namespace QFramework
{
    public interface ICanGetConfig
    {
        T GetConfig<T>() where T : class, IArchitecture;
    }
}