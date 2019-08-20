using System.Collections;
using System.Collections.Generic;
using QF;
using UnityEngine;
using UniRx;

public class OnAnimationDone {}

public class A
{
	public A()
	{
		SimpleEventSystem.GetEvent<OnAnimationDone>()
			.Subscribe(_=>{
				// ...
			});

		var b = new B();
		b.PlayAnimation();
	}
}

public class B
{
	public void PlayAnimation()
	{

	}
}
