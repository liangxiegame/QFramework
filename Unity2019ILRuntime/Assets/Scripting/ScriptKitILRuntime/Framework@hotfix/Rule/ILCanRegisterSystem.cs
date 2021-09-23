namespace QFramework
{
    public interface ILCanRegisterSystem
    {
        void RegisterSystem<T>(T system) where T : class, ILSystem;
    }
}