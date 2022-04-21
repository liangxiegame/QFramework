using QFramework.NetworkExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkTest : MonoBehaviour
{
    NetworkingUtility networking;
    void Start()
    {
        networking= this.gameObject.AddComponent<NetworkingUtility>();
        networking.Init("玩家 1","1");
        networking.onConnectedEvent += () =>
        {
            networking.JoinOrCreateRoom("一夜暴富", 255, new string[2] { "1", "2"});
        };
        networking.SetMasterClient();
    }
}
