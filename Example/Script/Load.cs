using UnityEngine;
using System.Collections;

public class Load : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (LoadPrefab ());
	}

	IEnumerator LoadPrefab()
	{

//		#if unity_android
//		string url = Application.streamingAssetsPath + "/cube.assetbundle");
//		#elif
//		string url = "file://" + Application.streamingAssetsPath + "/cube.assetbundle");
//		#endif

		WWW download = new WWW ("file://" + Application.streamingAssetsPath + "/cube.assetbundle");
		yield return download;

		GameObject.Instantiate(download.assetBundle.LoadAsset<GameObject>("cube"));
	}
	

}
