namespace QFramework
{
    public interface IReactiveSystem : IExecuteSystem
    {
        void Activate();
        void Deactivate();
        void Clear();
    }
}