using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace QFramework.Example
{
    public class RandomDelayExample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            ActionKit.Repeat()
                .Delay(() => Random.Range(0.5f, 2.5f))
                .Callback(() => { Debug.Log("Time:" + Time.time); })
                .Start(this);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}