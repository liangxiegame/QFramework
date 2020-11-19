namespace QFramework
{
    public interface IModel : ICanGetUtility, ICanGetModel
    {
    }

    public class Model<TConfig> : IModel where TConfig : Architecture<TConfig>
    {
        IArchitecture mConfig
        {
            get { return SingletonProperty<TConfig>.Instance; }
        }

        public T GetUtility<T>() where T : class, IUtility
        {
            return mConfig.GetUtility<T>();
        }

        public T GetModel<T>() where T : class, IModel
        {
            return mConfig.GetModel<T>();
        }
    }
}