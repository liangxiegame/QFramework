namespace QFramework
{
    public interface ILCanGetUtility
    {
        T GetUtility<T>() where T : class, ILUtility;

    }
}