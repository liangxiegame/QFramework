using System;
using System.IO;

namespace QFramework
{
    public class ILBehaviourCodeTemplate
    {
        public static void Write(string name, string scriptsFolder, string ns)
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

            writer.WriteLine("namespace {0}".FillFormat(ns));
            writer.WriteLine("{");
            writer.WriteLine("\tpublic partial class {0}".FillFormat(name));
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tvoid OnStart()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t// Code Here");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine("\t\tvoid OnDestroy()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t// Destory Code Here");
            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");
            writer.Close();
        }
    }


    public class ILBehaviourCodeDesignerTemplate
    {
        public static void Write(string name, string scriptsFolder, PanelCodeInfo panelCodeInfo,
            string ns)
        {

            var scriptFile = scriptsFolder + "/{0}.Designer.cs".FillFormat(name);
            var writer = File.CreateText(scriptFile);

            writer.WriteLine("// Generate Id:{0}".FillFormat(Guid.NewGuid().ToString()));
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine("using QFramework;");
            writer.WriteLine();

            writer.WriteLine("namespace {0}".FillFormat(ns));
            writer.WriteLine("{");
            writer.WriteLine("\tpublic partial class {0}".FillFormat(name));
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tpublic const string NAME = \"{0}\";\n", name);

            foreach (var bindInfo in panelCodeInfo.BindInfos)
            {
                writer.WriteLine("\t\tpublic {0} {1};".FillFormat(bindInfo.BindScript.ComponentName,
                    bindInfo.Name));
            }

            writer.WriteLine("\t\tpublic Transform  transform  { get; set; }");
            writer.WriteLine("\t\tpublic GameObject gameObject { get; set; }");
            writer.WriteLine("\t\tpublic  ILKitBehaviour MonoBehaviour { get; set; }");
            writer.WriteLine();
            writer.WriteLine("\t\tpublic static void Start(ILKitBehaviour ilkitBehaviour)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tvar ilBehaviour = new {0}".FillFormat(name));
            writer.WriteLine("\t\t\t{");
            writer.WriteLine("\t\t\t\ttransform = ilkitBehaviour.transform,");
            writer.WriteLine("\t\t\t\tgameObject = ilkitBehaviour.gameObject,");
            writer.WriteLine("\t\t\t\tMonoBehaviour = ilkitBehaviour");
            writer.WriteLine("\t\t\t};");
            writer.WriteLine();
            writer.WriteLine("\t\t\tilkitBehaviour.Script = ilBehaviour;");
            writer.WriteLine();
            writer.WriteLine("\t\t\tilBehaviour.SetupBinds();");
            writer.WriteLine("\t\t\tilBehaviour.OnStart();");
            writer.WriteLine();
            writer.WriteLine("\t\t\tilkitBehaviour.OnDestroyAction += ilBehaviour.DestroyScript;");
            writer.WriteLine("\t\t}");

            writer.WriteLine();
            writer.WriteLine("\t\tvoid SetupBinds()");
            writer.WriteLine("\t\t{");
            foreach (var bindInfo in panelCodeInfo.BindInfos)
            {
                writer.WriteLine("\t\t\t{0} = transform.Find(\"{1}\").GetComponent<{2}>();"
                    .FillFormat(bindInfo.Name, bindInfo.PathToElement, bindInfo.BindScript.ComponentName));
            }

            writer.WriteLine("\t\t}");

            writer.WriteLine();
            writer.WriteLine("\t\tvoid ClearBinds()");
            writer.WriteLine("\t\t{");
            foreach (var bindInfo in panelCodeInfo.BindInfos)
            {
                writer.WriteLine("\t\t\t{0} = null;"
                    .FillFormat(bindInfo.Name));
            }

            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine("\t\tvoid DestroyScript()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tOnDestroy();");
            writer.WriteLine();
            writer.WriteLine("\t\t\tClearBinds();");
            writer.WriteLine();
            writer.WriteLine("\t\t\ttransform = null;");
            writer.WriteLine("\t\t\tgameObject = null;");
            writer.WriteLine("\t\t\tMonoBehaviour = null;");
            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Close();

        }
    }

    public class ILUIPanelCodeTemplate
    {
        public static void Write(string name, string scriptsFolder, string ns)
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

            writer.WriteLine("namespace {0}".FillFormat(ns));
            writer.WriteLine("{");
            writer.WriteLine("\tpublic class {0}Data : ILUIData".FillFormat(name));
            writer.WriteLine("\t{");
            writer.WriteLine("\t}");
            writer.WriteLine();
            writer.WriteLine("\tpublic partial class {0} : ILUIPanel".FillFormat(name));
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tprotected override void OnOpen(ILUIData uiData = null)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tmData = uiData as {0}Data ?? new {0}Data();".FillFormat(name));
            writer.WriteLine("\t\t\t// Code Here");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine("\t\tprotected override void OnClose()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t");
            writer.WriteLine("\t\t\t// Close Code Here");
            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");
            writer.Close();
        }
    }

    public class ILUIPanelCodeDesignerTemplate
    {
        public static void Write(string name, string scriptsFolder, PanelCodeInfo panelCodeInfo,
            string ns)
        {

            var scriptFile = scriptsFolder + "/{0}.Designer.cs".FillFormat(name);
            var writer = File.CreateText(scriptFile);

            writer.WriteLine("// Generate Id:{0}".FillFormat(Guid.NewGuid().ToString()));
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine("using QFramework;");
            writer.WriteLine();

            writer.WriteLine("namespace {0}".FillFormat(ns));
            writer.WriteLine("{");
            writer.WriteLine("\tpublic partial class {0}".FillFormat(name));
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tpublic const string NAME = \"{0}\";\n", name);

            foreach (var bindInfo in panelCodeInfo.BindInfos)
            {
                writer.WriteLine("\t\tpublic {0} {1};".FillFormat(bindInfo.BindScript.ComponentName,
                    bindInfo.Name));
            }

            writer.WriteLine("\t\tpublic Transform  transform  { get; set; }");
            writer.WriteLine("\t\tpublic GameObject gameObject { get; set; }");
            writer.WriteLine("\t\tpublic  ILKitBehaviour MonoBehaviour { get; set; }");
            writer.WriteLine();
            writer.WriteLine("\t\tpublic static void Start(ILKitBehaviour ilkitBehaviour)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tvar ilBehaviour = new {0}".FillFormat(name));
            writer.WriteLine("\t\t\t{");
            writer.WriteLine("\t\t\t\ttransform = ilkitBehaviour.transform,");
            writer.WriteLine("\t\t\t\tgameObject = ilkitBehaviour.gameObject,");
            writer.WriteLine("\t\t\t\tMonoBehaviour = ilkitBehaviour");
            writer.WriteLine("\t\t\t};");
            writer.WriteLine();
            writer.WriteLine("\t\t\tilkitBehaviour.Script = ilBehaviour;");
            writer.WriteLine();
            writer.WriteLine("\t\t\tilBehaviour.SetupBinds();");
            writer.WriteLine("\t\t\tilBehaviour.OnStart();");
            writer.WriteLine();
            writer.WriteLine("\t\t\tilkitBehaviour.OnDestroyAction += ilBehaviour.DestroyScript;");
            writer.WriteLine("\t\t}");

            writer.WriteLine();
            writer.WriteLine("\t\tvoid SetupBinds()");
            writer.WriteLine("\t\t{");
            foreach (var bindInfo in panelCodeInfo.BindInfos)
            {
                writer.WriteLine("\t\t\t{0} = transform.Find(\"{1}\").GetComponent<{2}>();"
                    .FillFormat(bindInfo.Name, bindInfo.PathToElement, bindInfo.BindScript.ComponentName));
            }

            writer.WriteLine("\t\t}");

            writer.WriteLine();
            writer.WriteLine("\t\tvoid ClearBinds()");
            writer.WriteLine("\t\t{");
            foreach (var bindInfo in panelCodeInfo.BindInfos)
            {
                writer.WriteLine("\t\t\t{0} = null;"
                    .FillFormat(bindInfo.Name));
            }

            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine("\t\tvoid DestroyScript()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tOnDestroy();");
            writer.WriteLine();
            writer.WriteLine("\t\t\tClearBinds();");
            writer.WriteLine();
            writer.WriteLine("\t\t\ttransform = null;");
            writer.WriteLine("\t\t\tgameObject = null;");
            writer.WriteLine("\t\t\tMonoBehaviour = null;");
            writer.WriteLine();
            writer.WriteLine("\t\t\tmPrivateData = null;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine("\t\tprotected {0}Data mData".FillFormat(name));
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tget");
            writer.WriteLine("\t\t\t{");
            writer.WriteLine("\t\t\t\treturn mPrivateData ?? (mPrivateData = new {0}Data());".FillFormat(name));
            writer.WriteLine("\t\t\t}");
            writer.WriteLine("\t\t\tset");
            writer.WriteLine("\t\t\t{");
            writer.WriteLine("\t\t\t\tmPrivateData = value;");
            writer.WriteLine("\t\t\t}");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine("\t\tprivate {0}Data mPrivateData = null;".FillFormat(name));

            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Close();
        }
    }
}