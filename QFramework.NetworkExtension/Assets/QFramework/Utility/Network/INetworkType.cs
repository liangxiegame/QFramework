using ExitGames.Client.Photon;
using Photon.Realtime;

namespace QFramework.NetworkExtension
{


    public class INetworkOperation : OperationResponse
    {
    }

    public class INetworkResponse : OperationResponse
    {
        public INetworkResponse(OperationResponse peo)
        {
            SynchronizationProperties(peo, this);
        }

        void SynchronizationProperties(OperationResponse src, INetworkResponse des)
        {
            des.ReturnCode = src.ReturnCode;
            des.Parameters = src.Parameters;
            des.DebugMessage = src.DebugMessage;
        }
    }

    public class INetEventData : EventData
    {
        public object customData;
        public ParameterDictionary param;
        public INetEventData(EventData peo)
        {
            SynchronizationProperties(peo, this);
        }

        void SynchronizationProperties(EventData src, INetEventData des)
        {
            des.Code = src.Code;
            des.param = src.Parameters;
            des.customData = src.CustomData;
        }
    }

    public class INetPlayer : Player
    {
        public string userId;
        public bool hasRejoined;
        public bool isMasterClient;
        public INetPlayer(Player player, string nickName, int actorNumber, bool isLocal, Hashtable playerProperties) : base
        (nickName, actorNumber, isLocal, playerProperties)
        {
            this.TagObject = player.TagObject;
            this.userId = player.UserId;
            this.hasRejoined = player.HasRejoined;
            this.isMasterClient = player.IsMasterClient;
        }

    }

    public class INetRoom : Room
    {
        public INetRoom(string roomName, RoomOptions options, bool isOffline = false)
            : base(roomName, options)
        {

        }
    }

    public class INetRoomInfo
    {
        /// <summary>用于大堂，用来标记不再被列出的房间(表示已满、关闭或隐藏).</summary>
        public bool RemovedFromList;

        public Hashtable customProperties = new Hashtable();

        public byte maxPlayers = 0;

        public int playerCount;

        public string[] expectedUsers;

        public bool isOpen = true;

        public bool isVisible = true;

        public string name;

        public int masterClientId;

        public string[] propertiesListedInLobby;

    }

    public class INetLobbyInfo
    {
        /// <summary>当前加入此大厅的玩家数量.</summary>
        public int PlayerCount;

        /// <summary>当前大厅数量.</summary>
        public int RoomCount;

        public override string ToString()
        {
            return string.Format("TypedLobbyInfo] rooms: {2} players: {3}", this.RoomCount, this.PlayerCount);
        }
    }

    public enum INetDisconnectCause
    {
        /// <summary>No error was tracked.</summary>
        None,

        /// <summary>因为地址错误或者服务器未开启.</summary>
        ExceptionOnConnect,

        /// <summary>Dns解析主机名失败。此异常被捕获并以错误级别记录.</summary>
        DnsExceptionOnConnect,

        /// <summary>服务器地址格式错误.</summary>
        ServerAddressInvalid,

        /// <summary>一些错误导致的服务器连接失败</summary>
        Exception,

        /// <summary>服务器连接超时断开.</summary>
        ServerTimeout,

        /// <summary>客户端连接超时断开.</summary>
        ClientTimeout,

        /// <summary>服务断开这个客户端的连接.</summary>
        DisconnectByServerLogic,

        /// <summary>服务器因未知原因断开此客户端.</summary>
        DisconnectByServerReasonUnknown,

        /// <summary>在Photon Cloud中验证无效的AppId.</summary>
        InvalidAuthentication,

        /// <summary>认证在Photon云与无效的客户端值或自定义认证设置在云仪表板.</summary>
        CustomAuthenticationFailed,

        /// <summary>机票过期了.</summary>
        AuthenticationTicketExpired,

        /// <summary>CCU超过.</summary>
        MaxCcuReached,

        /// <summary>区域错误.</summary>
        InvalidRegion,

        /// <summary>当前这个客户端(通常没有授权)不可用的操作.</summary>
        OperationNotAllowedInCurrentState,

        /// <summary>客户端与逻辑(c#代码)断开连接.</summary>
        DisconnectByClientLogic,


        /// <remarks>操作频繁断开连接.</remarks>
        DisconnectByOperationLimit,

        /// <summary>客户端收到来自服务器的“断开连接消息”.</summary>
        DisconnectByDisconnectMessage
    }

    public class INetFriendInfo
    {
        public string UserId;

        public bool IsOnline;

        public string Room;

        public bool IsInRoom;
    }
}