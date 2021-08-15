namespace QFramework.ILKitDemo.Tetris
{
    public class PauseGameCommand : ILCommand<Tetris>
    {

        public override void Execute()
        {
            SendCommand<PlayCursorSoundCommand>();

            ILUIKit.GetPanel<UITetrisPanel>()
                .DoBeforeLeavingPlay();

            SendCommand(new EnteringMenuCommand());
        }
    }
}