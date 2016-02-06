// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class FxmTestControls : MonoBehaviour
{
	// -------------------------------------------------------------------------------------------
	// const
	protected	const int	m_nRepeatIndex			= 3;

	public		bool		m_bMinimize				= false;

	// gui
	protected	int			m_nTriangles			= 0;
	protected	int			m_nVertices				= 0;
	protected	int			m_nMeshCount			= 0;
	protected	int			m_nParticleCount		= 0;

	protected	int			m_nPlayIndex;
	protected	int			m_nTransIndex;
	protected	float[]		m_fPlayToolbarTimes		= {/*AutoRepeat*/ 1.0f, 1.0f, /*OneShot*/ 1.0f, /*Repeat*/ 0.3f, 0.6f, 1.0f, 2.0f, 3.0f};
	public		enum AXIS	{X=0, Y, Z};
	protected	AXIS		m_nTransAxis			= AXIS.Z;

	protected	float		m_fDelayCreateTime		= 0.2f;
	protected	bool		m_bCalledDelayCreate	= false;
	protected	bool		m_bStartAliveAnimation	= false;
	protected	float		m_fDistPerTime			= 10.0f;
	protected	int			m_nRotateIndex			= 0;
	protected	int			m_nMultiShotCount		= 1;

	protected	float		m_fTransRate			= 1.0f;
	protected	float		m_fStartPosition		= 0;

	public		float		m_fTimeScale			= 1.0f;
	protected	float		m_fPlayStartTime;
	protected	float		m_fOldTimeScale			= 1.0f;

	protected	float		m_fCreateTime			= 0;

	// -------------------------------------------------------------------------------------------
	public float GetTimeScale()
	{
		return m_fTimeScale;
	}

	public bool IsRepeat()
	{
		return (m_nRepeatIndex <= m_nPlayIndex);
	}

	public bool IsAutoRepeat()
	{
		return (m_nPlayIndex == 0);
	}

	public float GetRepeatTime()
	{
		// return RepeatTime
		return m_fPlayToolbarTimes[m_nPlayIndex];
	}

	public void SetStartTime()
	{
		m_fPlayStartTime = Time.time;
	}

	void LoadPrefs()
	{
		m_nPlayIndex	= PlayerPrefs.GetInt("FxmTestControls.m_nPlayIndex"		, m_nPlayIndex);
		m_nTransIndex	= PlayerPrefs.GetInt("FxmTestControls.m_nTransIndex"	, m_nTransIndex);
		m_fTimeScale	= PlayerPrefs.GetFloat("FxmTestControls.m_fTimeScale"	, m_fTimeScale);
		m_fDistPerTime	= PlayerPrefs.GetFloat("FxmTestControls.m_fDistPerTime"	, m_fDistPerTime);
		m_nRotateIndex	= PlayerPrefs.GetInt("FxmTestControls.m_nRotateIndex"	, m_nRotateIndex);
		m_nTransAxis	= (AXIS)PlayerPrefs.GetInt("FxmTestControls.m_nTransAxis", (int)m_nTransAxis);
		m_bMinimize		= PlayerPrefs.GetInt("FxmTestControls.m_bMinimize", m_bMinimize ? 1 : 0) == 1;
		SetTimeScale(m_fTimeScale);
	}

	void SavePrefs()
	{
		PlayerPrefs.SetInt("FxmTestControls.m_nPlayIndex"		, m_nPlayIndex);
		PlayerPrefs.SetInt("FxmTestControls.m_nTransIndex"		, m_nTransIndex);
		PlayerPrefs.SetFloat("FxmTestControls.m_fTimeScale"		, m_fTimeScale);
		PlayerPrefs.SetFloat("FxmTestControls.m_fDistPerTime"	, m_fDistPerTime);
		PlayerPrefs.SetInt("FxmTestControls.m_nRotateIndex"		, m_nRotateIndex);
		PlayerPrefs.SetInt("FxmTestControls.m_nTransAxis"		, (int)m_nTransAxis);
	}

	public void SetDefaultSetting()
	{
		m_nPlayIndex		= 0;
		m_nTransIndex		= 0;
		m_nTransAxis		= AXIS.Z;
		m_fDistPerTime		= 10;
		m_nRotateIndex		= 0;
		m_nMultiShotCount	= 1;
		m_fTransRate		= 1;
		m_fStartPosition	= 0;
		SavePrefs();
	}

	public void AutoSetting(int nPlayIndex, int nTransIndex, AXIS nTransAxis, float fDistPerTime, int nRotateIndex, int nMultiShotCount, float fTransRate, float fStartAdjustRate)
	{
		m_nPlayIndex		= nPlayIndex;
		m_nTransIndex		= nTransIndex;
		m_nTransAxis		= nTransAxis;
		m_fDistPerTime		= fDistPerTime;
		m_nRotateIndex		= nRotateIndex;
		m_nMultiShotCount	= nMultiShotCount;
		m_fTransRate		= fTransRate;
		m_fStartPosition	= fStartAdjustRate;
	}

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
		NgUtil.LogDevelop("Awake - m_FXMakerControls");
		LoadPrefs();
	}

	void OnEnable()
	{
		NgUtil.LogDevelop("OnEnable - m_FXMakerControls");
		LoadPrefs();
	}

	void Start()
	{
	}

	void Update()
	{
		m_fTimeScale = Time.timeScale;

		if (FxmTestMain.inst.GetInstanceEffectObject() == null && IsAutoRepeat() == false)
		{
			DelayCreateInstanceEffect(false);
		} else {
			// mesh info
			NgObject.GetMeshInfo(NcEffectBehaviour.GetRootInstanceEffect(), true, out m_nVertices, out m_nTriangles, out m_nMeshCount);

			// particle count
			m_nParticleCount = 0;
 			ParticleSystem[]	psComs = NcEffectBehaviour.GetRootInstanceEffect().GetComponentsInChildren<ParticleSystem>();
 			foreach (ParticleSystem com in psComs)
 				m_nParticleCount += com.particleCount;
 			ParticleEmitter[]	peComs = NcEffectBehaviour.GetRootInstanceEffect().GetComponentsInChildren<ParticleEmitter>();
 			foreach (ParticleEmitter com in peComs)
 				m_nParticleCount += com.particleCount;

			if (m_fDelayCreateTime < Time.time - m_fPlayStartTime)
			{
				// repeat
				if (IsRepeat() && m_fCreateTime + GetRepeatTime() < Time.time)
					DelayCreateInstanceEffect(false);

				// auto repeat
				if (m_nTransIndex == 0 && IsAutoRepeat() && m_bCalledDelayCreate == false)	// m_bStartAliveAnimation
				{
					if (IsAliveAnimation() == false)
						DelayCreateInstanceEffect(false);
				}
			}
		}
	}

	bool IsAliveAnimation()
	{
		GameObject	rootInstObj		= NcEffectBehaviour.GetRootInstanceEffect();

		Transform[]	tranComs = rootInstObj.GetComponentsInChildren<Transform>(true);
		foreach (Transform trans in tranComs)
		{
			int			bNcAni		= -1;	// -1:None, 0:End, 1:Alive
			int			bParticle	= -1;
			bool		bRen		= false;

			// Check Animation
			NcEffectBehaviour[]	effList = trans.GetComponents<NcEffectBehaviour>();
			foreach (NcEffectBehaviour eff in effList)
			{
				switch (eff.GetAnimationState())
				{
					case 1 : bNcAni = 1;	break;
					case 0 : bNcAni = 0;	break;
				}
			}

			// Check ParticleSystem
			if (trans.GetComponent<ParticleSystem>() != null)
			{
				bParticle = 0;
				if (NgObject.IsActive(trans.gameObject) && ((trans.GetComponent<ParticleSystem>().enableEmission && trans.GetComponent<ParticleSystem>().IsAlive()) || 0 < trans.GetComponent<ParticleSystem>().particleCount))
					bParticle = 1;
			}

			// Check ParticleSystem
			if (bParticle < 1)
			{
				if (trans.GetComponent<ParticleEmitter>() != null)
				{
					bParticle = 0;
					if (NgObject.IsActive(trans.gameObject) && (trans.GetComponent<ParticleEmitter>().emit || 0 < trans.GetComponent<ParticleEmitter>().particleCount))
						bParticle = 1;
				}
			}

			// Check Renderer
			if (trans.GetComponent<Renderer>() != null)
			{
				if (trans.GetComponent<Renderer>().enabled && NgObject.IsActive(trans.gameObject))
					bRen = true;
			}

//   			Debug.Log("bNcAni " + bNcAni + ", bParticle " + bParticle + ", bRen " + bRen);
			if (0 < bNcAni)
				return true;
			if (bParticle == 1)
				return true;
			if (bRen && (trans.GetComponent<MeshFilter>() != null || trans.GetComponent<TrailRenderer>() != null || trans.GetComponent<LineRenderer>() != null))
				return true;
		}
		return false;
	}

	// -------------------------------------------------------------------------------------------
	public void OnGUIControl()
	{
		// Selected Info Window -----------------------------------------------
		GUI.Window(10, GetActionToolbarRect(), winActionToolbar, "PrefabSimulate - " + (FxmTestMain.inst.IsCurrentEffectObject() ? FxmTestMain.inst.GetOriginalEffectObject().name : "Not Selected"));
	}

	public Rect GetActionToolbarRect()
	{
		float fHeight = Screen.height*(m_bMinimize ? 0.1f : 0.35f);
		return new Rect(0, Screen.height-fHeight, Screen.width, fHeight);
	}

	string NgTooltipTooltip(string str)
	{
		return str;
	}

	public static GUIContent[] GetHcEffectControls_Play(float fAutoRet, float timeScale, float timeOneShot1, float timeRepeat1, float timeRepeat2, float timeRepeat3, float timeRepeat4, float timeRepeat5)
	{
		GUIContent[]	cons = new GUIContent[8];
		cons[0]	= new GUIContent("AutoRet"	, "");
		cons[1]	= new GUIContent(timeScale.ToString("0.00")+"x S"	, "");
		cons[2]	= new GUIContent(timeOneShot1.ToString("0.0")+"x S"	, "");
		cons[3]	= new GUIContent(timeRepeat1.ToString("0.0")+"s R"	, "");
		cons[4]	= new GUIContent(timeRepeat2.ToString("0.0")+"s R"	, "");
		cons[5]	= new GUIContent(timeRepeat3.ToString("0.0")+"s R"	, "");
		cons[6]	= new GUIContent(timeRepeat4.ToString("0.0")+"s R"	, "");
		cons[7]	= new GUIContent(timeRepeat5.ToString("0.0")+"s R"	, "");

		return cons;
	}

	public static GUIContent[] GetHcEffectControls_Trans(AXIS nTransAxis)
	{
		GUIContent[]	cons = new GUIContent[8];

		cons[0]	= new GUIContent("Stop"		,  "");
		cons[1]	= new GUIContent(nTransAxis.ToString() + " Move"	, "");
		cons[2]	= new GUIContent(nTransAxis.ToString() + " Scale", "");
		cons[3]	= new GUIContent("Arc"		, "");
		cons[4]	= new GUIContent("Fall"		, "");
		cons[5]	= new GUIContent("Raise"	, "");
		cons[6]	= new GUIContent("Circle"	, "");
		cons[7]	= new GUIContent("Tornado"	, "");
		return cons;
	}

	public static GUIContent[] GetHcEffectControls_Rotate()
	{
		GUIContent[]	cons = new GUIContent[2];
		cons[0]	= new GUIContent("Rot"	, "");
		cons[1]	= new GUIContent("Fix"	, "");
		return cons;
	}

	// -------------------------------------------------------------------------------------------
	void winActionToolbar(int id)
	{
		Rect		popupRect	= GetActionToolbarRect();
		Rect		baseRect;
		Rect		rect;
		string		info		= "";
		string		infotooltip	= "";
		int			nColCount	= 10;
		int			nRowCount	= 5;
		GUIContent	content;

		// mini ----------------------------------------------------------------
		m_bMinimize = GUI.Toggle(new Rect(3, 1, FXMakerLayout.m_fMinimizeClickWidth, FXMakerLayout.m_fMinimizeClickHeight), m_bMinimize, "Mini");
		if (GUI.changed)
			PlayerPrefs.SetInt("FxmTestControls.m_bMinimize", m_bMinimize ? 1 : 0);
		GUI.changed = false;
		if (FXMakerLayout.m_bMinimizeAll || m_bMinimize)
		{
			nRowCount = 1;
			// mesh info -----------------------------------------------------------------
			baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 0, 1);
			if (FxmTestMain.inst.IsCurrentEffectObject())
			{
				info		= string.Format("P={0} M={1} T={2}", m_nParticleCount, m_nMeshCount, m_nTriangles);
				infotooltip	= string.Format("ParticleCount = {0} MeshCount = {1}\n Mesh: Triangles = {2} Vertices = {3}", m_nParticleCount, m_nMeshCount, m_nTriangles, m_nVertices);
			}
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 0, 2), info);

			// CurrentTime Horizontal Slider ----------------------------------------------
			if (FxmTestMain.inst.IsCurrentEffectObject())
			{
				float fMaxTime = (m_nRepeatIndex <= m_nPlayIndex) ? m_fPlayToolbarTimes[m_nPlayIndex] : 10.0f;
				baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 0, 1);
				GUI.Box(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 2, 2), "ElapsedTime " + (Time.time - m_fPlayStartTime).ToString("0.000"));
				rect = FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 4, 4);
				rect.y += 5;
				GUI.HorizontalSlider(rect, Time.time - m_fPlayStartTime, 0.0f, fMaxTime);

				// restart
				baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 0, 1);
				if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 8, 2), "Restart"))
					CreateInstanceEffect();
			}

			return;
		}

		// mesh info -----------------------------------------------------------------
		baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 0, 2);
		if (NcEffectBehaviour.GetRootInstanceEffect())
		{
			info		= string.Format("P = {0}\nM = {1}\nT = {2}", m_nParticleCount, m_nMeshCount, m_nTriangles);
			infotooltip	= string.Format("ParticleCount = {0} MeshCount = {1}\n Mesh: Triangles = {2} Vertices = {3}", m_nParticleCount, m_nMeshCount, m_nTriangles, m_nVertices);
		}
		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 0, 1), new GUIContent(info, NgTooltipTooltip(infotooltip)));

		// control button ------------------------------------------------------------
		if (FxmTestMain.inst.IsCurrentEffectObject())
		{
			bool bClick = false;

			// Play ---------------------------------------
			GUIContent[]	playToolbarContents	= GetHcEffectControls_Play(0, m_fTimeScale, m_fPlayToolbarTimes[1], m_fPlayToolbarTimes[m_nRepeatIndex], m_fPlayToolbarTimes[m_nRepeatIndex+1], m_fPlayToolbarTimes[m_nRepeatIndex+2], m_fPlayToolbarTimes[m_nRepeatIndex+3], m_fPlayToolbarTimes[m_nRepeatIndex+4]);
			baseRect		= FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 0, 1);
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 1, 1), new GUIContent("Play", ""));
//			int nPlayIndex	= GUI.SelectionGrid(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 2, 8), m_nPlayIndex, playToolbarContents, playToolbarContents.Length);
			int nPlayIndex	= FXMakerLayout.TooltipSelectionGrid(popupRect, FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 2, 8), m_nPlayIndex, playToolbarContents, playToolbarContents.Length);

			if (GUI.changed)
				bClick = true;

			// Trans ---------------------------------------
			GUIContent[]	TransToolbarContents	= GetHcEffectControls_Trans(m_nTransAxis);
			baseRect		= FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 1, 1);
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 1, 1), new GUIContent("Trans", ""));
// 			int nTransIndex	= GUI.SelectionGrid(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 2, 8), m_nTransIndex, TransToolbarContents, TransToolbarContents.Length);
			int nTransIndex	= FXMakerLayout.TooltipSelectionGrid(popupRect, FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 2, 8), m_nTransIndex, TransToolbarContents, TransToolbarContents.Length);

			if (GUI.changed)
			{
				bClick = true;
				if ((nTransIndex == 1 || nTransIndex == 2) && Input.GetMouseButtonUp(1))	// m_nTransIndex scale
				{
					if (m_nTransAxis == AXIS.Z)
						m_nTransAxis = 0;
					else m_nTransAxis++;
					PlayerPrefs.SetInt("FxmTestControls.m_nTransAxis", (int)m_nTransAxis);
				}
			}

			if (bClick)
			{
				FxmTestMain.inst.CreateCurrentInstanceEffect(false);
				RunActionControl(nPlayIndex, nTransIndex);
				PlayerPrefs.SetInt("FxmTestControls.m_nPlayIndex", m_nPlayIndex);
				PlayerPrefs.SetInt("FxmTestControls.m_nTransIndex", m_nTransIndex);
			}
		}

		// TransSpeed Horizontal Slider -----------------------------------------------
		float	TransSpeed	= m_fDistPerTime;
		baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 2, 1);
		content  = new GUIContent("DistPerTime", "");
		content.text += " " + m_fDistPerTime.ToString("00.00");
		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 0, 2), content);
		rect = FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 2, 5);
		rect.y += 5;
		TransSpeed = GUI.HorizontalSlider(rect, TransSpeed, 0.1f, 40.0f);
		// TransSpeed Trans ----------------------------------------------
// 		if (GUI.Button(NgLayout.GetInnerHorizontalRect(baseRect, nColCount*2, 23, 1), new GUIContent("1", "")))
// 			TransSpeed = 1;
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount*2, 14, 1), new GUIContent("<", "")))
			TransSpeed = (int)(TransSpeed-1);
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount*2, 15, 1), new GUIContent(">", "")))
			TransSpeed = (int)(TransSpeed+1);
		if (TransSpeed != m_fDistPerTime)
		{
			m_fDistPerTime = (TransSpeed == 0 ? 0.1f : TransSpeed);
			PlayerPrefs.SetFloat("FxmTestControls.m_fDistPerTime", m_fDistPerTime);
			// Trans ���¸�.. �ٷ� ����
			if (0 < m_nTransIndex)
				CreateInstanceEffect();
		}

		if (NgLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 9, 1), new GUIContent("Multi", m_nMultiShotCount.ToString()), true))
		{
			if (Input.GetMouseButtonUp(0))
			{
				m_nMultiShotCount++;
				if (4 < m_nMultiShotCount)
					m_nMultiShotCount = 1;
			} else {
				m_nMultiShotCount = 1;
			}
			CreateInstanceEffect();
		}

		// front Rotation ----------------------------------------------
		GUIContent[]		rotateToolbarContents	= GetHcEffectControls_Rotate();
		baseRect			= FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 2, 1);
// 		int nRotateIndex	= GUI.SelectionGrid(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 8, 1), m_nRotateIndex, rotateToolbarContents, rotateToolbarContents.Length);
		int nRotateIndex	= FXMakerLayout.TooltipSelectionGrid(popupRect, FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 8, 1), m_nRotateIndex, rotateToolbarContents, rotateToolbarContents.Length);

		if (nRotateIndex != m_nRotateIndex)
		{
			m_nRotateIndex = nRotateIndex;
			PlayerPrefs.SetInt("FxmTestControls.m_nRotateIndex", m_nRotateIndex);
			// Trans ���¸�.. �ٷ� ����
			if (0 < m_nTransIndex)
				CreateInstanceEffect();
		}

		// timeScale Horizontal Slider -----------------------------------------------
		float timeScale = m_fTimeScale;
		baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 3, 1);
		content  = new GUIContent("TimeScale", "");
		content.text += " " + m_fTimeScale.ToString("0.00");
		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 0, 2), content);
		rect = FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 2, 5);
		rect.y += 5;
		timeScale = GUI.HorizontalSlider(rect, timeScale, 0.0f, 3.0f);
		if (timeScale == 0)
		{
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 7, 1), new GUIContent("Resume", "")))
				timeScale = m_fOldTimeScale;
		} else {
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 7, 1), new GUIContent("Pause", "")))
				timeScale = 0;
		}
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 8, 1), new GUIContent("Reset", "")))
			timeScale = 1;
		SetTimeScale(timeScale);

		// CurrentTime Horizontal Slider ----------------------------------------------
		if (FxmTestMain.inst.IsCurrentEffectObject())
		{
			float fMaxTime = (m_nRepeatIndex <= m_nPlayIndex) ? m_fPlayToolbarTimes[m_nPlayIndex] : 10.0f;
			baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 4, 1);
			content  = new GUIContent("ElapsedTime", "");
			content.text += " " + (Time.time - m_fPlayStartTime).ToString("0.000");
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 0, 2), content);
			rect = FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 2, 5);
			rect.y += 5;
			GUI.HorizontalSlider(rect, Time.time - m_fPlayStartTime, 0.0f, fMaxTime);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount*2, 14, 1), new GUIContent("+.5", "")))
			{
				SetTimeScale(1.0f);
				Invoke("invokeStopTimer", 0.5f);
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount*2, 15, 1), new GUIContent("+.1", "")))
			{
				SetTimeScale(0.4f);
				Invoke("invokeStopTimer", 0.1f);
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount*2, 16, 1), new GUIContent("+.05", "")))
			{
				SetTimeScale(0.2f);
				Invoke("invokeStopTimer", 0.05f);
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount*2, 17, 1), new GUIContent("+.01", "")))
			{
				SetTimeScale(0.04f);
				Invoke("invokeStopTimer", 0.01f);
			}

			// restart
			baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 3, 2);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 9, 1), new GUIContent("Restart", "")))
				CreateInstanceEffect();
		}
	}

	void invokeStopTimer()
	{
		SetTimeScale(0);
	}

	// -------------------------------------------------------------------------------------------
	public void RunActionControl()
	{
		RunActionControl(m_nPlayIndex, m_nTransIndex);
	}

	protected void RunActionControl(int nPlayIndex, int nTransIndex)
	{
		NgUtil.LogDevelop("RunActionControl() - nPlayIndex " + nPlayIndex);

		CancelInvoke();
		m_bCalledDelayCreate = false;

		// ������ �ð��� ���� �����̵��� �ص���...
		ResumeTimeScale();

		// Play ---------------------------------------
		m_bStartAliveAnimation = false;
		switch (nPlayIndex)
		{
			/*AutoRet*/
			case 0: break;
			case 1: break;
			/*OneShot*/
			case 2: SetTimeScale(m_fPlayToolbarTimes[nPlayIndex]);	break;
			/*Repeat*/
			case m_nRepeatIndex:
			case m_nRepeatIndex+1:
			case m_nRepeatIndex+2:
			case m_nRepeatIndex+3:
			case m_nRepeatIndex+4: {
						// repeat�����̸�.. �̵����� �ɼ��� ����.
						if (nPlayIndex != m_nPlayIndex)
							nTransIndex = 0;
						break;
					}
		}

		// Trans ---------------------------------------
		if (0 < nTransIndex)
		{
			// init multishot
// 			if (m_nTransIndex != nTransIndex && (m_nTransIndex != 0 && nTransIndex != 0))
// 				m_nMultiShotCount = 1;

			float				fTransHalfDist	= (Camera.main != null ? (Vector3.Magnitude(Camera.main.transform.position) * 0.8f) : 1) * m_fTransRate;
			GameObject			instEffectObject = FxmTestMain.inst.GetInstanceEffectObject();
			GameObject			simRoot			= NgObject.CreateGameObject(instEffectObject.transform.parent.gameObject, "simulate");
			FxmTestSimulate	simComponent	= simRoot.AddComponent<FxmTestSimulate>();
			instEffectObject.transform.parent	= simRoot.transform;
			FxmTestMain.inst.ChangeRoot_InstanceEffectObject(simRoot);
			simComponent.Init(this, m_nMultiShotCount);

			switch (nTransIndex)
			{
				case 1:	// Move
					{
						simComponent.SimulateMove(m_nTransAxis, fTransHalfDist, m_fDistPerTime, (m_nRotateIndex == 0));
						break;
					}
				case 2:	// scale
					{
						simComponent.SimulateScale(m_nTransAxis, fTransHalfDist*0.3f, m_fStartPosition, m_fDistPerTime, (m_nRotateIndex == 0));
						break;
					}
				case 3:	// Arc
					{
						simComponent.SimulateArc(fTransHalfDist*0.7f, m_fDistPerTime, (m_nRotateIndex == 0));
						break;
					}
				case 4:	// fall
					{
						simComponent.SimulateFall(fTransHalfDist*0.7f, m_fDistPerTime, (m_nRotateIndex == 0));
						break;
					}
				case 5:	// raise
					{
						simComponent.SimulateRaise(fTransHalfDist*0.7f, m_fDistPerTime, (m_nRotateIndex == 0));
						break;
					}
				case 6:	// circle
					{
						simComponent.SimulateCircle(fTransHalfDist*0.5f, m_fDistPerTime, (m_nRotateIndex == 0));
						break;
					}
				case 7:	// tornado
					{
						simComponent.SimulateTornado(fTransHalfDist*0.3f, fTransHalfDist*0.7f, m_fDistPerTime, (m_nRotateIndex == 0));
						break;
					}
			}
		}

		// �̵�ó�����̸�... �ڵ��ݺ��ǹǷ� ����repeat�ɼ��� ����.
		if (0 < nTransIndex)
			if (m_nRepeatIndex <= nPlayIndex)
				nPlayIndex = 0;

		// ���� ������ ����
		m_nPlayIndex	= nPlayIndex;
		m_nTransIndex	= nTransIndex;

		if (IsRepeat())
			m_fCreateTime = Time.time;
	}

	public void OnActionTransEnd()
	{
		DelayCreateInstanceEffect(true);
	}

	// front rotattion
	void RotateFront(Transform target)
	{
		Quaternion	quat	= FxmTestMain.inst.GetOriginalEffectObject().transform.localRotation;
		Vector3		rot		= quat.eulerAngles;

		switch (m_nRotateIndex)
		{
			case 0: break;	// defult
			case 1: rot.y += 90;	break;	// Front Z+
			case 2: rot.y -= 90;	break;	// Front Z-
			case 3: rot.z -= 90;	break;	// Front Y+
		}
		quat.eulerAngles = rot;
		target.localRotation = quat;
	}

	void DelayCreateInstanceEffect(bool bEndMove)
	{
// 		Debug.Log("DelayCreateInstanceEffect");
		m_bCalledDelayCreate = true;
		Invoke("NextInstanceEffect", (bEndMove ? 3 : 1) * m_fDelayCreateTime);
	}

	void NextInstanceEffect()
	{
		if (FxmTestMain.inst.m_bAutoChange)
			FxmTestMain.inst.ChangeEffect(true);
		else CreateInstanceEffect();
	}

	void CreateInstanceEffect()
	{
//		Debug.Log("CreateInstanceEffect");
		if (FxmTestMain.inst.IsCurrentEffectObject())
			FxmTestMain.inst.CreateCurrentInstanceEffect(true);
	}

	// -------------------------------------------------------------------------------------------
	void SetTimeScale(float timeScale)
	{
		if (m_fTimeScale != timeScale || m_fTimeScale != Time.timeScale)
		{
			if (timeScale == 0 && m_fTimeScale != 0)
				m_fOldTimeScale = m_fTimeScale;
			m_fTimeScale = timeScale;
			if (0.01f <= m_fTimeScale)	// �ٽ� ���������� ������������ �������ϵ���..
				PlayerPrefs.SetFloat("FxmTestControls.m_fTimeScale", m_fTimeScale);
			Time.timeScale = m_fTimeScale;
		}
	}

	public void ResumeTimeScale()
	{
		if (m_fTimeScale == 0)
			SetTimeScale(m_fOldTimeScale);
	}

}

