#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_SWITCH || UNITY_IOS
#define PHOTON_AUDIO_CHANGE_IN_NOTIFIER
#endif

namespace Photon.Voice.Unity.Editor {
    using Unity;
    using UnityEditor;

    [CustomEditor(typeof(AudioChangesHandler))]
    public class AudioChangesHandlerEditor : Editor {
        public override void OnInspectorGUI() {
            this.serializedObject.UpdateIfRequiredOrScript();
            VoiceLogger.ExposeLogLevel(this.serializedObject, this.target as ILoggable);
            EditorGUI.BeginChangeCheck();
            this.DrawSerializedProperty("StartWhenDeviceChange");
            SerializedProperty useUnity = this.DrawSerializedProperty("UseOnAudioConfigurationChanged");
            bool showMore = useUnity.boolValue;
            if (showMore) {
                this.DrawSerializedProperty("HandleConfigChange");
            }
            #if PHOTON_AUDIO_CHANGE_IN_NOTIFIER
            SerializedProperty usePhoton = this.DrawSerializedProperty("UseNativePluginChangeNotifier");
            showMore |= usePhoton.boolValue;
            #endif
            if (showMore) {
                if (this.DrawSerializedProperty("HandleDeviceChange").boolValue) {
                    this.DrawSerializedProperty("Android_AlwaysHandleDeviceChange");
                    this.DrawSerializedProperty("iOS_AlwaysHandleDeviceChange");
                }
            }
            if (EditorGUI.EndChangeCheck()) {
                this.serializedObject.ApplyModifiedProperties();
            }
        }

        private SerializedProperty DrawSerializedProperty(string propertyName) {
            SerializedProperty serializedProperty = this.serializedObject.FindProperty(propertyName);
            EditorGUILayout.PropertyField(serializedProperty);
            return serializedProperty;
        }
    }
}
