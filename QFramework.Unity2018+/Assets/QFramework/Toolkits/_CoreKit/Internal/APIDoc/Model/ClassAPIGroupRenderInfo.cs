/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Collections.Generic;

namespace QFramework
{
    internal class ClassAPIGroupRenderInfo
    {
        public List<ClassAPIRenderInfo> ClassAPIRenderInfos { get; set; }

        public string GroupName { get; private set; }

        public ClassAPIGroupRenderInfo(string groupName)
        {
            GroupName = groupName;
            Open = new EditorPrefsBoolProperty(groupName);
        }

        public EditorPrefsBoolProperty Open { get; private set; }
    }
}
#endif