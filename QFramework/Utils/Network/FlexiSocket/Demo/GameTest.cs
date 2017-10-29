//using UnityEngine;
//using System.Collections;
//using QGame.BLE;
//
//public class GameTest : MonoBehaviour, IActionListener
//{
//	// Use this for initialization
//	void Start()
//	{
//		this.GetComponent<Server>().RegisterActionListener(this);
//
//		StartCoroutine(SetupReceiverPage());
//	}
//
//	// Update is called once per frame
//	void Update(){}
//
//	public void OnAction(QSimulateAction action)
//	{
//		switch (action)
//		{
//			case QSimulateAction.LEFT:
//				this.transform.position = Vector3.left * 2;
//				break;
//			case QSimulateAction.RIGHT:
//				this.transform.position = Vector3.right * 2;
//				break;
//			case QSimulateAction.UP:
//				this.transform.position = Vector3.up * 2;
//				break;
//			case QSimulateAction.DOWN:
//				this.transform.position = Vector3.down * 2;
//				break;
//
//		}
//	}
//
//	bool mConnected = false;
//	bool mConnecting = false;
//	string mConnectedID = null;
//	string mServiceUUID = "713D1235-503E-4C75-BA94-3148F18D941E";
//	string mReadCharacteristicUUID = "9999";
//	string mWriteCharacteristicUUID = "0000";
//
//	IEnumerator SetupReceiverPage()
//	{
//
//		//作为中心设备初始化
//		BluetoothLEHardwareInterface.Initialize(true, false, delegate
//		{
//			Debug.Log("开始扫描:");
//			// the first callback will only get called the first time this device is seen
//			// this is because it gets added to a list in the BluetoothDeviceScript
//			// after that only the second callback will get called and only if there is
//			// advertising data available
//			// 扫描外围设备
//			ScanController();
//		}, delegate(string error)
//		{
//
//		});
//
//		yield return null;
//	}
//
//	void ScanController()
//	{
//		BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(new string[] {mServiceUUID},
//			delegate(string address, string name)
//			{
//				Debug.Log("扫描到 服务:" + address + " :" + name);
//
//				if (!mConnecting)
//				{
//					BluetoothLEHardwareInterface.StopScan();
//
//					if (mConnected)
//					{
//						Debug.Log("取消连接1");
//
//						BluetoothLEHardwareInterface.DisconnectPeripheral(address, delegate(string obj)
//						{
//							mConnected = false;
//							Debug.Log("取消连接2");
//						});
//
//
//					}
//					else
//					{
//						Debug.Log("开始连接");
//
//						BluetoothLEHardwareInterface.ConnectToPeripheral(address, delegate(string addressName)
//						{
//
//							Debug.LogWarning("连接外围设备成功:" + addressName);
//
//						}, delegate(string addressName, string serviceUUID)
//						{
//							Debug.LogWarning("连接外围设备成功:" + serviceUUID);
//						}, delegate(string addressName, string serviceUUID, string characteristicUUID)
//						{
//							Debug.LogWarning("接收特征:" + characteristicUUID);
//
//							// discovered characteristic
//							if (IsEqual(serviceUUID, mServiceUUID))
//							{
//								mConnectedID = addressName;
//
//								mConnected = true;
//
//
//								if (IsEqual(characteristicUUID, mReadCharacteristicUUID))
//								{
//									BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(mConnectedID, mServiceUUID,
//										mReadCharacteristicUUID,
//										delegate(string deviceAddress, string notification)
//										{
//
//										}, delegate(string deviceAddress2, string characteristic, byte[] data)
//										{
//
//											if (deviceAddress2.CompareTo(mConnectedID) == 0)
//											{
//												if (IsEqual(characteristicUUID, mReadCharacteristicUUID))
//												{
//													string msg = System.Text.UTF8Encoding.Default.GetString(data);
//													Debug.Log("接收数据:" + System.Text.UTF8Encoding.Default.GetString(data));
//													GetComponent<Server>().SendMessage("OnReceivedFromBLEClient", msg);
//
//												}
//											}
//
//										});
//								}
//							}
//						}, delegate(string addressName)
//						{
//							Debug.LogError("取消连接");
//							// this will get called when the device disconnects
//							// be aware that this will also get called when the disconnect
//							// is called above. both methods get call for the same action
//							// this is for backwards compatibility
//							mConnected = false;
//							mConnecting = false;
//							if (!mDeinitialized)
//							{
//								ScanController();
//							}
//						});
//
//						mConnecting = true;
//					}
//				}
//
//			}, delegate(string address, string name, int rssi, byte[] advertisingInfo)
//			{
//				if (advertisingInfo != null)
//					BluetoothLEHardwareInterface.Log(string.Format("Device: {0} RSSI: {1} Data Length: {2} Bytes: {3}", name, rssi,
//						advertisingInfo.Length, QBLEUtil.BytesToString(advertisingInfo)));
//			});
//	}
//
//	bool mDeinitialized = false;
//
//	void OnDestroy()
//	{
//		BluetoothLEHardwareInterface.DeInitialize(delegate
//		{
//			mDeinitialized = true;
//			mConnected = false;
//			mConnecting = false;
//		});
//
//	}
//
//
//	bool IsEqual(string uuid1, string uuid2)
//	{
//		if (uuid1.Length == 4)
//			uuid1 = FullUUID(uuid1);
//		if (uuid2.Length == 4)
//			uuid2 = FullUUID(uuid2);
//
//		return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
//	}
//
//	string FullUUID(string uuid)
//	{
//		return "0000" + uuid + "-0000-1000-8000-00805f9b34fb";
//	}
//}