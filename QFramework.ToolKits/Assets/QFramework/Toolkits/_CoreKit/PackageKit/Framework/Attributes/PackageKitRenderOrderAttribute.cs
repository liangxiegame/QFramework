/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/
using System;

namespace QFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PackageKitRenderOrderAttribute : Attribute
    {
        public int Order { get; private set; }
        
        public PackageKitRenderOrderAttribute(int order)
        {
            Order = order;
        }
    }
}