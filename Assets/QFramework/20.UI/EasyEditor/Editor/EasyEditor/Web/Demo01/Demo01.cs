using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demonstrate calling csharp script by javascript which is within HTML file.
/// 
/// See Assets/HTML/index.html to know how it works.
/// 
/// </summary>
public class Demo01 : MonoBehaviour
{
    public float speed = 50f;
    	
	void Update ()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
