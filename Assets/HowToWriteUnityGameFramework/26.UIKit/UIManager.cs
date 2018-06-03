using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class UIManager : QMonoSingleton<UIManager>
{
	public GameObject UIMainPanelPrefab;
	public GameObject UILevelPanelPrefab;
	public GameObject UIGamePanelPrefab;
	public GameObject UIGamePausePanelPrefab;
	public GameObject UIGameOverPanelPrefab;
	public GameObject UIAboutPanelPrefab;
	public GameObject UIRolePanelPrefab;


	private Dictionary<string, GameObject> mPanels = new Dictionary<string, GameObject>();
	
	public void Open(GameObject panelPrefab)
	{
		var panelGameObj = Instantiate(panelPrefab);
		panelGameObj.transform.SetParent(transform);
		panelGameObj.transform.localPosition = Vector3.zero;
		panelGameObj.transform.localScale = Vector3.one;
		panelGameObj.transform.localRotation = Quaternion.identity;
		panelGameObj.SetActive(true);
		
		mPanels.Add(panelPrefab.name,panelGameObj);
	}

	public void Close(GameObject panelPrefab)
	{
		Destroy(mPanels[panelPrefab.name]);
	}
}


public class UICtrlTest
{
	void Test()
	{
		UIManager.Instance.Open(UIManager.Instance.UIMainPanelPrefab);

		UIManager.Instance.Close(UIManager.Instance.UIMainPanelPrefab);
	}
}