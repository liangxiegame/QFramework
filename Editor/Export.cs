using UnityEngine;
using System.Collections;
using UnityEditor;

public class Export : MonoBehaviour {

	[MenuItem("QFramework/Prefab")]
	static void ExportPrefab()
	{
		Object prefab = AssetDatabase.LoadMainAssetAtPath ("Assets/QFramework/Cube.prefab");

		string savePath = Application.dataPath + "/StreamingAssets/cube.assetbundle";

		BuildPipeline.BuildAssetBundle (prefab,null,savePath,BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
			BuildTarget.StandaloneOSXUniversal);
	}
}
