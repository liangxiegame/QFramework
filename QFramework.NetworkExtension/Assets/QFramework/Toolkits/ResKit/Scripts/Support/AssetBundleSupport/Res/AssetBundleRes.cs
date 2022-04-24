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


using UnityEngine.Networking;

namespace QFramework
{
    using UnityEngine;
    using System.Collections;
    using System.IO;

    public class AssetBundleRes : Res
    {
        private bool                     mUnloadFlag = true;
        private string[]                 mDependResList;
        private AsyncOperation mAssetBundleCreateRequest;
        public string AESKey = string.Empty;


        public static AssetBundleRes Allocate(string name)
        {
            var res = SafeObjectPool<AssetBundleRes>.Instance.Allocate();

            res.AssetName = name;
            res.AssetType = typeof(AssetBundle);
            res.InitAssetBundleName();
            
            return res;
        }

        private void InitAssetBundleName()
        {
            mDependResList =  AssetBundleSettings.AssetBundleConfigFile.GetAllDependenciesByUrl(AssetName);
        }

        public AssetBundle AssetBundle
        {
            get { return (AssetBundle) mAsset; }
            private set { mAsset = value; }
        }
        
        public override bool LoadSync()
        {
            if (!CheckLoadAble())
            {
                return false;
            }

            State = ResState.Loading;


            if (AssetBundlePathHelper.SimulationMode)
            {
            }
            else
            {
                var url = AssetBundleSettings.AssetBundleName2Url(mAssetName);
                AssetBundle bundle; 
                // var zipFileHelper = ResKit.Architecture.Interface.GetUtility<IZipFileHelper>();

                // if (File.ReadAllText(url).Contains(AES.AESHead))
                // {
                //     if (AESKey == string.Empty)
                //     {
                //         AESKey = JsonUtility.FromJson<EncryptConfig>(Resources.Load<TextAsset>("EncryptConfig").text).AESKey;
                //     }
                //  
                //      bundle= AssetBundle.LoadFromMemory((AES.AESFileByteDecrypt(url, AESKey)));
                //  
                // }
                // else
                // {
                bundle = AssetBundle.LoadFromFile(url);
                // }


                mUnloadFlag = true;

                if (bundle == null)
                {
                    Debug.LogError("Failed Load AssetBundle:" + mAssetName);
                    OnResLoadFaild();
                    return false;
                }

                AssetBundle = bundle;
            }

            State = ResState.Ready;

            return true;
        }

        public override void LoadAsync()
        {
            if (!CheckLoadAble())
            {
                return;
            }

            State = ResState.Loading;

            ResMgr.Instance.PushIEnumeratorTask(this);
        }

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            //开启的时候已经结束了
            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            if (AssetBundlePathHelper.SimulationMode)
            {
                yield return null;
            }
            else
            {
                var url = AssetBundleSettings.AssetBundleName2Url(mAssetName);

                if (PlatformCheck.IsWebGL)
                {
                    var abcR = UnityWebRequestAssetBundle.GetAssetBundle(url);
                    var request = abcR.SendWebRequest();
                    
                    mAssetBundleCreateRequest = request;
                    yield return request;
                    mAssetBundleCreateRequest = null;

                    if (!request.isDone)
                    {
                        Debug.LogError("AssetBundleCreateRequest Not Done! Path:" + mAssetName);
                        OnResLoadFaild();
                        finishCallback();
                        yield break;
                    }
                    
                    var ab = DownloadHandlerAssetBundle.GetContent(abcR);

                    AssetBundle = ab;
                    
                    // 销毁
                    abcR.Dispose();
                }
                else
                {
                    var abcR = AssetBundle.LoadFromFileAsync(url);

                    mAssetBundleCreateRequest = abcR;
                    yield return abcR;
                    mAssetBundleCreateRequest = null;

                    if (!abcR.isDone)
                    {
                        Debug.LogError("AssetBundleCreateRequest Not Done! Path:" + mAssetName);
                        OnResLoadFaild();
                        finishCallback();
                        yield break;
                    }

                    AssetBundle = abcR.assetBundle;
                }
            }

            State = ResState.Ready;
            finishCallback();
        }

        public override string[] GetDependResList()
        {
            return mDependResList;
        }

        public override bool UnloadImage(bool flag)
        {
            if (AssetBundle != null)
            {
                mUnloadFlag = flag;
            }

            return true;
        }

        public override void Recycle2Cache()
        {
            SafeObjectPool<AssetBundleRes>.Instance.Recycle(this);
        }

        public override void OnRecycled()
        {
            base.OnRecycled();
            mUnloadFlag = true;
            mDependResList = null;
        }

        protected override float CalculateProgress()
        {
            if (mAssetBundleCreateRequest == null)
            {
                return 0;
            }

            return mAssetBundleCreateRequest.progress;
        }

        protected override void OnReleaseRes()
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(mUnloadFlag);
                AssetBundle = null;
            }
        }

        public override string ToString()
        {
            return string.Format("Type:AssetBundle\t {0}", base.ToString());
        }
    }
}