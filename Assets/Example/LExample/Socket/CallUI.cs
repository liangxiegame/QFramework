using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using QFramework.Example;

public class CallUI : MonoBehaviour {

	void Start () {

        ResMgr.Init();

        UIMgr.OpenPanel<UIConnect>(UILevel.PopUI);

        QEventSystem.RegisterEvent(101, (x, y) => { UIMgr.ClosePanel<UIConnect>(); UIMgr.OpenPanel<UIMsg>(); });
	}
	
}
