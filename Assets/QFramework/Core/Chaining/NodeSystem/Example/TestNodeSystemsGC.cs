using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class TestNodeSystemsGC : MonoBehaviour
{
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			this.Sequence()
				.Delay(3.0f)
				.Event(() => Debug.Log("A"))
				.Begin()
				.OnDisposed(() => Log.I("Sequece: dispose when sequence ended"));
		}
	}
}