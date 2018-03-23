/****************************************************************************
 * Copyright (c) 2017 maoling@putao.com
 * Copyright (c) 2017 liangxie
****************************************************************************/

/// <summary>
/// Project config editor window.
/// </summary>
namespace QFramework
{
	using UnityEngine;
	using UnityEditor;
	
	public class QFrameworkConfigEditorWindow : EditorWindow
	{
		[MenuItem("QFramework/FrameworkConfig")]
		static void Open() 
		{
			QFrameworkConfigEditorWindow frameworkConfigEditorWindow = (QFrameworkConfigEditorWindow)EditorWindow.GetWindow(typeof(QFrameworkConfigEditorWindow),true);
			frameworkConfigEditorWindow.titleContent = new  GUIContent("QFrameworkConfig");
			frameworkConfigEditorWindow.CurConfigData = QFrameworkConfigData.Load ();
			frameworkConfigEditorWindow.Show ();
		}

		public QFrameworkConfigEditorWindow() {}

		public QFrameworkConfigData CurConfigData;
	
		void OnGUI() 
		{
			CurConfigData.Namespace = EditorGUIUtils.GUILabelAndTextField ("Namespace", CurConfigData.Namespace);
		
			CurConfigData.UIScriptDir = EditorGUIUtils.GUILabelAndTextField ("UI Script Generate Dir", CurConfigData.UIScriptDir);
			CurConfigData.UIPrefabDir = EditorGUIUtils.GUILabelAndTextField ("UI Prefab Dir", CurConfigData.UIPrefabDir);

			if (GUILayout.Button ("Apply")) 
			{
				CurConfigData.Save ();
			}
		}
	}
}