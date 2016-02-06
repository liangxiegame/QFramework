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

public class FXMakerTextureConvertWindow : EditorWindow
{
	protected	string[]				textureSizeStr			= {"32", "64", "128", "256", "512", "1024", "2048", "4096"};
	protected	string[]				textureFormatStr		= {"AutomaticCompressed", "Automatic16bit", "AutomaticTruecolor"};
	protected	TextureImporterFormat[]	textureFormatVal		= {TextureImporterFormat.AutomaticCompressed, TextureImporterFormat.Automatic16bit, TextureImporterFormat.AutomaticTruecolor};

	protected	string					m_assetPath				= "IGSoft_Resources/EffectProject/[Resources]/[Texture]";
	protected	bool					m_bRecursively			= true;

	protected	bool					m_bSetGUI				= false;
	protected	bool					m_bSetWrapMode			= false;
	protected	bool					m_bSetFilterMode		= false;
	protected	bool					m_bSetAnisoLevel		= false;
	protected	bool					m_bSetMaxTextureSizeIdx	= false;
	protected	bool					m_bSetTextureFormatIdx	= false;

	protected	bool					m_bGUI					= false;
	protected	TextureWrapMode			m_wrapMode				= TextureWrapMode.Repeat;
	protected	FilterMode				m_filterMode			= FilterMode.Bilinear;
	protected	int						m_anisoLevel			= 4;
	protected	int						m_maxTextureSizeIdx		= 3;
	protected	int						m_textureFormatIdx		= 2;

	// ---------------------------------------------------------------------
	[MenuItem("Assets/FXMaker - ConvertTexture")]
	public static EditorWindow Init()
	{
		EditorWindow window = GetWindow(typeof(FXMakerTextureConvertWindow));

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

	void EnableSet(ref bool value, string name)
	{
		Rect rect	= EditorGUILayout.BeginHorizontal(GUILayout.Height(10));
		NgGUIDraw.DrawHorizontalLine(new Vector2(rect.x, rect.y+3), (int)rect.width, Color.gray, 1.0f, false);
		EditorGUILayout.EndHorizontal();
		GUI.enabled	= true;
		value	= EditorGUILayout.Toggle(name, value, GUILayout.MaxWidth(Screen.width));
		GUI.enabled	= value;
	}

	void OnGUI()
	{
		Object	tarPath		= EditorGUILayout.ObjectField	("AutoTarget", null, typeof(Object), false, GUILayout.MaxWidth(Screen.width));
		m_assetPath			= EditorGUILayout.TextField		("AssetPath", m_assetPath, GUILayout.MaxWidth(Screen.width));
		m_bRecursively		= EditorGUILayout.Toggle		("Recursively", m_bRecursively, GUILayout.MaxWidth(Screen.width));

		EnableSet(ref m_bSetGUI, "SetGUI");
		m_bGUI				= EditorGUILayout.Toggle("GUITexture", m_bGUI, GUILayout.MaxWidth(Screen.width));

		EnableSet(ref m_bSetFilterMode, "SetFilterMode");
		m_filterMode		= (FilterMode)EditorGUILayout.EnumPopup("filterMode", m_filterMode, GUILayout.MaxWidth(Screen.width));

		if (m_bSetGUI == false || m_bGUI == false)
		{
			EnableSet(ref m_bSetWrapMode, "SetWrapMode");
			m_wrapMode		= (TextureWrapMode)EditorGUILayout.EnumPopup("wrapMode", m_wrapMode, GUILayout.MaxWidth(Screen.width));
			EnableSet(ref m_bSetAnisoLevel, "SetAnisoLevel");
			m_anisoLevel	= EditorGUILayout.IntSlider("anisoLevel", m_anisoLevel, 0, 9, GUILayout.MaxWidth(Screen.width));
		}

		EnableSet(ref m_bSetMaxTextureSizeIdx, "SetMaxTextureSizeIdx");
		m_maxTextureSizeIdx	= EditorGUILayout.Popup("maxTextureSize", m_maxTextureSizeIdx, textureSizeStr, GUILayout.MaxWidth(Screen.width));

		EnableSet(ref m_bSetTextureFormatIdx, "SetTextureFormatIdx");
		m_textureFormatIdx	= EditorGUILayout.Popup("textureFormat", m_textureFormatIdx, textureFormatStr, GUILayout.MaxWidth(Screen.width));
		GUI.enabled	= true;

		if (tarPath != null)
		{
			string path = AssetDatabase.GetAssetPath(tarPath);
			path = path.Replace("Assets/", "");
			m_assetPath = path.Replace(Path.GetFileName(path), "");
		}

		EditorGUILayout.Space();
		FXMakerLayout.GUIEnableBackup((m_assetPath.Trim() != ""));
		if (GUILayout.Button("Start Reimport", GUILayout.Height(40)))
			ReimportTextures(m_assetPath, m_bRecursively, m_wrapMode, m_filterMode, m_anisoLevel, System.Convert.ToInt32(textureSizeStr[m_maxTextureSizeIdx]), textureFormatVal[m_textureFormatIdx]);
		FXMakerLayout.GUIEnableRestore();
	}

	// Property -------------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	void ReimportTextures(string assetPath, bool bRecursively, TextureWrapMode wrapMode, FilterMode filterMode, int anisoLevel, int maxTextureSize, TextureImporterFormat textureFormat)
	{
		int nOutFindFile;

		NgAsset.ObjectNode[]	objNodes = NgAsset.GetTexturePathList("Assets/" + assetPath, bRecursively, 0, out nOutFindFile);
		for (int n = 0; n < objNodes.Length; n++)
		{
			ReimportTexture(objNodes[n].m_AssetPath, wrapMode, filterMode, anisoLevel, maxTextureSize, textureFormat);
		}
	}

	void ReimportTexture(string assetPath, TextureWrapMode wrapMode, FilterMode filterMode, int anisoLevel, int maxTextureSize, TextureImporterFormat textureFormat)
	{
// 		Debug.Log("ChangeImportTextureToGUI - " + assetPath);
		TextureImporter	texImporter = TextureImporter.GetAtPath(assetPath) as TextureImporter;

		TextureImporterSettings settings = new TextureImporterSettings();
		texImporter.ReadTextureSettings(settings);
// 		settings.ApplyTextureType(TextureImporterType.GUI, false);
// 		texImporter.SetTextureSettings(settings);

		if (m_bSetGUI)
		{
			if (m_bGUI)
		 		 texImporter.textureType	= TextureImporterType.GUI;
			else texImporter.textureType	= TextureImporterType.Image;
		}

		if (m_bSetWrapMode)
			texImporter.wrapMode			= wrapMode;

		if (m_bSetGUI)
		{
			if (!m_bGUI)
			{
				if (m_bSetFilterMode)
					texImporter.filterMode	= filterMode;
				if (m_bSetAnisoLevel)
					texImporter.anisoLevel	= anisoLevel;
			}
		} else {
				if (m_bSetFilterMode)
					texImporter.filterMode	= filterMode;
				if (m_bSetAnisoLevel)
					texImporter.anisoLevel	= anisoLevel;
		}

		if (m_bSetMaxTextureSizeIdx)
			texImporter.maxTextureSize		= maxTextureSize;
		if (m_bSetTextureFormatIdx)
 			texImporter.textureFormat		= textureFormat;
//  	texImporter.npotScale = TextureImporterNPOTScale.None;
		AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
	}
}



