using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework {
	/// <summary>
	/// 音效管理器
	/// </summary>
	public class SoundMgr : QMonoSingleton<SoundMgr> {

		private List<AudioSource> clipPlayers = new List<AudioSource>();		// 音效播放器
		private AudioSource musicPlayer;										// 音乐播放器
		private AudioListener listener;											// 音监听器
		private string mCurClipName;											// 当前的音效名字

		private SoundMgr(){}


		public AudioClip[] clips = new AudioClip[SOUND.COUNT];					// 多少种Clips

		/// <summary>
		/// 创建音效播放器和音乐播放器
		/// </summary>
		void Awake()
		{
			listener = gameObject.AddComponent<AudioListener> ();
			clipPlayers.Add (gameObject.AddComponent<AudioSource> ());
			musicPlayer = gameObject.AddComponent<AudioSource> ();
		}

		public void PreloadClips(string path,int id)
		{
			ResMgr.Instance ().LoadRes (path, delegate(string resName, Object resObj) {
				if (resObj)
				{
					Debug.LogWarning ("loaded: " + path + " " + id.ToString());
					clips[id] = resObj as AudioClip;
				}
			}); 

		}

		public void PlayClip(int id,bool loop = false)
		{

			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();

			for (int i = 0; i < clipPlayers.Count; i++) {
				if (clipPlayers [i].isPlaying) {

				} else {

					clipPlayers [i].clip = clips [id];
					clipPlayers [i].loop = loop;
					clipPlayers [i].Play ();

					watch.Stop ();
					Debug.Log ("playclip:" + watch.ElapsedMilliseconds);

					return;
				}
			}

			var newSource = gameObject.AddComponent<AudioSource> ();
			clipPlayers.Add (newSource);

			newSource.clip = clips [id];
			newSource.Play ();


			watch.Stop ();
			Debug.Log ("playclip:" + watch.ElapsedMilliseconds);
		}

		public void PlayMusic(int id,bool loop = true)
		{
			QPrint.Warn (id + "" + loop);
			musicPlayer.loop = loop;
			musicPlayer.clip = clips [id];
			musicPlayer.volume = 1.0f;
			musicPlayer.Play ();
		}

		public void StopMusic()
		{
			musicPlayer.Stop ();
		}

		/// <summary>
		/// 停止所有音效
		/// </summary>
		public void StopSounds()
		{
			for (int i = 0; i < clipPlayers.Count; i++) {
				clipPlayers [i].Stop ();
			}
		}


		public void On()
		{
			QPrint.Warn ("Sound On");
			listener.enabled = true;
			DataManager.Instance ().soundState = SOUND.ON;
		}


		public void Off()
		{
			QPrint.Warn ("Sound Off");
			listener.enabled = false;
			DataManager.Instance ().soundState = SOUND.OFF;
		}
	}

}