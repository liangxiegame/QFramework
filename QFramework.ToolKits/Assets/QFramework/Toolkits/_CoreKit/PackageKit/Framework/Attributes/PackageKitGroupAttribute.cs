/****************************************************************************
 * Copyright (c) 2017 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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