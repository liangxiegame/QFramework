/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QF.Res
{
    using System;
        
    public interface IResLoader : IPoolable,IPoolType
    {
        void Add2Load(string assetName, Action<bool, IRes> listener, bool lastOrder = true);
        void Add2Load(string ownerBundleName, string assetName, Action<bool, IRes> listener, bool lastOrder = true);
        
        void ReleaseAllRes();
        void UnloadAllInstantiateRes(bool flag);
    }
}
