namespace QFramework
{
    public interface ILCanGetSystem
    {
        T GetSystem<T>() where T : class, ILSystem;
    }
}