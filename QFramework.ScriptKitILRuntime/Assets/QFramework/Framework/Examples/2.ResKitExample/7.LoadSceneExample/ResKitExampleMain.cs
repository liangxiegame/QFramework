using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFramework.Example
{
	public class ResKitExampleMain : MonoBehaviour
	{
		void Awake()
		{
			ResMgr.Init();
		}

		// Use this for initialization
		void Start()
		{
			var resLoader = ResLoader.Allocate();

			resLoader.Add2Load("BattleScene");

			resLoader.LoadAsync(() => { SceneManager.LoadScene("BattleScene"); });
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}