using UnityEngine;
using System.Collections;

public class FxmReplayDemo : MonoBehaviour
{
	public	GameObject		m_TargetPrefab;
	public	GameObject		m_InstanceObj;

	void Start()
	{
		CreateEffect();
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void CreateEffect()
	{
		if (m_TargetPrefab == null)
			return;

		// Create from prefab.
		m_InstanceObj = NsEffectManager.CreateReplayEffect(m_TargetPrefab);
		NsEffectManager.PreloadResource(m_InstanceObj);
	}

	void Replay(bool bClearOldParticle)
	{
		NsEffectManager.RunReplayEffect(m_InstanceObj, bClearOldParticle);
	}

	void OnGUI()
	{
		if (GUI.Button(GetButtonRect(0), "Replay"))
			Replay(false);
		if (GUI.Button(GetButtonRect(1), "Replay(ClearParticle)"))
			Replay(true);
	}

	public static Rect GetButtonRect(int nIndex)
	{
		return new Rect(Screen.width-Screen.width/8*(nIndex+1), Screen.height-Screen.height/10, Screen.width/8, Screen.height/10);
	}

	public static void SetActiveRecursively(GameObject target, bool bActive)
	{
#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9)
		for (int n = target.transform.GetChildCount()-1; 0 <= n; n--)
			if (n < target.transform.GetChildCount())
				SetActiveRecursively(target.transform.GetChild(n).gameObject, bActive);
		target.SetActive(bActive);
#else
		target.SetActiveRecursively(bActive);
#endif
	}
}
