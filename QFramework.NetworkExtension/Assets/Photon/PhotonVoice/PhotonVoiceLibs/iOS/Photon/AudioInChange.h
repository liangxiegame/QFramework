extern "C" {
    typedef void (*Photon_IOSAudio_ChangeCallback)(int);
}

@interface Photon_Audio_Change : NSObject {
}


@end

extern "C" {
    Photon_Audio_Change* Photon_Audio_In_CreateChangeNotifier(int hostID, Photon_IOSAudio_ChangeCallback changeCallback);
    
    void Photon_Audio_In_DestroyChangeNotifier(Photon_Audio_Change* handle);
}
