namespace QFramework.ILKitDemo.Tetris
{
	public partial class GameStart
	{
		void OnStart()
		{
			UIKit.Root.SetResolution(640, 1136, 1);

			ResKit.Init();

			ILUIKit.OpenPanel<UITetrisPanel>();
		}

		void OnDestroy()
		{
		}
	}
}
