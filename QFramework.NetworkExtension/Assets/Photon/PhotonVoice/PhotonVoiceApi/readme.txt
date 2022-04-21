
This is the readme for the Photon Voice API.
(C) Exit Games GmbH 2015


Overview
----------------------------------------------------------------------------------------------------
Photon Voice API extends Photon LoadBalancing API with audio streaming functionality.
Each client can broadcast multiple audio streams to other clients.

Using the Photon Voice API
----------------------------------------------------------------------------------------------------
- Use Voice.Client to follow standard LoadBalancing connection workflow: connect to the Photon cloud with Realtime application id and join the room.
- Sending audio. Invoke Client.CreateLocalVoice with audio source and desired audio parameters to create new streaming source (voice). 
  Returning value is new local voice. Set Transmit = true for the voice to start broadcasting audio. 
  Audio source is an object which feeds Photon Voice client with audio data via simple IAudioStream interface.
  Multiple local voices can be added. To remove local voice, call Client.RemoveLocalVoice with local voice as parameter.
- Receiving audio. Each remote voice uniquely identified by playerId and voiceId. To be able playback remote voices, client must provide several handlers:
  - OnRemoteVoiceInfoAction - called each time new remote voice created or new client with existing voices joins the room.
    Set up new audio source to playback data streamed by new voice in this handler.
  - OnRemoteVoiceRemoveAction - called each time remote voice removed by client or client leaves or disconnects.
  - OnAudioFrameAction - audio data frame from remote voice received and decompressed. Route the data to the speaker according to playerId and voiceId.
