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
#endif

public class NgMaterial
{
	// ------------------------------------------------------------------
	public static bool IsMaterialColor(Material mat)
	{
		string[] propertyNames = { "_Color", "_TintColor", "_EmisColor" };

		if (mat != null)
			foreach (string name in propertyNames)
				if (mat.HasProperty(name))
					return true;
		return false;
	}

	public static string GetMaterialColorName(Material mat)
	{
		string[] propertyNames = { "_Color", "_TintColor", "_EmisColor" };

		if (mat != null)
			foreach (string name in propertyNames)
				if (mat.HasProperty(name))
					return name;
		return null;
	}

	public static Color GetMaterialColor(Material mat)
	{
		return GetMaterialColor(mat, Color.white);
	}

	public static Color GetMaterialColor(Material mat, Color defaultColor)
	{
		string[] propertyNames = { "_Color", "_TintColor", "_EmisColor" };

		if (mat != null)
			foreach (string name in propertyNames)
				if (mat.HasProperty(name))
					return mat.GetColor(name);
		return defaultColor;
	}

	public static void SetMaterialColor(Material mat, Color color)
	{
		string[] propertyNames = { "_Color", "_TintColor", "_EmisColor" };

		if (mat != null)
			foreach (string name in propertyNames)
				if (mat.HasProperty(name))
					mat.SetColor(name, color);
	}

	// ---------------------------------------------------------------------------------------
	public static bool IsSameMaterial(Material mat1, Material mat2, bool bCheckAddress)
	{
		if (bCheckAddress && (mat1 != mat2)) return false;
		if (mat2 == null) return false;

		if (mat1.shader != mat2.shader) return false;
		if (mat1.mainTexture != mat2.mainTexture) return false;
		if (mat1.mainTextureOffset != mat2.mainTextureOffset) return false;
		if (mat1.mainTextureScale != mat2.mainTextureScale) return false;
		if (IsSameColorProperty(mat1, mat2, "_Color") == false) return false;
		if (IsSameColorProperty(mat1, mat2, "_TintColor") == false) return false;
		if (IsSameColorProperty(mat1, mat2, "_EmisColor") == false) return false;
		if (IsSameFloatProperty(mat1, mat2, "_InvFade") == false) return false;
		if (IsMaskTexture(mat1) != IsMaskTexture(mat2)) return false;
		if (IsMaskTexture(mat1))
			if (GetMaskTexture(mat1) != GetMaskTexture(mat2)) return false;
		return true;
	}

	public static void CopyMaterialArgument(Material srcMat, Material tarMat)
	{
		tarMat.mainTexture			= srcMat.mainTexture;
		tarMat.mainTextureOffset	= srcMat.mainTextureOffset;
		tarMat.mainTextureScale		= srcMat.mainTextureScale;
		if (IsMaskTexture(srcMat) && IsMaskTexture(tarMat))
			SetMaskTexture(tarMat, GetMaskTexture(srcMat));
		NgMaterial.SetMaterialColor(tarMat, NgMaterial.GetMaterialColor(srcMat, new Color(0.5f, 0.5f, 0.5f, 0.5f)));
	}

	public static bool IsSameColorProperty(Material mat1, Material mat2, string propertyName)
	{
		bool has1 = mat1.HasProperty(propertyName);
		bool has2 = mat2.HasProperty(propertyName);

		if (has1 && has2)
		{
			return (mat1.GetColor(propertyName) == mat2.GetColor(propertyName));
		} else return (!has1 && !has2);
	}

	public static void CopyColorProperty(Material srcMat, Material tarMat, string propertyName)
	{
		bool has1 = srcMat.HasProperty(propertyName);
		bool has2 = tarMat.HasProperty(propertyName);

		if (has1 && has2)
			tarMat.SetColor(propertyName, srcMat.GetColor(propertyName));
	}

	public static bool IsSameFloatProperty(Material mat1, Material mat2, string propertyName)
	{
		bool has1 = mat1.HasProperty(propertyName);
		bool has2 = mat2.HasProperty(propertyName);

		if (has1 && has2)
		{
			return (mat1.GetFloat(propertyName) == mat2.GetFloat(propertyName));
		} else return (!has1 && !has2);
	}

	// --------------------------------------------------------------------------------

	public static Texture GetTexture(Material mat, bool bMask)
	{
		if (mat == null)
			return null;
		if (bMask)
		{
			if (IsMaskTexture(mat))
				return mat.GetTexture("_Mask");
			return null;
		}
		return mat.mainTexture;
	}

	public static void SetMaskTexture(Material mat, bool bMask, Texture newTexture)
	{
		if (mat == null)
			return;
		if (bMask)
			SetMaskTexture(mat, newTexture);
		else mat.mainTexture = newTexture;
	}

	public static bool IsMaskTexture(Material tarMat)
	{
		return tarMat.HasProperty("_Mask");
	}

	public static void SetMaskTexture(Material tarMat, Texture maskTex)
	{
		tarMat.SetTexture("_Mask", maskTex);
	}

	public static Texture GetMaskTexture(Material mat)
	{
		if (mat == null || mat.HasProperty("_Mask") == false)
			return null;
		return mat.GetTexture("_Mask");
	}

#if UNITY_EDITOR
	// ===========================================================================================================================
	public static Material SetSharedMaterial(Renderer tarRenderer, int tarIndex, Material srcMat)
	{
		Material[]	currentMats = tarRenderer.sharedMaterials;
		currentMats[tarIndex] = srcMat;
		tarRenderer.sharedMaterials = currentMats;
		return currentMats[tarIndex];
	}

	public static Material MoveSharedMaterial(Renderer tarRenderer, int nSrcIndex, int nTarIndex)
	{
		Material[]	currentMats = tarRenderer.sharedMaterials;
		Material	tmpMat		= currentMats[nSrcIndex];

		ArrayUtility.RemoveAt<Material>(ref currentMats, nSrcIndex);
		ArrayUtility.Insert<Material>(ref currentMats, nTarIndex, tmpMat);
		tarRenderer.sharedMaterials = currentMats;
		return currentMats[nTarIndex];
	}

	public static void AddSharedMaterial(Renderer tarRenderer)
	{
		Material[]	currentMats = tarRenderer.sharedMaterials;
		currentMats	= NgConvert.ResizeArray<Material>(currentMats, currentMats.Length+1);
		tarRenderer.sharedMaterials = currentMats;
	}

	public static void RemoveSharedMaterial(Renderer tarRenderer, int tarIndex)
	{
		Material[]	currentMats = tarRenderer.sharedMaterials;
		ArrayUtility.RemoveAt<Material>(ref currentMats, tarIndex);
		tarRenderer.sharedMaterials = currentMats;
	}

	// asset ----------------------------------------------------------------
	public static string SaveMaterial(Material newMat, string savePath, string saveMatName, bool bDevelopState)
	{
		// Create Path
		string	devMatDir	= "_MaterialsTool";
		string	userMatDir	= "_MaterialsUser";
		string	matDir;

		if (bDevelopState)
			 matDir = devMatDir;
		else matDir = userMatDir;

		string  matPath = NgFile.CombinePath(savePath, matDir);

		// Default SubDirectory
		if (NgAsset.ExistsDirectory(matPath) == false)
			AssetDatabase.CreateFolder(savePath, matDir);

		return SaveMaterial(newMat, matPath, saveMatName);
	}

	public static string SaveMaterial(Material newMat, string savePath, string saveMatName)
	{
		string		matPath			= "";
		int			nLoopCount		= 0;
		int			nUniqueCount	= 0;
		string		uniquePath;
		Object		existsObj;

		matPath	= NgFile.PathSeparatorNormalize(savePath);

		// Unique Name
		while (true)
		{
			string matName = saveMatName + (0<nUniqueCount ? "_"+nUniqueCount.ToString() : "") + ".mat";
			uniquePath	= NgFile.CombinePath(matPath, matName);
			existsObj = AssetDatabase.LoadAssetAtPath(uniquePath, typeof(Material));
			if (existsObj == null)
			{
				break;
			}
			nLoopCount++;
			nUniqueCount++;
			if (999 < nUniqueCount)
				nUniqueCount = 1;
			if (999 < nLoopCount)
			{
				Debug.LogError("Over Loop ----------------------");
				return "";
			}
		}

		AssetDatabase.CreateAsset(newMat, uniquePath);
//		NgUtil.LogMessage(FXMakertip.GetHsToolMessage("MATERIAL_NEWSAVED", "") + "\n" + uniquePath);
 		AssetDatabase.Refresh();
		AssetDatabase.SaveAssets();
		return uniquePath;
	}
#endif
}

