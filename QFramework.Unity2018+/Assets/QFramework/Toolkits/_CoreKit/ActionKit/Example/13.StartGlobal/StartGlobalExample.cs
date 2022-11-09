using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class StartGlobalExample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

            var action = ActionKit.Repeat()
                .Delay(1.0f)
                .Callback(() => Debug.Log("wait"))
                .StartGlobal();
            
            // action.Pause();
            // action.Resume();
            // action.Deinit(); // Stop
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}