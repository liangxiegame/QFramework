namespace QFramework.ILKitDemo.Tetris
{
    public class OnDestoryButtonClickCommand : ILCommand<Tetris>
    {
        
        public override void Execute()
        {
            ILUIKit.GetPanel<UITetrisPanel>().Model.ClearData();
            SendCommand(new OnRankButtonClickCommand());
        }
    }
}