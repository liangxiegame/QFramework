/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2024 liangxie UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    public interface IBinaryHeapElement
    {
        float SortScore { get; }

        int HeapIndex { set; }
    }
}