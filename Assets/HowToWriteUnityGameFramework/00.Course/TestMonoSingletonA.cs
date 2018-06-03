using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class TestMonoSingletonA : MonoBehaviour {

	// Use this for initialization
	private void Start()
	{
		var playerData = PlayerDataMgr.GetPlayerData();

		playerData.Level++;

		PlayerDataMgr.SavePlayerData();		
	}

	// Update is called once per frame
	void Update () {
		
	}
}
