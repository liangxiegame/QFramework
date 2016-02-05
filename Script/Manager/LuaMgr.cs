//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
////using SLua;
//
///// <summary>
///// Lua脚本管理器
///// </summary>
//public class LuaMgr : Singleton<LuaMgr>
//{
//    public const string LuaFilter = "*.lua";
//    public const string LuaExt = ".lua";
//
//    public static string LuaPathPrefix = null;
//
//    // 缓存所有lua文件内存，避免每次加载都执行硬盘到内存IO
//    // 不要有同名lua文件，即便是不同路径下
//    private Dictionary<string, byte[]> luaDict = null;
//    // 全局唯一LuaState
//    private LuaSvr luaSvr = null;
//
//    private LuaMgr()
//    { }
//
//    public IEnumerator Init()
//    {
//        this.luaDict = new Dictionary<string, byte[]>();
//        // 这样会导致每帧只加载一个脚本文件，效率太低，可考虑将所有脚本打包在一起
//#if ASSETBUNDLE
//        WWW www = new WWW("file://" + Application.streamingAssetsPath + "/AssetBundles/script.lua");
//        yield return www;
//        AssetBundle scriptAssetBundle = www.assetBundle;
//        string luasIni = (scriptAssetBundle.LoadAsset("Assets/LuaProject/luas.txt") as TextAsset).text;
//        string[] luas = luasIni.Split('\n');
//        for (int i = 0; i < luas.Length; ++i)
//        {
//            string lua = luas[i];
//            if (string.IsNullOrEmpty(lua))
//                continue;
//            string luaAssetBundlePath = "Assets/LuaProject/" + lua + ".txt";
//            Debug.Log(luaAssetBundlePath);
//            TextAsset asset = scriptAssetBundle.LoadAsset(luaAssetBundlePath) as TextAsset;
//            this.luaDict[lua] = asset.bytes;
//        }
//#else
//#if UNITY_ANDROID || UNITY_IOS
//        LuaPathPrefix = Application.streamingAssetsPath + "/LuaProject/";
//        //LuaPathPrefix = "file://" + Application.dataPath + "/StreamingAssets/LuaProject/";
//        WWW www = new WWW(LuaPathPrefix + "luas.txt");
//        yield return www;
//
//        // 预加载lua文件至内存
//        string[] luas = www.text.Split('\n');
//        Debug.Log("Luas Count : " + luas.Length);
//        for (int i = 0; i < luas.Length; ++i)
//        {
//            string luaPath = luas[i].Trim();
//            if (string.IsNullOrEmpty(luaPath))
//                continue;
//            www = new WWW(LuaPathPrefix + luaPath);
//            yield return www;
//            this.luaDict[luaPath] = www.bytes;
//            //Debug.Log("Load " + luaPath);
//            www.Dispose();
//            www = null;
//        }
//#else
//        // 简化PC及Editor平台加载lua步骤
//        LuaPathPrefix = Application.streamingAssetsPath + "/LuaProject/";
//        string[] luas = File.ReadAllLines(LuaPathPrefix + "luas.txt");
//        Debug.Log("Luas Count : " + luas.Length);
//        for (int i = 0; i < luas.Length; ++i)
//        {
//            string luaPath = luas[i].Trim();
//            //Debug.Log("Load " + luaPath);
//            if (string.IsNullOrEmpty(luaPath))
//                continue;
//            this.luaDict[luaPath] = File.ReadAllBytes(LuaPathPrefix + luaPath);
//        }
//        yield return null;
//#endif
//#endif
//        // 重写加载规则
//        LuaState.loaderDelegate = RequireLua;
//        Debug.LogWarning("Lua Svr Start : " + System.DateTime.Now);
//        // 初始化lua环境，启动脚本
//        this.luaSvr = new LuaSvr();
//        this.luaSvr.init(
//            (progress) =>
//            {
//                Debug.Log("BindProgress : " + progress);
//            },
//            () =>
//            {
//                this.luaSvr.start(Game.Instance().startScript);
//            });
//        Debug.LogWarning("Lua Svr Start Done : " + System.DateTime.Now);
//    }
//
//    public void CallGlobalFunction(string funcName, params object[] args)
//    {
//        if (string.IsNullOrEmpty(funcName))
//        {
//            Debug.LogError("CallGlobalFunction funcName is NULL!");
//            return;
//        }
//        LuaFunction func = this.luaSvr.luaState.getFunction(funcName);
//        if (func == null)
//        {
//            Debug.LogError("NOT found Global Function : " + funcName);
//            return;
//        }
//        func.call(args);
//    }
//
//    public void CallTableFunction(string tabelName, string funcName, params object[] args)
//    {
//        if (string.IsNullOrEmpty(tabelName))
//        {
//            Debug.LogError("CallTableFunction tableName is NULL!");
//        }
//        if (string.IsNullOrEmpty(funcName))
//        {
//            Debug.LogError("CallTableFunction funcName is NULL!");
//            return;
//        }
//        LuaTable table = this.luaSvr.luaState.getTable(tabelName);
//        LuaFunction func = table[funcName] as LuaFunction;
//        if (func == null)
//        {
//            Debug.LogError("NOT found CallTableFunction Function : " + funcName);
//            return;
//        }
//        func.call(args);
//    }
//
//    // 自定义加载Lua脚本实现
//    byte[] RequireLua(string file)
//    {
//        if (!file.EndsWith(LuaExt))
//            file += LuaExt;
//
//        byte[] luaCode = null;
//        // 查找策略：先根目录，后逻辑目录，再库目录，待优化
//        // 1.根目录查找
//        if (this.luaDict.TryGetValue(file, out luaCode))
//            return luaCode;
//        // 2.查找库目录
//        // 查找thrift目录
//        if (this.luaDict.TryGetValue("Lib/Thrift/" + file, out luaCode))
//            return luaCode;
//        // 查找cjson目录
//        if (this.luaDict.TryGetValue("Lib/cjson/" + file, out luaCode))
//            return luaCode;
//        // 3.配置目录
//        if (this.luaDict.TryGetValue("AutoGeneration/Config/" + file, out luaCode))
//            return luaCode;
//        // 4.协议目录
//        if (this.luaDict.TryGetValue("AutoGeneration/Protocol/" + file, out luaCode))
//            return luaCode;
//
//        // 没找到
//        return null;
//    }
//}
