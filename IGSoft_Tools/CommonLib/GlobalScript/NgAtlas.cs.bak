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

public class NgAtlas
{
#if UNITY_EDITOR
	// ----------------------------------------------------------------------------------
	public static NcSpriteFactory.NcFrameInfo[] TileToTrimTexture(Material tarMat, int nTileX, int nTileY, int nStartFrame, int nFrameCount, int nMaxAtlasTextureSize)
	{
		if (tarMat == null)
			return null;

		// split texture
		Texture2D						tileTexture		= CloneAtlasTexture(tarMat.mainTexture);
		List<Texture2D>					frameTextures	= new List<Texture2D>();
		List<Rect>						frameRects		= new List<Rect>();
		bool							bFindAlpha;
		bool							bFindBlack;
		NcSpriteFactory.NcFrameInfo[]	frameInfos		= NgAtlas.TrimTexture(tileTexture, true, true, nTileX, nTileY, nStartFrame, nFrameCount, ref frameTextures, ref frameRects, out bFindAlpha, out bFindBlack);

		// free tempTexture
		Object.DestroyImmediate(tileTexture);

		// packTextures
		Color		clearColor		= new Color(0, 0, 0, (bFindAlpha ? 0 : 1));
		Texture2D	AtlasTexture	= new Texture2D(32, 32, TextureFormat.ARGB32, false);
		Rect[]		uvRects			= AtlasTexture.PackTextures(frameTextures.ToArray(), 2, nMaxAtlasTextureSize);

		// clear
		for (int x = 0; x < AtlasTexture.width; x++)
			for (int y = 0; y < AtlasTexture.height; y++)
				AtlasTexture.SetPixel(x, y, clearColor);

		// copy
		for (int n = 0; n < frameInfos.Length; n++)
		{
			int		nFrameIndex = frameInfos[n].m_nFrameIndex;
			Rect	imageUvRect	= frameInfos[n].m_TextureUvOffset	= uvRects[frameInfos[n].m_nFrameIndex];
			AtlasTexture.SetPixels((int)(imageUvRect.x*AtlasTexture.width), (int)(imageUvRect.y*AtlasTexture.height), (int)(imageUvRect.width*AtlasTexture.width), (int)(imageUvRect.height*AtlasTexture.height), frameTextures[nFrameIndex].GetPixels());
			Object.DestroyImmediate(frameTextures[n]);
		}

 		SaveAtlasTexture(AtlasTexture, tarMat);
		Object.DestroyImmediate(AtlasTexture);
		return frameInfos;
	}

	public static string ExportSplitTexture(string tarBasePath, Texture srcTex, NcSpriteFactory.NcFrameInfo[] ncSpriteFrameInfos)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		return "";
#else
		if (srcTex == null)
			return "";
		if (ncSpriteFrameInfos == null || ncSpriteFrameInfos.Length <= 0)
			return "";

		// split texture
		Texture2D		atlasTexture	= CloneAtlasTexture(srcTex);
		bool			bHadAlpha		= IsFindAlphaPixel(atlasTexture);

		// create subFolder
		string	filename	= srcTex.name;
		string	tarPath		= NgFile.CombinePath(tarBasePath, filename);
		if (Directory.Exists(tarPath) == false)
			Directory.CreateDirectory(tarPath);

		// export
		for (int n = 0; n < ncSpriteFrameInfos.Length; n++)
		{
			// packTextures
			Color		clearColor		= new Color(0, 0, 0, (bHadAlpha ? 0 : 1));
			Texture2D	splitTexture	= new Texture2D(ncSpriteFrameInfos[n].m_nTexWidth, ncSpriteFrameInfos[n].m_nTexHeight, TextureFormat.ARGB32, false);

			// clear
			for (int x = 0; x < splitTexture.width; x++)
				for (int y = 0; y < splitTexture.height; y++)
					splitTexture.SetPixel(x, y, clearColor);

			Color[]	colors = atlasTexture.GetPixels((int)(ncSpriteFrameInfos[n].m_TextureUvOffset.x*atlasTexture.width), (int)(ncSpriteFrameInfos[n].m_TextureUvOffset.y*atlasTexture.height), (int)(ncSpriteFrameInfos[n].m_TextureUvOffset.width*atlasTexture.width), (int)(ncSpriteFrameInfos[n].m_TextureUvOffset.height*atlasTexture.height), 0);
			splitTexture.SetPixels((int)((ncSpriteFrameInfos[n].m_FrameUvOffset.x+1)*splitTexture.width/2), (int)((ncSpriteFrameInfos[n].m_FrameUvOffset.y+1)*splitTexture.height/2), (int)(ncSpriteFrameInfos[n].m_FrameUvOffset.width*splitTexture.width/2), (int)(ncSpriteFrameInfos[n].m_FrameUvOffset.height*splitTexture.height/2), colors, 0);
			SaveTexture(splitTexture, tarPath, string.Format("{0}_{1:00}", filename, n));
			// free tempTexture
			Object.DestroyImmediate(splitTexture);
		}
		Object.DestroyImmediate(atlasTexture);
		return tarPath;
#endif
	}

	public static string ExportSplitTexture(string tarBasePath, Texture srcTex, int nTileX, int nTileY, int nStartFrame, int nFrameCount)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		return "";
#else
		if (srcTex == null)
			return "";

		// split texture
		Texture2D						tileTexture		= CloneAtlasTexture(srcTex);
		List<Texture2D>					frameTextures	= new List<Texture2D>();
		List<Rect>						frameRects		= new List<Rect>();
		bool							bFindAlpha;
		bool							bFindBlack;
		NcSpriteFactory.NcFrameInfo[]	frameInfos		= NgAtlas.TrimTexture(tileTexture, false, false, nTileX, nTileY, nStartFrame, nFrameCount, ref frameTextures, ref frameRects, out bFindAlpha, out bFindBlack);

		// free tempTexture
		Object.DestroyImmediate(tileTexture);

		// create subFolder
		string	filename	= srcTex.name;
		string	tarPath		= NgFile.CombinePath(tarBasePath, filename);
		if (Directory.Exists(tarPath) == false)
			Directory.CreateDirectory(tarPath);

		// export
		for (int n = 0; n < frameInfos.Length; n++)
		{
			SaveTexture(frameTextures[n], tarPath, string.Format("{0}_{1:00}", filename, n));
			Object.DestroyImmediate(frameTextures[n]);
		}
		return tarPath;
#endif
	}

	public static bool IsFindAlphaPixel(Texture2D srcTexture)
	{
		Color[] colors = srcTexture.GetPixels(0);
		for (int n = 0; n < colors.Length; n++)
			if (colors[n].a < 1)
				return true;
		return false;
	}

	public static void SaveTexture(Texture2D srcTex, string targetPath, string filename)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
#else
		byte[]	bytes	= srcTex.EncodeToPNG();
		File.WriteAllBytes(NgFile.CombinePath(targetPath, filename + ".png"), bytes);
#endif
	}

	public static int GetTextureSize(int nResultTotalFrame, int nResultCaptureSize)
	{
		if (nResultTotalFrame <= 1)		return nResultCaptureSize * 1;
		if (nResultTotalFrame <= 4)		return nResultCaptureSize * 2;
		if (nResultTotalFrame <= 16)	return nResultCaptureSize * 4;
		if (nResultTotalFrame <= 64)	return nResultCaptureSize * 8;
		if (nResultTotalFrame <= 256)	return nResultCaptureSize * 16;
		if (nResultTotalFrame <= 1024)	return nResultCaptureSize * 32;
		return nResultCaptureSize * 64;
	}

	public static void SetSourceTexture(Texture2D tex)
	{
		string texturePath;

		texturePath = AssetDatabase.GetAssetPath(tex);
		TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(texturePath);
		if (!importer.isReadable || importer.textureFormat != TextureImporterFormat.ARGB32 || importer.npotScale != TextureImporterNPOTScale.None)
		{
			importer.isReadable		= true;
			importer.maxTextureSize	= 4096;
			importer.textureFormat	= TextureImporterFormat.ARGB32;
			importer.npotScale		= TextureImporterNPOTScale.None;
			AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceSynchronousImport);
		}
	}

	public static NcSpriteFactory.NcFrameInfo[] TrimTexture(Texture2D srcTex, bool bTrimBlack, bool bTrimAlpha, int nTileX, int nTileY, int nStartFrame, int nFrameCount, ref List<Texture2D> frameTextures, ref List<Rect> frameRects, out bool bFindAlpha, out bool bFindBlack)
	{
		Rect		newRect;
		int			nFrameWidth	= srcTex.width	/ nTileX;
		int			nFrameHeight= srcTex.height	/ nTileY;
		Texture2D	frameTex	= new Texture2D(nFrameWidth, nFrameHeight, TextureFormat.ARGB32, false);

		NcSpriteFactory.NcFrameInfo[] ncFrameInfos = new NcSpriteFactory.NcFrameInfo[nFrameCount];
		bFindAlpha	= false;
		bFindBlack	= false;

		for (int n = nStartFrame; n < nStartFrame+nFrameCount; n++)
		{
			bool	bEmptyTexture = false;
			int		nIndex = n-nStartFrame;
			ncFrameInfos[nIndex] = new NcSpriteFactory.NcFrameInfo();
			frameTex.SetPixels(srcTex.GetPixels(nFrameWidth * (n % nTileX), srcTex.height - (nFrameHeight * (n / nTileX + 1)), nFrameWidth, nFrameHeight));

			if (bTrimAlpha || bTrimBlack)
			{
				bool bOutAlpha;
				bool bOutBlack;
				newRect = GetTrimRect(frameTex, bTrimAlpha, bTrimBlack, out bOutAlpha, out bOutBlack, out bEmptyTexture);
				if (bOutAlpha)
					bFindAlpha	= true;
				if (bOutBlack)
					bFindBlack	= true;
			}
			else newRect = new Rect(0, 0, nFrameWidth, nFrameHeight);

			Texture2D	newTex		= GetTrimTexture(frameTex, newRect);
			int			nTexIndex	= NgTexture.FindTextureIndex(frameTextures, newTex);

			if (0 <= nTexIndex)
			{
				Object.DestroyImmediate(newTex);
				newTex		= frameTextures[nTexIndex];
				newRect		= frameRects[nTexIndex];
			} else {
				nTexIndex = frameTextures.Count;
				frameTextures.Add(newTex);
				frameRects.Add(newRect);
			}

			GetTrimOffsets(frameTex, newRect, ref ncFrameInfos[nIndex].m_FrameUvOffset);
			ncFrameInfos[nIndex].m_FrameScale		= new Vector2(frameTex.width / 128.0f, frameTex.height / 128.0f);

			ncFrameInfos[nIndex].m_scaleFactor.x	= (nFrameWidth  / newRect.width ) * 0.5f;
			ncFrameInfos[nIndex].m_scaleFactor.y	= (nFrameHeight / newRect.height) * 0.5f;

			ncFrameInfos[nIndex].m_nFrameIndex		= nTexIndex;
			ncFrameInfos[nIndex].m_nTexWidth		= nFrameWidth;
			ncFrameInfos[nIndex].m_nTexHeight		= nFrameHeight;
			ncFrameInfos[nIndex].m_bEmptyFrame		= bEmptyTexture;
		}
		Object.DestroyImmediate(frameTex);
		return ncFrameInfos;
	}

	public static void ConvertAlphaTexture(Material tarMat, bool bEnableAlphaChannel, AnimationCurve curveAlphaWeight, float redWeight, float greenWeight, float blueWeight)
	{
 		Texture2D	tarTexture	= CloneAtlasTexture(tarMat.mainTexture);
		Color[]		srcColors	= tarTexture.GetPixels(0);
		srcColors = ConvertAlphaTexture(srcColors, bEnableAlphaChannel, curveAlphaWeight, redWeight, greenWeight, blueWeight);
		tarTexture.SetPixels(srcColors, 0);
		SaveAtlasTexture(tarTexture, tarMat);
		// free tempTexture
		Object.DestroyImmediate(tarTexture);
	}

	public static Color[] ConvertAlphaTexture(Color[] srcColors, bool bEnableAlphaChannel, AnimationCurve curveAlphaWeight, float redWeight, float greenWeight, float blueWeight)
	{
		for (int c = 0; c < srcColors.Length; c++)
		{
			if (bEnableAlphaChannel)
			{
				if (curveAlphaWeight != null)
					srcColors[c].a = curveAlphaWeight.Evaluate(srcColors[c].grayscale);
				else srcColors[c].a = srcColors[c].grayscale;
			} else srcColors[c].a = 1;
		}
		return srcColors;
	}

	// ----------------------------------------------------------------------------------
	public static Texture2D CloneAtlasTexture(Texture tex)
	{
		string						texturePath		= AssetDatabase.GetAssetPath(tex);
		TextureImporter				importer		= (TextureImporter)TextureImporter.GetAtPath(texturePath);
		bool						bReadable		= importer.isReadable;
		int							maxTexSize		= importer.maxTextureSize;
		TextureImporterFormat		textureFormat	= importer.textureFormat;
		TextureImporterNPOTScale	npotScale 		= importer.npotScale;

		Texture2D		srcTex		= (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
		SetSourceTexture(srcTex);
		Texture2D		tarTex		= new Texture2D(srcTex.width, srcTex.height, TextureFormat.ARGB32, false);
		Color[] colBuf	= srcTex.GetPixels(0);
		tarTex.SetPixels(colBuf, 0);
		tarTex.Apply(false);

		// Restore
		importer.isReadable		= bReadable;
		importer.maxTextureSize	= maxTexSize;
		importer.textureFormat	= textureFormat;
		importer.npotScale		= npotScale;
		AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceSynchronousImport);

		return tarTex;
	}

	private static void SaveAtlasTexture(Texture2D atlasTexture, Material tarMat)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
#else
		byte[]	bytes		= atlasTexture.EncodeToPNG();
		string	pathTexture	= (tarMat.mainTexture != null ? AssetDatabase.GetAssetPath(tarMat.mainTexture) : NgFile.TrimFileExt(AssetDatabase.GetAssetPath(tarMat)) + ".png");

		// save texture
		File.WriteAllBytes(pathTexture, bytes);
 		AssetDatabase.Refresh();

		// Material
		tarMat.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath(pathTexture, typeof(Texture));
		AssetDatabase.SaveAssets();
#endif
	}

	private static Rect GetTrimRect(Texture2D tex, bool bTrimAlpha, bool bTrimBlack, out bool bFindAlpha, out bool bFindBlack, out bool bEmptyTexture)
	{
		Color[]		pixels		= tex.GetPixels();
		Rect		newRect		= new Rect(0, 0, 0, 0);
		bool		bCheckAlpha	= false;
		bool		bCheckBlack	= false;

		// alpha -------------------------------------------
		// top
		for (int n = 0; n < pixels.Length; n++)
			if (IsVaildPixel(pixels[n], bTrimAlpha, bTrimBlack, ref bCheckAlpha, ref bCheckBlack))
			{
				newRect.y = n / tex.width;
				break;
			}

		// bottom
		for (int n = pixels.Length-1; 0 <= n; n--)
			if (IsVaildPixel(pixels[n], bTrimAlpha, bTrimBlack, ref bCheckAlpha, ref bCheckBlack))
			{
				newRect.yMax = n / tex.width + 1;
				break;
			}

		// left
		for (int x = 0; x < tex.width; x++)
		{
			bool bFind = false;
			for (int y = 0; y < tex.height; y++)
				if (IsVaildPixel(pixels[x+(y*(int)tex.width)], bTrimAlpha, bTrimBlack, ref bCheckAlpha, ref bCheckBlack))
				{
					bFind = true;
					newRect.x = x;
					break;
				}
			if (bFind) break;
		}

		// right
		for (int x = (int)tex.width-1; 0 <= x; x--)
		{
			bool bFind = false;
			for (int y = 0; y < tex.height; y++)
				if (IsVaildPixel(pixels[x+(y*(int)tex.width)], bTrimAlpha, bTrimBlack, ref bCheckAlpha, ref bCheckBlack))
				{
					bFind = true;
					newRect.xMax = x+1;
					break;
				}
			if (bFind) break;
		}

		if (newRect.width == 0 || newRect.height == 0)
		{
			newRect = new Rect(tex.width/2f-1f, tex.height/2f-1f, 2f, 2f);
			bEmptyTexture = true;
		} else bEmptyTexture = false;

		bFindAlpha	= bCheckAlpha;
		bFindBlack	= bCheckBlack;
		return newRect;
	}

	private static bool IsVaildPixel(Color color, bool bTrimAlpha, bool bTrimBlack, ref bool bFindAlpha, ref bool bFindBlack)
	{
		if (bTrimBlack)
		{
			if (color == Color.black)
			{
				bFindBlack = true;
				return false;
			}
			if (bTrimAlpha && color.a == 0)
			{
				bFindAlpha = true;
				return false;
			}
		} else {
			if (bTrimAlpha && color.a == 0)
			{
				bFindAlpha = true;
				return false;
			}
		}
		return true;
	}

	private static Texture2D GetTrimTexture(Texture2D tex, Rect newRect)
	{
// 		if (tex.width == newRect.width && tex.height == newRect.height)
// 			return tex;
		Texture2D newTex = new Texture2D((int)newRect.width, (int)newRect.height, TextureFormat.ARGB32, false);
		newTex.SetPixels(tex.GetPixels((int)newRect.xMin, (int)newRect.yMin, (int)newRect.width, (int)newRect.height, 0));
		return newTex;
	}

	private static void GetTrimOffsets(Texture2D tex, Rect newRect, ref Rect offset)
	{
		Vector2 textureCenter = new Vector2(tex.width / 2f, tex.height / 2f);
		offset.xMin	= (newRect.xMin - textureCenter.x) / textureCenter.x;
		offset.yMax = (newRect.yMax - textureCenter.y) / textureCenter.y;
		offset.xMax = (newRect.xMax - textureCenter.x) / textureCenter.x;
		offset.yMin = (newRect.yMin - textureCenter.y) / textureCenter.y;
	}

#endif
}

