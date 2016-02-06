// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class NgAsset
{
	// ==========================================================================================================
	public static Texture2D GetAssetPreview(Object tarObj)
	{
#if (!UNITY_3_5)
		return AssetPreview.GetAssetPreview(tarObj);
#else
		return EditorUtility.GetAssetPreview(tarObj);
#endif
	}

	public static Texture2D GetMiniThumbnail(Object tarObj)
	{
#if (!UNITY_3_5)
		return AssetPreview.GetMiniThumbnail(tarObj);
#else
		return EditorUtility.GetMiniThumbnail(tarObj);
#endif
	}
	// ==========================================================================================================

	public class ObjectNode
	{
		public		Object			m_Object;
		public		string			m_AssetPath;
	}

	public static float		m_fOldCameraSize;

	public static bool ExistsDirectory(string strDir)
	{
		if (strDir[strDir.Length-1] != '/' && strDir[strDir.Length-1] != '\\')
			strDir = strDir + "/";
		DirectoryInfo	dir	= new DirectoryInfo(strDir);
		return (dir.Exists == true);
	}

	// ==========================================================================================================
	// Project의 특정폴더 밑에 있는 모든 폴더명을 가져온다.
	public static string[] GetFolderList(string strDir, string inclusiveName, string exclusiveName, int nMinArrayCount, out int nFindFolder)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		nFindFolder = 0;
		return null;
#else
		if (strDir[strDir.Length-1] != '/' && strDir[strDir.Length-1] != '\\')
			strDir = strDir + "/";

		int				nCount			= 0;
		DirectoryInfo	dir				= new DirectoryInfo(strDir);

		// CheckDir
		if (dir.Exists == false)
			Debug.LogError("Directory not found - " + strDir);

		DirectoryInfo[]	info			= dir.GetDirectories();
		List<string>	strList			= new List<string>();

		NgUtil.LogDevelop("GetFolderList() - " + strDir);

		// 프로젝트 폴더명 로드
		foreach (DirectoryInfo dirInfo in info)
		{
//			gUtil.LogDevelop("GetFolderList() - FindFolder - " + dirInfo.Name);
			if (exclusiveName != null && dirInfo.Name.Contains(exclusiveName) == true)
				continue;
			if (inclusiveName != null && dirInfo.Name.Contains(inclusiveName) == false)
				continue;
			strList.Add(dirInfo.Name);
			nCount++;
		}
		string[]	folderStrings	= new string[Mathf.Max(nMinArrayCount, strList.Count)];
		folderStrings = strList.ToArray();
		if (strList.Count < nMinArrayCount)
			folderStrings = NgConvert.ResizeArray<string>(folderStrings, nMinArrayCount);
		nFindFolder = nCount;
		return folderStrings;
#endif
	}

	// Project의 특정폴더안에 있는 파일 리스트를 가져온다.
	public static string[] GetFileList(string strDir, int nMaxFile, out int nFindFile)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		nFindFile = 0;
		return null;
#else
		if (strDir[strDir.Length-1] != '/' && strDir[strDir.Length-1] != '\\')
			strDir = strDir + "/";

		NgUtil.LogDevelop("GetFileList() - LoadDir - " + strDir);

		DirectoryInfo	dir			= new DirectoryInfo(strDir);

		// CheckDir
		if (dir.Exists == false)
			Debug.LogError("Directory not found - " + strDir);

		FileInfo[]		info		= dir.GetFiles();
		string[]		fileStrings	= new string[(0 < nMaxFile ? Mathf.Min(info.Length, nMaxFile) : info.Length)];
		int				nCount		= 0;

		foreach (FileInfo fileInfo in info)
		{
			if (0 < nMaxFile && nMaxFile <= nCount)
			{
				Debug.LogWarning("GetFileList - Over MaxCount!!!");
				break;
			}
			NgUtil.LogDevelop("GetFileList() - FindFile - " + fileInfo.Name);
			fileStrings[nCount++]	= fileInfo.Name;
		}
		nFindFile = nCount;
		return fileStrings;
#endif
	}

	// Project의 특정폴더안에서 특정path가 몇번째에 있는지 리턴한다. 0인덱스
	public static int FindPathIndex(string strDir, string findPath, string ext)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		return -1;
#else
		if (strDir[strDir.Length-1] != '/' && strDir[strDir.Length-1] != '\\')
			strDir = strDir + "/";

		findPath = NgFile.PathSeparatorNormalize(findPath);

		NgUtil.LogDevelop("FindPathIndex() - findPath - " + findPath);

		DirectoryInfo	dir			= new DirectoryInfo(strDir);

		// CheckDir
		if (dir.Exists == false)
			Debug.LogError("Directory not found - " + strDir);

		FileInfo[]		info		= dir.GetFiles();
		int				nIndex		= 0;
// 		string			filename	= NgUtil.GetFileFullName(findPath);
// 
// 		Debug.Log(info[0].FullName);
// 		Debug.Log(findPath);
		ext = ("." + ext).ToLower();
		foreach (FileInfo fileInfo in info)
		{
			if (fileInfo.Extension.ToLower() == ext)
			{
				if (NgFile.PathSeparatorNormalize(fileInfo.FullName).Contains(findPath))
					return nIndex;
				nIndex++;
			}
		}
		return -1;
#endif
	}

	public static GameObject CloneGameObject(GameObject srcObj)
	{
		GameObject newObj = NgObject.CreateGameObject(srcObj.transform.parent, srcObj);
		NgObject.SetActiveRecursively(newObj, false);
		newObj.name = newObj.name.Replace("(Clone)", "");
		return newObj;
	}

#if UNITY_EDITOR
	//=====================================================================================================================================
	public static GameObject[] GetPrefabList(string strDir, bool bIncludeMesh, bool bRecursively, int nMaxFile, out int nOutFindFile)
	{
		ArrayList	retArray;
		int			nCount = 0;

		if (bRecursively)
			 retArray = GetResourceFileRecursively<GameObject>(strDir, null, nMaxFile, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);
		else retArray = GetResourceFiles<GameObject>(strDir, null, nMaxFile, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);

		if (bIncludeMesh == false)
		{
			while (true)
			{
				if (retArray.Count <= nCount)
					break;
				if (PrefabUtility.GetPrefabType((Object)retArray[nCount]) == PrefabType.ModelPrefab)
				{
// 					Debug.Log(((GameObject)retArray[nCount]).name);
					retArray.RemoveAt(nCount);
				}
				else nCount++;
			}
			nOutFindFile = retArray.Count;
		}

		return NgConvert.ToArray<GameObject>(retArray);
	}

	public static ObjectNode[] GetPrefabList(string strDir, bool bIncludeMesh, bool bRecursively, int nMaxFile, bool bAsyncLoad, NgEnum.PREFAB_TYPE prefabType, out int nOutFindFile)
	{
		ArrayList	retArray;
		int			nCount = 0;

		if (bRecursively)
			 retArray = GetResourceFileRecursively<GameObject>(strDir, null, nMaxFile, bAsyncLoad, prefabType, out nOutFindFile);
		else retArray = GetResourceFiles<GameObject>(strDir, null, nMaxFile, bAsyncLoad, prefabType, out nOutFindFile);
		ObjectNode[] retObjNodes = ArrayListToObjectNodes(retArray);

		if (bIncludeMesh == false)
		{
			while (true)
			{
				if (retObjNodes.Length <= nCount)
					break;
				if (NgFile.GetFileExt(retObjNodes[nCount].m_AssetPath).ToLower() != "prefab")
				{
// 					Debug.Log(((GameObject)retArray[nCount]).name);
					ArrayUtility.RemoveAt<ObjectNode>(ref retObjNodes, nCount);
				}
				else nCount++;
			}
			nOutFindFile = retObjNodes.Length;
		}
		return retObjNodes;
	}

	public static ObjectNode[] GetMeshList(string strDir, bool bRecursively, int nMaxFile, out int nOutFindFile)
	{
		ArrayList	retArray;
		int			nCount = 0;

		if (bRecursively)
			 retArray = GetResourceFileRecursively<GameObject>(strDir, null, nMaxFile, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);
		else retArray = GetResourceFiles<GameObject>(strDir, null, nMaxFile, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);

		while (true)
		{
			if (retArray.Count <= nCount)
				break;
			if (PrefabUtility.GetPrefabType((Object)retArray[nCount]) != PrefabType.ModelPrefab)
			{
				MeshFilter meshFilter = ((GameObject)retArray[nCount]).GetComponent<MeshFilter>();
				if (meshFilter != null && meshFilter.sharedMesh != null)
				{
					nCount++;
					continue;
				}
				retArray.RemoveAt(nCount);
			} else nCount++;
		}
		nOutFindFile = retArray.Count;

		return ArrayListToObjectNodes(retArray);
	}

	public static ObjectNode[] GetTextureList(string strDir, bool bRecursively, int nMaxFile, bool bAsyncLoad, out int nOutFindFile)
	{
		ArrayList retArray;
		if (bRecursively)
			 retArray = GetResourceFileRecursively<Texture>(strDir, null, nMaxFile, bAsyncLoad, NgEnum.PREFAB_TYPE.All, out nOutFindFile);
		else retArray = GetResourceFiles<Texture>(strDir, null, nMaxFile, bAsyncLoad, NgEnum.PREFAB_TYPE.All, out nOutFindFile);
		return ArrayListToObjectNodes(retArray);
	}

	public static Material[] GetMaterialList(string strDir, bool bRecursively, int nMaxFile, out int nOutFindFile)
	{
		if (bRecursively)
		{
			ArrayList retArray = GetResourceFileRecursively<Material>(strDir, null, nMaxFile, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);
			return NgConvert.ToArray<Material>(retArray);
		}
		return NgConvert.ToArray<Material>(GetResourceFiles<Material>(strDir, null, nMaxFile, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile));
	}

	public static GameObject[] GetCurvePrefabList(string strDir, string containFileName, bool bRecursively, int nMaxFile, out int nOutFindFile)
	{
		if (bRecursively)
		{
			ArrayList retArray = GetResourceFileRecursively<GameObject>(strDir, containFileName, nMaxFile, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);
			return NgConvert.ToArray<GameObject>(retArray);
		}
		return NgConvert.ToArray<GameObject>(GetResourceFiles<GameObject>(strDir, containFileName, nMaxFile, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile));
	}

	public static ObjectNode[] GetTexturePathList(string strDir, bool bRecursively, int nMaxFile, out int nOutFindFile)
	{
		ArrayList retArray;
		if (bRecursively)
			 retArray = GetResourceFileRecursively<Texture>(strDir, null, nMaxFile, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);
		else retArray = GetResourceFiles<Texture>(strDir, null, nMaxFile, false, NgEnum.PREFAB_TYPE.All, out nOutFindFile);
		return ArrayListToObjectNodes(retArray);
	}

	static ObjectNode[] ArrayListToObjectNodes(ArrayList retArray)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		return null;
#else
		ObjectNode[]	retObjs = new ObjectNode[retArray.Count];

		for (int n = 0; n < retArray.Count; n++)
		{
			retObjs[n] = new ObjectNode();
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

	// ----------------------------------------------------------------------------------------------------------------------------------
	public static ArrayList GetResourceFileRecursively<TT>(string strDir, string containFileName, int nMaxFile, bool bAsyncLoad, NgEnum.PREFAB_TYPE prefabType, out int nOutFindFile) where TT : Object
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		nOutFindFile = 0;
		return null;
#else
		if (strDir[strDir.Length-1] != '/' && strDir[strDir.Length-1] != '\\')
			strDir = strDir + "/";

		DirectoryInfo	dir		= new DirectoryInfo(strDir);

		// CheckDir
		if (dir.Exists == false)
			Debug.LogError("Directory not found - " + strDir);

		ArrayList		retArray;
		DirectoryInfo[]	info	= dir.GetDirectories();
		int				nFindFile;

		retArray = GetResourceFiles<TT>(strDir, containFileName, nMaxFile, bAsyncLoad, prefabType, out nFindFile);
		nOutFindFile = nFindFile;
		if (nMaxFile != 0)
		{
			nMaxFile = nMaxFile - nFindFile;
			if (nMaxFile <= 0)
				return retArray;
		}

		foreach (DirectoryInfo dirInfo in info)
		{
			retArray.AddRange(GetResourceFileRecursively<TT>(NgFile.CombinePath(strDir, dirInfo.Name), containFileName, nMaxFile, bAsyncLoad, prefabType, out nFindFile));
			nOutFindFile += nFindFile;
			if (nMaxFile != 0)
			{
				nMaxFile = nMaxFile - nFindFile;
				if (nMaxFile <= 0)
					break;
			}
		}
		return retArray;
#endif
	}

	public static ArrayList GetResourceFiles<TT>(string strDir, string containFileName, int nMaxFile, bool bAsyncLoad, NgEnum.PREFAB_TYPE prefabType, out int nOutFindFile) where TT : Object
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		nOutFindFile = 0;
		return null;
#else
		if (strDir[strDir.Length-1] != '/' && strDir[strDir.Length-1] != '\\')
			strDir = strDir + "/";
		strDir = NgFile.PathSeparatorNormalize(strDir);

		NgUtil.LogDevelop("GetPrefabList() - LoadDir - " + strDir);
		ArrayList	retArray	= new ArrayList();

		{
			DirectoryInfo	dir		= new DirectoryInfo(strDir);

			// CheckDir
			if (dir.Exists == false)
				Debug.LogError("Directory not found - " + strDir);

			FileInfo[]	info	= dir.GetFiles();

			foreach (FileInfo fileInfo in info)
			{
				if (0 < nMaxFile && nMaxFile <= retArray.Count)
				{
					Debug.LogWarning("GetPrefabList - Over MaxCount!!!");
					break;
				}
				if (containFileName != null)
					if (fileInfo.Name.Contains(containFileName) == false)
						continue;

//	 			TT obj	= (TT)Resources.LoadAssetAtPath(strDir + fileInfo.Name, typeof(TT));
				TT obj	= null;
//				if (fileInfo.Name.Contains(".prefab"))
//				if (fileInfo.Name.Contains(".prefab") || fileInfo.Name.Contains(".png"))
				string	compareName = NgFile.GetFileExt(fileInfo.Name).ToLower();
				if (typeof(TT) == typeof(GameObject))
				{
					if (compareName == "mat")		continue;
					if (compareName == "png")		continue;
					if (compareName == "tga")		continue;
					if (compareName == "jpg")		continue;
					if (compareName == "psd")		continue;
					if (compareName == "tif")		continue;
					if (compareName == "wav")		continue;
					if (bAsyncLoad && (compareName == "prefab"))
					{
						retArray.Add(strDir + fileInfo.Name);
						continue;
					}
				} else
				if (typeof(TT) == typeof(Texture))
				{
					if (compareName == "mat")		continue;
					if (compareName == "prefab")	continue;
					if (compareName == "fbx")		continue;
					if (compareName == "wav")		continue;
					if (bAsyncLoad && (compareName == "png" || compareName == "tga" || compareName == "jpg" || compareName == "psd" || compareName == "tif"))
					{
						retArray.Add(strDir + fileInfo.Name);
						continue;
					}
				} else
				if (typeof(TT) == typeof(Material))
				{
					if (compareName != "mat")		continue;
					if (bAsyncLoad && (compareName == "mat"))
					{
						retArray.Add(strDir + fileInfo.Name);
						continue;
					}
				} else
				if (typeof(TT) == typeof(AudioClip))
				{
					if (compareName == "mat")		continue;
					if (compareName == "png")		continue;
					if (compareName == "tga")		continue;
					if (compareName == "jpg")		continue;
					if (compareName == "psd")		continue;
					if (compareName == "tif")		continue;
					if (compareName == "prefab")	continue;
					if (compareName == "fbx")		continue;
				}

				// 싱크 로드
				obj	= (TT)AssetDatabase.LoadAssetAtPath(strDir + fileInfo.Name, typeof(TT));
// 				Debug.Log(strDir + fileInfo.Name);
// 				Debug.Log(AssetDatabase.GetAssetPath(obj));


				if (typeof(TT) == typeof(GameObject) && prefabType != NgEnum.PREFAB_TYPE.All)
				{
					GameObject chkObj = obj as GameObject;
					if (chkObj == null)
						continue;
// 					Debug.Log(prefabType);
// 					Debug.Log(chkObj.GetComponent<ParticleEmitter>());
// 					Debug.Log(chkObj.GetComponent<ParticleSystem>());
					if (prefabType == NgEnum.PREFAB_TYPE.ParticleSystem && (chkObj.GetComponent<ParticleEmitter>() == null && chkObj.GetComponent<ParticleSystem>() == null))
						continue;
					if (prefabType == NgEnum.PREFAB_TYPE.NcSpriteFactory && (chkObj.GetComponent<NcSpriteFactory>() == null))
						continue;
					if (prefabType == NgEnum.PREFAB_TYPE.LegacyParticle && (chkObj.GetComponent<ParticleEmitter>() == null))
						continue;
				}

				if (obj != null)
					retArray.Add(obj);
			}

			NgUtil.LogDevelop("GetPrefabList() - nCount - " + retArray.Count);
			nOutFindFile = retArray.Count;
			return retArray;
		}
#endif
	}

	// GameObject의 밑에 있는 모든 GameObject명을 가져온다.
	public static string[] GetChildNameList(GameObject rootObj, int nMaxChild, out int nFindChild)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		nFindChild = 0;
		return null;
#else
		string[]	objStrings	= new string[nMaxChild];
		int			nCount		= 0;

		for (int n=0; n < rootObj.transform.childCount; n++)
		{
			if (nMaxChild <= nCount)
			{
				Debug.LogWarning("GetChildNameList - Over MaxCount!!!");
				break;
			}
			Transform	transChild = rootObj.transform.GetChild(n);
			objStrings[n] = transChild.name;
			nCount++;
		}
		nFindChild = nCount;
		return objStrings;
#endif
	}

	// 특정폴더의 prefab을 로드해서 target 밑으로 생성한다.
	public static void LoadPrefabList(string strPrefabDir, GameObject targetObject)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		return;
#else
		if (strPrefabDir[strPrefabDir.Length-1] != '/' && strPrefabDir[strPrefabDir.Length-1] != '\\')
			strPrefabDir = strPrefabDir + "/";

		// load Current Group
		DirectoryInfo	dir	= new DirectoryInfo(strPrefabDir);

		// CheckDir
		if (dir.Exists == false)
			Debug.LogError("Directory not found - " + strPrefabDir);

		FileInfo[]	info	= dir.GetFiles();

		NgUtil.LogDevelop("BuildPrefabList() - LoadDir - " + strPrefabDir);
//		Object[]	objs	= AssetDatabase.LoadAllAssetsAtPath(loaddir);

		// 기존것 모두 삭제
 		NgObject.RemoveAllChildObject(targetObject, true);

		// 선택된 그룹폴더 asset 로드 (project to Hierarchy)
		foreach (FileInfo fileInfo in info)
		{
			NgUtil.LogDevelop("BuildPrefabList() - FindFile - " + fileInfo.Name);
			GameObject prefab	= (GameObject)AssetDatabase.LoadAssetAtPath(strPrefabDir + fileInfo.Name, typeof(GameObject));
			NgUtil.LogDevelop("BuildPrefabList() - " + prefab.name);
			GameObject obj		= NgObject.CreateGameObject(targetObject, prefab);
			NgObject.SetActiveRecursively(obj, false);
		}
#endif
	}

	// prefab만 로드해서 리턴
	public static GameObject LoadPrefab(string strPrefabPath)
	{
 		NgUtil.LogDevelop("LoadPrefab() - strPrefabPath - " + strPrefabPath);

		// 선택된 asset 로드 (project to Hierarchy)
		GameObject prefab	= (GameObject)AssetDatabase.LoadAssetAtPath(strPrefabPath, typeof(GameObject));
		return prefab;
	}

	// 특정 prefab을 로드해서 target 밑으로 생성한다.
	public static GameObject LoadPrefab(string strPrefabPath, GameObject targetObject)
	{
		NgUtil.LogDevelop("LoadPrefab() - strPrefabPath - " + strPrefabPath);

		// 선택된 asset 로드 (project to Hierarchy)
		GameObject prefab	= (GameObject)AssetDatabase.LoadAssetAtPath(strPrefabPath, typeof(GameObject));
		GameObject obj		= NgObject.CreateGameObject(targetObject, prefab);
		NgObject.SetActiveRecursively(obj, false);
//		gUtil.LogDevelop(AssetDatabase.GetAssetPath(prefab));
		return obj;
	}
	public static GameObject LoadPrefab(GameObject srcPrefab, GameObject targetObject)
	{
		string path = AssetDatabase.GetAssetPath(srcPrefab);
		NgUtil.LogDevelop("LoadPrefab() - strPrefabPath - " + path);

		// 선택된 asset 로드 (project to Hierarchy)
// 		GameObject prefab	= (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
//		GameObject obj		= NgObject.CreateGameObject(targetObject, prefab);
		GameObject obj		= PrefabUtility.InstantiatePrefab(srcPrefab) as GameObject;
		obj.transform.parent = targetObject.transform;
		
		NgObject.SetActiveRecursively(obj, false);
		return obj;
	}

	// DefaultPrefab을 이용해서 새로운 prefab 생성
	public static string CreateDefaultUniquePrefab(GameObject srcPrefab, string dstPath)
	{
		NgUtil.LogMessage("CreateDefaultUniquePrefab() - dstPath - " + dstPath);
		string srcPath = AssetDatabase.GetAssetPath(srcPrefab);
		string UnqPath = AssetDatabase.GenerateUniqueAssetPath(dstPath);
		if (AssetDatabase.CopyAsset(srcPath, UnqPath))
		{
			AssetDatabase.Refresh();
			return UnqPath;
		} else
		{
			Debug.LogWarning("Copy Error !!!");
			return "";
		}
//		EditorUtility.CreateEmptyPrefab(defaultPrefab);
	}

	// 특정 prefab의 삭제한다.
	public static void DeletePrefab(GameObject srcPrefab)
	{
		string path = AssetDatabase.GetAssetPath(srcPrefab);
		NgUtil.LogMessage("DeletePrefab() - path - " + path);
// 		PrefabUtility.SetPropertyModifications
// 		AssetDatabase.DeleteAsset(path);
		AssetDatabase.MoveAssetToTrash(path);
	}

	// 특정 prefab의 clone를 prjoect에 생성한다.
	public static string ClonePrefab(GameObject srcPrefab)
	{
		NgUtil.LogMessage("ClonePrefab() - srcPrefab - " + srcPrefab);
		string srcPath = AssetDatabase.GetAssetPath(srcPrefab);
		string UnqPath = AssetDatabase.GenerateUniqueAssetPath(srcPath);
		if (AssetDatabase.CopyAsset(srcPath, UnqPath))
		{
			AssetDatabase.Refresh();
			return UnqPath;
		} else
		{
			Debug.LogWarning("Copy Error !!!");
			return "";
		}
	}

	// 특정 prefab의 이름을 변경한다.
	public static void RenamePrefab(GameObject srcPrefab, string dstName)
	{
		string path = AssetDatabase.GetAssetPath(srcPrefab);
		NgUtil.LogMessage("RenamePrefab() - path - " + path);
		AssetDatabase.RenameAsset(path, dstName);
	}

	// 특정 prefab을 기존 project에 저장한다.
// 	public static GameObject SavePrefab(GameObject srcObject, GameObject tarPrefab)
// 	{
// //		InstantiatePrefab
// //		CreateEmptyPrefab 
// // 		ReplacePrefab
// // 		ReconnectToLastPrefab
// // 		ResetToPrefabState
// // 		ResetGameObjectToPrefabState 
// // 		EditorUtility.ReconnectToLastPrefab(srcObject);
// 		GameObject aa = EditorUtility.ReplacePrefab(srcObject, tarPrefab, ReplacePrefabOptions.Default);
// // 		Object.DestroyImmediate(srcObject);
// //		AssetDatabase.Refresh();
// //		AssetDatabase.SaveAssets();
// 		return aa;
// 	}

	// prefab의 thumb파일 fullpath리턴
	public static string GetPrefabThumbFilename(GameObject prefabObj)
	{
		string assetPath	= AssetDatabase.GetAssetPath(prefabObj);
		string assetGuid	= AssetDatabase.AssetPathToGUID(assetPath);
		return assetGuid + ".png";
	}
	public static string GetPrefabThumbFilename(string assetPath)
	{
		string assetGuid	= AssetDatabase.AssetPathToGUID(assetPath);
		return assetGuid + ".png";
	}

	// 현재 화면을 저장한다.
	public static void ScreenshotCapture(string filename, float captureZoomRate)
	{
		m_fOldCameraSize = Camera.main.fieldOfView;
		Camera.main.fieldOfView = m_fOldCameraSize / captureZoomRate;
		Application.CaptureScreenshot(filename);
	}

	// 저장된 화면을 특정 디렉토리로 이동한다.
	public static void ScreenshotSave(string targetPath, string filename)
	{
//		AssetDatabase.ImportAsset("../" + filename);
//		File.Copy("../" + filename, targetPath + filename);
		Camera.main.fieldOfView = m_fOldCameraSize;
 		FileUtil.CopyFileOrDirectory(filename, targetPath + filename);
		FileUtil.DeleteFileOrDirectory(filename);
		Debug.Log("ScreenshotSave -------------------------------");
		AssetDatabase.Refresh();
	}

	public static Texture2D Capture(Rect rect)
	{
		Texture2D tex2D = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
		tex2D.ReadPixels(rect, 0, 0);
		tex2D.Apply(); // That's the heavy part, it takes a lot of time.
		return tex2D;
	}

	public static Texture2D CaptureAlpha(Rect rect)
	{
		Texture2D tex2D = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
// 		RenderBuffer	depthBuffer	= Graphics.activeDepthBuffer;

		tex2D.ReadPixels(rect, 0, 0);
		tex2D.Apply(); // That's the heavy part, it takes a lot of time.
		Debug.LogWarning("need 		Object.DestroyImmediate(Texture2D);");
		return tex2D;
	}

	// 현재 화면을 저장한다.
	public static void CaptureRectPreprocess(float captureZoomRate)
	{
		//	직접 세이브방식... 
		if (Camera.main != null)
		{
			if (Camera.main.orthographic)
			{
				m_fOldCameraSize = Camera.main.orthographicSize;
				Camera.main.orthographicSize = m_fOldCameraSize / captureZoomRate;
			} else {
				m_fOldCameraSize = Camera.main.fieldOfView;
				Camera.main.fieldOfView = m_fOldCameraSize / captureZoomRate;
			}
		}
	}

	public static void CaptureResize(string assetPath)
	{
		Debug.Log(assetPath);
		TextureImporter	texImporter = TextureImporter.GetAtPath(assetPath) as TextureImporter;
		TextureImporterSettings settings = new TextureImporterSettings();
		texImporter.ReadTextureSettings(settings);

 		texImporter.textureType		= TextureImporterType.Image;
		texImporter.wrapMode		= TextureWrapMode.Clamp;
		texImporter.filterMode		= FilterMode.Bilinear;
		texImporter.anisoLevel		= 0;
		texImporter.maxTextureSize	= FXMakerLayout.m_nThumbImageSize;
 		texImporter.textureFormat	= TextureImporterFormat.Automatic16bit;
		AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
	}

	// 저장된 화면을 thumb 디렉토리로 이동한다.
	public static void CaptureRectSave(Rect guirect, string targetPath, string filename)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
#else
		//	직접 세이브방식... 
		Texture2D	tex			= Capture(guirect);
		byte[]		bytes		= tex.EncodeToPNG();
		File.WriteAllBytes(NgFile.CombinePath(targetPath, filename), bytes);
// 		Debug.Log("File.WriteAllBytes -------------------------------");
		AssetDatabase.Refresh();
		Object.DestroyImmediate(tex);
		CaptureResize(NgFile.CombinePath(targetPath, filename));
		if (Camera.main.orthographic)
			Camera.main.orthographicSize = m_fOldCameraSize;
		else Camera.main.fieldOfView = m_fOldCameraSize;
#endif
	}

	// 미리보기용 이미지를 리턴한다.
	public static Texture GetPrefabThumb(string targetPath, GameObject prefabObj)
	{
		string filename	= GetPrefabThumbFilename(prefabObj);
		Texture tex = (Texture)AssetDatabase.LoadAssetAtPath(NgFile.CombinePath(targetPath, filename), typeof(Texture));
		if (tex != null)
			return tex;
		Texture2D tex2 = GetAssetPreview(prefabObj);
		if (tex2 != null)
			tex = GameObject.Instantiate(tex2) as Texture2D;
		return tex;
	}
	public static Texture GetThumbImage(string targetPath, string filename)
	{
		Texture tex = (Texture)AssetDatabase.LoadAssetAtPath(NgFile.CombinePath(targetPath, filename), typeof(Texture));
		if (tex != null)
			return tex;
		return null;
	}

	public static void SetSelectObject(Object selObj)
	{
		Object[]	objs = { selObj };
		Selection.objects = new Object[0];
		Selection.objects = objs;

//  		Debug.Log("SetSelectObject");
	}

#endif

}

#endif
