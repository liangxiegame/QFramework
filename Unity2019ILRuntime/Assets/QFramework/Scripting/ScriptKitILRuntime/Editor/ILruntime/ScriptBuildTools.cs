using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Debug = UnityEngine.Debug;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
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
        EditorUtility.DisplayProgressBar("编译服务", "准备编译环境...", 0.1f);

        //输出环境
        var path = outPath + "/hotifx";
        
        //准备输出环境
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("提示", "请手动删除hotfix文件后重试!", "OK");
            return;
        }
        EditorUtility.DisplayProgressBar("编译服务", "开始处理脚本", 0.2f);
        #region CS DLL引用搜集处理

        List<string> dllFiles =new List<string>();
        List<string> csFiles  =new List<string>();
        
        csFiles = FindDLLByCSPROJ("Assembly-CSharp.csproj", ref  dllFiles);

        csFiles = csFiles.Select(f => f.Replace('\\', Path.DirectorySeparatorChar)).ToList();
        
        var baseCs   = csFiles.FindAll(f => !f.Contains("@hotfix") && f.EndsWith(".cs"));
        var hotfixCs = csFiles.FindAll(f => f.Contains("@hotfix")  && f.EndsWith(".cs"));
        #endregion

        var outHotfixPath = outPath + "/hotfix.dll";

        if (mode == BuildMode.Release)
        {
            Build(baseCs,hotfixCs, dllFiles, outHotfixPath);
        }
        else if (mode == BuildMode.Debug)
        {
            Build(baseCs,hotfixCs, dllFiles, outHotfixPath,true);
        }
        
        AssetDatabase.Refresh();
        Debug.Log("脚本打包完毕");
    }
    
    /// <summary>
    /// 编译
    /// </summary>
    /// <param name="tempCodePath"></param>
    /// <param name="outBaseDllPath"></param>
    /// <param name="outHotfixDllPath"></param>
    static public void Build(
        List<string>  baseCs,
        List<string> hotfixCS,
        List<string> dllFiles,
        string outHotfixDllPath,
        bool isdebug=false)
    {
        var baseDll = outHotfixDllPath.Replace("hotfix.dll", "Assembly-CSharp.dll");
        //开始执行
        EditorUtility.DisplayProgressBar("编译服务", "[1/2]开始编译base.dll...", 0.5f);
        try
        {
            BuildByRoslyn(dllFiles.ToArray(), baseCs.ToArray(), baseDll,false);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            EditorUtility.ClearProgressBar();
            return;
        }
        
        EditorUtility.DisplayProgressBar("编译服务", "[2/2]开始编译hotfix.dll...", 0.7f);
        //将base.dll加入
        //var mainDll = BApplication.ProjectRoot + "/Library/ScriptAssemblies/Assembly-CSharp.dll";
        var mainDll = baseDll;
        if (!dllFiles.Contains(mainDll))
        {
            dllFiles.Add(mainDll);
        }
        //build
        try
        {
            BuildByRoslyn(dllFiles.ToArray(), hotfixCS.ToArray(), outHotfixDllPath,isdebug);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            EditorUtility.ClearProgressBar();
            return;
        }

        EditorUtility.DisplayProgressBar("编译服务", "清理临时文件", 0.9f);
        File.Delete(baseDll);
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
  private static string define =
        @"DEBUG;TRACE;UNITY_5_3_OR_NEWER;UNITY_5_4_OR_NEWER;UNITY_5_5_OR_NEWER;UNITY_5_6_OR_NEWER;UNITY_2017_1_OR_NEWER;UNITY_2017_2_OR_NEWER;UNITY_2017_3_OR_NEWER;UNITY_2017_4_OR_NEWER;UNITY_2018_1_OR_NEWER;UNITY_2018_2_OR_NEWER;UNITY_2018_3_OR_NEWER;UNITY_2018_3_8;UNITY_2018_3;UNITY_2018;PLATFORM_ARCH_64;UNITY_64;UNITY_INCLUDE_TESTS;ENABLE_AUDIO;ENABLE_CACHING;ENABLE_CLOTH;ENABLE_DUCK_TYPING;ENABLE_MICROPHONE;ENABLE_MULTIPLE_DISPLAYS;ENABLE_PHYSICS;ENABLE_SPRITES;ENABLE_GRID;ENABLE_TILEMAP;ENABLE_TERRAIN;ENABLE_TEXTURE_STREAMING;ENABLE_DIRECTOR;ENABLE_UNET;ENABLE_LZMA;ENABLE_UNITYEVENTS;ENABLE_WEBCAM;ENABLE_WWW;ENABLE_CLOUD_SERVICES_COLLAB;ENABLE_CLOUD_SERVICES_COLLAB_SOFTLOCKS;ENABLE_CLOUD_SERVICES_ADS;ENABLE_CLOUD_HUB;ENABLE_CLOUD_PROJECT_ID;ENABLE_CLOUD_SERVICES_USE_WEBREQUEST;ENABLE_CLOUD_SERVICES_UNET;ENABLE_CLOUD_SERVICES_BUILD;ENABLE_CLOUD_LICENSE;ENABLE_EDITOR_HUB;ENABLE_EDITOR_HUB_LICENSE;ENABLE_WEBSOCKET_CLIENT;ENABLE_DIRECTOR_AUDIO;ENABLE_DIRECTOR_TEXTURE;ENABLE_TIMELINE;ENABLE_EDITOR_METRICS;ENABLE_EDITOR_METRICS_CACHING;ENABLE_MANAGED_JOBS;ENABLE_MANAGED_TRANSFORM_JOBS;ENABLE_MANAGED_ANIMATION_JOBS;INCLUDE_DYNAMIC_GI;INCLUDE_GI;ENABLE_MONO_BDWGC;PLATFORM_SUPPORTS_MONO;RENDER_SOFTWARE_CURSOR;INCLUDE_PUBNUB;ENABLE_VIDEO;ENABLE_CUSTOM_RENDER_TEXTURE;ENABLE_LOCALIZATION;PLATFORM_STANDALONE_WIN;PLATFORM_STANDALONE;UNITY_STANDALONE_WIN;UNITY_STANDALONE;ENABLE_SUBSTANCE;ENABLE_RUNTIME_GI;ENABLE_MOVIES;ENABLE_NETWORK;ENABLE_CRUNCH_TEXTURE_COMPRESSION;ENABLE_UNITYWEBREQUEST;ENABLE_CLOUD_SERVICES;ENABLE_CLOUD_SERVICES_ANALYTICS;ENABLE_CLOUD_SERVICES_PURCHASING;ENABLE_CLOUD_SERVICES_CRASH_REPORTING;ENABLE_OUT_OF_PROCESS_CRASH_HANDLER;ENABLE_EVENT_QUEUE;ENABLE_CLUSTER_SYNC;ENABLE_CLUSTERINPUT;ENABLE_VR;ENABLE_AR;ENABLE_WEBSOCKET_HOST;ENABLE_MONO;NET_4_6;ENABLE_PROFILER;UNITY_ASSERTIONS;ENABLE_UNITY_COLLECTIONS_CHECKS;ENABLE_BURST_AOT;UNITY_TEAM_LICENSE;UNITY_PRO_LICENSE;ODIN_INSPECTOR;UNITY_POST_PROCESSING_STACK_V2;CSHARP_7_OR_LATER;CSHARP_7_3_OR_NEWER";

    /// <summary>
    /// 编译dll
    /// </summary>
    /// <param name="rootpaths"></param>
    /// <param name="output"></param>
    static public bool BuildByRoslyn(string[] dlls, string[] codefiles, string output, bool isdebug = false)
    {
        //添加语法树
        //宏解析
        var Symbols = define.Split(';');
        List< Microsoft.CodeAnalysis.SyntaxTree> codes = new List< Microsoft.CodeAnalysis.SyntaxTree>();
        var opa = new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: Symbols);
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
    static List<string> FindDLLByCSPROJ(string projName, ref List<string> dllList)
    {
        if (dllList == null) dllList = new List<string>();
        //cs list
        List<string> csList = new List<string>();

        var projpath = BApplication.ProjectRoot + "/" + projName;

        if (!File.Exists(projpath)) return csList; 
        
        XmlDocument xml = new XmlDocument();
        xml.Load(projpath);
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
                    if (item.Name == "Compile") //cs 引用
                    {
                        var csproj = item.Attributes[0].Value;
                        csList.Add(csproj);
                    }
                    else if (item.Name == "Reference") //DLL 引用
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

        return csList;
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
