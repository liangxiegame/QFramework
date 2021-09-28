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

using System;

namespace QFramework
{
    public class ResSearchKeys : IPoolable,IPoolType
    {   
        public string AssetName { get; set; }

        public string OwnerBundle { get;  set; }

        public Type AssetType { get; set; }

        public string OriginalAssetName { get; set; }
        
        public static ResSearchKeys Allocate(string assetName, string ownerBundleName = null, Type assetType = null)
        {
            var resSearchRule = SafeObjectPool<ResSearchKeys>.Instance.Allocate();
            resSearchRule.AssetName = assetName.ToLower();
            resSearchRule.OwnerBundle = ownerBundleName == null ? null : ownerBundleName.ToLower();
            resSearchRule.AssetType = assetType;
            resSearchRule.OriginalAssetName = assetName;
            return resSearchRule;
        }
        
        // public static ResSearchKeys Allocate(IRes res)
        // {
        //     var resSearchRule = SafeObjectPool<ResSearchKeys>.Instance.Allocate();
        //     res.FillInfo2ResSearchKeys(resSearchRule);
        //     return resSearchRule;
        // }
        
        public void Recycle2Cache()
        {
            SafeObjectPool<ResSearchKeys>.Instance.Recycle(this);
        }

        public bool Match(IRes res)
        {
            if (res.AssetName == AssetName)
            {
                var isMatch = true;

                if (AssetType != null)
                {
                    isMatch = res.AssetType == AssetType;
                }

                if (OwnerBundle != null)
                {
                    isMatch = isMatch && res.OwnerBundleName == OwnerBundle;
                }
                 
                return isMatch;
            }
            

            return false;
        }

        public override string ToString()
        {
            return string.Format("AssetName:{0} BundleName:{1} TypeName:{2}", AssetName, OwnerBundle,
                AssetType);
        }

        void IPoolable.OnRecycled()
        {
            AssetName = null;

            OwnerBundle = null;

            AssetType = null;
        }

        bool IPoolable.IsRecycled { get; set; }
    }
}