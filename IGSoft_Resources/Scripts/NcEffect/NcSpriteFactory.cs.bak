// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NcSpriteFactory : NcEffectBehaviour
{
	// class ------------------------------------------------------------------------
	[System.Serializable]
	public class NcFrameInfo
	{
		public		int				m_nFrameIndex;
		public		bool			m_bEmptyFrame;
		public		int				m_nTexWidth;
		public		int				m_nTexHeight;
		public		Rect			m_TextureUvOffset;
		public		Rect			m_FrameUvOffset;
		public		Vector2			m_FrameScale;				// texture / 128
		public		Vector2			m_scaleFactor;
	}
	[SerializeField]

	[System.Serializable]
	public class NcSpriteNode
	{
		public		bool			m_bIncludedAtlas	= true;
		public		string			m_TextureGUID		= "";
 		public		string			m_TextureName		= "";
		public		float			m_fMaxTextureAlpha	= 1.0f;
		public		string			m_SpriteName		= "";

		// Sprite
		public		NcFrameInfo[]	m_FrameInfos		= null;
		public		int				m_nTilingX			= 1;
		public		int				m_nTilingY			= 1;
		public		int				m_nStartFrame		= 0;
		public		int				m_nFrameCount		= 1;

		// NcSpriteAnimation
		public		bool			m_bLoop				= false;
		public		int				m_nLoopStartFrame	= 0;
		public		int				m_nLoopFrameCount	= 0;
		public		int				m_nLoopingCount		= 0;
		public		float			m_fFps				= 20;
		public		float			m_fTime				= 0;

		// char animation
		public		int				m_nNextSpriteIndex	= -1;
		public		int				m_nTestMode			= 0;
		public		float			m_fTestSpeed		= 1;
		// char effect
		public		bool			m_bEffectInstantiate= true;
		public		GameObject		m_EffectPrefab		= null;
		public		int				m_nSpriteFactoryIndex	= -1;
		public		int				m_nEffectFrame		= 0;
		public		bool			m_bEffectOnlyFirst	= true;
		public		bool			m_bEffectDetach		= true;
		public		float			m_fEffectSpeed		= 1;
		public		float			m_fEffectScale		= 1;
		public		Vector3			m_EffectPos			= Vector3.zero;
		public		Vector3			m_EffectRot			= Vector3.zero;
		// char sound
		public		AudioClip		m_AudioClip			= null;
		public		int				m_nSoundFrame		= 0;
		public		bool			m_bSoundOnlyFirst	= true;
		public		bool			m_bSoundLoop		= false;
		public		float			m_fSoundVolume		= 1;
		public		float			m_fSoundPitch		= 1;

		// function
		public NcSpriteNode GetClone()
		{
			return null;
		}

		public int GetStartFrame()
		{
			if (m_FrameInfos == null || m_FrameInfos.Length == 0)
				return 0;
			return m_FrameInfos[0].m_nFrameIndex;
		}

		public void SetEmpty()
		{
			m_FrameInfos = null;
			m_TextureGUID	= "";
		}

		public bool IsEmptyTexture()
		{
			return m_TextureGUID == "";
		}

		public bool IsUnused()
		{
			return (m_bIncludedAtlas == false);
		}
	}
	[SerializeField]

	// Attribute ------------------------------------------------------------------------
	public		enum	MESH_TYPE		{BuiltIn_Plane, BuiltIn_TwosidePlane};
	public		enum	ALIGN_TYPE		{TOP, CENTER, BOTTOM, LEFTCENTER, RIGHTCENTER};
	public		enum	SPRITE_TYPE		{NcSpriteTexture, NcSpriteAnimation, Auto};

	public		SPRITE_TYPE				m_SpriteType		= SPRITE_TYPE.Auto;
	public		List<NcSpriteNode>		m_SpriteList;
	public		int						m_nCurrentIndex;
// 	public		Material				m_AtlasMaterial;
	public		int						m_nMaxAtlasTextureSize	= 2048;

	public		bool					m_bNeedRebuild		= true;
	public		int						m_nBuildStartIndex	= 0;

	// NcUvAnimation
	public		bool					m_bTrimBlack		= true;
	public		bool					m_bTrimAlpha		= true;
	public		float					m_fUvScale			= 1;
	public		float					m_fTextureRatio		= 1;

	public		GameObject				m_CurrentEffect;
	public		NcAttachSound			m_CurrentSound;

	// Internal
	protected	bool					m_bEndSprite		= true;

	// ShowOption
	public		enum SHOW_TYPE			{NONE, ALL, SPRITE, ANIMATION, EFFECT};
	public		SHOW_TYPE				m_ShowType			= SHOW_TYPE.SPRITE;
	public		bool					m_bShowEffect		= true;
	public		bool					m_bTestMode			= true;
	public		bool					m_bSequenceMode		= false;

	protected	bool					m_bbInstance		= false;

//	[HideInInspector]

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		return "";	// no error
	}
#endif

// 	public float GetDurationTime()
// 	{
// 		return (m_PlayMode == PLAYMODE.PINGPONG ? m_nFrameCount*2-2 : m_nFrameCount) / m_fFps;
// 	}
// 
// 	public int GetShowIndex()
// 	{
// 		return m_nLastIndex + m_nStartFrame;
// 	}

	public bool IsUnused(int nNodeIndex)
	{
		return (m_SpriteList[nNodeIndex].IsUnused() || nNodeIndex < m_nBuildStartIndex);
	}

	public NcSpriteNode GetSpriteNode(int nIndex)
	{
		if (m_SpriteList == null || nIndex < 0 || m_SpriteList.Count <= nIndex)
			return null;
		return m_SpriteList[nIndex] as NcSpriteNode;
	}

	public NcSpriteNode GetSpriteNode(string spriteName)
	{
		if (m_SpriteList == null)
			return null;
		foreach (NcSpriteNode spriteNode in m_SpriteList)
			if (spriteNode.m_SpriteName == spriteName)
				return spriteNode;
		return null;
	}

	public int GetSpriteNodeIndex(string spriteName)
	{
		if (m_SpriteList == null)
			return -1;
		for (int n = 0; n < m_SpriteList.Count; n++)
			if (m_SpriteList[n].m_SpriteName == spriteName)
				return n;
		return -1;
	}

	public NcSpriteNode SetSpriteNode(int nIndex, NcSpriteNode newInfo)
	{
		if (m_SpriteList == null || nIndex < 0 || m_SpriteList.Count <= nIndex)
			return null;
		NcSpriteNode	oldSpriteNode = m_SpriteList[nIndex] as NcSpriteNode;
		m_SpriteList[nIndex] = newInfo;
		return oldSpriteNode;
	}

	public int AddSpriteNode()
	{
		NcSpriteNode	SpriteNode	= new NcSpriteNode();

		if (m_SpriteList == null)
			m_SpriteList = new List<NcSpriteNode>();
		m_SpriteList.Add(SpriteNode);
// 		m_bNeedRebuild = true;
		return m_SpriteList.Count-1;
	}

	public int AddSpriteNode(NcSpriteNode addSpriteNode)
	{
		if (m_SpriteList == null)
			m_SpriteList = new List<NcSpriteNode>();
		m_SpriteList.Add(addSpriteNode.GetClone());
		m_bNeedRebuild = true;
		return m_SpriteList.Count-1;
	}

	public void DeleteSpriteNode(int nIndex)
	{
		if (m_SpriteList == null || nIndex < 0 || m_SpriteList.Count <= nIndex)
			return;
		m_bNeedRebuild = true;
		m_SpriteList.Remove(m_SpriteList[nIndex]);
	}

	public void MoveSpriteNode(int nSrcIndex, int nTarIndex)
	{
		NcSpriteNode node = m_SpriteList[nSrcIndex];
		m_SpriteList.Remove(node);
		m_SpriteList.Insert(nTarIndex, node);
	}

	public void ClearAllSpriteNode()
	{
		if (m_SpriteList == null)
			return;
		m_bNeedRebuild = true;
		m_SpriteList.Clear();
	}

	public int GetSpriteNodeCount()
	{
		if (m_SpriteList == null)
			return 0;
		return m_SpriteList.Count;
	}

	public NcSpriteNode GetCurrentSpriteNode()
	{
		if (m_SpriteList == null || m_SpriteList.Count <= m_nCurrentIndex)
			return null;
		return m_SpriteList[m_nCurrentIndex];
	}

	public Rect GetSpriteUvRect(int nStriteIndex, int nFrameIndex)
	{
//   		Debug.Log("GetSpriteUvRect() - m_SpriteList.Count = " + m_SpriteList.Count + ", nStriteIndex = " + nStriteIndex + ", nFrameIndex = " + nFrameIndex);
		if (m_SpriteList.Count <= nStriteIndex || m_SpriteList[nStriteIndex].m_FrameInfos == null || m_SpriteList[nStriteIndex].m_FrameInfos.Length <= nFrameIndex)
			return new Rect(0,0,0,0);
		return m_SpriteList[nStriteIndex].m_FrameInfos[nFrameIndex].m_TextureUvOffset;
	}

	public bool IsValidFactory()
	{
		if (m_bNeedRebuild)
			return false;
		return true;
	}

	// Loop Function --------------------------------------------------------------------
	void Awake()
	{
		m_bbInstance	= true;
	}

	// Control Function -----------------------------------------------------------------
	public NcEffectBehaviour SetSprite(int nNodeIndex)
	{
		return SetSprite(nNodeIndex, true);
	}

	public NcEffectBehaviour SetSprite(string spriteName)
	{
		if (m_SpriteList == null)
			return null;
		int nCnt = 0;
		foreach (NcSpriteNode spriteNode in m_SpriteList)
		{
			if (spriteNode.m_SpriteName == spriteName)
				return SetSprite(nCnt, true);
			nCnt++;
		}
		return null;
	}

	public NcEffectBehaviour SetSprite(int nNodeIndex, bool bRunImmediate)
	{
		if (m_SpriteList == null || nNodeIndex < 0 || m_SpriteList.Count <= nNodeIndex)
			return null;

		if (bRunImmediate)
			OnChangingSprite(m_nCurrentIndex, nNodeIndex);
		m_nCurrentIndex = nNodeIndex;

// 		if (m_SpriteType == SPRITE_TYPE.NcSpriteAnimation)
		NcSpriteAnimation spriteCom = GetComponent<NcSpriteAnimation>();
		if (spriteCom != null)
		{
			spriteCom.SetSpriteFactoryIndex(nNodeIndex, false);

			if (bRunImmediate)
				spriteCom.ResetAnimation();
		}
// 		if (m_SpriteType == SPRITE_TYPE.NcSpriteTexture)
		NcSpriteTexture uvCom	= GetComponent<NcSpriteTexture>();
		if (uvCom != null)
		{
			uvCom.SetSpriteFactoryIndex(nNodeIndex, -1, false);
			if (bRunImmediate)
			{
//				UpdateUvScale(nNodeIndex, transform);
				CreateEffectObject();
			}
		}

		if (spriteCom != null)
			return spriteCom;
		if (spriteCom != null)
			return uvCom;

		return null;
	}

	public int GetCurrentSpriteIndex()
	{
		return m_nCurrentIndex;
	}

	public bool IsEndSprite()
	{
		if (m_SpriteList == null || m_nCurrentIndex < 0 || m_SpriteList.Count <= m_nCurrentIndex)
			return true;
		if (IsUnused(m_nCurrentIndex) || m_SpriteList[m_nCurrentIndex].IsEmptyTexture())
			return true;
		return m_bEndSprite;
	}

	void CreateEffectObject()
	{
		if (m_bbInstance == false)
			return;
		if (m_bShowEffect == false)
			return;
		DestroyEffectObject();
// 		Debug.Log("CreateEffectObject() - new = " + ncSpriteNode.m_EffectPrefab);

		// Notify EffectFrame
		if (transform.parent != null)
			transform.parent.SendMessage("OnSpriteListEffectFrame", m_SpriteList[m_nCurrentIndex], SendMessageOptions.DontRequireReceiver);

		if (m_SpriteList[m_nCurrentIndex].m_bEffectInstantiate)
		{
			m_CurrentEffect = CreateSpriteEffect(m_nCurrentIndex, transform);
			// Notify CreateEffectInstance
			if (transform.parent != null)
				transform.parent.SendMessage("OnSpriteListEffectInstance", m_CurrentEffect, SendMessageOptions.DontRequireReceiver);
		}
	}

// 	public void UpdateUvScale(int nSrcSpriteIndex, Transform targetTrans)
// 	{
// 		Vector3	scale;
// 		if (1 <= m_fTextureRatio)
// 			 scale = new Vector3(m_SpriteList[nSrcSpriteIndex].m_UvRect.width * m_fUvScale * m_fTextureRatio, m_SpriteList[nSrcSpriteIndex].m_UvRect.height * m_fUvScale, 1);
// 		else scale = new Vector3(m_SpriteList[nSrcSpriteIndex].m_UvRect.width * m_fUvScale, m_SpriteList[nSrcSpriteIndex].m_UvRect.height * m_fUvScale / m_fTextureRatio, 1);
// 		targetTrans.localScale	= Vector3.Scale(targetTrans.localScale, scale);
// 	}

	public GameObject CreateSpriteEffect(int nSrcSpriteIndex, Transform parentTrans)
	{
		GameObject createEffect = null;

		if (m_SpriteList[nSrcSpriteIndex].m_EffectPrefab != null)
		{
			// Create BaseGameObject
			createEffect	= CreateGameObject("Effect_" + m_SpriteList[nSrcSpriteIndex].m_EffectPrefab.name);
			if (createEffect == null)
				return null;

			// Change Parent
			ChangeParent(parentTrans, createEffect.transform, true, null);

			NcAttachPrefab	attachCom		= createEffect.AddComponent<NcAttachPrefab>();
// 			attachCom.m_fDelayTime			= 0;
			attachCom.m_AttachPrefab		= m_SpriteList[nSrcSpriteIndex].m_EffectPrefab;
			attachCom.m_fPrefabSpeed		= m_SpriteList[nSrcSpriteIndex].m_fEffectSpeed;
			attachCom.m_bDetachParent		= m_SpriteList[nSrcSpriteIndex].m_bEffectDetach;
			attachCom.m_nSpriteFactoryIndex	= m_SpriteList[nSrcSpriteIndex].m_nSpriteFactoryIndex;
			attachCom.UpdateImmediately();

// 			createEffect.transform.localScale *= ncSpriteNode.m_fEffectScale;
// 			createEffect.transform.Translate(ncSpriteNode.m_EffectPos, Space.Self);
// 			createEffect.transform.Rotate(ncSpriteNode.m_EffectRot, Space.Self);
			createEffect.transform.localScale	*= m_SpriteList[nSrcSpriteIndex].m_fEffectScale;
			createEffect.transform.localPosition += m_SpriteList[nSrcSpriteIndex].m_EffectPos;
			createEffect.transform.localRotation *= Quaternion.Euler(m_SpriteList[nSrcSpriteIndex].m_EffectRot);
		}
		return createEffect;
	}

	void DestroyEffectObject()
	{
//  		Debug.Log("DestroyEffectObject - " + m_CurrentEffect);
 		if (m_CurrentEffect != null)
 			Destroy(m_CurrentEffect);
		m_CurrentEffect = null;
	}

	void CreateSoundObject(NcSpriteNode ncSpriteNode)
	{
// 		Debug.Log("CreateSoundObject");
		if (m_bShowEffect == false)
			return;

		if (ncSpriteNode.m_AudioClip != null)
		{
			if (m_CurrentSound == null)
				m_CurrentSound = gameObject.AddComponent<NcAttachSound>();

			m_CurrentSound.m_AudioClip	= ncSpriteNode.m_AudioClip;
			m_CurrentSound.m_bLoop		= ncSpriteNode.m_bSoundLoop;
			m_CurrentSound.m_fVolume	= ncSpriteNode.m_fSoundVolume;
			m_CurrentSound.m_fPitch		= ncSpriteNode.m_fSoundPitch;
			m_CurrentSound.enabled		= true;
			m_CurrentSound.Replay();
		}
	}

	// Event Function -------------------------------------------------------------------
	// 변경 중일때...
	public void OnChangingSprite(int nOldNodeIndex, int nNewNodeIndex)
	{
// 		Debug.Log("OnChangingSprite() - nOldNodeIndex = " + nOldNodeIndex + ", nNewNodeIndex = " + nNewNodeIndex);

		m_bEndSprite = false;
		DestroyEffectObject();
	}

	// 첫 frame 시작할때...
	public void OnAnimationStartFrame(NcSpriteAnimation spriteCom)
	{
	}

	// frame 변경될 때 마다... (frame skip 되기도 함)
	public void OnAnimationChangingFrame(NcSpriteAnimation spriteCom, int nOldIndex, int nNewIndex, int nLoopCount)
	{
// 		Debug.Log("OnAnimationChangingFrame() - nOldIndex = " + nOldIndex + ", nNewIndex = " + nNewIndex + ", nLoopCount = " + nLoopCount + ", m_nCurrentIndex = " + m_nCurrentIndex);
		if (m_SpriteList.Count <= m_nCurrentIndex)
			return;

		if (m_SpriteList[m_nCurrentIndex].m_EffectPrefab != null)
		{
			if ((nOldIndex < m_SpriteList[m_nCurrentIndex].m_nEffectFrame || nNewIndex <= nOldIndex) && m_SpriteList[m_nCurrentIndex].m_nEffectFrame <= nNewIndex)
				if (nLoopCount == 0 || m_SpriteList[m_nCurrentIndex].m_bEffectOnlyFirst == false)
					CreateEffectObject();
		}
		if (m_SpriteList[m_nCurrentIndex].m_AudioClip != null)
		{
			if ((nOldIndex < m_SpriteList[m_nCurrentIndex].m_nSoundFrame || nNewIndex <= nOldIndex)  && m_SpriteList[m_nCurrentIndex].m_nSoundFrame <= nNewIndex)
				if (nLoopCount == 0 || m_SpriteList[m_nCurrentIndex].m_bSoundOnlyFirst == false)
					CreateSoundObject(m_SpriteList[m_nCurrentIndex]);
		}
	}

	// 마지막 frame시간 지나면..(다음 loop 첫프레임이 되면), ret가 참이면 애니변경됨
	public bool OnAnimationLastFrame(NcSpriteAnimation spriteCom, int nLoopCount)
	{
		if (m_SpriteList.Count <= m_nCurrentIndex)
			return false;

		m_bEndSprite = true;
// 		DestroyEffectObject();

		if (m_bSequenceMode)
		{
			if (m_nCurrentIndex < GetSpriteNodeCount() - 1)
			{
				if ((m_SpriteList[m_nCurrentIndex].m_bLoop ? 3 : 1) == nLoopCount)
				{
					SetSprite(m_nCurrentIndex+1);
					return true;
				}
			} else SetSprite(0);
		} else {
			NcSpriteAnimation ncTarSprite = SetSprite(m_SpriteList[m_nCurrentIndex].m_nNextSpriteIndex) as NcSpriteAnimation;
			if (ncTarSprite != null)
			{
				ncTarSprite.ResetAnimation();
				return true;
			}
		}
		return false;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
// 		m_fFps	*= fSpeedRate;
	}

	// ------------------------------------------------------------------------------------------
	public static void CreatePlane(MeshFilter meshFilter, float fUvScale, NcFrameInfo ncSpriteFrameInfo, bool bTrimCenterAlign, ALIGN_TYPE alignType, MESH_TYPE m_MeshType, float fHShowRate)
	{
// 		Debug.Log(ncSpriteFrameInfo.m_FrameUvOffset);

		// m_MeshType, m_AlignType
		Vector3[]	planeVertices;
		Vector2		texScale = new Vector2(fUvScale * ncSpriteFrameInfo.m_FrameScale.x, fUvScale * ncSpriteFrameInfo.m_FrameScale.y);
		float		fVAlignHeight = (alignType == ALIGN_TYPE.BOTTOM ? 1.0f*texScale.y : (alignType == ALIGN_TYPE.TOP ? -1.0f*texScale.y : 0));
		float		fHAlignHeight = (alignType == ALIGN_TYPE.LEFTCENTER ? 1.0f*texScale.x : (alignType == ALIGN_TYPE.RIGHTCENTER ? -1.0f*texScale.x : 0));
		Rect		frameUvOffset = ncSpriteFrameInfo.m_FrameUvOffset;
		if (bTrimCenterAlign)
				frameUvOffset.center = Vector2.zero;

		// Vertices
		planeVertices		= new Vector3[4];
		if (alignType == ALIGN_TYPE.LEFTCENTER && 0 < fHShowRate)
		{
			planeVertices[0]	= new Vector3(frameUvOffset.xMax*texScale.x*fHShowRate+fHAlignHeight*fHShowRate, frameUvOffset.yMax*texScale.y+fVAlignHeight);
			planeVertices[1]	= new Vector3(frameUvOffset.xMax*texScale.x*fHShowRate+fHAlignHeight*fHShowRate, frameUvOffset.yMin*texScale.y+fVAlignHeight);
		} else {
			planeVertices[0]	= new Vector3(frameUvOffset.xMax*texScale.x+fHAlignHeight, frameUvOffset.yMax*texScale.y+fVAlignHeight);
			planeVertices[1]	= new Vector3(frameUvOffset.xMax*texScale.x+fHAlignHeight, frameUvOffset.yMin*texScale.y+fVAlignHeight);
		}
		planeVertices[2]		= new Vector3(frameUvOffset.xMin*texScale.x+fHAlignHeight, frameUvOffset.yMin*texScale.y+fVAlignHeight);
		planeVertices[3]		= new Vector3(frameUvOffset.xMin*texScale.x+fHAlignHeight, frameUvOffset.yMax*texScale.y+fVAlignHeight);

		Color	defColor = Color.white;
		if (meshFilter.mesh.colors != null && 0 < meshFilter.mesh.colors.Length)
			defColor = meshFilter.mesh.colors[0];

		// Color
		Color[]		colors = new Color[4];
		colors[0]	= defColor;
		colors[1]	= defColor;
		colors[2]	= defColor;
		colors[3]	= defColor;

		// Normals
		Vector3[]	normals = new Vector3[4];
		normals[0]	= new Vector3(0, 0, -1.0f);
		normals[1]	= new Vector3(0, 0, -1.0f);
		normals[2]	= new Vector3(0, 0, -1.0f);
		normals[3]	= new Vector3(0, 0, -1.0f);

		// Tangents
		Vector4[]	tangents = new Vector4[4];
		tangents[0]	= new Vector4(1, 0, 0, -1.0f);
		tangents[1]	= new Vector4(1, 0, 0, -1.0f);
		tangents[2]	= new Vector4(1, 0, 0, -1.0f);
		tangents[3]	= new Vector4(1, 0, 0, -1.0f);

		// Triangles
		int[]	triangles;
		if (m_MeshType == MESH_TYPE.BuiltIn_Plane)
		{
			triangles = new int[6];
			triangles[0]	= 1;
			triangles[1]	= 2;
			triangles[2]	= 0;
			triangles[3]	= 0;
			triangles[4]	= 2;
			triangles[5]	= 3;
		} else {
			triangles = new int[12];
			triangles[0]	= 1;
			triangles[1]	= 2;
			triangles[2]	= 0;
			triangles[3]	= 0;
			triangles[4]	= 2;
			triangles[5]	= 3;
			triangles[6]	= 1;
			triangles[7]	= 0;
			triangles[8]	= 3;
			triangles[9]	= 3;
			triangles[10]	= 2;
			triangles[11]	= 1;
		}
		// Uv
		Vector2[]	uvs = new Vector2[4];
		float		fHUvRate = 1;
		if (alignType == ALIGN_TYPE.LEFTCENTER && 0 < fHShowRate)
			fHUvRate	= fHShowRate;
		uvs[0]			= new Vector2(fHUvRate, 1);
		uvs[1]			= new Vector2(fHUvRate, 0);
		uvs[2]			= new Vector2(0, 0);
		uvs[3]			= new Vector2(0, 1);

		meshFilter.mesh.Clear();
		meshFilter.mesh.vertices	= planeVertices;
		meshFilter.mesh.colors		= colors;
		meshFilter.mesh.normals		= normals;
		meshFilter.mesh.tangents	= tangents;
		meshFilter.mesh.triangles	= triangles;
		meshFilter.mesh.uv			= uvs;
		meshFilter.mesh.RecalculateBounds();

// 		for (int n = 0; n < meshFilter.mesh.vertices.Length; n++)
//  			Debug.Log(meshFilter.mesh.vertices[n]);
	}

	public static void CreateEmptyMesh(MeshFilter meshFilter)
	{
		int nTCount = 3;

		Vector3[]	planeVertices = new Vector3[nTCount];
		Color[]		colors = new Color[nTCount];
		Vector3[]	normals = new Vector3[nTCount];
		Vector4[]	tangents = new Vector4[nTCount];
		int[]		triangles = new int[nTCount];
		Vector2[]	uvs = new Vector2[nTCount];

		for (int n = 0; n < nTCount; n++)
		{
			planeVertices[n]	= Vector3.zero;
			colors[n]			= Color.white;
			normals[n]			= Vector3.zero;
			tangents[n]			= Vector4.zero;
			triangles[n]		= 0;
			uvs[n]				= Vector2.zero;
		}

		meshFilter.mesh.Clear();
		meshFilter.mesh.vertices	= planeVertices;
		meshFilter.mesh.colors		= colors;
		meshFilter.mesh.normals		= normals;
		meshFilter.mesh.tangents	= tangents;
		meshFilter.mesh.triangles	= triangles;
		meshFilter.mesh.uv			= uvs;
		meshFilter.mesh.RecalculateBounds();
	}

	public static void UpdatePlane(MeshFilter meshFilter, float fUvScale, NcFrameInfo ncSpriteFrameInfo, bool bTrimCenterAlign, ALIGN_TYPE alignType, float fHShowRate)
	{
// 		Debug.Log(ncSpriteFrameInfo.m_FrameUvOffset);

		// m_AlignType
		Vector3[]	planeVertices;
		Vector2		texScale = new Vector2(fUvScale * ncSpriteFrameInfo.m_FrameScale.x, fUvScale * ncSpriteFrameInfo.m_FrameScale.y);
		float		fVAlignHeight = (alignType == ALIGN_TYPE.BOTTOM ? 1.0f*texScale.y : (alignType == ALIGN_TYPE.TOP ? -1.0f*texScale.y : 0));
		float		fHAlignHeight = (alignType == ALIGN_TYPE.LEFTCENTER ? 1.0f*texScale.x : (alignType == ALIGN_TYPE.RIGHTCENTER ? -1.0f*texScale.x : 0));
		Rect		frameUvOffset = ncSpriteFrameInfo.m_FrameUvOffset;
		if (bTrimCenterAlign)
				frameUvOffset.center = Vector2.zero;

		// Vertices
		planeVertices		= new Vector3[4];
		if (alignType == ALIGN_TYPE.LEFTCENTER && 0 < fHShowRate)
		{
			planeVertices[0]	= new Vector3(frameUvOffset.xMax*texScale.x*fHShowRate+fHAlignHeight*fHShowRate, frameUvOffset.yMax*texScale.y+fVAlignHeight);
			planeVertices[1]	= new Vector3(frameUvOffset.xMax*texScale.x*fHShowRate+fHAlignHeight*fHShowRate, frameUvOffset.yMin*texScale.y+fVAlignHeight);
		} else {
			planeVertices[0]	= new Vector3(frameUvOffset.xMax*texScale.x+fHAlignHeight, frameUvOffset.yMax*texScale.y+fVAlignHeight);
			planeVertices[1]	= new Vector3(frameUvOffset.xMax*texScale.x+fHAlignHeight, frameUvOffset.yMin*texScale.y+fVAlignHeight);
		}
		planeVertices[2]	= new Vector3(frameUvOffset.xMin*texScale.x, frameUvOffset.yMin*texScale.y+fVAlignHeight);
		planeVertices[3]	= new Vector3(frameUvOffset.xMin*texScale.x, frameUvOffset.yMax*texScale.y+fVAlignHeight);

		meshFilter.mesh.vertices = planeVertices;
		meshFilter.mesh.RecalculateBounds();

// 		for (int n = 0; n < meshFilter.mesh.vertices.Length; n++)
//  			Debug.Log(meshFilter.mesh.vertices[n]);
	}

	public static void UpdateMeshUVs(MeshFilter meshFilter, Rect uv, ALIGN_TYPE alignType, float fHShowRate)
	{
		Vector2[]	uvs = new Vector2[4];
		float		fHUvRate = 1;
		if (alignType == ALIGN_TYPE.LEFTCENTER && 0 < fHShowRate)
			fHUvRate	= fHShowRate;
		uvs[0]			= new Vector2(uv.x+uv.width*fHUvRate	, uv.y+uv.height);
		uvs[1]			= new Vector2(uv.x+uv.width*fHUvRate	, uv.y);
		uvs[2]			= new Vector2(uv.x			, uv.y);
		uvs[3]			= new Vector2(uv.x			, uv.y+uv.height);
 		meshFilter.mesh.uv = uvs;
	}
}

