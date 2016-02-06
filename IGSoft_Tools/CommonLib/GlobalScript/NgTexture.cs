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
#endif

using System.Collections;
using System.Collections.Generic;

public class NgTexture
{
	// Load --------------------------------------------------------------------------
	public static void UnloadTextures(GameObject rootObj)
	{
		if (rootObj == null)
			return;
		Renderer[]	rens = rootObj.GetComponentsInChildren<Renderer>(true);
		foreach (Renderer ren in rens)
		{
			if (ren.material != null && ren.material.mainTexture != null)
			{
				Debug.Log("UnloadTextures - " + ren.material.mainTexture);
				Resources.UnloadAsset(ren.material.mainTexture);
			}
		}

	}

	// Texture Function --------------------------------------------------------------
	public static Texture2D CopyTexture(Texture2D srcTex, Texture2D tarTex)
	{
		Color32[] colBuf = srcTex.GetPixels32();
		tarTex.SetPixels32(colBuf);
		tarTex.Apply(false);
		return tarTex;
	}

	public static Texture2D InverseTexture32(Texture2D srcTex, Texture2D tarTex)
	{
		Color32[] colBuf = srcTex.GetPixels32();

		for (int n = 0; n < colBuf.Length; n++)
			colBuf[n].a = (byte)(255 - colBuf[n].a);
		tarTex.SetPixels32(colBuf);
		tarTex.Apply(false);
		return tarTex;
	}

	public static Texture2D CombineTexture(Texture2D baseTexture, Texture2D combineTexture)
	{
		Texture2D returnTexture = new Texture2D(baseTexture.width, baseTexture.height, baseTexture.format, false);
		Debug.LogWarning("need 	Object.DestroyImmediate(returnTexture);");

		Color[] baseTexturePixels		= baseTexture.GetPixels();
		Color[] combineTexturePixels	= combineTexture.GetPixels();
		Color[] colorList				= new Color[baseTexturePixels.Length];

		int pixelLength = baseTexturePixels.Length;
		for(int p = 0; p < pixelLength; p++)
		{
		   colorList[p] = Color.Lerp(baseTexturePixels[p], combineTexturePixels[p], combineTexturePixels[p].a);
		}

		returnTexture.SetPixels(colorList);
		returnTexture.Apply(false);
		return returnTexture;
	}

	public static bool CompareTexture(Texture2D tex1, Texture2D tex2)
	{
		Color[] buf1 = tex1.GetPixels();
		Color[] buf2 = tex2.GetPixels();

		if (buf1.Length != buf2.Length)
			return false;

		int pixelLength = buf1.Length;
		for(int n = 0; n < pixelLength; n++)
			if (buf1[n] != buf2[n])
				return false;
		return true;
	}

	public static Texture2D FindTexture(List<Texture2D> findList, Texture2D findTex)
	{
		for(int n = 0; n < findList.Count; n++)
			if (CompareTexture(findList[n], findTex))
				return findList[n];
		return null;
	}

	public static int FindTextureIndex(List<Texture2D> findList, Texture2D findTex)
	{
		for(int n = 0; n < findList.Count; n++)
			if (CompareTexture(findList[n], findTex))
				return n;
		return -1;
	}

	public static Texture2D CopyTexture(Texture2D srcTex, Rect srcRect, Texture2D tarTex, Rect tarRect)
	{
		Color[]		srcBuf;
		srcBuf		= srcTex.GetPixels((int)srcRect.x, (int)srcRect.y, (int)srcRect.width, (int)srcRect.height);
		tarTex.SetPixels((int)tarRect.x, (int)tarRect.y, (int)tarRect.width, (int)tarRect.height, srcBuf);
 		tarTex.Apply();
		return tarTex;
	}

	public static Texture2D CopyTextureHalf(Texture2D srcTexture, Texture2D tarHalfTexture)
	{
		if (srcTexture.width != tarHalfTexture.width*2)
			Debug.LogError("size error");
		if (srcTexture.height != tarHalfTexture.height*2)
			Debug.LogError("size error");

		Color[] srcBuf	= srcTexture.GetPixels();
		Color[] tarBuf	= new Color[srcBuf.Length/4];
		int		nWidth	= tarHalfTexture.width;
		int		nHeight	= tarHalfTexture.height;
		int		nCount	= 0;
		int		nMultiX	= 2;
		int		nMultiY	= nMultiX*2;

		for(int y = 0; y < nHeight; y++)
			for(int x = 0; x < nWidth; x++, nCount++)
			   tarBuf[nCount] = Color.Lerp(Color.Lerp(srcBuf[y*nWidth*nMultiY+x*nMultiX], srcBuf[y*nWidth*nMultiY+x*nMultiX+1], 0.5f), Color.Lerp(srcBuf[y*nWidth*nMultiY+nWidth*nMultiX+x*nMultiX], srcBuf[y*nWidth*nMultiY+nWidth*nMultiX+x*nMultiX+1], 0.5f), 0.5f);

		tarHalfTexture.SetPixels(tarBuf);
		tarHalfTexture.Apply(false);
		return tarHalfTexture;
	}

	public static Texture2D CopyTextureQuad(Texture2D srcTexture, Texture2D tarQuadTexture)
	{
		if (srcTexture.width != tarQuadTexture.width*4)
			Debug.LogError("size error");
		if (srcTexture.height != tarQuadTexture.height*4)
			Debug.LogError("size error");

		Color[] srcBuf	= srcTexture.GetPixels();
		Color[] tarBuf	= new Color[srcBuf.Length/16];
		int		nWidth	= tarQuadTexture.width;
		int		nHeight	= tarQuadTexture.height;
		int		nCount	= 0;
		int		nMultiX	= 4;
		int		nMultiY	= nMultiX*4;

		for(int y = 0; y < nHeight; y++)
			for(int x = 0; x < nWidth; x++, nCount++)
			   tarBuf[nCount] = Color.Lerp(Color.Lerp(Color.Lerp(Color.Lerp(srcBuf[y*nWidth*nMultiY+x*nMultiX  ], srcBuf[y*nWidth*nMultiY+x*nMultiX+1], 0.5f), Color.Lerp(srcBuf[y*nWidth*nMultiY+nWidth*nMultiX+x*nMultiX  ], srcBuf[y*nWidth*nMultiY+nWidth*nMultiX+x*nMultiX+1], 0.5f), 0.5f),
													  Color.Lerp(Color.Lerp(srcBuf[y*nWidth*nMultiY+x*nMultiX+2], srcBuf[y*nWidth*nMultiY+x*nMultiX+3], 0.5f), Color.Lerp(srcBuf[y*nWidth*nMultiY+nWidth*nMultiX+x*nMultiX+2], srcBuf[y*nWidth*nMultiY+nWidth*nMultiX+x*nMultiX+3], 0.5f), 0.5f), 0.5f),
										   Color.Lerp(Color.Lerp(Color.Lerp(srcBuf[y*nWidth*nMultiY+nWidth*nMultiX*2+x*nMultiX  ], srcBuf[y*nWidth*nMultiY+nWidth*nMultiX*2+x*nMultiX+1], 0.5f), Color.Lerp(srcBuf[y*nWidth*nMultiY+nWidth*nMultiX*3+x*nMultiX  ], srcBuf[y*nWidth*nMultiY+nWidth*nMultiX*3+x*nMultiX+1], 0.5f), 0.5f), 
													  Color.Lerp(Color.Lerp(srcBuf[y*nWidth*nMultiY+nWidth*nMultiX*2+x*nMultiX+2], srcBuf[y*nWidth*nMultiY+nWidth*nMultiX*2+x*nMultiX+3], 0.5f), Color.Lerp(srcBuf[y*nWidth*nMultiY+nWidth*nMultiX*3+x*nMultiX+2], srcBuf[y*nWidth*nMultiY+nWidth*nMultiX*3+x*nMultiX+3], 0.5f), 0.5f), 0.5f), 0.5f);

		tarQuadTexture.SetPixels(tarBuf);
		tarQuadTexture.Apply(false);
		return tarQuadTexture;
	}

	public static Texture2D CopyTexture(Texture2D srcTex, Texture2D tarTex, Rect drawRect)
	{
		Rect srcRect	= new Rect(0, 0, srcTex.width, srcTex.height);
		if (drawRect.x < 0)
		{
			srcRect.x		-= drawRect.x;
			srcRect.width	+= drawRect.x;
			drawRect.width	+= drawRect.x;
			drawRect.x		 = 0;
		}
		if (drawRect.y < 0)
		{
			srcRect.y		-= drawRect.y;
			srcRect.height	+= drawRect.y;
			drawRect.height += drawRect.y;
			drawRect.y		 = 0;
		}
		if (tarTex.width < drawRect.x+drawRect.width)
		{
			srcRect.width	-= drawRect.x+drawRect.width - tarTex.width;
			drawRect.width	-= drawRect.x+drawRect.width - tarTex.width;
		}
		if (tarTex.height < drawRect.y+drawRect.height)
		{
			srcRect.height	-= drawRect.y+drawRect.height - tarTex.height;
			drawRect.height	-= drawRect.y+drawRect.height - tarTex.height;
		}
// 		Debug.Log("srcRect " + srcRect);
// 		Debug.Log("drawRect " + drawRect);
 		return CopyTexture(srcTex, srcRect, tarTex, drawRect);
	}

#if UNITY_EDITOR
	// ===========================================================================================================================
	public static void ReimportTexture(string texturePath, bool bGUITexture, TextureWrapMode wrapMode, FilterMode filterMode, int anisoLevel, int maxTextureSize, TextureImporterFormat textureFormat)
	{
// 		Debug.Log("ChangeImportTextureToGUI - " + assetPath);
		TextureImporter	texImporter = TextureImporter.GetAtPath(texturePath) as TextureImporter;

		TextureImporterSettings settings = new TextureImporterSettings();
		texImporter.ReadTextureSettings(settings);
// 		settings.ApplyTextureType(TextureImporterType.GUI, false);
// 		texImporter.SetTextureSettings(settings);

		texImporter.wrapMode			= wrapMode;
		texImporter.filterMode			= filterMode;
		texImporter.anisoLevel			= anisoLevel;

		if (bGUITexture)
	 		 texImporter.textureType	= TextureImporterType.GUI;
		else texImporter.textureType	= TextureImporterType.Image;

		texImporter.maxTextureSize		= maxTextureSize;
		texImporter.textureFormat		= textureFormat;
//  	texImporter.npotScale			= TextureImporterNPOTScale.None;
		AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceSynchronousImport);
	}

	public static string UniqueTexturePath(string basePath, string texName)
	{
		// Unique Name
		int			nLoopCount		= 0;
		int			nUniqueCount	= 0;
		string		uniquePath;
		Object		existsObj;

		while (true)
		{
			string matName = texName + (0<nUniqueCount ? "_"+nUniqueCount.ToString() : "") + ".png";
			uniquePath	= NgFile.CombinePath(basePath, matName);
			existsObj = AssetDatabase.LoadAssetAtPath(uniquePath, typeof(Texture));
			if (existsObj == null)
				break;
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
		return uniquePath;
	}

#endif
}

