/****************************************************************************
 * Copyright (c) 2016 ~ 2024 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("11.GridKit", "DynaGrid", 1, "DynaGrid")]
    [APIDescriptionCN("动态 Grid 数据结构")]
    [APIDescriptionEN("Dynamic Grid DataStructure")]
    [APIExampleCode(@"
using UnityEngine;

namespace QFramework.Example
{
    public class DynaGridExample : MonoBehaviour
    {
        public class MyData
        {
            public string Key;
        }

        void Start()
        {
            var dynaGrid = new DynaGrid<MyData>();
            dynaGrid[1, 1] = new MyData() { Key = ""Hero"" };
            dynaGrid[-1, -10] = new MyData() { Key = ""Enemy"" };

            dynaGrid.ForEach((x, y, data) => { Debug.Log($""{x} {y} {data.Key}""); });
        }
    }
}

// 1 1 Hero
// -1 -10 Enemy
")]
#endif
    public class DynaGrid<T>
    {
        private Dictionary<Tuple<int, int>, T> mGrid = null;

        public DynaGrid()
        {
            mGrid = new Dictionary<Tuple<int, int>, T>();
        }

        public void ForEach(Action<int, int, T> each)
        {
            foreach (var kvp in mGrid)
            {
                each(kvp.Key.Item1, kvp.Key.Item2, kvp.Value);
            }
        }

        public void ForEach(Action<T> each)
        {
            foreach (var kvp in mGrid)
            {
                each(kvp.Value);
            }
        }
        
        public T this[int xIndex, int yIndex]
        {
            get
            {
                var key = new Tuple<int, int>(xIndex, yIndex);
                return mGrid.TryGetValue(key, out var value) ? value : default;
            }
            set
            {
                var key = new Tuple<int, int>(xIndex, yIndex);
                mGrid[key] = value;
            }
        }

        public void Clear(Action<T> cleanupItem = null)
        {
            mGrid.Clear();
        }
    }
}