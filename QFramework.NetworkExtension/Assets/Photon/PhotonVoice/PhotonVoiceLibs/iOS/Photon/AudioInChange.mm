#import <AVFoundation/AVAudioSession.h>
#import "AudioInChange.h"

static NSMutableSet* handles = [[NSMutableSet alloc] init];

@interface Photon_Audio_Change() {
@public
    int hostID;
    Photon_IOSAudio_ChangeCallback callback;
}
@end

Photon_Audio_Change* Photon_Audio_In_CreateChangeNotifier(int hostID, Photon_IOSAudio_ChangeCallback callback) {
    Photon_Audio_Change* handle = [[Photon_Audio_Change alloc] init];
    handle->callback = callback;
    handle->hostID = hostID;
    @synchronized(handles) {
        [handles addObject:handle];
    }
    [[NSNotificationCenter defaultCenter] addObserver:handle
                                               selector:@selector(handleRouteChange:)
                                                   name:AVAudioSessionRouteChangeNotification
                                                 object:nil];
    return handle;
}

void Photon_Audio_In_DestroyChangeNotifier(Photon_Audio_Change* handle) {
    @synchronized(handles) {
        [handles removeObject:handle];
    }
}

@implementation Photon_Audio_Change

- (void)notify
{
    dispatch_async(dispatch_get_main_queue(), ^{
        self->callback(self->hostID);
    });
}

- (void)handleRouteChange:(NSNotification *)notification
{
    UInt8 reasonValue = [[notification.userInfo valueForKey:AVAudioSessionRouteChangeReasonKey] intValue];

    switch (reasonValue) {
        case AVAudioSessionRouteChangeReasonNewDeviceAvailable:
            NSLog(@"[PV] [AC] Route change: NewDeviceAvailable");
            [self notify];
            break;
        case AVAudioSessionRouteChangeReasonOldDeviceUnavailable:
            NSLog(@"[PV] [AC] Route change: OldDeviceUnavailable");
            [self notify];
            break;
        case AVAudioSessionRouteChangeReasonCategoryChange:
            NSLog(@"[PV] [AC] Route change: CategoryChange: %@", [[AVAudioSession sharedInstance] category]);
            break;
        case AVAudioSessionRouteChangeReasonOverride:
            NSLog(@"[PV] [AC] Route change: Override");
            break;
        case AVAudioSessionRouteChangeReasonWakeFromSleep:
            NSLog(@"[PV] [AC] Route change: WakeFromSleep");
            break;
        case AVAudioSessionRouteChangeReasonNoSuitableRouteForCategory:
            NSLog(@"[PV] [AC] Route change: NoSuitableRouteForCategory");
            break;
        default:
            NSLog(@"[PV] [AC] Route change: ReasonUnknown: %d", reasonValue);
    }
}

@end
