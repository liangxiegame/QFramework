using System;
using NUnit.Framework;
using UnityEngine;

namespace QFramework.Tests
{
	/// <summary>
	/// 主要测试 Music 相关的 API
	/// </summary>
	public class AudioKitV0_8_XTests
	{
		[Test]
		public void PlayMusicTest()
		{

#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif

			ResMgr.Init();

			var loader = ResLoader.Allocate();

			// 提前预加载，否则会进行异步加载
			loader.LoadSync<AudioClip>("sound1");

			AudioKit.Settings.IsMusicOn.Value = true;
			AudioKit.PlayMusic("sound1");

			Assert.AreEqual(AudioKit.MusicPlayer.AudioSource.clip.name, "sound1");
			Assert.IsTrue(AudioKit.MusicPlayer.AudioSource.isPlaying);

			loader.Recycle2Cache();
			loader = null;
		}

		[Test]
		public void IsMusicOnTest()
		{
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
			ResMgr.Init();

			var loader = ResLoader.Allocate();

			// 提前预加载，否则会进行异步加载
			loader.LoadSync<AudioClip>("sound1");

			AudioKit.Settings.IsMusicOn.Value = false;
			AudioKit.PlayMusic("sound1");

			Assert.IsFalse(AudioKit.MusicPlayer.AudioSource);

			Assert.Throws<NullReferenceException>(() =>
			{
				var name = AudioKit.MusicPlayer.AudioSource.clip.name;
			});

			loader.Recycle2Cache();
			loader = null;
		}

		[Test]
		public void ReplaceMusicTest()
		{
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
			ResMgr.Init();

			var loader = ResLoader.Allocate();

			// 提前预加载，否则会进行异步加载
			loader.LoadSync<AudioClip>("sound1");
			loader.LoadSync<AudioClip>("failed1");

			AudioKit.Settings.IsMusicOn.Value = true;

			AudioKit.PlayMusic("sound1");
			Assert.AreEqual(AudioKit.MusicPlayer.AudioSource.clip.name, "sound1");
			Assert.IsTrue(AudioKit.MusicPlayer.AudioSource.isPlaying);

			AudioKit.PlayMusic("failed1");
			Assert.AreEqual(AudioKit.MusicPlayer.AudioSource.clip.name, "failed1");

			loader.Recycle2Cache();
			loader = null;

		}

		[Test]
		public void PauseAndResumeMusicTest()
		{
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
			ResMgr.Init();

			var loader = ResLoader.Allocate();

			// 提前预加载，否则会进行异步加载
			loader.LoadSync<AudioClip>("sound1");

			AudioKit.Settings.IsMusicOn.Value = true;

			AudioKit.PlayMusic("sound1");
			AudioKit.PauseMusic();

			Assert.IsFalse(AudioKit.MusicPlayer.AudioSource.isPlaying);

			AudioKit.ResumeMusic();

			Assert.IsTrue(AudioKit.MusicPlayer.AudioSource.isPlaying);

			loader.Recycle2Cache();
			loader = null;
		}

		[Test]
		public void StopMusicTest()
		{
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
			ResMgr.Init();

			var loader = ResLoader.Allocate();

			// 提前预加载，否则会进行异步加载
			loader.LoadSync<AudioClip>("sound1");

			AudioKit.Settings.IsMusicOn.Value = true;

			AudioKit.PlayMusic("sound1");

			Assert.IsTrue(AudioKit.MusicPlayer.AudioSource.isPlaying);
			Assert.IsTrue(AudioKit.MusicPlayer.AudioSource.clip);

			AudioKit.StopMusic();

			Assert.IsFalse(AudioKit.MusicPlayer.AudioSource.isPlaying);
			Assert.IsFalse(AudioKit.MusicPlayer.AudioSource.clip);

			loader.Recycle2Cache();
			loader = null;
		}

		[Test]
		public void MusicVolumeTest()
		{
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
			ResMgr.Init();

			var loader = ResLoader.Allocate();

			// 提前预加载，否则会进行异步加载
			loader.LoadSync<AudioClip>("sound1");

			AudioKit.Settings.IsMusicOn.Value = true;

			AudioKit.Settings.MusicVolume.Value = 0.5f;

			AudioKit.PlayMusic("sound1");

			Assert.AreEqual(AudioKit.MusicPlayer.AudioSource.volume, 0.5f);

			AudioKit.Settings.MusicVolume.Value = 1.0f;

			Assert.AreEqual(AudioKit.MusicPlayer.AudioSource.volume, 1.0f);

			loader.Recycle2Cache();
			loader = null;
		}

		[Test]
		public void MusicOnTest()
		{
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
			ResMgr.Init();

			var loader = ResLoader.Allocate();

			// 提前预加载，否则会进行异步加载
			loader.LoadSync<AudioClip>("sound1");

			AudioKit.Settings.IsMusicOn.Value = true;

			AudioKit.PlayMusic("sound1");

			AudioKit.Settings.IsMusicOn.Value = false;

			Assert.IsFalse(AudioKit.MusicPlayer.AudioSource.isPlaying);

			loader.Recycle2Cache();
			loader = null;
		}

		[Test]
		public void MusicOffTest()
		{
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetInt("SimulateAssetBundles", 1);
#endif
			ResMgr.Init();

			var loader = ResLoader.Allocate();

			// 提前预加载，否则会进行异步加载
			loader.LoadSync<AudioClip>("sound1");

			AudioKit.Settings.IsMusicOn.Value = false;

			AudioKit.PlayMusic("sound1");

			AudioKit.Settings.IsMusicOn.Value = true;

			Assert.IsTrue(AudioManager.Instance.MusicPlayer.AudioSource.isPlaying);

			loader.Recycle2Cache();
			loader = null;
		}

		[Test]
		public void IsOnTest()
		{
			var isOn = true;

			AudioKit.Settings.IsOn.Bind(on =>
			{
				isOn = on;
			});
			
			AudioKit.Settings.IsOn.Value = false;

			Assert.IsFalse(AudioKit.Settings.IsMusicOn.Value);
			Assert.IsFalse(AudioKit.Settings.IsSoundOn.Value);
			Assert.IsFalse(AudioKit.Settings.IsVoiceOn.Value);
			Assert.IsFalse(AudioKit.Settings.IsOn.Value);
			

			AudioKit.Settings.IsOn.Value = true;

			Assert.IsTrue(AudioKit.Settings.IsMusicOn.Value);
			Assert.IsTrue(AudioKit.Settings.IsSoundOn.Value);
			Assert.IsTrue(AudioKit.Settings.IsVoiceOn.Value);
			Assert.IsTrue(AudioKit.Settings.IsOn.Value);
		}
	}
}