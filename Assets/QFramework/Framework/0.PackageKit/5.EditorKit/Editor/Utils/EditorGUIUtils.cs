/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QF
{
	using UnityEngine;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	public class EditorGUIUtils 
	{
		public static string GUILabelAndTextField(string labelContent,string textFieldContent,bool horizontal = true)
		{
			if (horizontal)
			EditorGUILayout.BeginHorizontal ();

			GUILayout.Label (labelContent);

			var retString = EditorGUILayout.TextField (textFieldContent);

			if (horizontal)
			EditorGUILayout.EndHorizontal();

			return retString;
		}
		
		public static string GUILabelAndPasswordField(string labelContent,string textFieldContent,bool horizontal = true)
		{
			if (horizontal)
				EditorGUILayout.BeginHorizontal ();

			GUILayout.Label (labelContent);

			string retString = EditorGUILayout.PasswordField(textFieldContent);

			if (horizontal)
				EditorGUILayout.EndHorizontal();

			return retString;
		}
		

		public static int GUILabelAndPopup(string labelContent,int popupIndex,string[] popupContents)
		{
			EditorGUILayout.BeginHorizontal ();

			GUILayout.Label (labelContent);

			int retIndex = EditorGUILayout.Popup (popupIndex,popupContents);

			EditorGUILayout.EndHorizontal ();

			return retIndex;
		}
	}
}