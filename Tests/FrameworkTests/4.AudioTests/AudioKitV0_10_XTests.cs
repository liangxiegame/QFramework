using NUnit.Framework;
using UnityEngine;

namespace QFramework.Tests
{
    /// <summary>
    /// Sound 相关的 API 测试
    /// </summary>
    public class AudioKitV0_10_XTests
    {
        [Test]
        public void PlaySound()
        {
            ResMgr.Init();

            var resLoader = ResLoader.Allocate();

            resLoader.LoadSync<AudioClip>("sound1");

            AudioKit.Settings.IsSoundOn.Value = true;

            var player = AudioKit.PlaySound("sound1");

            Assert.IsTrue(player.AudioSource.isPlaying);
            Assert.AreEqual(player.AudioSource.clip.name, "sound1");

            resLoader.Recycle2Cache();
            resLoader = null;
        }
        
        
        
        [Test]
        public void SoundOffTest()
        {
            ResMgr.Init();

            var resLoader = ResLoader.Allocate();

            resLoader.LoadSync<AudioClip>("sound1");

            AudioKit.Settings.IsSoundOn.Value = true;

            var player = AudioKit.PlaySound("sound1");

            AudioKit.Settings.IsSoundOn.Value = false;
            
            Assert.IsFalse(player.AudioSource.isPlaying);

            resLoader.Recycle2Cache();
            resLoader = null;
        }
        
        [Test]
        public void SoundVolumeTest()
        {
            ResMgr.Init();

            var resLoader = ResLoader.Allocate();

            resLoader.LoadSync<AudioClip>("sound1");

            AudioKit.Settings.IsSoundOn.Value = true;

            var player = AudioKit.PlaySound("sound1");

            AudioKit.Settings.SoundVolume.Value = 0.5f;
            
            Assert.AreEqual(player.AudioSource.volume,0.5f);
            
            AudioKit.Settings.SoundVolume.Value = 1.0f;
            
            Assert.AreEqual(player.AudioSource.volume,1.0f);

            resLoader.Recycle2Cache();
            resLoader = null;
        }

        [Test]
        public void StopAllSoundTest()
        {
            ResMgr.Init();

            var resLoader = ResLoader.Allocate();

            resLoader.LoadSync<AudioClip>("sound1");
            resLoader.LoadSync<AudioClip>("failed1");

            AudioKit.Settings.IsSoundOn.Value = true;

            var player = AudioKit.PlaySound("sound1");
            var player2 = AudioKit.PlaySound("failed1");

            AudioKit.StopAllSound();
            
            Assert.IsFalse(player.AudioSource.isPlaying);
            
            Assert.IsFalse(player2.AudioSource.isPlaying);

            resLoader.Recycle2Cache();
            resLoader = null;
        }
    }
}