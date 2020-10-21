namespace QFramework
{
    public interface ICanRegisterUtility
    {
        void RegisterUtility<T>(T utility) where T : class, IUtility;
    }
}