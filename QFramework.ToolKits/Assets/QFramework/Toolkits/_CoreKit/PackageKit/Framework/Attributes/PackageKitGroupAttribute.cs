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
    public class PackageKitGroupAttribute : Attribute
    {
        public string GroupName { get; set; }

        public PackageKitGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}