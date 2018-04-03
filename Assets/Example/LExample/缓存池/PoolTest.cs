using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class PoolTest :IPoolable
{
    public static int Index=0;
    private int CurrentIndex;

    public PoolTest()
    {
        Index++;
        CurrentIndex = Index;
        Debug.Log("创建");
    }

    public void DebugIndex()
    {
        Debug.Log("当前"+ CurrentIndex);
    }

    public void OnRecycled()
    {
        CurrentIndex = 0;
    }

    private bool isrecyscled;
    public bool IsRecycled { get { return isrecyscled; } set { isrecyscled = value; } }

}
