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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public static class ResFactory
    {
        public static IRes Create(ResSearchKeys resSearchKeys)
        {
            var retRes = mResCreators
                .Where(creator => creator.Match(resSearchKeys))
                .Select(creator => creator.Create(resSearchKeys))
                .FirstOrDefault();

            if (retRes == null)
            {
                Log.E("Failed to Create Res. Not Find By ResSearchKeys:" + resSearchKeys);
                return null;
            }

            return retRes;
        }

        public static void AddResCreator<T>() where T : IResCreator, new()
        {
            mResCreators.Add(new T());
        }

        public static void RemoveResCreator<T>() where T : IResCreator, new()
        {
            mResCreators.RemoveAll(r => r.GetType() == typeof(T));
        }

        public static void AddResCreator(IResCreator resCreator)
        {
            mResCreators.Add(resCreator);
        }

        static List<IResCreator> mResCreators = new List<IResCreator>()
        {
            new ResourcesResCreator(),
            new AssetBundleResCreator(),
            new AssetResCreator(),
            new SceneResCreator(),
            new NetImageResCreator(),
            new LocalImageResCreator()
        };

    }

    public interface IResCreator
    {
        bool Match(ResSearchKeys resSearchKeys);
        IRes Create(ResSearchKeys resSearchKeys);
    }

    public class ResourcesResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.AssetName.StartsWith("resources/") ||
                   resSearchKeys.AssetName.StartsWith("resources://");
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            var resourcesRes = ResourcesRes.Allocate(resSearchKeys.AssetName,
                resSearchKeys.AssetName.StartsWith("resources://")
                    ? InternalResNamePrefixType.Url
                    : InternalResNamePrefixType.Folder);
            resourcesRes.AssetType = resSearchKeys.AssetType;
            return resourcesRes;
        }
    }

    public class AssetBundleResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.AssetType == typeof(AssetBundle);
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return AssetBundleRes.Allocate(resSearchKeys.AssetName);
        }
    }

    public class AssetResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            var assetData = ResKit.ResDatas.GetAssetData(resSearchKeys);

            if (assetData != null)
            {
                return assetData.AssetType == ResType.ABAsset;
            }

            return false;
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return AssetRes.Allocate(resSearchKeys.AssetName, resSearchKeys.OwnerBundle, resSearchKeys.AssetType);
        }
    }

    public class SceneResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            var assetData = ResKit.ResDatas.GetAssetData(resSearchKeys);

            if (assetData != null)
            {
                return assetData.AssetType == ResType.ABScene;
            }

            return false;
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return SceneRes.Allocate(resSearchKeys.AssetName);
        }
    }

    public class NetImageResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.AssetName.StartsWith("netimage:");
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return NetImageRes.Allocate(resSearchKeys.AssetName);
        }
    }

    public class LocalImageResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.AssetName.StartsWith("localimage:");
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return NetImageRes.Allocate(resSearchKeys.AssetName);
        }
    }
}