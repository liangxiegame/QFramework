/*
* Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at 
*  
* Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. 
*/
using System;
using System.Diagnostics;
using System.IO;
using QFramework;
using UnityEditor;

public static class GenAdapter
{

    public static void CreateAdapter(Type type ,string outPath)
    {
        if (!Directory.Exists(outPath))
        {
            Directory.CreateDirectory(outPath);
        }

        string typeName = type.Name;
        //如果有先删除
        if (File.Exists($"{outPath}/{typeName}Adapter.cs"))
        {
            File.Delete($"{outPath}/{typeName}Adapter.cs");
            if (File.Exists($"{outPath}/{typeName}Adapter.cs.meta"))
            {
                File.Delete($"{outPath}/{typeName}Adapter.cs.meta");
            }
        }

        //生成适配器
        FileStream stream = new FileStream($"{outPath}/{typeName}Adapter.cs", FileMode.Append, FileAccess.Write);
        StreamWriter sw = new StreamWriter(stream);
        Stopwatch watch = new Stopwatch();
        sw.WriteLine(
            ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(type, "Adapter"));
        watch.Stop();
        Log.I($"Generated {outPath}/{typeName}Adapter.cs in: " + watch.ElapsedMilliseconds + " ms.");
        sw.Dispose();
        UnityEngine.Debug.Log("CreateAdapter Ok");
        AssetDatabase.Refresh();
    }


    private static void GenAdapterFile(Type t, string dir)
    {
        var path = Path.Combine(dir, t.Name + "Adapter.cs");
    }

}

