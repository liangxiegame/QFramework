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
        
        string className = $"{type.Name}Adapter";
        string fileName = Path.Combine(outPath, $"{className}.cs");
        //如果有先删除
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
            if (File.Exists($"{fileName}.meta"))
            {
                File.Delete($"{fileName}.meta");
            }
        }
        
        //生成适配器
        Stopwatch watch = new Stopwatch();
        string content =
            ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(type, "Adapter");
        File.WriteAllText(fileName, content);
        watch.Stop();
        Log.I($"Generated {fileName} in: " + watch.ElapsedMilliseconds + " ms.");
        UnityEngine.Debug.Log("CreateAdapter Ok");
        AssetDatabase.Refresh();
    }
}

