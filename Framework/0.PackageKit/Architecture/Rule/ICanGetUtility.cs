namespace QFramework
{
    public interface ICanGetUtility
    {
        T GetUtility<T>() where T : class, IUtility;
    }
}