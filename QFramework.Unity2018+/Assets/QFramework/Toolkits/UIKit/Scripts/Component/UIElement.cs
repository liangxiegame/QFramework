/****************************************************************************
 * Copyright (c) 2017 ~ 2025 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    using UnityEngine;
    
    public abstract class UIElement : QMonoBehaviour,IBindOld
    {
        public virtual BindType GetBindType() => BindType.Element;

        public virtual string TypeName => ComponentName;
        public abstract string ComponentName { get; }

        public string Comment => string.Empty;

        public Transform Transform => transform;

        public override IManager Manager => UIManager.Instance;
    }
}