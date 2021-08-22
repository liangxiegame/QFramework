namespace QFramework.ILKitDemo.Tetris
{
    public class PlayControllerSoundCommand : ILCommand<Tetris>
    {
        public override void Execute()
        {
            AudioKit.PlaySound("Balloon_003");
        }
    }
}