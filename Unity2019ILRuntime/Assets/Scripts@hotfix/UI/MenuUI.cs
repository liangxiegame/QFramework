namespace QFramework.ILKitDemo.Tetris
{
    public partial class MenuUI : ILController<Tetris>
    {
        void OnStart()
        {
            // Code Here
            RankButton.onClick.AddListener(() => SendCommand(new OnRankButtonClickCommand()));
            SettingButton.onClick.AddListener(() =>
            {
                AudioKit.PlaySound("Cursor_002");
                ILUIKit.GetPanel<UITetrisPanel>()
                    .SettingUI.Show();
            });
            StartButton.onClick.AddListener(() => SendCommand(new StartGameCommand()));


        }

        void OnDestroy()
        {
            // Destory Code Here
        }
    }
}