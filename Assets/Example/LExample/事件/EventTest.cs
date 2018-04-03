using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace QFramework
{
    public enum TestEvent
    {
        TestOne=QMgrID.Game,
    }

    public class EventTest : MonoBehaviour
    {       
        void Start()
        {
            Observable.EveryUpdate()
                .Where(x => Input.GetKeyDown(KeyCode.S))
                .Subscribe(_=>QEventSystem.SendEvent(TestEvent.TestOne,"Hello world"));
        }
    }

}
