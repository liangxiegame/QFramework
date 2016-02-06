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

public class FXMakerModelToPrefabWindow : EditorWindow
{
	protected	string			m_srcModelPath				= "srcModelPath";
	protected	string			m_tarPrefabPath				= "tarPrefabPath";
	protected	bool			m_bRecursively			= true;

	// ---------------------------------------------------------------------
	[MenuItem("Assets/FXMaker - ModelToPrefab")]
	public static EditorWindow Init()
	{
		EditorWindow window = GetWindow(typeof(FXMakerModelToPrefabWindow));

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
		m_srcModelPath		= EditorGUILayout.TextField		("srcModelPath", m_srcModelPath, GUILayout.MaxWidth(Screen.width));
		m_bRecursively		= EditorGUILayout.Toggle		("Recursively", m_bRecursively, GUILayout.MaxWidth(Screen.width));
		m_tarPrefabPath		= EditorGUILayout.TextField		("tarPrefabPath", m_tarPrefabPath, GUILayout.MaxWidth(Screen.width));

		if (tarPath != null)
		{
			string path = AssetDatabase.GetAssetPath(tarPath);
			path = path.Replace("Assets/", "");
			m_srcModelPath = path.Replace(Path.GetFileName(path), "");
		}

		EditorGUILayout.Space();
		FXMakerLayout.GUIEnableBackup((m_srcModelPath.Trim() != ""));
		if (GUILayout.Button("CreatePrefab", GUILayout.Height(40)))
			CreatePrefab(m_srcModelPath, m_bRecursively);
		FXMakerLayout.GUIEnableRestore();
	}

	// Property -------------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	void CreatePrefab(string assetPath, bool bRecursively)
	{
		NgAsset.ObjectNode[]	objNodes = GetModelList("Assets/" + assetPath, bRecursively);
		for (int n = 0; n < objNodes.Length; n++)
			CreatePrefab(objNodes[n]);
	}

	void CreatePrefab(NgAsset.ObjectNode modelNode)
	{
		string targetPath = NgFile.CombinePath("Assets/" + m_tarPrefabPath, NgFile.GetFilename(modelNode.m_AssetPath) + ".prefab");
		GameObject	loadPrefab = NgAsset.LoadPrefab(targetPath);
		if (loadPrefab != null)
		{
			// 있으면 pass
			return;
		}
 		PrefabUtility.CreatePrefab(targetPath, (GameObject)modelNode.m_Object);
		AssetDatabase.SaveAssets();
		EditorApplication.SaveAssets();
	}

	public static NgAsset.ObjectNode[] GetModelList(string strDir, bool bRecursively)
	{
		int						nOutFindFile;
		ArrayList	retArray;
		int			nCount = 0;

		if (bRecursively)
			 retArray = NgAsset.GetResourceFileRecursively<GameObject>(strDir, null, 0, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);
		else retArray = NgAsset.GetResourceFiles<GameObject>(strDir, null, 0, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);

		while (true)
		{
			if (retArray.Count <= nCount)
				break;
			if (PrefabUtility.GetPrefabType((Object)retArray[nCount]) != PrefabType.ModelPrefab)
			{
				retArray.RemoveAt(nCount);
			} else nCount++;
		}
		nOutFindFile = retArray.Count;

		return ArrayListToObjectNodes(retArray);
	}

	static NgAsset.ObjectNode[] ArrayListToObjectNodes(ArrayList retArray)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		return null;
#else
		NgAsset.ObjectNode[]	retObjs = new NgAsset.ObjectNode[retArray.Count];

		for (int n = 0; n < retArray.Count; n++)
		{
			retObjs[n] = new NgAsset.ObjectNode();
			if (retArray[n] is string)
			{
				retObjs[n].m_Object		= null;
				retObjs[n].m_AssetPath	= (string)retArray[n];
			} else {
				retObjs[n].m_Object		= (Object)retArray[n];
				retObjs[n].m_AssetPath	= AssetDatabase.GetAssetPath(retObjs[n].m_Object);
			}
		}
		return retObjs;
#endif
	}
}



