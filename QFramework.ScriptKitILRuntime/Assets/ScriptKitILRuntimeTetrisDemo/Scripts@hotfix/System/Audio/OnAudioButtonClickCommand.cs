namespace QFramework.ILKitDemo.Tetris
{
    public class OnAudioButtonClickCommand : ILCommand<Tetris>
    {
        public override void Execute()
        {
            AudioKit.Settings.IsOn.Value = !AudioKit.Settings.IsOn.Value;

            if (AudioKit.Settings.IsOn.Value)
            {
                SendCommand<PlayCursorSoundCommand>();
            }
        }
    }
}