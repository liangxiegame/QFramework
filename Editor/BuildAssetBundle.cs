using UnityEngine;
using System.Collections;
using UnityEditor;

public class BuildAssetBundle : MonoBehaviour {

	[MenuItem("QFramework/BuildAssetBundleCube")]
	static void BuildAssetBundleCube()
	{
		Object prefab = AssetDatabase.LoadMainAssetAtPath ("Assets/QFramework/Cube.prefab");

		string savePath = Application.dataPath + "/StreamingAssets/cube.assetbundle";

		BuildPipeline.BuildAssetBundle (prefab,null,savePath,BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
			BuildTarget.StandaloneOSXUniversal);
	}

	[MenuItem("QFramework/BuildAssetBundleMusic")]
	static void BuildAssetBundleMusic()
	{
		Object prefab = AssetDatabase.LoadMainAssetAtPath ("Assets/QFramework/Test.mp3");

		string savePath = Application.dataPath + "/StreamingAssets/test.assetbundle";

		BuildPipeline.BuildAssetBundle (prefab,null,savePath,BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
			BuildTarget.StandaloneOSXUniversal);
	}

	[MenuItem("QFramework/BuildAssetBundleScene")]
	static void ExportScene()
	{
		string[] scenes = { "Assets/QFramework/Scene.unity" };

		string savePath = Application.dataPath + "/StreamingAssets/scene.assetbundle";

		BuildPipeline.BuildStreamedSceneAssetBundle (scenes, savePath, BuildTarget.StandaloneOSXIntel);
	}
}
