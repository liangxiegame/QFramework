namespace QFramework
{
    public interface ICanGetModel
    {
        T GetModel<T>() where T : class, IModel;
    }
}