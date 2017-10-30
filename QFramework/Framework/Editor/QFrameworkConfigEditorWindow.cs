/****************************************************************************
 * Copyright (c) 2017 maoling@putao.com
 * Copyright (c) 2017 liangxie
****************************************************************************/

/// <summary>
/// Project config editor window.
/// </summary>
namespace QFramework.Editor
{
	using UnityEngine;
	using UnityEditor;
	using Libs.Editor;
	
	public class QFrameworkConfigEditorWindow : EditorWindow
	{
		[MenuItem("PuTaoTool/Framework/FrameworkConfig")]
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
		
			#if NONE_LUA_SUPPORT 
			CurConfigData.UIScriptDir = EditorGUIUtils.GUILabelAndTextField ("UI Script Generate Dir", CurConfigData.UIScriptDir);
			CurConfigData.UIPrefabDir = EditorGUIUtils.GUILabelAndTextField ("UI Prefab Dir", CurConfigData.UIPrefabDir);
			#endif

			CurConfigData.ResLoaderSupportIndex = EditorGUIUtils.GUILabelAndPopup("AB Support",
				CurConfigData.ResLoaderSupportIndex, QFrameworkConfigData.RES_LOADER_SUPPORT_TEXTS);
			CurConfigData.LuaSupportIndex = EditorGUIUtils.GUILabelAndPopup("Lua Support", CurConfigData.LuaSupportIndex,
				QFrameworkConfigData.LUA_SUPPORT_TEXTS);
			CurConfigData.CocosSupportIndex = EditorGUIUtils.GUILabelAndPopup("Cocos Support", CurConfigData.CocosSupportIndex,
				QFrameworkConfigData.COCOS_SUPPORT_TEXTS);
			if (GUILayout.Button ("Apply")) 
			{
				CurConfigData.Save ();
				MicroEditor.ApplyAllPlatform ();
			}
		}
	}

	[InitializeOnLoad]
	public class MicroEditor
	{
		static MicroEditor()
		{
			Debug.Log (">>>>>> Initialize MicroEditor");

			ApplyAllPlatform ();
		}

		public static void ApplyAllPlatform()
		{
			var frameworkConfigData = QFrameworkConfigData.Load ();

			ApplySymbol (frameworkConfigData, BuildTargetGroup.iOS);
			ApplySymbol (frameworkConfigData, BuildTargetGroup.Android);
			ApplySymbol (frameworkConfigData, BuildTargetGroup.Standalone);
		}


		public static void ApplySymbol(QFrameworkConfigData frameworkConfigData,BuildTargetGroup targetGroup)
		{
			string 	symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (targetGroup);

			if (string.IsNullOrEmpty (symbols)) {
				symbols = frameworkConfigData.ResLoaderSupportSymbol;
			}
			else {
				string[] symbolSplit = symbols.Split (new char[]{ ';' });

				symbols = "";

				for (int i = 0; i < symbolSplit.Length; i++) 
				{
					var symbol = symbolSplit [i];
					if (string.Equals (symbol, QFrameworkConfigData.RES_LOADER_SUPPORT_SYMBOLS [0]) ||
						string.Equals (symbol, QFrameworkConfigData.RES_LOADER_SUPPORT_SYMBOLS [1]) ||
						string.Equals (symbol, QFrameworkConfigData.LUA_SUPPORT_SYMBOLS [0]) ||
						string.Equals (symbol, QFrameworkConfigData.LUA_SUPPORT_SYMBOLS [1]) ||
						string.Equals (symbol, QFrameworkConfigData.LUA_SUPPORT_SYMBOLS [2]) ||
						string.Equals (symbol, QFrameworkConfigData.LUA_SUPPORT_SYMBOLS [3]) ||
					    string.Equals (symbol, QFrameworkConfigData.COCOS_SUPPORT_SYMBOLS[0]) ||
					    string.Equals (symbol, QFrameworkConfigData.COCOS_SUPPORT_SYMBOLS[1]))
					{

					}
					else {
						symbols += symbol + ";";
					}
				}

				symbols += frameworkConfigData.ResLoaderSupportSymbol + ";";
				symbols += frameworkConfigData.LuaSupportSymbol + ";";
				symbols += frameworkConfigData.CocosSupportSymbol + ";";
			}

			PlayerSettings.SetScriptingDefineSymbolsForGroup (targetGroup, symbols);
		}
	}
}
