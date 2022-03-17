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

        public string GroupName { get; set; }
        public bool Open { get; set; } = true;
    }
}
#endif