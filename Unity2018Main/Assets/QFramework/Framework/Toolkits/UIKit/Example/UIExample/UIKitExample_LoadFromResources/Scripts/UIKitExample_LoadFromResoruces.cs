using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.Example;
using UnityEngine;

public class UIKitExample_LoadFromResoruces : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{

		UIKit.OpenPanel<UISomePanelFromResources>(prefabName: "resources://UISomePanelFromResources");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
