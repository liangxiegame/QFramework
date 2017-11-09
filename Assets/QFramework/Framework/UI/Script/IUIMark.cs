/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
    using UnityEngine;

    public interface IUIMark
    {
        string ComponentName { get; }
        
        Transform Transform { get; }
    }
}