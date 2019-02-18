/****************************************************************************
 * Copyright 2017 ~ 2019.2 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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

using System.Linq;

namespace QFramework.Editor
{
	using UnityEngine;
	using UnityEditor;

	public class PackageKitWindow : QEditorWindow
	{
		[MenuItem(FrameworkMenuItems.Preferences, false, FrameworkMenuItemsPriorities.Preferences)]
		private static void Open()
		{
			PackageApplication.Container = null;
			
			var window = PackageApplication.Container.Resolve<PackageKitWindow>();

			if (window == null)
			{
				var frameworkConfigEditorWindow = Create<PackageKitWindow>(true);
				frameworkConfigEditorWindow.titleContent = new GUIContent("QFramework Settings");
				frameworkConfigEditorWindow.position = new Rect(100, 100, 690, 480);
				frameworkConfigEditorWindow.Init();
				frameworkConfigEditorWindow.Show();
				
				PackageApplication.Container.RegisterInstance(frameworkConfigEditorWindow);
			}
			else
			{
				window.Show();
			}
		}

		private const string URL_GITHUB_ISSUE = "https://github.com/liangxiegame/QFramework/issues/new";

		[MenuItem(FrameworkMenuItems.Feedback, false, FrameworkMenuItemsPriorities.Feedback)]
		private static void Feedback()
		{
			Application.OpenURL(URL_GITHUB_ISSUE);
		}

		private void Init()
		{
			RemoveAllChidren();
			
			PackageApplication.Container
				.ResolveAll<IPackageKitView>()
				.OrderBy(view => view.RenderOrder)
				.ForEach(view => AddChild(view as GUIView));
		}
	}	
}