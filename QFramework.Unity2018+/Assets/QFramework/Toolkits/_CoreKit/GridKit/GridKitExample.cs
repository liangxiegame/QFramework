using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class GridKitExample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var grid = new EasyGrid<string>(4, 4);

            grid.Fill("这是空");
            
            grid[2, 3] = "@@@ 你好呀 @@@";

            grid.ForEach((x, y, content) => Debug.Log($"({x},{y}):{content}"));

            grid.Clear();
        }

    }
}
