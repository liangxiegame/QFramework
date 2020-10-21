namespace QFramework
{
    public interface IPackageKitView
    {
        IQFrameworkContainer Container { get; set; }

        void Init(IQFrameworkContainer container);

        void OnUpdate();
        void OnGUI();

        void OnDispose();
        void OnShow();
        void OnHide();
    }
}