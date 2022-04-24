// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonTeamsManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
// Implements teams in a room/game with help of player properties.
// </summary>
// <remarks>
// Teams are defined by name and code. Change this to get more / different teams.
// There are no rules when / if you can join a team. You could add this in JoinTeam or something.
// </remarks>                                                                                           
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.UtilityScripts
{
    [Serializable]
    public class PhotonTeam
    {
        public string Name;
        public byte Code;

        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.Name, this.Code);
        }
    }

    /// <summary>
    /// Implements teams in a room/game with help of player properties. Access them by Player.GetTeam extension.
    /// </summary>
    /// <remarks>
    /// Teams are defined by enum Team. Change this to get more / different teams.
    /// There are no rules when / if you can join a team. You could add this in JoinTeam or something.
    /// </remarks>
    [DisallowMultipleComponent]
    public class PhotonTeamsManager : MonoBehaviour, IMatchmakingCallbacks, IInRoomCallbacks
    {
        #if UNITY_EDITOR
        #pragma warning disable 0414
        [SerializeField]
        private bool listFoldIsOpen = true;
        #pragma warning restore 0414
        #endif

        [SerializeField]
        private List<PhotonTeam> teamsList = new List<PhotonTeam>
        {
            new PhotonTeam { Name = "Blue", Code = 1 },
            new PhotonTeam { Name = "Red", Code = 2 }
        };

        private Dictionary<byte, PhotonTeam> teamsByCode;
        private Dictionary<string, PhotonTeam> teamsByName;
        
        /// <summary>The main list of teams with their player-lists. Automatically kept up to date.</summary>
        private Dictionary<byte, HashSet<Player>> playersPerTeam;

        /// <summary>Defines the player custom property name to use for team affinity of "this" player.</summary>
        public const string TeamPlayerProp = "_pt";

        public static event Action<Player, PhotonTeam> PlayerJoinedTeam;
        public static event Action<Player, PhotonTeam> PlayerLeftTeam;

        private static PhotonTeamsManager instance;
        public static PhotonTeamsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PhotonTeamsManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = "PhotonTeamsManager";
                        instance = obj.AddComponent<PhotonTeamsManager>();
                    }
                    instance.Init();
                }

                return instance;
            }
        }

        #region MonoBehaviour

        private void Awake()
        {
            if (instance == null || ReferenceEquals(this, instance))
            {
                this.Init();
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            this.ClearTeams();
        }

        private void Init()
        {
            teamsByCode = new Dictionary<byte, PhotonTeam>(teamsList.Count);
            teamsByName = new Dictionary<string, PhotonTeam>(teamsList.Count);
            playersPerTeam = new Dictionary<byte, HashSet<Player>>(teamsList.Count);
            for (int i = 0; i < teamsList.Count; i++)
            {
                teamsByCode[teamsList[i].Code] = teamsList[i];
                teamsByName[teamsList[i].Name] = teamsList[i];
                playersPerTeam[teamsList[i].Code] = new HashSet<Player>();
            }
        }

        #endregion

        #region IMatchmakingCallbacks

        void IMatchmakingCallbacks.OnJoinedRoom()
        {
            this.UpdateTeams();
        }

        void IMatchmakingCallbacks.OnLeftRoom()
        {
            this.ClearTeams();
        }

        void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            object temp;
            if (changedProps.TryGetValue(TeamPlayerProp, out temp))
            {
                if (temp == null)
                {
                    foreach (byte code in playersPerTeam.Keys)
                    {
                        if (playersPerTeam[code].Remove(targetPlayer))
                        {
                            if (PlayerLeftTeam != null)
                            {
                                PlayerLeftTeam(targetPlayer, teamsByCode[code]);
                            }
                            break;
                        }
                    }
                } 
                else if (temp is byte)
                {
                    byte teamCode = (byte) temp;
                    // check if player switched teams, remove from previous team 
                    foreach (byte code in playersPerTeam.Keys)
                    {
                        if (code == teamCode)
                        {
                            continue;
                        }
                        if (playersPerTeam[code].Remove(targetPlayer))
                        {
                            if (PlayerLeftTeam != null)
                            {
                                PlayerLeftTeam(targetPlayer, teamsByCode[code]);
                            }
                            break;
                        }
                    }
                    PhotonTeam team = teamsByCode[teamCode];
                    if (!playersPerTeam[teamCode].Add(targetPlayer))
                    {
                        Debug.LogWarningFormat("Unexpected situation while setting team {0} for player {1}, updating teams for all", team, targetPlayer);
                        this.UpdateTeams();
                    }
                    if (PlayerJoinedTeam != null)
                    {
                        PlayerJoinedTeam(targetPlayer, team);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Unexpected: custom property key {0} should have of type byte, instead we got {1} of type {2}. Player: {3}", 
                        TeamPlayerProp, temp, temp.GetType(), targetPlayer);
                }
            }
        }

        void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer.IsInactive)
            {
                return;
            }
            PhotonTeam team = otherPlayer.GetPhotonTeam();
            if (team != null && !playersPerTeam[team.Code].Remove(otherPlayer))
            {
                Debug.LogWarningFormat("Unexpected situation while removing player {0} who left from team {1}, updating teams for all", otherPlayer, team);
                // revert to 'brute force' in case of unexpected situation
                this.UpdateTeams();
            }
        }

        void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
        {
            PhotonTeam team = newPlayer.GetPhotonTeam();
            if (team == null)
            {
                return;
            }
            if (playersPerTeam[team.Code].Contains(newPlayer))
            {
                // player rejoined w/ same team
                return;
            }
            // check if player rejoined w/ different team, remove from previous team 
            foreach (var key in teamsByCode.Keys)
            {
                if (playersPerTeam[key].Remove(newPlayer))
                {
                    break;
                }
            }
            if (!playersPerTeam[team.Code].Add(newPlayer))
            {
                Debug.LogWarningFormat("Unexpected situation while adding player {0} who joined to team {1}, updating teams for all", newPlayer, team);
                // revert to 'brute force' in case of unexpected situation
                this.UpdateTeams();
            }
        }

        #endregion

        #region Private methods

        private void UpdateTeams()
        {
            this.ClearTeams();
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];
                PhotonTeam playerTeam = player.GetPhotonTeam();
                if (playerTeam != null)
                {
                    playersPerTeam[playerTeam.Code].Add(player);
                }
            }
        }

        private void ClearTeams()
        {
            foreach (var key in playersPerTeam.Keys)
            {
                playersPerTeam[key].Clear();
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Find a PhotonTeam using a team code.
        /// </summary>
        /// <param name="code">The team code.</param>
        /// <param name="team">The team to be assigned if found.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamByCode(byte code, out PhotonTeam team)
        {
            return teamsByCode.TryGetValue(code, out team);
        }

        /// <summary>
        /// Find a PhotonTeam using a team name.
        /// </summary>
        /// <param name="teamName">The team name.</param>
        /// <param name="team">The team to be assigned if found.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamByName(string teamName, out PhotonTeam team)
        {
            return teamsByName.TryGetValue(teamName, out team);
        }

        /// <summary>
        /// Gets all teams available.
        /// </summary>
        /// <returns>Returns all teams available.</returns>
        public PhotonTeam[] GetAvailableTeams()
        {
            if (teamsList != null)
            {
                return teamsList.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Gets all players joined to a team using a team code.
        /// </summary>
        /// <param name="code">The code of the team.</param>
        /// <param name="members">The array of players to be filled.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamMembers(byte code, out Player[] members)
        {
            members = null;
            HashSet<Player> players;
            if (this.playersPerTeam.TryGetValue(code, out players))
            {
                members = new Player[players.Count];
                int i = 0;
                foreach (var player in players)
                {
                    members[i] = player;
                    i++;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all players joined to a team using a team name.
        /// </summary>
        /// <param name="teamName">The name of the team.</param>
        /// <param name="members">The array of players to be filled.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamMembers(string teamName, out Player[] members)
        {
            members = null;
            PhotonTeam team;
            if (this.TryGetTeamByName(teamName, out team))
            {
                return this.TryGetTeamMembers(team.Code, out members);
            }
            return false;
        }

        /// <summary>
        /// Gets all players joined to a team.
        /// </summary>
        /// <param name="team">The team which will be used to find players.</param>
        /// <param name="members">The array of players to be filled.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamMembers(PhotonTeam team, out Player[] members)
        {
            members = null;
            if (team != null)
            {
                return this.TryGetTeamMembers(team.Code, out members);
            }
            return false;
        }

        /// <summary>
        /// Gets all team mates of a player.
        /// </summary>
        /// <param name="player">The player whose team mates will be searched.</param>
        /// <param name="teamMates">The array of players to be filled.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamMatesOfPlayer(Player player, out Player[] teamMates)
        {
            teamMates = null;
            if (player == null)
            {
                return false;
            }
            PhotonTeam team = player.GetPhotonTeam();
            if (team == null)
            {
                return false;
            }
            HashSet<Player> players;
            if (this.playersPerTeam.TryGetValue(team.Code, out players))
            {
                if (!players.Contains(player))
                {
                    Debug.LogWarningFormat("Unexpected situation while getting team mates of player {0} who is joined to team {1}, updating teams for all", player, team);
                    // revert to 'brute force' in case of unexpected situation
                    this.UpdateTeams();
                }
                teamMates = new Player[players.Count - 1];
                int i = 0;
                foreach (var p in players)
                {
                    if (p.Equals(player))
                    {
                        continue;
                    }
                    teamMates[i] = p;
                    i++;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of players in a team by team code.
        /// </summary>
        /// <param name="code">Unique code of the team</param>
        /// <returns>Number of players joined to the team.</returns>
        public int GetTeamMembersCount(byte code)
        {
            PhotonTeam team;
            if (this.TryGetTeamByCode(code, out team))
            {
                return this.GetTeamMembersCount(team);
            }
            return 0;
        }

        /// <summary>
        /// Gets the number of players in a team by team name.
        /// </summary>
        /// <param name="name">Unique name of the team</param>
        /// <returns>Number of players joined to the team.</returns>
        public int GetTeamMembersCount(string name)
        {
            PhotonTeam team;
            if (this.TryGetTeamByName(name, out team))
            {
                return this.GetTeamMembersCount(team);
            }
            return 0;
        }

        /// <summary>
        /// Gets the number of players in a team.
        /// </summary>
        /// <param name="team">The team you want to know the size of</param>
        /// <returns>Number of players joined to the team.</returns>
        public int GetTeamMembersCount(PhotonTeam team)
        {
            HashSet<Player> players;
            if (team != null && this.playersPerTeam.TryGetValue(team.Code, out players) && players != null)
            {
                return players.Count;
            }
            return 0;
        }

        #endregion

        #region Unused methods

        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        void IMatchmakingCallbacks.OnCreatedRoom()
        {
        }

        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
        {
        }

        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
        {
        }

        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
        {
        }

        void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }

        void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
        {
        }

        #endregion
    }

    /// <summary>Extension methods for the Player class that make use of PhotonTeamsManager.</summary>
    public static class PhotonTeamExtensions
    {
        /// <summary>Gets the team the player is currently joined to. Null if none.</summary>
        /// <returns>The team the player is currently joined to. Null if none.</returns>
        public static PhotonTeam GetPhotonTeam(this Player player)
        {
            object teamId;
            PhotonTeam team;
            if (player.CustomProperties.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamId) && PhotonTeamsManager.Instance.TryGetTeamByCode((byte)teamId, out team))
            {
                return team;
            }
            return null;
        }

        /// <summary>
        /// Join a team.
        /// </summary>
        /// <param name="player">The player who will join a team.</param>
        /// <param name="team">The team to be joined.</param>
        /// <returns></returns>
        public static bool JoinTeam(this Player player, PhotonTeam team)
        {
            if (team == null)
            {
                Debug.LogWarning("JoinTeam failed: PhotonTeam provided is null");
                return false;
            }
            PhotonTeam currentTeam = player.GetPhotonTeam();
            if (currentTeam != null)
            {
                Debug.LogWarningFormat("JoinTeam failed: player ({0}) is already joined to a team ({1}), call SwitchTeam instead", player, team);
                return false;
            }
            return player.SetCustomProperties(new Hashtable { { PhotonTeamsManager.TeamPlayerProp, team.Code } });
        }

        /// <summary>
        /// Join a team using team code.
        /// </summary>
        /// <param name="player">The player who will join the team.</param>
        /// <param name="teamCode">The code fo the team to be joined.</param>
        /// <returns></returns>
        public static bool JoinTeam(this Player player, byte teamCode)
        {
            PhotonTeam team;
            return PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out team) && player.JoinTeam(team);
        }

        /// <summary>
        /// Join a team using team name.
        /// </summary>
        /// <param name="player">The player who will join the team.</param>
        /// <param name="teamName">The name of the team to be joined.</param>
        /// <returns></returns>
        public static bool JoinTeam(this Player player, string teamName)
        {
            PhotonTeam team;
            return PhotonTeamsManager.Instance.TryGetTeamByName(teamName, out team) && player.JoinTeam(team);
        }

        /// <summary>Switch that player's team to the one you assign.</summary>
        /// <remarks>Internally checks if this player is in that team already or not. Only team switches are actually sent.</remarks>
        /// <param name="player"></param>
        /// <param name="team"></param>
        public static bool SwitchTeam(this Player player, PhotonTeam team)
        {
            if (team == null)
            {
                Debug.LogWarning("SwitchTeam failed: PhotonTeam provided is null");
                return false;
            }
            PhotonTeam currentTeam = player.GetPhotonTeam();
            if (currentTeam == null)
            {
                Debug.LogWarningFormat("SwitchTeam failed: player ({0}) was not joined to any team, call JoinTeam instead", player);
                return false;
            }
            if (currentTeam.Code == team.Code)
            {
                Debug.LogWarningFormat("SwitchTeam failed: player ({0}) is already joined to the same team {1}", player, team);
                return false;
            }
            return player.SetCustomProperties(new Hashtable { { PhotonTeamsManager.TeamPlayerProp, team.Code } },
                new Hashtable { { PhotonTeamsManager.TeamPlayerProp, currentTeam.Code }});
        }

        /// <summary>Switch the player's team using a team code.</summary>
        /// <remarks>Internally checks if this player is in that team already or not.</remarks>
        /// <param name="player">The player that will switch teams.</param>
        /// <param name="teamCode">The code of the team to switch to.</param>
        /// <returns>If the team switch request is queued to be sent to the server or done in case offline or not joined to a room yet.</returns>
        public static bool SwitchTeam(this Player player, byte teamCode)
        {
            PhotonTeam team;
            return PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out team) && player.SwitchTeam(team);
        }

        /// <summary>Switch the player's team using a team name.</summary>
        /// <remarks>Internally checks if this player is in that team already or not.</remarks>
        /// <param name="player">The player that will switch teams.</param>
        /// <param name="teamName">The name of the team to switch to.</param>
        /// <returns>If the team switch request is queued to be sent to the server or done in case offline or not joined to a room yet.</returns>
        public static bool SwitchTeam(this Player player, string teamName)
        {
            PhotonTeam team;
            return PhotonTeamsManager.Instance.TryGetTeamByName(teamName, out team) && player.SwitchTeam(team);
        }

        /// <summary>
        /// Leave the current team if any.
        /// </summary>
        /// <param name="player"></param>
        /// <returns>If the leaving team request is queued to be sent to the server or done in case offline or not joined to a room yet.</returns>
        public static bool LeaveCurrentTeam(this Player player)
        {
            PhotonTeam currentTeam = player.GetPhotonTeam();
            if (currentTeam == null)
            {
                Debug.LogWarningFormat("LeaveCurrentTeam failed: player ({0}) was not joined to any team", player);
                return false;
            }
            return player.SetCustomProperties(new Hashtable {{PhotonTeamsManager.TeamPlayerProp, null}}, new Hashtable {{PhotonTeamsManager.TeamPlayerProp, currentTeam.Code}});
        }

        /// <summary>
        /// Try to get the team mates.
        /// </summary>
        /// <param name="player">The player to get the team mates of.</param>
        /// <param name="teamMates">The team mates array to fill.</param>
        /// <returns>If successful or not.</returns>
        public static bool TryGetTeamMates(this Player player, out Player[] teamMates)
        {
            return PhotonTeamsManager.Instance.TryGetTeamMatesOfPlayer(player, out teamMates);
        }
    }
}