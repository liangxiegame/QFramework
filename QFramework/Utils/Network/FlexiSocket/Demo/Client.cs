// ************************
// Author: Sean Cheung
// Create: 2016/06/07/10:31
// Modified: 2016/06/08/14:46
// ************************

using System;
using System.Collections;

using System.Net.Sockets;
using System.Text;
using FlexiFramework.Networking;
using UnityEngine;

public class Client : MonoBehaviour
{
	private ISocketClient mSocketClient;
	string mCurIP;
	string mCurDeviceName;

	// Use this for initialization
	private IEnumerator Start()
	{
		if (!string.IsNullOrEmpty(QGlobal.serverIp))
		{
			ConnectToServer(QGlobal.serverIp);
		}
		yield return SetupControllerPage();
	}

	private IEnumerator ConnectToServer(string serverIp)
	{
		if (!isWifiConnected && !isWifiConnecting)
		{
			Debug.Log(serverIp + ":-------  .serverip>>>>>>>>>>>");
			mSocketClient = FlexiSocket.Create(serverIp, 1366, Protocols.BodyLengthPrefix);
			mSocketClient.Connected += OnConnected;
			mSocketClient.Disconnected += OnDisconnected;
			mSocketClient.Received += OnReceived;
			mSocketClient.Sent += OnSent;
			yield return new WaitForSeconds(1);
			mSocketClient.Connect();
		}
	}


	private void OnSent(bool success, Exception exception, SocketError error)
	{
		if (success)
			Debug.Log("Sent to server", this);
	}

	private void OnReceived(bool success, Exception exception, SocketError error, byte[] message)
	{
		if (success)
			Debug.Log("Received from server: " + Encoding.UTF8.GetString(message), this);
	}

	bool isWifiConnected = false;
	bool isWifiConnecting = false;

	private void OnDisconnected(bool success, Exception exception)
	{

		if (success)
		{
			Debug.Log("Disconnected", this);
			isWifiConnected = false;
			isWifiConnecting = false;
		}
	}

	private void OnConnected(bool success, Exception exception)
	{
		Debug.Log("Connecting result: " + success, this);
		if (success)
		{
			mSocketClient.Send(Encoding.UTF8.GetBytes("Let me join"));
			isWifiConnected = true;
			isWifiConnecting = false;
		}
	}

	private void OnDestroy()
	{
		mSocketClient.Close();
		mSocketClient = null;
		UnsubscribeEvent();

//		BluetoothLEHardwareInterface.DeInitialize (delegate {


//		});
	}

	private void Reset()
	{
		name = "Client";
	}

	void OnDisable()
	{
		UnsubscribeEvent();

	}

	void OnGUI()
	{
		if (!string.IsNullOrEmpty(mCurIP))
		{
			GUI.Label(new Rect(100, 100, 200, 100), mCurIP);

			if (!isWifiConnected && GUI.Button(new Rect(100, 400, 300, 200), "ConnectServer"))
			{
				StartCoroutine(ConnectToServer(mCurIP));
			}
		}

		if (!string.IsNullOrEmpty(mCurDeviceName))
		{
			GUI.Label(new Rect(100, 200, 200, 100), mCurDeviceName);
		}
	}

	void OnEnable()
	{
//		EasyTouch.On_SwipeStart += On_SwipeStart;
//		EasyTouch.On_Swipe += On_Swipe;
//		EasyTouch.On_SwipeEnd += On_SwipeEnd;		
	}

	void UnsubscribeEvent()
	{
//		EasyTouch.On_SwipeStart -= On_SwipeStart;
//		EasyTouch.On_Swipe -= On_Swipe;
//		EasyTouch.On_SwipeEnd -= On_SwipeEnd;	
	}

//	// At the swipe beginning 
//	private void On_SwipeStart( Gesture gesture){
//		Debug.Log("You start a swipe");
//	}
//
//	// During the swipe
//	private void On_Swipe(Gesture gesture){
//
//		// the world coordinate from touch for z=5
////		Vector3 position = gesture.GetTouchToWorldPoint(5);
////		trail.transform.position = position;
//
//	}
//
//	// At the swipe end 
//	private void On_SwipeEnd(Gesture gesture){
//		if (QGlobal.isWifiEnable) {
//			if (isWifiConnected) {
//				_client.Send (Encoding.UTF8.GetBytes (gesture.swipe.ToString ()));
//			}
//		} else {
//			if (isBLEReady) {
//				byte[] sendBytes = System.Text.UTF8Encoding.Default.GetBytes(gesture.swipe.ToString());
//				BluetoothLEHardwareInterface.UpdateCharacteristicValue (mReadCharacteristicUUID,sendBytes , sendBytes.Length);
//			}
//		}
//
//
//		// Get the swipe angle
////		float angles = gesture.GetSwipeOrDragAngle();
////		Debug.Log("Last swipe : " + gesture.swipe.ToString() + " /  vector : " + gesture.swipeVector.normalized + " / angle : " + angles.ToString("f2"));
//	}


	private bool isBLEReady = false;

	IEnumerator SetupControllerPage()
	{
//		// 作为外围设备初始化
//		BluetoothLEHardwareInterface.Initialize (false, true, delegate {
//			Debug.Log("设置名称");
//			// 设置名称
//			BluetoothLEHardwareInterface.PeripheralName ("Simulated SuperRun");
//
//			Debug.Log("设置可读特征");
//			// 设置可读特征 (可以被其他设备读取) 就是发送消息
//			BluetoothLEHardwareInterface.CreateCharacteristic (mReadCharacteristicUUID, 
//				BluetoothLEHardwareInterface.CBCharacteristicProperties.CBCharacteristicPropertyRead |
//				BluetoothLEHardwareInterface.CBCharacteristicProperties.CBCharacteristicPropertyNotify, 
//				BluetoothLEHardwareInterface.CBAttributePermissions.CBAttributePermissionsReadable, null, 0, null);
//		
//			// 设置可写特征
//			BluetoothLEHardwareInterface.CreateCharacteristic (mWriteCharacteristicUUID, BluetoothLEHardwareInterface.CBCharacteristicProperties.CBCharacteristicPropertyWrite,  
//				BluetoothLEHardwareInterface.CBAttributePermissions.CBAttributePermissionsWriteable, null, 0, 
//				(characteristicUUID, bytes) => {
//
//					if (bytes.Length > 0)
//					{
//						string receivedMsg = Encoding.UTF8.GetString(bytes);
//						Debug.LogWarning(receivedMsg);
//						string[] splits = receivedMsg.Split(":".ToCharArray());
//						if (splits.Length == 4) {
//							mCurIP = splits[1];
//							mCurDeviceName = splits[3];
//						}
//					}
//				});
//			
//			Debug.Log("创建服务");
//			// 创建服务,一个设备有多个服务,一个服务可以有多个特征
//			BluetoothLEHardwareInterface.CreateService (mServiceUUID, true, delegate(string uuid) {
//				Debug.Log("创建服务成功 开始准备广播:");
//
//				// 服务创建成功后 开始广播
//				BluetoothLEHardwareInterface.StartAdvertising (delegate {
//					isBLEReady = true;
//				});
//			});
//				
//		} ,delegate(string error) {
//			Debug.LogError(error);
//		});
		yield return new WaitForSeconds(0.5f);
	}

	string mServiceUUID = "713D1235-503E-4C75-BA94-3148F18D941E";
	string mReadCharacteristicUUID = "9999";
	string mWriteCharacteristicUUID = "0000";
}