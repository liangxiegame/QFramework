using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Debug = UnityEngine.Debug;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using QFramework;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

#endif
public class ScriptBuildTools
{

    public enum BuildMode
    {
        Release,
        Debug,
    }
#if UNITY_EDITOR

    private static Dictionary<int, string> csFilesMap;

#endif


    /// <summary>
    /// 编译DLL
    /// </summary>
    public static void BuildDll(string outPath, BuildMode mode)
    {
        try
        {
            EditorUtility.DisplayProgressBar("编译服务", "准备编译环境...", 0.1f);
            ILRuntimeScriptSetting.Default.UsePdb = mode == BuildMode.Debug;
            //输出环境
            var path = outPath + "/hotifx";

            //准备输出环境

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);

            EditorUtility.DisplayProgressBar("编译服务", "开始处理脚本", 0.2f);

            #region CS DLL引用搜集处理

            var hotfixAsmdefName = ILRuntimeScriptSetting.Default.HotfixAsmdefName;
            DirectoryInfo root = new DirectoryInfo(Application.dataPath);
            string codeSourcePath = "";
            if (string.IsNullOrEmpty(hotfixAsmdefName))
            {
                root.ForeachFiles(info =>
                {
                    var match = Regex.Match(info.Name, @".*?@hotfix\.asmdef");
                    if (match.Success)
                    {
                        codeSourcePath = info.DirectoryName;
                        hotfixAsmdefName = Path.GetFileNameWithoutExtension(info.Name);
                        return true;
                    }
                    return false;
                });
            }
            else
            {
                root.ForeachFiles(info =>
                {
                    if (info.Name == $"{hotfixAsmdefName}.asmdef")
                    {
                        codeSourcePath = info.DirectoryName;
                        return true;
                    }
                    return false;
                });
            }
            if (string.IsNullOrEmpty(codeSourcePath))
            {
                Log.E(
                    $"编译失败--{(string.IsNullOrEmpty(hotfixAsmdefName) ? "检查有没有@hotfix的程序集" : $@"检查设置中的程序集名字{hotfixAsmdefName}")}");
                return;
            }

            var csFiles = new List<string>(Directory.GetFiles(codeSourcePath, "*.cs", SearchOption.AllDirectories));
            if (mode == BuildMode.Release)
            {
                //删除pdb文件
                var pdbPath = Path.ChangeExtension(outPath, "pdb");
                if (File.Exists(pdbPath))
                    File.Delete(pdbPath);
            }
            List<string> dllFiles = new List<string>();

            FindDLLByCSPROJ("Assembly-CSharp.csproj", ref dllFiles);
            for (int i = 0; i < dllFiles.Count; i++)
            {
                if (Path.GetFileNameWithoutExtension(dllFiles[i]) == hotfixAsmdefName)
                {
                    dllFiles.RemoveAt(i);
                    break;
                }
            }
            csFiles = csFiles.Select(f => f.Replace('\\', Path.DirectorySeparatorChar)).ToList();

            #endregion

            var outHotfixPath = outPath + "/hotfix.dll";

            if (mode == BuildMode.Release)
            {
                Build(csFiles, dllFiles, outHotfixPath);
            }
            else if (mode == BuildMode.Debug)
            {
                Build(csFiles, dllFiles, outHotfixPath, true);
            }

            AssetDatabase.Refresh();
            Debug.Log("脚本打包完毕");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    /// <summary>
    /// 编译
    /// </summary>
    /// <param name="tempCodePath"></param>
    /// <param name="outBaseDllPath"></param>
    /// <param name="outHotfixDllPath"></param>
    static public void Build(
        List<string> hotfixCS,
        List<string> dllFiles,
        string outHotfixDllPath,
        bool isdebug=false)
    {
        //开始执行
        EditorUtility.DisplayProgressBar("编译服务", "开始编译hotfix.dll...", 0.7f);
        //build
        try
        {
            BuildByRoslyn(dllFiles.ToArray(), hotfixCS.ToArray(), outHotfixDllPath, isdebug);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            EditorUtility.ClearProgressBar();
            return;
        }

        EditorUtility.DisplayProgressBar("编译服务", "清理临时文件", 0.9f);
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
    private static List<string> defineList = new List<string>();
    /// <summary>
    /// 编译dll
    /// </summary>
    /// <param name="rootpaths"></param>
    /// <param name="output"></param>
    static public bool BuildByRoslyn(string[] dlls, string[] codefiles, string output, bool isdebug = false)
    {
        //添加语法树
        //宏解析
        List< Microsoft.CodeAnalysis.SyntaxTree> codes = new List< Microsoft.CodeAnalysis.SyntaxTree>();
        var opa = new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: defineList);
        foreach (var cs in codefiles)
        {
            var content = File.ReadAllText(cs);
            var syntaxTree = CSharpSyntaxTree.ParseText(content, opa, cs, Encoding.UTF8);
            codes.Add(syntaxTree);
        }

        //添加dll
        List<MetadataReference> assemblies = new List<MetadataReference>();
        foreach (var dll in dlls)
        {
            var metaref = MetadataReference.CreateFromFile(dll);
            if (metaref != null)
            {
                assemblies.Add(metaref);
            }
        }

        //创建目录
        var dir = Path.GetDirectoryName(output);
        Directory.CreateDirectory(dir);
        //编译参数
        CSharpCompilationOptions option = null;
        if (isdebug)
        {
            option = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Debug,warningLevel:4,
                allowUnsafe: true
            );
        }
        else
        {
            option = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release, warningLevel: 4,
                allowUnsafe: true
            );
        }

        //创建编译器代理
        var assemblyname = Path.GetFileNameWithoutExtension(output);
        var compilation = CSharpCompilation.Create(assemblyname, codes, assemblies, option);
        EmitResult result = null;
        if (!isdebug)
        {
            result =compilation.Emit(output);
        }
        else
        {
            var pdbPath = output + ".pdb";
            var emitOptions = new EmitOptions(
                debugInformationFormat: DebugInformationFormat.PortablePdb,
                pdbFilePath: pdbPath);
            using (var dllStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                result = compilation.Emit(dllStream, pdbStream,  options: emitOptions);
                
                File.WriteAllBytes(output,dllStream.GetBuffer()); 
                File.WriteAllBytes(pdbPath,pdbStream.GetBuffer()); 
            }
        }
        // 编译失败，提示
        if (!result.Success)
        {
            IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
            {
                Debug.LogError(diagnostic.ToString());
            }
        }

        return result.Success;
    }

    /// <summary>
    /// 解析project中的dll
    /// </summary>
    /// <returns></returns>
    static void FindDLLByCSPROJ(string projName, ref List<string> dllList)
    {
        if (dllList == null) dllList = new List<string>();

        var projectPath = BApplication.ProjectRoot + "/" + projName;

        if (!File.Exists(projectPath)) return;
        
        XmlDocument xml = new XmlDocument();
        xml.Load(projectPath);
        XmlNode ProjectNode = null;

        foreach (XmlNode x in xml.ChildNodes)
        {
            if (x.Name == "Project")
            {
                ProjectNode = x;
                break;
            }
        }

        List<string> csprojList = new List<string>();
        foreach (XmlNode childNode in ProjectNode.ChildNodes)
        {
            if (childNode.Name == "ItemGroup")
            {
                foreach (XmlNode item in childNode.ChildNodes)
                {
                    if (item.Name == "Reference") //DLL 引用
                    {
                        var HintPath = item.FirstChild;

                        var dir = HintPath.InnerText.Replace('\\', Path.DirectorySeparatorChar)
                            .Replace('/',Path.DirectorySeparatorChar);
                        dllList.Add(dir);
                    }
                    else if (item.Name == "ProjectReference") //工程引用
                    {
                        var csproj = item.Attributes[0].Value;
                        csprojList.Add(csproj);
                    }
                }
            }else if (childNode.Name == "PropertyGroup")
            {
                foreach (XmlNode item in childNode.ChildNodes)
                {
                    if (item.Name == "DefineConstants")
                    {
                        var define = item.InnerText;
                
                        var defines = define.Split(';');
                
                        defineList.AddRange(defines);
                    }
                
                }
            }
        }

        //csproj也加入
        foreach (var csproj in csprojList)
        {
            //有editor退出
            if (csproj.ToLower().Contains("editor")) continue;
            //添加扫描到的dll
            FindDLLByCSPROJ(csproj, ref dllList);
            //
            var gendll = BApplication.Library + "/ScriptAssemblies/" + csproj.Replace(".csproj", ".dll");
            if (!File.Exists(gendll))
            {
                Debug.LogError("不存在:" + gendll);
            }

            dllList.Add(gendll);
        }


        //去重
        dllList = dllList.Distinct().ToList();
    }

    static public class BApplication
    {
        /// <summary>
        /// 项目根目录
        /// </summary>
        static public string ProjectRoot { get; private set; } = Application.dataPath.Replace("/Assets", "");

        /// <summary>
        /// Library
        /// </summary>
        static public string Library { get; private set; } = ProjectRoot + "/Library";
    }
}
