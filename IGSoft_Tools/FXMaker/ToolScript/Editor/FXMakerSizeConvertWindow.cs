// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

// Attribute ------------------------------------------------------------------------
// Property -------------------------------------------------------------------------
// Loop Function --------------------------------------------------------------------
// Control Function -----------------------------------------------------------------
// Event Function -------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class FXMakerSizeConvertWindow : EditorWindow
{
	protected	string			m_assetPath				= "TargetFoler";
	protected	bool			m_bRecursively			= true;
	protected	float			m_fScale				= 2.0f;

	// ---------------------------------------------------------------------
	[MenuItem("Assets/FXMaker - ConvertSize")]
	public static EditorWindow Init()
	{
		EditorWindow window = GetWindow(typeof(FXMakerSizeConvertWindow));

		window.minSize	= new Vector2(280, 300);
		window.Show();
		return window;
	}

    void OnEnable()
    {
// 		Debug.Log("OnEnable");
   }

    void OnDisable()
    {
//		Debug.Log("OnDisable");
    }

	void OnGUI()
	{
		Object	tarPath		= EditorGUILayout.ObjectField	("AutoTarget", null, typeof(Object), false, GUILayout.MaxWidth(Screen.width));
		m_assetPath			= EditorGUILayout.TextField		("AssetPath", m_assetPath, GUILayout.MaxWidth(Screen.width));
		m_bRecursively		= EditorGUILayout.Toggle		("Recursively", m_bRecursively, GUILayout.MaxWidth(Screen.width));
		m_fScale			= EditorGUILayout.FloatField	("Scale", m_fScale, GUILayout.MaxWidth(Screen.width));

		if (tarPath != null)
		{
			string path = AssetDatabase.GetAssetPath(tarPath);
			path = path.Replace("Assets/", "");
			m_assetPath = path.Replace(Path.GetFileName(path), "");
		}

		EditorGUILayout.Space();
		FXMakerLayout.GUIEnableBackup((m_assetPath.Trim() != ""));
		if (GUILayout.Button("Start Resize", GUILayout.Height(40)))
			ResizePrefabs(m_assetPath, m_bRecursively);
		FXMakerLayout.GUIEnableRestore();
	}

	// Property -------------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	void ResizePrefabs(string assetPath, bool bRecursively)
	{
		int nOutFindFile;

		NgAsset.ObjectNode[]	objNodes = NgAsset.GetPrefabList("Assets/" + assetPath, false, bRecursively, 0, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);
		for (int n = 0; n < objNodes.Length; n++)
			ResizePrefab(objNodes[n].m_AssetPath);
	}

	void ResizePrefab(string assetPath)
	{
		GameObject	loadPrefab = NgAsset.LoadPrefab(assetPath);
		loadPrefab.transform.localScale *= m_fScale;
		AssetDatabase.SaveAssets();
		EditorApplication.SaveAssets();
	}

// 	void ConvertToStaticScale(GameObject targetPrefab)
// 	{
// 		NcParticleSystem	parCom	= targetPrefab.GetComponent<NcParticleSystem>();
// 		ParticleEmitter		emitCom	= targetPrefab.GetComponent<ParticleEmitter>();
// 		if (parCom != null && parCom.m_bScaleWithTransform && parCom.transform.lossyScale != Vector3.one)
// 		{
// 			NcParticleSystemEditor.ConvertToStaticScale(emitCom, emitCom.GetComponent<ParticleAnimator>());
// 			parCom.m_bScaleWithTransform	= false;
// 			return;
// 		}
// 	}
}



