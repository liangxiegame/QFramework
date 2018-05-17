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

namespace QFramework
{
    public static class ResFactory
    {
        static ResFactory()
        {
            Log.I("Init[ResFactory]");
            SafeObjectPool<AssetBundleRes>.Instance.MaxCacheCount = 20;
            SafeObjectPool<AssetRes>.Instance.MaxCacheCount = 40;
            SafeObjectPool<InternalRes>.Instance.MaxCacheCount = 40;
            SafeObjectPool<NetImageRes>.Instance.MaxCacheCount = 20;
        }
        
        public static IRes Create(string assetName,string ownerBundleName)
        {
            short assetType = 0;
            if (assetName.StartsWith("Resources/"))
            {
                assetType = ResType.Internal;
            }
            else if (assetName.StartsWith("NetImage:"))
            {
                assetType = ResType.NetImageRes;
            }
            else
            {
                AssetData data = ResDatas.Instance.GetAssetData(assetName,ownerBundleName);
                if (data == null)
                {
                    Log.E("Failed to Create Res. Not Find AssetData:" + ownerBundleName + assetName );
                    return null;
                }
                else
                {
                    assetType = data.AssetType;
                }
            }

            return Create(assetName,ownerBundleName,assetType);
        }
        
        public static IRes Create(string assetName,string ownerBundleName, short assetType)
        {
            switch (assetType)
            {
                case ResType.AssetBundle:
                    return AssetBundleRes.Allocate(assetName);
                case ResType.ABAsset:
                    return AssetRes.Allocate(assetName,ownerBundleName);
                case ResType.ABScene:
                    return SceneRes.Allocate(assetName);
                case ResType.Internal:
                    return InternalRes.Allocate(assetName);
                case ResType.NetImageRes:
                    return NetImageRes.Allocate(assetName);
                case ResType.LocalImageRes:
                    return LocalImageRes.Allocate(assetName);
                default:
                    Log.E("Invalid assetType :" + assetType);
                    return null;
            }
        }

        public static IRes Create(string assetName)
        {
            short assetType = 0;
            if (assetName.StartsWith("Resources/"))
            {
                assetType = ResType.Internal;
            }
            else if (assetName.StartsWith("NetImage:"))
            {
                assetType = ResType.NetImageRes;
            }
            else if (assetName.StartsWith("LocalImage:"))
            {
                assetType = ResType.LocalImageRes;
            }
            else
            {
                var data = ResDatas.Instance.GetAssetData(assetName);
                if (data == null)
                {
                    Log.E("Failed to Create Res. Not Find AssetData:" + assetName);
                    return null;
                }

                assetType = data.AssetType;
            }

            return Create(assetName, assetType);
        }

        private static IRes Create(string assetName, short assetType)
        {
            switch (assetType)
            {
                case ResType.AssetBundle:
                    return AssetBundleRes.Allocate(assetName);
                case ResType.ABAsset:
                    return AssetRes.Allocate(assetName);
                case ResType.ABScene:
                    return SceneRes.Allocate(assetName);
                case ResType.Internal:
                    return InternalRes.Allocate(assetName);
                case ResType.NetImageRes:
                    return NetImageRes.Allocate(assetName);
                case ResType.LocalImageRes:
                    return LocalImageRes.Allocate(assetName);
                default:
                    Log.E("Invalid assetType :" + assetType);
                    return null;
            }
        }
    }
}
