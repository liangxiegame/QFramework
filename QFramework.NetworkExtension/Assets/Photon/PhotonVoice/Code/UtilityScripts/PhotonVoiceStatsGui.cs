// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonVoiceStatsGui.cs" company="Exit Games GmbH">
//   Part of: Photon Voice Utilities for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// This MonoBehaviour is a basic GUI for the Photon Voice client's network statistics.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Voice.Unity.UtilityScripts
{
    /// <summary>
    /// Basic GUI to show traffic and health statistics of the connection to Photon,
    /// toggled by shift+tab.
    /// </summary>
    /// <remarks>
    /// The shown health values can help identify problems with connection losses or performance.
    /// Example:
    /// If the time delta between two consecutive SendOutgoingCommands calls is a second or more,
    /// chances rise for a disconnect being caused by this (because acknowledgments to the server
    /// need to be sent in due time).
    /// </remarks>
    public class PhotonVoiceStatsGui : MonoBehaviour
    {
        /// <summary>Shows or hides GUI (does not affect if stats are collected).</summary>
        private bool statsWindowOn = true;

        /// <summary>Option to turn collecting stats on or off (used in Update()).</summary>
        private bool statsOn;

        /// <summary>Shows additional "health" values of connection.</summary>
        private bool healthStatsVisible;

        /// <summary>Shows additional "lower level" traffic stats.</summary>
        private bool trafficStatsOn;

        /// <summary>Show buttons to control stats and reset them.</summary>
        private bool buttonsOn;

        private bool voiceStatsOn = true;

        /// <summary>Positioning rect for window.</summary>
        private Rect statsRect = new Rect(0, 100, 300, 50);

        /// <summary>Unity GUI Window ID (must be unique or will cause issues).</summary>
        private int windowId = 200;

        /// <summary>The peer currently in use (to set the network simulation).</summary>
        private PhotonPeer peer;

        private VoiceConnection voiceConnection;

        private VoiceClient voiceClient;

        private void OnEnable()
        {
            VoiceConnection[] voiceConnections = this.GetComponents<VoiceConnection>();
            if (voiceConnections == null || voiceConnections.Length == 0)
            {
                Debug.LogError("No VoiceConnection component found, PhotonVoiceStatsGui disabled", this);
                this.enabled = false;
                return;
            }
            if (voiceConnections.Length > 1)
            {
                Debug.LogWarningFormat(this, "Multiple VoiceConnection components found, using first occurrence attached to GameObject {0}", voiceConnections[0].name);
            }
            this.voiceConnection = voiceConnections[0];
            this.voiceClient = this.voiceConnection.VoiceClient;
            this.peer = this.voiceConnection.Client.LoadBalancingPeer;
            if (this.statsRect.x <= 0)
            {
                this.statsRect.x = Screen.width - this.statsRect.width;
            }
        }

        /// <summary>Checks for shift+tab input combination (to toggle statsOn).</summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                this.statsWindowOn = !this.statsWindowOn;
                this.statsOn = true;    // enable stats when showing the window
            }
        }

        private void OnGUI()
        {
            if (this.peer.TrafficStatsEnabled != this.statsOn)
            {
                this.peer.TrafficStatsEnabled = this.statsOn;
            }

            if (!this.statsWindowOn)
            {
                return;
            }

            this.statsRect = GUILayout.Window(this.windowId, this.statsRect, this.TrafficStatsWindow, "Voice Client Messages (shift+tab)");
        }

        private void TrafficStatsWindow(int windowId)
        {
            bool statsToLog = false;
            TrafficStatsGameLevel gls = this.peer.TrafficStatsGameLevel;
            long elapsedMs = this.peer.TrafficStatsElapsedMs / 1000;
            if (elapsedMs == 0)
            {
                elapsedMs = 1;
            }

            GUILayout.BeginHorizontal();
            this.buttonsOn = GUILayout.Toggle(this.buttonsOn, "buttons");
            this.healthStatsVisible = GUILayout.Toggle(this.healthStatsVisible, "health");
            this.trafficStatsOn = GUILayout.Toggle(this.trafficStatsOn, "traffic");
            this.voiceStatsOn = GUILayout.Toggle(this.voiceStatsOn, "voice stats");
            GUILayout.EndHorizontal();

            string total = string.Format("Out {0,4} | In {1,4} | Sum {2,4}", gls.TotalOutgoingMessageCount, gls.TotalIncomingMessageCount, gls.TotalMessageCount);
            string elapsedTime = string.Format("{0}sec average:", elapsedMs);
            string average = string.Format("Out {0,4} | In {1,4} | Sum {2,4}", gls.TotalOutgoingMessageCount / elapsedMs, gls.TotalIncomingMessageCount / elapsedMs, gls.TotalMessageCount / elapsedMs);
            GUILayout.Label(total);
            GUILayout.Label(elapsedTime);
            GUILayout.Label(average);

            if (this.buttonsOn)
            {
                GUILayout.BeginHorizontal();
                this.statsOn = GUILayout.Toggle(this.statsOn, "stats on");
                if (GUILayout.Button("Reset"))
                {
                    this.peer.TrafficStatsReset();
                    this.peer.TrafficStatsEnabled = true;
                }
                statsToLog = GUILayout.Button("To Log");
                GUILayout.EndHorizontal();
            }

            string trafficStatsIn = string.Empty;
            string trafficStatsOut = string.Empty;
            if (this.trafficStatsOn)
            {
                GUILayout.Box("Voice Client Traffic Stats");
                trafficStatsIn = string.Concat("Incoming: \n", this.peer.TrafficStatsIncoming);
                trafficStatsOut = string.Concat("Outgoing: \n", this.peer.TrafficStatsOutgoing);
                GUILayout.Label(trafficStatsIn);
                GUILayout.Label(trafficStatsOut);
            }

            string healthStats = string.Empty;
            if (this.healthStatsVisible)
            {
                GUILayout.Box("Voice Client Health Stats");
                healthStats = string.Format(
                    "ping: {6}|{9}[+/-{7}|{10}]ms resent:{8} \n\nmax ms between\nsend: {0,4} \ndispatch: {1,4} \n\nlongest dispatch for: \nev({3}):{2,3}ms \nop({5}):{4,3}ms",
                    gls.LongestDeltaBetweenSending,
                    gls.LongestDeltaBetweenDispatching,
                    gls.LongestEventCallback,
                    gls.LongestEventCallbackCode,
                    gls.LongestOpResponseCallback,
                    gls.LongestOpResponseCallbackOpCode,
                    this.peer.RoundTripTime,
                    this.peer.RoundTripTimeVariance,
                    this.peer.ResentReliableCommands,
                    this.voiceClient.RoundTripTime,
                    this.voiceClient.RoundTripTimeVariance);
                GUILayout.Label(healthStats);
            }

            string voiceStats = string.Empty;
            if (this.voiceStatsOn)
            {
                GUILayout.Box("Voice Frames Stats");
                voiceStats = string.Format("received: {0}, {1:F2}/s \n\nlost: {2}, {3:F2}/s ({4:F2}%) \n\nsent: {5} ({6} bytes)",
                    this.voiceClient.FramesReceived,
                    this.voiceConnection.FramesReceivedPerSecond,
                    this.voiceClient.FramesLost,
                    this.voiceConnection.FramesLostPerSecond,
                    this.voiceConnection.FramesLostPercent,
                    this.voiceClient.FramesSent,
                    this.voiceClient.FramesSentBytes);
                GUILayout.Label(voiceStats);
            }

            if (statsToLog)
            {
                string complete = string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", total, elapsedTime, average, trafficStatsIn, trafficStatsOut, healthStats);
                Debug.Log(complete);
            }

            // if anything was clicked, the height of this window is likely changed. reduce it to be layouted again next frame
            if (GUI.changed)
            {
                this.statsRect.height = 100;
            }

            GUI.DragWindow();
        }
    }
}


