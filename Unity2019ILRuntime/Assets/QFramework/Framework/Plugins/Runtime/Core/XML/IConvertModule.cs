namespace QFramework
{
    public interface IConvertModule
    {
        IXMLToObjectConverter GetConverter(string name);

        void RegisterConverter(string name, IXMLToObjectConverter converter);
    }
}