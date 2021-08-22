namespace QFramework
{
    public interface ILCanRegisterModel
    {
        void RegisterModel<T>(T model) where T : class, ILModel;
    }
}