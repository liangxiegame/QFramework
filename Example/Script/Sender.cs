using UnityEngine;
using System.Collections;
using QFramework;

public class Sender : MonoBehaviour,IMsgSender {


	[SerializeField]
	Receiver mReceiver;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.SendLogicMsg ("Receiver Show Sth","你好","世界");
	}



}
