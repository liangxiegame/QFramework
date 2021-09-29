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

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

namespace QFramework
{
    using System;
    
    public static partial class ObjectAssetTypeCode
    {
        public const short GameObject  = 1;
        public const short AudioClip   = 2;
        public const short Sprite      = 3;
        public const short Scene       = 4;
        public const short SpriteAtlas = 5;
        public const short Mesh        = 6;
        public const short Texture2D   = 7;
        public const short TextAsset   = 8;
        public const short AssetBundle   = 8;

        public static Type GameObjectType  = typeof(GameObject);
        public static Type AudioClipType   = typeof(AudioClip);
        public static Type SpriteType      = typeof(Sprite);
        public static Type SceneType       = typeof(Scene);
        public static Type SpriteAtlasType = typeof(SpriteAtlas);
        public static Type MeshType        = typeof(Mesh);
        public static Type Texture2DType   = typeof(Texture2D);
        public static Type TextAssetType   = typeof(TextAsset);
        public static Type AssetBundleType   = typeof(AssetBundle);

        public static short ToCode(this Type type)
        {
            if (type == GameObjectType)
            {
                return GameObject;
            }

            if (type == AudioClipType)
            {
                return AudioClip;
            }

            if (type == SpriteType)
            {
                return Sprite;
            }

            if (type == SceneType)
            {
                return Scene;
            }

            if (type == SpriteAtlasType)
            {
                return SpriteAtlas;
            }

            if (type == MeshType)
            {
                return Mesh;
            }

            if (type == Texture2DType)
            {
                return Texture2D;
            }

            if (type == TextAssetType)
            {
                return TextAsset;
            }

            if (type == AssetBundleType)
            {
                return AssetBundle;
            }

            return 0;
        }

        public static Type ToType(this short code)
        {
            if (code == GameObject)
            {
                return GameObjectType;
            }

            if (code == AudioClip)
            {
                return AudioClipType;
            }

            if (code == Sprite)
            {
                return SpriteType;
            }

            if (code == Scene)
            {
                return SceneType;
            }

            if (code == SpriteAtlas)
            {
                return SpriteAtlasType;
            }

            if (code == Mesh)
            {
                return MeshType;
            }

            if (code == Texture2D)
            {
                return Texture2DType;
            }

            if (code == TextAsset)
            {
                return TextAssetType;
            }

            if (code == AssetBundle)
            {
                return AssetBundleType;
            }

            return null;
        }
    }

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
        private short  mAssetObjectTypeCode = 0;

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

        public short AssetObjectTypeCode
        {
            get { return mAssetObjectTypeCode; }
            set { mAssetObjectTypeCode = value; }
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

        public AssetData(string assetName, short assetType, int abIndex, string ownerBundleName,
            short assetObjectTypeCode = 0)
        {
            mAssetName = assetName;
            mAssetType = assetType;
            mAbIndex = abIndex;
            mOwnerBundleName = ownerBundleName;
            mAssetObjectTypeCode = assetObjectTypeCode;
        }

        public AssetData()
        {
        }
    }
}