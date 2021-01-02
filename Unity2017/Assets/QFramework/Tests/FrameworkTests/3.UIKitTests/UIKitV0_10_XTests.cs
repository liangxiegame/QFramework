using NUnit.Framework;
using TravelStory;

namespace QFramework.Tests
{
	/// <summary>
	/// UIKit 界面管理方面的基本 API 测试
	/// </summary>
	public class UIKitV0_10_XTests
	{
		[Test]
		public void UIKit_OpenPanelDefaultTest()
		{
			ResKit.Init();

			var uiKitTestPanel = UIKit.OpenPanel<UIKitTestPanel>();

			Assert.IsTrue(uiKitTestPanel);

			Assert.AreEqual(uiKitTestPanel.State, PanelState.Opening);
			Assert.AreEqual(uiKitTestPanel.PanelInfo.Level, UILevel.Common);
			Assert.AreEqual(uiKitTestPanel.Data.OnInitCalledCount, 1);
			Assert.AreEqual(uiKitTestPanel.Data.OnOpenCalledCount, 1);
			Assert.AreEqual(uiKitTestPanel.Data.OnShowCalledCount, 1);

			UIKit.ClosePanel<UIKitTestPanel>();

			UIKit.CloseAllPanel();
		}

		[Test]
		public void UIKit_ClosePanelTest()
		{
			ResKit.Init();

			var uiKitTestPanel = UIKit.OpenPanel<UIKitTestPanel>();

			var data = uiKitTestPanel.Data;

			UIKit.ClosePanel<UIKitTestPanel>();

			Assert.AreEqual(data.OnInitCalledCount, 1);
			Assert.AreEqual(data.OnOpenCalledCount, 1);
			Assert.AreEqual(data.OnShowCalledCount, 1);
			Assert.AreEqual(data.OnHideCalledCount, 1);
			Assert.AreEqual(data.OnCloseCalledCount, 1);

			Assert.AreEqual(uiKitTestPanel.State, PanelState.Closed);

			UIKit.CloseAllPanel();
		}

		[Test]
		public void UIKit_GetPanelTest()
		{
			var uiKitTestPanel = UIKit.GetPanel<UIKitTestPanel>();

			Assert.IsFalse(uiKitTestPanel);

			ResKit.Init();

			UIKit.OpenPanel("UIKitTestPanel");

			uiKitTestPanel = UIKit.GetPanel<UIKitTestPanel>();

			Assert.IsTrue(uiKitTestPanel);
			Assert.AreEqual(uiKitTestPanel.State, PanelState.Opening);

			UIKit.ClosePanel("UIKitTestPanel");

			UIKit.CloseAllPanel();
		}

		[Test]
		public void UIKit_CloseAllPanelTest()
		{
			ResKit.Init();

			var uiKitTestPanel = UIKit.OpenPanel<UIKitTestPanel>();
			var uiKitTestPanel2 = UIKit.OpenPanel<UIKitTestPanel2>();

			UIKit.CloseAllPanel();

			Assert.AreEqual(uiKitTestPanel.State, PanelState.Closed);
			Assert.IsFalse(uiKitTestPanel.gameObject.activeSelf);
			Assert.AreEqual(uiKitTestPanel2.State, PanelState.Closed);
			Assert.IsFalse(uiKitTestPanel2.gameObject.activeSelf);

			UIKit.CloseAllPanel();
		}

		[Test]
		public void UIKit_ShowPanelTest()
		{
			ResKit.Init();

			var uiKitTestPanel = UIKit.OpenPanel<UIKitTestPanel>();

			Assert.AreEqual(uiKitTestPanel.Data.OnShowCalledCount, 1);

			UIKit.ShowPanel<UIKitTestPanel>();

			Assert.AreEqual(uiKitTestPanel.Data.OnShowCalledCount, 2);

			UIKit.HidePanel<UIKitTestPanel>();

			UIKit.ShowPanel<UIKitTestPanel>();

			Assert.AreEqual(uiKitTestPanel.Data.OnShowCalledCount, 3);

			UIKit.CloseAllPanel();
		}

		[Test]
		public void UIKit_HideAllPanelTest()
		{
			ResKit.Init();

			var uiKitTestPanel = UIKit.OpenPanel<UIKitTestPanel>();

			var uikitTestPanel2 = UIKit.OpenPanel<UIKitTestPanel2>();

			UIKit.HideAllPanel();

			Assert.AreEqual(uiKitTestPanel.State, PanelState.Hide);
			Assert.IsFalse(uiKitTestPanel.gameObject.activeSelf);
			Assert.AreEqual(uikitTestPanel2.State, PanelState.Hide);
			Assert.IsFalse(uikitTestPanel2.gameObject.activeSelf);

			UIKit.CloseAllPanel();
		}


		[Test]
		public void UIKit_PushPopTest()
		{
			ResKit.Init();

			var uiKitTestPanel = UIKit.OpenPanel<UIKitTestPanel>();

			UIKit.Stack.Push<UIKitTestPanel>();

			Assert.AreEqual(uiKitTestPanel.State, PanelState.Closed);

			UIKit.Stack.Pop();

			uiKitTestPanel = UIKit.GetPanel<UIKitTestPanel>();

			Assert.AreEqual(uiKitTestPanel.State, PanelState.Opening);

			UIKit.CloseAllPanel();
		}

		[Test]
		public void UIKit_PushBackTest()
		{
			ResKit.Init();

			var uiKitTestPanel = UIKit.OpenPanel<UIKitTestPanel>();

			UIKit.Stack.Push<UIKitTestPanel>();

			var uiKitTestPanel2 = UIKit.OpenPanel<UIKitTestPanel2>();

			Assert.AreEqual(uiKitTestPanel.State, PanelState.Closed);

			UIKit.Back(uiKitTestPanel2);

			uiKitTestPanel = UIKit.GetPanel<UIKitTestPanel>();

			Assert.AreEqual(uiKitTestPanel.State, PanelState.Opening);

			UIKit.CloseAllPanel();
		}
	}
}