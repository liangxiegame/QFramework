/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.ComponentModel;

namespace QFramework
{
	[DisplayName("技术支持")]
	[PackageKitGroup("QFramework")]
	[PackageKitRenderOrder(int.MinValue)]
	public class AdvertisementView : IPackageKitView
	{
		public IQFrameworkContainer Container { get; set; }

		private IVerticalLayout mRootLayout;

		public void Init(IQFrameworkContainer container)
		{
			mRootLayout = EasyIMGUI.Vertical();

			var verticalLayout =  EasyIMGUI.Vertical()
				.AddTo(mRootLayout);
			EasyIMGUI.Label().Text("技术支持").FontBold().FontSize(12).AddTo(verticalLayout);

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

		public void OnShow()
		{
			
		}

		public void OnHide()
		{
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