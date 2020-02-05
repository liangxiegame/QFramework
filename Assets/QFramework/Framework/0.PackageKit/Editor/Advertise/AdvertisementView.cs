using UnityEngine;

namespace QFramework.PackageKit
{
	public class AdvertisementView : IPackageKitView
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

			var treeNode = new TreeNode(true, LocalText.TechSupport, autosaveSpreadState: true)
				.AddTo(mRootLayout);


			var verticalLayout = new VerticalLayout();

			treeNode.Add2Spread(verticalLayout);


			new AdvertisementItemView("官方文档：《QFramework 使用指南 2020》",
					"https://liangxiegame.com/zhuanlan/list/f0510c91-a90e-44df-87d2-f2ab94268fd1")
				.AddTo(verticalLayout);

			new AdvertisementItemView("官方 qq 群：623597263",
					"https://shang.qq.com/wpa/qunwpa?idkey=706b8eef0fff3fe4be9ce27c8702ad7d8cc1bceabe3b7c0430ec9559b3a9ce6")
				.AddTo(verticalLayout);

			new AdvertisementItemView("提问",
					"https://liangxiegame.com/zhuanlan/content/detail/7aed2448-a0a7-4de3-968b-375b3f06abe8")
				.AddTo(verticalLayout);


			new AdvertisementItemView("提需求",
					"https://liangxiegame.com/zhuanlan/content/detail/2a7cf754-e57c-4e9a-85a0-93b9226b1ad8")
				.AddTo(verticalLayout);


			new AdvertisementItemView("提 Bug",
					"https://liangxiegame.com/zhuanlan/content/detail/1d45ba9e-12df-42c5-abc3-1312effb5cce")
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