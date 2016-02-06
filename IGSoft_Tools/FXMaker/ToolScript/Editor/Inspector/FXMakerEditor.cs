// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------


// Property -------------------------------------------------------------------------
// Loop Function --------------------------------------------------------------------
// Control Function -----------------------------------------------------------------
// Event Function -------------------------------------------------------------------
// --------------------------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class FXMakerEditor : Editor
{
	// Attribute ------------------------------------------------------------------------
	protected	string				m_LastTooltip	= "";
	protected	float				m_fButtonHeight	= 23;
	protected	FXMakerUndoManager	m_UndoManager;

	// ----------------------------------------------------------------------------------
	protected FXMakerMain GetFXMakerMain()
	{
		// check instance
		if ((target is Component) && (target as Component).GetComponent<FxmInfoIndexing>())
			return null;

		// find toolmain
		GameObject fxMaker = GameObject.Find("_FXMaker");
		if (Application.isPlaying && fxMaker != null)
			return fxMaker.GetComponent<FXMakerMain>();
		return null;
	}

	protected FxmPopupManager GetFxmPopupManager()
	{
		GameObject fxMaker = GameObject.Find("_FXMaker");
		if (Application.isPlaying && fxMaker != null && fxMaker.GetComponent("FXMakerMain") != null)
			return fxMaker.GetComponentInChildren<FxmPopupManager>();
		return null;
	}

	protected string GetScriptName(Component com)
	{
		string	name	= com.ToString();
		int		idx		= 0;

		for (int n = 0; n < name.Length; n++)
		{
			if (name[n] == '(')
				idx = n;
		}
		int		start	= name.IndexOf('(', idx);
		int		end		= name.IndexOf(')', idx);
		return name.Substring(start+1, end-start-1);
	}

	protected void AddScriptNameField(Component com)
	{
		EditorGUILayout.TextField(new GUIContent("Script", "Script"), GetScriptName(com));
	}

	protected virtual void HelpBox(string caption)
	{
		GUILayout.Space(10);
		GUILayout.TextArea(caption, GUILayout.Height(130));
		GUILayout.Space(20);
	}

	protected virtual void WarringBox(string caption)
	{
 		FXMakerLayout.GUIColorBackup(Color.red);
		GUILayout.TextArea(caption, GUILayout.Height(80));
		FXMakerLayout.GUIColorRestore();
	}

	protected GUIContent GetHelpContent(string caption, string text)
	{
		if (2 < caption.Length)
			if (caption.Substring(0, 2) == "m_")
				caption = caption.Substring(2);
		return new GUIContent(caption, text);
	}

	protected GUIContent GetCommonContent(string caption)
	{
		string text = FXMakerTooltip.GetHsToolInspector(caption);
		if (2 < caption.Length)
			if (caption.Substring(0, 2) == "m_")
				caption = caption.Substring(2);
		return new GUIContent(caption, text);
	}

	protected void OnEditComponent()
	{
		GetFXMakerMain().GetComponent<FXMakerEffect>().SetChangePrefab();
		GetFXMakerMain().CreateCurrentInstanceEffect(true);
	}

	// --------------------------------------------------------------------------------------------------
	protected void SetMinValue(ref float value, float min)
	{
		if (value < min)
			value = min;
	}

	protected void SetMinValue(ref int value, int min)
	{
		if (value < min)
			value = min;
	}

	protected void SetMaxValue(ref float value, float max)
	{
		if (max < value)
			value = max;
	}

	protected void SetMaxValue(ref int value, int max)
	{
		if (max < value)
			value = max;
	}

	protected float GetDrawTextureHeight(Renderer render)
	{
		float fDrawHeight = 250;
		if (render == null || render.sharedMaterial == null || render.sharedMaterial.mainTexture == null)
			return 50;
		fDrawHeight *= render.sharedMaterial.mainTexture.height / (float)render.sharedMaterial.mainTexture.width;
		return fDrawHeight;
	}

	protected bool DrawTrimTexture(Rect drawRect, bool bSelEnable, Material srcMat, NcSpriteFactory ncSpriteFactory, int nSelFactoryIndex, int nSelFrameIndex, bool bRotateRight, out int nClickFactoryIndex, out int nClickFrameIndex)
	{
//		FXMakerLayout.GetOffsetRect(rect, 0, 5, 0, -5);
		Texture srcTex = srcMat.mainTexture;
		GUI.DrawTexture(drawRect, srcTex, ScaleMode.StretchToFill, true);

		Event	ev	= Event.current;

		for (int n = 0; n < ncSpriteFactory.GetSpriteNodeCount(); n++)
		{
			NcSpriteFactory.NcSpriteNode	ncSpriteNode = ncSpriteFactory.GetSpriteNode(n);
			if (ncSpriteNode.IsEmptyTexture())
				continue;

			for (int fc = 0; fc < ncSpriteNode.m_nFrameCount; fc++)
			{
				Color	color;
				Rect	uvRect	= ncSpriteFactory.GetSpriteUvRect(n, fc);
				// draw indexRect
				Rect currRect = new Rect(drawRect.x+uvRect.xMin*drawRect.width, drawRect.y+(1-uvRect.yMin-uvRect.height)*drawRect.height, uvRect.width*drawRect.width, uvRect.height*drawRect.height);
				if (nSelFactoryIndex == n)
				{
					color = (nSelFrameIndex == fc ? Color.green : Color.red);
				} else {
					color = Color.yellow;
				}
				NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(currRect, 0), color, 1, false);

				// Change selIndex
				if (bSelEnable && (ev.type == EventType.MouseDown) && currRect.Contains(ev.mousePosition))
				{
//					m_Sel.SetSpriteFactoryIndex(n, fc, false);
					nClickFactoryIndex	= n;
					nClickFrameIndex	= fc;
					// Rotate
					if (bRotateRight && ev.button == 1)
						(target as NcEffectBehaviour).transform.Rotate(0, 180, 0);
					Repaint();
					return true;
				}
			}
		}
		nClickFactoryIndex	= 0;
		nClickFrameIndex	= 0;
		return false;
	}

	protected bool DrawTileTexture(Rect drawRect, bool bSelEnable, Material srcMat, int nTilingX, int nTilingY, int nStartFrame, int nFrameCount, int nSelFrameIndex, out int nClickFrameIndex)
	{
		GUI.DrawTexture(drawRect, srcMat.mainTexture, ScaleMode.StretchToFill, true);
		Event	ev	= Event.current;

		Vector2	mousePos	= ev.mousePosition;
		Rect	calRect		= drawRect;

		mousePos.x -= calRect.x;
		calRect.x = 0;
		mousePos.y -= calRect.y;
		calRect.y = 0;

		float	tileWidth		= (calRect.width  / nTilingX);
		float	tileHeight		= (calRect.height / nTilingY);
//		int		absSelectPos	= m_Sel.m_nStartFrame + m_Sel.m_nSelectFrame;

		for (int n = nStartFrame; n < Mathf.Min(nStartFrame + nFrameCount, nTilingX * nTilingY); n++)
		{
			int posx = n % nTilingX;
			int posy = n / nTilingX;

			// draw current
			Rect selRect = new Rect(drawRect.x+posx*tileWidth, drawRect.y+posy*tileHeight, tileWidth, tileHeight);
			NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(selRect, -1), ((nStartFrame+nSelFrameIndex == n && bSelEnable) ? Color.green : Color.red), 1, false);

			// Change selIndex
			if (bSelEnable && (ev.type == EventType.MouseDown && ev.button == 0) && selRect.Contains(ev.mousePosition))
			{
				nClickFrameIndex = n - nStartFrame;
				Repaint();
				return true;
			}
		}
		nClickFrameIndex = 0;
		return false;
	}

	protected bool DrawTrimTexture(Rect drawRect, bool bSelEnable, Material srcMat, NcSpriteFactory.NcFrameInfo[] ncSpriteFrameInfos, int nStartFrame, int nFrameCount, int nSelFrameIndex, out int nClickFrameIndex)
	{
		nClickFrameIndex = 0;
		GUI.DrawTexture(drawRect, srcMat.mainTexture, ScaleMode.StretchToFill, true);

		if (ncSpriteFrameInfos == null)
			return false;
		Event	ev	= Event.current;

		if (ncSpriteFrameInfos != null)
		{
			for (int n = Mathf.Max(nStartFrame, 0); n < Mathf.Min(nStartFrame + nFrameCount, ncSpriteFrameInfos.Length); n++)
			{
				Rect uvRect	= ncSpriteFrameInfos[n].m_TextureUvOffset;
				// draw indexRect
				Rect currRect = new Rect(drawRect.x+uvRect.xMin*drawRect.width, drawRect.y+(1-uvRect.yMin-uvRect.height)*drawRect.height, uvRect.width*drawRect.width, uvRect.height*drawRect.height);
				NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(currRect, 0), ((nStartFrame+nSelFrameIndex == n && bSelEnable) ? Color.green : Color.red), 1, false);

				// Change selIndex
				if (bSelEnable && (ev.type == EventType.MouseDown && ev.button == 0) && currRect.Contains(ev.mousePosition))
				{
					nClickFrameIndex = n - nStartFrame;
					Repaint();
					return true;
				}
			}
		}
		return false;
	}

	// --------------------------------------------------------------------------------------------------
	protected static LayerMask LayerMaskField(GUIContent con, LayerMask selected)
	{
	    List<string>	layers		 = new List<string>();

		for (int i=0; i < 32; i++)
		{
			string layerName = LayerMask.LayerToName(i);
			if (layerName != "")
				layers.Add(layerName);
		}

		selected = EditorGUILayout.MaskField(con, selected, layers.ToArray(), EditorStyles.layerMaskField);
		return selected;
	}
}


