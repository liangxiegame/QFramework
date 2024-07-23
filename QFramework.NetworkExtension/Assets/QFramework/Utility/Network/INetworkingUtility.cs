using System;
using System.Collections.Generic;
using QFramework;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace QFramework.NetworkExtension
{
	public enum NetReceiverGroupCode : byte
	{
		/// <summary>Default value (not sent). Anyone else gets my event.</summary>
		Others = 0,

		/// <summary>Everyone in the current room (including this peer) will get this event.</summary>
		All = 1,

		/// <summary>The server sends this event only to the actor with the lowest actorNumber.</summary>
		/// <remarks>The "master client" does not have special rights but is the one who is in this room the longest time.</remarks>
		MasterClient = 2,
	}

	public enum IEventCaching : byte
	{
		/// <summary>默认不缓存</summary>
		DoNotCache = 0,

		/// <summary>Adds an event to the room's cache</summary>
		AddToRoomCache = 4,

		/// <summary>Adds this event to the cache for actor 0 (becoming a "globally owned" event in the cache).</summary>
		AddToRoomCacheGlobal = 5,

		/// <summary>Remove fitting event from the room's cache.</summary>
		RemoveFromRoomCache = 6,
	}

	/// <summary>
	/// 扩展
	/// </summary>
	public interface INetworkingSpaceUtility
	{
		/// <summary>
		/// 变更房间，从一个房间离开进入另外一个房间
		/// </summary>
		void JoinOrChangeRoom(string roomName, byte maximumPlayers = 100);
	}

	public interface INetworkingConnectionUtility
	{
		/// <summary>
		/// 用于初始化 INetworkingUtility
		/// </summary>
		void Init(string userID = null, string nickname = null, bool EnableLobbyStatistics = false);

		/// <summary>
		/// 连接到大厅
		/// </summary>
		void Connect();


		/// <summary>
		/// 是否连接并且准备好了
		/// </summary>
		bool IsConnectedAndReady { get; }

		bool IsMessageQueueRunning { get; set; }

		/// <summary>
		/// 连接到大厅的事件
		/// </summary>
		event Action onConnectedEvent;

		/// <summary>
		/// 当进入大厅
		/// </summary>
		event Action onJoinLobbyEvent;

		/// <summary>
		/// 断开连接
		/// </summary>
		void Disconnect();

		/// <summary>
		/// 获取链接状态
		/// </summary>
		/// <returns></returns>
		string ConnectState();
	}

	public interface INetworkingInstantiateUtility
	{
		/// <summary>
		/// 用来网络房间生成物体，生成的物体属于房间，不会随着玩家 销毁而销毁（中立）
		/// </summary>
		GameObject InstantiateRoomObject(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null);

		/// <summary>
		/// 用来网络玩家生成物体，生成的物体属于玩家，玩家离开物体相应消失
		/// </summary>
		GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null);

		/// <summary>
		/// 创建网络角色
		/// </summary>
		/// <param name="prefab">必须添加 PhotonView组件</param>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <returns></returns>
		GameObject CreateCharacter(GameObject prefab, Vector3 position, Quaternion rotation, object[] data = null);

		/// <summary>
		/// 删除网络对象
		/// </summary>
		void Destroy(PhotonView targetView);

		/// <summary>
		/// 删除网络对象,除非它是静态的或不在这个客户端控制下。
		/// </summary>
		void Destroy(GameObject targetGo);

		/// <summary>
		/// 删除玩家
		/// </summary>
		void DestroyPlayerObjects(Player targetPlayer);

		void DestroyAll();
	}

	public interface INetworkingMessageUtility
	{
		/// <summary>
		/// 发送网络自定义消息，主要用来面向玩家进行操作
		/// </summary>
		void Send(byte eventCode, byte group, object message, IEventCaching caching = IEventCaching.DoNotCache);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventCode"></param>
		/// <param name="group"></param>
		/// <param name="receiver"> 0 除发送者外的人，1所有人，2发给房主 </param>
		/// <param name="message"></param>
		/// <param name="caching"></param>
		void Send(byte eventCode, byte group, NetReceiverGroupCode receiver, object message, IEventCaching caching = IEventCaching.DoNotCache);

		/// <summary>
		/// 发送操作码用来和服务器通讯，主要用于创建房间前的一些特殊操作需要与服务器协商设置
		/// </summary>
		void SendOperation(byte operationCode, Dictionary<byte, object> operationParameters);

		/// <summary>
		/// 网络消息的响应
		/// </summary>
		event Action<INetEventData> EventReceived;

		/// <summary>
		/// 操作码的相应
		/// </summary>
		event Action<INetworkResponse> OpResponseReceived;

	}

	public interface INetworkingInRoomUtility
	{
		/// <summary>
		/// 设置玩家名称
		/// </summary>
		/// <param name="Name"></param>
		void SetPlayName(string Name);

		/// <summary>
		/// 获取本地玩家
		/// </summary>
		INetPlayer GetLocalPlayer { get; }

		/// <summary>
		/// 获取Room在线玩家列表
		/// </summary>
		INetPlayer[] GetPlayers();

		/// <summary>
		/// 强制设置这个客户端为主客户端
		/// </summary>
		void SetMasterClient();

		/// <summary>
		/// 加入或退出一个兴趣组
		/// </summary>
		/// <param name="group"></param>
		/// <param name="enable"></param>
		void SetInterestGroups(byte group, bool enable);

		/// <summary>
		/// 禁用兴趣组/禁用兴趣组
		/// </summary>
		/// <param name="disableGroups">可以为空,不为空时为禁用这个兴趣组 </param>
		/// <param name="enableGroups">可以为空,不为空时为进入这个兴趣组</param>
		void SetInterestGroups(byte[] disableGroups, byte[] enableGroups);

		/// <summary>
		/// 主客户端调用有效,可以用来删除任何RPC组
		/// </summary>
		/// <param name="group"></param>
		void RemoveRPCsInGroup(byte group);

		/// <summary>
		/// 用来设置玩家属性信息
		/// </summary>
		void SetPlayerProperties(Dictionary<string, object> propertiesToSet);

		/// <summary>
		/// 当房间列表更新时
		/// </summary>
		event Action<List<INetRoomInfo>> onRoomListUpdate;

		/// <summary>
		/// 获取当前房间列表
		/// </summary>
		void GetCurrentRoomList();


		/// <summary>
		/// 当玩家属性改变时
		/// </summary>
		event Action<INetPlayer, Dictionary<object, object>> OnPlayerPropertiesChangeEvent;

		/// <summary>
		/// 当玩家进入
		/// </summary>
		event Action<INetPlayer> OnPlayerEnteredRoomEvent;

		/// <summary>
		/// 当玩家离开房间时
		/// </summary>
		event Action<INetPlayer> OnPlayerLeftRoomEvent;
	}

	public interface INetworkingLobbyUtility
	{

	}

	public interface INetworkingMatchmakingUtility
	{
		/// <summary>
		/// 创建房间 房间名称  最大玩家数量
		/// </summary>
		/// <param name="roomNme"></param>
		/// <param name="maxPlay"></param>
		void CreateRoom(string roomNme, byte maxPlay);

		/// <summary>
		/// 如果有房间创建房间，如果没有房间进入房间
		/// </summary>
		/// <param name="roomNme"></param>
		/// <param name="maxPlay"></param>
		void JoinOrCreateRoom(string roomNme, byte maxPlay);

		/// <summary>
		/// 进入房间
		/// </summary>
		void JoinRoom();

		/// <summary>
		/// 进入房间
		/// </summary>
		/// <param name="roomName"></param>
		void JoinRoom(string roomName);

		/// <summary>
		/// 离开房间
		/// </summary>
		void LeaveRoom();

		/// <summary>
		/// 设置房间属性
		/// </summary>
		/// <param name="propertiesToSet"></param>
		void SetRoomProperties(Dictionary<string, object> propertiesToSet);

		/// <summary>
		/// 找好友
		/// </summary>
		/// <param name="FriendUsers"></param>
		void FindFriends(string[] FriendUsers);

		/// <summary>
		/// 当前房间数
		/// </summary>
		int CountOfRooms { get; }

		/// <summary>
		/// 未加入房间的玩家数量
		/// </summary>
		int CountOfPlayersOnMaster { get; }

		/// <summary>
		/// 房间内的玩家人数
		/// </summary>
		int CountOfPlayersInRooms { get; }

		/// <summary>
		/// 已连接玩家总数
		/// </summary>
		int CountOfPlayers { get; }

		/// <summary>
		/// 当前room 的人数，如果没有进入Room那么人数为0
		/// </summary>
		int CurrentRoomPlayers { get; }

		/// <summary>
		/// 当进入房间
		/// </summary>
		/// <returns></returns>
		event Action<INetRoom> OnJoinedRoomEvent;

		/// <summary>
		/// 当离开房间
		/// </summary>
		/// <returns></returns>
		event Action OnLeaveRoomEvent;

		/// <summary>
		/// 当进入房间失败
		/// </summary>
		event Action OnJoinRoomFailedEvent;

		/// <summary>
		/// 当房间属性变化
		/// </summary>
		event Action<Dictionary<object, object>> OnRoomPropertiesChangeEvent;

		/// <summary>
		/// 当朋友列表发生改变
		/// </summary>
		event Action<List<INetFriendInfo>> onFriendListUpdate;
	}

	public interface INetworkingUtility : INetworkingLobbyUtility, INetworkingConnectionUtility, INetworkingMessageUtility, INetworkingInstantiateUtility, INetworkingInRoomUtility, INetworkingMatchmakingUtility, INetworkingSpaceUtility, IUtility
	{

	}
}