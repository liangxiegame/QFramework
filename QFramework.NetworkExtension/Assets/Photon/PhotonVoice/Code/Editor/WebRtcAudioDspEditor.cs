#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_ANDROID || UNITY_WSA
#define WEBRTC_AUDIO_DSP_SUPPORTED_PLATFORMS
#endif

#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
#define WEBRTC_AUDIO_DSP_SUPPORTED_EDITOR
#endif

using UnityEngine;

namespace Photon.Voice.Unity.Editor
{
    using UnityEditor;
    using Unity;

    [CustomEditor(typeof(WebRtcAudioDsp))]
    public class WebRtcAudioDspEditor : Editor
    {
        private WebRtcAudioDsp processor;
        private Recorder recorder;

        private SerializedProperty aecSp;
        private SerializedProperty aecHighPassSp;
        private SerializedProperty agcSp;
        private SerializedProperty agcCompressionGainSp;
        private SerializedProperty vadSp;
        private SerializedProperty highPassSp;
        private SerializedProperty bypassSp;
        private SerializedProperty noiseSuppressionSp;
        private SerializedProperty reverseStreamDelayMsSp;

        private void OnEnable()
        {
            this.processor = this.target as WebRtcAudioDsp;
            this.recorder = this.processor.GetComponent<Recorder>();
            this.aecSp = this.serializedObject.FindProperty("aec");
            this.aecHighPassSp = this.serializedObject.FindProperty("aecHighPass");
            this.agcSp = this.serializedObject.FindProperty("agc");
            this.agcCompressionGainSp = this.serializedObject.FindProperty("agcCompressionGain");
            this.vadSp = this.serializedObject.FindProperty("vad");
            this.highPassSp = this.serializedObject.FindProperty("highPass");
            this.bypassSp = this.serializedObject.FindProperty("bypass");
            this.noiseSuppressionSp = this.serializedObject.FindProperty("noiseSuppression");
            this.reverseStreamDelayMsSp = this.serializedObject.FindProperty("reverseStreamDelayMs");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.UpdateIfRequiredOrScript();
            if (!PhotonVoiceEditorUtils.IsPrefab(this.processor.gameObject))
            {
                #if WEBRTC_AUDIO_DSP_SUPPORTED_PLATFORMS
                #elif WEBRTC_AUDIO_DSP_SUPPORTED_EDITOR
                string message = string.Format("WebRtcAudioDsp is not supported on this target platform {0}. The component will be disabled in build.", EditorUserBuildSettings.activeBuildTarget);
                EditorGUILayout.HelpBox(message, MessageType.Warning);
                #else
                string message = string.Format("WebRtcAudioDsp is not supported on this target platform {0}. This component is disabled.", EditorUserBuildSettings.activeBuildTarget);
                EditorGUILayout.HelpBox(message, MessageType.Warning);
                #endif
            }
            if (!this.processor.isActiveAndEnabled && this.processor.AecOnlyWhenEnabled && this.aecSp.boolValue)
            {
                EditorGUILayout.HelpBox("WebRtcAudioDsp is not enabled, AEC will not be used.", MessageType.Warning);
            }
            if (this.recorder != null && this.recorder.SourceType != Recorder.InputSourceType.Microphone)
            {
                EditorGUILayout.HelpBox("WebRtcAudioDsp is better suited to be used with Microphone as Recorder Input Source Type.", MessageType.Warning);
            }
            VoiceLogger.ExposeLogLevel(this.serializedObject, this.processor);
            bool bypassed;
            EditorGUI.BeginChangeCheck();
            bool isInSceneInPlayMode = PhotonVoiceEditorUtils.IsInTheSceneInPlayMode(this.processor.gameObject);
            if (isInSceneInPlayMode)
            {
                this.processor.Bypass = EditorGUILayout.Toggle(new GUIContent("Bypass", "Bypass WebRTC Audio DSP"), this.processor.Bypass);
                bypassed = this.processor.Bypass;
            }
            else
            {
                EditorGUILayout.PropertyField(this.bypassSp, new GUIContent("Bypass", "Bypass WebRTC Audio DSP"));
                bypassed = this.bypassSp.boolValue;
            }
            #if UNITY_ANDROID
            SerializedObject serializedObject = new SerializedObject(this.recorder);
            SerializedProperty serializedProperty = serializedObject.FindProperty("nativeAndroidMicrophoneSettings");
            #endif
            if (!bypassed)
            {
                if (isInSceneInPlayMode)
                {
                    this.processor.AEC = EditorGUILayout.Toggle(new GUIContent("AEC", "Acoustic Echo Cancellation"), this.processor.AEC);
                    if (this.processor.AEC)
                    {
                        if (this.recorder.SourceType == Recorder.InputSourceType.Microphone && this.recorder.MicrophoneType == Recorder.MicType.Photon)
                        {
                            #if UNITY_ANDROID
                            if (serializedProperty.FindPropertyRelative("AcousticEchoCancellation").boolValue)
                            {
                                EditorGUILayout.HelpBox("You have enabled AEC here and are using a Photon Mic as input on the Recorder, which might add its own echo cancellation. Please use only one AEC algorithm.", MessageType.Warning);
                            }
                            #else
                            EditorGUILayout.HelpBox("You have enabled AEC here and are using a Photon Mic as input on the Recorder, which might add its own echo cancellation. Please use only one AEC algorithm.", MessageType.Warning);
                            #endif
                        }
                        this.processor.ReverseStreamDelayMs = EditorGUILayout.IntField(new GUIContent("ReverseStreamDelayMs", "Reverse stream delay (hint for AEC) in Milliseconds"), this.processor.ReverseStreamDelayMs);
                        this.processor.AecHighPass = EditorGUILayout.Toggle(new GUIContent("AEC High Pass"), this.processor.AecHighPass);
                    }
                    this.processor.AGC = EditorGUILayout.Toggle(new GUIContent("AGC", "Automatic Gain Control"), this.processor.AGC);
                    if (this.processor.AGC)
                    {
                        #if UNITY_ANDROID
                        if (serializedProperty.FindPropertyRelative("AutomaticGainControl").boolValue)
                        {
                            EditorGUILayout.HelpBox("You have enabled AGC here and are using a AGC from native plugin (Photon microphone type). Please use only one AGC algorithm.", MessageType.Warning);
                        }
                        #endif
                        this.processor.AgcCompressionGain = EditorGUILayout.IntField(new GUIContent("AGC Compression Gain"), this.processor.AgcCompressionGain);
                    }
                    if (this.processor.VAD && this.recorder.VoiceDetection)
                    {
                        EditorGUILayout.HelpBox("You have enabled VAD here and in the associated Recorder. Please use only one Voice Detection algorithm.", MessageType.Warning);
                    }
                    this.processor.VAD = EditorGUILayout.Toggle(new GUIContent("VAD", "Voice Activity Detection"), this.processor.VAD);
                    this.processor.HighPass = EditorGUILayout.Toggle(new GUIContent("HighPass", "High Pass Filter"), this.processor.HighPass);
                    this.processor.NoiseSuppression = EditorGUILayout.Toggle(new GUIContent("NoiseSuppression", "Noise Suppression"), this.processor.NoiseSuppression);
                    if (this.processor.NoiseSuppression)
                    {
                        #if UNITY_ANDROID
                        if (serializedProperty.FindPropertyRelative("NoiseSuppression").boolValue)
                        {
                            EditorGUILayout.HelpBox("You have enabled NS here and are using a NS from native plugin (Photon microphone type). Please use only one NS algorithm.", MessageType.Warning);
                        }
                        #endif
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(this.aecSp, new GUIContent("AEC", "Acoustic Echo Cancellation"));
                    if (this.aecSp.boolValue)
                    {
                        if (this.recorder.SourceType == Recorder.InputSourceType.Microphone && this.recorder.MicrophoneType == Recorder.MicType.Photon)
                        {
                            #if UNITY_ANDROID
                            if (serializedProperty.FindPropertyRelative("AcousticEchoCancellation").boolValue)
                            {
                                EditorGUILayout.HelpBox("You have enabled AEC here and are using AEC from native plugin (Photon microphone type). Please use only one AEC algorithm.", MessageType.Warning);
                            }
                            #else
                            EditorGUILayout.HelpBox("You have enabled AEC here and are using a Photon Mic as input on the Recorder, which might add its own echo cancellation. Please use only one AEC algorithm.", MessageType.Warning);
                            #endif
                        }
                        EditorGUILayout.PropertyField(this.reverseStreamDelayMsSp,
                            new GUIContent("ReverseStreamDelayMs", "Reverse stream delay (hint for AEC) in Milliseconds"));
                        EditorGUILayout.PropertyField(this.aecHighPassSp, new GUIContent("AEC High Pass"));
                    }
                    EditorGUILayout.PropertyField(this.agcSp, new GUIContent("AGC", "Automatic Gain Control"));
                    if (this.agcSp.boolValue)
                    {
                        #if UNITY_ANDROID
                        if (serializedProperty.FindPropertyRelative("AutomaticGainControl").boolValue)
                        {
                            EditorGUILayout.HelpBox("You have enabled AGC here and are using a AGC from native plugin (Photon microphone type). Please use only one AGC algorithm.", MessageType.Warning);
                        }
                        #endif
                        EditorGUILayout.PropertyField(this.agcCompressionGainSp, new GUIContent("AGC Compression Gain"));
                    }
                    if (this.vadSp.boolValue && this.recorder.VoiceDetection)
                    {
                        EditorGUILayout.HelpBox("You have enabled VAD here and in the associated Recorder. Please use only one Voice Detection algorithm.", MessageType.Warning);
                    }
                    EditorGUILayout.PropertyField(this.vadSp, new GUIContent("VAD", "Voice Activity Detection"));
                    EditorGUILayout.PropertyField(this.highPassSp, new GUIContent("HighPass", "High Pass Filter"));
                    EditorGUILayout.PropertyField(this.noiseSuppressionSp, new GUIContent("NoiseSuppression", "Noise Suppression"));
                    if (this.noiseSuppressionSp.boolValue)
                    {
                        #if UNITY_ANDROID
                        if (serializedProperty.FindPropertyRelative("NoiseSuppression").boolValue)
                        {
                            EditorGUILayout.HelpBox("You have enabled NS here and are using a NS from native plugin (Photon microphone type). Please use only one NS algorithm.", MessageType.Warning);
                        }
                        #endif
                    }
                }
            }
                
            if (EditorGUI.EndChangeCheck())
            {
                this.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
