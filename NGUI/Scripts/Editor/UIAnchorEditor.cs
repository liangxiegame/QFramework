//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIAnchor))]
public class UIAnchorEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
		EditorGUILayout.HelpBox("UIAnchor is a legacy component and should not be used anymore. All widgets have anchoring functionality built-in.", MessageType.Warning);
	}
}
