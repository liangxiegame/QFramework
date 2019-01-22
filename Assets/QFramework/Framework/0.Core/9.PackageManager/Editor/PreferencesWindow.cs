﻿/****************************************************************************
 * Copyright 2017 ~ 2018.12 liangxie
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

namespace QFramework
{
	using UnityEngine;
	using UnityEditor;

	public class PreferencesWindow : EditorWindow
	{
		[MenuItem(FrameworkMenuItems.Preferences, false, FrameworkMenuItemsPriorities.Preferences)]
		private static void Open()
		{
			var frameworkConfigEditorWindow = (PreferencesWindow) GetWindow(typeof(PreferencesWindow), true);
			frameworkConfigEditorWindow.titleContent = new GUIContent("QFramework Settings");
			frameworkConfigEditorWindow.CurSettingData = FrameworkSettingData.Load();
			frameworkConfigEditorWindow.position = new Rect(100, 100, 690, 460);
			frameworkConfigEditorWindow.Init();
			frameworkConfigEditorWindow.Show();
		}

		private const string URL_GITHUB_ISSUE = "https://github.com/liangxiegame/QFramework/issues/new";

		[MenuItem(FrameworkMenuItems.Feedback, false, FrameworkMenuItemsPriorities.Feedback)]
		private static void Feedback()
		{
			Application.OpenURL(URL_GITHUB_ISSUE);
		}

		public FrameworkSettingData CurSettingData;

		private FrameworkPMView mPMView;

		private void Init()
		{
			mPMView = new FrameworkPMView();
			mPMView.Init(this);
		}

		private void OnGUI()
		{
			GUILayout.Label("UI Kit Settings:");
			GUILayout.BeginVertical("box");
			
			CurSettingData.Namespace = EditorGUIUtils.GUILabelAndTextField("Namespace", CurSettingData.Namespace);
			CurSettingData.UIScriptDir =
				EditorGUIUtils.GUILabelAndTextField("UI Script Generate Dir", CurSettingData.UIScriptDir);
			CurSettingData.UIPrefabDir =
				EditorGUIUtils.GUILabelAndTextField("UI Prefab Dir", CurSettingData.UIPrefabDir);

			if (GUILayout.Button("Apply"))
			{
				CurSettingData.Save();
			}

			GUILayout.EndVertical();

			mPMView.OnGUI();
		}
	}
}