/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
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

namespace QF.Res
{
        
    public class SceneRes : AssetRes
    {
        public static SceneRes Allocate(string name)
        {
            SceneRes res = SafeObjectPool<SceneRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;
                res.InitAssetBundleName();
            }
            return res;
        }

        public SceneRes(string assetName) : base(assetName)
        {

        }

        public SceneRes()
        {

        }

        public override void AcceptLoaderStrategySync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnSyncLoadFinish(loader, this);
        }

        public override void AcceptLoaderStrategyAsync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnAsyncLoadFinish(loader, this);
        }

        public override bool LoadSync()
        {
            if (!CheckLoadAble())
            {
                return false;
            }

            if (string.IsNullOrEmpty(AssetBundleName))
            {
                return false;
            }

            var abR = ResMgr.Instance.GetRes<AssetBundleRes>(AssetBundleName);

            if (abR == null || abR.AssetBundle == null)
            {
                Log.E("Failed to Load Asset, Not Find AssetBundleImage:" + abR);
                return false;
            }


            State = ResState.Ready;
            return true;
        }

        public override void LoadAsync()
        {
            LoadSync();
        }


        public override void Recycle2Cache()
        {
            SafeObjectPool<SceneRes>.Instance.Recycle(this);
        }
    }
}