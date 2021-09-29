namespace QFramework.ILKitDemo.Tetris
{
    public class PlayDropSoundCommand : ILCommand<Tetris>
    {
        public override void Execute()
        {
            AudioKit.PlaySound("Drop");
        }
    }
}