// ----------------------------------------------------------------------------
// <copyright file="VoiceComponent.cs" company="Exit Games GmbH">
//   Photon Voice for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Base class for voice components.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Voice.Unity
{
    using ExitGames.Client.Photon;
    using UnityEngine;

    [HelpURL("https://doc.photonengine.com/en-us/voice/v2")]
    public abstract class VoiceComponent : MonoBehaviour, ILoggableDependent
    {
        private VoiceLogger logger;

        public VoiceLogger Logger
        {
            get
            {
                if (this.logger == null)
                {
                    this.logger = new VoiceLogger(this, string.Format("{0}.{1}", this.name, this.GetType().Name), this.logLevel);
                }
                return this.logger;
            }
            protected set { this.logger = value; }
        }

        [SerializeField]
        protected DebugLevel logLevel = DebugLevel.WARNING;
        public DebugLevel LogLevel
        {
            get
            {
                if (this.Logger != null)
                {
                    this.logLevel = this.Logger.LogLevel;
                }
                return this.logLevel;
            }
            set
            {
                this.logLevel = value;
                if (this.Logger == null)
                {
                    return;
                }
                this.Logger.LogLevel = this.logLevel;
            }
        }

        [SerializeField, HideInInspector]
        private bool ignoreGlobalLogLevel;

        public bool IgnoreGlobalLogLevel
        {
            get { return this.ignoreGlobalLogLevel; }
            set { this.ignoreGlobalLogLevel = value; }
        }

        private static string currentPlatform;
        public static string CurrentPlatform
        {
            get
            {
                if (string.IsNullOrEmpty(currentPlatform))
                {
                    #if UNITY_EDITOR
                    currentPlatform = System.Enum.GetName(typeof(UnityEditor.BuildTarget), UnityEditor.EditorUserBuildSettings.activeBuildTarget);
                    #else
                    currentPlatform = System.Enum.GetName(typeof(RuntimePlatform), Application.platform);
                    #endif
                }
                return currentPlatform;
            }
        }

        protected virtual void Awake()
        {
            if (this.logger == null)
            {
                this.logger = new VoiceLogger(this, string.Format("{0}.{1}", this.name, this.GetType().Name), this.logLevel);
            }
        }
    }
}