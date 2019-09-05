using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QF.Extensions;

namespace QFramework
{
    public class ViewControllerDesignerTemplate
    {
        public static void Write(string name,string scriptsFolder, PanelCodeInfo panelCodeInfo,UIKitSettingData uiKitSettingData)
        {
            
            var scriptFile = scriptsFolder + "/{0}.Designer.cs".FillFormat(name);
            var writer = File.CreateText(scriptFile);
            
            writer.WriteLine("// Generate Id:{0}".FillFormat(Guid.NewGuid().ToString()));
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine();

            if (uiKitSettingData.IsDefaultNamespace)
            {
                writer.WriteLine("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间");
                writer.WriteLine("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
            }

            writer.WriteLine("namespace {0}".FillFormat(uiKitSettingData.Namespace));
            writer.WriteLine("{");
            writer.WriteLine("\tpublic partial class {0}".FillFormat(name));
            writer.WriteLine("\t{");

            foreach (var bindInfo in panelCodeInfo.BindInfos)
            {
                writer.WriteLine("\t\tpublic {0} {1};".FillFormat(bindInfo.BindScript.ComponentName,bindInfo.Name));
            }

            writer.WriteLine();
            writer.WriteLine("\t}");
            writer.WriteLine("}");
            
            writer.Close();

        }
    }
    
    public class ViewControllerTemplate
    {
        public static void Write(string name,string scriptsFolder,UIKitSettingData uiKitSettingData)
        {
            
            var scriptFile = scriptsFolder + "/{0}.cs".FillFormat(name);

            if (File.Exists(scriptFile))
            {
                return;
            }
            
            var writer = File.CreateText(scriptFile);

            writer.WriteLine("using UnityEngine;");
            writer.WriteLine("using QFramework;");
            writer.WriteLine();

            if (uiKitSettingData.IsDefaultNamespace)
            {
                writer.WriteLine("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间");
                writer.WriteLine("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
            }

            writer.WriteLine("namespace {0}".FillFormat(uiKitSettingData.Namespace));
            writer.WriteLine("{");
            writer.WriteLine("\tpublic partial class {0} : ViewController".FillFormat(name));
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tvoid Start()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t// Code Here");
            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");
            writer.Close();
        }
    }
}