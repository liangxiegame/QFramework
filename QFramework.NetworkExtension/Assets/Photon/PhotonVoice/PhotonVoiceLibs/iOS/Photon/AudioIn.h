extern "C" {
    typedef void (*Photon_IOSAudio_PushCallback)(int, float*, int);
}

@interface Photon_Audio_In : NSObject {
}

@property (nonatomic, assign, readonly) BOOL audioChainIsBeingReconstructed;

- (OSStatus)    startIOUnit;
- (OSStatus)    stopIOUnit;
- (double)      sessionSampleRate;

@end

extern "C" {
    Photon_Audio_In* Photon_Audio_In_CreateReader(int sessionCategory, int sessionMode, int sessionCategoryOptions);
    bool Photon_Audio_In_Read(Photon_Audio_In* handle, float* buf, int len);
    
    Photon_Audio_In* Photon_Audio_In_CreatePusher(int hostID, Photon_IOSAudio_PushCallback pushCallback,  int sessionCategory, int sessionMode, int sessionCategoryOptions);
    
	void Photon_Audio_In_Reset(Photon_Audio_In* handle);

    void Photon_Audio_In_Destroy(Photon_Audio_In* handle);
}
