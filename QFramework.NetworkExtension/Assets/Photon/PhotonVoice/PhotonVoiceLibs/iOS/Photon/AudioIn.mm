// Photon_Audio_In works in 2 modes.
// 1. Fills buffer provided in ..._Read call. Takes minimum cpu cycles in render callback. But requires ring buffer adding some latency
// 2. Pushes audio data via callback as soon as data is available. Minimal latency and ability to use "push" Photon Voice interface which is more efficient.
// Push mode enabled if CallbackData.pushCallback property set.

#import <AudioToolbox/AudioToolbox.h>
#import <AVFoundation/AVFoundation.h>
#import "AudioIn.h"

#define SAMPLE_RATE 48000

#define XThrowIfError(error, operation)                                                 \
do {                                                                                    \
if (error) {                                                                            \
throw [NSException                                                                      \
exceptionWithName:@"PhotonAudioException"                                               \
reason:[NSString stringWithFormat:@"%s (%i)", operation, (int)error] userInfo:nullptr]; \
}                                                                                       \
} while (0)

const int BUFFER_SIZE = 4096000;
static NSMutableSet* handles = [[NSMutableSet alloc] init];

struct CallbackData {
    AudioUnit rioUnit;
    BOOL audioChainIsBeingReconstructed;
    float* ringBuffer;
    int ringWritePos;
    int ringReadPos;
    
    int pushHostID;
    Photon_IOSAudio_PushCallback pushCallback;
    
    CallbackData(): rioUnit(NULL), audioChainIsBeingReconstructed(false),
    ringBuffer(NULL), ringWritePos(0), ringReadPos(0), pushHostID(0), pushCallback(NULL) {}
    
    ~CallbackData() {
        free(ringBuffer);
    }
};

@interface Photon_Audio_In() {
@public
    CallbackData cd;
    AVAudioSessionCategory sessionCategory;
    AVAudioSessionMode sessionMode;
    AVAudioSessionCategoryOptions sessionCategoryOptions;
}
- (void)setupHandlers;
- (void)storeCategory:(int)category mode:(int)mode options:(int)options;
- (void)setSessionCategory;
- (void)setupAudioSession;
- (void)setupIOUnit;
- (void)setupAudioChain; // just calls 2 methods above
@end


Photon_Audio_In* Photon_Audio_In_CreateReader(int sessionCategory, int sessionMode, int sessionCategoryOptions) {
    Photon_Audio_In* handle = [[Photon_Audio_In alloc] init];
    [handle storeCategory:sessionCategory mode:sessionMode options:sessionCategoryOptions];
    handle->cd.ringBuffer = (float*)malloc(sizeof(float)*BUFFER_SIZE);
    @synchronized(handles) {
        [handles addObject:handle];
    }
    [handle setupHandlers];
    
    [handle setupAudioChain];
    [handle startIOUnit];
    return handle;
}

bool Photon_Audio_In_Read(Photon_Audio_In* handle, float* buf, int len) {
    CallbackData& cd = handle->cd;
    if (cd.ringReadPos + len > cd.ringWritePos) {
        return false;
    }
    if (cd.ringReadPos + BUFFER_SIZE < cd.ringWritePos) {
        cd.ringReadPos = cd.ringWritePos - BUFFER_SIZE;
    }
    
    int pos = cd.ringReadPos % BUFFER_SIZE;
    if (pos + len > BUFFER_SIZE) {
        int remains = BUFFER_SIZE - pos;
        memcpy(buf, cd.ringBuffer + pos, remains * sizeof(float));
        memcpy(buf + remains, cd.ringBuffer, (len - remains) * sizeof(float));
    } else {
        memcpy(buf, cd.ringBuffer + pos, len * sizeof(float));
    }
    cd.ringReadPos += len;
    
    // tone test
    //    static int tonePos = 0;
    //    for(int i = 0;i < len;i++) buf[i] = sin((tonePos++) / 16.0f)/4;
    
    return true;
}

Photon_Audio_In* Photon_Audio_In_CreatePusher(int hostID, Photon_IOSAudio_PushCallback callback, int sessionCategory, int sessionMode, int sessionCategoryOptions) {
    Photon_Audio_In* handle = [[Photon_Audio_In alloc] init];
    [handle storeCategory:sessionCategory mode:sessionMode options:sessionCategoryOptions];
    handle->cd.pushCallback = callback;
    handle->cd.pushHostID = hostID;
    @synchronized(handles) {
        [handles addObject:handle];
    }
    [handle setupHandlers];
    
    [handle setupAudioChain];
    [handle startIOUnit];
    return handle;
}

void Photon_Audio_In_Reset(Photon_Audio_In* handle) {
    NSLog(@"[PV] [AI] Reset");
    [handle stopIOUnit];
    [handle setupAudioChain];
    [handle startIOUnit];
}

void Photon_Audio_In_Destroy(Photon_Audio_In* handle) {
    [handle stopIOUnit];
    // remove reference to the handle in the same queue as used for push callback to make sure that all pending buffers processed before handle destroyed
    dispatch_async(dispatch_get_main_queue(), ^{
        @synchronized(handles) {
            [handles removeObject:handle];
        }
    });
}

// Render callback function
static OSStatus    performRender (void                         *inRefCon,
                                  AudioUnitRenderActionFlags     *ioActionFlags,
                                  const AudioTimeStamp         *inTimeStamp,
                                  UInt32                         inBusNumber,
                                  UInt32                         inNumberFrames,
                                  AudioBufferList              *ioData)
{
    OSStatus err = noErr;
    CallbackData& cd = *((CallbackData*)inRefCon);
    if (cd.audioChainIsBeingReconstructed == NO)
    {
        // we are calling AudioUnitRender on the input bus of AURemoteIO
        // this will store the audio data captured by the microphone in ioData
        err = AudioUnitRender(cd.rioUnit, ioActionFlags, inTimeStamp, 1, inNumberFrames, ioData);
        
        if (!err) {
            if (cd.pushCallback) {
                NSData *data = [[NSData alloc] initWithBytes:ioData->mBuffers[0].mData length:ioData->mBuffers[0].mDataByteSize];
                dispatch_async(dispatch_get_main_queue(), ^{
                    cd.pushCallback(cd.pushHostID, (float*)data.bytes, (int)data.length/sizeof(float));
                });
            } else {
                int pos = cd.ringWritePos % BUFFER_SIZE;
                if (pos + inNumberFrames > BUFFER_SIZE) {
                    int remains = BUFFER_SIZE - pos;
                    memcpy(cd.ringBuffer + pos, (float*)ioData->mBuffers[0].mData, remains * sizeof(float));
                    memcpy(cd.ringBuffer, (float*)ioData->mBuffers[0].mData + remains, (inNumberFrames - remains) * sizeof(float));
                } else {
                    memcpy(cd.ringBuffer + pos, (float*)ioData->mBuffers[0].mData, inNumberFrames * sizeof(float));
                }
                // tone test
                //for(int i = 0;i < inNumberFrames;i++) cd.ringBuffer[(cd.ringWritePos + i) % BUFFER_SIZE] = sin((cd.ringWritePos + i) / 16.0f)/4;
                cd.ringWritePos += inNumberFrames;
            }
        }
        
        // mute output buffer
        for (UInt32 i=0; i<ioData->mNumberBuffers; ++i)
            memset(ioData->mBuffers[i].mData, 0, ioData->mBuffers[i].mDataByteSize);
        
    }
    
    return err;
}

@implementation Photon_Audio_In

- (void)storeCategory:(int)category mode:(int)mode options:(int)options {
    switch (category)
    {
        case 0: self->sessionCategory = AVAudioSessionCategoryAmbient; break;
        case 1: self->sessionCategory = AVAudioSessionCategorySoloAmbient; break;
        case 2: self->sessionCategory = AVAudioSessionCategoryPlayback; break;
        case 3: self->sessionCategory = AVAudioSessionCategoryRecord; break;
        case 4: self->sessionCategory = AVAudioSessionCategoryPlayAndRecord; break;
        case 5: self->sessionCategory = AVAudioSessionCategoryAudioProcessing; break;
        case 6: self->sessionCategory = AVAudioSessionCategoryMultiRoute; break;
        default: throw [NSException exceptionWithName:@"PhotonAudioException" reason:[NSString stringWithFormat:@"Unknown session category %d", category] userInfo:nullptr];
    }
    
    switch (mode)
    {
        case 0: self->sessionMode = AVAudioSessionModeDefault; break;
        case 1: self->sessionMode = AVAudioSessionModeVoiceChat; break;
        case 2: self->sessionMode = AVAudioSessionModeGameChat; break;
        case 3: self->sessionMode = AVAudioSessionModeVideoRecording; break;
        case 4: self->sessionMode = AVAudioSessionModeMeasurement; break;
        case 5: self->sessionMode = AVAudioSessionModeMoviePlayback; break;
        case 6: self->sessionMode = AVAudioSessionModeVideoChat; break;
        case 7: self->sessionMode = AVAudioSessionModeSpokenAudio; break;
            //       case 8: self->sessionMode = AVAudioSessionModeVoicePrompt; break;
        default: throw [NSException exceptionWithName:@"PhotonAudioException" reason:[NSString stringWithFormat:@"Unknown session mode %d", mode] userInfo:nullptr];
    }
    self->sessionCategoryOptions = options;
}

- (void)handleInterruption:(NSNotification *)notification
{
    try {
        UInt8 theInterruptionType = [[notification.userInfo valueForKey:AVAudioSessionInterruptionTypeKey] intValue];
        NSLog(@"[PV] [AI] Session interrupted > --- %s ---\n", theInterruptionType == AVAudioSessionInterruptionTypeBegan ? "Begin Interruption" : "End Interruption");
        
        if (theInterruptionType == AVAudioSessionInterruptionTypeBegan) {
            // do not stop recording            
        }
        
        if (theInterruptionType == AVAudioSessionInterruptionTypeEnded) {
            // reset
            [self stopIOUnit];
            [self setupAudioChain];
            [self startIOUnit];
        }
    } catch (NSException* e) {
        NSLog(@"[PV] [AI] Error: %@\n", e);
    }
}


- (void)handleRouteChange:(NSNotification *)notification
{
    UInt8 reasonValue = [[notification.userInfo valueForKey:AVAudioSessionRouteChangeReasonKey] intValue];
//    AVAudioSessionRouteDescription *routeDescription = [notification.userInfo valueForKey:AVAudioSessionRouteChangePreviousRouteKey];
    
    bool deviceChange = false;
    switch (reasonValue) {
        case AVAudioSessionRouteChangeReasonNewDeviceAvailable:
            deviceChange = true;
            NSLog(@"[PV] [AI] Route change: NewDeviceAvailable");
            break;
        case AVAudioSessionRouteChangeReasonOldDeviceUnavailable:
            deviceChange = true;
            NSLog(@"[PV] [AI] Route change: OldDeviceUnavailable");
            break;
        case AVAudioSessionRouteChangeReasonCategoryChange:
            NSLog(@"[PV] [AI] Route change: CategoryChange: %@", [[AVAudioSession sharedInstance] category]);
            break;
        case AVAudioSessionRouteChangeReasonOverride:
            NSLog(@"[PV] [AI] Route change: Override");
            break;
        case AVAudioSessionRouteChangeReasonWakeFromSleep:
            NSLog(@"[PV] [AI] Route change: WakeFromSleep");
            break;
        case AVAudioSessionRouteChangeReasonNoSuitableRouteForCategory:
            NSLog(@"[PV] [AI] Route change: NoSuitableRouteForCategory");
            break;
        default:
            NSLog(@"[PV] [AI] Route change: ReasonUnknown");
    }
    
//    NSLog(@"[PV] [AI] Previous route:\n");
//    NSLog(@"[PV] [AI] %@\n", routeDescription);
//    NSLog(@"[PV] [AI] Current route:\n");
//    NSLog(@"[PV] [AI] %@\n", [AVAudioSession sharedInstance].currentRoute);
    
    if (deviceChange) {
        // reset
        [self stopIOUnit];
        [self setupAudioChain];
        [self startIOUnit];
    }
}

// https://developer.apple.com/documentation/avfaudio/avaudiosessionmediaserviceswereresetnotification
- (void)handleMediaServerReset:(NSNotification *)notification
{
    NSLog(@"[PV] [AI] Media server has reset");
    cd.audioChainIsBeingReconstructed = YES;
    
    usleep(25000); //wait here for some time to ensure that we don't delete these objects while they are being accessed elsewhere
    
    [self setSessionCategory];

    cd.audioChainIsBeingReconstructed = NO;
}

- (void)setupHandlers
{
    AVAudioSession *sessionInstance = [AVAudioSession sharedInstance];
    // add interruption handler
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(handleInterruption:)
                                                 name:AVAudioSessionInterruptionNotification
                                               object:sessionInstance];
    
    // we don't do anything special in the route change notification
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(handleRouteChange:)
                                                 name:AVAudioSessionRouteChangeNotification
                                               object:sessionInstance];
    
    // if media services are reset, we need to rebuild our audio chain
    [[NSNotificationCenter defaultCenter]    addObserver:self
                                                selector:@selector(handleMediaServerReset:)
                                                    name:    AVAudioSessionMediaServicesWereResetNotification
                                                  object:sessionInstance];
}

- (void) setSessionCategory
{
    NSLog(@"[PV] [AI] setSessionCategory");
    AVAudioSession *sessionInstance = [AVAudioSession sharedInstance];
    
    NSError *error = nil;
	NSLog(@"[PV] [AI] Current category = %@, mode = %@, options = %lu", sessionInstance.category, sessionInstance.mode, (unsigned long)sessionInstance.categoryOptions);
	NSLog(@"[PV] [AI] Setting category = %@, mode = %@, options = %lu", self->sessionCategory, self->sessionMode, (unsigned long)self->sessionCategoryOptions);
    [sessionInstance setCategory:self->sessionCategory
                            mode:self->sessionMode
                         options:self->sessionCategoryOptions
                           error:&error];
     XThrowIfError((OSStatus)error.code, "couldn't set session's audio category");
}

- (void)setupAudioSession
{
    NSLog(@"[PV] [AI] setupAudioSession");
    try {
        // Configure the audio session
        [self setSessionCategory];
        AVAudioSession *sessionInstance = [AVAudioSession sharedInstance];
        
        NSError *error = nil;
        // set the buffer duration to 5 ms
        NSTimeInterval bufferDuration = .005;
        [sessionInstance setPreferredIOBufferDuration:bufferDuration error:&error];
        XThrowIfError((OSStatus)error.code, "couldn't set session's I/O buffer duration");
        
        // set the session's sample rate
        //        [sessionInstance setPreferredSampleRate:44100 error:&error];
        //        XThrowIfError((OSStatus)error.code, "couldn't set session's preferred sample rate");
        
        // activate the audio session
        [[AVAudioSession sharedInstance] setActive:YES error:&error];
        XThrowIfError((OSStatus)error.code, "couldn't set session active");
    }
    
    catch (NSException* e) {
        NSLog(@"[PV] [AI] Error returned from setupAudioSession: %@", e);
    }
    catch (...) {
        NSLog(@"[PV] [AI] Unknown error returned from setupAudioSession");
    }
    
    NSLog(@"[PV] [AI] AudioSession successfully set up.");
    return;
}


- (void)setupIOUnit
{
    NSLog(@"[PV] [AI] setupIOUnit");
    try {
        // Create a new instance of AURemoteIO
        
        AudioComponentDescription desc;
        desc.componentType = kAudioUnitType_Output;
        desc.componentSubType = kAudioUnitSubType_VoiceProcessingIO;
        desc.componentManufacturer = kAudioUnitManufacturer_Apple;
        desc.componentFlags = 0;
        desc.componentFlagsMask = 0;
        
        AudioComponent comp = AudioComponentFindNext(NULL, &desc);
        XThrowIfError(AudioComponentInstanceNew(comp, &cd.rioUnit), "couldn't create a new instance of AURemoteIO");
        
        //  Enable input and output on AURemoteIO
        //  Input is enabled on the input scope of the input element
        //  Output is enabled on the output scope of the output element
        
        UInt32 one = 1;
        XThrowIfError(AudioUnitSetProperty(cd.rioUnit, kAudioOutputUnitProperty_EnableIO, kAudioUnitScope_Input, 1, &one, sizeof(one)), "could not enable input on AURemoteIO");
        XThrowIfError(AudioUnitSetProperty(cd.rioUnit, kAudioOutputUnitProperty_EnableIO, kAudioUnitScope_Output, 0, &one, sizeof(one)), "could not enable output on AURemoteIO");
        
        // Explicitly set the input and output client formats
        
        int sampleRate = SAMPLE_RATE;
        int channels = 1;
        AudioStreamBasicDescription ioFormat;
        ioFormat.mSampleRate = sampleRate;
        ioFormat.mFormatID = kAudioFormatLinearPCM;
        ioFormat.mFormatFlags = kAudioFormatFlagsNativeEndian | kAudioFormatFlagIsPacked | kAudioFormatFlagIsFloat;
        ioFormat.mFramesPerPacket = 1;
        ioFormat.mChannelsPerFrame = channels;
        ioFormat.mBytesPerFrame = ioFormat.mBytesPerPacket = sizeof(float) * channels;
        ioFormat.mBitsPerChannel = sizeof(float) * 8;
        ioFormat.mReserved = 0;
        
        XThrowIfError(AudioUnitSetProperty(cd.rioUnit, kAudioUnitProperty_StreamFormat, kAudioUnitScope_Input, 0, &ioFormat, sizeof(ioFormat)), "couldn't set input stream format AURemoteIO");
        
        XThrowIfError(AudioUnitSetProperty(cd.rioUnit, kAudioUnitProperty_StreamFormat, kAudioUnitScope_Output, 1, &ioFormat, sizeof(ioFormat)), "couldn't set output stream format AURemoteIO");
        
        // Set the MaximumFramesPerSlice property. This property is used to describe to an audio unit the maximum number
        // of samples it will be asked to produce on any single given call to AudioUnitRender
        UInt32 maxFramesPerSlice = 4096;
        XThrowIfError(AudioUnitSetProperty(cd.rioUnit, kAudioUnitProperty_MaximumFramesPerSlice, kAudioUnitScope_Global, 0, &maxFramesPerSlice, sizeof(UInt32)), "couldn't set max frames per slice on AURemoteIO");
        
        // Get the property value back from AURemoteIO. We are going to use this value to allocate buffers accordingly
        UInt32 propSize = sizeof(UInt32);
        XThrowIfError(AudioUnitGetProperty(cd.rioUnit, kAudioUnitProperty_MaximumFramesPerSlice, kAudioUnitScope_Global, 0, &maxFramesPerSlice, &propSize), "couldn't get max frames per slice on AURemoteIO");
        
        // We need references to certain data in the render callback
        // This simple struct is used to hold that information
        
        // Set the render callback on AURemoteIO
        AURenderCallbackStruct renderCallback;
        renderCallback.inputProc = performRender;
        renderCallback.inputProcRefCon = &cd;
        XThrowIfError(AudioUnitSetProperty(cd.rioUnit, kAudioUnitProperty_SetRenderCallback, kAudioUnitScope_Input, 0, &renderCallback, sizeof(renderCallback)), "couldn't set render callback on AURemoteIO");
        
        // Initialize the AURemoteIO instance
        
        // from https://chromium.googlesource.com/external/webrtc/+/9eeb6240c93efe2219d4d6f4cf706030e00f64d7/webrtc/modules/audio_device/ios/voice_processing_audio_unit.mm
        // Calls to AudioUnitInitialize() can fail if called back-to-back on
        // different ADM instances. The error message in this case is -66635 which is
        // undocumented. Tests have shown that calling AudioUnitInitialize a second
        // time, after a short sleep, avoids this issue.
        // See webrtc:5166 for details.
        
        int attempts = 5;
        int result = !noErr;
        while (true) {
            result = AudioUnitInitialize(cd.rioUnit);
            if (result == noErr)
                break;
            attempts--;
            if (attempts == 0) {
                XThrowIfError(result, "couldn't initialize AURemoteIO instance, too many attempts");
            } else {
                NSLog(@"[PV] [AI] Failed to initialize the Voice Processing I/O unit. "
                           "Error=%ld. Retrying in 100ms...",
                          (long)result);
            }
          [NSThread sleepForTimeInterval:0.1f];
        }
        NSLog(@"[PV] [AI] Voice Processing I/O unit successfully initialized.");
    }
    
    catch (NSException* e) {
        NSLog(@"[PV] [AI] Error returned from setupIOUnit: %@", e);
    }
    catch (...) {
        NSLog(@"[PV] [AI] Unknown error returned from setupIOUnit");
    }
    
    return;
}

- (void)setupAudioChain
{
    NSLog(@"[PV] [AI] setupAudioChain");
    [self setupAudioSession];
    [self setupIOUnit];
}

- (OSStatus)startIOUnit
{
    NSLog(@"[PV] [AI] startIOUnit");
    OSStatus err = AudioOutputUnitStart(cd.rioUnit);
    if (err) NSLog(@"[PV] [AI] couldn't start AURemoteIO: %d", (int)err);
    else NSLog(@"[PV] [AI] AURemoteIO successfully started.");
    return err;
}

- (OSStatus)stopIOUnit
{
    NSLog(@"[PV] [AI] stopIOUnit");
    OSStatus err = AudioOutputUnitStop(cd.rioUnit);
    if (err) NSLog(@"[PV] [AI] couldn't stop AURemoteIO: %d", (int)err);
    else NSLog(@"[PV] [AI] AURemoteIO successfully stopped.");

	AVAudioSession *sessionInstance = [AVAudioSession sharedInstance];
	NSError *error = nil;
	NSLog(@"[PV] [AI] Current category = %@, mode = %@, options = %lu", sessionInstance.category, sessionInstance.mode, (unsigned long)sessionInstance.categoryOptions);
	[sessionInstance setCategory:AVAudioSessionCategoryAmbient
							mode:AVAudioSessionModeDefault
						 options:AVAudioSessionCategoryOptionMixWithOthers
						   error:&error];
	NSLog(@"[PV] [AI] Reset to default category = %@, mode = %@, options = %lu", sessionInstance.category, sessionInstance.mode, (unsigned long)sessionInstance.categoryOptions);

    return err;
}

- (double)sessionSampleRate
{
    return [[AVAudioSession sharedInstance] sampleRate];
}

- (BOOL)audioChainIsBeingReconstructed
{
    return cd.audioChainIsBeingReconstructed;
}

@end
