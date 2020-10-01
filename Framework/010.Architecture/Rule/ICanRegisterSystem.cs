namespace QFramework
{
    public interface ICanRegisterSystem
    {
        void RegisterSystem<T>(T system) where T : class, ISystem;
    }
}