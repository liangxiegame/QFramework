namespace QFramework.ILKitDemo.Tetris
{
    public class PlayLineClearSoundCommand : ILCommand<Tetris>
    {
        public override void Execute()
        {
            AudioKit.PlaySound("Lineclear");
        }
    }
}