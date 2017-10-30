using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QInputScene : MonoBehaviour {

	public InputField inputField;
	public Toggle toggle;

	private const string key_serverip = "key_serverip";
	// Use this for initialization
	void Start () {
	
		QGlobal.serverIp = PlayerPrefs.GetString (key_serverip,"localhost");

		inputField.text = QGlobal.serverIp;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnFinishEdit(string content){
		
		QGlobal.serverIp = inputField.text.Trim ();
		Debug.Log (QGlobal.serverIp+": >>>>content>>>");
		PlayerPrefs.SetString (key_serverip,QGlobal.serverIp);
	}

	public void OnToggleChanged(bool value){
	
		QGlobal.isWifiEnable= toggle.isOn;
		Debug.Log (toggle.isOn+">>>>>>>value *********");
	}
	public void StartGame(){

		UnityEngine.SceneManagement.SceneManager.LoadScene ("client");
	}
}
