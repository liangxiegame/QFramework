using System.Collections.Generic;
using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

public static class LuaProfiler
{
    public static List<string> list = new List<string>();

    public static void Clear()
    {
        list.Clear();
    }

    public static int GetID(string name)
    {
        int id = list.Count;
        list.Add(name);
        return id;
    }

    public static void BeginSample(int id)
    {
        string name = list[id];
        Profiler.BeginSample(name);
    }

    public static void EndSample()
    {
        Profiler.EndSample();
    }
}
