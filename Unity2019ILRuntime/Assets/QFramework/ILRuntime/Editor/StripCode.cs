using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Tool
{

    public static class StripCode
    {

        public static void GenLinkXml()
        {

           var assemblies = new List<Assembly>()
           {
               //这里后面unity dll分了很多dll，待考证是不是只添加几个
               typeof(UnityEngine.Object).Assembly,
               typeof(UnityEngine.Transform).Assembly,
               typeof(UnityEngine.GameObject).Assembly,
               typeof(UnityEngine.UI.Button).Assembly,
               //添加一个系统的类
               typeof(System.Object).Assembly,            
           };
            
           //扫描当前项目引用的.dll
            var files = Directory.GetFiles(Application.dataPath, "*.dll", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                if (f.Contains("Editor")|| f.Contains("editor")
                    ||f.Contains("Plugins") )
                {
                    continue;
                }

                try
                {
                    assemblies.Add(Assembly.LoadFile(f));
                }
                catch (Exception)
                {
                    Debug.LogError("加载失败:" + f);
                    throw;
                }

            }
            
            //打完收工
            assemblies =  assemblies.Distinct().ToList();
            
            
            //生成xml
            var xmlDoc = new XmlDocument();
            var rootElement = xmlDoc.CreateElement("linker");
            
            foreach (var assm in assemblies)
            {
                var assmElement = xmlDoc.CreateElement("assembly");
                assmElement.SetAttribute("fullname", assm.GetName().Name);
                var types = assm.GetTypes();
                foreach (var t in types)
                {
                    if (t.FullName.Equals("Win32"))
                    {
                        continue;
                    }
                    var typeElement = xmlDoc.CreateElement("type");
                    typeElement.SetAttribute("fullname", t.FullName);
                    typeElement.SetAttribute("preserve", "all");
                    //添加type节点
                    assmElement.AppendChild(typeElement);
                }

                rootElement.AppendChild(assmElement);
            }

            xmlDoc.AppendChild(rootElement);

            var path = Path.Combine(Application.dataPath, "link.xml");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            xmlDoc.Save(path);

            EditorUtility.DisplayDialog("提示", "生成完毕!", "OK");
        }
    }
}
