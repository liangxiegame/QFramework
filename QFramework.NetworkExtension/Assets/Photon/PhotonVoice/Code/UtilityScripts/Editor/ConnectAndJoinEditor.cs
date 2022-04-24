namespace Photon.Voice.Unity.UtilityScripts.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ConnectAndJoin))]
    public class ConnectAndJoinEditor : Editor
    {
        private ConnectAndJoin connectAndJoin;
        private SerializedProperty randomRoomSp;
        private SerializedProperty roomNameSp;
        private SerializedProperty autoConnectSp;
        private SerializedProperty autoTransmitSp;
        private SerializedProperty publishUserIdSp;

        private void OnEnable()
        {
            this.connectAndJoin = this.target as ConnectAndJoin;
            this.randomRoomSp = this.serializedObject.FindProperty("RandomRoom");
            this.roomNameSp = this.serializedObject.FindProperty("RoomName");
            this.autoConnectSp = this.serializedObject.FindProperty("autoConnect");
            this.autoTransmitSp = this.serializedObject.FindProperty("autoTransmit");
            this.publishUserIdSp = this.serializedObject.FindProperty("publishUserId");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.autoConnectSp);
            EditorGUILayout.PropertyField(this.autoTransmitSp);
            EditorGUILayout.PropertyField(this.randomRoomSp);
            EditorGUILayout.PropertyField(this.publishUserIdSp);
            if (!this.randomRoomSp.boolValue)
            {
                EditorGUILayout.PropertyField(this.roomNameSp);
            }
            if (Application.isPlaying && !this.connectAndJoin.IsConnected)
            {
                if (GUILayout.Button("Connect"))
                {
                    this.connectAndJoin.ConnectNow();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}