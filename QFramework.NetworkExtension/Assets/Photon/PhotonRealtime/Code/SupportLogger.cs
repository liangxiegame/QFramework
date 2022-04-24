// ----------------------------------------------------------------------------
// <copyright file="SupportLogger.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Implements callbacks of the Realtime API to logs selected information
//   for support cases.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------



#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using Stopwatch = System.Diagnostics.Stopwatch;

    using ExitGames.Client.Photon;

    #if SUPPORTED_UNITY
    using UnityEngine;
    #endif

    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif

    /// <summary>
    /// Helper class to debug log basic information about Photon client and vital traffic statistics.
    /// </summary>
    /// <remarks>
    /// Set SupportLogger.Client for this to work.
    /// </remarks>
    #if SUPPORTED_UNITY
    [DisallowMultipleComponent]
    #if PUN_2_OR_NEWER || FUSION_UNITY
	[AddComponentMenu("")] // hide from Unity Menus and searches
    #endif
	public class SupportLogger : MonoBehaviour, IConnectionCallbacks , IMatchmakingCallbacks , IInRoomCallbacks, ILobbyCallbacks, IErrorInfoCallback
    #else
	public class SupportLogger : IConnectionCallbacks, IInRoomCallbacks, IMatchmakingCallbacks , ILobbyCallbacks
    #endif
    {
        /// <summary>
        /// Toggle to enable or disable traffic statistics logging.
        /// </summary>
        public bool LogTrafficStats = true;
        private bool loggedStillOfflineMessage;

        private LoadBalancingClient client;

        private Stopwatch startStopwatch;

        /// helps skip the initial OnApplicationPause call, which is not really of interest on start
        private bool initialOnApplicationPauseSkipped = false;

        private int pingMax;
        private int pingMin;

        /// <summary>
        /// Photon client to log information and statistics from.
        /// </summary>
        public LoadBalancingClient Client
        {
            get { return this.client; }
            set
            {
                if (this.client != value)
                {
                    if (this.client != null)
                    {
                        this.client.RemoveCallbackTarget(this);
                    }
                    this.client = value;
                    if (this.client != null)
                    {
                        this.client.AddCallbackTarget(this);
                    }
                }
            }
        }


        #if SUPPORTED_UNITY
        protected void Start()
        {
            this.LogBasics();

            if (this.startStopwatch == null)
            {
                this.startStopwatch = new Stopwatch();
                this.startStopwatch.Start();
            }
        }

        protected void OnDestroy()
        {
            this.Client = null; // will remove this SupportLogger as callback target
        }

        protected void OnApplicationPause(bool pause)
        {
            if (!this.initialOnApplicationPauseSkipped)
            {
                this.initialOnApplicationPauseSkipped = true;
                return;
            }

            Debug.Log(string.Format("{0} SupportLogger OnApplicationPause({1}). Client: {2}.", this.GetFormattedTimestamp(), pause, this.client == null ? "null" : this.client.State.ToString()));
        }

        protected void OnApplicationQuit()
        {
            this.CancelInvoke();
        }
        #endif

        public void StartLogStats()
        {
            #if SUPPORTED_UNITY
            this.InvokeRepeating("LogStats", 10, 10);
            #else
            Debug.Log("Not implemented for non-Unity projects.");
            #endif
        }

        public void StopLogStats()
        {
            #if SUPPORTED_UNITY
            this.CancelInvoke("LogStats");
            #else
            Debug.Log("Not implemented for non-Unity projects.");
            #endif
        }

        private void StartTrackValues()
        {
            #if SUPPORTED_UNITY
            this.InvokeRepeating("TrackValues", 0.5f, 0.5f);
            #else
            Debug.Log("Not implemented for non-Unity projects.");
            #endif
        }

        private void StopTrackValues()
        {
            #if SUPPORTED_UNITY
            this.CancelInvoke("TrackValues");
            #else
            Debug.Log("Not implemented for non-Unity projects.");
            #endif
        }

        private string GetFormattedTimestamp()
        {
            if (this.startStopwatch == null)
            {
                this.startStopwatch = new Stopwatch();
                this.startStopwatch.Start();
            }

            TimeSpan span = this.startStopwatch.Elapsed;
            if (span.Minutes > 0)
            {
                return string.Format("[{0}:{1}.{1}]", span.Minutes, span.Seconds, span.Milliseconds);
            }

            return string.Format("[{0}.{1}]", span.Seconds, span.Milliseconds);
        }


        // called via InvokeRepeatedly
        private void TrackValues()
        {
            if (this.client != null)
            {
                int currentRtt = this.client.LoadBalancingPeer.RoundTripTime;
                if (currentRtt > this.pingMax)
                {
                    this.pingMax = currentRtt;
                }
                if (currentRtt < this.pingMin)
                {
                    this.pingMin = currentRtt;
                }
            }
        }


        /// <summary>
        /// Debug logs vital traffic statistics about the attached Photon Client.
        /// </summary>
        public void LogStats()
        {
            if (this.client == null || this.client.State == ClientState.PeerCreated)
            {
                return;
            }

            if (this.LogTrafficStats)
            {
                Debug.Log(string.Format("{0} SupportLogger {1} Ping min/max: {2}/{3}", this.GetFormattedTimestamp() , this.client.LoadBalancingPeer.VitalStatsToString(false) , this.pingMin , this.pingMax));
            }
        }

        /// <summary>
        /// Debug logs basic information (AppId, AppVersion, PeerID, Server address, Region) about the attached Photon Client.
        /// </summary>
        private void LogBasics()
        {
            if (this.client != null)
            {
                List<string> buildProperties = new List<string>(10);
                #if SUPPORTED_UNITY
                buildProperties.Add(Application.unityVersion);
                buildProperties.Add(Application.platform.ToString());
                #endif
                #if ENABLE_IL2CPP
                buildProperties.Add("ENABLE_IL2CPP");
                #endif
                #if ENABLE_MONO
                buildProperties.Add("ENABLE_MONO");
                #endif
                #if DEBUG
                buildProperties.Add("DEBUG");
                #endif
                #if MASTER
                buildProperties.Add("MASTER");
                #endif
                #if NET_4_6
                buildProperties.Add("NET_4_6");
                #endif
                #if NET_STANDARD_2_0
                buildProperties.Add("NET_STANDARD_2_0");
                #endif
                #if NETFX_CORE
                buildProperties.Add("NETFX_CORE");
                #endif
                #if NET_LEGACY
                buildProperties.Add("NET_LEGACY");
                #endif
                #if UNITY_64
                buildProperties.Add("UNITY_64");
                #endif
                #if UNITY_FUSION
                buildProperties.Add("UNITY_FUSION");
                #endif


                StringBuilder sb = new StringBuilder();

                string appIdShort = string.IsNullOrEmpty(this.client.AppId) || this.client.AppId.Length < 8 ? this.client.AppId : string.Concat(this.client.AppId.Substring(0, 8), "***");

                sb.AppendFormat("{0} SupportLogger Info: ", this.GetFormattedTimestamp());
                sb.AppendFormat("AppID: \"{0}\" AppVersion: \"{1}\" Client: v{2} ({4}) Build: {3} ", appIdShort, this.client.AppVersion, PhotonPeer.Version, string.Join(", ", buildProperties.ToArray()), this.client.LoadBalancingPeer.TargetFramework);
                if (this.client != null && this.client.LoadBalancingPeer != null && this.client.LoadBalancingPeer.SocketImplementation != null)
                {
                    sb.AppendFormat("Socket: {0} ", this.client.LoadBalancingPeer.SocketImplementation.Name);
                }

                sb.AppendFormat("UserId: \"{0}\" AuthType: {1} AuthMode: {2} {3} ", this.client.UserId, (this.client.AuthValues != null) ? this.client.AuthValues.AuthType.ToString() : "N/A", this.client.AuthMode, this.client.EncryptionMode);

                sb.AppendFormat("State: {0} ", this.client.State);
                sb.AppendFormat("PeerID: {0} ", this.client.LoadBalancingPeer.PeerID);
                sb.AppendFormat("NameServer: {0} Current Server: {1} IP: {2} Region: {3} ", this.client.NameServerHost, this.client.CurrentServerAddress, this.client.LoadBalancingPeer.ServerIpAddress, this.client.CloudRegion);

                Debug.LogWarning(sb.ToString());
            }
        }


        public void OnConnected()
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnConnected().");
            this.pingMax = 0;
            this.pingMin = this.client.LoadBalancingPeer.RoundTripTime;
            this.LogBasics();

            if (this.LogTrafficStats)
            {
                this.client.LoadBalancingPeer.TrafficStatsEnabled = false;
                this.client.LoadBalancingPeer.TrafficStatsEnabled = true;
                this.StartLogStats();
            }

            this.StartTrackValues();
        }

        public void OnConnectedToMaster()
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnConnectedToMaster().");
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnFriendListUpdate(friendList).");
        }

        public void OnJoinedLobby()
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnJoinedLobby(" + this.client.CurrentLobby + ").");
        }

        public void OnLeftLobby()
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnLeftLobby().");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnCreateRoomFailed(" + returnCode+","+message+").");
        }

        public void OnJoinedRoom()
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnJoinedRoom(" + this.client.CurrentRoom + "). " + this.client.CurrentLobby + " GameServer:" + this.client.GameServerAddress);
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnJoinRoomFailed(" + returnCode+","+message+").");
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnJoinRandomFailed(" + returnCode+","+message+").");
        }

        public void OnCreatedRoom()
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnCreatedRoom(" + this.client.CurrentRoom + "). " + this.client.CurrentLobby + " GameServer:" + this.client.GameServerAddress);
        }

        public void OnLeftRoom()
        {
            Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnLeftRoom().");
        }

		public void OnDisconnected(DisconnectCause cause)
        {
            this.StopLogStats();
            this.StopTrackValues();

			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnDisconnected(" + cause + ").");
			this.LogBasics();
            this.LogStats();
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnRegionListReceived(regionHandler).");
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnRoomListUpdate(roomList). roomList.Count: " + roomList.Count);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerEnteredRoom(" + newPlayer+").");
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerLeftRoom(" + otherPlayer+").");
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnRoomPropertiesUpdate(propertiesThatChanged).");
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerPropertiesUpdate(targetPlayer,changedProps).");
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnMasterClientSwitched(" + newMasterClient+").");
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnCustomAuthenticationResponse(" + data.ToStringFull()+").");
        }

		public void OnCustomAuthenticationFailed (string debugMessage)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnCustomAuthenticationFailed(" + debugMessage+").");
		}

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnLobbyStatisticsUpdate(lobbyStatistics).");
        }


        #if !SUPPORTED_UNITY
        private static class Debug
        {
            public static void Log(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
            public static void LogWarning(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
            public static void LogError(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
        }
        #endif

        public void OnErrorInfo(ErrorInfo errorInfo)
        {
            Debug.LogError(errorInfo.ToString());
        }
    }
}