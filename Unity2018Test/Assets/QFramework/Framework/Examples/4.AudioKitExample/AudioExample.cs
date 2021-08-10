using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class AudioExample : MonoBehaviour
    {
        private void Awake()
        {
            var btnPlayHome = transform.Find("BtnPlayHome").GetComponent<Button>();
            var btnPlayGame = transform.Find("BtnPlayGame").GetComponent<Button>();
            var btnPlaySound = transform.Find("BtnPlaySoundClick").GetComponent<Button>();


            var btnPlayVoiceA = transform.Find("BtnPlayVoice").GetComponent<Button>();


            var btnSoundOn = transform.Find("BtnSoundOn").GetComponent<Button>();
            var btnSoundOff = transform.Find("BtnSoundOff").GetComponent<Button>();
            var btnMusicOn = transform.Find("BtnMusicOn").GetComponent<Button>();
            var btnMusicOff = transform.Find("BtnMusicOff").GetComponent<Button>();
            var btnVoiceOn = transform.Find("BtnVoiceOn").GetComponent<Button>();
            var btnVoiceOff = transform.Find("BtnVoiceOff").GetComponent<Button>();

            var musicVolumeSlider = transform.Find("MusicVolume").GetComponent<Slider>();
            var voiceVolumeSlider = transform.Find("VoiceVolume").GetComponent<Slider>();
            var soundVolumeSlider = transform.Find("SoundVolume").GetComponent<Slider>();

            btnPlayHome.onClick.AddListener(() => { AudioKit.PlayMusic("resources://home_bg"); });


            btnPlayGame.onClick.AddListener(() => { AudioKit.PlayMusic("resources://game_bg"); });

            btnPlaySound.onClick.AddListener(() => { AudioKit.PlaySound("resources://game_bg"); });

            btnPlayVoiceA.onClick.AddListener(() => { AudioKit.PlayVoice("resources://game_bg"); });

            btnSoundOn.onClick.AddListener(() => { AudioKit.Settings.IsSoundOn.Value = true; });

            btnSoundOff.onClick.AddListener(() => { AudioKit.Settings.IsSoundOn.Value = false; });

            btnMusicOn.onClick.AddListener(() => { AudioKit.Settings.IsMusicOn.Value = true; });

            btnMusicOff.onClick.AddListener(() => { AudioKit.Settings.IsMusicOn.Value = false; });

            btnVoiceOn.onClick.AddListener(() => { AudioKit.Settings.IsVoiceOn.Value = true; });

            btnVoiceOff.onClick.AddListener(() => { AudioKit.Settings.IsVoiceOn.Value = false; });

            AudioKit.Settings.MusicVolume.BindWithInitialValue(v => musicVolumeSlider.value = v);
            AudioKit.Settings.VoiceVolume.BindWithInitialValue(v => voiceVolumeSlider.value = v);
            AudioKit.Settings.SoundVolume.BindWithInitialValue(v => soundVolumeSlider.value = v);
            
            
            musicVolumeSlider.onValueChanged.AddListener(v => { AudioKit.Settings.MusicVolume.Value = v; });
            voiceVolumeSlider.onValueChanged.AddListener(v => { AudioKit.Settings.VoiceVolume.Value = v; });
            soundVolumeSlider.onValueChanged.AddListener(v => { AudioKit.Settings.SoundVolume.Value = v; });

         
        }
    }
}