using UnityEngine;
using System.Collections;

public class iOSTest : MonoBehaviour {

	private string label2;
	private string label1;

	void OnGUI()
	{
		if (GUI.Button(new Rect(0,0,100,50),"send"))
		{
			label1 = UnityToiOS.Receive ("Hello", "world");
		}

		if (!string.IsNullOrEmpty(label1)) {
			GUI.Label(new Rect(0,100,100,50),label1);
		}
		if (!string.IsNullOrEmpty (label2)) {
			GUI.Label (new Rect (0, 50, 100, 50), label2);
		}
	}


	public void unityGet(string str2)
	{
		Debug.LogWarning ("main camera @@@@@@@" + str2);

		label2 = str2;
	}
}
