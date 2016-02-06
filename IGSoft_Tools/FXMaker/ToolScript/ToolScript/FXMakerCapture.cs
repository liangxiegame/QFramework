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

public class FXMakerCapture
{
	protected	static string		m_SaveBackThumbFilename	= "";

	protected	static Texture2D[]	m_SpriteTextures;
	protected	static Color		m_SpriteOldBackColor;
	protected	static Rect			m_SpriteRect;
	protected	static float		m_fOldTime;

	// ScreenShot ---------------------------------------------------------------------------------------------
	public static string GetCaptureScreenShotDir()
	{
		string dir = NgFile.TrimLastFolder(Application.dataPath);
		return NgFile.CombinePath(dir, FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.SCREENSHOTSAVEDIR));
	}

	public static void CaptureScreenShot()
	{
		string prefabName	= ((FXMakerMain.inst.GetOriginalEffectPrefab() != null) ? FXMakerMain.inst.GetOriginalEffectPrefab().name : "Empty") + "_";
		string filename		= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.SCREENSHOTSAVEDIR), prefabName + System.DateTime.Now.ToString("yyyyddMM_hhmmss") + ".png");

		if (Directory.Exists(GetCaptureScreenShotDir()) == false)
			Directory.CreateDirectory(GetCaptureScreenShotDir());
		Application.CaptureScreenshot(filename);
		Debug.Log("CaptureScreenshot filename - " + filename);
	}

	// export
	public static string GetExportSlitDir()
	{
		string dir = NgFile.TrimLastFolder(Application.dataPath);
		return NgFile.CombinePath(dir, FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.EXPORTSPLITDIR));
	}

	// Effect Capture ---------------------------------------------------------------------------------------------
	public static void StartSaveEffectThumb()
	{
//		gameObject.active = false;
		FXMakerMain.inst.GetComponent<FXMakerBackground>().enabled			= false;
		FXMakerMain.inst.GetComponent<FXMakerEffect>().enabled				= false;
// 		FXMakerMain.inst.GetComponentInChildren<FXMakerGizmo>().enabled	= false;

		NgAsset.CaptureRectPreprocess(FXMakerLayout.m_fScreenShotEffectZoomRate);
		FXMakerMain.inst.GetFXMakerControls().SetTimeScale(0);
		FXMakerMain.inst.StartThumbCapture();
	}

	public static void EndSaveEffectThumb()
	{
		string filename	= NgAsset.GetPrefabThumbFilename(FXMakerMain.inst.GetOriginalEffectPrefab());
		FXMakerMain.inst.GetFXMakerControls().ResumeTimeScale();
		NgAsset.CaptureRectSave(GetThumbCaptureRect(), FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), filename);
 		FXMakerMain.inst.SaveTool("");
// 		gameObject.active = true;
//		LoadTool("");
	}

	public static IEnumerator EndSaveEffectThumbCoroutine()
	{
		yield return new WaitForEndOfFrame();

		string filename	= NgAsset.GetPrefabThumbFilename(FXMakerMain.inst.GetOriginalEffectPrefab());
		FXMakerMain.inst.GetFXMakerControls().ResumeTimeScale();
		NgAsset.CaptureRectSave(GetThumbCaptureRect(), FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), filename);
 		FXMakerMain.inst.SaveTool("");
		FXMakerMain.inst.SetActiveTool(1);
	}

	public static Rect GetThumbCaptureRect()
	{
		// 하면 가운데
		int		width	= Mathf.Min(FXMakerLayout.m_nThumbCaptureSize, Screen.width);
		int		height	= Mathf.Min((int)(FXMakerLayout.m_nThumbCaptureSize*0.7f), Screen.height);
		int		x		= (Screen.width - width) / 2;
		int		y		= (Screen.height - height) / 2; // - (int)(Screen.height * 0.1f);
		return new Rect(x, y, width, height);
	}

	// Background Capture ---------------------------------------------------------------------------------------------
	public static void StartSaveBackThumb(string filename)
	{
		m_SaveBackThumbFilename	= filename;
//		gameObject.active = false;
		FXMakerMain.inst.GetComponent<FXMakerBackground>().enabled			= false;
		FXMakerMain.inst.GetComponent<FXMakerEffect>().enabled				= false;
		FXMakerMain.inst.GetComponentInChildren<FXMakerGizmo>().enabled		= false;

		NgAsset.CaptureRectPreprocess(FXMakerLayout.m_fScreenShotBackZoomRate);
		FXMakerMain.inst.GetFXMakerControls().SetTimeScale(0);
		FXMakerMain.inst.StartThumbCapture();
	}

	public static void EndSaveBackThumb()
	{
		string filename	= m_SaveBackThumbFilename;
		FXMakerMain.inst.GetFXMakerControls().ResumeTimeScale();
		NgAsset.CaptureRectSave(GetThumbCaptureRect(), FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), filename);
// 		gameObject.active = true;
// 		LoadTool("");
	}

	public static IEnumerator EndSaveBackThumbCoroutine()
	{
		yield return new WaitForEndOfFrame();

		string filename	= m_SaveBackThumbFilename;
		FXMakerMain.inst.GetFXMakerControls().ResumeTimeScale();
		NgAsset.CaptureRectSave(GetThumbCaptureRect(), FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), filename);
// 		gameObject.active = true;
// 		LoadTool("");
		FXMakerMain.inst.SetActiveTool(0);
	}

	// Sprite Capture ---------------------------------------------------------------------------------------------
	public static void StartSpriteCapture(FxmSpritePopup.CAPTURE_TYPE captureType, int nSpriteCaptureCount, float fCaptureInterval, Rect CaptureRect)
	{
		// hide
		FXMakerMain.inst.GetComponent<FXMakerEffect>().enabled			= false;
		FXMakerMain.inst.GetComponentInChildren<FXMakerGizmo>().SetSpriteCaptureState(true);
		m_fOldTime = FXMakerMain.inst.GetFXMakerControls().GetTimeScale();
		FXMakerMain.inst.GetFXMakerControls().SetTimeScale(1);
		Camera cam = Camera.main;
		m_SpriteOldBackColor = cam.backgroundColor;
		cam.backgroundColor	= Color.black;

		m_SpriteTextures	= new Texture2D[nSpriteCaptureCount];
		m_SpriteRect		= CaptureRect;
		FXMakerMain.inst.StartSpriteCapture(captureType, nSpriteCaptureCount, fCaptureInterval);
	}

	public static void CaptureSprite(int nSpriteCaptureCount, int nSpriteCaptureCurrent)
	{
		m_SpriteTextures[nSpriteCaptureCurrent]	= NgAsset.Capture(m_SpriteRect);
	}

	public static void EndSpriteCapture(int nSpriteCaptureCount)
	{
		FxmSpritePopup	spritePopup = FxmPopupManager.inst.GetSpritePopup();

		// Save Capture
 		FXMakerEffect.inst.SaveProject();
		string path = spritePopup.EndCapture(m_SpriteTextures);

		// Restore gui
		Camera.main.backgroundColor = m_SpriteOldBackColor;
		FXMakerMain.inst.GetComponent<FXMakerEffect>().enabled			= true;
		FXMakerMain.inst.GetComponentInChildren<FXMakerGizmo>().SetSpriteCaptureState(false);
		FXMakerMain.inst.GetFXMakerControls().SetTimeScale(m_fOldTime);

		// select
		if (spritePopup.m_bCreatePrefab)
	 		FXMakerEffect.inst.LoadProject(path);
 		else {
			FXMakerEffect.inst.LoadProject();
			if (path != "")
			{
				Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
				FXMakerAsset.SetPingObject(obj);
			}
		}
	}

}

#endif
