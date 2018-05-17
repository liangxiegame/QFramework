/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
    using System;
        
    public interface IResLoader
    {
        void Add2Load(string assetName, Action<bool, IRes> listener, bool lastOrder = true);
        void ReleaseAllRes();
        void UnloadAllInstantialteRes(bool flag);
    }
}
