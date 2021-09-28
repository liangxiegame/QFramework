namespace QFramework
{
    public interface ILModel : ILCanGetUtility, ILCanGetModel
    {
    }

    public abstract class ILModel<TArchitecture> : ILModel
        where TArchitecture : ILArchitecture<TArchitecture>, new()
    {
        public T GetUtility<T>() where T : class, ILUtility
        {
            return ILSingleton<TArchitecture>.Instance.GetUtility<T>();
        }

        public T GetModel<T>() where T : class, ILModel
        {
            return ILSingleton<TArchitecture>.Instance.GetModel<T>();
        }
    }
}