using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace QFramework.ResKit.Tests
{
	public class ResKitTests
	{

		[Test]
		public void ResKit_TestPlatformName()
		{
			var platformName = FromUnityToDll.Setting.GetPlatformName();
			var abPlatformName = SettingFromUnityToDll.GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
			
			Assert.AreEqual(platformName,abPlatformName);
		}

		// A UnityTest behaves like a coroutine in PlayMode
		// and allows you to yield null to skip a frame in EditMode
		[UnityTest]
		public IEnumerator ResKitTestsWithEnumeratorPasses()
		{
			// Use the Assert class to test conditions.
			// yield to skip a frame
			yield return null;
		}
	}
}