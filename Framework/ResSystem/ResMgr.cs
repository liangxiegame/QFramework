/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

namespace QFramework
{
    using UnityEngine;
    using System.Collections.Generic;

    [QMonoSingletonPath("[Framework]/ResMgr")]
    public class ResMgr : QMonoSingleton<ResMgr>, IEnumeratorTaskMgr
    {
        #region 字段

        private Dictionary<string, IRes> mResDictionary = new Dictionary<string, IRes>();
        private List<IRes> mResList = new List<IRes>();
        [SerializeField] private int mCurrentCoroutineCount = 0;
        private int mMaxCoroutineCount = 8; //最快协成大概在6到8之间
        //private TimeDebugger mTimeDebugger;
        private LinkedList<IEnumeratorTask> mIEnumeratorTaskStack = new LinkedList<IEnumeratorTask>();

        private bool mIsWorking = true;

        //Res 在ResMgr中 删除的问题，ResMgr定时收集列表中的Res然后删除
        private bool mIsResMapDirty = false;

        #endregion


#if UNITY_EDITOR

#endif
        public void InitResMgr()
        {
#if UNITY_EDITOR
            if (AbstractRes.SimulateAssetBundleInEditor)
            {
                AssetBundleExporterForSimulateMode.BuildDataTable();
            }
#endif
            AssetDataTable.Instance.Reset();
            List<string> outResult = new List<string>();
            FileMgr.Instance.GetFileInInner("asset_bindle_config.bin", outResult);
            for (int i = 0; i < outResult.Count; ++i)
            {
                Log.I("Init[ResMgr]: {0}",outResult[i]);
                AssetDataTable.Instance.LoadFromFile(outResult[i]);
            }
            
            AssetDataTable.Instance.SwitchLanguage("cn");
            Log.I("Init[ResMgr]");
        }

        #region 属性

        //public TimeDebugger timeDebugger
        //{
        //    get
        //    {
        //        if (mTimeDebugger == null)
        //        {
        //            mTimeDebugger = new TimeDebugger("#Res");
        //        }
        //        return mTimeDebugger;
        //    }
        //}

        public void SetResMapDirty()
        {
            mIsResMapDirty = true;
        }

        public void PostIEnumeratorTask(IEnumeratorTask task)
        {
            if (task == null)
            {
                return;
            }

            mIEnumeratorTaskStack.AddLast(task);
            TryStartNextIEnumeratorTask();
        }

        public IRes GetRes(string ownerBundleName, string assetName, bool createNew = false)
        {
            IRes res = null;

            if (mResDictionary.TryGetValue((ownerBundleName + assetName).ToLower(), out res))
            {
                return res;
            }

            if (!createNew)
            {
                return null;
            }

            res = ResFactory.Create(assetName, ownerBundleName);

            if (res != null)
            {
                mResDictionary.Add((ownerBundleName + assetName).ToLower(), res);
                if (!mResList.Contains(res))
                {
                    mResList.Add(res);
                }
            }
            return res;
        }

        public IRes GetRes(string assetName, bool createNew = false)
        {
            IRes res = null;
            if (mResDictionary.TryGetValue(assetName, out res))
            {
                return res;
            }

            if (!createNew)
            {
                Log.I("createNew:{0}", createNew);
                return null;
            }

            res = ResFactory.Create(assetName);

            if (res != null)
            {
                mResDictionary.Add(assetName, res);
                mResList.Add(res);
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

        public R GetAsset<R>(string name) where R : UnityEngine.Object
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
            if (mIsWorking)
            {
                if (mIsResMapDirty)
                {
                    RemoveUnusedRes();
                }
            }
        }

        private void RemoveUnusedRes()
        {
            if (!mIsResMapDirty)
            {
                return;
            }

            mIsResMapDirty = false;

            IRes res = null;
            for (int i = mResList.Count - 1; i >= 0; --i)
            {
                res = mResList[i];
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

            IEnumeratorTask task = mIEnumeratorTaskStack.First.Value;
            mIEnumeratorTaskStack.RemoveFirst();

            ++mCurrentCoroutineCount;
            StartCoroutine(task.StartIEnumeratorTask(OnIEnumeratorTaskFinish));
        }

        #endregion
    }
}