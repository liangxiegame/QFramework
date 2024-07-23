/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassAPIAttribute : Attribute
    {
        public string DisplayMenuName { get; private set; }
        public string GroupName { get; private set; }
        
        public int RenderOrder { get;private set; }
        
        public string DisplayClassName { get; private set; }

        public ClassAPIAttribute(string groupName, string displayMenuName,int renderOrder,string displayClassName = null)
        {
            GroupName = groupName;
            DisplayMenuName = displayMenuName;
            RenderOrder = renderOrder;
            DisplayClassName = displayClassName;
        }
    }
}