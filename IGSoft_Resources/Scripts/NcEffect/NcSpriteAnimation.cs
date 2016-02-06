// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcSpriteAnimation : NcEffectAniBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		enum			TEXTURE_TYPE				{TileTexture, TrimTexture, SpriteFactory};
	public		TEXTURE_TYPE	m_TextureType				= TEXTURE_TYPE.TileTexture;

	public		enum			PLAYMODE					{DEFAULT, INVERSE, PINGPONG, RANDOM, SELECT};
	public		PLAYMODE		m_PlayMode					= 0;
	public		float			m_fDelayTime;

	public		int				m_nStartFrame				= 0;
	public		int				m_nFrameCount				= 0;
	public		int				m_nSelectFrame				= 0;			// m_nSpriteFactoryIndex
	public		bool			m_bLoop						= true;
	public		int				m_nLoopStartFrame			= 0;
	public		int				m_nLoopFrameCount			= 0;
	public		int				m_nLoopingCount				= 0;
	public		bool			m_bLoopRandom				= false;
	public		bool			m_bAutoDestruct				= false;
	public		float			m_fFps						= 10;
	public		int				m_nTilingX					= 2;
	public		int				m_nTilingY					= 2;

	public		GameObject						m_NcSpriteFactoryPrefab	= null;
	protected	NcSpriteFactory					m_NcSpriteFactoryCom	= null;
	public		NcSpriteFactory.NcFrameInfo[]	m_NcSpriteFrameInfos	= null;
	public		float							m_fUvScale				= 1;
	public		int								m_nSpriteFactoryIndex	= 0;

	public		NcSpriteFactory.MESH_TYPE		m_MeshType				= NcSpriteFactory.MESH_TYPE.BuiltIn_Plane;
	public		NcSpriteFactory.ALIGN_TYPE		m_AlignType				= NcSpriteFactory.ALIGN_TYPE.CENTER;
	public		float							m_fShowRate				= 1.0f;
	public		bool							m_bTrimCenterAlign		= false;

	protected	Component		m_OnChangeFrameComponent	= null;
	protected	string			m_OnChangeFrameFunction;

	protected	bool			m_bCreateBuiltInPlane		= false;
	[HideInInspector]
	public		bool			m_bBuildSpriteObj			= false;
	[HideInInspector]
	public		bool			m_bNeedRebuildAlphaChannel	= false;
	[HideInInspector]
	public		AnimationCurve	m_curveAlphaWeight			= null;
// 	[HideInInspector]
// 	public		float			m_fRedAlphaWeight			= 1;
// 	[HideInInspector]
// 	public		float			m_fGreenAlphaWeight			= 1;
// 	[HideInInspector]
// 	public		float			m_fBlueAlphaWeight			= 1;

	protected	Vector2			m_size;
	protected	Renderer		m_Renderer;
	protected	float			m_fStartTime;
	protected	int				m_nLastIndex				= -999;
	protected	int				m_nLastSeqIndex				= -1;
	protected	bool			m_bInPartLoop				= false;
	protected	bool			m_bBreakLoop				= false;
	protected	Vector2[]		m_MeshUVsByTileTexture;

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";
		if (1 < GetEditingUvComponentCount())
			return "SCRIPT_DUPERR_EDITINGUV";
		if (GetComponent<Renderer>() == null || GetComponent<Renderer>().sharedMaterial == null)
			return "SCRIPT_EMPTY_MATERIAL";

		return "";	// no error
	}
#endif
 	public override int GetAnimationState()
	{
		if (enabled == false || IsActive(gameObject) == false)
			return -1;
		if (m_fStartTime == 0 || IsEndAnimation() == false)
			return 1;
		return 0;
	}

	public float GetDurationTime()
	{
		return (m_PlayMode == PLAYMODE.PINGPONG ? m_nFrameCount*2-1 : m_nFrameCount) / m_fFps;
	}

	public int GetShowIndex()
	{
		return m_nLastIndex + m_nStartFrame;
	}
	
	public void SetBreakLoop()
	{
		m_bBreakLoop = true;
	}

	public bool IsInPartLoop()
	{
		return m_bInPartLoop;
	}

	public override void ResetAnimation()
	{
		m_nLastIndex	= -1;
		m_nLastSeqIndex	= -1;
		if (enabled == false)
			enabled = true;
		Start();
	}

	public void SetSelectFrame(int nSelFrame)
	{
		m_nSelectFrame = nSelFrame;
		SetIndex(m_nSelectFrame);
	}

	public bool IsEmptyFrame()
	{
		return m_NcSpriteFrameInfos[m_nSelectFrame].m_bEmptyFrame;
	}

	public int GetMaxFrameCount()
	{
		if (m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
			return m_nTilingX * m_nTilingY;
		else return m_NcSpriteFrameInfos.Length;
	}

	public int GetValidFrameCount()
	{
		if (m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
			return m_nTilingX * m_nTilingY - m_nStartFrame;
		else return m_NcSpriteFrameInfos.Length - m_nStartFrame;
	}

	public void SetCallBackChangeFrame(Component changeFrameComponent, string changeFrameFunction)
	{
		m_OnChangeFrameComponent	= changeFrameComponent;
		m_OnChangeFrameFunction		= changeFrameFunction;
	}

	// Loop Function --------------------------------------------------------------------
	void Awake()
	{
		if (m_MeshFilter == null)
		{
			m_MeshFilter = gameObject.GetComponent<MeshFilter>();
#if UNITY_EDITOR
			if (IsCreatingEditObject() == false)
#endif
			if (m_MeshFilter == null)
				m_MeshFilter = gameObject.AddComponent<MeshFilter>();
		}

		if (m_NcSpriteFactoryPrefab == null && gameObject.GetComponent<NcSpriteFactory>() != null)
			m_NcSpriteFactoryPrefab = gameObject;
		UpdateFactoryInfo(m_nSpriteFactoryIndex);

		if (m_nLoopFrameCount == 0)
			m_nLoopFrameCount = m_nFrameCount - m_nLoopStartFrame;
		m_Renderer		= GetComponent<Renderer>();
	}

	void ResetLocalValue()
	{
		m_size			= new Vector2(1.0f / m_nTilingX, 1.0f / m_nTilingY);
		m_Renderer		= GetComponent<Renderer>();

		m_fStartTime	= GetEngineTime();
		m_nFrameCount	= (m_nFrameCount <= 0) ? m_nTilingX * m_nTilingY : m_nFrameCount;
		m_nLastIndex	= -999;
		m_nLastSeqIndex	= -1;
		m_bInPartLoop	= false;
		m_bBreakLoop	= false;
	}

	void Start()
	{
		ResetLocalValue();

		if (m_Renderer == null)
		{
			enabled = false;
			return;
		}

		if (m_PlayMode == PLAYMODE.SELECT)
		{
			SetIndex(m_nSelectFrame);
		} else {
			if (0 < m_fDelayTime)
			{
				m_Renderer.enabled = false;
				return;
			}
			if (m_PlayMode == PLAYMODE.RANDOM)
				SetIndex(Random.Range(0, m_nFrameCount-1));
			else {
				InitAnimationTimer();
				if (m_bLoopRandom)
					m_Timer.Reset(Random.value);
				SetIndex(0);
			}
		}
	}

	void Update()
	{
		if (m_PlayMode == PLAYMODE.SELECT)
			return;
		if (m_Renderer == null || m_nTilingX * m_nTilingY == 0)
			return;

		if (m_fDelayTime != 0)
		{
			if (GetEngineTime() < m_fStartTime + m_fDelayTime)
				return;
			m_fDelayTime = 0;
			InitAnimationTimer();
			m_Renderer.enabled = true;
		}

		if (m_PlayMode != PLAYMODE.RANDOM)
		{
			int nSeqIndex = (int)(m_Timer.GetTime() * m_fFps);

			if (nSeqIndex == 0)
			{
				if (m_NcSpriteFactoryCom != null)
					m_NcSpriteFactoryCom.OnAnimationStartFrame(this);
//				if (m_funcStartSprite != "")
//					gameObject.SendMessage(m_funcStartSprite, this, SendMessageOptions.DontRequireReceiver);
			}

			if (m_NcSpriteFactoryCom != null && m_nFrameCount <= 0)
				m_NcSpriteFactoryCom.OnAnimationLastFrame(this, 0);
			else {
				if ((m_PlayMode == PLAYMODE.PINGPONG ? m_nFrameCount*2-1 : m_nFrameCount) <= nSeqIndex)	// first loop
				{
					if (m_bLoop == false)
					{
						if (m_NcSpriteFactoryCom != null)
							if (m_NcSpriteFactoryCom.OnAnimationLastFrame(this, 1))
								return;

						UpdateEndAnimation();
						return;
					} else {
						if (m_PlayMode == PLAYMODE.PINGPONG)
						{
							if (m_NcSpriteFactoryCom != null && nSeqIndex % (m_nFrameCount*2-2) == 1)
								if (m_NcSpriteFactoryCom.OnAnimationLastFrame(this, nSeqIndex / (m_nFrameCount*2-1)))
									return;
						} else {
							if (m_NcSpriteFactoryCom != null && nSeqIndex % m_nFrameCount == 0)
								if (m_NcSpriteFactoryCom.OnAnimationLastFrame(this, nSeqIndex / m_nFrameCount))
									return;
						}
					}
				}
				SetIndex(nSeqIndex);
			}
		}
	}

	// Control Function -----------------------------------------------------------------
	public void SetSpriteFactoryIndex(int nSpriteFactoryIndex, bool bRunImmediate)
	{
		UpdateFactoryInfo(nSpriteFactoryIndex);
	}

	public void SetShowRate(float fShowRate)
	{
		m_fShowRate = fShowRate;
		UpdateSpriteTexture(m_nLastIndex, true);
	}

	bool UpdateFactoryInfo(int nSpriteFactoryIndex)
	{
		m_nSpriteFactoryIndex	= nSpriteFactoryIndex;

		if (m_NcSpriteFactoryCom == null)
		{
			if (m_NcSpriteFactoryPrefab && m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null)
				m_NcSpriteFactoryCom = m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
			else return false;
		}
		NcSpriteFactory.NcSpriteNode	spriteNode = m_NcSpriteFactoryCom.GetSpriteNode(m_nSpriteFactoryIndex);

		m_bBuildSpriteObj		= false;
		m_bAutoDestruct			= false;
		m_fUvScale				= m_NcSpriteFactoryCom.m_fUvScale;
		m_nStartFrame			= 0;
		if (spriteNode != null)
		{
			m_nFrameCount		= spriteNode.m_nFrameCount;
			m_fFps				= spriteNode.m_fFps;
			m_bLoop				= spriteNode.m_bLoop;
			m_nLoopStartFrame	= spriteNode.m_nLoopStartFrame;
			m_nLoopFrameCount	= spriteNode.m_nLoopFrameCount;
			m_nLoopingCount		= spriteNode.m_nLoopingCount;
			m_NcSpriteFrameInfos= spriteNode.m_FrameInfos;
		}
		ResetLocalValue();
		return true;
	}


	// ret : 0 frameIndex
	int GetPartLoopFrameIndex(int nSeqIndex)
	{
		if (nSeqIndex < 0)
			return -1;
		int nLoopFrameIndex	= (nSeqIndex - m_nLoopStartFrame);
		// before Loop
		if (nLoopFrameIndex < 0)
			return nSeqIndex;
		int nLoopCount		= nLoopFrameIndex / m_nLoopFrameCount;
		// loop
		if (m_nLoopingCount == 0 || nLoopCount < m_nLoopingCount)
			return nLoopFrameIndex % m_nLoopFrameCount + m_nLoopStartFrame;
		// over
		return (nLoopFrameIndex - (m_nLoopFrameCount * (m_nLoopingCount - 1)) + m_nLoopStartFrame);
	}

	// ret : 0 ~ loopCount-1, loopCount = over, -1 = beforeLoop
	int GetPartLoopCount(int nSeqIndex)
	{
		if (nSeqIndex < 0)
			return -1;
		int nLoopFrameIndex	= (nSeqIndex - m_nLoopStartFrame);
		// before Loop
		if (nLoopFrameIndex < 0)
			return -1;
		int nLoopCount		= nLoopFrameIndex / m_nLoopFrameCount;
		// loop
		if (m_nLoopingCount == 0 || nLoopCount < m_nLoopingCount)
			return nLoopCount;
		// over
		return m_nLoopingCount;
	}

	// ret nSeqIndex
	int CalcPartLoopInfo(int nSeqIndex, ref int nLoopCount)
	{
		if (m_nLoopFrameCount <= 0)	// error
			return 0;
		if (m_bBreakLoop)
		{
			nLoopCount	= GetPartLoopCount(nSeqIndex);
			UpdateEndAnimation();
			m_bBreakLoop = false;
			return m_nLoopStartFrame + m_nLoopFrameCount;	// loop다음frame
		}

		if (nSeqIndex < m_nLoopStartFrame)
			return nSeqIndex;

		m_bInPartLoop	= true;
//		int nLastSetIndex	= GetPartLoopFrameIndex(m_nLastSeqIndex);
		int nLastLoopCount	= GetPartLoopCount(m_nLastSeqIndex);
		int nSetIndex		= GetPartLoopFrameIndex(nSeqIndex);
		nLoopCount			= GetPartLoopCount(nSeqIndex);

		int cnt = 0;
		// Change LoopIndex
		for (int n = nLastLoopCount; n < Mathf.Min(nLoopCount, m_nLoopFrameCount-1); n++, cnt++)
		{
			// PartLoop LoopStartEvent
			if (transform.parent != null)
				transform.parent.SendMessage("OnSpriteAnimationLoopStart", nLoopCount, SendMessageOptions.DontRequireReceiver);
		}

		// Over
		if (0 < m_nLoopingCount && m_nLoopingCount <= nLoopCount)
		{
			m_bInPartLoop	= false;
			// PartLoop LoopEndEvent
			if (transform.parent != null)
				transform.parent.SendMessage("OnSpriteAnimationLoopEnd", nLoopCount, SendMessageOptions.DontRequireReceiver);

			if (m_nFrameCount <= nSetIndex)
			{
				nSetIndex = m_nFrameCount - 1;	// last frame
				UpdateEndAnimation();
			}
		}

		return nSetIndex;
	}

	void UpdateEndAnimation()
	{
		enabled = false;
		OnEndAnimation();
		// Destroy
		if (m_bAutoDestruct)
		{
			if (m_bReplayState)
				SetActiveRecursively(gameObject, false);
			else DestroyObject(gameObject);
		}
	}

	void SetIndex(int nSeqIndex)
	{
		if (m_Renderer != null)
		{
			int	nSetIndex  = m_nLastSeqIndex = nSeqIndex;
			int nLoopCount = nSeqIndex / m_nFrameCount;

			switch (m_PlayMode)
			{
				case PLAYMODE.DEFAULT:
					{
						if (m_bLoop)
						{
							nSetIndex = CalcPartLoopInfo(nSeqIndex, ref nLoopCount) % m_nFrameCount;
						} else {
							nSetIndex = nSeqIndex % m_nFrameCount;
						}
						break;
					}
				case PLAYMODE.INVERSE:	nSetIndex = m_nFrameCount - (nSetIndex % m_nFrameCount) - 1;	break;
				case PLAYMODE.PINGPONG:
					{
						nLoopCount = (nSetIndex / (m_nFrameCount * 2 - (nSetIndex == 0 ? 1 : 2)));
						nSetIndex  = (nSetIndex % (m_nFrameCount * 2 - (nSetIndex == 0 ? 1 : 2)));
						if (m_nFrameCount <= nSetIndex)
							nSetIndex = m_nFrameCount - (nSetIndex % m_nFrameCount) - 2;
						break;
					}
				case PLAYMODE.RANDOM:	break;
				case PLAYMODE.SELECT:	nSetIndex = nSeqIndex % m_nFrameCount;		break;
			}

			if (nSetIndex == m_nLastIndex)
				return;

			if (m_TextureType == TEXTURE_TYPE.TileTexture)
			{
				int		uIndex = (nSetIndex + m_nStartFrame) % m_nTilingX;
				int		vIndex = (nSetIndex + m_nStartFrame) / m_nTilingX;
				Vector2 offset = new Vector2(uIndex * m_size.x, 1.0f - m_size.y - vIndex * m_size.y);

  				if (UpdateMeshUVsByTileTexture(new Rect(offset.x, offset.y, m_size.x, m_size.y)) == false)
				{
					m_Renderer.material.mainTextureOffset	= new Vector2(offset.x - ((int)offset.x), offset.y - ((int)offset.y));
					m_Renderer.material.mainTextureScale	= m_size;
					AddRuntimeMaterial(m_Renderer.material);
				}
			} else
			if (m_TextureType == TEXTURE_TYPE.TrimTexture)
			{
				UpdateSpriteTexture(nSetIndex, true);
			} else
			if (m_TextureType == TEXTURE_TYPE.SpriteFactory)
			{
				UpdateFactoryTexture(nSetIndex, true);
			}

			if (m_NcSpriteFactoryCom != null)
				m_NcSpriteFactoryCom.OnAnimationChangingFrame(this, m_nLastIndex, nSetIndex, nLoopCount);
			if (m_OnChangeFrameComponent	!= null)
				m_OnChangeFrameComponent.SendMessage(m_OnChangeFrameFunction, nSetIndex, SendMessageOptions.DontRequireReceiver);

			m_nLastIndex = nSetIndex;
		}
	}

	void UpdateSpriteTexture(int nSelIndex, bool bShowEffect)
	{
		nSelIndex += m_nStartFrame;
		if (m_NcSpriteFrameInfos == null || nSelIndex < 0 || m_NcSpriteFrameInfos.Length <= nSelIndex)
			return;
		CreateBuiltInPlane(nSelIndex);
		UpdateBuiltInPlane(nSelIndex);
	}

	void UpdateFactoryTexture(int nSelIndex, bool bShowEffect)
	{
		nSelIndex += m_nStartFrame;
		if (m_NcSpriteFrameInfos == null || nSelIndex < 0 || m_NcSpriteFrameInfos.Length <= nSelIndex)
			return;
		if (UpdateFactoryMaterial() == false)
			return;
		if (m_NcSpriteFactoryCom.IsValidFactory() == false)
			return;
		CreateBuiltInPlane(nSelIndex);
		UpdateBuiltInPlane(nSelIndex);
	}

	public bool UpdateFactoryMaterial()
	{
		if (m_NcSpriteFactoryPrefab == null)
			return false;
		if (m_NcSpriteFactoryPrefab.GetComponent<Renderer>() == null || m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial == null || m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial.mainTexture == null)
			return false;
		if (GetComponent<Renderer>() == null)
			return false;
		
		if (m_NcSpriteFactoryCom == null)
			return false;
		if (m_nSpriteFactoryIndex < 0 || m_NcSpriteFactoryCom.GetSpriteNodeCount() <= m_nSpriteFactoryIndex)
			return false;
		if (m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation && m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.Auto)
			return false;
		GetComponent<Renderer>().sharedMaterial = m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial;
		return true;
	}

	void CreateBuiltInPlane(int nSelIndex)
	{
		if (m_bCreateBuiltInPlane)
			return;
		m_bCreateBuiltInPlane = true;
		if (m_MeshFilter == null)
		{
			if (gameObject.GetComponent<MeshFilter>() != null)
				 m_MeshFilter = gameObject.GetComponent<MeshFilter>();
			else m_MeshFilter = gameObject.AddComponent<MeshFilter>();
		}
		NcSpriteFactory.CreatePlane(m_MeshFilter, m_fUvScale, m_NcSpriteFrameInfos[nSelIndex], (m_TextureType == TEXTURE_TYPE.TileTexture ? false : m_bTrimCenterAlign), m_AlignType, m_MeshType, m_fShowRate);
	}

	void UpdateBuiltInPlane(int nSelIndex)
	{
		NcSpriteFactory.UpdatePlane(m_MeshFilter, m_fUvScale, m_NcSpriteFrameInfos[nSelIndex], (m_TextureType == TEXTURE_TYPE.TileTexture ? false : m_bTrimCenterAlign), m_AlignType, m_fShowRate);
  		NcSpriteFactory.UpdateMeshUVs(m_MeshFilter, m_NcSpriteFrameInfos[nSelIndex].m_TextureUvOffset, m_AlignType, m_fShowRate);
	}

	bool UpdateMeshUVsByTileTexture(Rect uv)
	{
// 		return false;
//  		Debug.Log(uv);
		if (m_MeshFilter != null && m_MeshUVsByTileTexture == null)
			return false;
		if (m_MeshFilter == null)
			m_MeshFilter	= (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
		if (m_MeshFilter == null || m_MeshFilter.sharedMesh == null)
			return false;
// 		if (16 < m_MeshFilter.sharedMesh.uv.Length)	// planeMesh only
// 			return false;
		if (m_MeshUVsByTileTexture == null)
		{
			for (int n = 0; n < m_MeshFilter.sharedMesh.uv.Length; n++)	// check planeUVs
			{
				if (m_MeshFilter.sharedMesh.uv[n].x != 0 && m_MeshFilter.sharedMesh.uv[n].x != 1.0f) return false;
				if (m_MeshFilter.sharedMesh.uv[n].y != 0 && m_MeshFilter.sharedMesh.uv[n].y != 1.0f) return false;
			}
			m_MeshUVsByTileTexture	= m_MeshFilter.sharedMesh.uv;
		}

		Vector2[]	value = new Vector2[m_MeshUVsByTileTexture.Length];
		for (int n = 0; n < m_MeshUVsByTileTexture.Length; n++)
		{
			if (m_MeshUVsByTileTexture[n].x == 0)		value[n].x = uv.x;
			if (m_MeshUVsByTileTexture[n].y == 0)		value[n].y = uv.y;
			if (m_MeshUVsByTileTexture[n].x == 1.0f)	value[n].x = uv.x+uv.width;
			if (m_MeshUVsByTileTexture[n].y == 1.0f)	value[n].y = uv.y+uv.height;
//  			Debug.Log(m_MeshUVs[n] + " " + " " + value[n]);
		}
 		m_MeshFilter.mesh.uv = value;
		return true;
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime *= fSpeedRate;
		m_fFps	*= fSpeedRate;
	}

	public override void OnResetReplayStage(bool bClearOldParticle)
	{
		base.OnResetReplayStage(bClearOldParticle);
		ResetAnimation();
	}
}

