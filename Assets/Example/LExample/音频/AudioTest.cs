using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class AudioTest : MonoBehaviour {

    private void Start()
    {
        ResMgr.Init();

        AudioManager.Instance.SendMsg(new AudioSoundMsg("TestSound"));

        AudioManager.Instance.SendMsg(new AudioMusicMsg("BackGroundMusic"));

        AudioManager.Instance.SendMsg(new AudioStopMusicMsg());
    }

}
