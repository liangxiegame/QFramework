using System;
using System.IO;

namespace QFramework
{
    public class ViewControllerDesignerTemplate
    {
        public static void Write(string name, string scriptsFolder, string scriptNamespace, PanelCodeInfo panelCodeInfo,
            UIKitSettingData uiKitSettingData)
        {
            var scriptFile = string.Format(scriptsFolder + "/{0}.Designer.cs",name);
            var writer = File.CreateText(scriptFile);

            writer.WriteLine($"// Generate Id:{Guid.NewGuid().ToString()}");
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine();

            if (uiKitSettingData.IsDefaultNamespace)
            {
                writer.WriteLine("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间");
                writer.WriteLine("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
            }

            writer.WriteLine(
                $"namespace {(string.IsNullOrWhiteSpace(scriptNamespace) ? uiKitSettingData.Namespace : scriptNamespace)}");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic partial class {name}");
            writer.WriteLine("\t{");

            foreach (var bindInfo in panelCodeInfo.BindInfos)
            {
                writer.WriteLine(string.Format("\t\tpublic {0} {1};",bindInfo.BindScript.ComponentName, bindInfo.Name));
            }

            writer.WriteLine();
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Close();
        }
    }

    public class ViewControllerTemplate
    {
        public static void Write(string name, string scriptsFolder, string scriptNamespace,
            UIKitSettingData uiKitSettingData)
        {
            var scriptFile = string.Format(scriptsFolder + "/{0}.cs",(name));


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

            writer.WriteLine(string.Format("namespace {0}",(string.IsNullOrWhiteSpace(scriptNamespace))
                ? uiKitSettingData.Namespace
                : scriptNamespace));
            writer.WriteLine("{");
            writer.WriteLine(string.Format("\tpublic partial class {0} : ViewController",name));
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