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

namespace QFramework
{
    using System;

    /// <summary>
    /// maybe assetbundle,asset
    /// </summary>
    [Serializable]
    public class AssetData
    {

        /// <summary>
        /// 资源名字
        /// </summary>
        public readonly string AssetName;

        /// <summary>
        /// 资源类型
        /// </summary>
        public readonly short AssetType;

        /// <summary>
        /// 资源索引 
        /// </summary>
        public readonly int AssetBundleIndex;

        /// <summary>
        /// 所属的 Bundle 名字
        /// </summary>
        public readonly string OwnerBundleName;

        public string UUID
        {
            get
            {
                return string.IsNullOrEmpty(OwnerBundleName)
                    ? AssetName.ToLower()
                    : OwnerBundleName.ToLower() + AssetName.ToLower();
            }
        }

        public AssetData()
        {
        }

        public AssetData(string assetName, short assetType, int abIndex, string ownerBundleName)
        {
            AssetName = assetName;
            AssetType = assetType;
            AssetBundleIndex = abIndex;
            OwnerBundleName = ownerBundleName;
        }
    }
}