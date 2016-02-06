// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FXMakerAsset
{
	protected	static int			m_nTempSaveFrameIndex;

	// Prefab ---------------------------------------------------------------------------------------------------------------
	public static string CopyEffectPrefab(GameObject srcPrefab, string tarPath, bool bCutCopy)
	{
		NgUtil.LogMessage("CopyPrefab() - tarPath : " + tarPath);

		bool	bCopy		= false;
		string	errstr		= "err";
		string	srcPath		= AssetDatabase.GetAssetPath(srcPrefab);
		tarPath = NgFile.CombinePath(tarPath, NgFile.GetFilenameExt(srcPath));
		tarPath = AssetDatabase.GenerateUniqueAssetPath(tarPath);
		NgUtil.LogDevelop("CopyEffectPrefab() - tarPath : " + tarPath);
// 		tarPath = AssetDatabase.ValidateMoveAsset(srcPath, tarPath);

		NgUtil.LogDevelop("CopyEffectPrefab() - src : " + srcPath);
		NgUtil.LogDevelop("CopyEffectPrefab() - tar : " + tarPath);
		if (bCutCopy)
			 errstr = AssetDatabase.MoveAsset(srcPath, tarPath);
		else bCopy	= AssetDatabase.CopyAsset(srcPath, tarPath);

		// copy preview image
		if (errstr == ""  || bCopy == true)
		{
			string srcFile	= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), NgAsset.GetPrefabThumbFilename(srcPrefab));
			string tarFile	= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), NgAsset.GetPrefabThumbFilename(tarPath));
			if (bCutCopy)
				 AssetDatabase.MoveAsset(srcFile, tarFile);
			else AssetDatabase.CopyAsset(srcFile, tarFile);
		} else {
			Debug.LogWarning("CopyEffectPrefab() - CopyPreview : Faild");
		}
		AssetDatabaseRefresh();
		AssetDatabaseSaveAssets();
		return tarPath;
	}

	public static string CloneEffectPrefab(GameObject srcPrefab)
	{
		string createPath = NgAsset.ClonePrefab(srcPrefab);
		NgUtil.LogMessage("CloneEffectPrefab() - " + createPath);
		// copy preview image
		if (createPath != "")
		{
			string srcFile	= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), NgAsset.GetPrefabThumbFilename(srcPrefab));
			string tarFile	= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), NgAsset.GetPrefabThumbFilename(createPath));
			AssetDatabase.CopyAsset(srcFile, tarFile);
			AssetDatabaseRefresh();
		}
		return createPath;
	}

	public static void DeleteEffectPrefab(GameObject tarPrefab)
	{
		// delete preview image
		string filename	= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), NgAsset.GetPrefabThumbFilename(tarPrefab));
		NgUtil.LogMessage("DeleteEffectPrefab() - delete preview - " + filename);
		AssetDatabase.MoveAssetToTrash(filename);
//		AssetDatabase.DeleteAsset(filename);
		// check clipboard
		FXMakerClipboard.inst.CheckDeletePrefab(tarPrefab);
		// delete prefab
		NgAsset.DeletePrefab(tarPrefab);

		AssetDatabaseRefresh();
		AssetDatabaseSaveAssets();
	}

	public static GameObject SaveEffectPrefab(GameObject srcObject, GameObject tarPrefab)
	{
		NgUtil.LogMessage("SaveEffectPrefab() - " + tarPrefab);
		if (srcObject == null || tarPrefab == null)
		{
			Debug.LogError("SaveEffectPrefab arg is null");
			return null;
		}

		if (PrefabUtility.GetPrefabType(tarPrefab) == PrefabType.ModelPrefab)
		{
			FxmPopupManager.inst.ShowToolMessage("Can't save file, ModelPrefab");
			return null;
		}
//		PrefabUtility.ReconnectToLastPrefab(srcObject);
// 		Transform oldParent = srcObject.transform.parent;
		GameObject ret = PrefabUtility.ReplacePrefab(srcObject, tarPrefab, ReplacePrefabOptions.ConnectToPrefab);
// 		EditorUtility.SetDirty(tarPrefab);
//		Destroy(srcObject);

//		srcObject.transform.parent = oldParent;
// 		ret.transform.parent = oldParent;

//		AssetDatabase.SaveAssets();
		AssetDatabaseSaveAssets();
		return  ret;
	}

	public static string RenameEffectPrefab(GameObject srcObject, GameObject srcPrefab, string dstName)
	{
		string path = AssetDatabase.GetAssetPath(srcPrefab);
// 		NgUtil.LogMessage("RenameEffectPrefab() - path - " + path);
		Transform oldParent = srcObject.transform.parent;
		AssetDatabase.RenameAsset(path, dstName);
//		Destroy(srcObject);

// 		AssetDatabaseRefresh();
// 		AssetDatabaseSaveAssets();

		srcObject.transform.parent = oldParent;
//  		path = NgFile.CombinePath(NgUtil.TrimFilename(path), dstName + ".prefab");
//  		return NgAsset.LoadPrefab(path);
		return NgFile.CombinePath(NgFile.TrimFilenameExt(path), dstName + ".prefab");
	}

	public static void AssetDatabaseRefresh()
	{
// 		if (m_nTempRefreshFrameIndex == GetUnityFrameCount())
// 		{
// 			Debug.Log("AssetDatabaseRefresh skip --------------------------- ");
// 			return;
// 		}
// 		m_nTempRefreshFrameIndex = GetUnityFrameCount();
		AssetDatabase.Refresh();
	}

	public static void AssetDatabaseSaveAssets()
	{
		Debug.Log("save ----------------");
		if (m_nTempSaveFrameIndex == FXMakerMain.inst.GetUnityFrameCount())
		{
			Debug.Log("AssetDatabaseSaveAssets skip --------------------------- ");
			return;
		}
		m_nTempSaveFrameIndex = FXMakerMain.inst.GetUnityFrameCount();
		AssetDatabase.SaveAssets();
		EditorApplication.SaveAssets();
	}

	// PingObject ---------------------------------------------------------------------------------------------------------------
	// 하나의 obj를 선택한다.(Hierarchy or project)
	public static void SetPingObject(Object selObj)
	{
		if (FXMakerMain.inst.GetFocusUnityWindow() != FXMakerMain.UNITYWINDOW.GameView)
			return;

		Object[]	objs = {selObj};
		Selection.objects = new Object[0];
		Selection.objects = objs;

//   		Debug.Log("SetPingObject");

		EditorGUIUtility.PingObject(selObj);
	}

	// 두개의 obj를 선택한다.(Hierarchy or project)
	public static void SetPingObject(Object selObj1, Object selObj2)
	{
		if (FXMakerMain.inst.GetFocusUnityWindow() != FXMakerMain.UNITYWINDOW.GameView)
			return;

		Object[]	objs = { selObj1, selObj2 };
		Selection.objects = new Object[0];
		Selection.objects = objs;

//   		Debug.Log("SetPingObject");

 		EditorGUIUtility.PingObject(selObj1);
		EditorGUIUtility.PingObject(selObj2);
	}

	// objs를 선택한다.(Hierarchy or project)
	public static void SetPingObject(Object[] selObjs)
	{
		if (FXMakerMain.inst.GetFocusUnityWindow() != FXMakerMain.UNITYWINDOW.GameView)
			return;

		Selection.objects = new Object[0];
		Selection.objects = selObjs;

//   		Debug.Log("SetPingObject");

		foreach (Object obj in selObjs)
			EditorGUIUtility.PingObject(obj);
	}

}

#endif
