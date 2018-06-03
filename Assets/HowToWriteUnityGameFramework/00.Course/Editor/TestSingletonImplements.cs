using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using QFramework;

public class TestSingletonImplements
{
	[Test]
	public void EditorTest()
	{
		var aSingelton1 = ASingleton.Instance;
		var aSingelton2 = ASingleton.Instance;
		
		
		Assert.AreEqual(aSingelton1, aSingelton2);
		Assert.AreEqual(aSingelton1.GetHashCode(), aSingelton2.GetHashCode());
	}
}

/// <summary>
/// 1. 反射
/// 2. 泛型
/// </summary>

public class ASingleton : Singleton<ASingleton>
{
	private ASingleton()
	{
	}
}