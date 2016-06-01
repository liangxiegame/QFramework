using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class AssetBundleLoad : MonoBehaviour {

	[SerializeField]
	AudioSource mAudioSource;

	// Use this for initialization
	void Start () {
//		StartCoroutine (LoadPrefab ());
	}


//	void OnGUI()
//	{
//		if (GUILayout.Button ("LoadScene")) {
//			StartCoroutine (LoadScene ());
//		}
//	}


	IEnumerator LoadPrefab()
	{
		WWW download = new WWW ("file://" + Application.streamingAssetsPath + "/cube.assetbundle");
		yield return download;

//		GameObject.Instantiate(download.assetBundle.LoadAsset<GameObject>("cube"));
		var go = GameObject.Instantiate(download.assetBundle.mainAsset) as GameObject;
		go.SetActive (false);

		WWW material = new WWW ("file://" + Application.streamingAssetsPath + "/ma.assetbundle");

		yield return material;
		go.GetComponent<MeshRenderer> ().material = material.assetBundle.LoadAsset<Material> ("ma");
		go.SetActive (true);


		yield return LoadMusic ();
	}

	IEnumerator LoadMusic()
	{
		WWW download = new WWW ("file://" + Application.streamingAssetsPath + "/test.assetbundle");
		yield return download;

		mAudioSource.clip = download.assetBundle.mainAsset as AudioClip;

		mAudioSource.Play ();
	}

	IEnumerator LoadScene()
	{
		WWW download = new WWW ("file://" + Application.streamingAssetsPath + "/scene.assetbundle");
		yield return download;


		// 一定要Log一次 否则会报错 Bug
		Debug.LogWarning (download.assetBundle.name);
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Scene");
	}

}
