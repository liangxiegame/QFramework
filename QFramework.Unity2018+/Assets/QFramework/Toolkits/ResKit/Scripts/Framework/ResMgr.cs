/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2023 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections;
using System.Linq;

namespace QFramework
{
    using System.Collections.Generic;
    using UnityEngine;

    [MonoSingletonPath("[Framework]/ResMgr")]
    public class ResMgr : MonoBehaviour,ISingleton
    {
        public static ResMgr Instance => MonoSingletonProperty<ResMgr>.Instance;

        #region ID:RKRM001 Init v0.1.0 Unity5.5.1p4

        private static bool mResMgrInited = false;

        public static bool ResMgrInited => mResMgrInited;
        
        /// <summary>
        /// 初始化bin文件
        /// </summary>
        public static void Init()
        {
            if (mResMgrInited) return;
            mResMgrInited = true;

            SafeObjectPool<AssetBundleRes>.Instance.Init(40, 20);
            SafeObjectPool<AssetRes>.Instance.Init(40, 20);
            SafeObjectPool<ResourcesRes>.Instance.Init(40, 20);
            SafeObjectPool<NetImageRes>.Instance.Init(40, 20);
            SafeObjectPool<ResSearchKeys>.Instance.Init(40, 20);
            SafeObjectPool<ResLoader>.Instance.Init(40, 20);


            Instance.InitResMgr();
        }


        public static IEnumerator InitAsync()
        {
            if (mResMgrInited) yield break;
            mResMgrInited = true;

            SafeObjectPool<AssetBundleRes>.Instance.Init(40, 20);
            SafeObjectPool<AssetRes>.Instance.Init(40, 20);
            SafeObjectPool<ResourcesRes>.Instance.Init(40, 20);
            SafeObjectPool<NetImageRes>.Instance.Init(40, 20);
            SafeObjectPool<ResSearchKeys>.Instance.Init(40, 20);
            SafeObjectPool<ResLoader>.Instance.Init(40, 20);

            yield return Instance.InitResMgrAsync();
        }

        #endregion

        public int Count
        {
            get { return mTable.Count(); }
        }

        public static bool IsApplicationQuit { get;private set; }

        private void OnApplicationQuit()
        {
            IsApplicationQuit = true;
        }

        #region 字段

        private ResTable mTable = new ResTable();

        [SerializeField] private int mCurrentCoroutineCount;
        private int mMaxCoroutineCount = 8; //最快协成大概在6到8之间
        private LinkedList<IEnumeratorTask> mIEnumeratorTaskStack = new LinkedList<IEnumeratorTask>();

        //Res 在ResMgr中 删除的问题，ResMgr定时收集列表中的Res然后删除
        private bool mIsResMapDirty;

        #endregion

        public IEnumerator InitResMgrAsync()
        {
            if (AssetBundlePathHelper.SimulationMode)
            {
                AssetBundleSettings.AssetBundleConfigFile = ConfigFileUtility.BuildEditorDataTable();
                yield return null;
            }
            else
            {
                AssetBundleSettings.AssetBundleConfigFile.Reset();

                var outResult = new List<string>();

                var pathPrefix = AssetBundlePathHelper.PathPrefix;

                // 未进行过热更
                if (AssetBundleSettings.LoadAssetResFromStreamingAssetsPath)
                {
                    var streamingPath = Application.streamingAssetsPath + "/AssetBundles/" +
                                        AssetBundlePathHelper.GetPlatformName() + "/" + ResDatas.FileName;
                    outResult.Add(pathPrefix + streamingPath);
                }
                // 进行过热更
                else
                {
                    var persistentPath = Application.persistentDataPath + "/AssetBundles/" +
                                         AssetBundlePathHelper.GetPlatformName() + "/" + ResDatas.FileName;
                    outResult.Add(pathPrefix + persistentPath);
                }

                foreach (var outRes in outResult)
                {
                    Debug.Log(outRes);
                    yield return AssetBundleSettings.AssetBundleConfigFile.LoadFromFileAsync(outRes);
                }

                yield return null;
            }
        }

        public void InitResMgr()
        {
            if (AssetBundlePathHelper.SimulationMode)
            {
                AssetBundleSettings.AssetBundleConfigFile = ConfigFileUtility.BuildEditorDataTable();
            }
            else
            {
                AssetBundleSettings.AssetBundleConfigFile.Reset();

                var outResult = new List<string>();

                // 未进行过热更
                if (AssetBundleSettings.LoadAssetResFromStreamingAssetsPath)
                {
                    ResKit.Get.Container.Get<IZipFileHelper>()
                        .GetFileInInner(ResDatas.FileName, outResult);
                }
                // 进行过热更
                else
                {
                    AssetBundlePathHelper.GetFileInFolder(AssetBundlePathHelper.PersistentDataPath, ResDatas.FileName,
                        outResult);
                }

                foreach (var outRes in outResult)
                {
                 
                    AssetBundleSettings.AssetBundleConfigFile.LoadFromFile(outRes);
                }
            }
        }

        #region 属性

        public void ClearOnUpdate()
        {
            mIsResMapDirty = true;
        }

        public void PushIEnumeratorTask(IEnumeratorTask task)
        {
            if (task == null)
            {
                return;
            }

            mIEnumeratorTaskStack.AddLast(task);
            TryStartNextIEnumeratorTask();
        }


        public IRes GetRes(ResSearchKeys resSearchKeys, bool createNew = false)
        {
            var res = mTable.GetResBySearchKeys(resSearchKeys);

            if (res != null)
            {
                return res;
            }

            if (!createNew)
            {
                Debug.LogFormat("createNew:{0}", createNew);
                return null;
            }

            res = ResFactory.Create(resSearchKeys);

            if (res != null)
            {
                mTable.Add(res);
            }

            return res;
        }

        public T GetRes<T>(ResSearchKeys resSearchKeys) where T : class, IRes
        {
            return GetRes(resSearchKeys) as T;
        }

        #endregion

        #region Private Func

        private void Update()
        {
            if (mIsResMapDirty)
            {
                RemoveUnusedRes();
            }
        }

        private void RemoveUnusedRes()
        {
            if (!mIsResMapDirty)
            {
                return;
            }

            mIsResMapDirty = false;

            foreach (var res in mTable.ToArray())
            {
                if (res.RefCount <= 0 && res.State != ResState.Loading)
                {
                    if (res.ReleaseRes())
                    {
                        mTable.Remove(res);

                        
                        res.Recycle2Cache();
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (PlatformCheck.IsEditor && Input.GetKey(KeyCode.F1))
            {
                GUILayout.BeginVertical("box");

                GUILayout.Label("ResKit", new GUIStyle {fontSize = 30});
                GUILayout.Space(10);
                GUILayout.Label("ResInfo", new GUIStyle {fontSize = 20});
                mTable.ToList().ForEach(res => { GUILayout.Label((res as Res).ToString()); });
                GUILayout.Space(10);

                GUILayout.Label("Pools", new GUIStyle() {fontSize = 20});
                GUILayout.Label(string.Format("ResSearchRule:{0}",
                    SafeObjectPool<ResSearchKeys>.Instance.CurCount));
                GUILayout.Label(string.Format("ResLoader:{0}",
                    SafeObjectPool<ResLoader>.Instance.CurCount));
                GUILayout.EndVertical();
            }
        }

        private void OnIEnumeratorTaskFinish()
        {
            --mCurrentCoroutineCount;
            TryStartNextIEnumeratorTask();
        }

        private void TryStartNextIEnumeratorTask()
        {
            if (mIEnumeratorTaskStack.Count == 0)
            {
                return;
            }

            if (mCurrentCoroutineCount >= mMaxCoroutineCount)
            {
                return;
            }

            var task = mIEnumeratorTaskStack.First.Value;
            mIEnumeratorTaskStack.RemoveFirst();

            ++mCurrentCoroutineCount;
            StartCoroutine(task.DoLoadAsync(OnIEnumeratorTaskFinish));
        }

        #endregion
        
        public void OnSingletonInit()
        {
            
        }
    }
}