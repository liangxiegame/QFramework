

namespace QFramework
{
        
    //策略描述:加载完AB后 立即加载AB所有Asset
    public class UILoaderStrategy : QSingleton<UILoaderStrategy>, IResLoaderStrategy
    {
        public void OnAllTaskFinish(IResLoader loader)
        {
            //Log.W("#OnAllTaskFinish: Will Unload AB Image.");
        }

        public void OnAsyncLoadFinish(IResLoader loader, AssetRes res)
        {
            //Log.W("OnAsyncLoadFinish:AssetRes:" + res.name);
        }

        public void OnAsyncLoadFinish(IResLoader loader, AssetBundleRes res)
        {
            //Log.W("OnAsyncLoadFinish:AssetBundleRes:" + res.name);
            if (res.assetBundle == null)
            {
                return;
            }

            string[] assetNames = res.assetBundle.GetAllAssetNames();

            if (assetNames == null)
            {
                return;
            }

            for (int i = assetNames.Length - 1; i >= 0; --i)
            {
                loader.Add2Load(PathHelper.FullAssetPath2Name(assetNames[i]), null, false);
            }
        }

        public void OnAsyncLoadFinish(IResLoader loader, InternalRes res)
        {
            //Log.W("OnAsyncLoadFinish:InternalRes:" + res.name);
        }

        public void OnAsyncLoadFinish(IResLoader loader, IRes res)
        {
            //Log.W("OnAsyncLoadFinish:IRes:" + res.name);
        }

        public void OnSyncLoadFinish(IResLoader loader, InternalRes res)
        {
            //Log.W("OnSyncLoadFinish:InternalRes:" + res.name);
        }

        public void OnSyncLoadFinish(IResLoader loader, AssetRes res)
        {
            //Log.W("OnSyncLoadFinish:AssetRes:" + res.name);
        }

        public void OnSyncLoadFinish(IResLoader loader, AssetBundleRes res)
        {
            //Log.W("OnSyncLoadFinish:AssetBundleRes:" + res.name);
            if (res.assetBundle == null)
            {
                return;
            }

            string[] assetNames = res.assetBundle.GetAllAssetNames();

            if (assetNames == null)
            {
                return;
            }

            for (int i = assetNames.Length - 1; i >= 0; --i)
            {
                loader.Add2Load(PathHelper.FullAssetPath2Name(assetNames[i]), null, false);
            }
        }

        public void OnSyncLoadFinish(IResLoader loader, IRes res)
        {
            //Log.W("OnSyncLoadFinish:IRes:" + res.name);
        }

    }
}
