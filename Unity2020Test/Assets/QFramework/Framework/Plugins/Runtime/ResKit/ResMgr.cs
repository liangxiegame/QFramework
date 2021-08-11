/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2021.1 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.Collections;
using System.Linq;

namespace QFramework
{
    using System.Collections.Generic;
    using UnityEngine;

    [MonoSingletonPath("[Framework]/ResMgr")]
    public class ResMgr : MonoSingleton<ResMgr>
    {
        #region ID:RKRM001 Init v0.1.0 Unity5.5.1p4

        private static bool mResMgrInited = false;
        
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
            if (FromUnityToDll.Setting.SimulationMode)
            {
                AssetBundleSettings.AssetBundleConfigFile = FromUnityToDll.Setting.BuildEditorDataTable();
                yield return null;
            }
            else
            {
                AssetBundleSettings.AssetBundleConfigFile.Reset();

                var outResult = new List<string>();

                var pathPrefix = FromUnityToDll.Setting.PathPrefix;

                // 未进行过热更
                if (AssetBundleSettings.LoadAssetResFromStreamingAssetsPath)
                {
                    var streamingPath = Application.streamingAssetsPath + "/AssetBundles/" +
                                        FromUnityToDll.Setting.GetPlatformName() + "/" + ResDatas.FileName;
                    outResult.Add(pathPrefix + streamingPath);
                }
                // 进行过热更
                else
                {
                    var persistentPath = Application.persistentDataPath + "/AssetBundles/" +
                                         FromUnityToDll.Setting.GetPlatformName() + "/" + ResDatas.FileName;
                    outResult.Add(pathPrefix + persistentPath);
                }

                foreach (var outRes in outResult)
                {
                    Log.I(outRes);
                    yield return AssetBundleSettings.AssetBundleConfigFile.LoadFromFileAsync(outRes);
                }

                yield return null;
            }
        }

        public void InitResMgr()
        {
            if (FromUnityToDll.Setting.SimulationMode)
            {
                AssetBundleSettings.AssetBundleConfigFile = FromUnityToDll.Setting.BuildEditorDataTable();
            }
            else
            {
                AssetBundleSettings.AssetBundleConfigFile.Reset();

                var outResult = new List<string>();

                // 未进行过热更
                if (AssetBundleSettings.LoadAssetResFromStreamingAssetsPath)
                {
                    ResKit.Architecture.Interface.GetUtility<IZipFileHelper>()
                        .GetFileInInner(ResDatas.FileName, outResult);
                }
                // 进行过热更
                else
                {
                    FromUnityToDll.Setting.GetFileInFolder(FromUnityToDll.Setting.PersistentDataPath, ResDatas.FileName,
                        outResult);
                }

                foreach (var outRes in outResult)
                {
                    Log.I(outRes);
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
                Log.I("createNew:{0}", createNew);
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
            if (FromUnityToDll.Platform.IsEditor && FromUnityToDll.Input.ShowResKitDebugInfo)
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

        private static List<string> mSubProjectNames = new List<string>();

        public static void AddSubProject(string projectName)
        {
            mSubProjectNames.Add(projectName);
        }

        public static IEnumerator AddSubProjectAsync(string projectName)
        {

            yield return null;
        }
    }
}