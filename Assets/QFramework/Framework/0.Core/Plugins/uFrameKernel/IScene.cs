namespace uFrame.Kernel
{
    public interface IScene
    {
        string Name { get; set; }

        ISceneSettings _SettingsObject { get; set; }

    }
}