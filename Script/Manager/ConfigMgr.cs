using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using SLua;

//[CustomLuaClass]
public class ConfigMgr : QSingleton<ConfigMgr>
{
    private Dictionary<string, byte[]> configDict = null;

    private ConfigMgr()
    { }

    public IEnumerator Init()
    {
        this.configDict = new Dictionary<string, byte[]>();
#if ASSETBUNDLE
        WWW www = new WWW(ResMgr.DeviceURL + "config.bytes");
        yield return www;
        AssetBundle configAssetBundle = www.assetBundle;
        string configsIni = (configAssetBundle.LoadAsset("Assets/Config/configs.txt") as TextAsset).text;
        string[] configs = configsIni.Split('\n');
        for (int i = 0; i < configs.Length; ++i)
        {
            string config = configs[i];
            TextAsset asset = configAssetBundle.LoadAsset("Assets/Config/" + config) as TextAsset;
            this.configDict[config] = asset.bytes;
        }
        Debug.Log("Config Init OK!");
#else
#if UNITY_ANDROID || UNITY_IOS
        string configPathPrefix = "jar:file://" + Application.dataPath + "!/assets/Config/";
        //string configPathPrefix = "file://" + Application.dataPath + "/StreamingAssets/Config/";
        WWW www = new WWW(configPathPrefix + "configs.txt");
        yield return www;
        string[] configs = www.text.Split('\n');
        Debug.Log("Configs Count : " + configs.Length);
        for (int i = 0; i < configs.Length; ++i)
        {
            string config = configs[i];
            string path = configPathPrefix + config;
            Debug.Log(path);
            www = new WWW(path);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
            }
            else
            {
                this.configDict[config] = www.bytes;
                Debug.Log("Load Config " + config + " " + www.bytes.Length);
                www.Dispose();
                www = null; 
            }
        }
#else
        // 简化PC及Editor平台加载config步骤
        string configPathPrefix = Application.streamingAssetsPath + "/Config/";
        string[] configs = File.ReadAllLines(configPathPrefix + "configs.txt");
        Debug.Log("Configs Count : " + configs.Length);
        for (int i = 0; i < configs.Length; ++i)
        {
            string config = configs[i].Trim();
            Debug.Log("Load " + config);
            if (string.IsNullOrEmpty(config))
                continue;
            this.configDict[config] = File.ReadAllBytes(configPathPrefix + config);
        }
        yield return null;
#endif
#endif
    }

    public byte[] GetConfig(string name)
    {
        byte[] bytes = null;
        if (!this.configDict.TryGetValue(name, out bytes))
        {
            Debug.Log("NOT Found " + name);
            return null;
        }
        return bytes;
    }
}
