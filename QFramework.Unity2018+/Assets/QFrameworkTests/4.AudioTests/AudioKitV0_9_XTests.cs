using System;
using NUnit.Framework;
using UnityEngine;

namespace QFramework.Tests
{
    /// <summary>
    /// 主要测试 Voice 相关的 API
    /// </summary>
    public class AudioKitV0_9_XTests
    {
        [Test]
        public void PlayVoiceTest()
        {

#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif

            ResMgr.Init();

            var loader = ResLoader.Allocate();

            // 提前预加载，否则会进行异步加载
            loader.LoadSync<AudioClip>("sound1");

            AudioKit.Settings.IsVoiceOn.Value = true;

            AudioKit.PlayVoice("sound1");

            Assert.AreEqual(AudioKit.VoicePlayer.AudioSource.clip.name, "sound1");
            Assert.IsTrue(AudioKit.VoicePlayer.AudioSource.isPlaying);

            loader.Recycle2Cache();
            loader = null;
        }

        [Test]
        public void IsVoiceOnTest()
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
            ResMgr.Init();

            var loader = ResLoader.Allocate();

            // 提前预加载，否则会进行异步加载
            loader.LoadSync<AudioClip>("sound1");

            AudioKit.Settings.IsVoiceOn.Value = false;

            AudioKit.PlayVoice("sound1");

            Assert.IsFalse(AudioKit.VoicePlayer.AudioSource);

            Assert.Throws<NullReferenceException>(() =>
            {
                var name = AudioKit.VoicePlayer.AudioSource.clip.name;
            });

            loader.Recycle2Cache();
            loader = null;
        }

        [Test]
        public void ReplaceVoiceTest()
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
            ResMgr.Init();

            var loader = ResLoader.Allocate();

            // 提前预加载，否则会进行异步加载
            loader.LoadSync<AudioClip>("sound1");
            loader.LoadSync<AudioClip>("failed1");

            AudioKit.Settings.IsVoiceOn.Value = true;

            AudioKit.PlayVoice("sound1");
            Assert.AreEqual(AudioKit.VoicePlayer.AudioSource.clip.name, "sound1");
            Assert.IsTrue(AudioKit.VoicePlayer.AudioSource.isPlaying);

            AudioKit.PlayVoice("failed1");
            Assert.AreEqual(AudioKit.VoicePlayer.AudioSource.clip.name, "failed1");

            loader.Recycle2Cache();
            loader = null;

        }

        [Test]
        public void PauseAndResumeVoiceTest()
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
            ResMgr.Init();

            var loader = ResLoader.Allocate();

            // 提前预加载，否则会进行异步加载
            loader.LoadSync<AudioClip>("sound1");

            AudioKit.Settings.IsVoiceOn.Value = true;

            AudioKit.PlayVoice("sound1");
            AudioKit.PauseVoice();

            Assert.IsFalse(AudioKit.VoicePlayer.AudioSource.isPlaying);

            AudioKit.ResumeVoice();

            Assert.IsTrue(AudioKit.VoicePlayer.AudioSource.isPlaying);

            loader.Recycle2Cache();
            loader = null;
        }
        
        [Test]
        public void StopVoiceTest()
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
            ResMgr.Init();

            var loader = ResLoader.Allocate();

            // 提前预加载，否则会进行异步加载
            loader.LoadSync<AudioClip>("sound1");

            AudioKit.Settings.IsVoiceOn.Value = true;

            AudioKit.PlayVoice("sound1");

            Assert.IsTrue(AudioKit.VoicePlayer.AudioSource.isPlaying);
            Assert.IsTrue(AudioKit.VoicePlayer.AudioSource.clip);

            AudioKit.StopVoice();

            Assert.IsFalse(AudioKit.VoicePlayer.AudioSource.isPlaying);
            Assert.IsFalse(AudioKit.VoicePlayer.AudioSource.clip);

            loader.Recycle2Cache();
            loader = null;
        }
        
        [Test]
        public void VoiceVolumeTest()
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
            ResMgr.Init();

            var loader = ResLoader.Allocate();

            // 提前预加载，否则会进行异步加载
            loader.LoadSync<AudioClip>("sound1");

            AudioKit.Settings.IsVoiceOn.Value = true;

            AudioKit.Settings.VoiceVolume.Value = 0.5f;

            AudioKit.PlayVoice("sound1");

            Assert.AreEqual(AudioKit.VoicePlayer.AudioSource.volume, 0.5f);

            AudioKit.Settings.VoiceVolume.Value = 1.0f;

            Assert.AreEqual(AudioKit.VoicePlayer.AudioSource.volume, 1.0f);

            loader.Recycle2Cache();
            loader = null;
        }
        
        [Test]
        public void VoiceOnTest()
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
            ResMgr.Init();

            var loader = ResLoader.Allocate();

            // 提前预加载，否则会进行异步加载
            loader.LoadSync<AudioClip>("sound1");

            AudioKit.Settings.IsVoiceOn.Value = true;

            AudioKit.PlayVoice("sound1");

            AudioKit.Settings.IsVoiceOn.Value = false;

            Assert.IsFalse(AudioKit.VoicePlayer.AudioSource.isPlaying);

            loader.Recycle2Cache();
            loader = null;
        }

        [Test]
        public void VoiceOffTest()
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
            ResMgr.Init();

            var loader = ResLoader.Allocate();

            // 提前预加载，否则会进行异步加载
            loader.LoadSync<AudioClip>("sound1");

            AudioKit.Settings.IsVoiceOn.Value = false;

            AudioKit.PlayVoice("sound1");

            AudioKit.Settings.IsVoiceOn.Value = true;

            Assert.IsTrue(AudioManager.Instance.VoicePlayer.AudioSource.isPlaying);

            loader.Recycle2Cache();
            loader = null;
        }
        
    }
}