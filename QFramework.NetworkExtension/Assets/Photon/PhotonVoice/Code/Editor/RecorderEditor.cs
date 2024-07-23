#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
#define PHOTON_MICROPHONE_ENUMERATOR
#endif

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_ANDROID
#define WEBRTC_AUDIO_DSP_SUPPORTED_PLATFORMS
#endif

namespace Photon.Voice.Unity.Editor
{
    using POpusCodec.Enums;
    using System;
    using System.Linq;
    using Unity;
    using UnityEditor;
    using UnityEngine;
    #if UNITY_IOS
    using IOS;
    #endif

    [CustomEditor(typeof(Recorder))]
    public class RecorderEditor : Editor
    {
        private Recorder recorder;

        private int unityMicrophoneDeviceIndex;

        #if PHOTON_MICROPHONE_ENUMERATOR
        private string[] photonDeviceNames;
        private int[] photonDeviceIDs;
        private int photonDeviceIndex;
        #endif

        private int calibrationTime = 200;

        private SerializedProperty voiceDetectionSp;
        private SerializedProperty voiceDetectionThresholdSp;
        private SerializedProperty voiceDetectionDelayMsSp;
        private SerializedProperty photonMicrophoneDeviceIdSp;
        private SerializedProperty interestGroupSp;
        private SerializedProperty debugEchoModeSp;
        private SerializedProperty reliableModeSp;
        private SerializedProperty encryptSp;
        private SerializedProperty transmitEnabledSp;
        private SerializedProperty samplingRateSp;
        private SerializedProperty frameDurationSp;
        private SerializedProperty bitrateSp;
        private SerializedProperty sourceTypeSp;
        private SerializedProperty microphoneTypeSp;
        private SerializedProperty audioClipSp;
        private SerializedProperty loopAudioClipSp;
        private SerializedProperty autoStartSp;
        private SerializedProperty recordOnlyWhenEnabledSp;
        private SerializedProperty stopRecordingWhenPausedSp;
        private SerializedProperty useMicrophoneTypeFallbackSp;
        private SerializedProperty recordOnlyWhenJoinedSp;

        #if UNITY_IOS
        private SerializedProperty useCustomAudioSessionParametersSp;
        private SerializedProperty audioSessionParametersSp;
        private SerializedProperty audioSessionPresetIndexSp;
        private SerializedProperty audioSessionParametersCategorySp;
        private SerializedProperty audioSessionParametersModeSp;
        private SerializedProperty audioSessionParametersCategoryOptionsSp;

        private string[] iOSAudioSessionPresetsNames = {"Game", "VoIP"};
        private AudioSessionParameters[] iOSAudioSessionPresetsValues =
            {AudioSessionParametersPresets.Game, AudioSessionParametersPresets.VoIP};
        #elif UNITY_ANDROID
        private SerializedProperty nativeAndroidMicrophoneSettingsSp;
        #else
        private SerializedProperty reactOnSystemChangesSp;
        private SerializedProperty skipDeviceChecksSp;
        #endif

        private void OnEnable()
        {
            this.recorder = this.target as Recorder;
            AudioSettings.OnAudioConfigurationChanged += this.OnAudioConfigChanged;
            this.RefreshMicrophones();
            this.voiceDetectionSp = this.serializedObject.FindProperty("voiceDetection");
            this.voiceDetectionThresholdSp = this.serializedObject.FindProperty("voiceDetectionThreshold");
            this.voiceDetectionDelayMsSp = this.serializedObject.FindProperty("voiceDetectionDelayMs");
            this.photonMicrophoneDeviceIdSp = this.serializedObject.FindProperty("photonMicrophoneDeviceId");
            this.interestGroupSp = this.serializedObject.FindProperty("interestGroup");
            this.debugEchoModeSp = this.serializedObject.FindProperty("debugEchoMode");
            this.reliableModeSp = this.serializedObject.FindProperty("reliableMode");
            this.encryptSp = this.serializedObject.FindProperty("encrypt");
            this.transmitEnabledSp = this.serializedObject.FindProperty("transmitEnabled");
            this.samplingRateSp = this.serializedObject.FindProperty("samplingRate");
            this.frameDurationSp = this.serializedObject.FindProperty("frameDuration");
            this.bitrateSp = this.serializedObject.FindProperty("bitrate");
            this.sourceTypeSp = this.serializedObject.FindProperty("sourceType");
            this.microphoneTypeSp = this.serializedObject.FindProperty("microphoneType");
            this.audioClipSp = this.serializedObject.FindProperty("audioClip");
            this.loopAudioClipSp = this.serializedObject.FindProperty("loopAudioClip");
            this.autoStartSp = this.serializedObject.FindProperty("autoStart");
            this.recordOnlyWhenEnabledSp = this.serializedObject.FindProperty("recordOnlyWhenEnabled");
            this.stopRecordingWhenPausedSp = this.serializedObject.FindProperty("stopRecordingWhenPaused");
            this.useMicrophoneTypeFallbackSp = this.serializedObject.FindProperty("useMicrophoneTypeFallback");
            this.recordOnlyWhenJoinedSp = this.serializedObject.FindProperty("recordOnlyWhenJoined");
            #if UNITY_IOS
            this.useCustomAudioSessionParametersSp = this.serializedObject.FindProperty("useCustomAudioSessionParameters");
            this.audioSessionPresetIndexSp = this.serializedObject.FindProperty("audioSessionPresetIndex");
            this.audioSessionParametersSp = this.serializedObject.FindProperty("audioSessionParameters");
            this.audioSessionParametersCategorySp = this.audioSessionParametersSp.FindPropertyRelative("Category");
            this.audioSessionParametersModeSp = this.audioSessionParametersSp.FindPropertyRelative("Mode");
            this.audioSessionParametersCategoryOptionsSp = this.audioSessionParametersSp.FindPropertyRelative("CategoryOptions");
            #elif UNITY_ANDROID
            this.nativeAndroidMicrophoneSettingsSp = this.serializedObject.FindProperty("nativeAndroidMicrophoneSettings");
            #else
            this.reactOnSystemChangesSp = this.serializedObject.FindProperty("reactOnSystemChanges");
            this.skipDeviceChecksSp = this.serializedObject.FindProperty("skipDeviceChangeChecks");
            #endif
        }

        private void OnDisable()
        {
            AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigChanged;
        }

        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            this.serializedObject.UpdateIfRequiredOrScript();
            //serializedObject.Update();
            WebRtcAudioDsp webRtcAudioDsp = this.recorder.GetComponent<WebRtcAudioDsp>();
            bool webRtcAudioDspAttached = webRtcAudioDsp && webRtcAudioDsp != null;
            bool webRtcAudioDspAvailable = webRtcAudioDspAttached && webRtcAudioDsp.enabled;
            AudioChangesHandler audioChangesHandler = this.recorder.GetComponent<AudioChangesHandler>();
            bool audioChangesHandlerAttached = !ReferenceEquals(null, audioChangesHandler) && audioChangesHandler;
            if (PhotonVoiceEditorUtils.IsInTheSceneInPlayMode(this.recorder.gameObject))
            {
                if (this.recorder.RequiresRestart)
                {
                    EditorGUILayout.HelpBox("Recorder requires restart. Call Recorder.RestartRecording().", MessageType.Warning);
                    if (GUILayout.Button("RestartRecording"))
                    {
                        this.recorder.RestartRecording();
                    }
                }
                else if (!this.recorder.IsInitialized)
                {
                    EditorGUILayout.HelpBox("Recorder requires initialization. Call Recorder.Init or VoiceConnection.InitRecorder.", MessageType.Warning);
                }
            }
            VoiceLogger.ExposeLogLevel(this.serializedObject, this.recorder);

            EditorGUI.BeginChangeCheck();
            if (PhotonVoiceEditorUtils.IsInTheSceneInPlayMode(this.recorder.gameObject))
            {
                if (!audioChangesHandlerAttached)
                {
                    #if !UNITY_ANDROID && !UNITY_IOS
                    this.recorder.ReactOnSystemChanges = EditorGUILayout.Toggle(new GUIContent("React On System Changes", "If true, recording is restarted when Unity detects Audio Config. changes."), this.recorder.ReactOnSystemChanges);
                    if (this.recorder.ReactOnSystemChanges)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(this.skipDeviceChecksSp, new GUIContent("Skip Device Checks", "If true, restarts recording without checking if audio config/device changes affected recording."));
                        EditorGUI.indentLevel--;
                        EditorGUILayout.HelpBox("ReactOnSystemChanges will be deprecated. AudioChangesHandler component is now the preferred solution to handle audio changes.", MessageType.Warning);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("ReactOnSystemChanges will be deprecated. AudioChangesHandler component is now the preferred solution to handle audio changes.", MessageType.Info);
                    }
                    #endif
                    if (GUILayout.Button("Add AudioChangesHandler component"))
                    {
                        this.recorder.gameObject.AddComponent<AudioChangesHandler>();
                    }
                }
                this.recorder.RecordOnlyWhenJoined = EditorGUILayout.Toggle(new GUIContent("Record Only When Joined", "If true, recording can start only when client is joined to a room. Auto start is also delayed until client is joined to a room."),
                    this.recorder.RecordOnlyWhenJoined);
                this.recorder.RecordOnlyWhenEnabled = EditorGUILayout.Toggle(new GUIContent("Record Only When Enabled", "If true, component will work only when enabled and active in hierarchy. Auto start is also delayed until these conditions are met."),
                    this.recorder.RecordOnlyWhenEnabled);
                EditorGUILayout.PropertyField(this.stopRecordingWhenPausedSp,
                    new GUIContent("Stop Recording When Paused",
                        "If true, stop recording when paused resume/restart when un-paused."));
                this.recorder.TransmitEnabled = EditorGUILayout.Toggle(new GUIContent("Transmit Enabled", "If true, audio transmission is enabled."), this.recorder.TransmitEnabled);
                if (this.recorder.IsInitialized)
                {
                    this.recorder.IsRecording = EditorGUILayout.Toggle(new GUIContent("IsRecording", "If true, audio recording is on."), this.recorder.IsRecording);
                }
                else
                {
                    this.recorder.AutoStart = EditorGUILayout.Toggle(new GUIContent("Auto Start", "If true, recording is started when Recorder is initialized."), this.recorder.AutoStart);
                }
                if (this.recorder.IsRecording && this.recorder.TransmitEnabled)
                {
                    float amplitude = 0f;
                    if (this.recorder.IsCurrentlyTransmitting)
                    {
                        amplitude = this.recorder.LevelMeter.CurrentPeakAmp;
                    }
                    EditorGUILayout.Slider("Level", amplitude, 0, 1);
                }
                this.recorder.Encrypt = EditorGUILayout.Toggle(new GUIContent("Encrypt", "If true, voice stream is sent encrypted."), this.recorder.Encrypt);
                this.recorder.InterestGroup = (byte)EditorGUILayout.IntField(new GUIContent("Interest Group", "Target interest group that will receive transmitted audio."), this.recorder.InterestGroup);
                if (this.recorder.InterestGroup == 0)
                {
                    this.recorder.DebugEchoMode = EditorGUILayout.Toggle(new GUIContent("Debug Echo", "If true, outgoing stream routed back to client via server same way as for remote client's streams."), this.recorder.DebugEchoMode);
                }
                this.recorder.ReliableMode = EditorGUILayout.Toggle(new GUIContent("Reliable Mode", "If true, stream data sent in reliable mode."), this.recorder.ReliableMode);

                EditorGUILayout.LabelField("Codec Parameters", EditorStyles.boldLabel);
                OpusCodec.FrameDuration frameDuration = (OpusCodec.FrameDuration)EditorGUILayout.EnumPopup(new GUIContent("Frame Duration", "Outgoing audio stream encoder delay."), this.recorder.FrameDuration);
                if (webRtcAudioDspAvailable)
                {
                    switch (frameDuration)
                    {
                        case OpusCodec.FrameDuration.Frame2dot5ms:
                        case OpusCodec.FrameDuration.Frame5ms:
                            if (this.recorder.FrameDuration == OpusCodec.FrameDuration.Frame10ms)
                            {
                                Debug.LogWarningFormat("Frame duration requested ({0}ms) is not supported by WebRTC Audio DSP. Ignoring change.", frameDuration);
                            }
                            else
                            {
                                Debug.LogWarningFormat("Frame duration requested ({0}ms) is not supported by WebRTC Audio DSP (it needs to be N x 10ms), switching to the closest supported value: 10ms.", (int)frameDuration / 1000);
                            }
                            frameDuration = OpusCodec.FrameDuration.Frame10ms;
                            break;
                    }
                }
                SamplingRate samplingRate = (SamplingRate)EditorGUILayout.EnumPopup(new GUIContent("Sampling Rate", "Outgoing audio stream sampling rate."), this.recorder.SamplingRate);
                if (webRtcAudioDspAvailable)
                {
                    int samplingRateInt = (int) samplingRate;
                    switch (samplingRate)
                    {
                        case SamplingRate.Sampling12000:
                            if (this.recorder.SamplingRate == SamplingRate.Sampling16000)
                            {
                                Debug.LogWarningFormat("Sampling rate requested ({0}Hz) is not supported by WebRTC Audio DSP. Ignoring change.", samplingRateInt);
                            }
                            else
                            {
                                Debug.LogWarningFormat("Sampling rate requested ({0}Hz) is not supported by WebRTC Audio DSP, switching to the closest supported value: {1}Hz.", samplingRateInt, "16k");
                            }
                            samplingRate = SamplingRate.Sampling16000;
                            break;
                        case SamplingRate.Sampling24000:
                            if (this.recorder.SamplingRate == SamplingRate.Sampling48000)
                            {
                                Debug.LogWarningFormat("Sampling rate requested ({0}Hz) is not supported by WebRTC Audio DSP. Ignoring change.", samplingRateInt);
                            }
                            else
                            {
                                Debug.LogWarningFormat("Sampling rate requested ({0}Hz) is not supported by WebRTC Audio DSP, switching to the closest supported value: {1}Hz.", samplingRateInt, "48k");
                            }
                            samplingRate = SamplingRate.Sampling48000;
                            break;
                    }
                }
                this.recorder.SamplingRate = samplingRate;
                this.recorder.FrameDuration = frameDuration;
                this.recorder.Bitrate = EditorGUILayout.IntSlider(new GUIContent("Bitrate", "Outgoing audio stream bitrate."), this.recorder.Bitrate, Recorder.MIN_OPUS_BITRATE, Recorder.MAX_OPUS_BITRATE);
                EditorGUILayout.LabelField("Audio Source Settings", EditorStyles.boldLabel);
                this.recorder.SourceType = (Recorder.InputSourceType) EditorGUILayout.EnumPopup(new GUIContent("Input Source Type", "Input audio data source type"), this.recorder.SourceType);
                switch (this.recorder.SourceType)
                {
                    case Recorder.InputSourceType.Microphone:
                        this.recorder.MicrophoneType = (Recorder.MicType) EditorGUILayout.EnumPopup(
                            new GUIContent("Microphone Type",
                                "Which microphone API to use when the Source is set to UnityMicrophone."),
                            this.recorder.MicrophoneType);
                        EditorGUILayout.PropertyField(this.useMicrophoneTypeFallbackSp, new GUIContent("Use Fallback", "If true, if recording fails to start with Unity microphone type, Photon microphone type is used -if available- as a fallback and vice versa."));
                        switch (this.recorder.MicrophoneType)
                        {
                            case Recorder.MicType.Unity:
                                if (UnityMicrophone.devices.Length == 0)
                                {
                                    EditorGUILayout.HelpBox("No microphone device found", MessageType.Error);
                                }
                                else
                                {
                                    EditorGUILayout.HelpBox("Devices list and current selection is valid in Unity Editor only. In build, you need to set it via code preferably at runtime.", MessageType.Info);
                                    this.unityMicrophoneDeviceIndex = EditorGUILayout.Popup("Microphone Device", this.GetUnityMicrophoneDeviceIndex(), UnityMicrophone.devices);
                                    this.recorder.UnityMicrophoneDevice = UnityMicrophone.devices[this.unityMicrophoneDeviceIndex];
                                    int minFreq, maxFreq;
                                    UnityMicrophone.GetDeviceCaps(UnityMicrophone.devices[this.unityMicrophoneDeviceIndex], out minFreq, out maxFreq);
                                    EditorGUILayout.LabelField("Microphone Device Caps", string.Format("{0}..{1} Hz", minFreq, maxFreq));
                                }
                                break;
                            case Recorder.MicType.Photon:
                                #if PHOTON_MICROPHONE_ENUMERATOR
                                if (this.recorder.MicrophonesEnumerator.IsSupported)
                                {
                                    if (!this.recorder.MicrophonesEnumerator.Any())
                                    {
                                        EditorGUILayout.HelpBox("No microphone device found", MessageType.Error);
                                    }
                                    else
                                    {
                                        EditorGUILayout.HelpBox("Devices list and current selection is valid in Unity Editor only. In build, you need to set it via code preferably at runtime.", MessageType.Info);
                                        EditorGUILayout.BeginHorizontal();
                                        if (this.photonDeviceIndex >= this.photonDeviceNames.Length)
                                        {
                                            Debug.LogWarningFormat("Unexpected photonDeviceIndex = {0} >= photonDeviceNames.Length = {1}, forcing refresh.", this.photonDeviceIndex, this.photonDeviceNames.Length);
                                            this.RefreshMicrophones();
                                        }
                                        this.photonDeviceIndex = EditorGUILayout.Popup("Microphone Device", this.photonDeviceIndex, this.photonDeviceNames);
                                        this.recorder.PhotonMicrophoneDeviceId = this.photonDeviceIDs[this.photonDeviceIndex];
                                        if (GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(70)))
                                        {
                                            this.RefreshPhotonMicrophoneDevices();
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                                else
                                {
                                    this.recorder.PhotonMicrophoneDeviceId = -1;
                                    EditorGUILayout.HelpBox("PhotonMicrophoneEnumerator Not Supported", MessageType.Error);
                                }
                                #endif
                                #if UNITY_IOS
                                EditorGUILayout.LabelField("iOS Audio Session Parameters", EditorStyles.boldLabel);
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(this.useCustomAudioSessionParametersSp, new GUIContent("Use Custom"));
                                if (this.useCustomAudioSessionParametersSp.boolValue)
                                {
                                    EditorGUILayout.PropertyField(this.audioSessionParametersCategorySp);
                                    EditorGUILayout.PropertyField(this.audioSessionParametersModeSp);
                                    EditorGUILayout.PropertyField(this.audioSessionParametersCategoryOptionsSp, true);
                                }
                                else
                                {
                                    int index = EditorGUILayout.Popup("Preset", this.audioSessionPresetIndexSp.intValue, this.iOSAudioSessionPresetsNames);
                                    if (index != this.audioSessionPresetIndexSp.intValue)
                                    {
                                        this.audioSessionPresetIndexSp.intValue = index;
                                        AudioSessionParameters parameters = this.iOSAudioSessionPresetsValues[index];
                                        this.SetEnumIndex(this.audioSessionParametersCategorySp,
                                            typeof(AudioSessionCategory), parameters.Category);
                                        this.SetEnumIndex(this.audioSessionParametersModeSp,
                                            typeof(AudioSessionMode), parameters.Mode);
                                        if (parameters.CategoryOptions != null)
                                        {
                                            this.audioSessionParametersCategoryOptionsSp.ClearArray();
                                            this.audioSessionParametersCategoryOptionsSp.arraySize =
                                                parameters.CategoryOptions.Length;
                                            if (index == 0)
                                            {
                                                this.SetEnumIndex(this.audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(0), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.DefaultToSpeaker);
                                                this.SetEnumIndex(this.audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(1), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.AllowBluetooth);
                                            }
                                            else if (index == 1)
                                            {
                                                this.SetEnumIndex(this.audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(0), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.AllowBluetooth);

                                            }
                                        }
                                    }
                                }
                                EditorGUI.indentLevel--;
                                #elif UNITY_ANDROID
                                EditorGUILayout.LabelField("Android Native Microphone Settings", EditorStyles.boldLabel);
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(this.nativeAndroidMicrophoneSettingsSp.FindPropertyRelative("AcousticEchoCancellation"));
                                EditorGUILayout.PropertyField(this.nativeAndroidMicrophoneSettingsSp.FindPropertyRelative("AutomaticGainControl"));
                                EditorGUILayout.PropertyField(this.nativeAndroidMicrophoneSettingsSp.FindPropertyRelative("NoiseSuppression"));
                                EditorGUI.indentLevel--;
                                #endif
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case Recorder.InputSourceType.AudioClip:
                        this.recorder.AudioClip = EditorGUILayout.ObjectField(new GUIContent("Audio Clip", "Source audio clip."), this.recorder.AudioClip, typeof(AudioClip), false) as AudioClip;
                        this.recorder.LoopAudioClip =
                            EditorGUILayout.Toggle(new GUIContent("Loop", "Loop playback for audio clip sources."),
                                this.recorder.LoopAudioClip);
                        break;
                    case Recorder.InputSourceType.Factory:
                        EditorGUILayout.HelpBox("Add a custom InputFactory method in code.", MessageType.Info);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                EditorGUILayout.LabelField("Voice Activity Detection (VAD)", EditorStyles.boldLabel);
                if (webRtcAudioDspAvailable)
                {
                    if (webRtcAudioDsp.VAD)
                    {
                        EditorGUILayout.HelpBox("WebRtcAudioDsp.VAD is already enabled no need to use the built-in Recorder VAD", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("It's recommended to use VAD from WebRtcAudioDsp instead of built-in Recorder VAD", MessageType.Info);
                    }
                }
                this.recorder.VoiceDetection = EditorGUILayout.Toggle(new GUIContent("Detect", "If true, voice detection enabled."), this.recorder.VoiceDetection);
                if (this.recorder.VoiceDetection)
                {
                    if (webRtcAudioDspAvailable && !webRtcAudioDsp.VAD && GUILayout.Button("Use WebRtcAudioDsp.VAD instead"))
                    {
                        this.recorder.VoiceDetection = false;
                        webRtcAudioDsp.VAD = true;
                    }
                    this.recorder.VoiceDetectionThreshold =
                        EditorGUILayout.Slider(
                            new GUIContent("Threshold", "Voice detection threshold (0..1, where 1 is full amplitude)."),
                            this.recorder.VoiceDetectionThreshold, 0f, 1f);
                    this.recorder.VoiceDetectionDelayMs =
                        EditorGUILayout.IntField(new GUIContent("Delay (ms)", "Keep detected state during this time after signal level dropped below threshold. Default is 500ms"), this.recorder.VoiceDetectionDelayMs);
                    EditorGUILayout.HelpBox("Do not speak and stay in a silent environment when calibrating.", MessageType.Info);
                    if (this.recorder.VoiceDetectorCalibrating)
                    {
                        EditorGUILayout.LabelField(string.Format("Calibrating {0} ms", this.calibrationTime));
                    }
                    else
                    {
                        this.calibrationTime = EditorGUILayout.IntField("Calibration Time (ms)", this.calibrationTime);
                        if (this.recorder.IsRecording && this.recorder.TransmitEnabled)
                        {
                            if (GUILayout.Button("Calibrate"))
                            {
                                this.recorder.VoiceDetectorCalibrate(this.calibrationTime);
                            }
                        }
                    }
                }
            }
            else
            {
                if (!audioChangesHandlerAttached)
                {
                    #if !UNITY_ANDROID && !UNITY_IOS
                    EditorGUILayout.PropertyField(this.reactOnSystemChangesSp,
                        new GUIContent("React On System Changes",
                            "If true, recording is restarted when Unity detects Audio Config. changes."));
                    if (this.reactOnSystemChangesSp.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(this.skipDeviceChecksSp, new GUIContent("Skip Device Checks", "If true, restarts recording without checking if audio config/device changes affected recording."));
                        EditorGUI.indentLevel--;
                        EditorGUILayout.HelpBox("ReactOnSystemChanges will be deprecated. AudioChangesHandler component is now the preferred solution to handle audio changes.", MessageType.Warning);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("ReactOnSystemChanges will be deprecated. AudioChangesHandler component is now the preferred solution to handle audio changes.", MessageType.Info);
                    }
                    #endif
                    if (GUILayout.Button("Add AudioChangesHandler component"))
                    {
                        this.recorder.gameObject.AddComponent<AudioChangesHandler>();
                    }
                }
                EditorGUILayout.PropertyField(this.recordOnlyWhenEnabledSp,
                    new GUIContent("Record Only When Enabled",
                        "If true, component will work only when enabled and active in hierarchy."));
                EditorGUILayout.PropertyField(this.recordOnlyWhenJoinedSp,
                    new GUIContent("Record Only When Joined",
                        "If true, recording can start only when client is joined to a room. Auto start is also delayed until client is joined to a room."));
                EditorGUILayout.PropertyField(this.stopRecordingWhenPausedSp,
                    new GUIContent("Stop Recording When Paused",
                        "If true, stop recording when paused resume/restart when un-paused."));
                EditorGUILayout.PropertyField(this.transmitEnabledSp,
                    new GUIContent("Transmit Enabled", "If true, audio transmission is enabled."));
                EditorGUILayout.PropertyField(this.autoStartSp,
                    new GUIContent("Auto Start", "If true, recording is started when Recorder is initialized."));
                EditorGUILayout.PropertyField(this.encryptSp,
                    new GUIContent("Encrypt", "If true, voice stream is sent encrypted."));
                EditorGUILayout.PropertyField(this.interestGroupSp,
                    new GUIContent("Interest Group", "Target interest group that will receive transmitted audio."));
                if (this.interestGroupSp.intValue == 0)
                {
                    EditorGUILayout.PropertyField(this.debugEchoModeSp,
                        new GUIContent("Debug Echo",
                            "If true, outgoing stream routed back to client via server same way as for remote client's streams."));
                }
                else if (this.debugEchoModeSp.boolValue)
                {
                    Debug.LogWarningFormat("DebugEchoMode disabled because InterestGroup changed to {0}. DebugEchoMode works only with Interest Group 0.", this.interestGroupSp.intValue);
                    this.debugEchoModeSp.boolValue = false;
                }
                EditorGUILayout.PropertyField(this.reliableModeSp, new GUIContent("Reliable Mode",
                        "If true, stream data sent in reliable mode."));

                EditorGUILayout.LabelField("Codec Parameters", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(this.frameDurationSp,
                    new GUIContent("Frame Duration", "Outgoing audio stream encoder delay."));
                if (webRtcAudioDspAvailable)
                {
                    OpusCodec.FrameDuration frameDuration = (OpusCodec.FrameDuration)Enum.GetValues(typeof(OpusCodec.FrameDuration)).GetValue(this.frameDurationSp.enumValueIndex);
                    switch (frameDuration)
                    {
                        case OpusCodec.FrameDuration.Frame2dot5ms:
                        case OpusCodec.FrameDuration.Frame5ms:
                            string warningMessage = string.Format("Frame duration requested ({0}ms) is not supported by WebRTC Audio DSP (it needs to be N x 10ms), switching to the closest supported value: 10ms.", (int)frameDuration / 1000);
                            EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
                            break;
                    }
                }
                EditorGUILayout.PropertyField(this.samplingRateSp,
                    new GUIContent("Sampling Rate", "Outgoing audio stream sampling rate."));
                if (webRtcAudioDspAvailable)
                {
                    SamplingRate samplingRate = (SamplingRate)Enum.GetValues(typeof(SamplingRate)).GetValue(this.samplingRateSp.enumValueIndex);
                    switch (samplingRate)
                    {
                        case SamplingRate.Sampling12000:
                            string warningMessage = "Sampling rate requested (12kHz) is not supported by WebRTC Audio DSP. When recording starts, this will be automatically switched to the closest supported value: 16kHz.";
                            EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
                            break;
                        case SamplingRate.Sampling24000:
                            warningMessage = "Sampling rate requested (24kHz) is not supported by WebRTC Audio DSP. When recording starts, this will be automatically switched to the closest supported value: 48kHz.";
                            EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
                            break;
                    }
                }
                EditorGUILayout.PropertyField(this.bitrateSp,
                    new GUIContent("Bitrate", "Outgoing audio stream bitrate."));
                EditorGUILayout.LabelField("Audio Source Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(this.sourceTypeSp,
                    new GUIContent("Input Source Type", "Input audio data source type"));
                switch ((Recorder.InputSourceType)this.sourceTypeSp.enumValueIndex)
                {
                    case Recorder.InputSourceType.Microphone:
                        EditorGUILayout.PropertyField(this.microphoneTypeSp, new GUIContent("Microphone Type",
                            "Which microphone API to use when the Source is set to UnityMicrophone."));
                        EditorGUILayout.PropertyField(this.useMicrophoneTypeFallbackSp, new GUIContent("Use Fallback", "If true, if recording fails to start with Unity microphone type, Photon microphone type is used -if available- as a fallback and vice versa."));
                        switch (this.recorder.MicrophoneType)
                        {
                            case Recorder.MicType.Unity:
                                break;
                            case Recorder.MicType.Photon:
                                #if PHOTON_MICROPHONE_ENUMERATOR
                                if (this.recorder.MicrophonesEnumerator.IsSupported)
                                {
                                    if (!this.recorder.MicrophonesEnumerator.Any())
                                    {
                                        EditorGUILayout.HelpBox("No microphone device found", MessageType.Error);
                                    }
                                    else
                                    {
                                        EditorGUILayout.HelpBox("Devices list and current selection is valid in Unity Editor only. In build, you need to set it via code preferably at runtime.", MessageType.Info);
                                        EditorGUILayout.BeginHorizontal();
                                        if (this.photonDeviceIndex >= this.photonDeviceNames.Length)
                                        {
                                            Debug.LogWarningFormat("Unexpected photonDeviceIndex = {0} >= photonDeviceNames.Length = {1}, forcing refresh.", this.photonDeviceIndex, this.photonDeviceNames.Length);
                                            this.RefreshMicrophones();
                                        }
                                        this.photonDeviceIndex = EditorGUILayout.Popup("Microphone Device", this.photonDeviceIndex, this.photonDeviceNames);
                                        this.photonMicrophoneDeviceIdSp.intValue = this.photonDeviceIDs[this.photonDeviceIndex];
                                        if (GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(70)))
                                        {
                                            this.RefreshPhotonMicrophoneDevices();
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                                else
                                {
                                    this.photonMicrophoneDeviceIdSp.intValue = -1;
                                    EditorGUILayout.HelpBox("PhotonMicrophoneEnumerator Not Supported", MessageType.Error);
                                }
                                #endif
                                #if UNITY_IOS
                                EditorGUILayout.LabelField("iOS Audio Session Parameters", EditorStyles.boldLabel);
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(this.useCustomAudioSessionParametersSp, new GUIContent("Use Custom"));
                                if (this.useCustomAudioSessionParametersSp.boolValue)
                                {
                                    EditorGUILayout.PropertyField(this.audioSessionParametersCategorySp);
                                    EditorGUILayout.PropertyField(this.audioSessionParametersModeSp);
                                    EditorGUILayout.PropertyField(this.audioSessionParametersCategoryOptionsSp, true);
                                }
                                else
                                {
                                    int index = EditorGUILayout.Popup("Preset", this.audioSessionPresetIndexSp.intValue, this.iOSAudioSessionPresetsNames);
                                    if (index != this.audioSessionPresetIndexSp.intValue)
                                    {
                                        this.audioSessionPresetIndexSp.intValue = index;
                                        AudioSessionParameters parameters = this.iOSAudioSessionPresetsValues[index];
                                        this.SetEnumIndex(this.audioSessionParametersCategorySp,
                                            typeof(AudioSessionCategory), parameters.Category);
                                        this.SetEnumIndex(this.audioSessionParametersModeSp,
                                            typeof(AudioSessionMode), parameters.Mode);
                                        if (parameters.CategoryOptions != null)
                                        {
                                            this.audioSessionParametersCategoryOptionsSp.ClearArray();
                                            this.audioSessionParametersCategoryOptionsSp.arraySize =
                                                parameters.CategoryOptions.Length;
                                            if (index == 0)
                                            {
                                                this.SetEnumIndex(this.audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(0), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.DefaultToSpeaker);
                                                this.SetEnumIndex(this.audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(1), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.AllowBluetooth);
                                            }
                                            else if (index == 1)
                                            {
                                                this.SetEnumIndex(this.audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(0), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.AllowBluetooth);

                                            }
                                        }
                                    }
                                }
                                EditorGUI.indentLevel--;
                                #elif UNITY_ANDROID
                                EditorGUILayout.LabelField("Android Native Microphone Settings", EditorStyles.boldLabel);
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(this.nativeAndroidMicrophoneSettingsSp.FindPropertyRelative("AcousticEchoCancellation"));
                                EditorGUILayout.PropertyField(this.nativeAndroidMicrophoneSettingsSp.FindPropertyRelative("AutomaticGainControl"));
                                EditorGUILayout.PropertyField(this.nativeAndroidMicrophoneSettingsSp.FindPropertyRelative("NoiseSuppression"));
                                EditorGUI.indentLevel--;
                                #endif
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case Recorder.InputSourceType.AudioClip:
                        EditorGUILayout.PropertyField(this.audioClipSp,
                            new GUIContent("Audio Clip", "Source audio clip."));
                        EditorGUILayout.PropertyField(this.loopAudioClipSp,
                            new GUIContent("Loop", "Loop playback for audio clip sources."));
                        break;
                    case Recorder.InputSourceType.Factory:
                        EditorGUILayout.HelpBox("Add a custom InputFactory method in code.", MessageType.Info);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                EditorGUILayout.LabelField("Voice Activity Detection (VAD)", EditorStyles.boldLabel);
                if (webRtcAudioDspAvailable)
                {
                    if (webRtcAudioDsp.VAD)
                    {
                        EditorGUILayout.HelpBox("WebRtcAudioDsp.VAD is already enabled no need to use the built-in Recorder VAD.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("It's recommended to use VAD from WebRtcAudioDsp instead of built-in Recorder VAD.", MessageType.Info);
                    }
                }
                EditorGUILayout.PropertyField(this.voiceDetectionSp,
                    new GUIContent("Detect", "If true, voice detection enabled."));
                if (this.voiceDetectionSp.boolValue)
                {
                    if (webRtcAudioDspAvailable && !webRtcAudioDsp.VAD && GUILayout.Button("Use WebRtcAudioDsp.VAD instead"))
                    {
                        this.recorder.VoiceDetection = false;
                        webRtcAudioDsp.VAD = true;
                    }
                    this.voiceDetectionThresholdSp.floatValue = EditorGUILayout.Slider(
                            new GUIContent("Threshold", "Voice detection threshold (0..1, where 1 is full amplitude)."),
                            this.voiceDetectionThresholdSp.floatValue, 0f, 1f);
                    this.voiceDetectionDelayMsSp.intValue =
                        EditorGUILayout.IntField(new GUIContent("Delay (ms)", "Keep detected state during this time after signal level dropped below threshold. Default is 500ms"), this.voiceDetectionDelayMsSp.intValue);
                }
            }

            #if WEBRTC_AUDIO_DSP_SUPPORTED_PLATFORMS
            if (!webRtcAudioDspAvailable)
            {
                if (webRtcAudioDspAttached)
                {
                    if (GUILayout.Button("Enabled WebRtcAudioDsp component"))
                    {
                        webRtcAudioDsp.enabled = true;
                    }
                }
                else if (GUILayout.Button("Add WebRtcAudioDsp component"))
                {
                    this.recorder.gameObject.AddComponent<WebRtcAudioDsp>();
                }
            }
            #endif

            if (EditorGUI.EndChangeCheck())
            {
                this.serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnAudioConfigChanged(bool deviceWasChanged)
        {
            if (deviceWasChanged)
            {
                this.RefreshMicrophones();
            }
        }

        private void RefreshMicrophones()
        {
            if (UnityMicrophone.devices.Length == 0)
            {
                this.unityMicrophoneDeviceIndex = 0;
            }
            else
            {
                this.unityMicrophoneDeviceIndex = Mathf.Clamp(ArrayUtility.IndexOf(UnityMicrophone.devices, this.recorder.UnityMicrophoneDevice), 0, UnityMicrophone.devices.Length - 1);
            }
            #if PHOTON_MICROPHONE_ENUMERATOR
            this.RefreshPhotonMicrophoneDevices();
            #endif
        }

        #if PHOTON_MICROPHONE_ENUMERATOR
        private void RefreshPhotonMicrophoneDevices()
        {
            if (this.recorder.MicrophonesEnumerator.IsSupported)
            {
                this.recorder.MicrophonesEnumerator.Refresh();
                int count = this.recorder.MicrophonesEnumerator.Count();
                if (count == 0)
                {
                    this.recorder.PhotonMicrophoneDeviceId = -1;
                    this.photonDeviceNames = null;
                    this.photonDeviceIDs = null;
                    this.photonDeviceIndex = 0;
                }
                else
                {
                    this.photonDeviceNames = new string[count];
                    this.photonDeviceIDs = new int[count];
                    int i = 0;
                    foreach (DeviceInfo deviceInfo in this.recorder.MicrophonesEnumerator)
                    {
                        this.photonDeviceIDs[i] = deviceInfo.IDInt;
                        this.photonDeviceNames[i] = string.Format("{0} - {1} [{2}]", i, deviceInfo.Name.Replace('/', '_'), deviceInfo.IDInt);
                        i++;
                    }
                    this.photonDeviceIndex = Mathf.Clamp(Array.IndexOf(this.photonDeviceIDs, this.recorder.PhotonMicrophoneDeviceId), 0, count - 1);
                    this.recorder.PhotonMicrophoneDeviceId = this.photonDeviceIDs[this.photonDeviceIndex];
                }
            }
            else
            {
                this.recorder.PhotonMicrophoneDeviceId = -1;
            }
            
        }
        #endif

        #if UNITY_IOS
        private void SetEnumIndex(SerializedProperty property, Type enumType, object enumValue)
        {
            string enumName = Enum.GetName(enumType, enumValue);
            int index = Array.IndexOf(property.enumNames, enumName);
            if (index >= 0)
            {
                property.enumValueIndex = index;
            }
        }
        #endif

        private int GetUnityMicrophoneDeviceIndex()
        {
            if (this.unityMicrophoneDeviceIndex == 0 && !Recorder.IsDefaultUnityMic(this.recorder.UnityMicrophoneDevice) || 
                this.unityMicrophoneDeviceIndex > 0 && this.unityMicrophoneDeviceIndex < UnityMicrophone.devices.Length && 
                !Recorder.CompareUnityMicNames(UnityMicrophone.devices[this.unityMicrophoneDeviceIndex], this.recorder.UnityMicrophoneDevice))
            {
                int newIndex = Array.IndexOf(UnityMicrophone.devices, this.recorder.UnityMicrophoneDevice);
                if (newIndex >= 0)
                {
                    return newIndex;
                }
            }
            return this.unityMicrophoneDeviceIndex;
        }
    }
}