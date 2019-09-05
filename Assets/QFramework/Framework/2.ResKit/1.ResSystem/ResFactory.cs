/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2019.1 liangxie
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
    public static class ResFactory
    {
        public static IRes Create(ResSearchRule resSearchRule)
        {
            var lowerAssetName = resSearchRule.AssetName.ToLower();
            
            short assetType = 0;
            if (lowerAssetName.StartsWith("resources/") || lowerAssetName.StartsWith("resources://"))
            {
                assetType = ResType.Internal;
            }
            else if (lowerAssetName.StartsWith("netimage:"))
            {
                assetType = ResType.NetImageRes;
            }
            else if (lowerAssetName.StartsWith("localimage:"))
            {
                assetType = ResType.LocalImageRes;
            }
            else
            {
                var data = ResDatas.Instance.GetAssetData(resSearchRule);

                if (data == null)
                {
                    Log.E("Failed to Create Res. Not Find AssetData:" + resSearchRule);
                    return null;
                }

                assetType = data.AssetType;
            }

            return Create(resSearchRule, assetType);
        }

        public static IRes Create(ResSearchRule resSearchRule, short assetType)
        {
            switch (assetType)
            {
                case ResType.AssetBundle:
                    return AssetBundleRes.Allocate(resSearchRule.AssetName);
                case ResType.ABAsset:
                    return AssetRes.Allocate(resSearchRule.AssetName, resSearchRule.OwnerBundle);
                case ResType.ABScene:
                    return SceneRes.Allocate(resSearchRule.AssetName);
                case ResType.Internal:
                    return ResourcesRes.Allocate(resSearchRule.AssetName,
                        resSearchRule.AssetName.StartsWith("resources://")
                            ? InternalResNamePrefixType.Url
                            : InternalResNamePrefixType.Folder);
                case ResType.NetImageRes:
                    return NetImageRes.Allocate(resSearchRule.AssetName);
                case ResType.LocalImageRes:
                    return LocalImageRes.Allocate(resSearchRule.AssetName);
                default:
                    Log.E("Invalid assetType :" + assetType);
                    return null;
            }
        }
    }
}