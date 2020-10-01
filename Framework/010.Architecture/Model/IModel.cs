namespace QFramework
{
    public interface IModel : ICanGetUtility, ICanGetModel
    {
    }

    public class Model<TConfig> : IModel where TConfig: Architecture<TConfig>
    {
        public T GetUtility<T>() where T : class, IUtility
        {
            return SingletonProperty<TConfig>.Instance.GetUtility<T>();
        }

        public T GetModel<T>() where T : class, IModel
        {
            return SingletonProperty<TConfig>.Instance.GetModel<T>();
        }
    }
}