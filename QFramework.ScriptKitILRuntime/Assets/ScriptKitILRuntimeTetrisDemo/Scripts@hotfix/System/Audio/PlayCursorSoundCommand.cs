namespace QFramework.ILKitDemo.Tetris
{
    public class PlayCursorSoundCommand : ILCommand<Tetris>
    {
        public override void Execute()
        {
            AudioKit.PlaySound("Cursor_002");
        }
    }
}