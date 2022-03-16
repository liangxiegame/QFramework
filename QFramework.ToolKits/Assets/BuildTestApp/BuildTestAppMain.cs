using System;
using QFramework;
using UnityEngine;

namespace BuildTestApp
{
    public class BuildTestAppMain : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            "Hello World".LogInfo();

            Action a = () => { "Invoked".LogInfo(); };
            a();
        }
    }
}