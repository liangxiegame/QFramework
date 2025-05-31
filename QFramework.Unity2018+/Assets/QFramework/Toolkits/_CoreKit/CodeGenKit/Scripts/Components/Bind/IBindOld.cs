/****************************************************************************
 * Copyright (c) 2017 ~ 2025 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    public enum BindType
    {
        DefaultUnityElement,
        Element,
        Component
    }
    
    public interface IBindOld : IBind // TODO  UIKit 绑定的时候支持 
    {
        BindType GetBindType();
    }
}