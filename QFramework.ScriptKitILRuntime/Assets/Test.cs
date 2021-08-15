using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            print(assembly.GetName().Name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
