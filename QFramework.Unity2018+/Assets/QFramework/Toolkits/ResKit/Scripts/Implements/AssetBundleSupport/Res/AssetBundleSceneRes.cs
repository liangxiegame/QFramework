/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2022 liangxie UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


namespace QFramework
{
        
    public class AssetBundleSceneRes : AssetRes
    {
        public static AssetBundleSceneRes Allocate(string name)
        {
            AssetBundleSceneRes res = SafeObjectPool<AssetBundleSceneRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;
                res.InitAssetBundleName();
            }
            return res;
        }

        public AssetBundleSceneRes(string assetName) : base(assetName)
        {

        }

        public AssetBundleSceneRes()
        {

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

            var resSearchKeys = ResSearchKeys.Allocate(AssetBundleName);
            
            var abR = ResMgr.Instance.GetRes<AssetBundleRes>(resSearchKeys);

            resSearchKeys.Recycle2Cache();

            if (abR == null || abR.AssetBundle == null)
            {
              
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
            SafeObjectPool<AssetBundleSceneRes>.Instance.Recycle(this);
        }
    }
}