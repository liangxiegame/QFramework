namespace QFramework
{
    public interface ICanRegisterModel
    {
        void RegisterModel<T>(T model) where T : class, IModel;
    }
}