namespace QFramework
{
    public interface ILCanGetModel
    {
        T GetModel<T>() where T : class, ILModel;
    }
}