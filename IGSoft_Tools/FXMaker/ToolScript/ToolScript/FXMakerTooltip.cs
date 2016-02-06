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
using System.Reflection; 
using System.Runtime.InteropServices;

public class FXMakerTooltip
{
	// Attribute ------------------------------------------------------------------------
	public		enum	TOOLTIPSPLIT	{HintRect, HoverCommand_Button, HoverCommand_Popup, CursorTooltip, Tooltip};
	protected	static	GUIContent		m_emptyContent	= new GUIContent(" ", " ");
	protected	static	bool			m_bTDisable		= false;

	protected	static	Dictionary<string, GUIContent>		m_DictionaryCon	= new Dictionary<string, GUIContent>();
	protected	static	Dictionary<string, string>			m_DictionaryStr	= new Dictionary<string, string>();

	// -----------------------------------------------------------------------------------------------------------------------------------------------------------
	public static GUIContent GetGUIContent(string caption, string tooltip)
	{
		return new GUIContent(caption, (tooltip == "" ? Tooltip(caption) : Tooltip(tooltip)));
	}

	public static GUIContent GetGUIContentImage(Texture image, string tooltip)
	{
		return new GUIContent(image, Tooltip(tooltip));
	}

	public static GUIContent GetGUIContentNoTooltip()
	{
		return new GUIContent(" ", " ");
	}

	public static GUIContent GetGUIContentNoTooltip(string caption)
	{
		return new GUIContent(caption, " ");
	}

	// -----------------------------------------------------------------------------------------------------------------------------------------------------------
	public static string Tooltip(string str)
	{
		return str + "|$" + str;
	}

	public static string Tooltip(string msgStr, string cursorStr)
	{
		if (cursorStr == null || cursorStr.Length <= 0)
			return msgStr;
		return msgStr + "|$" + cursorStr;
	}

	public static string GetTooltip(TOOLTIPSPLIT findSplit, string tooltips)
	{
		string[]	splitTooltips	= TooltipSplit(tooltips);

		foreach (string toolTip in splitTooltips)
		{
			if (toolTip.Trim().Length <= 0)
				continue;

			// HintRect
			if (findSplit == TOOLTIPSPLIT.HintRect && toolTip[0] == '!')
				return toolTip.Substring(1);

			// HoverCommand_Button
			if (findSplit == TOOLTIPSPLIT.HoverCommand_Button && toolTip[0] == '@')
				return toolTip.Substring(1);

			// HoverCommand_Popup Object
			if (findSplit == TOOLTIPSPLIT.HoverCommand_Popup && toolTip[0] == '#')
				return toolTip.Substring(1);

			// Cursor Shot Tooltip
			if (findSplit == TOOLTIPSPLIT.CursorTooltip && toolTip[0] == '$')
				return toolTip.Substring(1);

			// Bottom Long Tooltip
			if (findSplit == TOOLTIPSPLIT.Tooltip)
				return toolTip;
		}
		return "";
	}

	public static string AddCommand(string cmdString)
	{
		return "|"+"@"+cmdString;
	}

	public static string AddPopupPreview(Object hoverCom)
	{
		return "|"+"#"+hoverCom.GetInstanceID();
	}

	public delegate Rect delGetHintRect();

	public static string AddHintRect(delGetHintRect funcGetHintRect)
	{
		System.IntPtr intPtr =	Marshal.GetFunctionPointerForDelegate(funcGetHintRect);
		string encode = string.Format("{0}", intPtr.ToString());
// 		string encode = string.Format("{0},{1},{2},{3}", hintRect.x, hintRect.y, hintRect.width, hintRect.height);
		return "|"+"!"+encode;
	}

	public static Rect GetHintRect(string strGetHintRect)
	{
		System.IntPtr	intPtr		= new System.IntPtr(System.Convert.ToInt64(strGetHintRect));
		delGetHintRect	getHintRect = (delGetHintRect)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(delGetHintRect));

		if (getHintRect == null)
			return new Rect(0,0,0,0);
		return getHintRect();

// 		string[] param = hintRectStr.Split(',');
// 
// 		if (param.Length < 4)
// 		{
// 			Debug.LogError("GetHintRect(string hintRectStr) - format error");
// 			return new Rect(0, 0, 0, 0);
// 		}
// 		return new Rect(NgUtil.ToFloat(param[0], 0), NgUtil.ToFloat(param[1], 0), NgUtil.ToFloat(param[2], 0), NgUtil.ToFloat(param[3], 0));
	}

	// -------------------------------------------------------------------------------------------
	public static string[] TooltipSplit(string str)
	{
		return str.Split('|');
	}

	static void TooltipError(string caption)
	{
		if (caption == "_ehcck_")
			return;
		Debug.LogError("Toolip not found !!! - " + caption);
	}

	static int GetLang()
	{
		return EditorPrefs.GetInt("FXMakerOption.m_LanguageType", 0);
//		return (int)FXMakerOption.inst.m_LanguageType;
	}

	public static void CheckAllFunction()
	{
		GetHsToolMessage("_ehcck_", "");
		GetHsScriptMessage("_ehcck_");
		WindowDescription(new Rect(), 0, null);
		GetHcToolMain("_ehcck_", "");
		GetHsToolBackground("_ehcck_", "");
		GetHcToolBackground("_ehcck_", "");
		GetHsToolEffect("_ehcck_", "");
		GetHcToolEffect("_ehcck_");
		GetHcToolEffect_CameraX();
		GetHcToolEffect_CameraY();
		GetHcEffectControls("_ehcck_", "");
		GetHcEffectControls_Play(0, 0, 0, 0, 0, 0, 0, 0);
		GetHcEffectControls_Trans(0);
		GetHcEffectControls_Rotate();
		GetHcGizmo();
		GetHcEffectHierarchy("_ehcck_");
		GetHcEffectHierarchy_Box("_ehcck_", true, true);
		GetHcPopup_Sprite("_ehcck_");
		GetHcPopup_GameObject("_ehcck_", "");
		GetHcPopup_EffectScript("_ehcck_");
		GetHcPopup_Transform("_ehcck_");
		GetHcFolderPopup_Common("_ehcck_");
		GetHcFolderPopup_NcInfoCurve("_ehcck_");
		GetHsFolderPopup_Texture("_ehcck_");
		GetHcFolderPopup_Texture("_ehcck_");
		GetHsEditor_FXMakerOption("_ehcck_");
		GetHsEditor_NcAddForce("_ehcck_");
		GetHsEditor_NcDelayActive("_ehcck_");
		GetHsEditor_NcAutoDeactive("_ehcck_");
		GetHsEditor_NcAutoDestruct("_ehcck_");
		GetHsEditor_NcCurveAnimation("_ehcck_");
		GetHsEditor_NcUvAnimation("_ehcck_");
		GetHsEditor_NcAttachPrefab("_ehcck_");
		GetHsEditor_NcAttachSound("_ehcck_");
		GetHsEditor_NcBillboard("_ehcck_");
		GetHsEditor_NcDetachParent("_ehcck_");
		GetHsEditor_NcDuplicator("_ehcck_");
		GetHsEditor_NcParticleSpiral("_ehcck_");
		GetHsEditor_NcParticleEmit("_ehcck_");
		GetHsEditor_NcParticleSystem("_ehcck_");
		GetHsEditor_NcRotation("_ehcck_");
		GetHsEditor_NcTilingTexture("_ehcck_");
		GetHsEditor_NcSpriteAnimation("_ehcck_");
		GetHsEditor_NcSpriteTexture("_ehcck_");
		GetHsEditor_NcSpriteFactory("_ehcck_");
	}

	static void CheckValid(string[,] helpStr)
	{
		if (helpStr.GetLength(0) != 2/*(int)FXMakerOption.LANGUAGE.COUNT*/)
			Debug.LogWarning("Tooltip error : LANGUAGE Count");

		for (int n = 0; n < helpStr.GetLength(0); n++)
		{
			if (helpStr[n, helpStr.GetLength(1)-1] == null || helpStr[n, helpStr.GetLength(1)-1].Length == 0)
				Debug.LogWarning("Tooltip error : not defined Tooltup");
		}
	}

	// Tool Message ===================================================================================
	protected static string[,]	HsToolMessage = {
								{
									 "-- This is a read only folder. (For safe Upgrade, the folder shall not be used.)"
									,"-- You can modify or test the folder but cannot save. (ReadOnly Folder)"
									,"-- New Material is saved in Project."
									,"-- Same configured material already exists." + "\n" +
									 "-- Use existing material instead of greating a new one."
									,"-- Are you sure to delete with EffectPrefab ?"
									,"-- Are you sure you want to delete SpriteResources(Material, Texture) ? "
									,"-- UnityHierarchy/_FXMaker/ToolOption/FXMakerOption/bAutoSave, is off." + "\n" +
									 "-- Last work does not get auto-saved when reconfigured to Pause/Quit state."
									,"-- Previous work has been auto-saved."
									,"-- This is a ReadOnly Folder."
									,"-- Did not AutoSave.(This is a ReadOnly Folder)"
								}
								,
								{
									 "-- 읽기 전용 Folder 입니다.(안전한 Upgrade를 위해서 폴더 사용을 막습니다.)"
									,"-- 수정 및 테스트는 가능하나 저장은 하지 않습니다.(ReadOnly Folder)"
									,"-- 새로운 Meterial을 Project에 저장하였습니다."
									,"-- 같은 설정의 Material이 이미 존재합니다." + "\n" +
									 "-- 새로은 Material을 만들지 않고 기존 것을 사용합니다."
									,"-- Are you sure you want to delete EffectPrefab ?"
									,"-- Are you sure you want to delete SpriteResources(Material, Texture) ? "
									,"-- UnityHierarchy/_FXMaker/ToolOption/FXMakerOption/bAutoSave, off 상태입니다." + "\n" +
									 "-- Pause/Quit 상태로 변경될 때 마지막 작업이 자동 저장되지 않습니다."
									,"-- 이전 작업은 자동 저장 되었습니다."
									,"-- 읽기전용 폴더이기에 저장할 수 없습니다."
									,"-- 자동저장 하지 않았습니다.(현재 폴더는 읽기 전용 폴더입니다.)"
								}
							  };

	public static string GetHsToolMessage(string keyword, string arg)
	{
		if (m_bTDisable)	return " ";

		string keystr = "GetHsToolMessage" + keyword + arg;
		if (m_DictionaryStr.ContainsKey(keystr))
			return m_DictionaryStr[keystr];

		string		tooltip = "";
		CheckValid(HsToolMessage);

		switch (keyword)
		{
			case "": break;
			case "FOLDER_READONLY"			: tooltip = HsToolMessage[GetLang(), 0];		break;
			case "FOLDER_READONLY_BOTTOM"	: tooltip = HsToolMessage[GetLang(), 1];		break;
			case "MATERIAL_NEWSAVED"		: tooltip = HsToolMessage[GetLang(), 2];		break;
			case "MATERIAL_EXISTSAVED"		: tooltip = HsToolMessage[GetLang(), 3];		break;
			case "DIALOG_DELETEPREFAB"		: tooltip = HsToolMessage[GetLang(), 4];		break;
			case "DIALOG_DELETESPRITE"		: tooltip = HsToolMessage[GetLang(), 5];		break;
			case "WARRING_AUTOSAVE"			: tooltip = HsToolMessage[GetLang(), 6];		break;
			case "TOOL_AUTOSAVE"			: tooltip = HsToolMessage[GetLang(), 7];		break;
			case "READONLY_FOLDER"			: tooltip = HsToolMessage[GetLang(), 8];		break;
			case "READONLY_NOTSAVE"			: tooltip = HsToolMessage[GetLang(), 9];		break;
			default: tooltip = keyword; break;
		}
		m_DictionaryStr.Add(keystr, tooltip);
		return m_DictionaryStr[keystr];
	}

	// Inspector Message ===================================================================================
	protected static string[,]	HcToolInspector = {
								{
									 "-- Tag is used by the user."
								}
								,
								{
									 "-- 유저용 tag 입니다."
								}
							  };

	public static string GetHsToolInspector(string keyword)
	{
		if (m_bTDisable)	return " ";

		string		tooltip = "";
		CheckValid(HcToolInspector);

		switch (keyword)
		{
			case "": break;
			case "m_fUserTag"			: tooltip = HcToolInspector[GetLang(), 0];		break;
			default: tooltip = keyword; break;
		}
		return keyword + "\n" + tooltip;
	}

	// Script Error Message ===================================================================================
	protected static string[,]	HsScriptMessage = {
								{
									 "-The last operation is not automatically save when the mode changes into Pause/Quit mode." + "\n" +
									 "- NcCurveAnimation needs more than one CurveInfo." + "\n" +
									 "- Delete unused Component."
									,"- m_AttachPrefab is empty."
									,"- AudioClip is empty."
									,"- This Component cannot be added to Root GameObject." + "\n" +
									 "- Add to Sub GameObject."
									,"- NcParticleSystem only operates when LegacyParticle or ParticleSystem exists." + "\n" +
									 "- Use creating button in 'Unity Inspector'."

									,"- 'Unity Particle' is included ans value of Lossyscale is not 1." + "\n" +
									 "- If bScaleWithTransform is wanted, add NcParticleSystem Script."
									,"- Warning of adjusting vleocity" + "\n" +
									 "- Animation : Speed of Unity animation is not adjustable.(Support is scheduled)" + "\n" +
									 "- Particle emitter: Adjustment of Damping and Velocity of LegacyParticle is required."
									,"- GameObject which is used only by ShurikenParticleSystem is not Speed adjustable." + "\n" +
									 "- Add NcParticleSystem Script which is Speed adjustable to relevant Gameobject."
									,"- 'NcUvAnimation has to be added in order to use TEXTUREUV." + "\n" +
									 " 'NcUvAnimation' will be added by clicking 'Inspector/Add Component' button"

									,"- Do not put NcDetachParent to GameObject together with Component which does Transform process, such as NcCurveAnimation, NcRotation, Animation, Rigidbody and etc."

									,"- Component is overlapped at Gameobject." + "\n" +
									 "- Delete all excepct one."
									,"- Attempting of modification of color value, but no ColorProperty is modifiable at Material." + "\n" +
									 "- Change to Shader which contains Colorproperty."
									,"- Renderer Material does not exist." + "\n" +
									 "- Add Material to GameObject."
									,"- Components that modifies UV(Tiling, Offset) of Texture exist more than one." + "\n" +
									 "- Only a single component can exist among the ones displayed below." + "\n" +
									 "- (NcSpriteAnimation, NcUvAnimation, NcTilingTexture, NcSpriteTexture)"
									,"- Does not work properly when used with 'NcBillboard'." + "\n" +
									 "- Use 'Ratate' from 'NcBillboard' after deleting 'NcRotation'."
									,"- m_NcSpriteFactoryPrefab is not valid." + "\n" +
									 "- 'NcSpritwFactory.cs' has to be contained by Prefab when m_NcSpriteFactoryPrefab is designated." + "\n" +
									 "- 'NcSpritwFactory.cs' has to be contained by GameObject when m_NcSpriteFactoryPrefab is not designated."
									,"- RigidBody Component is required."
									,"- MeshRenderer does not exist." + "\n" +
									 "- Add MeshRenderer to GameObject."
									,"- m_LegacyParticlePrefab is not valid." + "\n" +
									 "- 'ParticleEmitter' has to be contained by Prefab when m_LegacyParticlePrefab is designated." + "\n" +
									 "- 'ParticleEmitter' has to be contained by GameObject when m_LegacyParticlePrefab is not designated."
									,"- m_ParticlePrefab is not valid." + "\n" +
									 "- 'ParticleEmitter or ParticleSystem' has to be contained by Prefab when m_ParticlePrefab is designated."
								}
								,
								{
									 "- NcCurveAnimation는 하나 이상의 CurveInfo가 있어야 합니다." + "\n" +
									 "- 사용하지 않는 Component는 삭제 하세요."
									,"- m_AttachPrefab이 비어 있습니다."
									,"- AudioClip이 비어 있습니다."
									,"- 이 Component는 Root GameObject에 추가할 수 없습니다." + "\n" +
									 "- Sub GameObject에 추가 하세요."
									,"- NcParticleSystem은 LegacyParticle 또는 ParticleSystem 있을 때만 작동 합니다." + "\n" +
									 "- 'Unity Inspector'에 있는 생성 버튼을 이용하세요."

									,"- 'Unity Particle'이 포함되어 있으면서 lossyScale 값은 1이 아닙니다." + "\n" +
									 "- bScaleWithTransform을 원하시면, NcParticleSystem Script를 추가하세요."
									,"- 속도조절 주의사항" + "\n" + 
									 "- Aanimation : Unity animation은 속도조절이 안됩니다.(지원예정)" + "\n" + 
									 "- particleEmitter : LegacyParticle은 Damping 및 Velocity조절이 필요 합니다."
									,"- ShurikenParticleSystem만 사용 하고 있는 GameObject는 속도조절이 안됩니다." + "\n" +
									 "- 해당 GameObject에 속도조절을 할 수 있는NcParticlesystem Script를 추가 하세요."
									,"- TEXTUREUV를 사용 하기 위해선 NcUvAnimation Component를 추가 해야 합니다." + "\n" +
									 "'Inspector/Add Component Button'을 클릭하시면 추가 됩니다."

									,"- NcDetachParent는 NcCurveAnimation, NcRotation, Animation, Rigidbody등 Transform 처리를 하는 Component와 같은 GameObject에 두지 마십시오."

									,"- GameObject에 중복 추가되어 있는 Component 입니다." + "\n" +
									 "- 한개를 뺀 나버지는 모두 삭제하세요."
									,"- Color 값을 변경 할려고 하였으나, Material에는 변경 가능한 ColorProperty가 없습니다." + "\n" +
									 "- ColorProperty가 있는 Shader로 변경 하세요."
									,"- 출력할 수 있는 Material이 없습니다." + "\n" +
									 "- GameObject에 Material을 추가하세요."
									,"- Texture의 Uv(Tiling, Offset)값을 수정하는 Component가 여러개 있습니다." + "\n" +
									 "- 아래줄에 표시된 Component중 하나만 존재 해야 합니다." + "\n" +
									 "- (NcSpriteAnimation, NcUvAnimation, NcTilingTexture, NcSpriteTexture)"
									,"- 'NcBillboard'와 같이 사용할 경우 제대로 작동하지 않습니다." + "\n" +
									 "- 'NcRotation'를 삭제하시고 'NcBillboard'에 있는 'Ratate'를 사용하세요."
									,"- m_NcSpriteFactoryPrefab가 유효하지 않습니다." + "\n" +
									 "- m_NcSpriteFactoryPrefab을 지정할 경우 해당 Prefab에 'NcSpriteFactory.cs'가 포함되어 있어야 합니다." + "\n" +
									 "- m_NcSpriteFactoryPrefab을 지정안할 경우 현재 GameObject에 'NcSpriteFactory.cs'가 포함되어 있어야 합니다."
									,"- RigidBody Component가 필요 합니다."
									,"- MeshRenderer가 없습니다." + "\n" +
									 "- GameObject에 MeshRenderer을 추가하세요."
									,"- m_LegacyParticlePrefab이 유효하지 않습니다." + "\n" +
									 "- m_LegacyParticlePrefab을 지정할 경우 해당 Prefab에 'ParticleEmitter'가 포함되어 있어야 합니다." + "\n" +
									 "- m_LegacyParticlePrefab을 지정안할 경우 현재 GameObject에 'ParticleEmitter'가 포함되어 있어야 합니다."
									,"- m_ParticlePrefab이 유효하지 않습니다." + "\n" +
									 "- m_ParticlePrefab을 지정할 경우 해당 Prefab에 'ParticleEmitter' 또는 'ParticleSystem'이 포함되어 있어야 합니다."
								}
							  };
	public static string GetHsScriptMessage(string keyword)
	{
		if (m_bTDisable)	return " ";

		string keystr = "GetHsScriptMessage" + keyword;
		if (m_DictionaryStr.ContainsKey(keystr))
			return m_DictionaryStr[keystr];

		string	tooltip = "";
		CheckValid(HsScriptMessage);

		switch (keyword)
		{
			case "": break;
			case "SCRIPT_EMPTY_CURVE"					: tooltip = HsScriptMessage[GetLang(), 0];		break;
			case "SCRIPT_EMPTY_ATTACHPREFAB"			: tooltip = HsScriptMessage[GetLang(), 1];		break;
			case "SCRIPT_EMPTY_AUDIOCLIP"				: tooltip = HsScriptMessage[GetLang(), 2];		break;
			case "SCRIPT_ERROR_ROOT"					: tooltip = HsScriptMessage[GetLang(), 3];		break;
			case "SCRIPT_EMPTY_PARTICLE"				: tooltip = HsScriptMessage[GetLang(), 4];		break;

			case "SCRIPT_NEED_SCALEPARTICLE"			: tooltip = HsScriptMessage[GetLang(), 5];		break;
			case "SCRIPT_WARRING_SCALESPEED"			: tooltip = HsScriptMessage[GetLang(), 6];		break;
			case "SCRIPT_WARRING_SCALESPEED_SHURIKEN"	: tooltip = HsScriptMessage[GetLang(), 7];		break;
			case "SCRIPT_NEED_NCUVANIMATION"			: tooltip = HsScriptMessage[GetLang(), 8];		break;
			case "SCRIPT_WARRING_NCDETACHPARENT"		: tooltip = HsScriptMessage[GetLang(), 9];		break;

			case "SCRIPT_WARRING_DUPLICATE"				: tooltip = HsScriptMessage[GetLang(),10];		break;
			case "SCRIPT_EMPTY_COLOR"					: tooltip = HsScriptMessage[GetLang(),11];		break;

			case "SCRIPT_EMPTY_MATERIAL"				: tooltip = HsScriptMessage[GetLang(),12];		break;
			case "SCRIPT_DUPERR_EDITINGUV"				: tooltip = HsScriptMessage[GetLang(),13];		break;
			case "SCRIPT_CLASH_ROTATEBILL"				: tooltip = HsScriptMessage[GetLang(),14];		break;
			case "SCRIPT_EMPTY_SPRITEFACTORY"			: tooltip = HsScriptMessage[GetLang(),15];		break;

			case "SCRIPT_NEED_RIGIDBODY"				: tooltip = HsScriptMessage[GetLang(),16];		break;
			case "SCRIPT_EMPTY_MESHRENDERER"			: tooltip = HsScriptMessage[GetLang(),17];		break;
			case "SCRIPT_EMPTY_LEGACYPARTICLEPREFAB"	: tooltip = HsScriptMessage[GetLang(),18];		break;
			case "SCRIPT_EMPTY_PARTICLEPREFAB"			: tooltip = HsScriptMessage[GetLang(),19];		break;

			default: tooltip = keyword; break;
		}
		m_DictionaryStr.Add(keystr, tooltip);
		return m_DictionaryStr[keystr];
	}

	// window desc ===================================================================================
	protected static string[,]	WindowDescriptionStr = {
								{
									 "- The name of the folder loaded from 'Unity Project'" + "\n" +
									 "- consists of the top 'Projet Folder' and the bottom 'Group Folder'."
									,"- All 'Effect Prefab' of the chosen folder (including subfolders) will be shown." + "\n" +
									 "- If you click 'Effect Prefab,' editing process will start." + "\n" +
									 "- Major Functions: Effect Prefab(New, Delete, Clone)"
									,"- Print out 'GameObject Hierarchy' of the chosen 'Effect Prefab'." + "\n" +
									 "- If you click Delete Key, the chosen GameObject/Component will be immediately deleted." + "\n" +
									 "- Drag & Drop : GameObject - move to the location of Drop." + "\n" +
									 "- Drag & Drop : Transform - only copy operations of Transform.(Defaelt Option - World)" + "\n" +
									 "- Drag & Drop : Component - if there is the same Component, Cut/OverWrite; if not, create and then Cut/Paste." + "\n" +
									 "- Drag & Drop : Material, AniClip - Copy/Paste to the location of Drop.(In the same GameObject, change the order.)"
									,"- Do several simulations for Effect test. Those simulations will not be saved in 'Effect Prefab'." + "\n" +
									 "- Major Functions: Transform Test(Move, Scale, Rotate), Speed Adjustment, Multi-shot test of bullets"
								}
								,
								{
									 "- 'Unity Project'에서 로드된 폴더명" + "\n" +
									 "- 상단 'Project Folder'와 하단 'Group Folder'로 구성 됩니다."
									,"- 선택한 폴더의(하위 폴더내용 포함) 모든 'Effect Prefab'을 보여 줍니다." + "\n" +
									 "- 'Effect Prefab'을 선택하시면 편집작업이 시작 됩니다." + "\n" +
									 "- 주요기능: Effect Prefab(New, Delete, Clone)"
									,"- 선택된 'Effect Prefab'의 'GameObject Hierarchy'를 출력 합니다." + "\n" +
									 "- Delete Key를 누르시면 선택된 GameObject/Component를 바로 삭제 합니다." + "\n" +
									 "- Drag & Drop : GameObject - Drop위치에 이동 처리 합니다." + "\n" +
									 "- Drag & Drop : Transform - Transform의 내용만 복사합니다.(Defaelt Option - World)" + "\n" +
									 "- Drag & Drop : Component - 같은 Component가 있으면 Cut/Overwrite, 없다면 생성 후 Cut/Paste." + "\n" +
									 "- Drag & Drop : Material, AniClip - Drop위치에 Copy/Paste 됩니다.(같은 GameObject에서는 순서를 바꿉니다.)"
									,"- 이펙트 테스트를 위한 여러가지 시뮬레이션을 합니다, 'Effect Prefab'에 저장되지 않습니다." + "\n" +
									 "- 주요기능: Transform Test(Move, Scale, Rotate), 속도조절, 총탄 멀티샷 테스트"
								}
							  };
	public static void WindowDescription(Rect popupRect, FXMakerLayout.WINDOWID winId, FxmPopup popup)
	{
		CheckValid(WindowDescriptionStr);

		Vector2	mousePos	= FXMakerLayout.GetGUIMousePosition();
		Rect	rectDesc	= FXMakerLayout.GetInnerTopRect(popupRect, 0, 19);
		string	tooltip		= "";

		if (rectDesc.Contains(mousePos) && GUI.enabled && (popup == null || popup.GetPopupRect().Contains(mousePos) == false))
		{
			switch (winId)
			{
				case FXMakerLayout.WINDOWID.TOP_CENTER			: tooltip = "MainToolbar\n" + WindowDescriptionStr[GetLang(), 0];			break;
				case FXMakerLayout.WINDOWID.EFFECT_LIST			: tooltip = "PREFAB_LIST\n" + WindowDescriptionStr[GetLang(), 1];			break;
				case FXMakerLayout.WINDOWID.EFFECT_HIERARCHY	: tooltip = "PREFAB_HIERARCHY\n" + WindowDescriptionStr[GetLang(), 2];		break;
				case FXMakerLayout.WINDOWID.EFFECT_CONTROLS		: tooltip = "PREFAB_CONTROLS\n" + WindowDescriptionStr[GetLang(), 3];		break;
				default: return;
			}
			FXMakerMain.inst.SaveTooltip(Tooltip(tooltip));
		}
	}

	// ToolMain ===================================================================================
	public static GUIContent GetHcToolMain(string caption)
	{
		if (m_bTDisable)	return m_emptyContent;

		return GetHcToolMain(caption, "");
	}

	protected static string[,]	HcToolMain = {
								{
									 "- Move to the background settings screen.(Backgroud, Camera, Light)" + "\n" +
									 "- You can insert practical background images, which will be used in the game." + "\n" +
									 "- After modified Effect is automatically saved, the operating screen will be switched."
									,"- Move to the Effect processing screen." + "\n" +
									 "- After modified background information is automatically saved, the operating screen will be switched."
									,"- Reload all assets from 'Unity Project'." + "\n" +
									 "- 'LoadPrj' is rarely used directly as it is automatically reloaded when the user changes 'Unity Project'."
									,"- Save current operations in 'Unity Project'." + "\n" +
									 "- 'SavePrj' is rearely used directly as it is automatically saved when the user changes operating Effect."
									,"- Capture and save the current screen." + "\n" +
									 "- CTRL + SHIFT + S Key" + "\n" +
									 "- if you do right-click, the window of the saved folder will open."
								}
								,
								{
									 "- 배경설정 화면으로 이동 합니다.(Backgroud, Camera, Light)" + "\n" +
									 "- 게임에 사용되는 실 배경을 삽입할 수 있습니다." + "\n" +
									 "- 수정된 이펙트는 자동 저장된 후 작업화면이 전환 됩니다."
									,"- 이펙트 작업 화면으로 이동합니다." + "\n" +
									 "- 수정된 배경정보는 자동 저장된 후 작업화면이 전환 됩니다."
									,"- 'Unity Project'로 부터 모든 assets을 리로드 합니다." + "\n" +
									 "- 사용자가 'Unity Project'를 변경할 경우 자동으로 리로드 되므로, 'LoadPrj'를 직접 사용할 일은 거의 없습니다."
									,"- 현재의 작업을 'Unity Project'에 저장합니다." + "\n" +
									 "- 작업 Effect를 변경할 경우 자동 저장 되므로, 'SavePrj'를 직접 사용할 일은 거의 없습니다."
									,"- 현재의 화면을 Capture해서 저장합니다." + "\n" +
									 "- CTRL + SHIFT + S Key" + "\n" +
									 "- 오른쪽 클릭을 하시면 저장된 폴더창이 열립니다."
								}
							  };

	public static GUIContent GetHcToolMain(string caption, string arg)
	{
		if (m_bTDisable)	return m_emptyContent;

		string keystr = "GetHcToolMain" + caption + arg;
		if (m_DictionaryCon.ContainsKey(keystr))
			return m_DictionaryCon[keystr];

		CheckValid(HcToolMain);

		string	tooltip = "";

		switch (caption)
		{
			case "": break;
			case "go Background"	: tooltip = HcToolMain[GetLang(), 0];		break;
			case "go PrefabTool"	: tooltip = HcToolMain[GetLang(), 1];		break;
			case "LoadPrj"			: tooltip = HcToolMain[GetLang(), 2];		break;
			case "SavePrj"			: tooltip = HcToolMain[GetLang(), 3];		break;
			case "FullCapture"		: tooltip = HcToolMain[GetLang(), 4] + "\n" + arg;		break;
			default: TooltipError(caption); break;
		}
		if (tooltip != null) tooltip = caption + "\n" + tooltip;

		m_DictionaryCon.Add(keystr, GetGUIContent(caption, tooltip));
		return m_DictionaryCon[keystr];
	}

	// ToolBackground ===================================================================================
	public static string GetHsToolBackground(string keyword, string arg)
	{
		if (m_bTDisable)	return " ";

		string[,]	helpStr = {
								{
									 "- This is an empty folder" + "\n" +
									 "- 'Unity Project/" + arg + "' - You can add a new folder under the existing folder."
									," - If you click, you can load the background settings in the folder."
									," - The background settings is empty." + "\n" +
									 " - You can make new settings with 'New' button in the right."
									," - If you click, the new settings will be used as current background information." + "\n" +
									 "- In 'Unity Hierarchy,' you can change and save the highlighted object."
									,"- This is an object for the background." + "\n" +
									 "- If you click, the location of the object will be found in 'Unity Hierarchy" + "\n" +
									 "- You can modify that object or add sub GameObject."
									,"- This is an ReferencePrefab for the background." + "\n" +
									 "- If you click, the location of the object will be found in 'Unity Project"
									,"- If you click, it will be used as the background object."
								}
								,
								{
									 "- 빈 폴더 입니다." + "\n" +
									 "- 'Unity Project/" + arg + "'- 폴더 아래에 새로운 폴더를 직접 추가 할 수 있습니다."
									,"- 클릭하시면, 폴더에 있는 배경설정을 로드 합니다."
									,"- 배경 설정이 비어 있습니다." + "\n" +
									 "- 오른쪽의 'New'버튼으로 새로운 설정을 만드실 수 있습니다."
									,"- 클릭하시면, 현재의 배경용 정보로 사용됩니다." + "\n" +
									 "- 'Unity Hierarchy'에서 하이라이트 된 오브젝트를 변경하여 저장 할 수 있습니다."
									,"- 배경용으로 선택된 오브젝트 입니다." + "\n" +
									 "- 클릭하시면, 'Unity Hierarchy'에서 오브젝트의 위치를 찾아 줍니다." + "\n" +
									 "- 해당 오브젝트를 수정하거나 sub GameObject를 추가 하실 수 있습니다."
									,"- 배경용으로 선택된 ReferencePrefab 입니다." + "\n" +
									 "- 클릭하시면, Prefab 위치를 찾아 줍니다."
									,"- 클릭하시면, 배경용 오브젝트로 사용됩니다."
								}
							  };
		CheckValid(helpStr);
		string	tooltip = "";

		switch (keyword)
		{
			case "": break;
			case "EMPTYGROUP_HOVER"		: tooltip = helpStr[GetLang(), 0];					break;
			case "GROUP_HOVER"			: tooltip = arg + "\n" + helpStr[GetLang(), 1];		break;
			case "EMPTYBACKGROUND_HOVER": tooltip = helpStr[GetLang(), 2];					break;
			case "BACKGROUND_HOVER"		: tooltip = arg + "\n" + helpStr[GetLang(), 3];		break;
// 			case "EMPTYBACKGROUND_HOVER": tooltip = helpStr[GetLang(), 2]					+ AddHintRect(FXMakerBackground.GetRect_Resources); break;
// 			case "BACKGROUND_HOVER"		: tooltip = arg + "\n" + helpStr[GetLang(), 3]		+ AddHintRect(FXMakerBackground.GetRect_Resources); break;
			case "RES_CLONE_HOVER"		: tooltip = arg + "\n" + helpStr[GetLang(), 4];		break;
			case "RES_REFERENCE_HOVER"	: tooltip = arg + "\n" + helpStr[GetLang(), 5];		break;
			case "RESOURCE_HOVER"		: tooltip = arg + "\n" + helpStr[GetLang(), 6];		break;
			default: tooltip = keyword; break;
		}
		return Tooltip(tooltip);
	}

	public static GUIContent GetHcToolBackground(string caption, string arg)
	{
		if (m_bTDisable)	return m_emptyContent;

		string[,]	helpStr = {
								{
									 "- Create new background information."
									,"- Delete [" + arg + "] the selected items."
									,"- Create [" + arg + "] the clones of the selected items"
									,"- Capture the current screen and use it [" + arg + "] as a Preview image of the selected items."
									,"- Remove [" + arg + "] the selected items."
									,"- Set-up is not completed." + "\n" +
									 "- You can choose among lists under the button." + "\n" +
									 "- [" + arg + "] If you put user prefab in directory, it will be shown in the list."
									,"- Select Prefab to be used as background." + "\n" +
									 "- Original Prefab will not be affected even it is modified at inspector after selecting."
									,"- Open the original Prefab in order to modify refered one."
								}
								,
								{
									 "- 새로운 배경정보를 만듭니다."
									,"- 선택된 [" + arg + "] 항목을 삭제합니다."
									,"- 선택된 [" + arg + "] 항목의 클론을 만듭니다"
									,"- 현재 화면을 캡쳐하여, 선택된 [" + arg + "] 항목의 Preview 이미지로 사용합니다."
									,"- 선택된 [" + arg + "] 오브젝트를 제거 합니다."
									,"- 설정 하지 않았습니다." + "\n" +
									 "- 버튼 아래의 리스트 중 선택 하실 수 있습니다." + "\n" +
									 "- [" + arg + "] 디렉토리에 사용자 prefab을 넣어두시면 리스트에 표시됩니다."
									,"- 배경으로 사용될 Prefab을 선택 합니다." + "\n" +
									 "- 선택 후 Inspector에서 수정하더라도 원본 prefab에는 영향을 주지 않습니다."
									,"- 참조된 Prefab을 수정할 수 있도록 원본 Prefab을 오픈합니다."
								}
							  };
		CheckValid(helpStr);
		string	tooltip = "";

		switch (caption)
		{
			case "": break;
			case "New":					tooltip	= helpStr[GetLang(), 0]	+ "\n" + arg	+ AddHintRect(FXMakerLayout.GetMenuToolbarRect);		break;
			case "Del":					tooltip	= arg + "\n" + helpStr[GetLang(), 1]	+ AddHintRect(FXMakerLayout.GetMenuToolbarRect);		break;
			case "Clone":				tooltip	= arg + "\n" + helpStr[GetLang(), 2]	+ AddHintRect(FXMakerLayout.GetMenuToolbarRect);		break;
			case "Thumb": 				tooltip	= arg + "\n" + helpStr[GetLang(), 3]	+ AddHintRect(FXMakerLayout.GetMenuToolbarRect);		break;
			case "Clear Selected": 		tooltip	= arg + "\n" + helpStr[GetLang(), 4];	break;
			case "Create Thumb": 		tooltip	= arg + "\n" + helpStr[GetLang(), 3];	break;
			case "[Not Selected]": 		tooltip	= helpStr[GetLang(), 5]; break;
			case "Select":				tooltip	= arg + "\n" + helpStr[GetLang(), 6];	break;
			case "Open":				tooltip	= arg + "\n" + helpStr[GetLang(), 7];	break;
			default: TooltipError(caption); break;
		}
		return GetGUIContent(caption, tooltip);
	}

	// ToolEffect ===================================================================================
	public static string GetHsToolEffect(string keyword, string arg)
	{
		if (m_bTDisable)	return " ";

		string keystr = "GetHsToolEffect" + keyword + arg;
		if (m_DictionaryStr.ContainsKey(keystr))
			return m_DictionaryStr[keystr];

		string[,]	helpStr = {
								{
									 "- This is an empty Project folder." + "\n" +
									 "- 'Unity Project/" + arg + "' You can add a new folder under the existing folder."
									,"- If you click, the list of Group folders in the folder is loaded."
									,"- This is an empty Group folder." + "\n" +
									 "- 'Unity Project/" + arg + "' You can add a new folder under the existing folder."
									,"- If you click, the list of Effect in the folder (including subfolders) is loaded."
									,"- L-Click: Choose Effect which you will operate." + "\n" +
									 "- (Previous operations are automatically saved)" + "\n" +
									 "- R-Click: PopupMenu." + "\n" +
									 "- DeleteKey - permanently deleted."
								}
								,
								{
									 "- 빈 Project 폴더 입니다." + "\n" +
									 "- 'Unity Project/" + arg + "' 폴더 아래에 새로운 폴더를 직접 추가 할 수 있습니다."
									,"- 클릭하시면, 폴더에 있는 그룹폴더 목록을 로드 합니다."
									,"- 빈 Group 폴더 입니다." + "\n" +
									 "- 'Unity Project/" + arg + "' 폴더에서 새로운 폴더를 직접 추가 할 수 있습니다."
									,"- 클릭하시면, 폴더 안에 있는(하위 폴더 포함) 이펙트 목록을 불러옵니다."
									,"- 좌클릭: 작업할 이펙트를 선택합니다. (이전 작업 자동저장)" + "\n" +
									 "- 우클릭: PopupMenu." + "\n" +
									 "- DeleteKey 누르면 선택된 prefab이 영구 삭제 됩니다."
								}
							  };
		CheckValid(helpStr);
		string	tooltip = "";

		switch (keyword)
		{
			case "": break;
			case "EMPTYPROJECT_HOVER"	: tooltip	= helpStr[GetLang(), 0]					+ AddHintRect(FXMakerEffect.GetRect_Group);	break;
			case "PROJECT_HOVER"		: tooltip	= arg + "\n" + helpStr[GetLang(), 1]	+ AddHintRect(FXMakerEffect.GetRect_Group);	break;
			case "EMPTYGROUP_HOVER"		: tooltip	= helpStr[GetLang(), 2]					+ AddHintRect(FXMakerEffect.GetWindowRect);	break;
			case "GROUP_HOVER"			: tooltip	= arg + "\n" + helpStr[GetLang(), 3]	+ AddHintRect(FXMakerEffect.GetWindowRect);	break;
			case "EFFECT_HOVER"			: tooltip	= arg + "\n" + helpStr[GetLang(), 4]	+ AddHintRect(FXMakerHierarchy.GetWindowRect) + AddHintRect(FXMakerLayout.GetClientRect);	break;
			default: tooltip = keyword; break;
		}
		m_DictionaryStr.Add(keystr, Tooltip(tooltip));
		return m_DictionaryStr[keystr];
	}

	protected static string[,]	HcToolEffect = {
								{
									 "- Create an empty 'Effect Prefab' in the current folder."
									,"- Delete the chosen 'Effect Prefab'." + "\n" +
									 "- If the option of 'Hierarchy/_FXMaker/ToolOption/m_bActiveRecycleBin' is turned on, move to [RecycleBin]." + "\n" +
									 "- Current status = "
									,"- Create a Clone of the chosen 'Effect Prefab'."
									,"- Capture the current screen and use it as a Preview image of the chosen Effect."
									,"- Load the past Data of the chosen Effect." + "\n" +
									 "- Effect loaded in the last will be saved as Current." + "\n" +
									 "- If you are not satisfied, choose again."
									,"- Take selected Effect and make SpriteTexture."
								}
								,
								{
									 "- 빈 'Effect Prefab'을 현재의 폴더에 생성 합니다."
									,"- 선택된 'Effect Prefab'을 삭제 합니다." + "\n" +
									 "- 'Hierarchy/_FXMaker/ToolOption/m_bActiveRecycleBin' 옵션이 켜진 경우 [RecycleBin]으로 이동 합니다." + "\n" +
									 "- 현재상태 = "
									,"- 선택된 'Effect Prefab'의 Clone을 생성 합니다."
									,"- 현재 화면을 캡쳐하여, 선택된 Effect의 Preview 이미지로 사용 합니다."
									,"- 선택된 Effect의 예전 Data를 불러 옵니다." + "\n" +
									 "- 마지막으로 불러온 Effect가 Current로 저장됩니다." + "\n" +
									 "- 원하지 않을 경우 다시 선택하세요."
									,"- 선택된 Effect를 SpriteTexture로 만듭니다."
								}
							  };
	public static GUIContent GetHcToolEffect(string caption)
	{
		if (m_bTDisable)	return m_emptyContent;

		string keystr = "GetHcToolEffect" + caption;
		if (m_DictionaryCon.ContainsKey(keystr))
			return m_DictionaryCon[keystr];

		CheckValid(HcToolEffect);
		string	tooltip = "";

		switch (caption)
		{
			case ""			: break;
			case "New"		: tooltip	= HcToolEffect[GetLang(), 0];	break;
			case "Del"		: tooltip	= HcToolEffect[GetLang(), 1]	+ FXMakerOption.inst.m_bActiveRecycleBin.ToString();	break;
			case "Clone"	: tooltip	= HcToolEffect[GetLang(), 2];	break;
			case "Thumb"	: tooltip	= HcToolEffect[GetLang(), 3]	+ FXMakerTooltip.AddCommand("ShowThumbCaptureRect");	break;
			case "History"	: tooltip	= HcToolEffect[GetLang(), 4];	break;
			case "Sprite"	: tooltip	= HcToolEffect[GetLang(), 5]	+ FXMakerTooltip.AddCommand("ShowSpriteCaptureRect");	break;
			default: TooltipError(caption); break;
		}
		if (tooltip != null) tooltip = caption + "\n" + tooltip;

		m_DictionaryCon.Add(keystr, GetGUIContent(caption, tooltip));
		return m_DictionaryCon[keystr];
	}

	protected static GUIContent[] HcToolEffect_CameraX;
	public static GUIContent[] GetHcToolEffect_CameraX()
	{
		string addStr = "\nModification of angle - Hierarchy/_FXMaker/ToolOption.nCameraAngleXValue";
		if (HcToolEffect_CameraX == null)
		{
			HcToolEffect_CameraX = new GUIContent[4];
			HcToolEffect_CameraX[0]	= GetGUIContent(FXMakerOption.inst.m_nCameraAangleXValues[0].ToString(), "axis-x " + FXMakerOption.inst.m_nCameraAangleXValues[0] + addStr + "0" + " " + AddHintRect(FXMakerLayout.GetClientRect));
			HcToolEffect_CameraX[1]	= GetGUIContent(FXMakerOption.inst.m_nCameraAangleXValues[1].ToString(), "axis-x " + FXMakerOption.inst.m_nCameraAangleXValues[1] + addStr + "1" + " " + AddHintRect(FXMakerLayout.GetClientRect));
			HcToolEffect_CameraX[2]	= GetGUIContent(FXMakerOption.inst.m_nCameraAangleXValues[2].ToString(), "axis-x " + FXMakerOption.inst.m_nCameraAangleXValues[2] + addStr + "2" + " " + AddHintRect(FXMakerLayout.GetClientRect));
			HcToolEffect_CameraX[3]	= GetGUIContent(FXMakerOption.inst.m_nCameraAangleXValues[3].ToString(), "axis-x " + FXMakerOption.inst.m_nCameraAangleXValues[3] + addStr + "3" + " " + AddHintRect(FXMakerLayout.GetClientRect));
		}
		return HcToolEffect_CameraX;
	}
	protected static GUIContent[] HcToolEffect_CameraY;
	public static GUIContent[] GetHcToolEffect_CameraY()
	{
		string addStr = "\nModification of angle - Hierarchy/_FXMaker/ToolOption.nCameraAngleYValue";
		if (HcToolEffect_CameraY == null)
		{
			HcToolEffect_CameraY = new GUIContent[4];
			HcToolEffect_CameraY[0]	= GetGUIContent(FXMakerOption.inst.m_nCameraAangleYValues[0].ToString(), "axis-y " + FXMakerOption.inst.m_nCameraAangleYValues[0] + addStr + "0" + " " + AddHintRect(FXMakerLayout.GetClientRect));
			HcToolEffect_CameraY[1]	= GetGUIContent(FXMakerOption.inst.m_nCameraAangleYValues[1].ToString(), "axis-y " + FXMakerOption.inst.m_nCameraAangleYValues[1] + addStr + "1" + " " + AddHintRect(FXMakerLayout.GetClientRect));
			HcToolEffect_CameraY[2]	= GetGUIContent(FXMakerOption.inst.m_nCameraAangleYValues[2].ToString(), "axis-y " + FXMakerOption.inst.m_nCameraAangleYValues[2] + addStr + "2" + " " + AddHintRect(FXMakerLayout.GetClientRect));
			HcToolEffect_CameraY[3]	= GetGUIContent(FXMakerOption.inst.m_nCameraAangleYValues[3].ToString(), "axis-y " + FXMakerOption.inst.m_nCameraAangleYValues[3] + addStr + "3" + " " + AddHintRect(FXMakerLayout.GetClientRect));
		}
		return HcToolEffect_CameraY;
	}

	// EffectControls ===================================================================================
	protected static string[,]	HcEffectControls = {
								{
									"- Play once at the assigned speed and play repeatedly within the assigned repetition time." + "\n" +
									 "- This will be used only for testing, not be saved in 'Effect Prefab'."
									,"- Have Effect a Transform(Move, Rotate, Scale) test." + "\n" +
									 "- You can test Beam, Bullet, Move Particle, Fire, Trail, and etc."
									,"- This is a speed which will be applied in movement test. 1(One) means moving 1,0f per second." + "\n" +
									 "- It can be automatically changed by TransType or can be changed as you choose."
									,"- This is the speed of 'Unity Engine Timer'" + "\n" +
									 "- It will be used in order to slowly watch fast Effect." + "\n" +
									"- It can be automatically changed by PlayType or can be changed as you choose."
									,"- This is the time elapsed after Effect started." + "\n" +
									 "- In case of PlayType which is played once, it will increase infinitely." + "\n" +
									 "- In case of PlayType which is played repeatedly"

									,"- Change the value of movement to 1."
									,"- Decrease the value of movement by a unit of 1.0."
									,"- Increase the value of movement by a unit of 1,0."
									,"- Create the same Effect in order on 0.2-second basis in Trans simulation (successive shooting test)" + "\n" +
									 "- Left-Click: Change the number of created Effects in order of 1 to 4." + "\n" +
									 "- Right-Click: Initialize the value to 1."

									,"- Change the speed of 'Unity Engine Timer' to a default value, 1."
									,"- Change the speed of 'Unity Engine Timer' to 0 and stop Effect."
									,"- Change the speed of 'Unity Engine Timer' to the value before Pause."

									,"- Recreate Instance of Effect."
									,"- Stop after moved for 0.5 seconds."
									,"- Stop after moved for 0.1 seconds."
									,"- Stop after moved for 0.05 seconds."
									,"- Stop after moved for 0.01 seconds."
								}
								,
								{
									 "- 지정된 속도로 한번 재생, 지정된 반복시간으로 반복재생을 합니다." + "\n" +
									 "- 'Effect Prefab'에 저장되는 것은 아니며, 테스트로만 사용 됩니다."
									,"- Effect를 Transform(Move, Rotate, Scale) 테스트 합니다." + "\n" +
									 "- Beam, Bullet, Move Particle, Fire, Trail 등을 테스트 하실 수 있습니다."
									,"- 이동 테스트에 사용될 속도이며, 1일 경우 초당 1.0f 이동 합니다." + "\n" +
									 "- TransType에 의해 자동으로 변경되거나 임의로 변경 할 수 있습니다."
									,"- 'Unity Engine Timer'의 속도 입니다." + "\n" +
									 "- 빠른 이펙트를 천천히 보기 위해서 사용 됩니다." + "\n" +
									 "- PlayType에 의해 자동으로 변경되거나 임의로 변경 할 수 있습니다."
									,"- Effect가 시작된 경과 시간 입니다." + "\n" +
									 "- 한번재생 되는 PlayType의 경우 무한 증가 합니다." + "\n" +
									 "- 반복재생 되는 PlayTyye의 경우 반복 시간이 되면 Restart 됩니다."

									,"- 이동 값을 1로 변경 합니다."
									,"- 이동 값을 1.0 단위로 감소 합니다."
									,"- 이동 값을 1.0 단위로 증가 합니다."
									,"- Trans 시뮬레이션에서 같은 이펙트를 0.2초 단위로 순차 생성 합니다.(연발 테스트)" + "\n" +
									 "- 좌클릭: 생성될 이펙트 개수가 1~4 순으로 변경 됩니다." + "\n" +
									 "- 우클릭: 1로 초기화 합니다."

									,"- 'Unity Engine Timer' 속도를 기본값인 1로 변경 합니다."
									,"- 'Unity Engine Timer' 속도를 0으로 변경하며, Effect를 정지 시킵니다."
									,"- 'Unity Engine Timer' 속도를 Pause되기 전 값으로 변경합니다."

									,"- Effect의 Instance를 다시 생성 합니다."
									,"- 0.5초 이동 후 정지 합니다."
									,"- 0.1초 이동 후 정지 합니다."
									,"- 0.05초 이동 후 정지 합니다."
									,"- 0.01초 이동 후 정지 합니다."
								}
							};
	public static GUIContent GetHcEffectControls(string caption, string arg)
	{
		if (m_bTDisable)	return m_emptyContent;

		string keystr = "GetHcEffectControls" + caption + arg;
		if ((caption != "DistPerTime" && caption != "TimeScale" && caption != "ElapsedTime") && m_DictionaryCon.ContainsKey(keystr))
			return m_DictionaryCon[keystr];

		CheckValid(HcEffectControls);
		string	tooltip = "";

		switch (caption)
		{
			case "": break;

			case "Play"			:	tooltip = HcEffectControls[GetLang(), 0];		break;
			case "Trans"		:	tooltip = HcEffectControls[GetLang(), 1];		break;
			case "DistPerTime"	:	tooltip = HcEffectControls[GetLang(), 2];		break;
			case "TimeScale"	:	tooltip = HcEffectControls[GetLang(), 3];		break;
			case "ElapsedTime"	:	tooltip = HcEffectControls[GetLang(), 4];		break;

			case "1"			:	tooltip = HcEffectControls[GetLang(), 5];		break;
			case "<"			:	tooltip = HcEffectControls[GetLang(), 6];		break;
			case ">"			:	tooltip = HcEffectControls[GetLang(), 7];		break;
			case "Multi"		:	return GetGUIContent(arg + " " + caption, HcEffectControls[GetLang(), 8]		+ AddHintRect(FXMakerLayout.GetClientRect));

			case "Reset"		:	tooltip = HcEffectControls[GetLang(), 9];		break;
			case "Pause"		:	tooltip = HcEffectControls[GetLang(), 10];		break;
			case "Resume"		:	tooltip = HcEffectControls[GetLang(), 11];		break;

			case "Restart"		:	tooltip = HcEffectControls[GetLang(), 12];		break;
			case "+.5"			:	tooltip = HcEffectControls[GetLang(), 13];		break;
			case "+.1"			:	tooltip = HcEffectControls[GetLang(), 14];		break;
			case "+.05"			:	tooltip = HcEffectControls[GetLang(), 15];		break;
			case "+.01"			:	tooltip = HcEffectControls[GetLang(), 16];		break;

			default: TooltipError(caption);	break;
		}
		if (tooltip != null) tooltip = caption + "\n" + tooltip + AddHintRect(FXMakerLayout.GetClientRect);
		if (!(caption != "DistPerTime" && caption != "TimeScale" && caption != "ElapsedTime"))
			return GetGUIContent(caption, tooltip);

		m_DictionaryCon.Add(keystr, GetGUIContent(caption, tooltip));
		return m_DictionaryCon[keystr];
	}

	protected static string[,]	HcEffectControls_Play = {
								{
									 "- Calculate the time that Effect disappears on the screen and automatically repeat Play." + "\n" +
									 "- In case of Loop Effect, with 'x.xs R,' use a compulsory repeating play." + "\n" +
									 "- In case of Effect whose time of none-printing out is long, repetition processing can be inaccurate."
									,"- Play once with the value assigned in TimeScale."
									,"Play once with {0}X speed."
									,"Play repeatedly within {0}-second repetition time."
								}
								,
								{
									 "- 화면에서 사라지는 시간을 계산하여 자동으로 반복 Play 합니다." + "\n" +
									 "- Loop Effect의 경우 'x.xs R'로 강제 반복재생을 이용 하세요." + "\n" +
									 "- 무출력 타임이 긴 이펙트의 경우 반복 처리가 불 정확할 수 있습니다."
									,"- TimeScale에서 설정된 값으로 한번 Play 합니다."	
									,"- {0}배속의 속도로 한번 Play 합니다."
									,"- {0}초 반복시간으로 반복 Play 합니다."
								}
							  };
	protected static GUIContent[]	HcEffectControls_Plays = null;
	protected static float			HcEffectControls_Plays_timeScale = 0;
	public static GUIContent[] GetHcEffectControls_Play(float fAutoRet, float timeScale, float timeOneShot1, float timeRepeat1, float timeRepeat2, float timeRepeat3, float timeRepeat4, float timeRepeat5)
	{
		if (HcEffectControls_Plays == null || HcEffectControls_Plays_timeScale != timeScale)
		{
			CheckValid(HcEffectControls_Play);
			HcEffectControls_Plays = new GUIContent[8];
			HcEffectControls_Plays_timeScale = timeScale;

			GUIContent[]	cons = HcEffectControls_Plays;

			cons[0]	= GetGUIContent("AutoRet"	, HcEffectControls_Play[GetLang(), 0] + AddHintRect(FXMakerLayout.GetClientRect));
			cons[1]	= GetGUIContent(timeScale.ToString("0.00")+"x S"	, HcEffectControls_Play[GetLang(), 1]	+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[2]	= GetGUIContent(timeOneShot1.ToString("0.0")+"x S"	, "- " + string.Format(HcEffectControls_Play[GetLang(), 2], timeOneShot1.ToString("0.0"))	+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[3]	= GetGUIContent(timeRepeat1.ToString("0.0")+"s R"	, "- " + string.Format(HcEffectControls_Play[GetLang(), 3], timeRepeat1.ToString("0.0"))	+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[4]	= GetGUIContent(timeRepeat2.ToString("0.0")+"s R"	, "- " + string.Format(HcEffectControls_Play[GetLang(), 3], timeRepeat2.ToString("0.0"))	+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[5]	= GetGUIContent(timeRepeat3.ToString("0.0")+"s R"	, "- " + string.Format(HcEffectControls_Play[GetLang(), 3], timeRepeat3.ToString("0.0"))	+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[6]	= GetGUIContent(timeRepeat4.ToString("0.0")+"s R"	, "- " + string.Format(HcEffectControls_Play[GetLang(), 3], timeRepeat4.ToString("0.0"))	+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[7]	= GetGUIContent(timeRepeat5.ToString("0.0")+"s R"	, "- " + string.Format(HcEffectControls_Play[GetLang(), 3], timeRepeat5.ToString("0.0"))	+ AddHintRect(FXMakerLayout.GetClientRect));
		}
		return HcEffectControls_Plays;
	}

	protected static string[,]	HcEffectControls_Trans = {
								{
									 "- Do no movement simulation."
									,"- Do simulation of straight line movement on {0}-axis." + "\n" +
									 "- Right-Click: Change Move axis in order of x, y, and z."
									,"- Do simulation that increases Scale of {0}-axis." + "\n" +
									 "- Right-Click: Change Scale axis in order of x, y, and z."
									,"- Do simulation that makes a parabolic movement from side to side." + "\n" +
									 "- A parabolic can be modified in 'Hierarchy/_FXMaker/ToolOption/Simulrate Arc Curve'."	
									,"- Do simulation of top-to-bottom movement."
									,"- Do simulation of bottom-to-top movement."
									,"- Do simulation of rotation drawing a circle."
									,"- Do simulation of rising with drawing a circle."
								}
								,
								{
									 "- 이동 시뮬레이션을 하지 않습니다."
									,"- {0}축 직선 이동 시뮬레이션 합니다." + "\n" +
									 "- 우클릭: Move 축을 x,y,z 순으로 변경 합니다."
									,"- {0}축 Scale이 증가하는 시뮬레이션을 합니다." + "\n" +
									 "- 우클릭: scale 축을 x,y,z 순으로 변경 합니다."
									,"- 좌우로 포물선 이동하는 시뮬레이션을 합니다." + "\n" +
									 "- 포물선은 'Hierarchy/_FXMaker/ToolOption/Simulrate Arc Curve' 에서 변경 가능 합니다."	
									,"- 상하 이동 시뮬레이션 합니다."
									,"- 하상 이동 시뮬레이션 합니다."
									,"- 원을 그리는 회전 시뮬레이션을 합니다."
									,"- 원을 그리며 상승하는 시뮬레이션을 합니다."
								}
							  };
	protected static GUIContent[]	HcEffectControls_Transs;
	protected static NgEnum.AXIS	HcEffectControls_Transs_nTransAxis;
	public static GUIContent[] GetHcEffectControls_Trans(NgEnum.AXIS nTransAxis)
	{
		if (HcEffectControls_Transs == null || HcEffectControls_Transs_nTransAxis != nTransAxis)
		{
			CheckValid(HcEffectControls_Trans);
			HcEffectControls_Transs = new GUIContent[8];
			HcEffectControls_Transs_nTransAxis = nTransAxis;
			GUIContent[]	cons = HcEffectControls_Transs;

			cons[0]	= GetGUIContent("Stop"		, HcEffectControls_Trans[GetLang(), 0] + AddHintRect(FXMakerLayout.GetClientRect));
			cons[1]	= GetGUIContent(nTransAxis.ToString() + " Move"	, "- " + string.Format(HcEffectControls_Trans[GetLang(), 1], nTransAxis.ToString()) + AddHintRect(FXMakerLayout.GetClientRect));
			cons[2]	= GetGUIContent(nTransAxis.ToString() + " Scale"	, "- " + string.Format(HcEffectControls_Trans[GetLang(), 2], nTransAxis.ToString())+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[3]	= GetGUIContent("Arc"		, HcEffectControls_Trans[GetLang(), 3]	+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[4]	= GetGUIContent("Fall"		, HcEffectControls_Trans[GetLang(), 4]	+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[5]	= GetGUIContent("Raise"		, HcEffectControls_Trans[GetLang(), 5]	+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[6]	= GetGUIContent("Circle"	, HcEffectControls_Trans[GetLang(), 6]	+ AddHintRect(FXMakerLayout.GetClientRect));
			cons[7]	= GetGUIContent("Tornado"	, HcEffectControls_Trans[GetLang(), 7]	+ AddHintRect(FXMakerLayout.GetClientRect));
		}
		return HcEffectControls_Transs;
	}

	protected static string[,]	HcEffectControls_Rotate = {
								{
									 "- During movement simulation, use Front as Z+."
									,"- During movement simulation, do not rotate an axis."
								}
								,
								{
									 "- 이동 시뮬레이션 할 때 Front를 Z+로 사용합니다."
									,"- 이동 시뮬레이션 할 때 축을 회전하지 않습니다."
								}
							  };
	protected static GUIContent[] HcEffectControls_Rotates;
	public static GUIContent[] GetHcEffectControls_Rotate()
	{
		if (HcEffectControls_Rotates == null)
		{
			CheckValid(HcEffectControls_Rotate);
			HcEffectControls_Rotates = new GUIContent[2];

			HcEffectControls_Rotates[0]	= GetGUIContent("Rot"	, HcEffectControls_Rotate[GetLang(), 0]	+ AddHintRect(FXMakerLayout.GetClientRect));
			HcEffectControls_Rotates[1]	= GetGUIContent("Fix"	, HcEffectControls_Rotate[GetLang(), 1]	+ AddHintRect(FXMakerLayout.GetClientRect));
		}
		return HcEffectControls_Rotates;
	}

	// Gizmo Toolbar ===================================================================================
	protected static string[,]	HcGizmo = {
								{
									 "- You can indicate the axis of the chosen GameObject and move a camera."
									,"- You can indicate the axis of the chosen GameObject and edit the value of the position of WorldAxis."
									,"- You can indicate the axis of the chosen GameObject and edit the value of the rotation of WorldAxis."
									,"- You can indicate the axis of the chosen GameObject and edit the value of the size of WorldAxis."
									,"- Do not indicate Gizmos." + "\n" +
									 "- (Comments) If Gizmos becomes off, BoundsBox is also not indicated."
								}
								,
								{
									 "- 선택된 GameObject의 Axis를 표시하고, 카메라를 이동 할 수 있습니다."
									,"- 선택된 GameObject의 Axis를 표시하고, WorldAxis 위치 값을 편집 할 수 있습니다."
									,"- 선택된 GameObject의 Axis를 표시하고, WorldAxis 회전 값을 편집 할 수 있습니다."
									,"- 선택된 GameObject의 Axis를 표시하고, WorldAxis 크기 값을 편집 할 수 있습니다."
									,"- Gizmos를 표시하지 않습니다." + "\n" +
									 "- (참고) Gizmos가 off되면 BoundsBox도 표시되지 않습니다."
								}
							  };
	protected static GUIContent[] HcGizmos;
	public static GUIContent[] GetHcGizmo()
	{
		if (HcGizmos == null)
		{
			CheckValid(HcGizmo);
			HcGizmos = new GUIContent[5];

			HcGizmos[0]	= GetGUIContentImage(null, "Hand [ ShotKey = Q ]\n"		+ HcGizmo[GetLang(), 0]);
			HcGizmos[1]	= GetGUIContentImage(null, "Position [ ShotKey = W ]\n" + HcGizmo[GetLang(), 1]);
			HcGizmos[2]	= GetGUIContentImage(null, "Rotation [ ShotKey = E ]\n" + HcGizmo[GetLang(), 2]);
			HcGizmos[3]	= GetGUIContentImage(null, "Scale [ ShotKey = R ]\n"	+ HcGizmo[GetLang(), 3]);
			HcGizmos[4]	= GetGUIContentImage(null, "None [ ShotKey = T ]\n"		+ HcGizmo[GetLang(), 4]);
		}
		return HcGizmos;
	}

	protected static string[,]	HcEffectGizmoStrrings = {
								{
									 "- Do Rendering only chosen GameObject into Color." + "\n" +
									 "- The rest GameObject will become Rendering into Gray." + "\n" +
									 "- This function is supported by Unity Pro only."
									,"- Print out Bounds Box of the chosen GameObject." + "\n" +
									 "- This function is only indicated when the Gizmos mode is ON."
									,"- Do Rendering chosen GameObject into Wireframe."
									,"- Decide method for print and control of axis(World/Local). [Shotkey = Y]" + "\n" +
									 "- Warning for 'Local' : GameObject being handled by Billboard has to be restarted when controled by GizmoAxis."
									,"- z-Axis will be fixed once selected."
									,"- Decide whether to display XY-Grid." + "\n" +
									 "- Modification of size - Hierarchy/_FXMaker/ToolOption.GridUnit,GridSize"
									,"- Decide whether to display XZ-Grid." + "\n" +
									 "- Modification of size - Hierarchy/_FXMaker/ToolOption.GridUnit,GridSize"
									,"- Decide whether to display YZ-Grid." + "\n" +
									 "- Modification of size - Hierarchy/_FXMaker/ToolOption.GridUnit,GridSize"
									,"- Decide whether to display Background set up."
									,"- Using grid movement.(GridUnit:Hierarchy/_FXMaker/ToolOption.fGizmoGridMoveUnit)"
								},{
									 "- 선택된 GameObject만 Color로 Rendering 합니다." + "\n" +
									 "- 나머지 GameObject는 Gray로 Rendering 됩니다." + "\n" +
									 "- 이 기능은 Unity Pro 버전에서만 지원하는 기능 입니다."
									,"- 선택된 GameObject의 Bounds Box를 출력 합니다." + "\n" +
									 "- 이 기능은 Gizmos가 ON 상태일 때만 표시 됩니다."
									,"- 선택된 GameObject를 Wireframe으로 Rendering 합니다."
									,"- Axis의 출력 및 컨트롤 방식(World/Local)을 결정합니다.[ ShotKey = Y ]" + "\n" +
									 "- 'Local' - 주의사항 :  Billboard 처리중인 GameObject는 GizmoAxis로 컨트롤 할 경우 Restart를 해야합니다."
									,"- 선택될 경우 z-Axis은 고정 됩니다."
									,"- XY-Grid를 표시 여부를 결정 합니다." + "\n" +
									 "- 크기수정 - Hierarchy/_FXMaker/ToolOption.GridUnit,GridSize"
									,"- XZ-Grid를 표시 여부를 결정 합니다." + "\n" +
									 "- 크기수정 - Hierarchy/_FXMaker/ToolOption.GridUnit,GridSize"
									,"- YZ-Grid를 표시 여부를 결정 합니다." + "\n" +
									 "- 크기수정 - Hierarchy/_FXMaker/ToolOption.GridUnit,GridSize"
									,"- 설정한 Background의 표시 여부를 결정 합니다."
									,"- 마우스로 gizmo 이동을 할 경우 Grid단위로 이동합니다.(이동값 설정:Hierarchy/_FXMaker/ToolOption.fGizmoGridMoveUnit)"
								}
							  };

	public static GUIContent GetHcEffectGizmo(string caption)
	{
		if (m_bTDisable)	return m_emptyContent;

		string keystr = "GetHcEffectGizmo" + caption;
		if (m_DictionaryCon.ContainsKey(keystr))
			return m_DictionaryCon[keystr];

// 		CheckValid(HcEffectGizmoStrrings);
		string	tooltip = "";

		switch (caption)
		{
			case "": break;
			case "Gray"				:	tooltip = caption + "\n" + HcEffectGizmoStrrings[GetLang(), 0];		break;
			case "Box"				:	tooltip = caption + "\n" + HcEffectGizmoStrrings[GetLang(), 1];		break;
			case "Wire"				:	tooltip = caption + "\n" + HcEffectGizmoStrrings[GetLang(), 2];		break;

			case "World"			:	tooltip = caption + "\n" + HcEffectGizmoStrrings[GetLang(), 3];		break;
			case "Fixed-Z"			:	tooltip = caption + "\n" + HcEffectGizmoStrrings[GetLang(), 4];		break;

			case "XY"				:	tooltip = caption + "\n" + HcEffectGizmoStrrings[GetLang(), 5];		break;
			case "XZ"				:	tooltip = caption + "\n" + HcEffectGizmoStrrings[GetLang(), 6];		break;
			case "YZ"				:	tooltip = caption + "\n" + HcEffectGizmoStrrings[GetLang(), 7];		break;
			case "Background"		:	tooltip = caption + "\n" + HcEffectGizmoStrrings[GetLang(), 8];		break;
			case "GM"				:	tooltip = caption + "\n" + HcEffectGizmoStrrings[GetLang(), 9];		break;

			default: TooltipError(caption);	break;
		}

		m_DictionaryCon.Add(keystr, GetGUIContent(caption, tooltip));
		return m_DictionaryCon[keystr];
	}

	// EffectHierarchy ===================================================================================
	protected static string[,]	HcEffectHierarchy = {
								{
									 "- Indicate the whole Effect."
									,"- Do Rendering only chosen GameObject." + "\n" +
									 "- Do no Rendering the rest GameObjet." + "\n" +
									 "- As Transform of PareantObject is not applied, the values of Position/Rotate/Scale can be different."

									,"- Indicate Unity Transform on PrefabHierarchy."
									,"- Indicate FXMaker Component on PrefabHierarchy."
									,"- Indicate UnityEngine Component on PrefabHierarchy."
									,"- Indicate Material on PrefabHierarchy."
									,"- Indicate Animation on PrefabHierarchy."
									,"- Inidicate Component made by User on PrefabHierarchy."

									,"- Regulate the speed of all Component including children." + "\n" +
									 "Possibility: FXMaker Script, ShurikenParticle" + "\n" +
									 "Impossibility: Animation, LegacyParticle"

									,"- Name : Change the name of GameObject." + "\n" +
									 "- Add Component : Add EffectScript to GameObject."
									,"- Add EffectPrefab to the chosen GameObject."
									,"- Designate Mesh registered in FX Maker." + "\n" +
									 "- MeshPrefab or Object that includes MeshFilter in its Root will be indicated only."
									,""
									,"- Edit Material : Edit Material, Shader, Color, Texture, and etc."
									,"- Add Material : Increase the number of Materials of Renderer."
									,"- GameObject : In Inspector, you cannot change the Active mode; You can decide whether to use or not with this function." + "\n" +
									 "- Component : the same with Enable/Disable function in Inspector."
								}
								,
								{
									 "- 이펙트 전체를 표시 합니다."
									,"- 선택된 GameObject만 Rendering 합니다." + "\n" +
									 "- 나머지 GameObject는 Rendering하지 않습니다." + "\n" +
									 "- Pareant의 Transform이 적용되지 않기에 Position/Rotate/Scale 값이 다를 수 있습니다."

									,"- Unity Transform을 PrefabHierarchy에 표시 합니다."
									,"- FXMaker Component를 PrefabHierarchy에 표시 합니다."
									,"- UnityEngine Component를 PrefabHierarchy에 표시 합니다."
									,"- Material을 PrefabHierarchy에 표시 합니다."
									,"- Animation을 PrefabHierarchy에 표시 합니다."
									,"- User가 만든 Component를 PrefabHierarchy에 표시 합니다."

									,"- children을 포함한 모든 Compoent의 속도를 조절한다." + "\n" +
									 "가능: FXMaker Script, ShurikenParticle" + "\n" +
									 "불가: Animation, LegacyParticle"

									,"- Name : GameObject 이름 변경합니다." + "\n" +
									 "- Add Component : GameObject에 EffectScript를 추가합니다."
									,"- 선택된 GameObject에 EffectPrefab을 추가합니다."
									,"- FX Maker에 등록된 Mesh를 지정 합니다." + "\n" +
									 "- MeshPrefab 또는 Root에 MeshFilter를 포함한 Object만 표시 됩니다."
									,""
									,"- Edit Material : Material, Shader, Color, Texture등을 변경합니다."
									,"- Add Material : Renderer의 Materials 수를 증가 시킵니다."
									,"- GameObject : Inspector에서는 Active 상태를 변경할 수 없으며, 이 기능으로 사용 유무를 결정할 수 있습니다." + "\n" +
									 "- Component : Inspector에 있는 Enable/Disable 기능과 동일합니다."
								}
							  };
	public static GUIContent GetHcEffectHierarchy(string caption)
	{
		if (m_bTDisable)	return m_emptyContent;

		string keystr = "GetHcEffectHierarchy" + caption;
		if (m_DictionaryCon.ContainsKey(keystr))
			return m_DictionaryCon[keystr];

		CheckValid(HcEffectHierarchy);
		string	tooltip = "";

		switch (caption)
		{
			case "": break;
			case "All"					:	tooltip = caption + "\n" + HcEffectHierarchy[GetLang(), 0];		break;
			case "Selected"				:	tooltip = caption + "\n" + HcEffectHierarchy[GetLang(), 1];		break;

			case "Transform"			:	tooltip = caption + "\n" + HcEffectHierarchy[GetLang(), 2];		break;
			case "FXMaker"				:	tooltip = caption + "\n" + HcEffectHierarchy[GetLang(), 3];		break;
			case "UnityEngine"			:	tooltip = caption + "\n" + HcEffectHierarchy[GetLang(), 4];		break;
			case "Material"				:	tooltip = caption + "\n" + HcEffectHierarchy[GetLang(), 5];		break;
			case "Animation"			:	tooltip = caption + "\n" + HcEffectHierarchy[GetLang(), 6];		break;
			case "Other"				:	tooltip = caption + "\n" + HcEffectHierarchy[GetLang(), 7];		break;

			case "Speed"				:	tooltip	= caption + "\n" + HcEffectHierarchy[GetLang(), 8];		break;

			case "Popup_GameObject"		:	caption = "E";		tooltip = HcEffectHierarchy[GetLang(), 9];	break;
			case "Popup_AddPrefab"		:	caption = "P";		tooltip = "- Add Prefab"	+ "\n" + HcEffectHierarchy[GetLang(), 10]; break;
			case "Popup_Mesh"			:	caption = "...";	tooltip = "- Change Mesh"	+ "\n" + HcEffectHierarchy[GetLang(), 11]; break;
			case "Popup_Transform"		:	caption = "...";	tooltip = "- Edit Transform";	break;
			case "Popup_Material"		:	caption = "...";	tooltip = HcEffectHierarchy[GetLang(), 13]; break;
			case "AddOn_AddMaterial"	:	caption = "+";		tooltip = HcEffectHierarchy[GetLang(), 14]; break;
			case "Enable"				:	return GetGUIContent("", "- Enable / Disable\n" + HcEffectHierarchy[GetLang(), 15]);

			default: TooltipError(caption);	break;
		}
		m_DictionaryCon.Add(keystr, GetGUIContent(caption, tooltip));
		return m_DictionaryCon[keystr];
	}

	public static GUIContent GetHcEffectHierarchy_Box(string caption, bool bGameObject, bool bScriptComponent)
	{
		if (m_bTDisable)	return m_emptyContent;

		string	tooltip = "";

//		CheckValid(helpStr);
		// ani, mat
		tooltip = "";

		if (bGameObject)
			tooltip = "";
		if (bScriptComponent)
			tooltip = "";

		return GetGUIContent(caption, tooltip);
	}


	// FxmSpritePopup ===================================================================================
	protected static string[,]	HcPopup_Sprite = {
								{
									""
									,"- Animation : Consecutively Capture according to Total Frame." + "\n" +
									 "- Random : Capture the first frame regenerating Effect according to TotalFrame."
									,"- Select Capture size."
									,"- Select number of Frame per second to capture." + "\n" +
									 "- This value can be modified by 'Edit/Project Settings/Time/Fixed Timestep'."
									,"- Select Capture time." + "\n" +
									 "- If 'PreFabSimulate/Autoret' is selected, play time can be brought by 'Get Duration' button."
									,"- Select the number of frame to skip."
									,"- Decide the form of Sprite output."
									,"- The number of frames to be captured (saved)"
									,"- Original size of SpriteTexture."
									,"- Additive : Use 'Paticles/Additive Shader'. (Color adjustable)" + "\n" +
									 "- AdditiveMobile : Use 'Mobile/Paticles/Additive Shader'. (Color Unadjustable)" + "\n" +
									 "- Additivesoft: Use 'Paticles/Additive (Soft) Shader'. (Color Unadjustable)" + "\n" +
									 "- AlphaBlended - Add an alpha channel to the texture.(You can adjust the NcSpriteAnimation AlphaChannel)" + "\n" +
									 "- AlphaBlended : Use 'Paticles/Alpha Blended Shader'. (Color adjustable)" + "\n" +
									 "- AlphaBlendedMobile : Use 'Mobile/Paticles/Alpha Blended Shader'. (Color Unadjustable)"
									,"- Decide whether to use FadeIn/FadeOut." + "\n" +
									 "- Only usable at Shader which ColorProperty exists. (Able : Additive)" + "\n" +
									 "- Range of FadeIn/FadeOut can be modified by Curve at relevant NcCurveAnimation/Color."
									,"- PreFab : EffectPreFab + Material + SpriteTexture" + "\n" +
									 "- Texture : Create only SpriteTexture. (CreatePath : [Resources]/[Sprite])"
									,"- Prefer to select Texture." + "\n" +
									 "- When selecting GUI, mipMap and AnisoLevel is unuvailable."
									,"- Reduce size by designating Texture maximum size."
									,"- Reduce size by selecting format of Texture."
									,"- Once : Play only once by designated PlayMode." + "\n" +
									 "- Loop : Play repeatedly by designated PlayMode."
									,"- Select AnisoLevel." + "\n" +
									 "- GUI=0, Texture.Stand=1, Texture.Ground=4"
									,"- Decide the number of frmaes to Capture."
									,"- Effect gets displayed, and when the time period is over, execute Capture."
								}
								,
								{
									 ""
									,"- Animation : TotalFrame만큼 연속 Capture 합니다." + "\n" +
									 "- Random : TotalFrame만큼 Effect를 다시 생성하면서 첫Frame만 Capture 합니다."
									,"- Capture할 사이즈를 선택 합니다."
									,"- Capture할 초당 Frame 수를 선택합니다." + "\n" +
									 "- 이 값은 'Edit/Project Settings/Time/Fixed Timestep'에 의해 변경할 수 있습니다."
									,"- Capture할 시간을 선택 합니다." + "\n" +
									 "- 'PrefabSimulate/AutoRet'를 선택하였을 경우 'Get Duration'버튼을 통해 play 시간을 가져올 수 있습니다."
									,"- Skip할 Frame 수를 선택합니다."
									,"- Sprite 출력 순서를 결정 합니다."
									,"- Capture될 Frame 수(저장 될 Frame 수)"
									,"- SpriteTexture의 원본 크기 입니다."
									,"- Additive : 'Paticles/Additive Shader'를 사용 합니다. (Color 변경 가능)" + "\n" +
									 "- AdditiveMobile : 'Mobile/Paticles/Additive Shader'를 사용 합니다. (Color 변경 불가능)" + "\n" +
									 "- AdditiveSoft : 'Paticles/Additive (Soft) Shader'를 사용 합니다. (Color 변경 불가능)" + "\n" +
									 "- AlphaBlended - 텍스쳐에 Alpha Chnnel이 포함됩니다.(NcSpriteAnimation 에서 AlphaChannel을 조절할 수 있습니다)" + "\n" +
									 "- AlphaBlended : 'Paticles/Alpha Blended Shader'를 사용 합니다. (Color 변경 가능)" + "\n" +
									 "- AlphaBlendedMobile : 'Mobile/Paticles/Alpha Blended Shader'를 사용 합니다. (Color 변경 불가능)"
									,"- FadeIn/FadeOut의 사용 여부를 선택합니다." + "\n" +
									 "- ColorProperty가 있는 Shader에서만 사용 가능 합니다. (가능 : Additive)" + "\n" +
									 "- FadeIn/FadeOut 범위는 포함되는 NcCurveAnimation/Color에서 Curve로 조절할 수 있습니다."
									,"- Prefab : EffectPrefab(Prefab, Material, SpriteTexture)을 생성 합니다. (path = Current)" + "\n" +
									 "- Texture : SpriteTexture만 생성 합니다.(CreatePath : [Resources]/[Sprite])"
									,"- 가급적 Texture를 선택 하세요." + "\n" +
									 "- GUI를 선택할 경우 mipMap 및 AnisoLevel을 사용할 수 없습니다."
									,"- Texture의 최대 크기를 지정하여, 사이즈를 줄입니다."
									,"- Texture의 의 포맷을 선택하여, 사이즈를 줄입니다."
									,"- Once : 지정된 PlayMode로 한번만 Play 합니다." + "\n" +
									 "- Loop : 지정된 PlayMode로 반복 Play 합니다."
									,"- AnisoLevel을 선택합니다." + "\n" +
									 "- GUI=0, Texture.Stand=1, Texture.Ground=4"
									,"- Capture할 Frame 수를 결정 합니다."
									,"- Efffect가 표시되고, 이 시간이 경과하면 Capture 합니다."
								}
							  };
	public static GUIContent GetHcPopup_Sprite(string caption)
	{
		if (m_bTDisable)	return m_emptyContent;

		string keystr = "GetHcPopup_Sprite" + caption;
		if (m_DictionaryCon.ContainsKey(keystr))
			return m_DictionaryCon[keystr];

		CheckValid(HcPopup_Sprite);
		string	tooltip = "";

		switch (caption)
		{
			case "": break;
			case "CaptureType":			tooltip	= HcPopup_Sprite[GetLang(), 1]; break;
			case "Size":				tooltip	= HcPopup_Sprite[GetLang(), 2]; break;
			case "Fps":					tooltip	= HcPopup_Sprite[GetLang(), 3]; break;
			case "Time":				tooltip = HcPopup_Sprite[GetLang(), 4]; break;
			case "SkipFrame":			tooltip = HcPopup_Sprite[GetLang(), 5]; break;
			case "PlayMode":			tooltip = HcPopup_Sprite[GetLang(), 6]; break;
			case "TotalFrame":			tooltip = HcPopup_Sprite[GetLang(), 7]; break;
			case "TextureSize":			tooltip = HcPopup_Sprite[GetLang(), 8]; break;
			case "Shader":				tooltip = HcPopup_Sprite[GetLang(), 9]; break;
			case "Fade":				tooltip = HcPopup_Sprite[GetLang(),10]; break;
			case "Create":				tooltip = HcPopup_Sprite[GetLang(),11]; break;
			case "TextureType":			tooltip = HcPopup_Sprite[GetLang(),12]; break;
			case "MaxSize":				tooltip = HcPopup_Sprite[GetLang(),13]; break;
			case "Format":				tooltip = HcPopup_Sprite[GetLang(),14]; break;
			case "Loop":				tooltip = HcPopup_Sprite[GetLang(),15]; break;
			case "AnisoLevel":			tooltip = HcPopup_Sprite[GetLang(),16]; break;
			case "CaptureCount":		tooltip = HcPopup_Sprite[GetLang(),17]; break;
			case "SkipTime":			tooltip = HcPopup_Sprite[GetLang(),18]; break;
					
			default: TooltipError(caption); break;
		}
		m_DictionaryCon.Add(keystr, GetGUIContent(caption, tooltip));
		return m_DictionaryCon[keystr];
	}

	// Popup GameObject ===================================================================================
	protected static string[,]	HcPopup_GameObject = {
								{
									 "- Save in Clipboard." + "\n" +
									 "- You can Paste by opening other Effect."
									,"- Save in Clipboard and delete the original." + "\n" +
									 "- You can Paste by opening other Effect."
									,"- Add Object in Clipboard to GameObject."
									,"- Overwrite Object in Clipboard on the chosen Object."
									,""
									,"- Change into Enable mode."
									,"- Change into Disable mode."
									,"- Delete permanently." + "\n" +
									 "- You can also delete with 'Delete Key'."

									,"- Add Empty GameObject as Child."
									,"- Choose Prefab registered in Tool and add it as Child."
									,"- Add Empty GameObject as Parent."
									,"- Move GameObject to the high rank."
									,"- Save GameObject separately." + "\n" +
									 "- You can load with Add Prefab function."
									,"- Save Color in Clipboard."
									,"- Apply Color in Clipboard to current Material."

									,"- Add Component(EffectScript, UserScript, UnityComponent) to GameObject."
									,"- Add the chosen Script to GameObject." + "\n" +
									 "- If you put 'User Script' in Script folder, it will be shown in the list." + "\n" +
									 "- If the name of folder includes 'Hide,' it will not be shown in the list."

									,"- Set and save Curve." + "\n" +
									 "- You can load Curve with LoadCurves function."
									,"- Copy assigned Curve."
								}
								,
								{
									 "- Clipboard에 저장 합니다." + "\n" +
									 "- 다른 Effect를 오픈하여 Paste 할 수 있습니다."
									,"- Clipboard에 저장하고 원본은 삭제 합니다." + "\n" +
									 "- 다른 Effect를 오픈하여 Paste 할 수 있습니다."
									,"- Clipboard에 있는 Object를 GameObject에 추가 합니다."
									,"- Clipboard에 있는 Object를 선택한 Object에 Overwrite 합니다."
									,""
									,"- Enable 상태로 전환합니다."
									,"- Disable 상태로 전환합니다."
									,"- 영구 삭제 합니다." + "\n" +
									 "- 'Delete Key'로도 삭제할 수 있습니다."

									,"- Empty GameObject를 Child로 추가 합니다."
									,"- Tool에 등록된 Prefab을 선택하여 Child로 추가 합니다."
									,"- Empty GameObject를 Parent로 추가 합니다."
									,"- GameObject를 상위로 이동 합니다."
									,"- GameObject를 따로 저장 합니다." + "\n" +
									 "- Add Prefab 기능으로 불러올 수 있습니다."
									,"- Clipboard에 Color를 저장 합니다."
									,"- Clipboard에 있는 Color를 현재의 Material에 적용 합니다."

									,"- 선택된 GameObject에 Component를 추가 합니다." + "\n" +
									 "- (EffectScript, UserScript, UnityComponent)"
									,"- GameObject에 선택된 Script를 추가합니다." + "\n" +
									 "- 'User Script'를 Script 폴더에 넣어두시면 목록에 표시됩니다." + "\n" +
									 "- 폴더명에 'Hide'가 포함될 경우 목록에 표시되지 않습니다.."

									,"- 설정된 Curve를 저장 합니다." + "\n" +
									 "- LoadCurves 기능으로 저장된 Curve를 불러올 수 있습니다."
									,"- 저정된 Curve를 복사해 옵니다."
								}
							  };
	public static GUIContent GetHcPopup_GameObject(string caption, string arg)
	{
		if (m_bTDisable)	return m_emptyContent;

		CheckValid(HcPopup_GameObject);
		string	tooltip = "";

		switch (caption)
		{
			case "": break;
			// GameObject
			case "Copy":			tooltip	= HcPopup_GameObject[GetLang(), 0];		break;
			case "Cut": 			tooltip	= HcPopup_GameObject[GetLang(), 1];		break;
			case "Paste": 			tooltip	= HcPopup_GameObject[GetLang(), 2]		+ "\n" + "Source : " + arg; break;
			case "Overwrite":		tooltip	= HcPopup_GameObject[GetLang(), 3]		+ "\n" + "Source : " + arg; break;
			case "Duplicate": 		tooltip	= "- Duplicate" + arg; break;
			case "Enable":			tooltip	= HcPopup_GameObject[GetLang(), 5];		break;
			case "Disable":			tooltip	= HcPopup_GameObject[GetLang(), 6];		break;
			case "Delete": 			tooltip	= HcPopup_GameObject[GetLang(), 7];		break;

			case "Add Child": 		tooltip	= HcPopup_GameObject[GetLang(), 8];		break;
			case "Add Prefab": 		tooltip	= HcPopup_GameObject[GetLang(), 9];		break;
			case "Add Parent": 		tooltip	= HcPopup_GameObject[GetLang(), 10];	break;
			case "MoveToParent": 	tooltip	= HcPopup_GameObject[GetLang(), 11];	break;
			case "Save Prefab": 	tooltip	= HcPopup_GameObject[GetLang(), 12];	break;
			case "Copy Color":	 	tooltip	= HcPopup_GameObject[GetLang(), 13];	break;
			case "Paste Color":		tooltip	= HcPopup_GameObject[GetLang(), 14];	break;

			case "Add Component": 	tooltip	= HcPopup_GameObject[GetLang(), 15];	break;
			case "Add Component ": 	tooltip	= HcPopup_GameObject[GetLang(), 16];	break;

			// Component
			case "SaveCurves": 		tooltip	= HcPopup_GameObject[GetLang(), 17];	break;
			case "LoadCurves": 		tooltip	= HcPopup_GameObject[GetLang(), 18];	break;

			default: TooltipError(caption); break;
		}
		return GetGUIContent(caption, tooltip);
	}

	// Popup EffectScript ===================================================================================
	public delegate string delNcScript(string caption);
	public static GUIContent GetHcPopup_EffectScript(string caption)
	{
		if (m_bTDisable)	return m_emptyContent;

		string keystr = "GetHcPopup_EffectScript" + caption;
		if (m_DictionaryCon.ContainsKey(keystr))
			return m_DictionaryCon[keystr];

		string	tooltip = caption;
		SortedList<string, delNcScript>	list = new SortedList<string,delNcScript>();

		list.Add("NcAddForce", GetHsEditor_NcAddForce);
		list.Add("NcDelayActive", GetHsEditor_NcDelayActive);
		list.Add("NcAutoDeactive", GetHsEditor_NcAutoDeactive);
		list.Add("NcAutoDestruct", GetHsEditor_NcAutoDestruct);
		list.Add("NcCurveAnimation", GetHsEditor_NcCurveAnimation);
		list.Add("NcUvAnimation", GetHsEditor_NcUvAnimation);
		list.Add("NcAttachPrefab", GetHsEditor_NcAttachPrefab);
		list.Add("NcAttachSound", GetHsEditor_NcAttachSound);
		list.Add("NcBillboard", GetHsEditor_NcBillboard);
		list.Add("NcDetachParent", GetHsEditor_NcDetachParent);
		list.Add("NcDuplicator", GetHsEditor_NcDuplicator);
		list.Add("NcParticleSpiral", GetHsEditor_NcParticleSpiral);
		list.Add("NcParticleEmit", GetHsEditor_NcParticleEmit);
		list.Add("NcParticleSystem", GetHsEditor_NcParticleSystem);
		list.Add("NcRotation", GetHsEditor_NcRotation);
		list.Add("NcTilingTexture", GetHsEditor_NcTilingTexture);
		list.Add("NcSpriteAnimation", GetHsEditor_NcSpriteAnimation);
		list.Add("NcSpriteTexture", GetHsEditor_NcSpriteTexture);
		list.Add("NcSpriteFactory", GetHsEditor_NcSpriteFactory);

		if (list.ContainsKey(caption))
		{
			delNcScript	fun = list[caption];
			if (fun != null)
				tooltip = fun("");
		}
		m_DictionaryCon.Add(keystr, GetGUIContent(caption, tooltip));
		return m_DictionaryCon[keystr];
	}

	// Popup_Transform ===================================================================================
	protected static string[,]	HcPopup_Transform = {
								{
									 "- Indicate World Transform and change to the value of World."
									,"- Indicate Local Transform and change to the value of Local."
									,"- Change to the default value."
									,"- Copy the current value to Clipboard."
									,"- Load the value in Clipboard."
									,"- Left-Click: -1" + "\n" +
									 "- Right-Click: -0.1"
									,"- Left-Click: +1" + "\n" +
									 "- Right-Click: +0.1"
									,"- Left-Click: -90" + "\n" +
									 "- Right-Click: -15"
									,"- Left-Click: +90" + "\n" +
									 "- Right-Click: +15"
									,"- Left-Click: decrease by half" + "\n" +
									 "- Right-Click: decrease by 10%"
									,"- Left-Click: increase by double" + "\n" +
									 "- Right-Click: increase by 10%"
								}
								,
								{
									 "- World Transform을 표시되며, World 값으로 수정 합니다."
									,"- Local Transform을 표시되며, Local 값으로 수정 합니다."
									,"- 초기값으로 변경합니다."
									,"- 현재의 값을 Clipboard에 복사해 둡니다."
									,"- Clipboard에 있는 값을 가져옵니다."
									,"- 좌클릭: -1" + "\n" +
									 "- 우클릭: -0.1"
									,"- 좌클릭: +1" + "\n" +
									 "- 우클릭: +0.1"
									,"- 좌클릭: -90" + "\n" +
									 "- 우클릭: -15"
									,"- 좌클릭: +90" + "\n" +
									 "- 우클릭: +15"
									,"- 좌클릭: 절반 감소" + "\n" +
									 "- 우클릭: 10% 감소"
									,"- 좌클릭: 두배 증가" + "\n" +
									 "- 우클릭: 10% 증가"
								}
							  };
	public static GUIContent GetHcPopup_Transform(string caption)
	{
		if (m_bTDisable)	return m_emptyContent;

		string keystr = "GetHcPopup_Transform" + caption;
		if (m_DictionaryCon.ContainsKey(keystr))
			return m_DictionaryCon[keystr];

		CheckValid(HcPopup_Transform);
		string	tooltip = "";

		switch (caption)
		{
			case "": break;
			// Transform
			case "World":	tooltip	= caption + "\n" + HcPopup_Transform[GetLang(), 0]; break;
			case "Local":	tooltip	= caption + "\n" + HcPopup_Transform[GetLang(), 1]; break;
			case "Reset":	tooltip	= caption + "\n" + HcPopup_Transform[GetLang(), 2]; break;
			case "Copy":	tooltip	= caption + "\n" + HcPopup_Transform[GetLang(), 3]; break;
			case "Paste": 	tooltip	= caption + "\n" + HcPopup_Transform[GetLang(), 4]; break;
			case "-1": 		tooltip	= caption + "\n" + HcPopup_Transform[GetLang(), 5]; break;
			case "1":		tooltip	= caption + "\n" + HcPopup_Transform[GetLang(), 6]; break;
			case "-90": 	tooltip	= caption + "\n" + HcPopup_Transform[GetLang(), 7]; break;
			case "90": 		tooltip	= caption + "\n" + HcPopup_Transform[GetLang(), 8]; break;
			case "0.5": 	tooltip	= caption + "\n" + HcPopup_Transform[GetLang(), 9]; break;
			case "2":		tooltip	= caption + "\n" + HcPopup_Transform[GetLang(),10]; break;
			default: TooltipError(caption); break;
		}
		m_DictionaryCon.Add(keystr, GetGUIContent(caption, tooltip));
		return m_DictionaryCon[keystr];
	}

	// FolderPopup Common ===================================================================================
	protected static string[,]	HcFolderPopup_Common = {
								{
									 "- Display an image and a name together."
									,"- Do not divide into sub directories; show all contents in the list."
									,"- Finish operations and close the Popup window." + "\n" +
									 "- As the chosen operation is already saved, choose Undo first if you want to cancel" + "\n" +
									 "- Though the window is closed, you can use Undo function by opening the window again."
									,"- Cancel save."
									,"- Save in the chosen category."
									,"- Remove the chosen prefab."
								}
								,
								{
									 "- 이미지와 이름을 같이 표시 합니다."
									,"- sub directory로 구분하지 않고, 모든 내용물을 리스트에 보여 줍니다."
									,"- 작업을 끝내고, Popup창을 닫습니다." + "\n" +
									 "- 선택한 작업은 이미 저장되어 있기에 취소를 원하시면 Undo를 먼저 선택하세요." + "\n" +
									 "- 창을 닫더라도 다시 열어서 Undo 기능을 사용할 수 있습니다."
									,"- 저장을 취소 합니다."
									,"- 선택된 카테고리에 저장합니다."
									,"- 선택된 prefab을 제거 합니다."
								}
							  };
	public static GUIContent GetHcFolderPopup_Common(string caption)
	{
		if (m_bTDisable)	return m_emptyContent;

		CheckValid(HcFolderPopup_Common);
		string	tooltip = "";

		switch (caption)
		{
			case "": break;
			case "ShowName":		tooltip	= HcFolderPopup_Common[GetLang(), 0]; break;
			case "Recursively": 	tooltip	= HcFolderPopup_Common[GetLang(), 1]; break;
			case "Close":			tooltip = HcFolderPopup_Common[GetLang(), 2]; break;
			case "Cancel":			tooltip = HcFolderPopup_Common[GetLang(), 3]; break;
			case "Save":			tooltip = HcFolderPopup_Common[GetLang(), 4]; break;
			case "Undo":			tooltip = HcFolderPopup_Common[GetLang(), 5]; break;
			default: TooltipError(caption); break;
		}
		return GetGUIContent(caption, tooltip);
	}

	// FolderPopup Texture ===================================================================================
	protected static string[,]	HcFolderPopup_NcInfoCurve = {
								{
									 "- Overwrite on the chosen CurveInfo."
									,"- Save in the chosen category (addition)."
								}
								,
								{
									 "- 선택된 CurveInfo에 Overwrite 합니다."
									,"- 선택된 카테고리에 저장합니다.(추가)"
								}
							  };
	public static GUIContent GetHcFolderPopup_NcInfoCurve(string caption)
	{
		if (m_bTDisable)	return m_emptyContent;

		CheckValid(HcFolderPopup_NcInfoCurve);
		string	tooltip = "";
		
		switch (caption)
		{
			case "": break;
			case "Overwrite":		tooltip	= HcFolderPopup_NcInfoCurve[GetLang(), 0]; break;
			case "Save":			tooltip	= HcFolderPopup_NcInfoCurve[GetLang(), 1]; break;
			default: GetHcFolderPopup_Common(caption); break;
		}
		return GetGUIContent(caption, tooltip);
	}

	// FolderPopup Texture ===================================================================================
	protected static string[,]	HsFolderPopup_Texture = {
								{
									 "- Left-Click: Use as mainTexture." + "\n" +
									 "- Right-Click: Assign as MaskTexture if you use MaskShader. "
								}
								,
								{
									 "- 좌클릭: mainTexture로 사용 합니다." + "\n" +
									 "- 우클릭: MaskShader를 사용할 경우 MaskTexture로 지정 됩니다."
								}
							  };
	public static string GetHsFolderPopup_Texture(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsFolderPopup_Texture);
		switch (caption)
		{
			case "HOVER_DESC":	return HsFolderPopup_Texture[GetLang(), 0];
			default: return caption;
		}
	}

	public static GUIContent GetHcFolderPopup_Texture(string caption)
	{
		if (m_bTDisable)	return m_emptyContent;

		string keystr = "GetHcFolderPopup_Texture" + caption;
		if (m_DictionaryCon.ContainsKey(keystr))
			return m_DictionaryCon[keystr];

		string[,]	helpStr = {
								{
									 "- This is a Shader Material list composed of the same Texture in order to choose Shader quickly." + "\n" +
									 "- You can add/remove Shader in the following folder." + "\n" +
									 "- " + FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.DEFAULTSHADERMATERIALS) + "\n" +
									 "- If you want to subtract from the list, do not delete but move to the following folder." + "\n" +
									 "- " + FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.DEFAULTSHADERMATERIALS) + " Unlisted" + "\n" +
									 "- If Hierarchy/_FXMaker/ToolOption/m_bUpdateNewMaterial option is true, Update will occur everytime texture is selected."
									,"- This is a Material list which is already made. " + "\n" +
									 "- If you choose, you will share Material." + "\n" +
									 "- Though you choose 'New Shader Material,' you will share it if there is the same in 'Found Material'."
									,"- Deselect and restore the original Material."
									,"- Do not use existing Mateiral which is possible to share; Save as New Material."
									,"- Finish operations and close the window." + "\n" +
									 "- If there is the same Material with the one you set, you will share it." + "\n" +
									 "- If you want to cancel, click Undo first."
								}
								,
								{
									 "- Shader를 빠르게 선택할 수 있도록, 같은 Texture로 만들어진 Shader Material 목록 입니다." + "\n" +
									 "- 다음에 나오는 폴더에서 Shader를 추가/제거 할 수 있습니다." + "\n" +
									 "- " + FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.DEFAULTSHADERMATERIALS) + "\n" +
									 "- 목록에서 빼고 싶을 경우 삭제가 아닌 다음에 나오는 폴더로 이동 시키세요." + "\n" +
									 "- " + FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.DEFAULTSHADERMATERIALS) + " Unlisted" + "\n" +
									 "- Hierarchy/_FXMaker/ToolOption/m_bUpdateNewMaterial 옵션아 참이면, Texture선택할때마다 Update됩니다."
									,"- 이미 만들어진 Material 목록입니다." + "\n" +
									 "- 선택하시면 Material을 공유하게 됩니다." + "\n" +
									 "- 'New Shader Material'을 선택하더라도 'Found Material'에 같은 것이 존재할 경우 공유 하게 됩니다."
									,"- 선택을 취소하고 원래 Material을 복구 합니다."
									,"- 공유 가능한 기존 Material을 사용하지 않고 New Material로 저장합니다."
									,"- 작업을 끝내고 창을 닫습니다." + "\n" +
									 "- 설정한 Material과 같은 Material이 이미 존재할 경우 공유 하게 됩니다." + "\n" +
									 "- 취소를 원할 경우 Undo를 먼저 클릭하세요."
 								}
							  };
		CheckValid(helpStr);
		string	tooltip = "";
		
		switch (caption)
		{
			case "": break;
			case "New Shader Material":	tooltip	= helpStr[GetLang(), 0]; break;
			case "Found Material":		tooltip	= helpStr[GetLang(), 1]; break;
			case "Undo":				tooltip	= helpStr[GetLang(), 2]; break;
			case "UniqueSave":			tooltip	= helpStr[GetLang(), 3]; break;
			case "Close":		 		tooltip	= helpStr[GetLang(), 4]; break;
			default: GetHcFolderPopup_Common(caption); break;
		}
		m_DictionaryCon.Add(keystr, GetGUIContent(caption, tooltip));
		return m_DictionaryCon[keystr];
	}


	// Editor FXMakerOption ===================================================================================
	protected static string[,]	HsEditor_FXMakerOption = {
								{
									 "- If you modify in PlayMode, it will not be saved."
									,"- If Cursor is on the button, affected GUIWindow will be indicated with Red line."
									,"- Indicate Help Tooltip on Cursor."
									,"- Indicate Help Tooltip on the bottom of the screen."
									,"- Select a language you want to use in Help." + "\n" +
									 "- Help is in FXMakertip.cs file."
									,"- Assign the minimum number of 'Top Toolbar' in the top Toolbar."
									,"- Assign the minimum number of 'Bottom Toolbar' in the top Toolbar."
									,"- Adjust the width of Effect list."
									,"- Assign the fixed height of the top Toolbar."
									,"- If you delete Effect, it will move to [RecycleBin]." + "\n" +
									 "- Whenever you save or automatically saved, previous Effect will be backup in [RecycleBin]." + "\n" +
									 "- But, if SharedMaterial is changed in 'Unity Inspector,' previous status will not be backup."
									,"- Automatically save when Unity becomes Pause mode." + "\n" +
									 "- This happens when you stop due to a script error in Unity or activate other programs."
									,"- When Unity Play mode is changed into stop mode, operations will be automatically saved."
									,"- Adjust the value of DistPerTime which is used in Trans simulation."
									,"- This is the curve which will be used in parabolic simulation." + "\n" +
									 "- PrefabSimulate Window / Arc Button."
									,"- If you Drag&Drop Transform in PrefabHierarchy, use the value of World."
									,"- Assign BoundsBox Color." + "\n" +
									 "- This will be used when PrefabHierarchy/Box Option is turned on." + "\n" +
									 "- This function will be indicated when Gizmos is ON mode."
									,"- Assign Wireframe Color." + "\n" +
									 "- Use when PrefabHierarchy/Wire Option is turned on."
									,"- Assign BoundsBox Color." + "\n" +
									 "- This will be used when PrefabHierarchy/Box Option is turned on." + "\n" +
									 "- This function will be indicated when Gizmos is ON mode."
									,"- Assign Wireframe Color." + "\n" +
									 "- Use when PrefabHierarchy/Wire Option is turned on."

									,"- Decide location of AddPrefab Dialog."
									,"- Select function for Right-click at 'AddPrefab Dialog." + "\n" +
									 "- PingObject : Finds location of Prefab." + "\n" +
									 "- Append : Adds selected Prefab by clicking."
									,"- Set interval of Grid. (World coordinates)"
									,"- Set overal size of grid. (World coordinates)"
									,"- Decide whether to update 'New Shader Material' as selecting texture from MaterialPopup." + "\n" +
									 "- When Texture selectin speed is low, we suggest to leave it as 'false' state."
									,"- Reset the timeScale when the tool is shut down."
									,"- The weights of the alpha channel. (This is used in BuildSprite)" + "\n" +
									 "- If the left value is low, Removed the black." + "\n" +
									 "- If the left value is low, Removed the white." + "\n" +
									 "- The document location (IGSoft_Tools/Readme/FX Maker 1.2 Update)"

									,"- This value is a movement unit of mouse"
									,"- This value is a movement unit of the arrow keys."
									,"- To determine how to move the arrow keys."

									,"- FXMaker.Camera(Upper right) - Axis-X 0"
									,"- FXMaker.Camera(Upper right) - Axis-X 1"
									,"- FXMaker.Camera(Upper right) - Axis-X 2"
									,"- FXMaker.Camera(Upper right) - Axis-X 3"
									,"- FXMaker.Camera(Upper right) - Axis-Y 0"
									,"- FXMaker.Camera(Upper right) - Axis-Y 1"
									,"- FXMaker.Camera(Upper right) - Axis-Y 2"
									,"- FXMaker.Camera(Upper right) - Axis-Y 3"
								}
								,
								{
									 "- PlayMode에서 수정할 경우 저장 되지 않습니다."
									,"- 버튼에 Cursor가 올려질 경우, 영향 받는 GUIWindow를 Red선으로 표시 합니다."
									,"- 도움말 Tooltip을 Cursor에 표시 합니다."
									,"- 도움말 Tooltip을 화면 하단에 표시 합니다."
									,"- 도움말 언어를 선택 합니다." + "\n" +
									 "- 도움말은 FXMakertip.cs 파일에 있습니다"
									,"- 상단 Toolbar에서 'Top Toolbar'의 최소 수를 지정 합니다."
									,"- 상단 Toolbar에서 'Bottom Toolbar'의 최소 수를 지정 합니다."
									,"- Effect목록의 가로 폭을 조절 합니다."
									,"- 상단 Toolbar의 고정 높이를 지정 합니다."
									,"- Effect를 삭제할 경우 [RecycleBin] 으로 이동합니다." + "\n" +
									 "- 저장 및 자동저장 할 때마다 이전 Effect는 [RecycleBin]에 백업 합니다." + "\n" +
									 "- 단, ‘Unity Inspector’에서 SharedMaterial을 변경할 경우 이전 상태가 백업 되지 않습니다."
									,"- Unity가 Pause상태로 될 경우 자동 저장 합니다." + "\n" +
									 "- Unity에서 script 오류로 멈추거나 다른 프로그램을 활성화 할 경우 발생 합니다."
									,"- Unity Play 상태에서 stop 상태로 변경될 때 작업을 자동 저장 합니다."
									,"- Trans 시뮬레이션을 할 때 사용되는 DistPerTime 값의 크기를 조절 합니다."
									,"- 포물선 시뮬레이션에 사용될 커브 입니다." + "\n" +
									 "- PrefabSimulate Window / Arc Button."
									,"- PrefabHierarchy에서 Transform을 Drag&Drop할 경우, World 값을 사용합니다."
									,"- BoundsBox Color를 지정합니다." + "\n" +
									 "- PrefabHierarchy/Box Option이 켜지면 사용됩니다." + "\n" +
									 "- 이 기능은 Gizmos가 ON 상태일 때만 표시 됩니다."
									,"- Wireframe Color를 지정합니다." + "\n" +
									 "- PrefabHierarchy/Wire Option이 켜지면 사용됩니다."
									,"- BoundsBox Color를 지정합니다." + "\n" +
									 "- PrefabHierarchy/Box Option이 켜지면 사용됩니다." + "\n" +
									 "- 이 기능은 Gizmos가 ON 상태일 때만 표시 됩니다."
									,"- Wireframe Color를 지정합니다." + "\n" +
									 "- PrefabHierarchy/Wire Option이 켜지면 사용됩니다."

									,"- AddPrefab Dialog의 위치를 결정 합니다."
									,"- 'AddPrefab Dialog'에서 Right-Click에 대한 기능을 선택 합니다." + "\n" +
									 "- PingObject : Prefab의 위치를 찾아 줍니다." + "\n" +
									 "- Append : 클릭할 때마다 선택된 Prefab이 추가 됩니다."
									,"- Grid 간격을 설정합니다. (월드좌표)"
									,"- Grid의 전체 크기를 설정합니다. (월드좌표)"
									,"- MaterialPopup에서 Texture를 선택할때마다 'New Shader Material'을 Update할건지 결정합니다." + "\n" +
									 "- Texture선택 속도가 느릴 경우 'false' 상태로 두길 권장합니다."
									,"- 알파채널의 가중값, 이것은 BuildSprite에서 사용된다."
									,"- 알파채널 가중값을 나타내는 커브입니다." + "\n" +
									 "- 왼쪽 값이 낮으면, 블랙이 제거된다." + "\n" +
									 "- 왼쪽 값이 낮으면, 화이트가 제거된다." + "\n" +
									 "- 참고 문서 위치 (IGSoft_Tools/Readme/FX Maker 1.2 Update)"

									,"- 선택된 GameObject를 마우스로 이동하였을 때 이동될 단위 입니다."
									,"- 선택된 GameObject를 방향키로 이동하였을 때 이동 방식을 결정합니다."
									,"- 선택된 GameObject를 방향키로 이동하였을 때 이동될 단위 입니다."

									,"- FXMaker의 우측 상단에 위치한 카메라 퀵전환 설정값 입니다. x축 0번째"
									,"- FXMaker의 우측 상단에 위치한 카메라 퀵전환 설정값 입니다. x축 1번째"
									,"- FXMaker의 우측 상단에 위치한 카메라 퀵전환 설정값 입니다. x축 2번째"
									,"- FXMaker의 우측 상단에 위치한 카메라 퀵전환 설정값 입니다. x축 3번째"
									,"- FXMaker의 우측 상단에 위치한 카메라 퀵전환 설정값 입니다. y축 0번째"
									,"- FXMaker의 우측 상단에 위치한 카메라 퀵전환 설정값 입니다. y축 1번째"
									,"- FXMaker의 우측 상단에 위치한 카메라 퀵전환 설정값 입니다. y축 2번째"
									,"- FXMaker의 우측 상단에 위치한 카메라 퀵전환 설정값 입니다. y축 3번째"
								}
							  };
	public static string GetHsEditor_FXMakerOption(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_FXMakerOption);
		string	tooltip = "";

		switch (caption)
		{
			case "m_bHintRedBox":				tooltip	= HsEditor_FXMakerOption[GetLang(), 1];		break;
			case "m_bShowCursorTooltip":		tooltip	= HsEditor_FXMakerOption[GetLang(), 2];		break;
			case "m_bShowBottomTooltip":		tooltip	= HsEditor_FXMakerOption[GetLang(), 3];		break;
			case "m_LanguageType":				tooltip	= HsEditor_FXMakerOption[GetLang(), 4];		break;
			case "m_nMinTopToolbarCount":		tooltip	= HsEditor_FXMakerOption[GetLang(), 5];		break;
			case "m_nMinBottomToolbarCount":	tooltip	= HsEditor_FXMakerOption[GetLang(), 6];		break;
			case "m_fFixedWindowWidth":			tooltip	= HsEditor_FXMakerOption[GetLang(), 7];		break;
			case "m_fTopMenuHeight":			tooltip	= HsEditor_FXMakerOption[GetLang(), 8];		break;
			case "m_bActiveRecycleBin":			tooltip	= HsEditor_FXMakerOption[GetLang(), 9];		break;
			case "m_bAutoSaveAppPause":			tooltip	= HsEditor_FXMakerOption[GetLang(), 10];	break;
			case "m_bAutoSaveAppQuit":			tooltip	= HsEditor_FXMakerOption[GetLang(), 11];	break;
			case "m_fScaleTransSpeed":			tooltip	= HsEditor_FXMakerOption[GetLang(), 12];	break;
			case "m_SimulateArcCurve":			tooltip	= HsEditor_FXMakerOption[GetLang(), 13];	break;
			case "m_bDragDrop_WorldSpace":		tooltip	= HsEditor_FXMakerOption[GetLang(), 14];	break;
			case "m_ColorRootBoundsBox":		tooltip	= HsEditor_FXMakerOption[GetLang(), 15];	break;
			case "m_ColorChildBoundsBox":		tooltip	= HsEditor_FXMakerOption[GetLang(), 16];	break;
			case "m_ColorRootWireframe":		tooltip	= HsEditor_FXMakerOption[GetLang(), 17];	break;
			case "m_ColorChildWireframe":		tooltip	= HsEditor_FXMakerOption[GetLang(), 18];	break;

			case "m_AddPrefabPosition":			tooltip	= HsEditor_FXMakerOption[GetLang(), 19];	break;
			case "m_PrefabDlg_RightClick":		tooltip	= HsEditor_FXMakerOption[GetLang(), 20];	break;

			case "m_GridUnit":					tooltip	= HsEditor_FXMakerOption[GetLang(), 21];	break;
			case "m_GridSize":					tooltip	= HsEditor_FXMakerOption[GetLang(), 22];	break;
			case "m_bUpdateNewMaterial":		tooltip	= HsEditor_FXMakerOption[GetLang(), 23];	break;

			case "m_bResetTimeScaleAppQuit":	tooltip	= HsEditor_FXMakerOption[GetLang(), 24];	break;
			case "m_AlphaWeightCurve":			tooltip	= HsEditor_FXMakerOption[GetLang(), 25];	break;

			case "m_fGizmoGridMoveUnit":		tooltip	= HsEditor_FXMakerOption[GetLang(), 26];	break;
			case "m_ArrowGridMoveType":			tooltip	= HsEditor_FXMakerOption[GetLang(), 27];	break;
			case "m_fArrowGridMoveUnit":		tooltip	= HsEditor_FXMakerOption[GetLang(), 28];	break;

			case "m_nCameraAangleXValues0":		tooltip	= HsEditor_FXMakerOption[GetLang(), 29];	break;
			case "m_nCameraAangleXValues1":		tooltip	= HsEditor_FXMakerOption[GetLang(), 30];	break;
			case "m_nCameraAangleXValues2":		tooltip	= HsEditor_FXMakerOption[GetLang(), 31];	break;
			case "m_nCameraAangleXValues3":		tooltip	= HsEditor_FXMakerOption[GetLang(), 32];	break;
			case "m_nCameraAangleYValues0":		tooltip	= HsEditor_FXMakerOption[GetLang(), 33];	break;
			case "m_nCameraAangleYValues1":		tooltip	= HsEditor_FXMakerOption[GetLang(), 34];	break;
			case "m_nCameraAangleYValues2":		tooltip	= HsEditor_FXMakerOption[GetLang(), 35];	break;
			case "m_nCameraAangleYValues3":		tooltip	= HsEditor_FXMakerOption[GetLang(), 36];	break;

			default: return "FXMakerOption.cs" + "\n" + HsEditor_FXMakerOption[GetLang(), 0];

		}
		return caption + "\n" + tooltip;
	}

	// NcScript Editor Inspector =========================================================================================================================================
	// Editor NcCurveAnimation ===================================================================================
	protected static string[,]	HsEditor_NcCurveAnimation = {
								{
									 "- By using Curve, process animation (Position, Rotation, Scale, Color, TextureUV)." + "\n" +
									 "- The width of Curve is the time that indicates m_fDurationTimer with the number 0~1." + "\n" +
									 "- The length of Curve is an adjustable Value." + "\n" +
									 "- Adjust The Value of the length with 'Value Scale'." + "\n" +
									 "- You can use a number of the same kind of CurveInfo." + "\n" +
									 "- You can save or load CurveInfo; you can also save or load all CurveInfo at a time."
									,"- This is the delay time of animation." + "\n" +
									 "- This is the time elapsed after GameObject.Active becomes True."
									,"- This is the operating time of animation (Widthwise direction of Curve = 1.0f)"
									,"- After m_fDurationTime elapses, Destroy GameObject."

									,"- Delete all CurveInfo."
									,"- Load CurveInfo saved in Project."
									,"- Save all CurveInfo in Project."
									,"- Add new CurveInfo."

									,"- There should be NcUvAnimation.cs in order to use TextureUV; if you click, it will be added automatically."
									,"- Change ToColor to Whilte."
									,"- Load CurrentColor in Material and save in ToColor."
									,"- If the Value of Curve is equal to 0, there is no change." + "\n" +
									 "- If the Value of Curve is equal to 1, CurrentColor+(ToColor-CurrentColor)" + "\n" +
									 "- If the Value of Curve is equal to -1, CurrentColor-(ToColor-CurrentColor)"

									,"- Change MaterialColors of all ChildgameObject." + "\n" +
									 "- Must be created before Animation occurs." + "\n" +
									 "- Does not apply in case runtime is created like NcDuplicator or NcAttachPrefab."
									,"- This is the value which will be multiplied by the height of Curve."

									,"- Load CurveInfo which is assigned beforehand."
									,"- Save current CurveInfo."
									,"- Delete CurveInfo."
								}
								,
								{
									 "- Curve를 사용하여 애니메이션(Position, Rotation, Scale, Color, TextureUV) 처리 합니다." + "\n" +
									 "- Curve의 가로는 m_fDurationTimer값을 0~1로 표현한 시간입니다." + "\n" +
									 "- Curve의 세로는 가감되는 Value입니다." + "\n" +
									 "- 세로 Value크기는 'Value Scale'로 조절 합니다." + "\n" +
									 "- 같은 종류의 CurveInfo를 여러 개 사용할 수 있습니다." + "\n" +
									 "- CurveInfo를 저장 또는 로드할 수 있으며, 한번에 모든 CurveInfo를 저장 또는 로드할 수 잇습니다."
									,"- 애니메이션 지연 시간 입니다." + "\n" +
									 "- GameObject.Active가 True로 된 후 경과될 시간 입니다."
									,"- 애니메이션 동작 시간 입니다.(Curve의 가로 방향 = 1.0f)"
									,"- m_fDurationTime이 경과하면 GameObject를 소멸 시킵니다."

									,"- 모든 CurveInfo를 삭제 합니다."
									,"- Project에 저장되어 있는 CurveInfo를 로드 하여 추가 합니다."
									,"- 모든 CurveInfo를 Project에 저장 합니다."
									,"- 새로운 CurveInfo를 추가 합니다."

									,"- TextureUV를 사용할 경우 NcUvAnimation.cs가 있어야 합니다, 클릭하시면 자동추가 됩니다."
									,"- ToColor를 Whilte로 변경합니다."
									,"- Material의 CurrentColor를 가져와서 ToColor에 저장 합니다."
									,"- Curve의 Value가 0이면 변화 없음" + "\n" +
									 "- Curve의 Value가 1이면 CurrentColor+(ToColor-CurrentColor)" + "\n" +
									 "- Curve의 Value가 Value가 -1이면 CurrentColor-(ToColor-CurrentColor)"

									,"- 모든 ChildGameObject의 MaterialColor를 변경 합니다." + "\n" +
									 "- 단, 애니메이션 시작전에 이미 생성되어 있어야 합니다." + "\n" +
									 "- NcDuplicator, NcAttachPrefab과 같이 Runtime생성되는 GameObject에는 적용되지 않습니다."
									,"- Curve의 세로 값에 곱해지는 값입니다."

									,"- 미리 저정된 CurveInfo를 로드 합니다."
									,"- 현재의 CurveInfo를 저장 합니다."
									,"- CurveInfo를 삭제 합니다."
								}
							  };
	public static string GetHsEditor_NcCurveAnimation(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcCurveAnimation);
		string	tooltip = "";

		switch (caption)
		{
			case "m_fDelayTime":			tooltip	= HsEditor_NcCurveAnimation[GetLang(), 1];	break;
			case "m_fDurationTime":			tooltip	= HsEditor_NcCurveAnimation[GetLang(), 2];	break;
			case "m_bAutoDestruct":			tooltip	= HsEditor_NcCurveAnimation[GetLang(), 3];	break;

			case "Clear All":				tooltip	= HsEditor_NcCurveAnimation[GetLang(), 4];	break;
			case "Load Curve":				tooltip	= HsEditor_NcCurveAnimation[GetLang(), 5];	break;
			case "Save Curve":				tooltip	= HsEditor_NcCurveAnimation[GetLang(), 6];	break;
			case "Add EmptyCurve":			tooltip	= HsEditor_NcCurveAnimation[GetLang(), 7];	break;

			case "Add NcUvAnimation Script":tooltip	= HsEditor_NcCurveAnimation[GetLang(), 8];	break;
			case "White":					tooltip	= HsEditor_NcCurveAnimation[GetLang(), 9];	break;
			case "Current":					tooltip	= HsEditor_NcCurveAnimation[GetLang(), 10];	break;
			case "ToColor":					tooltip	= HsEditor_NcCurveAnimation[GetLang(), 11];	break;
			case "Recursively":				tooltip	= HsEditor_NcCurveAnimation[GetLang(), 12];	break;
			case "Value Scale":				tooltip	= HsEditor_NcCurveAnimation[GetLang(), 13];	break;

			case "Change":					tooltip	= HsEditor_NcCurveAnimation[GetLang(), 14];	break;
			case "Save":					tooltip	= HsEditor_NcCurveAnimation[GetLang(), 15];	break;
			case "Delete":					tooltip	= HsEditor_NcCurveAnimation[GetLang(), 16];	break;
				
			default: return "NcCurveAnimation\n" + HsEditor_NcCurveAnimation[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcSpriteAnimation ===================================================================================
	protected static string[,]	HsEditor_NcSpriteAnimation = {
								{
									 "- Process Material.Texture with SpriteAnimation." + "\n" +
									 "- When using with NcSpriteFactory, several animation can be selectively applied." + "\n" +
									 "- Because it is Material which became Instance, the operation will not affect other Material."

									,"- Decide the output method of Sprite." + "\n" +
									 "- DEFALUT: Sequential play" + "\n" +
									 "- INVERSE: Inverse order play" + "\n" +
									 "- PINGPONG: PingPong play" + "\n" +
									 "- RANDOM: Randomly shows one from the relevant range." + "\n" +
									 "- SELECT: Selects frame to be shown from the relevant range."
									,"- Designate play delay."
									,"- Initial FrameIndex"
									,"- Designate when actual number of frame is less than that of available(TileX * TileY) frame."
									,"- Select frame to be shown from the relevant range."
									,"- Designate repetition of play."
									,"- GameObject will be destroy after Animation ends." + "\n" +
									 "- Endless loop when False."
									,"- Decide speed according to ouput frame per second."
									,"- Number of colums to divide Material.Texture"
									,"- Number of rows to divide Material.Texture"
									,"- Read-only value." + "\n" +
									 "- Notify time spended for single play loop. (m_nFramecount / m_fFps)" + "\n" +
									 "- Identical time has to be saved at NcCurveAnimation.Durationtime when using FadeIn/FadeOut."

									,"- TileTexture : Using evenly divided texture." + "\n" +
									 "- TrimTexture : Using the atlas texture.(will convert to the atlas texture When you click the 'ConvertTo : TrimTexture' button)" + "\n" +
									 "- SpriteFactory : Using shared prefab."
									,"- BuiltIn_Plane : Generate Single-sided mesh" + "\n" +
									 "- BuiltIn_TwosidePlane : Generate Two-sided mesh."
									,"- Determine the location of the pivot made ​​of mesh."
									,"-  "	// m_NcSpriteFactoryPrefab
									,"- The weights of the alpha channel. (This is used in BuildSprite)" + "\n" +
									 "- If the left value is low, Removed the black." + "\n" +
									 "- If the left value is low, Removed the white." + "\n" +
									 "- The document location (IGSoft_Tools/Readme/FX Maker 1.2 Update)"
								}
								,
								{
									 "- Material.Texture를 SpriteAnimation 처리합니다." + "\n" +
									 "- NcSpriteFactory를 같이 이용할 경우, 어러개의 애니메이션을 선택적으로 적용할 수 있습니다." + "\n" +
									 "- Instance된 Material이므로 다른 Material에 영향을 주지 않는다."

									,"- Sprite 출력 방식을 결정 합니다." + "\n" +
									 "- DEFAULT: 순차 재생" + "\n" +
									 "- INVERSE: 역순 재생" + "\n" +
									 "- PINGPONG: PingPong 재생" + "\n" +
									 "- RANDOM: 범위내에서 랜덤으로 하나만 보여 줍니다." + "\n" +
									 "- SELECT: 범위내에서 보여줄 Frame을 선택 합니다."
									,"- Play 지연시간을 지정 합니다."
									,"- 시작할 FrameIndex"
									,"- 실제 Frame 수가 가능한(TileX * TileY) Frame 수 보다 적을 경우 지정 합니다."
									,"- 범위내에서 보여줄 Frame을 선택 합니다."
									,"- 반복 플레이를 결정합니다."
									,"- Animation이 끝나면, GameObject를 소멸 시킵니다." + "\n" +
									 "- false면 무한반복 합니다."
									,"- 초당 출력 프레임으로 속도를 결정 합니다."
									,"- Material.Texture를 가로로 나눌 수"
									,"- Material.Texture를 세로로 나눌 수"
									,"- 읽기 전용 값입니다." + "\n" +
									 "- 한번 Play에 소요되는 시간을 알려줍니다. (m_nFrameCount / m_fFps)" + "\n" +
									 "- FadeIn/FadeOut을 사용할 경우 NcCurveAnimation.Durationtime에 같은 Time이 저장되어 있어야 합니다."

									,"- TileTexture : 가로 세로 균등하게 나누어진 텍스쳐 사용" + "\n" +
									 "- TrimTexture : TileTexture를 메모리 절약형 atlas텍스쳐 사용(ConvertTo 클릭)" + "\n" +
									 "- SpriteFactory : 별도 Prefab에 있는 NcSpriteFactory를 사용하여 atlas텍스쳐를 공유한다"
									,"- BuiltIn_Plane : 단면 플랜 사용" + "\n" +
									 "- BuiltIn_TwosidePlane : 양면 플랜 사용"
									,"- 만들어진 플랜에 대해서 피봇의 위치를 결정한다."
									,"-  "	// m_NcSpriteFactoryPrefab
									,"- 알파채널 가중값을 나타내는 커브입니다." + "\n" +
									 "- 왼쪽 값이 낮으면, 블랙이 제거된다." + "\n" +
									 "- 왼쪽 값이 낮으면, 화이트가 제거된다." + "\n" +
									 "- 참고 문서 위치 (IGSoft_Tools/Readme/FX Maker 1.2 Update)"
								}
							  };
	public static string GetHsEditor_NcSpriteAnimation(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcSpriteAnimation);
		string	tooltip = "";

		switch (caption)
		{
			case "m_PlayMode":				tooltip	= HsEditor_NcSpriteAnimation[GetLang(), 1];		break;
			case "m_fDelayTime":			tooltip	= HsEditor_NcSpriteAnimation[GetLang(), 2];		break;
			case "m_nStartFrame":			tooltip	= HsEditor_NcSpriteAnimation[GetLang(), 3];		break;
			case "m_nFrameCount":			tooltip	= HsEditor_NcSpriteAnimation[GetLang(), 4];		break;
			case "m_nSelectFrame":			tooltip	= HsEditor_NcSpriteAnimation[GetLang(), 5];		break;
			case "m_bLoop":					tooltip	= HsEditor_NcSpriteAnimation[GetLang(), 6];		break;
			case "m_bAutoDestruct":			tooltip	= HsEditor_NcSpriteAnimation[GetLang(), 7];		break;
			case "m_fFps":					tooltip	= HsEditor_NcSpriteAnimation[GetLang(), 8];		break;
			case "m_nTilingX":				tooltip	= HsEditor_NcSpriteAnimation[GetLang(), 9];		break;
			case "m_nTilingY":				tooltip	= HsEditor_NcSpriteAnimation[GetLang(),10];		break;
			case "DurationTime":			tooltip	= HsEditor_NcSpriteAnimation[GetLang(),11];		break;

			case "m_TextureType":			tooltip	= HsEditor_NcSpriteAnimation[GetLang(),12];		break;
			case "m_MeshType":				tooltip	= HsEditor_NcSpriteAnimation[GetLang(),13];		break;
			case "m_AlignType":				tooltip	= HsEditor_NcSpriteAnimation[GetLang(),14];		break;
			case "m_NcSpriteFactoryPrefab":	tooltip	= HsEditor_NcSpriteAnimation[GetLang(),15];		break;
			case "m_curveAlphaWeight":		tooltip	= HsEditor_NcSpriteAnimation[GetLang(),16];		break;

			default: return "NcSpriteAnimation\n" + HsEditor_NcSpriteAnimation[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcAddForce ===================================================================================
	protected static string[,]	HsEditor_NcAddForce = {
								{
									 "- Add physical value at the first rendering." + "\n" +
									 "- RigidBody must be already included."
									,"- Forced value to be applied as AddForce()."
									,"- Random range to be added to m_Addforce value."
									,"- ForceMode"
									,"- Add RigidBody Component"
								}
								,
								{
									 "- 첫 렌더링할 때 물리값을 추가합니다." + "\n" +
									 "- RigidBody가 미리 포합되어 있어야 합니다."
									,"- AddForce()로 적용될 Force 값입니다."
									,"- m_AddForce값에 더해질 랜덤범위 입니다."
									,"- ForceMode"
									,"- Add RigidBody Component"
								}
							  };
	public static string GetHsEditor_NcAddForce(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcAddForce);
		string	tooltip = "";

		switch (caption)
		{
			case "m_AddForce":				tooltip	= HsEditor_NcAddForce[GetLang(), 1];		break;
			case "m_RandomRange":			tooltip	= HsEditor_NcAddForce[GetLang(), 2];		break;
			case "m_ForceMode":				tooltip	= HsEditor_NcAddForce[GetLang(), 3];		break;

			case "Add RigidBody Component":	tooltip	= HsEditor_NcAddForce[GetLang(), 4];		break;

			default: return "NcAddForce\n" + HsEditor_NcAddForce[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcAttachPrefab ===================================================================================
	protected static string[,]	HsEditor_NcAttachPrefab = {
								{
									 "- Assign Prefab which is already made and do Runtime load and Rendering." + "\n" +
									 "- When m_AttachPrefab is modified, current Effect is also applied due to reference method."
									,""
									,"- Assign Delaytime. " + "\n" +
									 "- This is the time elapsed after GameObject.Active becomes True."
									,"- Input the repetition period. Repeatedly creates by the period designated according to m_fReapeatCount."
									,"- If larger than 0, limits the number of creation." + "\n" +
									 "- If 0, proceed repetition unlimitedly. (Only when m_fRepeatTime is larger than 0.)"
									,"- Choose EffectPrefab which you will paste."
									,"- Use WorldSpace."
									,"- Cumulatively substitute the value of rotation to GameObject being created repeatedly." + "\n" +
									 "- Obj0 = 0*rot, Obj1 = 1*rot, Obj2 = 2*rot"
									,"- Assign the range of 'Random Position' of GameObject being created repeatedly."

									,"- Adjust Speed of m_AttachPrefab to be created." + "\n" +
									 "- Warning of adjusting vleocity" + "\n" +
									 "- Animation : Speed of Unity animation is not adjustable.(Support is scheduled)" + "\n" +
									 "-Particle emitter: Adjustment of Damping and Velocity of LegacyParticle is required." + "\n" +
									 "- ShurikenParticleSystem : Not Support RuntimeSpeed"

									,"- Adjust auto-destruction period of m_AttachPrefab to be created." + "\n" +
									 "- If auto-destruction period(NcAutoDestruct) is designated at m_Attach Prefab to be created, the period resets to newly designated time."
									,"- Modify location of repeatedly created Gameobject."

									,"- Choose Prefab which is already saved."
									,"- Clear the chosen Prefab."
									,"- Open the chosen Prefab."
								}
								,
								{
									 "- 이미 만들어진 Prefab을 지정하여 Runtime 로드 및 Rendering을 합니다." + "\n" +
									 "- 레퍼런스 방식이므로 m_AttachPrefab이 수정될 경우 현재의 이펙트에도 적용 됩니다."
									,""
									,"- Delaytime을 지정 합니다." + "\n" +
									 "- GameObject.Active가 True로 된 후 경과될 시간 입니다."
									,"- 반복 시간을 입력합니다. 지정 간격으로 m_fRepeatCount 수만큼 반복 생성 합니다."
									,"- 0보다 큰 값이면, 생성할 수를 제한합니다." + "\n" +
									 "- 0이면 무한 반복 합니다.(단, m_fRepeatTime이 0보다 커야 합니다)."

									,"- 붙일 EffectPrefab를 선택 합니다."
									,"- WorldSpace를 사용 합니다."
									,"- 반복 생성되는 GameObject에 회전 값을 누적 대입 한다." + "\n" +
									 "- Obj0 = 0*rot, Obj1 = 1*rot, Obj2 = 2*rot"
									,"- 반복 생성되는 GameObject의 'Random Position' 범위를 지정한다."


									,"- 생성될 m_AttachPrefab의 속도를 Runtime 조절 합니다." + "\n" +
									 "- 속도조절 주의사항" + "\n" + 
									 "- Aanimation : Unity animation은 속도조절이 안됩니다.(지원예정)" + "\n" + 
									 "- particleEmitter : LegacyParticle은 Damping 및 Velocity조절이 필요 합니다." + "\n" + 
									 "- ShurikenParticleSystem : Runtime 속도조절을 지원하지 않습니다."

									,"- 생성될 m_AttachPrefab의 자동소멸 시간을 설정합니다." + "\n" +
									 "- 생성될 m_AttachPrefab에 자동소멸(NcAutoDestruct) 시간이 설정되어 있다면, 새롭게 설정한 시간으로 변경됩니다."
									,"- 반복 생성되는 GameObject의 위치를 조정 합니다."

									,"- 이미저 저장되어 있는 Prefab을 선택 합니다."
									,"- 선택된 Prefab을 Clear 합니다."
									,"- 선택된 Prefab을 Open 합니다."
								}
							  };
	public static string GetHsEditor_NcAttachPrefab(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcAttachPrefab);
		string	tooltip = "";

		switch (caption)
		{
			case "m_AttachType":			tooltip	= HsEditor_NcAttachPrefab[GetLang(), 1];		break;
			case "m_fDelayTime":			tooltip	= HsEditor_NcAttachPrefab[GetLang(), 2];		break;
			case "m_fRepeatTime":			tooltip	= HsEditor_NcAttachPrefab[GetLang(), 3];		break;
			case "m_nRepeatCount":			tooltip	= HsEditor_NcAttachPrefab[GetLang(), 4];		break;

			case "m_AttachPrefab":			tooltip	= HsEditor_NcAttachPrefab[GetLang(), 5];		break;
			case "m_bWorldSpace":			tooltip	= HsEditor_NcAttachPrefab[GetLang(), 6];		break;
			case "m_AccumStartRot":			tooltip	= HsEditor_NcAttachPrefab[GetLang(), 7];		break;
			case "m_RandomRange":			tooltip	= HsEditor_NcAttachPrefab[GetLang(), 8];		break;

			case "m_fPrefabSpeed":			tooltip	= HsEditor_NcAttachPrefab[GetLang(), 9];		break;
			case "m_fPrefabLifeTime":		tooltip	= HsEditor_NcAttachPrefab[GetLang(),10];		break;
			case "m_AddStartPos":			tooltip	= HsEditor_NcAttachPrefab[GetLang(),11];		break;

			case "Select Prefab":			tooltip	= HsEditor_NcAttachPrefab[GetLang(),12];		break;
			case "Clear Prefab":			tooltip	= HsEditor_NcAttachPrefab[GetLang(),13];		break;
			case "Open Prefab":				tooltip	= HsEditor_NcAttachPrefab[GetLang(),14];		break;

			default: return "NcAttachPrefab\n" + HsEditor_NcAttachPrefab[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcAttachSound ===================================================================================
	protected static string[,]	HsEditor_NcAttachSound = {
								{
									 "- Play the sound file." + "\n" +
									 "- Functions: Delaying Play, Repeating Play, Volume Control, AudioSource Option"
									,"- Assign the delay time of Playing AudioClip. " + "\n" +
									 "- This is the time which will be elapsed after GameObject.Active becomes True."
									,"- Input the repetition time; Play AutoClip at assigned intervals." + "\n" +
									 "- If the value is 0, Play once"
									,"- Input the number of repetition; Play as many times as the assigned number." + "\n" +
									 "- If the value is 0, play infintedly; the operation will be only applied when the value of m_fRepeatTime is greater than 0."
									,"- Assign the sound file which will be used."
								}
								,
								{
									 "- 사운드파일을 Play 한다." + "\n" +
									 "- 기능: 지연Play, 반복Play, 볼륨조절, AudioSource 옵션"
									,"- AudioClip을 Play 지연시간을 지정 합니다." + "\n" +
									 "- GameObject.Active가 True로 된 후 경과될 시간 입니다."
									,"- 반복 시간을 입력합니다. 지정 간격으로 AudioClip를 Play 합니다." + "\n" +
									 "- 0이면 한번만 Play"
									,"- 반복 수, 지정 수 만큼만 Play 됩니다." + "\n" +
									 "- 0이면 무한 반복, m_fRepeatTime가 0보다 클때만 적용 됩니다."
									,"- 사용될 사운드파일을 지정 합니다."
								}
							  };
	public static string GetHsEditor_NcAttachSound(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcAttachSound);
		string	tooltip = "";

		switch (caption)
		{
			case "m_fDelayTime":		tooltip	= HsEditor_NcAttachSound[GetLang(), 1];		break;
			case "m_fRepeatTime":		tooltip	= HsEditor_NcAttachSound[GetLang(), 2];		break;
			case "m_nRepeatCount":		tooltip	= HsEditor_NcAttachSound[GetLang(), 3];		break;
			case "m_AudioClip":			tooltip	= HsEditor_NcAttachSound[GetLang(), 4];		break;
			case "m_nPriority":			tooltip	= "- AudioSource.priority";		break;
			case "m_bLoop":				tooltip	= "- AudioSource.loop";			break;
			case "m_fVolume":			tooltip	= "- AudioSource.volume";		break;
			case "m_fPitch":			tooltip	= "- AudioSource.pitch";		break;
			default: return "NcAttachSound\n" + HsEditor_NcAttachSound[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcAutoDeactive ===================================================================================
	protected static string[,]	HsEditor_NcAutoDeactive = {
								{
									 "Automatically remove gameObject." + "\n" +
									 "-Automatically Deactive after time passes by designating LifeTime." + "\n" +
									 "- By setting SmoothDestrotTime, FadeOut can be applied while disappearing.(For only materials that can be Alpha proccessed)" + "\n" +
									 "- By setting CollisionTime, automatically Deactive even LifeTime remains."
									,"- If DestroyTime elapses after it becomes Active, remove GameObject." + "\n" +
									 "- Process only when the value is greater than 0."
									,"- After animation of NcCurveAnimation finishes, remove GameObject."

									,"- Deisgnate Deactive processing time." + "\n" +
									 "- During this time, Particle can be paused and disappear slowly." + "\n" +
									 "- In case ParentGameObject Deactive first, Particle Deactive immediately."
									,"- True: Pauses 'Emission of ParticleSystem' during Deactive." + "\n" +
									 "- Does not create any more Particle."
									,"- True: Reduces Alpha value of Material during Deactive.(Only to Material which contains Alpha.)"

									,"- Create additional Deactive condition besides Deactive method." + "\n" +
									 "- If other Effect after Deactive is wanted, use NcAttachPrefab.Destroy."
									,"- Select Layer to be used as collision object."
									,"- Radius to be used for collision test."
									,"- Deactive occurs if location of y from Particle is less than designated value."

									,"- Apply the MeshFilter only SmoothHide." + "\n" +
									 "- True: Can be used in MobileShader.(DrawCalls does not increase)" + "\n" +
									 "- False: Can be used in a shader that contains color."
								}
								,
								{
									 "gameObject를 자동으로 Deactive(sub포함) 처리한다." + "\n" +
									 "- LifeTime을 지정하여 시간이 경과하면 자동 Deactive 처리합니다." + "\n" +
									 "- SmoothDestrotTime을 설정하면, 사라지는 동안 FadeOut 처리가 가능하다.(Alpha처리가 가능한 Material만 사용 가능)" + "\n" +
									 "- CollisionType을 설정하면, LifeTime이 경과하지 않더라도 Deactive 처리 됩니다."
									,"- Active된 후 LifeTime이 경과하면 GameObject를 Deactive 시킨다." + "\n" +
									 "- 0 보다 큰 값일 때만 처리한다."
									,"- NcCurveAnimation의 애니메이션이 끝나면 GameObject를 Deactive 시킨다."

									,"- Deactive 처리 시간을 지정한다." + "\n" +
									 "- 이 시간 동안 Particle을 정지 시키고, 천천히 사라지게 할 수 있다." + "\n" +
									 "- 단, ParentGameObject가 먼저 Deactive할 경우 즉시 Deactive 처리된다."
									,"- true: Deactive되는 시간동안 'ParticleSystem의 Emission'을 정지 시킨다." + "\n" +
									 "- 더 이상 파티클은 생성되지 않는다."
									,"- true: Deactive되는 시간동안 Material의 alpha값을 감소 시킨다."

									,"- Deactive시간 방식과는 별개로 추가적인 Deactive 조건을 설정 합니다." + "\n" +
									 "- 충돌 후 다른 Effect를 표시하고 싶으면, NcAttachPrefab.Destroy를 사용하세요."
									,"- 충돌 대상이 되는 Layer를 선택 합니다."
									,"- 충돌 검사에 사용할 반지름 입니다."
									,"- Particle의 y 위치가 지정한 값보다 작을 경우 Deactive 합니다."

									,"- MeshFilter 만 SmoothHide을 적용합니다." + "\n" +
									 "- True일 경우 Color가 없는 MobileShader에서도 사용가능 합니다.(DrawCalls증가 안함)" + "\n" +
									 "- False일 경우 Color가 있는 Shader만 지원 합니다.(DrawCall증가함)"
								}
							  };
	public static string GetHsEditor_NcAutoDeactive(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcAutoDeactive);
		string	tooltip = "";

		switch (caption)
		{
			case "m_fLifeTime":					tooltip	= HsEditor_NcAutoDeactive[GetLang(), 1];		break;
			case "m_bEndNcCurveAnimation":		tooltip	= HsEditor_NcAutoDeactive[GetLang(), 2];		break;

			case "m_fSmoothDestroyTime":		tooltip	= HsEditor_NcAutoDeactive[GetLang(), 3];		break;
			case "m_bTDisableEmit":				tooltip	= HsEditor_NcAutoDeactive[GetLang(), 4];		break;
			case "m_bSmoothHide":				tooltip	= HsEditor_NcAutoDeactive[GetLang(), 5];		break;

			case "m_CollisionType":				tooltip	= HsEditor_NcAutoDeactive[GetLang(), 6];		break;
			case "m_CollisionLayer":			tooltip	= HsEditor_NcAutoDeactive[GetLang(), 7];		break;
			case "m_fCollisionRadius":			tooltip	= HsEditor_NcAutoDeactive[GetLang(), 8];		break;
			case "m_fDeactivePosY":				tooltip	= HsEditor_NcAutoDeactive[GetLang(), 9];		break;

			case "m_bMeshFilterOnlySmoothHide":	tooltip	= HsEditor_NcAutoDeactive[GetLang(),10];		break;

			default: return "NcAutoDeactive\n" + HsEditor_NcAutoDeactive[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcAutoDestruct ===================================================================================
	protected static string[,]	HsEditor_NcAutoDestruct = {
								{
									 "Automatically remove gameObject." + "\n" +
									 "-Automatically destructs after time passes by designating LifeTime." + "\n" +
									 "- By setting SmoothDestrotTime, FadeOut can be applied while disappearing.(For only materials that can be Alpha proccessed)" + "\n" +
									 "- By setting CollisionTime, automatically destructs even LifeTime remains."
									,"- If DestroyTime elapses after it becomes Active, remove GameObject." + "\n" +
									 "- Process only when the value is greater than 0."
									,"- After animation of NcCurveAnimation finishes, remove GameObject."

									,"- Deisgnate Destruction processing time." + "\n" +
									 "- During this time, Particle can be paused and disappear slowly." + "\n" +
									 "- In case ParentGameObject destructs first, Particle destructs immediately."
									,"- True: Pauses 'Emission of ParticleSystem' during destruction." + "\n" +
									 "- Does not create any more Particle."
									,"- True: Reduces Alpha value of Material during destruction.(Only to Material which contains Alpha.)"

									,"- Create additional destruction condition besides destruction method." + "\n" +
									 "- If other Effect after destruction is wanted, use NcAttachPrefab.Destroy."
									,"- Select Layer to be used as collision object."
									,"- Radius to be used for collision test."
									,"- Destruction occurs if location of y from Particle is less than designated value."

									,"- Apply the MeshFilter only SmoothHide." + "\n" +
									 "- True: Can be used in MobileShader.(DrawCalls does not increase)" + "\n" +
									 "- False: Can be used in a shader that contains color."
								}
								,
								{
									 "gameObject를 자동 소멸 처리한다." + "\n" +
									 "- LifeTime을 지정하여 시간이 경과하면 자동 소멸 처리합니다." + "\n" +
									 "- SmoothDestrotTime을 설정하면, 사라지는 동안 FadeOut 처리가 가능하다.(Alpha처리가 가능한 Material만 사용 가능)" + "\n" +
									 "- CollisionType을 설정하면, LifeTime이 경과하지 않더라도 소멸 처리 됩니다."
									,"- Active된 후 LifeTime이 경과하면 GameObject를 소멸 시킨다." + "\n" +
									 "- 0 보다 큰 값일 때만 처리한다."
									,"- NcCurveAnimation의 애니메이션이 끝나면 GameObject를 소멸 시킨다."

									,"- 소멸 처리 시간을 지정한다." + "\n" +
									 "- 이 시간 동안 Particle을 정지 시키고, 천천히 사라지게 할 수 있다." + "\n" +
									 "- 단, ParentGameObject가 먼저 소멸할 경우 즉시 소멸 처리된다."
									,"- true: 소멸되는 시간동안 'ParticleSystem의 Emission'을 정지 시킨다." + "\n" +
									 "- 더 이상 파티클은 생성되지 않는다."
									,"- true: 소멸되는 시간동안 Material의 alpha값을 감소 시킨다."

									,"- 소멸시간 방식과는 별개로 추가적인 소멸 조건을 설정 합니다." + "\n" +
									 "- 충돌 후 다른 Effect를 표시하고 싶으면, NcAttachPrefab.Destroy를 사용하세요."
									,"- 충돌 대상이 되는 Layer를 선택 합니다."
									,"- 충돌 검사에 사용할 반지름 입니다."
									,"- Particle의 y 위치가 지정한 값보다 작을 경우 소멸 합니다."

									,"- MeshFilter 만 SmoothHide을 적용합니다." + "\n" +
									 "- True일 경우 Color가 없는 MobileShader에서도 사용가능 합니다.(DrawCalls증가 안함)" + "\n" +
									 "- False일 경우 Color가 있는 Shader만 지원 합니다.(DrawCall증가함)"
								}
							  };
	public static string GetHsEditor_NcAutoDestruct(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcAutoDestruct);
		string	tooltip = "";

		switch (caption)
		{
			case "m_fLifeTime":					tooltip	= HsEditor_NcAutoDestruct[GetLang(), 1];		break;
			case "m_bEndNcCurveAnimation":		tooltip	= HsEditor_NcAutoDestruct[GetLang(), 2];		break;

			case "m_fSmoothDestroyTime":		tooltip	= HsEditor_NcAutoDestruct[GetLang(), 3];		break;
			case "m_bTDisableEmit":				tooltip	= HsEditor_NcAutoDestruct[GetLang(), 4];		break;
			case "m_bSmoothHide":				tooltip	= HsEditor_NcAutoDestruct[GetLang(), 5];		break;

			case "m_CollisionType":				tooltip	= HsEditor_NcAutoDestruct[GetLang(), 6];		break;
			case "m_CollisionLayer":			tooltip	= HsEditor_NcAutoDestruct[GetLang(), 7];		break;
			case "m_fCollisionRadius":			tooltip	= HsEditor_NcAutoDestruct[GetLang(), 8];		break;
			case "m_fDestructPosY":				tooltip	= HsEditor_NcAutoDestruct[GetLang(), 9];		break;

			case "m_bMeshFilterOnlySmoothHide":	tooltip	= HsEditor_NcAutoDestruct[GetLang(),10];		break;

			default: return "NcAutoDestruct\n" + HsEditor_NcAutoDestruct[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcBillboard ===================================================================================
	protected static string[,]	HsEditor_NcBillboard = {
								{
									 "- Process GameObject with Billboard." + "\n" +
									 "- Additional function : Rotation, RandomRotation"
									,"- true: Look at the camera." + "\n" +
									 "- false: Look at the camera's Plane and make parallel with it."
									,"- true: Ignore up vector of the camera and maintain up vector of GameObject."
									,"- true: Ignore the value of x-axis rotation among directions of the camera and maintain standing of Mesh." + "\n" +
									 "- Though x-axis of the camera rotates, Object is not affected."
									,"- Choose the axis on which you will look at the camera."

									,"- NONE : Unused" + "\n" +
									 "- RND : Random retation (LocalSpace)" + "\n" +
									 "- ROTATE : Consecutively rotate toward designated rotation value (LocalSpace)"
									,"- Decide Rotation axis. (LocalSpace)"
									,"- Input rotation value. (LocalSpace)"
								}
								,
								{
									 "- GameObject를 Billboard 처리한다." + "\n" +
									 "- 추가기능: Rotation, RandomRotation"
									,"- true: 카메라를 바라보게 한다." + "\n" +
									 "- false: 카메라의 Plane을 바라보게 하여, 평행이 되도록 한다."
									,"- true: 카메라 업벡터를 무시하고 GameObject의 업벡터를 유지한다."
									,"- true: 카메라의 방향중 x축 회전 값은 무시하고 Mesh의 세워짐을 유지한다." + "\n" +
									 "- 카메라의 x축이 회전 되더라도 Object는 영향을 안받는다."
									,"- 카메라를 바라볼 축을 선택한다."

									,"- NONE : 사용안함" + "\n" +
									 "- RND : Random 회전 (LocalSpace)" + "\n" +
									 "- ROTATE : 지정한 회전 값으로 계속 회전 (LocalSpace)"
									,"- 회전할 축을 결정 합니다. (LocalSpace)"
									,"- 회전 값을 입력 합니다. (LocalSpace)"
								}
							  };
	public static string GetHsEditor_NcBillboard(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcBillboard);
		string	tooltip = "";

		switch (caption)
		{
			case "m_bCameraLookAt":			tooltip	= HsEditor_NcBillboard[GetLang(), 1];		break;
			case "m_bFixedObjectUp":		tooltip	= HsEditor_NcBillboard[GetLang(), 2];		break;
			case "m_bFixedStand":			tooltip	= HsEditor_NcBillboard[GetLang(), 3];		break;
			case "m_FrontAxis":				tooltip	= HsEditor_NcBillboard[GetLang(), 4];		break;

			case "m_RatationAxis":			tooltip	= HsEditor_NcBillboard[GetLang(), 5];		break;
			case "m_RndAxis":				tooltip	= HsEditor_NcBillboard[GetLang(), 6];		break;
			case "m_fRotationValue":		tooltip	= HsEditor_NcBillboard[GetLang(), 7];		break;

			default: return "NcBillboard\n" + HsEditor_NcBillboard[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcDelayActive ===================================================================================
	protected static string[,]	HsEditor_NcDelayActive = {
								{
									 "- Delay the Active time of gameObject(The time indicated: Delay)." + "\n" +
									 "- As all components included in gameObject are affected, there will be a batch process of delay."
									,"- Assign the delay time in which the mode is changed into Active mode." + "\n" +
									 "- When EffectPrefab is created, it becomes automatically Deactive and will become Active after the delay time." + "\n" +
									 "- Process only when the value is greater than 0."
									,"- Apply operations to all ChildGameObject when it becomes Active/Deactive." + "\n" +
									 "- Process only when the value of m_fDelayTime is greater than 0."
									,"- After it becomes Active, if fDeactiveTime elapses, make GameObject Deactive." + "\n" +
									 "- Process only when the value is greater than 0."
									,"- (The value for reference for Editor)" + "\n" +
									 "- This is the time in which Parent GameObject becomes Active." + "\n" +
									 "- When Parent delay Active with NcDelayActive, the delay time will be indicated."
								}
								,
								{
									 "- 표시시간 Delay : gameObject의 Active 시간을 지연 한다." + "\n" +
									 "- gameObject에 포함된 모든 컴포넌트가 영향을 받게 되므로, 일괄 지연처리 된다."
									,"- Active상태로 변경되는 지연시간을 지정한다." + "\n" +
									 "- EffectPrefab이 생성될 때 자동으로 Deactive되며, 지연시간 후 Active 된다." + "\n" +
									 "- 0 보다 큰 값일 때만 처리한다."
									,"- Active/Deactive 될 때 모든 ChildGameObject도 적용한다." + "\n" +
									 "- m_fDelayTime이 0보다 큰 값일 때만 처리한다."
									,"- Active된 후 fDeactiveTime이 경과하면, GameObject를 Deactive 시킨다." + "\n" +
									 "- 0 보다 큰 값일 때만 처리한다."
									,"- (Editor용 참고 값)" + "\n" +
									 "- Parent GameObject가 Active되는 시간이다." + "\n" +
									 "- Parent가 NcDelayActive로 Active를 지연할 경우 그 시간이 표시된다."
								}
							  };
	public static string GetHsEditor_NcDelayActive(string caption)
	{
/*		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcDelayActive);
		string	tooltip = "";

		switch (caption)
		{
			case "m_fDelayTime":			tooltip	= HsEditor_NcDelayActive[GetLang(), 1];	break;
			case "m_bActiveRecursively":	tooltip	= HsEditor_NcDelayActive[GetLang(), 2];	break;
			case "m_fAliveTime":			tooltip	= HsEditor_NcDelayActive[GetLang(), 3];	break;
			case "m_fParentDelayTime":		tooltip	= HsEditor_NcDelayActive[GetLang(), 4];	break;
			default: return "NcDelayActive\n" + HsEditor_NcDelayActive[GetLang(), 0];
		}
 		return caption + "\n" + tooltip;
*/
		return "This component is not available.";
	}

	// Editor NcDetachParent ===================================================================================
	protected static string[,]	HsEditor_NcDetachParent = {
								{
									 "- Separate gameObject and ParentGameObject, not to be removed together." + "\n" +
									 "- Though the body and the tail of missile disappear together, the tail will be used to GameObject which should be indicated for a period of time."
									,"- true: Though separated from ParentGameObject, it will follow the position of ParentGameObject."
									,"- Automatic removal time: Start time of removal is decided according to m_bParentHideToStartDestroy." + "\n" +
									 "- It has own removal time as separated from ParentGameObject." + "\n" +
									 "- If the value is 0, it is not removed but remains after Scene disappears."
									,"- true: If ParentGameObject becomes Deactive, the timer of automatic removal starts; after m_fSmoothDestroyTime elapses, it is removed." + "\n" +
									 "- false: If gameObject becomes active, the timer of automatic removal starts; after m_fSmoothDestroyTime, it is removed."
									,"- true: if the timer of automatic removal starts, stop Emission of ParticleSystem." + "\n" +
									 "- Do not create particles anymore."
									,"- true: if the timer of automatic removal starts, decrease the value of alpha of Material during m_fSmoothDestroyTime (But, only applied to material which has Alpha)."

									,"- Apply the MeshFilter only SmoothHide." + "\n" +
									 "- True: Can be used in MobileShader.(DrawCalls does not increase.)" + "\n" +
									 "- False: Can be used in a shader that contains color."
								}
								,
								{
									 "- gameObject의 ParentGameObject를 분리 시켜서 같이 소멸되지 않도록 한다." + "\n" +
									 "- 미사일 꼬리와 같이 본체가 사라져도 꼬리는 일정시간 표시해야 할 GameObject에 사용된다."
									,"- true: ParentGameObject와 분리 되더라도, ParentGameObject의 위치를 따라 다닌다."
									,"- 자동 소멸 시간, m_bParentHideToStartDestroy에 따라 소멸시작 시간이 결정된다." + "\n" +
									 "- ParentGameObject와 분리되었기에 자체 소멸 시간을 가지고 있다." + "\n" +
									 "- 0이면 소멸되지 않고, Scene이 사라질 때까지 남아 있는다."
									,"- true: ParentGameObject가 Deactive되면 자동소멸 타이머가 시작하고, m_fSmoothDestroyTime 시간이 지나면 소멸 한다." + "\n" +
									 "- false: gameObject가 active되면 자동소멸 타이머가 시작하고, m_fSmoothDestroyTime 시간 이후에 소멸된다."
									,"- true: 자동소멸 타이머가 시작하면, ParticleSystem의 Emission을 정지 시킨다." + "\n" +
									 "- 더 이상 파티클은 생성되지 않는다."
									,"- true: 자동소멸 타이머가 시작하면, m_fSmoothDestroyTime 동안 Material의 alpha값을 감소 시킨다.(단, Alpha가 있는 material에만 적용 된다)"

									,"- MeshFilter 만 SmoothHide을 적용합니다." + "\n" +
									 "- True일 경우 Color가 없는 MobileShader에서도 사용가능 합니다.(DrawCalls증가 안함)" + "\n" +
									 "- False일 경우 Color가 있는 Shader만 지원 합니다.(DrawCall증가함)"
								}
							  };
	public static string GetHsEditor_NcDetachParent(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcDetachParent);
		string	tooltip = "";

		switch (caption)
		{
			case "m_bFollowParentTransform":	tooltip	= HsEditor_NcDetachParent[GetLang(), 1];		break;
			case "m_fSmoothDestroyTime":		tooltip	= HsEditor_NcDetachParent[GetLang(), 2];		break;
			case "m_bParentHideToStartDestroy":	tooltip	= HsEditor_NcDetachParent[GetLang(), 3];		break;
			case "m_bTDisableEmit":				tooltip	= HsEditor_NcDetachParent[GetLang(), 4];		break;
			case "m_bSmoothHide":				tooltip	= HsEditor_NcDetachParent[GetLang(), 5];		break;
			case "m_bMeshFilterOnlySmoothHide":	tooltip	= HsEditor_NcAutoDestruct[GetLang(), 6];		break;
			default: return "NcDetachParent\n" + HsEditor_NcDetachParent[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcDuplicator ===================================================================================
	protected static string[,]	HsEditor_NcDuplicator = {
								{
									"- Create gameObject repeatedly at certain intervals." + "\n" +
									 "- Be careful that Draw Call will happen as many times as the number of repetition if it becomes Color animation."
									,"- The time for repeated creation" + "\n" +
									 "- the minimum value 0.01"
									,"- The number of repeated creation" + "\n" +
									 "- if the value is 0, create continuously during Active mode"
									,"- if the value is greater than 0, GameObject will be destroyed after the assigned time elapses."
									,"- Cumulatively substitute the value of rotation of GameObject created repeatedly." + "\n" +
									 "- Obj0 = 0*rot, Obj1 = 1*rot, Obj2 = 2*rot"
									,"- The range of 'Random Position' of GameObject created repeatedly"
									,"- Modify location of repeatedly created Gameobject."
								}
								,
								{
									 "- gameObject를 일정시간 간격으로 반복 생성 한다." + "\n" +
									 "- Color 애니메이션이 될 경우 반복수 만큼 Draw Call이 발생 하므로 주의하세요."
									,"- 반복생성 시간" + "\n" +
									 "- 최소값 0.01"
									,"- 반복생성 수" + "\n" +
									 "- 0이면 Active 상태동안 계속 생성"
									,"- 0보다 크면, 생성된 GameObject가 지정 시간 경과 후 Destroy 된다."
									,"- 반복 생성되는 GameObject의 회전 값을 누적 대입 한다." + "\n" +
									 "- Obj0 = 0*rot, Obj1 = 1*rot, Obj2 = 2*rot"
									,"- 반복 생성되는 GameObject의 'Random Position' 범위"
									,"- Modify location of repeatedly created Gameobject."
								}
							  };
	public static string GetHsEditor_NcDuplicator(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcDuplicator);
		string	tooltip = "";

		switch (caption)
		{
			case "m_fDuplicateTime":			tooltip	= HsEditor_NcDuplicator[GetLang(), 1];		break;
			case "m_nDuplicateCount":			tooltip	= HsEditor_NcDuplicator[GetLang(), 2];		break;
			case "m_fDuplicateLifeTime":		tooltip	= HsEditor_NcDuplicator[GetLang(), 3];		break;
			case "m_AccumStartRot":				tooltip	= HsEditor_NcDuplicator[GetLang(), 4];		break;
			case "m_RandomRange":				tooltip	= HsEditor_NcDuplicator[GetLang(), 5];		break;
			case "m_AddStartPos":				tooltip	= HsEditor_NcDuplicator[GetLang(), 6];		break;

			default: return "NcDuplicator\n" + HsEditor_NcDuplicator[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcParticleSpiral ===================================================================================
	protected static string[,]	HsEditor_NcParticleSpiral = {
								{
									 "- "
									,"- Select Prefab which includes ParticleEmiter." + "\n" +
									 "- When not selected, Use ParticleEmiter from CurrentGameObject. (Has to be included by RootGameObject)."
									,"- Select Prefab which includes ParticleEmiter. (Has to be included by RootGameObject)."
									,"- Remove selected ParticlePrefab."
									,"- Open selected ParticlePrefab."
								},{
									 "- ParticleEmiter를 사용하여 다양한 회전 파티클을 구현 합니다."
									,"- ParticleEmiter를 포함 하고 있는 Prefab을 선택 합니다." + "\n" +
									 "- 선택하지 않을 경우 CurrentGameObject에 있는 ParticleEmiter를 사용 합니다."
									,"- ParticleEmiter를 포함하고 있는 Prefab을 선택합니다. (RootGameObject에 포함되어 있어야 한다)"
									,"- 선택된 ParticlePrefab을 제거 합니다."
									,"- 선택된 ParticlePrefab을 오픈 합니다."
								}
							  };
	public static string GetHsEditor_NcParticleSpiral(string caption)
	{
		if (m_bTDisable)	return " ";

 		CheckValid(HsEditor_NcParticleSpiral);
 		string	tooltip = "";

		switch (caption)
		{
			case "m_ParticlePrefab":			tooltip	= HsEditor_NcParticleSpiral[GetLang(), 1];		break;
			case "SelectParticlePrefab":		tooltip	= HsEditor_NcParticleSpiral[GetLang(), 2];		break;
			case "ClearPrefab":					tooltip	= HsEditor_NcParticleSpiral[GetLang(), 3];		break;
			case "OpenPrefab":					tooltip	= HsEditor_NcParticleSpiral[GetLang(), 4];		break;

			default: return "NcParticleSpiral\n" + HsEditor_NcParticleSpiral[GetLang(), 0];
		}
 		return caption + "\n" + tooltip;
	}

	// Editor NcParticleEmit ===================================================================================
	protected static string[,]	HsEditor_NcParticleEmit = {
								{
									 "- Emit Shared ParticleSystem(ParticleEmiter or ParticleSystem)" + "\n" +
									 "- Drawcall does not increase. (multiple instances)"
									,"- Select Prefab which includes ParticleSystem. (Has to be included by RootGameObject)."
									,"- Select Prefab which includes ParticleSystem. (Has to be included by RootGameObject)."
									,"- Remove selected ParticlePrefab."
									,"- Open selected ParticlePrefab."

									,""
									,"- Assign Delaytime. " + "\n" +
									 "- This is the time elapsed after GameObject.Active becomes True."
									,"- Input the repetition period. Repeatedly creates by the period designated according to m_fReapeatCount."
									,"- If larger than 0, limits the number of creation." + "\n" +
									 "- If 0, proceed repetition unlimitedly. (Only when m_fRepeatTime is larger than 0.)"

									,"- ParticleCount of OneShot"
								},{
									 "- ParticleEmiter or ParticleSystem을 공유하여 OneShot Emit 합니다." + "\n" +
									 "- 이 이펙트는 여러개 생성되더라도 DrawCalls가 증가 하지 않습니다."
									,"- ParticleSystem를 포함 하고 있는 Prefab을 선택 합니다. (RootGameObject에 포함되어 있어야 한다)"
									,"- ParticleSystem를 포함하고 있는 Prefab을 선택합니다. (RootGameObject에 포함되어 있어야 한다)"
									,"- 선택된 ParticlePrefab을 제거 합니다."
									,"- 선택된 ParticlePrefab을 오픈 합니다."

									,""
									,"- Delaytime을 지정 합니다." + "\n" +
									 "- GameObject.Active가 True로 된 후 경과될 시간 입니다."
									,"- 반복 시간을 입력합니다. 지정 간격으로 m_fRepeatCount 수만큼 반복 생성 합니다."
									,"- 0보다 큰 값이면, 생성할 수를 제한합니다." + "\n" +
									 "- 0이면 무한 반복 합니다.(단, m_fRepeatTime이 0보다 커야 합니다)."

									,"- 한번에 나올 파티클 수를 지정합니다."
								}
							  };
	public static string GetHsEditor_NcParticleEmit(string caption)
	{
		if (m_bTDisable)	return " ";

 		CheckValid(HsEditor_NcParticleEmit);
 		string	tooltip = "";

		switch (caption)
		{
			case "m_ParticlePrefab":			tooltip	= HsEditor_NcParticleEmit[GetLang(), 1];		break;
			case "SelectParticlePrefab":		tooltip	= HsEditor_NcParticleEmit[GetLang(), 2];		break;
			case "ClearPrefab":					tooltip	= HsEditor_NcParticleEmit[GetLang(), 3];		break;
			case "OpenPrefab":					tooltip	= HsEditor_NcParticleEmit[GetLang(), 4];		break;

			case "m_AttachType":				tooltip	= HsEditor_NcParticleEmit[GetLang(), 5];		break;
			case "m_fDelayTime":				tooltip	= HsEditor_NcParticleEmit[GetLang(), 6];		break;
			case "m_fRepeatTime":				tooltip	= HsEditor_NcParticleEmit[GetLang(), 7];		break;
			case "m_nRepeatCount":				tooltip	= HsEditor_NcParticleEmit[GetLang(), 8];		break;

			case "m_EmitCount":					tooltip	= HsEditor_NcParticleEmit[GetLang(), 9];		break;

			default: return "NcParticleEmit\n" + HsEditor_NcParticleEmit[GetLang(), 0];
		}
 		return caption + "\n" + tooltip;
	}

	// Editor NcParticleSystem ===================================================================================
	protected static string[,]	HsEditor_NcParticleSystem = {
								{
									 "- Provide additional functions to ParticleSystem(Legacy, Shuriken)." + "\n" +
									 "- Must be included when Transform.Scale is wanted to be auotomatically applied." + "\n" +
									 "- Major functions: Managing Particle Component, Delay, Repeat, Speed, ScaleWithTransfrm" + "\n" +
									 "- (ParticleSystem, EllipsoidParticleEmitter, MeshParticleEmitter) - able to operate only if one among the three exists. "
									,"- Assign Delay time. (This is the time elapsed after GameObject becomes Active.)"
									,"- If true, operate with Burst mode; if false, operate with basic Emit mode."
									,"- Particles, as many as the number of m_fBurstEmissionCount, will be created at assigned intervals." + "\n" +
									 "- Emission which is set in ParticleSystem(Legacy, Shuriken) will not operate."
									,"- Limit the number of repetition of m_fBurstRepeatTime." + "\n" +
									 "- If the value is 0, no limit"
									,"- This is the number of particles that will be displayed at one time."
									,"- Assign the time of operation of ParticleSystem(Legacy, Shuriken). (If the value is 0, no limit)"
									,"- It will operate when you assign m_fEmitTime; m_fSleepTime is the time in which it becomes stopped. (If the value is 0, it will not be played repeatedly)"

									,"- Apply Transform Scale." + "\n" +
									 "- Application: Legacy, Shuriken" + "\n" +
									 "- Change in Scale will be applied in the game." + "\n" +
									 "- But, in case of MeshParticleEmitter, change in Scale will be only applied in FX Maker."
									,"- Adjust the size of Particle. (This is the value which is multiplied by.)" + "\n" +
									 "- Application: Legacy/MinMaxSize, Shuriken/startSize"
									,"- Adjust the living time. (This is the value which is multiplied by.)" + "\n" +
									 "- Application: Legacy/MinMaxEnergy, Shuriken/startLifetime"
									,"- Adjust the number of Particle creation. (This is the value which is multiplied by.)" + "\n" +
									 "- Application: Legacy/MinMaxEmission, Shuriken/Emission"
									,"- Adjust the value of StartVelocity. (This is the value which is multiplied by.)" + "\n" +
									 "- Application: Legacy/AllVelocity, Shuriken/startSpeed"
									,"- Adjust the length of a particle of Stretch Particles. (This is the value which is multiplied by.)" + "\n" +
									 "- Application: Legacy/ParticleRenderer.LengthScale, Shuriken/Renderer.LengthScale" + "\n" +
									 "- Mode: Legacy/ParticleRenderer.Stretch Particles, Shuriken/Renderer.Renderer Mode.StretchedBillboard"

									,"- Assign MeshNormalVelocity of MeshParticleEmitter. (This is the value which is substituted.)" + "\n" +
									 "- It will be referred when Scale changes in ScaleWithTransform." + "\n" +
									 "- Do not modify MeshParticleEmitter.minmaxMeshNormalVelocity but modify the value here."
									,"- Assign MeshNormalVelocity of MeshParticleEmitter. (This is the value which is substituted.)" + "\n" +
									"- It will be referred when Scale changes in ScaleWithTransform." + "\n" +
									 "- Do not modify MeshParticleEmitter.minmaxMeshNormalVelocity but modify the value here."

									,"- This is the function only provided in ParticleSystem(Shuriken), which controls operating speed." + "\n" +
									 "- If the value is equal to 1, it operates with the basic speed; if the value is equal to 2, it operates with the double speed."
									,"- The value of ParticleSystem(Shuriken).SimulationSpace is automatically synchronized (when Restart)." + "\n" +
									 "- User does not need to change; this is the value referred by ScaleWithTransform."

									,"- Decide destruction time." + "\n" +
									 "- Particle destructs when collision occurs, and if designated Prefab exists, it replaces particle."
									,"- Select Layer which becomes collision object."
									,"- Radius of Particle which will be used for collision test."
									,"- Destruction occurs if location of y from Particle is less than designated value."

									,"- Prefab to be created at collision location when it occurs."
									,"- Scale value to be multiplied to created Prefab." + "\n" +
									 "- newPrefab.Scale = newPrefab.Scale * Particle.Size * m_fPrefabScale"

									,"- Adjust Speed of m_AttachPrefab to be created." + "\n" +
									 "- Warning of adjusting vleocity" + "\n" +
									 "- Animation : Speed of Unity animation is not adjustable.(Support is scheduled)" + "\n" +
									 "-Particle emitter: Adjustment of Damping and Velocity of LegacyParticle is required." + "\n" +
									 "- ShurikenParticleSystem : Not Support RuntimeSpeed"

									,"- Adjust auto-destruction period of m_AttachPrefab to be created." + "\n" +
									 "- If auto-destruction period(NcAutoDestruct) is designated at m_Attach Prefab to be created, the period resets to newly designated time."

									,"- Choose Prefab which is already saved."
									,"- Clear the chosen Prefab."
									,"- Open the chosen Prefab."

									,"- Real-scale changes to the static-scale." + "\n" +
									 "- Only LegacyParticleSystem.EllipsoidParticleEmitter." + "\n" +
									 "- 'NcParticleSystem.bScaleWithTransform' option automatically changes to off." + "\n" +
									 "- Replace the value of EllipsoidParticleEmitter. (reference Transform.Scale)"
									,"- If you use the replay(NsEffectManager.RunReplayEffect()), please click this button." + "\n" +
									 "- NsEffectManager.RunReplayEffect(), ParticleEmitter.OneShot is not supported."
								}
								,
								{
									 "- ParticleSystem(Legacy, Shuriken)에 부과 기능을 제공합니다." + "\n" +
									 "- Transform.Scale을 자동 적용하고 싶을 경우 포함해야 합니다." + "\n" +
									 "- 주요기능: Particle Component관리, Delay, Repeat, Speed, ScaleWithTransfrm" + "\n" +
									 "- (ParticleSystem, EllipsoidParticleEmitter, MeshParticleEmitter) - 셋 중 하나가 있어야 동작 됩니다. "
									,"- Delay 시간을 지정합니다.(GameObject가 Active된 후 경과 시간 입니다.)"
									,"- true이면 Burst 모드로 동작 하고, false면 기본 Emit모드로 동작 합니다."
									,"- 지정된 간격으로 m_fBurstEmissionCount 수만큼 파티클이 생성됩니다." + "\n" +
									 "- ParticleSystem(Legacy, Shuriken)에서 설정된 emission은 동작하지 않습니다."
									,"- m_fBurstRepeatTime의 반복 수를 제한 합니다." + "\n" +
									 "- 0이면 무제한"
									,"- 한번에 표시될 파티클 수 입니다."
									,"- ParticleSystem(Legacy, Shuriken)의 가동 시간을 지정 합니다. (0이면 무제한)"
									,"- m_fEmitTime을 지정했을 경우 동작하며, m_fSleepTime은 정지되는 시간 입니다. (0이면 반복재생 하지 않습니다)"

									,"- Transform Scale을 적용 합니다." + "\n" +
									 "- 적용: Legacy, Shuriken" + "\n" +
									 "- 게임에서도 Scale 변화가 적용 됩니다." + "\n" +
									 "- 단, MeshParticleEmitter의 경우 FX Maker에서만 Scale이 변경 됩니다."
									,"- Particle 크기를 조절 합니다. (곱해 지는 값입니다.)" + "\n" +
									 "- 적용: Legacy/MinMaxSize, Shuriken/startSize"
									,"- 살아 있는 시간을 조절 합니다. (곱해 지는 값입니다.)" + "\n" +
									 "- 적용: Legacy/MinMaxEnergy, Shuriken/startLifetime"
									,"- 파티클 생성 수를 조절 합니다. (곱해 지는 값입니다.)" + "\n" +
									 "- 적용: Legacy/MinMaxEmission, Shuriken/Emission"
									,"- StartVelocity값을 조절 합니다.(곱해 지는 값입니다.)" + "\n" +
									 "- 적용: Legacy/AllVelocity, Shuriken/startSpeed"
									,"- Stretch Particles의 파티클 길이를 조절 합니다.(곱해 지는 값입니다.)" + "\n" +
									 "- 적용: Legacy/ParticleRenderer.LengthScale, Shuriken/Renderer.LengthScale" + "\n" +
									 "- 모드: Legacy/ParticleRenderer.Stretch Particles, Shuriken/Renderer.Renderer Mode.StretchedBillboard"

									,"- MeshParticleEmitter의 MeshNormalVelocity를 지정 합니다.(대입되는 값입니다.)" + "\n" +
									 "- ScaleWithTransform에서 Scale이 변할 때 참조 됩니다." + "\n" +
									 "- MeshParticleEmitter.minmaxMeshNormalVelocity를 수정 하지 마시고, 여기 값을 수정 하세요."
									,"- MeshParticleEmitter의 MeshNormalVelocity를 지정 합니다.(대입되는 값입니다.)" + "\n" +
									 "- ScaleWithTransform에서 Scale이 변할 때 참조 됩니다." + "\n" +
									 "- MeshParticleEmitter.minmaxMeshNormalVelocity를 수정 하지 마시고, 여기 값을 수정 하세요."

									,"- ParticleSystem(Shuriken)에서만 제공되는 기능이며, 동작 속도르 조절 합니다." + "\n" +
									 "- 1이면 기본 시간으로 동작하고, 2이면 2배속으로 동작 합니다."
									,"- ParticleSystem(Shuriken).SimulationSpace 값이 자동 동기화(Restart될때) 됩니다." + "\n" +
									 "- User가 변경할 필요 없으며, ScaleWithTransform에서 참조 하는 값입니다."

									,"- Particle 소멸 시기를 결정 합니다." + "\n" +
									 "- 충돌할 경우 Particle은 소멸하며, 지정한 Prefab이 있을 경우 소멸 위치에 생성 합니다."
									,"- 충돌 대상이 되는 Layer를 선택 합니다."
									,"- 충돌 검사에 사용할 Particle의 반지름 입니다."
									,"- Particle의 y 위치가 지정한 값보다 작을 경우 소멸 합니다."

									,"- 충돌할 경우 충돌 위치에 생성할 Prefab 입니다."
									,"- 생성할 Prefab 크기에 곱해질 Scale 값 입니다." + "\n" +
									 "- newPrefab.Scale = newPrefab.Scale * Particle.Size * m_fPrefabScale"
									,"- 생성될 m_AttachPrefab의 속도를 Runtime 조절 합니다." + "\n" +
									 "- 속도조절 주의사항" + "\n" + 
									 "- Aanimation : Unity animation은 속도조절이 안됩니다.(지원예정)" + "\n" + 
									 "- particleEmitter : LegacyParticle은 Damping 및 Velocity조절이 필요 합니다." + "\n" + 
									 "- ShurikenParticleSystem : Runtime 속도조절을 지원하지 않습니다."

									,"- 생성될 m_AttachPrefab의 자동소멸 시간을 설정합니다." + "\n" +
									 "- 생성될 m_AttachPrefab에 자동소멸(NcAutoDestruct) 시간이 설정되어 있다면, 새롭게 설정한 시간으로 변경됩니다."

									,"- 이미 저장되어 있는 Prefab을 선택 합니다."
									,"- 선택된 Prefab을 Clear 합니다."
									,"- 선택된 Prefab을 Open 합니다."

									,"- 실시간 scale을 정적 scale로 변경 합니다." + "\n" +
									 "- 주의 : 이 옵션은 CPU를 많이 사용합니다." + "\n" +
									 "- LegacyParticleSystem.EllipsoidParticleEmitter 전용 입니다." + "\n" +
									 "- bScaleWithTransform 옵션은 자동으로 off 로 변경 됩니다." + "\n" +
									 "- Transform.Scale값을 참조하여 EllipsoidParticleEmitter의 값을 모두 바꿉니다."
									,"- ParticleEmitter의 OneShot을 NcParticleSystem의 bBurst로 전환합니다." + "\n" +
									 "- NsEffectManager.RunReplayEffect()가 정상적으로 작동하기 위해서는 ParticleEmitter의 OneShot을 사용하면 안됩니다."
								}
							  };
	public static string GetHsEditor_NcParticleSystem(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcParticleSystem);
		string	tooltip = "";

		switch (caption)
		{
			case "m_fStartDelayTime":					tooltip	= HsEditor_NcParticleSystem[GetLang(), 1];		break;
			case "m_bBurst":							tooltip	= HsEditor_NcParticleSystem[GetLang(), 2];		break;
			case "m_fBurstRepeatTime":					tooltip	= HsEditor_NcParticleSystem[GetLang(), 3];		break;
			case "m_nBurstRepeatCount":					tooltip	= HsEditor_NcParticleSystem[GetLang(), 4];		break;
			case "m_fBurstEmissionCount":				tooltip	= HsEditor_NcParticleSystem[GetLang(), 5];		break;
			case "m_fEmitTime":							tooltip	= HsEditor_NcParticleSystem[GetLang(), 6];		break;
			case "m_fSleepTime":						tooltip	= HsEditor_NcParticleSystem[GetLang(), 7];		break;

			case "m_bScaleWithTransform":				tooltip	= HsEditor_NcParticleSystem[GetLang(), 8];		break;
			case "m_fStartSizeRate":					tooltip	= HsEditor_NcParticleSystem[GetLang(), 9];		break;
			case "m_fStartLifeTimeRate":				tooltip	= HsEditor_NcParticleSystem[GetLang(), 10];		break;
			case "m_fStartEmissionRate":				tooltip	= HsEditor_NcParticleSystem[GetLang(), 11];		break;
			case "m_fStartSpeedRate":					tooltip	= HsEditor_NcParticleSystem[GetLang(), 12];		break;
			case "m_fRenderLengthRate":					tooltip	= HsEditor_NcParticleSystem[GetLang(), 13];		break;

			case "m_fLegacyMinMeshNormalVelocity":		tooltip	= HsEditor_NcParticleSystem[GetLang(), 14];		break;
			case "m_fLegacyMaxMeshNormalVelocity":		tooltip	= HsEditor_NcParticleSystem[GetLang(), 15];		break;

			case "m_fShurikenSpeedRate":				tooltip	= HsEditor_NcParticleSystem[GetLang(), 16];		break;
			case "m_bWorldSpace":						tooltip	= HsEditor_NcParticleSystem[GetLang(), 17];		break;

			case "m_ParticleDestruct":					tooltip	= HsEditor_NcParticleSystem[GetLang(), 18];		break;
			case "m_CollisionLayer":					tooltip	= HsEditor_NcParticleSystem[GetLang(), 19];		break;
			case "m_fCollisionRadius":					tooltip	= HsEditor_NcParticleSystem[GetLang(), 20];		break;
			case "m_fDestructPosY":						tooltip	= HsEditor_NcParticleSystem[GetLang(), 21];		break;

			case "m_AttachPrefab":						tooltip	= HsEditor_NcParticleSystem[GetLang(), 22];		break;
			case "m_fPrefabScale":						tooltip	= HsEditor_NcParticleSystem[GetLang(), 23];		break;
			case "m_fPrefabSpeed":						tooltip	= HsEditor_NcParticleSystem[GetLang(), 24];		break;
			case "m_fPrefabLifeTime":					tooltip	= HsEditor_NcParticleSystem[GetLang(), 25];		break;

			case "Select Prefab":						tooltip	= HsEditor_NcParticleSystem[GetLang(), 26];		break;
			case "Clear Prefab":						tooltip	= HsEditor_NcParticleSystem[GetLang(), 27];		break;
			case "Open Prefab":							tooltip	= HsEditor_NcParticleSystem[GetLang(), 28];		break;

			case "Delete Shuriken Components":			tooltip	= "- Delete Component: ParticleSystem/ParticleSystemRenderer";		break;
			case "Add ParticleSystemRenderer":			tooltip	= "- Add Component: ParticleSystemRenderer";		break;
			case "Delete Legacy(Ellipsoid) Components":	tooltip	= "- Delete Component: EllipsoidParticleEmitter/ParticleAnimator/ParticleRenderer";	break;
			case "Delete Legacy(Mesh) Components":		tooltip	= "- Delete Component: MeshParticleEmitter/ParticleAnimator/ParticleRenderer";		break;

			case "Add ParticleAnimator":				tooltip	= "- Add Component: ParticleAnimator";		break;
			case "Add ParticleRenderer":				tooltip	= "- Add Component: ParticleRenderer";		break;

			case "Add Shuriken Components":				tooltip	= "- Add Component: ParticleSystem/ParticleSystemRenderer";		break;
			case "Add Legacy(Ellipsoid) Components":	tooltip	= "- Add Component: EllipsoidParticleEmitter/ParticleAnimator/ParticleRenderer";	break;
			case "Add Legacy(Mesh) Components":			tooltip	= "- Add Component: MeshParticleEmitter/ParticleAnimator/ParticleRenderer";			break;

			case "Convert To Static Scale":				tooltip	= HsEditor_NcParticleSystem[GetLang(), 29];		break;
			case "Convert: OneShot To FXMakerBurst":	tooltip	= HsEditor_NcParticleSystem[GetLang(), 30];		break;
				
			default: return "NcParticleSystem\n" + HsEditor_NcParticleSystem[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcRotation ===================================================================================
	protected static string[,]	HsEditor_NcRotation = {
								{
									 "- Rotate gameObject at the assigned speed."
									,"- true: World Space" + "\n" +
									 "- false: Local Space"
									,"- the speed of rotation"
								}
								,
								{
									 "- gameObject를 지정한 속도로 회전 시킨다."
									,"- true: World Space" + "\n" +
									 "- false: Local Space"
									,"- 회전 속도"
								}
							  };
	public static string GetHsEditor_NcRotation(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcRotation);
		string	tooltip = "";

		switch (caption)
		{
			case "m_bWorldSpace":			tooltip	= HsEditor_NcRotation[GetLang(), 1];		break;
			case "m_vRotationValue":		tooltip	= HsEditor_NcRotation[GetLang(), 2];		break;
			default: return "NcRotation\n" + HsEditor_NcRotation[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcSpriteFactory ===================================================================================
	public static string GetHsEditor_NcSpriteFactory(string caption)
	{
		if (m_bTDisable)	return " ";

		string[,]	helpStr = {
								{
									"- "
								},{
									"- "
								}
							  };
//  		CheckValid(helpStr);
//  		string	tooltip = "";

		switch (caption)
		{
			default: return "NcSpriteFactory\n" + helpStr[GetLang(), 0];
		}
//  		return caption + "\n" + tooltip;
	}

	// Editor NcSpriteTexture ===================================================================================
	protected static string[,]	HsEditor_NcSpriteTexture = {
								{
									 "- Bring AtlasTexture from SpriteFactory and print." + "\n" +
									 "- Selectively print partial texture by using Atlas information."
									,"-  Select Prefab which includes SpriteFactory.cs." + "\n" +
									 "- When not selected, Use SpriteFactory from CurrentGameObject." + "\n" +
									 "- Used when SpriteFactory is needed to be shared."
									,"- Print specific Sprite which was registered at NcSpriteFactory."
									,"- Select Prefab which includes SpriteFactory.cs. (Has to be included by RootGameObject)."
									,"- Remove selected SpriteFactoryPrefab."
								},{
									 "- SpriteFactory에서 AtlasTexture를 가져와 출력 합니다." + "\n" +
									 "- Atlas정보를 이용하여 선택적으로 일부텍스쳐만 출력 합니다."
									,"- SpriteFactory.cs를 포함 하고 있는 Prefab을 선택 합니다." + "\n" +
									 "- 선택하지 않을 경우 CurrentGameObject에 있는 SpriteFactory.cs를 사용 합니다." + "\n" +
									 "- SpriteFactory를 공유 해야할 경우에 사용 됩니다."
									,"- NcSpriteFactory에서 등록한 특정 Sprite를 선택하여 출력 합니다."
									,"- SpriteFactory.cs를 포함하고 있는 Prefab을 선택합니다. (RootGameObject에 포함되어 있어야 한다)"
									,"- 선택된 SpriteFactoryPrefab을 제거 합니다."
								}
							  };
	public static string GetHsEditor_NcSpriteTexture(string caption)
	{
		if (m_bTDisable)	return " ";

 		CheckValid(HsEditor_NcSpriteTexture);
 		string	tooltip = "";

		switch (caption)
		{
			case "m_NcSpriteFactoryPrefab":		tooltip	= HsEditor_NcSpriteTexture[GetLang(), 1];		break;
			case "m_nSpriteListIndex":			tooltip	= HsEditor_NcSpriteTexture[GetLang(), 2];		break;
			case "Select SpriteFactory":		tooltip	= HsEditor_NcSpriteTexture[GetLang(), 3];		break;
			case "Clear SpriteFactory":			tooltip	= HsEditor_NcSpriteTexture[GetLang(), 4];		break;

			default: return "NcSpriteTexture\n" + HsEditor_NcSpriteTexture[GetLang(), 0];
		}
 		return caption + "\n" + tooltip;
	}

	// Editor NcTilingTexture ===================================================================================
	protected static string[,]	HsEditor_NcTilingTexture = {
								{
									 "- Do Tiling Material.Texture."
									,"- The number of widthwise repetitions of Material.Texture"
									,"- The number of lengthwise repetitions of Material.Texture"
									,"- Widthwise Offset of Material.Texture"
									,"- Lengthwise Offset of Material.Texture"
									,"- Though Scale of GameObject changes, maintain Texture which became Tiling in order not to increase or decrease."
								}
								,
								{
									 "- Material.Texture를 Tiling한다."
									,"- Material.Texture의 가로 반복 수"
									,"- Material.Texture의 세로 반복 수"
									,"- Material.Texture의 가로 Offset"
									,"- Material.Texture의 세로 Offset"
									,"- GameObject의 Scale이 바뀌어도 Tiling된 Texture가 늘거나 줄지 않도록 유지한다."
								}
							  };
	public static string GetHsEditor_NcTilingTexture(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcTilingTexture);
		string	tooltip = "";

		switch (caption)
		{
			case "m_fTilingX":			tooltip	= HsEditor_NcTilingTexture[GetLang(), 1];		break;
			case "m_fTilingY":			tooltip	= HsEditor_NcTilingTexture[GetLang(), 2];		break;
			case "m_fOffsetX":			tooltip	= HsEditor_NcTilingTexture[GetLang(), 3];		break;
			case "m_fOffsetY":			tooltip	= HsEditor_NcTilingTexture[GetLang(), 4];		break;
			case "m_bFixedTileSize":	tooltip	= HsEditor_NcTilingTexture[GetLang(), 5];		break;
			default: return "NcTilingTexture\n" + HsEditor_NcTilingTexture[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

	// Editor NcTrailTexture ===================================================================================
	protected static string[,]	HsEditor_NcTrailTexture = {
								{
									 "- "
									,"- Select Prefab which includes ParticleEmiter." + "\n" +
									 "- When not selected, Use ParticleEmiter from CurrentGameObject."
									,"- Select Prefab which includes ParticleEmiter. (Has to be included by RootGameObject)."
									,"- Remove selected ParticlePrefab."
								},{
									 "- ParticleEmiter를 사용하여 다양한 회전 파티클을 구현 합니다."
									,"- ParticleEmiter를 포함 하고 있는 Prefab을 선택 합니다." + "\n" +
									 "- 선택하지 않을 경우 CurrentGameObject에 있는 ParticleEmiter를 사용 합니다."
									,"- ParticleEmiter를 포함하고 있는 Prefab을 선택합니다. (RootGameObject에 포함되어 있어야 한다)"
									,"- 선택된 ParticlePrefab을 제거 합니다."
								}
							  };
	public static string GetHsEditor_NcTrailTexture(string caption)
	{
		if (m_bTDisable)	return " ";

 		CheckValid(HsEditor_NcTrailTexture);
 		string	tooltip = "";

		switch (caption)
		{
			case "m_ParticlePrefab":			tooltip	= HsEditor_NcTrailTexture[GetLang(), 1];		break;

			default: return "NcTrailTexture\n" + HsEditor_NcTrailTexture[GetLang(), 0];
		}
 		return caption + "\n" + tooltip;
	}

	// Editor NcUvAnimation ===================================================================================
	protected static string[,]	HsEditor_NcUvAnimation = {
								{
									 "- Have Material.Texture UVanimation." + "\n" +
									 "- Because it is Material which became Instance, the operation will not affect other Material."
									,"- X Speed"
									,"- Y Speed"
									,"- The number of X repetitions of Material.Texture"
									,"- The number of Y repetitions of Material.Texture"
									,"- X starting Offset of Material.Texture"
									,"- Y starting Offset of Material.Texture"
									,"- Though Scale of GameObject changes, maintain Texture which became Tiling in order not to increase or decrease." + "\n" +
									 "- Only activated when the value of Speed is not 0 and xy-axis of mesh is equal to xy-axis of uv"
									,"- true: Repeat Material.Texture." + "\n" +
									 "- false: Animate Material.Texture once. (Stop if x or y is satisfied)"
									,"- GameObject will be destroy after Animation ends."
								}
								,
								{
									 "- Material.Texture를 UV애니메이션 한다." + "\n" +
									 "- Instance된 Material이므로 다른 Material에 영향을 주지 않는다."
									,"- X 스피드"
									,"- Y 스피드"
									,"- Material.Texture의 X 반복 수"
									,"- Material.Texture의 Y 반복 수"
									,"- Material.Texture의 X 시작 Offset"
									,"- Material.Texture의 Y 시작 Offset"
									,"- GameObject의 Scale이 바뀌어도 Tiling된 Texture가 늘거나 줄지 않도록 유지한다." + "\n" +
									 "- Speed값이 0이 아니고, mesh의 xy와 uv의 xy축이 같을 때만 작동 됨"
									,"- true: Material.Texture를 Repeat 한다." + "\n" +
									 "- false: Material.Texture를 한번만 애니메이션 시킨다.(xy 둘중 하나라도 만족하면 중단)"
								    ,"- 반복이 아닐때만 사용가능하며, 한번의 loop가 끝나면 자동 소멸한다."
								}
							  };
	public static string GetHsEditor_NcUvAnimation(string caption)
	{
		if (m_bTDisable)	return " ";

		CheckValid(HsEditor_NcUvAnimation);
		string	tooltip = "";

		switch (caption)
		{
			case "m_fScrollSpeedX":		tooltip	= HsEditor_NcUvAnimation[GetLang(), 1];		break;
			case "m_fScrollSpeedY":		tooltip	= HsEditor_NcUvAnimation[GetLang(), 2];		break;
			case "m_fTilingX":			tooltip	= HsEditor_NcUvAnimation[GetLang(), 3];		break;
			case "m_fTilingY":			tooltip	= HsEditor_NcUvAnimation[GetLang(), 4];		break;
			case "m_fOffsetX":			tooltip	= HsEditor_NcUvAnimation[GetLang(), 5];		break;
			case "m_fOffsetY":			tooltip	= HsEditor_NcUvAnimation[GetLang(), 6];		break;
			case "m_bFixedTileSize":	tooltip	= HsEditor_NcUvAnimation[GetLang(), 7];		break;
			case "m_bRepeat":			tooltip	= HsEditor_NcUvAnimation[GetLang(), 8];		break;
			case "m_bAutoDestruct":		tooltip	= HsEditor_NcUvAnimation[GetLang(), 9];		break;
			default: return "NcUvAnimation\n" + HsEditor_NcUvAnimation[GetLang(), 0];
		}
		return caption + "\n" + tooltip;
	}

}
#endif

