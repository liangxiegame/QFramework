// ----------------------------------------------------------------------------
// <copyright file="VoiceLogger.cs" company="Exit Games GmbH">
//   Photon Voice for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Logger for voice components.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Voice.Unity
{
    using ExitGames.Client.Photon;
    using UnityEngine;

    public class VoiceLogger : Voice.ILogger
    {
        public VoiceLogger(Object context, string tag, DebugLevel level = DebugLevel.ERROR)
        {
            this.context = context;
            this.Tag = tag;
            this.LogLevel = level;
        }

        public VoiceLogger(string tag, DebugLevel level = DebugLevel.ERROR)
        {
            this.Tag = tag;
            this.LogLevel = level;
        }

        public string Tag { get; set; }

        public DebugLevel LogLevel { get; set; }

        public bool IsErrorEnabled
        {
            get { return this.LogLevel >= DebugLevel.ERROR; }
        }

        public bool IsWarningEnabled
        {
            get { return this.LogLevel >= DebugLevel.WARNING; }
        }

        public bool IsInfoEnabled
        {
            get { return this.LogLevel >= DebugLevel.INFO; }
        }

        public bool IsDebugEnabled { get { return this.LogLevel == DebugLevel.ALL; } }

        private readonly Object context;
        
        #region ILogger

        public void LogError(string fmt, params object[] args)
        {
            if (!this.IsErrorEnabled) return;
            fmt = this.GetFormatString(fmt);
            if (this.context == null)
            {
                Debug.LogErrorFormat(fmt, args);
            }
            else
            {
                Debug.LogErrorFormat(this.context, fmt, args);
            }
        }

        public void LogWarning(string fmt, params object[] args)
        {
            if (!this.IsWarningEnabled) return;            
            fmt = this.GetFormatString(fmt);
            if (this.context == null)
            {
                Debug.LogWarningFormat(fmt, args);
            }
            else
            {
                Debug.LogWarningFormat(this.context, fmt, args);
            }
        }

        public void LogInfo(string fmt, params object[] args)
        {
            if (!this.IsInfoEnabled) return;
            fmt = this.GetFormatString(fmt);
            if (this.context == null)
            {
                Debug.LogFormat(fmt, args);
            }
            else
            {
                Debug.LogFormat(this.context, fmt, args);
            }
        }

        public void LogDebug(string fmt, params object[] args)
        {
            if (!this.IsDebugEnabled) return;
            this.LogInfo(fmt, args);
        }

        #endregion

        private string GetFormatString(string fmt)
        {
            return string.Format("[{0}] {1}:{2}", this.Tag, this.GetTimestamp(), fmt);
        }

        private string GetTimestamp()
        {
            return System.DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", new System.Globalization.CultureInfo("en-US"));
        }

        #if UNITY_EDITOR
        public static void ExposeLogLevel(UnityEditor.SerializedObject obj, ILoggable loggable)
        {
            UnityEditor.SerializedProperty logLevelSp = obj.FindProperty("logLevel");
            UnityEditor.EditorGUI.BeginChangeCheck();
            UnityEditor.EditorGUILayout.PropertyField(logLevelSp, new GUIContent("Log Level", "Logging level for this Photon Voice component."));
            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                loggable.LogLevel = ExposeLogLevel(logLevelSp);
                obj.ApplyModifiedProperties();
            }
        }

        public static void ExposeLogLevel(UnityEditor.SerializedObject obj, ILoggableDependent loggable)
        {
            UnityEditor.SerializedProperty logLevelSp = obj.FindProperty("logLevel");
            UnityEditor.EditorGUI.BeginChangeCheck();
            UnityEditor.EditorGUILayout.BeginHorizontal();
            loggable.IgnoreGlobalLogLevel = UnityEditor.EditorGUILayout.Toggle(new GUIContent("Override Default Log Level", "Override the default logging level for this type of component."), loggable.IgnoreGlobalLogLevel);
            if (loggable.IgnoreGlobalLogLevel)
            {
                UnityEditor.EditorGUILayout.PropertyField(logLevelSp, new GUIContent("Log Level", "Logging level for this Photon Voice component."));
            }
            UnityEditor.EditorGUILayout.EndHorizontal();
            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                ExposeLogLevel(logLevelSp);
                loggable.LogLevel = (DebugLevel)logLevelSp.enumValueIndex;
                obj.ApplyModifiedProperties();
            }
        }

        public static DebugLevel ExposeLogLevel(UnityEditor.SerializedProperty sp)
        {
            UnityEditor.EditorGUILayout.PropertyField(sp, new GUIContent(sp.displayName, sp.tooltip));
            if (sp.enumValueIndex >= 4)
            {
                return DebugLevel.ALL;
            }
            return (DebugLevel)sp.enumValueIndex;
        }
        #endif
    }
}
