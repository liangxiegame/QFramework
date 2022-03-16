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
    public class ClassAPIAttribute : Attribute
    {
        public string DisplayName { get; private set; }
        public string GroupName { get; private set; }
        
        public int RenderOrder { get;private set; }

        public ClassAPIAttribute(string groupName, string displayName,int renderOrder)
        {
            GroupName = groupName;
            DisplayName = displayName;
            RenderOrder = renderOrder;
        }
    }
}