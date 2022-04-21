using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System;

namespace QFramework.NetworkExtension
{
    public class NetworkingUtility : MonoBehaviourPunCallbacks, INetworkingUtility
    {
        /// <summary>
        /// 用来区分当前版本，通常在升级的时候使用
        /// </summary>
        string gameVersion = "1";

        public event Action<INetEventData> EventReceived;
        public event Action<INetworkResponse> OpResponseReceived;
        public event Action<INetPlayer> OnPlayerLeftRoomEvent;
        public event Action<INetRoom> OnJoinedRoomEvent;
        public event Action onConnectedEvent;
        public event Action<INetPlayer, Dictionary<object, object>> OnPlayerPropertiesChangeEvent;
        public event Action OnJoinRoomFailedEvent;
        public event Action<INetPlayer> OnPlayerEnteredRoomEvent;
        public event Action OnLeaveRoomEvent;
        public event Action<Dictionary<object, object>> OnRoomPropertiesChangeEvent;
        public event Action<List<INetLobbyInfo>> onLobbysDataUpdate;
        public event Action<INetPlayer> onMasterClientSwitched;
        public event Action<List<INetRoomInfo>> onRoomListUpdate;
        public event Action<INetDisconnectCause> onDisconnected;
        public event Action<List<INetFriendInfo>> onFriendListUpdate;
        public event Action onJoinLobbyEvent;

        public int CountOfRooms
        {
            get
            {
                return PhotonNetwork.CountOfRooms;
            }
        }

        public int CountOfPlayersOnMaster
        {
            get
            {
                return PhotonNetwork.CountOfPlayersOnMaster;
            }
        }

        public int CountOfPlayersInRooms
        {
            get
            {
                return PhotonNetwork.CountOfPlayersInRooms;
            }
        }

        public int CountOfPlayers
        {
            get
            {
                return PhotonNetwork.CountOfPlayers;
            }
        }

        public int CurrentRoomPlayers
        {
            get
            {
                if (PhotonNetwork.InRoom)
                {
                    return PhotonNetwork.CurrentRoom.PlayerCount;
                }
                else
                {
                    return 0;
                }
    ;
            }
        }

        #region UNITY
        private void OnDestroy()
        {
            this.Disconnect();
        }



        public void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                //Debug.Log("当前房间数：" + PhotonNetwork.CountOfRooms);
                //Debug.Log("未加入房间的玩家数量：" + PhotonNetwork.CountOfPlayersOnMaster);
                //Debug.Log("房间内的玩家人数：" + PhotonNetwork.CountOfPlayersInRooms);
                //Debug.Log("已连接玩家总数：" + PhotonNetwork.CountOfPlayers);
                //if(PhotonNetwork.CurrentRoom!=null)
                //Debug.Log("当前房间名称：" + PhotonNetwork.CurrentRoom.Name);
                // PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");
            }

        }
        #endregion

        public void Init(string nickname = null, string userID = null, bool EnableLobbyStatistics = false)
        {
            PhotonNetwork.NetworkingClient.OpResponseReceived += OpReceived;
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
            PhotonNetwork.NetworkingClient.EnableLobbyStatistics = EnableLobbyStatistics;

            if (string.IsNullOrEmpty(userID) == false)
                PhotonNetwork.NetworkingClient.UserId = userID;
            if (string.IsNullOrEmpty(nickname) == false)
                this.SetPlayName(nickname);
            Connect();
        }

        private void OpReceived(OperationResponse operation)
        {
            switch (operation.OperationCode)
            {
                case 100:
                    INetworkResponse Response = new INetworkResponse(operation);
                    if (!string.IsNullOrEmpty(Response.DebugMessage))
                        OpResponseReceived?.Invoke(Response);
                    if (Response.ReturnCode == 0)
                    {
                        PhotonNetwork.JoinRandomRoom();
                    }
                    break;
            }

        }

        public void OnEvent(EventData photonEvent)
        {
            EventReceived?.Invoke(new INetEventData(photonEvent));
        }

        public bool IsConnectedAndReady
        {
            get
            {
                return PhotonNetwork.IsConnectedAndReady;
            }
        }

        public bool IsMessageQueueRunning
        {
            get
            {
                return PhotonNetwork.IsMessageQueueRunning;
            }
            set
            {
                PhotonNetwork.IsMessageQueueRunning = value;
            }
        }

        public void Send(byte eventCode, byte group, object message, IEventCaching caching = IEventCaching.DoNotCache)
        {
            Debug.Log(eventCode.ToString() + "   " + group.ToString());

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = Photon.Realtime.ReceiverGroup.Others,
                InterestGroup = group,
                CachingOption = (EventCaching)caching
            };
            PhotonNetwork.RaiseEvent(eventCode, message, raiseEventOptions, SendOptions.SendReliable);
        }

        public void Send(byte eventCode, byte group, NetReceiverGroupCode receiver, object message, IEventCaching caching = IEventCaching.DoNotCache)
        {
            Debug.Log(eventCode.ToString() + "   " + group.ToString());

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = (Photon.Realtime.ReceiverGroup)receiver,
                InterestGroup = group,
                CachingOption = (EventCaching)caching
            };
            PhotonNetwork.RaiseEvent(eventCode, message, raiseEventOptions, SendOptions.SendReliable);
        }

        public void SendOperation(byte operationCode, Dictionary<byte, object> operationParameters)
        {
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOperation(operationCode, operationParameters, SendOptions.SendReliable);
        }

        public string ConnectState()
        {
            return PhotonNetwork.NetworkClientState.ToString();
        }

        public void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
            }
        }

        public void Disconnect()
        {

            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom(false);

            PhotonNetwork.Disconnect();
        }

        #region 其他

        public void SetInterestGroups(byte group, bool enable)
        {
            PhotonNetwork.SetInterestGroups(group, enable);
        }

        public void SetInterestGroups(byte[] disableGroups, byte[] enableGroups)
        {
            PhotonNetwork.SetInterestGroups(disableGroups, disableGroups);
        }

        public void RemoveRPCsInGroup(byte group)
        {
            PhotonNetwork.RemoveRPCsInGroup(group);
        }

        public void FindFriends(string[] FriendUsers)
        {
            PhotonNetwork.NetworkingClient.OpFindFriends(FriendUsers);
        }

        public override void OnConnected()
        {
            Debug.Log("连接成功");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("验证成功");
            onConnectedEvent?.Invoke();

            PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("断开连接" + cause.ToString());
            onDisconnected?.Invoke((INetDisconnectCause)cause);
        }

        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            Debug.Log("出现错误:" + errorInfo.Info);
        }

        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            Debug.Log("验证失败的相应" + debugMessage);
        }

        public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            Debug.Log("自定义验证的相应");
        }

        public override void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            Debug.Log("朋友列表更新时");
            List<INetFriendInfo> infos = new List<INetFriendInfo>();

            foreach (var item in friendList)
            {
                INetFriendInfo info = new INetFriendInfo();

                info.IsInRoom = item.IsInRoom;
                info.IsOnline = item.IsOnline;
                info.Room = item.Room;
                info.UserId = item.UserId;
                infos.Add(info);
            }

            onFriendListUpdate?.Invoke(infos);
        }
        #endregion

        #region 大厅
        public override void OnJoinedLobby()
        {
            onJoinLobbyEvent?.Invoke();
            Debug.Log("加入大厅");
        }

        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            List<INetLobbyInfo> lobbyInfos = new List<INetLobbyInfo>();
            foreach (var item in lobbyStatistics)
            {
                Debug.Log("大厅数据更新" + item.Name);

                INetLobbyInfo info = new INetLobbyInfo();
                info.PlayerCount = item.PlayerCount;
                info.RoomCount = item.RoomCount;
                lobbyInfos.Add(info);
            }

            onLobbysDataUpdate?.Invoke(lobbyInfos);
        }

        public override void OnLeftLobby()
        {
            Debug.Log("离开大厅");
        }
        #endregion

        #region 房间

        public void CreateRoom(string roomNme, byte maxPlay)
        {
            PhotonNetwork.CreateRoom(roomNme, new RoomOptions
            {
                MaxPlayers = maxPlay,
                PlayerTtl = 5,
                EmptyRoomTtl = 5
            });
        }

        public void CreateRoom(string roomNme, byte maxPlay, string[] pluginNames)
        {
            PhotonNetwork.CreateRoom(roomNme, new RoomOptions
            {
                MaxPlayers = maxPlay,
                PlayerTtl = 5,
                EmptyRoomTtl = 5,
                Plugins = pluginNames
            });
        }

        public void JoinOrCreateRoom(string roomName, byte maxPlay, string[] expectedUsers = null)
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions
            {
                IsVisible = false,
                PublishUserId = true,
                MaxPlayers = maxPlay,
            }, TypedLobby.Default, expectedUsers);
        }

        public void JoinOrCreateRoom(string roomName, byte maxPlay, string[] expectedUsers = null, string[] pluginNames = null)
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions
            {
                IsVisible = false,
                PublishUserId = true,
                MaxPlayers = maxPlay,
                Plugins = pluginNames
            }, TypedLobby.Default, expectedUsers);
        }

        /// <summary>
        /// IsVisible 不可见，不能随机进入，只能通过房间名称进入
        /// </summary>
        /// <param name="roomNme"></param>
        /// <param name="maxPlay"></param>
        public void JoinOrCreateRoom(string roomName, byte maxPlay)
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions
            {
                MaxPlayers = maxPlay,
                PlayerTtl = 5,
                EmptyRoomTtl = 5
            }, TypedLobby.Default);
        }

        public void JoinOrChangeRoom(string roomName, byte maximumPlayers = 100)
        {

            if (PhotonNetwork.InRoom)
            {
                if (roomName.Equals(PhotonNetwork.CurrentRoom.Name)) return;

                PhotonNetwork.LeaveRoom(false);
                Debug.Log("离开当前空间");
            }
            else
            {
                Debug.Log("创建新的空间");

                this.JoinOrCreateRoom(roomName, maximumPlayers);
            }
        }

        public void JoinRoom()
        {
            if (PhotonNetwork.NetworkingClient.InRoom == false)
                PhotonNetwork.JoinRandomRoom();
        }

        public void JoinRoom(string rooName)
        {
            if (PhotonNetwork.NetworkingClient.InRoom == false)
                PhotonNetwork.JoinRoom(rooName);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void GetCurrentRoomList()
        {
            PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, null);
        }

        public override void OnJoinedRoom()
        {
            RoomOptions options = new RoomOptions();
            options.IsVisible = PhotonNetwork.CurrentRoom.IsVisible;
            options.IsOpen = PhotonNetwork.CurrentRoom.IsOpen;
            options.MaxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
            options.CustomRoomPropertiesForLobby = PhotonNetwork.CurrentRoom.PropertiesListedInLobby;

            INetRoom netRoom = new INetRoom(PhotonNetwork.CurrentRoom.Name, options);

            OnJoinedRoomEvent?.Invoke(netRoom);
            Debug.Log("进入房间");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(returnCode + " 随机进入失败：" + message);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("当创建房间");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log(returnCode + " 创建房间失败：" + message);
        }

        public override void OnLeftRoom()
        {
            OnLeaveRoomEvent?.Invoke();
            Debug.Log("离开房间");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            List<INetRoomInfo> infos = new List<INetRoomInfo>();
            foreach (var item in roomList)
            {
                INetRoomInfo info = new INetRoomInfo();

                info.RemovedFromList = item.RemovedFromList;
                info.customProperties = item.CustomProperties;
                info.maxPlayers = item.MaxPlayers;
                info.isOpen = item.IsOpen;
                info.isVisible = item.IsVisible;
                info.name = item.Name;
                info.masterClientId = item.masterClientId;
                info.playerCount = item.PlayerCount;
                infos.Add(info);
                Debug.Log(item.Name + "-----------item");
            }
            onRoomListUpdate?.Invoke(infos);
            Debug.Log("当房列表发生变化");
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {

            Dictionary<string, object> pairs = new Dictionary<string, object>();
            foreach (var item in propertiesThatChanged)
            {
                Debug.Log("当房间属性发生变化" + item.Key);
                pairs.Add(item.Key.ToString(), item.Value);
            }
            Debug.Log("当房间属性发生变化");

            OnRoomPropertiesChangeEvent?.Invoke(propertiesThatChanged);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("进入房间失败 " + message);
            OnJoinRoomFailedEvent?.Invoke();
        }
        #endregion

        #region 玩家

        public INetPlayer[] GetPlayers()
        {
            int playerList = PhotonNetwork.PlayerList.Length;
            INetPlayer[] players = new INetPlayer[playerList];
            for (int i = 0; i < playerList; i++)
            {
                players[i] = PlayerToINetPlayer(PhotonNetwork.PlayerList[i]);
            }

            return players;
        }

        public INetPlayer GetLocalPlayer
        {
            get
            {
                return PlayerToINetPlayer(PhotonNetwork.LocalPlayer);
            }
        }

        public void SetPlayName(string Name)
        {
            PhotonNetwork.NickName = Name;
        }

        public void SetPlayerProperties(Dictionary<string, object> propertiesToSet)
        {
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();

            foreach (var item in propertiesToSet)
            {
                hashtable.Add(item.Key, item.Value);
            }
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }


        public void SetRoomProperties(Dictionary<string, object> propertiesToSet)
        {

            if (PhotonNetwork.IsConnected == false) return;
            if (PhotonNetwork.InRoom == false) return;

            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();

            foreach (var item in propertiesToSet)
            {
                hashtable.Add(item.Key, item.Value);
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        }
        public void SetMasterClient()
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }

        private INetPlayer PlayerToINetPlayer(Player newPlayer)
        {
            return new INetPlayer(newPlayer, newPlayer.NickName, newPlayer.ActorNumber, newPlayer.IsLocal, newPlayer.CustomProperties);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {

            OnPlayerEnteredRoomEvent?.Invoke(PlayerToINetPlayer(newPlayer));

            foreach (var item in newPlayer.CustomProperties)
            {
                Debug.Log("当其他玩家进入房间" + item);
            }

            Debug.Log(newPlayer.UserId + "当其他玩家进入房间" + newPlayer.TagObject);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {

            OnPlayerLeftRoomEvent?.Invoke(PlayerToINetPlayer(otherPlayer));

            foreach (var item in otherPlayer.CustomProperties)
            {
                Debug.Log(otherPlayer.NickName + "当其他玩家离开房间" + item.Value + item.Key);
            }

            Debug.Log(otherPlayer.NickName + "当其他玩家离开房间");
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {

            OnPlayerPropertiesChangeEvent?.Invoke(PlayerToINetPlayer(targetPlayer), changedProps);

            Debug.Log("当其他玩家属性发生变化");

            foreach (var item in targetPlayer.CustomProperties)
            {
                Debug.Log("目标玩家的属性" + item.Value + item.Key);
            }

            foreach (var item in changedProps)
            {
                Debug.Log("当前设置的参数" + item.Value + item.Key);
            }

        }

        public override void OnRegionListReceived(RegionHandler regionHandler)
        {
            Debug.Log("当收到地区列表");
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            onMasterClientSwitched?.Invoke(PlayerToINetPlayer(newMasterClient));
            Debug.Log("切换主客户端");
        }
        #endregion

        #region 游戏对象
        public GameObject InstantiateRoomObject(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            return PhotonNetwork.InstantiateRoomObject(prefabName, position, rotation, group, data);
        }

        public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            return PhotonNetwork.Instantiate(prefabName, position, rotation, group, data);
        }

        public GameObject CreateCharacter(GameObject prefab, Vector3 position, Quaternion rotation, object[] data = null)
        {
            PhotonView photonView = prefab.GetComponent<PhotonView>();
            if (PhotonNetwork.AllocateViewID(photonView))
            {
                object[] eventContent = new object[]
                {
                prefab.name,
                prefab.transform.position,
                prefab.transform.rotation,
                photonView.ViewID,
                data,
                PhotonNetwork.NetworkingClient.UserId
                };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };

                PhotonNetwork.RaiseEvent(0, eventContent, raiseEventOptions, sendOptions);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");

                Destroy(prefab);
            }

            return prefab;
        }

        public void Destroy(PhotonView targetView)
        {
            PhotonNetwork.Destroy(targetView);
        }

        public void Destroy(GameObject targetGo)
        {
            PhotonNetwork.Destroy(targetGo);
        }

        public void DestroyPlayerObjects(Player targetPlayer)
        {
            PhotonNetwork.DestroyPlayerObjects(targetPlayer);
        }

        public void DestroyAll()
        {
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.DestroyAll();

            PhotonNetwork.CurrentRoom.ClearExpectedUsers();

        }



        #endregion

    }
}