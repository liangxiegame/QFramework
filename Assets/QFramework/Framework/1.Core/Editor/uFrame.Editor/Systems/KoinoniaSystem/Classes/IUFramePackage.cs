namespace QF.GraphDesigner.Unity.KoinoniaSystem.Classes
{
    public interface IUFramePackage
    {

        string Id { get; set; }
        string VersionTag { get; set; }

        void Install();
        void Update();
        void Uninstall();
        void OnLoaded();
        bool NeedsInstall();
        bool NeedsUpdate();
    }
}