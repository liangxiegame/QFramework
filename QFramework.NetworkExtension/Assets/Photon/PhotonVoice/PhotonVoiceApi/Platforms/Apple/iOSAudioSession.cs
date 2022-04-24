namespace Photon.Voice.IOS
{
    public enum AudioSessionCategory // values are the same as in AudioIn.mm enums
    {
        /// <summary>
        /// Use this category for background sounds such as rain, car engine noise, etc.  
        /// Mixes with other music.
        /// </summary>
        /// <remarks>API_AVAILABLE(ios(3.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);</remarks>
        Ambient = 0,

        /// <summary> Use this category for background sounds.  Other music will stop playing. </summary>
        /// <remarks>API_AVAILABLE(ios(3.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);</remarks>
        SoloAmbient = 1, 

        /// <summary> Use this category for music tracks. </summary>
        /// <remarks>API_AVAILABLE(ios(3.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);</remarks>
        Playback = 2, 

        /// <summary> Use this category when recording audio. </summary>
        /// <remarks>API_AVAILABLE(ios(3.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);</remarks>
        Record = 3,

        /// <summary> Use this category when recording and playing back audio. </summary>
        /// <remarks>API_AVAILABLE(ios(3.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);</remarks>
        PlayAndRecord = 4,

        /// <summary> Use this category when using a hardware codec or signal processor while
        /// not playing or recording audio. </summary>
        /// <remarks>API_DEPRECATED("No longer supported", ios(3.0, 10.0)) API_UNAVAILABLE(watchos, tvos) API_UNAVAILABLE(macos);</remarks>
        AudioProcessing = 5,

        /// <summary> Use this category to customize the usage of available audio accessories and built-in audio hardware.
        ///  For example, this category provides an application with the ability to use an available USB output 
        ///  and headphone output simultaneously for separate, distinct streams of audio data. Use of 
        ///  this category by an application requires a more detailed knowledge of, and interaction with, 
        ///  the capabilities of the available audio routes.  May be used for input, output, or both.
        ///  Note that not all output types and output combinations are eligible for multi-route.  Input is limited
        ///  to the last-in input port. Eligible inputs consist of the following:
        ///     AVAudioSessionPortUSBAudio, AVAudioSessionPortHeadsetMic, and AVAudioSessionPortBuiltInMic.  
        ///  Eligible outputs consist of the following: 
        ///     AVAudioSessionPortUSBAudio, AVAudioSessionPortLineOut, AVAudioSessionPortHeadphones, AVAudioSessionPortHDMI, 
        ///    and AVAudioSessionPortBuiltInSpeaker.  
        ///  Note that AVAudioSessionPortBuiltInSpeaker is only allowed to be used when there are no other eligible 
        ///  outputs connected. </summary>
        /// <remarks>API_AVAILABLE(ios(6.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);</remarks>
        MultiRoute = 6, 
    }

    public enum AudioSessionMode // values are the same as in AudioIn.mm enums
    {
        /// <summary>
        /// Modes modify the audio category in order to introduce behavior that is tailored to the specific
        /// use of audio within an application.  Available in iOS 5.0 and greater.
        /// </summary>
        /// <remarks>
        /// The default mode
        /// API_AVAILABLE(ios(5.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);
        /// </remarks>
        Default = 0,

        /// <summary>
        /// Only valid with AVAudioSessionCategoryPlayAndRecord.  Appropriate for Voice over IP
        /// (VoIP) applications.  Reduces the number of allowable audio routes to be only those
        /// that are appropriate for VoIP applications and may engage appropriate system-supplied
        /// signal processing.  Has the side effect of setting AVAudioSessionCategoryOptionAllowBluetooth
        /// </summary>
        /// <remarks>
        /// API_AVAILABLE(ios(5.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);
        /// </remarks>
        VoiceChat = 1,

        /* Set by Game Kit on behalf of an application that uses a GKVoiceChat object; valid
         only with the AVAudioSessionCategoryPlayAndRecord category.
         Do not set this mode directly. If you need similar behavior and are not using
         a GKVoiceChat object, use AVAudioSessionModeVoiceChat instead. */
//        GameChat = 2, // API_AVAILABLE(ios(5.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);

        /// <summary>
        /// Only valid with AVAudioSessionCategoryPlayAndRecord or AVAudioSessionCategoryRecord.
        /// Modifies the audio routing options and may engage appropriate system-supplied signal processing.
        /// </summary>
        /// <remarks>
        /// API_AVAILABLE(ios(5.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);
        /// </remarks>
        VideoRecording = 3, 

        /// <summary>
        /// Appropriate for applications that wish to minimize the effect of system-supplied signal
        /// processing for input and/or output audio signals.
        /// </summary>
        /// <remarks>
        /// API_AVAILABLE(ios(5.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);
        /// </remarks>
        Measurement = 4,

        /// <summary>
        /// Engages appropriate output signal processing for movie playback scenarios.  Currently
        /// only applied during playback over built-in speaker.
        /// </summary>
        /// <remarks>
        /// API_AVAILABLE(ios(6.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);
        /// </remarks>
        MoviePlayback = 5,

        /// <summary>
        /// Only valid with kAudioSessionCategory_PlayAndRecord. Reduces the number of allowable audio
        /// routes to be only those that are appropriate for video chat applications. May engage appropriate
        /// system-supplied signal processing.  Has the side effect of setting
        /// AVAudioSessionCategoryOptionAllowBluetooth and AVAudioSessionCategoryOptionDefaultToSpeaker.
        /// </summary>
        /// <remarks>
        /// API_AVAILABLE(ios(7.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);
        /// </remarks>
        VideoChat = 6,

        /* Appropriate for applications which play spoken audio and wish to be paused (via audio session interruption) rather than ducked
        if another app (such as a navigation app) plays a spoken audio prompt.  Examples of apps that would use this are podcast players and
        audio books.  For more information, see the related category option AVAudioSessionCategoryOptionInterruptSpokenAudioAndMixWithOthers. */
//        SpokenAudio = 7, // API_AVAILABLE(ios(9.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos);

        /* Appropriate for applications which play audio using text to speech. Setting this mode allows for different routing behaviors when
        connected to certain audio devices such as CarPlay. An example of an app that would use this mode is a turn by turn navigation app that
        plays short prompts to the user. Typically, these same types of applications would also configure their session to use
        AVAudioSessionCategoryOptionDuckOthers and AVAudioSessionCategoryOptionInterruptSpokenAudioAndMixWithOthers */
//        VoicePrompt = 8, // API_AVAILABLE(ios(12.0), watchos(5.0), tvos(12.0)) API_UNAVAILABLE(macos);
    }

    public enum AudioSessionCategoryOption // values as defined in Apple Audio Session API
    {
        /*
        AVAudioSessionCategoryOptionMixWithOthers -- 
            This allows an application to set whether or not other active audio apps will be interrupted or mixed with
            when your app's audio session goes active. The typical cases are:
             (1) AVAudioSessionCategoryPlayAndRecord or AVAudioSessionCategoryMultiRoute
                 this will default to false, but can be set to true. This would allow other applications to play in the background
                 while an app had both audio input and output enabled
             (2) AVAudioSessionCategoryPlayback
                 this will default to false, but can be set to true. This would allow other applications to play in the background,
                 but an app will still be able to play regardless of the setting of the ringer switch
             (3) Other categories
                 this defaults to false and cannot be changed (that is, the mix with others setting of these categories
                 cannot be overridden. An application must be prepared for setting this property to fail as behaviour 
                 may change in future releases. If an application changes their category, they should reassert the 
                 option (it is not sticky across category changes).

        AVAudioSessionCategoryOptionDuckOthers -- 
            This allows an application to set whether or not other active audio apps will be ducked when when your app's audio
            session goes active. An example of this is the Nike app, which provides periodic updates to its user (it reduces the
            volume of any music currently being played while it provides its status). This defaults to off. Note that the other
            audio will be ducked for as long as the current session is active. You will need to deactivate your audio
            session when you want full volume playback of the other audio. 
            If your category is AVAudioSessionCategoryPlayback, AVAudioSessionCategoryPlayAndRecord, or 
            AVAudioSessionCategoryMultiRoute, by default the audio session will be non-mixable and non-ducking. 
            Setting this option will also make your category mixable with others (AVAudioSessionCategoryOptionMixWithOthers
            will be set).

        AVAudioSessionCategoryOptionAllowBluetooth --
            This allows an application to change the default behaviour of some audio session categories with regards to showing
            bluetooth Hands-Free Profile (HFP) devices as available routes. The current category behavior is:
             (1) AVAudioSessionCategoryPlayAndRecord
                 this will default to false, but can be set to true. This will allow a paired bluetooth HFP device to show up as
                 an available route for input, while playing through the category-appropriate output
             (2) AVAudioSessionCategoryRecord
                 this will default to false, but can be set to true. This will allow a paired bluetooth HFP device to show up
                 as an available route for input
             (3) Other categories
                 this defaults to false and cannot be changed (that is, enabling bluetooth for input in these categories is
                 not allowed)
                 An application must be prepared for setting this option to fail as behaviour may change in future releases.
                 If an application changes their category or mode, they should reassert the override (it is not sticky
                 across category and mode changes).

        AVAudioSessionCategoryOptionDefaultToSpeaker --
            This allows an application to change the default behaviour of some audio session categories with regards to
            the audio route. The current category behavior is:
             (1) AVAudioSessionCategoryPlayAndRecord category
                 this will default to false, but can be set to true. this will route to Speaker (instead of Receiver)
                 when no other audio route is connected.
             (2) Other categories
                 this defaults to false and cannot be changed (that is, the default to speaker setting of these
                 categories cannot be overridden
                 An application must be prepared for setting this property to fail as behaviour may change in future releases.
                 If an application changes their category, they should reassert the override (it is not sticky across
                 category and mode changes). 

        AVAudioSessionCategoryOptionInterruptSpokenAudioAndMixWithOthers --
            If another app's audio session mode is set to AVAudioSessionModeSpokenAudio (podcast playback in the background for example),
            then that other app's audio will be interrupted when the current application's audio session goes active. An example of this
            is a navigation app that provides navigation prompts to its user (it pauses any spoken audio currently being played while it
            plays the prompt). This defaults to off. Note that the other app's audio will be paused for as long as the current session is
            active. You will need to deactivate your audio session to allow the other audio to resume playback.
            Setting this option will also make your category mixable with others (AVAudioSessionCategoryOptionMixWithOthers
            will be set).  If you want other non-spoken audio apps to duck their audio when your app's session goes active, also set
            AVAudioSessionCategoryOptionDuckOthers.

        AVAudioSessionCategoryOptionAllowBluetoothA2DP --
            This allows an application to change the default behaviour of some audio session categories with regards to showing
            bluetooth Advanced Audio Distribution Profile (A2DP), i.e. stereo Bluetooth, devices as available routes. The current 
            category behavior is:
            (1) AVAudioSessionCategoryPlayAndRecord
            this will default to false, but can be set to true. This will allow a paired bluetooth A2DP device to show up as
            an available route for output, while recording through the category-appropriate input
            (2) AVAudioSessionCategoryMultiRoute and AVAudioSessionCategoryRecord
            this will default to false, and cannot be set to true.
            (3) Other categories
            this defaults to true and cannot be changed (that is, bluetooth A2DP ports are always supported in output-only categories).
            An application must be prepared for setting this option to fail as behaviour may change in future releases.
            If an application changes their category or mode, they should reassert the override (it is not sticky
            across category and mode changes).
            Setting both AVAudioSessionCategoryOptionAllowBluetooth and AVAudioSessionCategoryOptionAllowBluetoothA2DP is allowed. In cases
            where a single Bluetooth device supports both HFP and A2DP, the HFP ports will be given a higher priority for routing. For HFP 
            and A2DP ports on separate hardware devices, the last-in wins rule applies.

         AVAudioSessionCategoryOptionAllowAirPlay --
            This allows an application to change the default behaviour of some audio session categories with regards to showing
            AirPlay devices as available routes. See the documentation of AVAudioSessionCategoryOptionAllowBluetoothA2DP for details on 
            how this option applies to specific categories.
         */

        /// <summary>
        /// This allows an application to set whether or not other active audio apps will be interrupted or mixed with
        /// when your app's audio session goes active. The typical cases are:
        /// (1) AVAudioSessionCategoryPlayAndRecord or AVAudioSessionCategoryMultiRoute
        /// this will default to false, but can be set to true. This would allow other applications to play in the background
        /// while an app had both audio input and output enabled
        /// (2) AVAudioSessionCategoryPlayback
        /// this will default to false, but can be set to true. This would allow other applications to play in the background,
        /// but an app will still be able to play regardless of the setting of the ringer switch
        /// (3) Other categories
        /// this defaults to false and cannot be changed (that is, the mix with others setting of these categories
        /// cannot be overridden. An application must be prepared for setting this property to fail as behaviour 
        /// may change in future releases. If an application changes their category, they should reassert the 
        /// option (it is not sticky across category changes).
        /// MixWithOthers is only valid with AVAudioSessionCategoryPlayAndRecord, AVAudioSessionCategoryPlayback, and  AVAudioSessionCategoryMultiRoute
        /// </summary>
        MixWithOthers = 0x1,
        /// <summary>
        /// This allows an application to set whether or not other active audio apps will be ducked when when your app's audio
        /// session goes active. An example of this is the Nike app, which provides periodic updates to its user (it reduces the
        /// volume of any music currently being played while it provides its status). This defaults to off. Note that the other
        /// audio will be ducked for as long as the current session is active. You will need to deactivate your audio
        /// session when you want full volume playback of the other audio. 
        /// If your category is AVAudioSessionCategoryPlayback, AVAudioSessionCategoryPlayAndRecord, or 
        /// AVAudioSessionCategoryMultiRoute, by default the audio session will be non-mixable and non-ducking. 
        /// Setting this option will also make your category mixable with others (AVAudioSessionCategoryOptionMixWithOthers
        /// will be set).
        /// DuckOthers is only valid with AVAudioSessionCategoryAmbient, AVAudioSessionCategoryPlayAndRecord, AVAudioSessionCategoryPlayback, and AVAudioSessionCategoryMultiRoute
        /// </summary>
        DuckOthers = 0x2,
        /// <summary>
        /// This allows an application to change the default behaviour of some audio session categories with regards to showing
        /// bluetooth Hands-Free Profile (HFP) devices as available routes. The current category behavior is:
        /// (1) AVAudioSessionCategoryPlayAndRecord
        /// this will default to false, but can be set to true. This will allow a paired bluetooth HFP device to show up as
        /// an available route for input, while playing through the category-appropriate output
        /// (2) AVAudioSessionCategoryRecord
        /// this will default to false, but can be set to true. This will allow a paired bluetooth HFP device to show up
        /// as an available route for input
        /// (3) Other categories
        /// this defaults to false and cannot be changed (that is, enabling bluetooth for input in these categories is
        /// not allowed)
        /// An application must be prepared for setting this option to fail as behaviour may change in future releases.
        /// If an application changes their category or mode, they should reassert the override (it is not sticky
        /// across category and mode changes).
        /// AllowBluetooth is only valid with AVAudioSessionCategoryRecord and AVAudioSessionCategoryPlayAndRecord
        /// </summary>
        AllowBluetooth = 0x4, // API_UNAVAILABLE(tvos, watchos, macos)
        /// <summary>
        /// This allows an application to change the default behaviour of some audio session categories with regards to
        /// the audio route. The current category behavior is:
        /// (1) AVAudioSessionCategoryPlayAndRecord category
        /// this will default to false, but can be set to true. this will route to Speaker (instead of Receiver)
        /// when no other audio route is connected.
        /// (2) Other categories
        /// this defaults to false and cannot be changed (that is, the default to speaker setting of these
        /// categories cannot be overridden
        /// An application must be prepared for setting this property to fail as behaviour may change in future releases.
        /// If an application changes their category, they should reassert the override (it is not sticky across
        /// category and mode changes). 
        /// DefaultToSpeaker is only valid with AVAudioSessionCategoryPlayAndRecord
        /// </summary>
        DefaultToSpeaker = 0x8, // API_UNAVAILABLE(tvos, watchos, macos)
        /* InterruptSpokenAudioAndMixWithOthers is only valid with AVAudioSessionCategoryPlayAndRecord, AVAudioSessionCategoryPlayback, and AVAudioSessionCategoryMultiRoute */
//        InterruptSpokenAudioAndMixWithOthers = 0x11, // API_AVAILABLE(ios(9.0), watchos(2.0), tvos(9.0)) API_UNAVAILABLE(macos)
        /* AllowBluetoothA2DP is only valid with AVAudioSessionCategoryPlayAndRecord */
//        AllowBluetoothA2DP = 0x20, // API_AVAILABLE(ios(10.0), watchos(3.0), tvos(10.0)) API_UNAVAILABLE(macos)
        /* AllowAirPlay is only valid with AVAudioSessionCategoryPlayAndRecord */
//        AllowAirPlay = 0x40, // API_AVAILABLE(ios(10.0), tvos(10.0)) API_UNAVAILABLE(watchos, macos)
    }

    [System.Serializable]
    public struct AudioSessionParameters
    {
        public AudioSessionCategory Category;//= AudioSessionCategory.PlayAndRecord;
        public AudioSessionMode Mode;// = AudioSessionMode.Default;
        public AudioSessionCategoryOption[] CategoryOptions;
        public int CategoryOptionsToInt()
        {
            int opt = 0;
            if (CategoryOptions != null)
            {
                for (int i = 0; i < CategoryOptions.Length; i++)
                {
                    opt |= (int)CategoryOptions[i];
                }
            }
            return opt;
        }
		public override string ToString()
		{
            var opt = "[";
            if (CategoryOptions != null)
            {
                for (int i = 0; i < CategoryOptions.Length; i++)
                {
                    opt += CategoryOptions[i];
                    if (i != CategoryOptions.Length - 1)
                    {
                        opt += ", ";
                    }
                }
            }
            opt += "]";
            return string.Format("category = {0}, mode = {1}, options = {2}", Category, Mode, opt);
		}
	}

    public static class AudioSessionParametersPresets
    {
        public static AudioSessionParameters Game = new AudioSessionParameters()
        {
            Category = AudioSessionCategory.PlayAndRecord,
            Mode = AudioSessionMode.Default,
            CategoryOptions = new AudioSessionCategoryOption[] { AudioSessionCategoryOption.DefaultToSpeaker, AudioSessionCategoryOption.AllowBluetooth }
        };
        public static AudioSessionParameters VoIP = new AudioSessionParameters()
        {
            Category = AudioSessionCategory.PlayAndRecord,
            Mode = AudioSessionMode.VoiceChat,
            // VoiceChat should have the side effect of setting AVAudioSessionCategoryOptionAllowBluetooth according to doc 
            // but tests don't confirm this
            CategoryOptions = new AudioSessionCategoryOption[] { AudioSessionCategoryOption.AllowBluetooth }
       };
    }
}
