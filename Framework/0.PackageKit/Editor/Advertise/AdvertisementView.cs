using System.ComponentModel;

namespace QFramework.PackageKit
{
	[DisplayName("技术支持")]
	[PackageKitRenderOrder(int.MinValue)]
	public class AdvertisementView : QFramework.IPackageKitView
	{
		public IQFrameworkContainer Container { get; set; }

		public int RenderOrder
		{
			get { return -1; }
		}

		public bool Ignore { get; private set; }

		public bool Enabled
		{
			get { return true; }
		}

		private VerticalLayout mRootLayout = null;

		public void Init(IQFrameworkContainer container)
		{
			mRootLayout = new VerticalLayout();

			var verticalLayout = new VerticalLayout()
				.AddTo(mRootLayout);

			new LabelView("技术支持").FontBold().FontSize(12).AddTo(verticalLayout);

			new AdvertisementItemView("官方文档：《QFramework 使用指南 2020》",
					"https://qframework.cn/doc")
				.AddTo(verticalLayout);

			new AdvertisementItemView("官方 qq 群：623597263",
					"https://shang.qq.com/wpa/qunwpa?idkey=706b8eef0fff3fe4be9ce27c8702ad7d8cc1bceabe3b7c0430ec9559b3a9ce6")
				.AddTo(verticalLayout);

			new AdvertisementItemView("提问/提需求/提 Bug/社区",
					"https://qframework.cn/community")
				.AddTo(verticalLayout);

			new AdvertisementItemView("github",
					"https://github.com/liangxiegame/QFramework")
				.AddTo(verticalLayout);

			new AdvertisementItemView("gitee",
					"https://gitee.com/liangxiegame/QFramework")
				.AddTo(verticalLayout);

			new AdvertisementItemView("Unity 开发者进阶班级：小班",
					"https://liangxiegame.com/zhuanlan/list/89064995-924f-43cd-b236-3eb3eaa01aa0")
				.AddTo(verticalLayout);
		}

		public void OnUpdate()
		{

		}

		public void OnGUI()
		{


			mRootLayout.DrawGUI();
		}

		public void OnDispose()
		{
			mRootLayout.Dispose();
			mRootLayout = null;
		}

		public class LocalText
		{
			public static string TechSupport
			{
				get { return Language.IsChinese ? "技术支持" : "Tech Support"; }
			}
		}
	}
}