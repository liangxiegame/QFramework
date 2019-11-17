/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * Copyright (c) 2018.3 liangxie
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
    using System;

    /// <summary>
    /// maybe assetbundle,asset
    /// </summary>
    [Serializable]
    public class AssetData
    {
        private string mAssetName;
        private string mOwnerBundleName;
        private int    mAbIndex;
        private short  mAssetType;

        public string AssetName
        {
            get { return mAssetName; }
            set { mAssetName = value; }
        }

        public int AssetBundleIndex
        {
            get { return mAbIndex; }
            set { mAbIndex = value; }
        }

        public string OwnerBundleName
        {
            get { return mOwnerBundleName; }
            set { mOwnerBundleName = value; }
        }

        public string UUID
        {
            get
            {
                return string.IsNullOrEmpty(mOwnerBundleName)
                    ? AssetName
                    : OwnerBundleName + AssetName;
            }
        }

        public short AssetType
        {
            get { return mAssetType; }
            set { mAssetType = value; }
        }

        public AssetData(string assetName, short assetType, int abIndex,string ownerBundleName)
        {
            mAssetName = assetName;
            mAssetType = assetType;
            mAbIndex = abIndex;
            mOwnerBundleName = ownerBundleName;
        }

        public AssetData(){}
    }
}