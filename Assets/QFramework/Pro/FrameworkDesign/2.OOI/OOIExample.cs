using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QF.Master.Example
{
	public class OOIExample : MonoBehaviour
	{
		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}

	class A
	{
		public void Say()
		{
			new B().Say();
		}
	}

	class B
	{
		public void Say()
		{
			Debug.Log("hello B");
		}
	}
}