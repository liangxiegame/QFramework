using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class EventGet : MonoBehaviour {

	
	void Start () {

        QEventSystem.RegisterEvent(TestEvent.TestOne,GetEvent);

	}

    void GetEvent(int key, params object[] obj)
    {
        switch (key)
        {
            case (int)TestEvent.TestOne:
                this.LogInfo(obj[0].ToString());
                break;
        }
    }
}
