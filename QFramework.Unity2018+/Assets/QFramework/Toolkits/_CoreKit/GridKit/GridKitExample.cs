using UnityEngine;

namespace QFramework.Example
{
    public class GridKitExample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var grid = new EasyGrid<string>(4, 4);

            grid.Fill("Empty");
            
            grid[2, 3] = "@@@ Hello @@@";

            grid.ForEach((x, y, content) => Debug.Log($"({x},{y}):{content}"));

            grid.Clear();
        }

    }
}
