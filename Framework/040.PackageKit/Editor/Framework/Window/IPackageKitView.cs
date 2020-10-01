namespace QFramework
{
    public interface IPackageKitView
    {
        IQFrameworkContainer Container { get; set; }

        bool Ignore { get; }

        bool Enabled { get; }

        void Init(IQFrameworkContainer container);

        void OnUpdate();
        void OnGUI();

        void OnDispose();
    }
}