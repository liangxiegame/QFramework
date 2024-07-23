using System;
using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace QFramework.Tests
{
	public class ResKitV0_10_XTests
	{
			/// <summary>
		/// 用于资源管理的资源
		/// </summary>
		class TestRes : Res
		{
			public TestRes(string name)
			{
				mAssetName = name;
			}
			public override bool LoadSync()
			{
				State = ResState.Ready;
				return true;
			}

			public override void LoadAsync()
			{
				State = ResState.Ready;
				
			}


			protected override void OnReleaseRes()
			{
				State = ResState.Waiting;
			}
		}

		/// <summary>
		/// 自定义的资源类型测试
		/// </summary>
		[Test]
		public void CustomResTest()
		{
			var customRes = new TestRes("TestRes");

			customRes.LoadSync();

			Assert.AreEqual(ResState.Ready, customRes.State);
		}

		/// <summary>
		/// ResMgr 的增删改查实现
		/// </summary>
		[Test]
		public void ResMgrCurdTest()
		{
			// var customRes = new TestRes("TestRes");
			//
			// var resMgr = ResMgr.Instance;
			//
			// resMgr.AddRes(customRes);
			//
			// Assert.IsNotNull(resMgr.GetRes(customRes.Name));
			// Assert.AreSame(resMgr.GetRes(customRes.Name), customRes);
			//
			// resMgr.RemoveRes(customRes);
			//
			// Assert.IsNull(resMgr.GetRes(customRes.Name));
		}

		public class TestResCreator : IResCreator
		{
			public bool Match(ResSearchKeys resSearchKeys)
			{
				return resSearchKeys.AssetName.StartsWith("test://");
			}

			public IRes Create(ResSearchKeys resSearchKeys)
			{
				return new TestRes(resSearchKeys.AssetName)
				{
					AssetType = resSearchKeys.AssetType
				};
			}
		}
		/// <summary>
		/// ResLoader 测试
		/// </summary>
		[Test]
		public void ResLoaderTest()
		{
			ResFactory.AddResCreator<TestResCreator>();

			// 测试
			var resLoader = ResLoader.Allocate();

			var resSearchKeys = ResSearchKeys.Allocate("test://icon_texture", null, typeof(Texture2D));

			var iconTextureRes = resLoader.LoadResSync(resSearchKeys);

			Assert.IsTrue(iconTextureRes is TestRes);
			Assert.AreEqual(1, iconTextureRes.RefCount);
			Assert.AreEqual(ResState.Ready, iconTextureRes.State);

			resLoader.Recycle2Cache();

			Assert.AreEqual(0, iconTextureRes.RefCount);
			Assert.AreEqual(ResState.Waiting, iconTextureRes.State);
			
			resLoader = null;
		}

		[Test]
		public void ResourcesResTest()
		{
			var resLoader = ResLoader.Allocate();

			var audioClip = resLoader.LoadSync<AudioClip>("resources://coin_get1");

			Assert.IsNotNull(audioClip);

			var resSearchKeys = ResSearchKeys.Allocate("resources://coin_get1", null, typeof(AudioClip));
			var audioClipRes = resLoader.LoadResSync(resSearchKeys);
			resSearchKeys.Recycle2Cache();

			Assert.AreEqual(1, audioClipRes.RefCount);
			Assert.AreEqual(ResState.Ready, audioClipRes.State);

			resLoader.Recycle2Cache();

			Assert.AreEqual(0, audioClipRes.RefCount);
			Assert.AreEqual(ResState.Waiting, audioClipRes.State);

			resLoader = null;
		}
		
		
		
		[Test]
		public static void StringAndTypeCompareSpeedTest()
		{
			var prefix = "ab://";
			var assetBundleName = "ab://assetBundle";

			var type = typeof(AssetBundle);

			TimeSpan stringTime;
			TimeSpan typeTime;

			var compareTimes = 10000;

			var watch = new Stopwatch();

			watch.Start();

			for (var i = 0; i < compareTimes; i++)
			{
				if (assetBundleName.StartsWith(prefix))
				{

				}
			}

			watch.Stop();

			stringTime = watch.Elapsed;

			watch.Reset();

			watch.Start();

			for (var i = 0; i < compareTimes; i++)
			{
				if (type == typeof(AssetBundle))
				{

				}
			}

			watch.Stop();

			typeTime = watch.Elapsed;

			Debug.LogFormat("stringTime:{0}", stringTime);
			Debug.LogFormat("typeTime:{0}", typeTime);

			Assert.IsTrue(stringTime > typeTime);
		}
		
		[Test]
		public void ReadConfigTest()
		{
#if UNITY_EDITOR
			ResKitEditorAPI.SimulationMode = false;
#endif
			ResKit.Init();

			var resDatas = AssetBundleSettings.AssetBundleConfigFile;

			var resSearchKeys = ResSearchKeys.Allocate("coin_get1", null, typeof(AudioClip));
			var coinGetData =resDatas.GetAssetData(resSearchKeys);
			resSearchKeys.Recycle2Cache();

			Assert.AreEqual(coinGetData.AssetName, "coin_get1");
			Assert.AreEqual(coinGetData.AssetObjectTypeCode.ToType(), typeof(AudioClip));

			resSearchKeys = ResSearchKeys.Allocate("coin_get1", null, typeof(GameObject));
			var coinGetObjData = resDatas.GetAssetData(resSearchKeys);
			resSearchKeys.Recycle2Cache();

			Assert.AreEqual(coinGetObjData.AssetName, "coin_get1");
			Assert.AreEqual(coinGetObjData.AssetObjectTypeCode.ToType(), typeof(GameObject));
		}
		
		
		[Test]
		public void LoadAssetWithNameAndTypeInRuntimeModeTest()
		{
#if UNITY_EDITOR
			ResKitEditorAPI.SimulationMode = false;
#endif
			ResKit.Init();
			
			var resLoader = ResLoader.Allocate();

			var audioClip = resLoader.LoadSync<AudioClip>("coin_get1");

			Assert.IsTrue(audioClip);

			var gameObj = resLoader.LoadSync<GameObject>("coin_get1");

			Assert.IsTrue(gameObj);

			resLoader.Recycle2Cache();
			resLoader = null;
		}

		[UnityTest]
		public IEnumerator LoadAssetWithNameAndTypeAsyncInRuntimeModeTest()
		{
#if UNITY_EDITOR
			ResKitEditorAPI.SimulationMode = false;
#endif
			ResKit.Init();

			var resLoader = ResLoader.Allocate();

			var loadCount = 0;

			resLoader.Add2Load<AudioClip>("coin_get1", (succeed, res) =>
			{
				Assert.IsTrue(res.Asset as AudioClip);

				loadCount++;
			});

			resLoader.Add2Load<GameObject>("coin_get1", (succeed, res) =>
			{
				Assert.IsTrue(res.Asset as GameObject);

				loadCount++;
			});

			resLoader.LoadAsync();

			while (loadCount != 2)
			{
				yield return null;
			}

			resLoader.Recycle2Cache();
			resLoader = null;
		}

		[UnityTest]
		public IEnumerator LoadAssetWithNameAndTypeAsyncTest()
		{
#if UNITY_EDITOR
			ResKitEditorAPI.SimulationMode = true;
#endif
			ResKit.Init();
			
			var resLoader = ResLoader.Allocate();

			var loadCount = 0;

			resLoader.Add2Load<AudioClip>("coin_get1", (succeed, res) =>
			{
				Assert.IsTrue(res.Asset as AudioClip);

				loadCount++;
			});

			resLoader.Add2Load<GameObject>("coin_get1", (succeed, res) =>
			{
				Assert.IsTrue(res.Asset as GameObject);

				loadCount++;
			});
			
			resLoader.LoadAsync();

			while (loadCount != 2)
			{
				yield return null;
			}

			resLoader.Recycle2Cache();
			resLoader = null;
		}
		
		
		[Test]
		public void LoadAssetWithNameAndTypeTest()
		{
#if UNITY_EDITOR
			ResKitEditorAPI.SimulationMode = true;
#endif

			ResKit.Init();
			
			var resLoader = ResLoader.Allocate();

			var audioClip = resLoader.LoadSync<AudioClip>("coin_get1");

			Assert.IsTrue(audioClip);

			var gameObj = resLoader.LoadSync<GameObject>("coin_get1");

			Assert.IsTrue(gameObj);

			resLoader.Recycle2Cache();
			resLoader = null;
		}
		
		[Test]
		public void AssetBundleTable_GetAssetDataTest()
		{
#if UNITY_EDITOR
			ResKitEditorAPI.SimulationMode = true;
#endif
			ResKit.Init();
			
			var assetBundleTable = AssetBundleSettings.AssetBundleConfigFile;

			var resSearchKeys = ResSearchKeys.Allocate("coin_get1", null, typeof(AudioClip));
			var audioClipData = assetBundleTable.GetAssetData(resSearchKeys);
			resSearchKeys.Recycle2Cache();

			Assert.IsNotNull(audioClipData);
			Assert.AreEqual(audioClipData.OwnerBundleName, "coin_get1_mp3");
			Assert.AreEqual(audioClipData.AssetObjectTypeCode, typeof(AudioClip).ToCode());

			resSearchKeys = ResSearchKeys.Allocate("coin_get1", null,typeof(GameObject));
			var gameObjectData = assetBundleTable.GetAssetData(resSearchKeys);
			resSearchKeys.Recycle2Cache();

			Assert.IsNotNull(gameObjectData);
			Assert.AreEqual(gameObjectData.OwnerBundleName, "coin_get1_prefab");
			Assert.AreEqual(gameObjectData.AssetObjectTypeCode,  typeof(GameObject).ToCode());
		}
		
		[UnityTest]
		public IEnumerator LoadAssetAsyncTest()
		{
			ResKit.Init();
			
			var resLoader = ResLoader.Allocate();

			var loadDone = false;

			resLoader.Add2Load<GameObject>("coin_get1_prefab", "coin_get1", (succeed, res) =>
			{
				var coinGetPrefab = res.Asset as GameObject;

				Assert.IsTrue(coinGetPrefab);

				loadDone = succeed;
			});
			
			resLoader.LoadAsync();
			
			while (!loadDone)
			{
				yield return null;
			}

			resLoader.Recycle2Cache();
		}
		
		[Test]
		public void LoadAssetTest()
		{
			ResKit.Init();
			
			var resLoader = ResLoader.Allocate();

			var coinGetPrefab = resLoader.LoadSync<GameObject>("coin_get1_prefab", "coin_get1");

			Assert.IsTrue(coinGetPrefab);

			resLoader.Recycle2Cache();
		}
		
		[UnityTest]
		public IEnumerator ResourcesResLoadAsyncTest()
		{
			var resLoader = ResLoader.Allocate();

			var loaded = false;
			AudioClip clip = null;
			
			// 异步加载一个资源
			resLoader.Add2Load<AudioClip>("resources://coin_get1", (succeed,res) =>
			{
				if (succeed)
				{
					clip = res.Asset as AudioClip;
					loaded = true;
				}
				else
				{
					loaded = true;
				}
			});
			
			resLoader.LoadAsync();

			// 等待回调成功
			while (!loaded)
			{
				yield return null;
			}

			Assert.IsNotNull(clip);
			
			resLoader.Recycle2Cache();
		}
		
		[Test]
		public void LoadAssetBundleDependenciesTest()
		{
			ResKit.Init();
			
			var resLoader = ResLoader.Allocate();
            
			var coinGetPrefab = resLoader.LoadSync<GameObject>("coin_get1_prefab", "coin_get1");

			var coinGetObj = Object.Instantiate(coinGetPrefab);

			var audioClip = coinGetObj.GetComponent<AudioSource>().clip;

			Assert.IsTrue(audioClip);
			
			resLoader.Recycle2Cache();
		}
		
		[UnityTest]
		public IEnumerator LoadAssetBundleDependenciesAsyncTest()
		{
			ResKit.Init();
			
			var resLoader = ResLoader.Allocate();

			var loadDone = false;

			resLoader.Add2Load<GameObject>("coin_get1_prefab", "coin_get1", (succeed, res) =>
			{
				var coinGetPrefab = res.Asset as GameObject;

				var coinGetObj = Object.Instantiate(coinGetPrefab);

				var audioClip = coinGetObj.GetComponent<AudioSource>().clip;

				Assert.IsTrue(audioClip);

				loadDone = true;
			});
			
			resLoader.LoadAsync();

			while (!loadDone)
			{
				yield return null;
			}
		}
		
		[UnityTest]
		public IEnumerator LoadAssetResAsyncTest()
		{
			ResKit.Init();
			
			var resLoader = ResLoader.Allocate();

			var loadDone = false;

			resLoader.Add2Load<AudioClip>("coin_get1_mp3", "coin_get1", (succeed, res) =>
			{
				var coinGetClip = res.Asset as AudioClip;

				Assert.IsTrue(coinGetClip);

				loadDone = succeed;
			});
			
			resLoader.LoadAsync();

			while (!loadDone)
			{
				yield return null;
			}
			
			resLoader.Recycle2Cache();

			resLoader = null;
		}
		
		[Test]
		public void LoadAssetResSyncTest()
		{
			ResKit.Init();
			
			var resLoader = ResLoader.Allocate();

			var coinGetClip = resLoader.LoadSync<AudioClip>("coin_get1_mp3", "coin_get1");

			Assert.IsTrue(coinGetClip);

			resLoader.Recycle2Cache();

			resLoader = null;
		}
		
		[UnityTest]
		public IEnumerator LoadAsyncTwiceBugTest()
		{
			var loadedCount = 0;

			var resLoader = ResLoader.Allocate();

			resLoader.Add2Load<AudioClip>("resources://coin_get1", (succeed, res) =>
			{
				Assert.AreEqual(ResState.Ready, res.State);
				loadedCount++;
			});

			resLoader.Add2Load<AudioClip>("resources://coin_get1", (succeed, res) =>
			{
				Assert.AreEqual(ResState.Ready, res.State);
				loadedCount++;
			});

			resLoader.LoadAsync();

			yield return new WaitUntil(() => loadedCount == 2);

			resLoader.Recycle2Cache();
		}

		[Test]
		public void GetWrongTypeBugTest()
		{
			var resLoader = ResLoader.Allocate();

			var coinGetTextAsset = resLoader.LoadSync<GameObject>("resources://coin_get1");

			Assert.IsNotNull(coinGetTextAsset);

			var coinGetAudioClip = resLoader.LoadSync<AudioClip>("resources://coin_get1");

			Assert.IsNotNull(coinGetAudioClip);

			resLoader.Recycle2Cache();
		}

		[Test]
		public void LoadAssetBundle()
		{
			ResKit.Init();

			var resLoader = ResLoader.Allocate();

			resLoader.LoadSync<AssetBundle>("coin_get1_mp3");

			var resSearchKeys = ResSearchKeys.Allocate("coin_get1_mp3", null, typeof(AssetBundle));
			var res = ResMgr.Instance.GetRes(resSearchKeys);
			;

			resSearchKeys.Recycle2Cache();

			Assert.AreEqual(res.State, ResState.Ready);
			Assert.AreEqual(res.AssetType, typeof(AssetBundle));

			resLoader.Recycle2Cache();
		}


		[UnityTest]
		public IEnumerator LoadAssetBundleAsync()
		{
			var resLoader = ResLoader.Allocate();

			var loadDone = false;

			resLoader.Add2Load<AssetBundle>("coin_get1_mp3", (succeed, _) =>
			{
				var resSearchKeys = ResSearchKeys.Allocate("coin_get1_mp3", null, typeof(AssetBundle));
				var res = ResMgr.Instance.GetRes(resSearchKeys);

				resSearchKeys.Recycle2Cache();

				Assert.AreEqual(res.State, ResState.Ready);
				Assert.AreEqual(res.AssetType, typeof(AssetBundle));
				loadDone = true;
			});

			resLoader.LoadAsync();

			while (!loadDone)
			{
				yield return null;
			}

			resLoader.Recycle2Cache();
		}
	}
}