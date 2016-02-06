// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class FxmTestSingleMain : MonoBehaviour
{
	// -------------------------------------------------------------------------------------------
	public		GameObject[]	m_EffectPrefabs		= new GameObject[1];
	public		GUIText			m_EffectGUIText;
	public		int				m_nIndex;
	public		float			m_fCreateScale		= 1.0f;
	public		int				m_nCreateCount		= 1;
	public		float			m_fRandomRange		= 1;
	public		FxmTestSingleMouse			m_FXMakerMouse;

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
	}

	void OnEnable()
	{
	}

	void Start()
	{
// 		m_EffectPrefab = (GameObject)Resources.Load("test", typeof(GameObject));
// 		NcEffectBehaviour.PreloadTexture(m_EffectPrefab);
		Resources.UnloadUnusedAssets();
		Invoke("CreateEffect", 1);
	}

	void CreateEffect()
	{
		if (m_EffectPrefabs[m_nIndex] == null)
			return;

		if (m_EffectGUIText != null)
			m_EffectGUIText.text = m_EffectPrefabs[m_nIndex].name;

		float fRandomRange = 0;
		if (1 < m_nCreateCount)
			fRandomRange = m_fRandomRange;

		for (int n = 0; n < GetInstanceRoot().transform.childCount; n++)
			Destroy(GetInstanceRoot().transform.GetChild(n).gameObject);
		for (int n = 0; n < m_nCreateCount; n++)
		{
			GameObject createObj = (GameObject)Instantiate(m_EffectPrefabs[m_nIndex], new Vector3(Random.Range(-fRandomRange, fRandomRange), 0, Random.Range(-fRandomRange, fRandomRange)), Quaternion.identity);
			createObj.transform.localScale = createObj.transform.localScale * m_fCreateScale;
			NsEffectManager.PreloadResource(createObj);
			createObj.transform.parent = GetInstanceRoot().transform;
#if (!UNITY_3_5)
			SetActiveRecursively(createObj, true);
#endif
		}
	}

	void Update()
	{
	}

	void OnGUI()
	{
		// distance
//		float fButtonWidth	= Screen.width/7;
		float fButtonHeight	= Screen.height/10;
		float dist = GUI.VerticalSlider(new Rect(10, fButtonHeight+10+30, 25, Screen.height - (fButtonHeight+10+50)), GetFXMakerMouse().m_fDistance, GetFXMakerMouse().m_fDistanceMin, GetFXMakerMouse().m_fDistanceMax);
		if (dist != GetFXMakerMouse().m_fDistance)
			GetFXMakerMouse().SetDistance(dist);

		if (GUI.Button(GetButtonRect(0), "Next"))
		{
			if (m_nIndex < m_EffectPrefabs.Length-1)
				m_nIndex++;
			else m_nIndex = 0;
			CreateEffect();
		}
		if (GUI.Button(GetButtonRect(1), "Recreate"))
			CreateEffect();
	}

	public FxmTestSingleMouse GetFXMakerMouse()
	{
		if (m_FXMakerMouse == null)
			m_FXMakerMouse = GetComponentInChildren<FxmTestSingleMouse>();
		return m_FXMakerMouse;
	}

	public GameObject GetInstanceRoot()
	{
		return NcEffectBehaviour.GetRootInstanceEffect();
	}

	public static Rect GetButtonRect()
	{
		int		nButtonCount = 2;
		return new Rect(Screen.width-Screen.width/10*nButtonCount, Screen.height-Screen.height/10, Screen.width/10*nButtonCount, Screen.height/10);
	}
	public static Rect GetButtonRect(int nIndex)
	{
		return new Rect(Screen.width-Screen.width/10*(nIndex+1), Screen.height-Screen.height/10, Screen.width/10, Screen.height/10);
	}

	public static void SetActiveRecursively(GameObject target, bool bActive)
	{
#if (!UNITY_3_5)
		for (int n = target.transform.childCount-1; 0 <= n; n--)
			if (n < target.transform.childCount)
				SetActiveRecursively(target.transform.GetChild(n).gameObject, bActive);
		target.SetActive(bActive);
#else
		target.SetActiveRecursively(bActive);
#endif
	}
}


