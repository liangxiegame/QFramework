using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using SLua;

/// <summary>
/// 场景管理器
/// </summary>
//[CustomLuaClass]
public class SceneMgr : QSingleton<SceneMgr>
{
    public delegate void LoadSceneDoneCallback(string sceneName);
    public delegate void LoadSceneUpdateProgressCallback(string sceneName, float progress);

    private string curSceneName = null;
    private string[] curLogicResNames = null;
    private Dictionary<string, Object> curLogicResObjDict = null;
    private LoadSceneDoneCallback curLoadingSceneDoneCallback = null;
    private LoadSceneDoneCallback curLoadSceneDoneCallback = null;
    private LoadSceneUpdateProgressCallback curUpdateProgressCallback = null;
    private int curTotalProgress = 0;
    private int curSceneProgress = 0;
    private int curLogicResProgress = 0;

    private SceneMgr()
    {
        this.curLogicResObjDict = new Dictionary<string, Object>();
    }

    /// <summary>
    /// 进入场景
    /// </summary>
    /// <param name="loadingSceneName">Loading场景名字，忽略路径，name.scene格式</param>
    /// <param name="loadingSceneDoneCallback">Loading场景加载完成回调，给予自定义Loading场景显示逻辑入口</param>
    /// <param name="sceneName">真正要加载的场景名，格式同loadingSceneName</param>
    /// <param name="logicResNames">逻辑资源名，这里只必要资源，即场景进入后所需的必要资源</param>
    /// <param name="loadSceneDoneCallback">场景加载完成回调，一般用于场景逻辑初始化</param>
    /// <param name="loadSceneUpdateProgressCallback">场景加载进度回调，用于Loading显示</param>
    public void EnterScene(string loadingSceneName, LoadSceneDoneCallback loadingSceneDoneCallback, 
        string sceneName, string[] logicResNames, LoadSceneDoneCallback loadSceneDoneCallback, LoadSceneUpdateProgressCallback loadSceneUpdateProgressCallback)
    {
        this.curSceneName = sceneName;
        this.curLogicResNames = logicResNames;
        this.curLoadingSceneDoneCallback = loadingSceneDoneCallback;
        this.curLoadSceneDoneCallback = loadSceneDoneCallback;
        this.curUpdateProgressCallback = loadSceneUpdateProgressCallback;

        // 4.释放中间资源
        ResMgr.Instance().Unload();
        // 1.加载Loading场景
        ResMgr.Instance().LoadScene(loadingSceneName, false, OnLoadingSceneDone, null);
    }

    /// <summary>
    /// 获取场景缓存资源
    /// </summary>
    /// <param name="resName">资源名</param>
    /// <returns>资源对象</returns>
    public Object GetResObj(string resName)
    {
        Object resObj = null;
        Debug.LogWarning("Load " + resName);
        foreach (KeyValuePair<string, Object> resKV in this.curLogicResObjDict)
            Debug.LogWarning("Loaded " + resKV.Key);
        if (this.curLogicResObjDict.TryGetValue(resName, out resObj))
        {
            Debug.LogWarning("SceneMgr GetResObj " + resName + " is OK");
            return resObj;
        }
        Debug.LogError("Get ResObj " + resName + " is NULL");
        return null;
    }

    /// <summary>
    /// Loading场景加载完成回调
    /// </summary>
    /// <param name="sceneName">Loading场景名，没啥意义</param>
    void OnLoadingSceneDone(string sceneName)
    {
        if (this.curLoadingSceneDoneCallback != null)
            this.curLoadingSceneDoneCallback(null);
        
        // 2.加载主场景
        this.curTotalProgress = 50;
        ResMgr.Instance().LoadScene(this.curSceneName, true, OnLoadSceneDone, OnLoadSceneUpdateProgress);
        // 3.加载逻辑资源
        if (this.curLogicResNames != null && this.curLogicResNames.Length > 0)
        {
            this.curTotalProgress += this.curLogicResNames.Length;
            for (int i = 0; i < this.curLogicResNames.Length; ++i)
                ResMgr.Instance().LoadRes(this.curLogicResNames[i], OnLoadLogicResDone);
        }
    }

    /// <summary>
    /// 场景加载更新回调
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="progress">加载进度</param>
    void OnLoadSceneUpdateProgress(string sceneName, float progress)
    {
        this.curSceneProgress = (int)(this.curSceneProgress * progress);
    }

    /// <summary>
    /// 场景加载完成回调
    /// </summary>
    /// <param name="sceneName">场景名</param>
    void OnLoadSceneDone(string sceneName)
    {
        this.curSceneProgress = 50;
        CheckLoadAllDone();
    }

    /// <summary>
    /// 逻辑资源加载完成回调
    /// </summary>
    /// <param name="resName">资源名</param>
    /// <param name="resObj">资源对象</param>
    void OnLoadLogicResDone(string resName, Object resObj)
    {
        this.curLogicResObjDict[resName] = resObj;
        ++this.curLogicResProgress;
        CheckLoadAllDone();
    }

    /// <summary>
    /// 检查是否全部加载完成
    /// </summary>
    void CheckLoadAllDone()
    {
        int progress = this.curSceneProgress + this.curLogicResProgress;
        if (progress == this.curTotalProgress)
        {
            // 加载完成
            this.curLoadSceneDoneCallback(this.curSceneName);
            this.curSceneProgress = 0;
            this.curLogicResProgress = 0;
            
        }
        else
        {
            // 加载进度更新
            float progressVal = (float)(progress) / this.curTotalProgress;
            this.curUpdateProgressCallback(this.curSceneName, progressVal);
        }
    }

    /// <summary>
    /// 异步清理场景资源
    /// </summary>
    /// <returns></returns>
    IEnumerator CleanScene()
    {
        ResMgr.Instance().Unload();
        this.curLogicResObjDict.Clear();
        this.curLogicResObjDict = null;
        Debug.LogWarning("Unload in SceneMgr");
        AsyncOperation req = Resources.UnloadUnusedAssets();
        yield return req;
        System.GC.Collect();
    }
}
