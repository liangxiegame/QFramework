// Attribute ------------------------------------------------------------------------
// Property -------------------------------------------------------------------------
// Loop Function --------------------------------------------------------------------
// Control Function -----------------------------------------------------------------
// Event Function -------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

// [CustomEditor(typeof(NcAnimationCurve))]

public class NcEditorWindow : EditorWindow
{
	// ---------------------------------------------------------------------
	public static EditorWindow Init()
	{
		EditorWindow window = GetWindow(typeof(NcEditorWindow));

		window.minSize	= new Vector2(300, 500);
		window.Show();
		return window;
	}

	void OnEnable()
    {
		Debug.Log("OnEnable");
    }

    void OnDisable()
    {
		Debug.Log("OnDisable");
    }

	void OnDestroy()
	{
		Debug.Log("OnDestroy");
	}

	void OnInspectorUpdate()
	{
		Debug.Log("OnInspectorUpdate");
	}

	void OnHierarchyChange()
	{
		Debug.Log("OnHierarchyChange");
	}

	void OnProjectChange()
	{
		Debug.Log("OnProjectChange");
	}

	void OnSelectionChange()
	{
		Debug.Log("OnSelectionChange");
	}

	void OnFocus()
	{
		Debug.Log("OnFocus");
	}

	void OnLostFocus()
	{
		Debug.Log("OnLostFocus");
	}

	// ----------------------------------------------------------------------------------
}
