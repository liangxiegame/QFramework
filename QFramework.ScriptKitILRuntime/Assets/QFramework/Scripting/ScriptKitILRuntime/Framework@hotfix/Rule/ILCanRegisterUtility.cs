
namespace QFramework
{
    public interface ILCanRegisterUtility
    {
        void RegisterUtility<T>(T utility) where T : class, ILUtility;
    }
}