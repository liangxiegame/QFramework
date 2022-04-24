using System;

namespace Photon.Voice.PUN.Editor
{
    using Unity.Editor;
    using UnityEditor;
    using UnityEngine;
    using Pun;

    [CustomEditor(typeof(PhotonVoiceNetwork))]
    public class PhotonVoiceNetworkEditor : VoiceConnectionEditor
    {
        private SerializedProperty autoConnectAndJoinSp;
        private SerializedProperty autoLeaveAndDisconnectSp;
        private SerializedProperty usePunAppSettingsSp;
        private SerializedProperty usePunAuthValuesSp;
        private SerializedProperty workInOfflineModeSp;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.autoConnectAndJoinSp = this.serializedObject.FindProperty("AutoConnectAndJoin");
            this.autoLeaveAndDisconnectSp = this.serializedObject.FindProperty("AutoLeaveAndDisconnect");
            this.usePunAppSettingsSp = this.serializedObject.FindProperty("usePunAppSettings");
            this.usePunAuthValuesSp = this.serializedObject.FindProperty("usePunAuthValues");
            this.workInOfflineModeSp = this.serializedObject.FindProperty("WorkInOfflineMode");
        }

        protected override void DisplayAppSettings()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(this.usePunAppSettingsSp, new GUIContent("Use PUN's App Settings", "Use App Settings From PUN's PhotonServerSettings"));
            if (GUILayout.Button("PhotonServerSettings", EditorStyles.miniButton, GUILayout.Width(120)))
            {
                Selection.objects = new Object[] { PhotonNetwork.PhotonServerSettings };
                EditorGUIUtility.PingObject(PhotonNetwork.PhotonServerSettings);
            }
            EditorGUILayout.EndHorizontal();
            if (!this.usePunAppSettingsSp.boolValue)
            {
                base.DisplayAppSettings();
            }
            EditorGUILayout.PropertyField(this.usePunAuthValuesSp, new GUIContent("Use PUN's Auth Values", "Use the same Authentication Values From PUN client"));
        }

        protected override void ShowHeader()
        {
            base.ShowHeader();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.autoConnectAndJoinSp, new GUIContent("Auto Connect And Join", "Auto connect voice client and join a voice room when PUN client is joined to a PUN room"));
            EditorGUILayout.PropertyField(this.autoLeaveAndDisconnectSp, new GUIContent("Auto Leave And Disconnect", "Auto disconnect voice client when PUN client is not joined to a PUN room"));
            EditorGUILayout.PropertyField(this.workInOfflineModeSp, new GUIContent("Work In Offline Mode", "Whether or not Photon Voice client should follow PUN client if the latter is in offline mode."));
            if (EditorGUI.EndChangeCheck())
            {
                this.serializedObject.ApplyModifiedProperties();
            }
        }

        protected override void ShowAssetVersions()
        {
            base.ShowAssetVersions();
            string version = this.GetVersionString(this.punChangelogVersion).TrimStart('v');
            if (!PhotonNetwork.PunVersion.Equals(version, StringComparison.OrdinalIgnoreCase))
            {
                EditorGUILayout.LabelField(string.Format("PUN2, Inside Voice: {0} != Imported Separately: {1}", version, PhotonNetwork.PunVersion));
            }
            else
            {
                EditorGUILayout.LabelField(string.Format("PUN2: {0}", version));
            }
        }
    }
}