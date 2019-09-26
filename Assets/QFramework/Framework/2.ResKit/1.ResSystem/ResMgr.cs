/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2018.5 liangxie
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

using QFramework;

namespace QF.Res
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [MonoSingletonPath("[Framework]/ResMgr")]
    public class ResMgr : MonoSingleton<ResMgr>, IEnumeratorTaskMgr
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
            SafeObjectPool<ResSearchRule>.Instance.Init(40, 20);            
            SafeObjectPool<ResLoader>.Instance.Init(40, 20);

            
            Instance.InitResMgr();
        }

        #endregion
        
        public int Count
        {
            get { return mResList.Count; }
        }
        
        #region 字段

        private readonly Dictionary<string, IRes> mResDictionary = new Dictionary<string, IRes>();
        private readonly List<IRes> mResList = new List<IRes>();
        [SerializeField] private int mCurrentCoroutineCount;
        private int mMaxCoroutineCount = 8; //最快协成大概在6到8之间
        private LinkedList<IEnumeratorTask> mIEnumeratorTaskStack = new LinkedList<IEnumeratorTask>();

        //Res 在ResMgr中 删除的问题，ResMgr定时收集列表中的Res然后删除
        private bool mIsResMapDirty;

        #endregion

		public void InitResMgr()
        {   
#if UNITY_EDITOR
            if (Res.SimulateAssetBundleInEditor)
            {
                EditorRuntimeAssetDataCollector.BuildDataTable();
            }
            else
#endif
            {
                ResDatas.Instance.Reset();
                var outResult = new List<string>();
                FileMgr.Instance.GetFileInInner("asset_bindle_config.bin", outResult);
                foreach (var outRes in outResult)
                {
                    ResDatas.Instance.LoadFromFile(outRes);
                }
            }

            ResDatas.Instance.SwitchLanguage("cn");
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


        public IRes GetRes(ResSearchRule resSearchRule, bool createNew = false)
        {
            IRes res = null;
            if (mResDictionary.TryGetValue(resSearchRule.DictionaryKey, out res))
            {
                return res;
            }

            if (!createNew)
            {
                Log.I("createNew:{0}",createNew);
                return null;
            }

            res = ResFactory.Create(resSearchRule);

            if (res != null)
            {
                mResDictionary.Add(resSearchRule.DictionaryKey, res);
                if (!mResList.Contains(res))
                {
                    mResList.Add(res);
                }
            }
            return res;
        }

        public R GetRes<R>(string assetName) where R : IRes
        {
            IRes res = null;
            if (mResDictionary.TryGetValue(assetName, out res))
            {
                return (R) res;
            }

            return default(R);
        }

        public R GetAsset<R>(string name) where R : Object
        {
            IRes res = null;
            if (mResDictionary.TryGetValue(name, out res))
            {
                return res.Asset as R;
            }

            return null;
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

        public void RemoveUnusedRes()
        {
            if (!mIsResMapDirty)
            {
                return;
            }

            mIsResMapDirty = false;

            for (var i = mResList.Count - 1; i >= 0; --i)
            {
                var res = mResList[i];
                if (res.RefCount <= 0 && res.State != ResState.Loading)
                {
                    if (res.ReleaseRes())
                    {
                        mResList.RemoveAt(i);
                        mResDictionary.Remove(res.AssetName);
                        mResDictionary.Remove((res.OwnerBundleName + res.AssetName).ToLower());
                        res.Recycle2Cache();
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (Platform.IsEditor && Input.GetKey(KeyCode.F1))
            {
                GUILayout.BeginVertical("box");
                
                GUILayout.Label("ResKit", new GUIStyle {fontSize = 30});
                GUILayout.Space(10);
                GUILayout.Label("ResInfo", new GUIStyle {fontSize = 20});
                mResList.ForEach(res => { GUILayout.Label((res as Res).ToString()); });
                GUILayout.Space(10);

                GUILayout.Label("Pools", new GUIStyle() {fontSize = 20});
                GUILayout.Label(string.Format("ResSearchRule:{0}",
                    SafeObjectPool<ResSearchRule>.Instance.CurCount));
                GUILayout.Label(string.Format("ResLoader:{0}", SafeObjectPool<ResLoader>.Instance.CurCount));
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

    }
}