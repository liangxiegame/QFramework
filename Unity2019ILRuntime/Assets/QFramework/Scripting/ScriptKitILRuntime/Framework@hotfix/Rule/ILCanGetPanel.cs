namespace QFramework
{
    public interface ILCanGetPanel
    {
        T GetPanel<T>() where T : ILUIPanel, new();
    }
}