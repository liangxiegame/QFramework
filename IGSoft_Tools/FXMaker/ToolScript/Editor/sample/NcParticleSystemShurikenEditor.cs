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

// [CustomEditor(typeof(NcParticleSystemShuriken))]
// [CustomEditor(typeof(ParticleSystem))]

public class NcParticleSystemShurikenEditor : Editor
{
	protected	FXMakerMain				m_FXMakerMain;

	// ---------------------------------------------------------------------
    void OnEnable()
    {
//  		m_SelectedObj = target as NcParticleSystemShuriken;
    }

    void OnDisable()
    {
// 		if (m_FXMakerMain != null)
// 			m_FXMakerMain.CloseNcPrefabPopup();
    }

	public override void OnInspectorGUI()
	{
// 		NgAssembly.GetPropertis(this.serializedObject);

		DrawDefaultInspector();

		SerializedObject so = this.serializedObject;

		SerializedProperty sp = so.FindProperty("ShapeModule");

		if (sp != null)
 			Debug.Log(sp.name);

//		SerializedProperty	sp = so.GetIterator();

		foreach (SerializedProperty aa in sp)
		{
			if (aa.name == "radius")
			{
				Debug.Log(aa.floatValue);
				aa.floatValue = 10;
				Debug.Log(aa.floatValue);
			}
		}
		so.ApplyModifiedProperties();
		(target as ParticleSystem).Play();

// 		// --------------------------------------------------------------
// 		m_FXMakerMain = GetFXMakerMain();
// 
// 		// --------------------------------------------------------------
// 		rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nAddHeight));
// 		if (NgLayout.GUIButton(NgLayout.GetInnerHorizontalRect(rect, 2, 0, 1), "Select ShapeMesh", (m_FXMakerMain != null)));
// // 		m_FXMakerMain.ShowSelectPrefabPopup(m_SelectedObj);
// // 		if (NgLayout.GUIButton(NgLayout.GetInnerHorizontalRect(rect, 2, 1, 1), " ", (m_SelectedObj.m_AttachPrefab != null)));
// // 		{
// // 			m_SelectedObj.m_AttachPrefab = null;
// // 			if (m_FXMakerMain != null)
// // 				m_FXMakerMain.CreateCurrentInstanceEffect(true);
// // 		}
//  		GUILayout.Label("");
// 		EditorGUILayout.EndHorizontal();
// 		EditorGUILayout.Space();
	}

	// ----------------------------------------------------------------------------------
	FXMakerMain GetFXMakerMain()
	{
		GameObject fxMaker = GameObject.Find("_FXMaker");
		if (Application.isPlaying && fxMaker != null && fxMaker.GetComponent("FXMakerMain") != null)
			return fxMaker.GetComponent<FXMakerMain>();
		return null;
	}

	Rect GetCurveRect(int line)
	{
		int		nLineWidth	= 100;
		int		nLineHeight	= 100;

		return new Rect(0, line * nLineHeight, nLineWidth, nLineHeight);
	}

	// Property -------------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	// Event Function -------------------------------------------------------------------

}
