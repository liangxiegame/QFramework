/****************************************************************************
 * Copyright (c) 2022 ~ 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("11.GridKit", "EasyGrid", 0, "EasyGrid")]
    [APIDescriptionCN("Grid 数据结构")]
    [APIDescriptionEN("Grid DataStructure")]
    [APIExampleCode(@"
using UnityEngine;

namespace QFramework.Example
{
    public class GridKitExample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var grid = new EasyGrid<string>(4, 4);

            grid.Fill(""Empty"");
            
            grid[2, 3] = ""Hello"";

            grid.Resize(5, 5, (x, y) => ""123"");

            grid.ForEach((x, y, content) => Debug.Log($""({x},{y}):{content}""));
            
            grid.Clear();
        }
    }
}
(0,0):Empty
(0,1):Empty
(0,2):Empty
(0,3):Empty
(1,0):Empty
(1,1):Empty
(1,2):Empty
(1,3):Empty
(2,0):Empty
(2,1):Empty
(2,2):Empty
(2,3):Hello
(3,0):Empty
(3,1):Empty
(3,2):Empty
(3,3):Empty
(0,0):Empty
(0,1):Empty
(0,2):Empty
(0,3):Empty
(0,4):123
(1,0):Empty
(1,1):Empty
(1,2):Empty
(1,3):Empty
(1,4):123
(2,0):Empty
(2,1):Empty
(2,2):Empty
(2,3):Hello
(2,4):123
(3,0):Empty
(3,1):Empty
(3,2):Empty
(3,3):Empty
(3,4):123
(4,0):123
(4,1):123
(4,2):123
(4,3):123
(4,4):123
")]
#endif
    public class EasyGrid<T>
    {
        private T[,] mGrid;
        private int mWidth;
        private int mHeight;

        public int Width => mWidth;
        public int Height => mHeight;

        public EasyGrid(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            mGrid = new T[width, height];
        }

        public void Fill(T t)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    mGrid[x, y] = t;
                }
            }
        }

        public void Fill(Func<int, int, T> onFill)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    mGrid[x, y] = onFill(x, y);
                }
            }
        }

        public void Resize(int width, int height, Func<int, int, T> onAdd)
        {
            var newGrid = new T[width, height];
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    newGrid[x, y] = mGrid[x, y];
                }

                // x addition
                for (var y = mHeight; y < height; y++)
                {
                    newGrid[x, y] = onAdd(x, y);
                }
            }

            for (var x = mWidth;  x < width; x++)
            {
                // y addition
                for (var y = 0; y < height; y++)
                {
                    newGrid[x, y] = onAdd(x, y);
                }
            }

            // 清空之前的
            Fill(default(T));

            mWidth = width;
            mHeight = height;
            mGrid = newGrid;
        }

        public void ForEach(Action<int, int, T> each)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    each(x, y, mGrid[x, y]);
                }
            }
        }

        public void ForEach(Action<T> each)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    each(mGrid[x, y]);
                }
            }
        }

        public T this[int xIndex, int yIndex]
        {
            get
            {
                if (xIndex >= 0 && xIndex < mWidth && yIndex >= 0 && yIndex < mHeight)
                {
                    return mGrid[xIndex, yIndex];
                }
                else
                {
                    Debug.LogWarning($"out of bounds [{xIndex}:{yIndex}] in grid[{mWidth}:{mHeight}]");
                    return default;
                }
            }
            set
            {
                if (xIndex >= 0 && xIndex < mWidth && yIndex >= 0 && yIndex < mHeight)
                {
                    mGrid[xIndex, yIndex] = value;
                }
                else
                {
                    Debug.LogWarning($"out of bounds [{xIndex}:{yIndex}] in grid[{mWidth}:{mHeight}]");
                }
            }
        }

        public void Clear(Action<T> cleanupItem = null)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    cleanupItem?.Invoke(mGrid[x, y]);
                    mGrid[x, y] = default;
                }
            }

            mGrid = null;
        }
    }
}