/****************************************************************************
 * Copyright (c) 2016 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    using System;
        
    public interface IResLoader 
    {
        IRes LoadResSync(ResSearchKeys resSearchKeys);
        UnityEngine.Object LoadAssetSync(ResSearchKeys resSearchKeys);
        
        void Add2Load(ResSearchKeys resSearchKeys, Action<bool, IRes> listener = null, bool lastOrder = true);
        void LoadAsync(System.Action listener = null);
        
        void ReleaseAllRes();
        void UnloadAllInstantiateRes(bool flag);
    }
}
