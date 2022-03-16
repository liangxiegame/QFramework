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
using UnityEditor;
using UnityEngine;

namespace QFramework
{
	[DisplayName("技术支持")]
	[PackageKitGroup("QFramework")]
	[PackageKitRenderOrder(int.MinValue)]
	public class AdvertisementView : IPackageKitView
	{
		private IXMLView mView = null;

		public EditorWindow EditorWindow { get; set; }

		public void Init()
		{
			var convertSystem = XMLKit.Get.SystemLayer.Get<IXMLToObjectConvertSystem>();
			
			if (!convertSystem.ContainsModule("EasyIMGUI"))
			{
				convertSystem.AddModule("EasyIMGUI", new EasyIMGUIXMLModule());
			}

			var easyIMGUIModule = convertSystem.GetConvertModule("EasyIMGUI");
			easyIMGUIModule.RegisterConverter("SupportItem",
				new CustomXMLToObjectConverter<AdvertisementItemView>(node =>
					new AdvertisementItemView(node.Attributes["Title"].Value, node.Attributes["Link"].Value)));
			
			mView = EasyIMGUI.XMLView();

			var supportAsset = Resources.Load<TextAsset>("Support") as TextAsset;
			mView.LoadXMLContent(supportAsset.text);
		}

		public void OnUpdate()
		{

		}

		public void OnGUI()
		{
			mView.DrawGUI();
		}

		public void OnWindowGUIEnd()
		{
			
		}

		public void OnDispose()
		{
			mView.Dispose();
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