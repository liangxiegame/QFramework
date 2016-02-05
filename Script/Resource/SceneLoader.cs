using UnityEngine;
using System.Collections;

/// <summary>
/// 开发模式下加载场景，保证场景添加到Build Setting中
/// </summary>
public class SceneLoader : ISceneLoader
{
    public delegate void LoadSceneDoneCallback(string sceneName);
    public delegate void LoadSceneUpdateCallback(string sceneName, float progress);

    public string SceneName { get; set; }
    public LoadSceneDoneCallback LoadDoneCallback { get; set; }
    public LoadSceneUpdateCallback LoadUpdateCallback { get; set; }
    protected AsyncOperation op = null;

    public SceneLoader()
    { }

    public SceneLoader(string sceneName, bool additive, LoadSceneDoneCallback loadDone, LoadSceneUpdateCallback updateProgress)
    {
        this.SceneName = sceneName;
        this.LoadDoneCallback = loadDone;
        this.LoadUpdateCallback = updateProgress;

        int end = sceneName.LastIndexOf('.');
        string scenePath = sceneName.Substring(0, (end == -1 ? sceneName.Length : end));
        if (additive)
            this.op = Application.LoadLevelAdditiveAsync(scenePath);
        else
            this.op = Application.LoadLevelAsync(scenePath);
    }

    public virtual bool IsDone()
    {
        return this.op.isDone;
    }

    public virtual float GetProgress()
    {
        return this.op.progress;
    }

    public bool MoveNext()
    {
        return !IsDone();
    }

    public void Reset()
    {
    }

    public object Current
    {
        get
        {
            return null;
        }
    }
}

/// <summary>
/// 从AssetBundle中加载场景
/// </summary>
public class AssetBundleSceneLoader : SceneLoader, IAssetBundleLoader
{
    private string[] dependences = null;
    private WWW www = null;
    private AssetBundle assetbundle = null;
    private bool additive = false;

    public AssetBundleSceneLoader(string sceneName, bool additive, LoadSceneDoneCallback loadDone, LoadSceneUpdateCallback updateProgress)
    {
        base.SceneName = sceneName;
        base.LoadDoneCallback = loadDone;
        base.LoadUpdateCallback = updateProgress;

        this.additive = additive;
        CheckDependences();
        string wwwPath = ResMgr.AssetBundlePath + sceneName.ToLower();
        this.www = new WWW(wwwPath);
    }

    public override bool IsDone()
    {
        if (this.dependences != null)
        {
            for (int i = 0; i < this.dependences.Length; ++i)
            {
                if (!ResMgr.Instance().IsLoadedAssetBundle(this.dependences[i]))
                    return false;
            }
        }
        if (this.op != null)
        {
            return this.op.isDone;
        }
        else
        {
            if (this.www.isDone)
            {
                this.assetbundle = this.www.assetBundle;
                string scene = base.SceneName.Substring(0, base.SceneName.LastIndexOf('.'));
                if (this.additive)
                    this.op = Application.LoadLevelAdditiveAsync(scene);
                else
                    this.op = Application.LoadLevelAsync(scene);
                this.www.Dispose();
                this.www = null;
            }
            return false;
        }
    }

    public override float GetProgress()
    {
        if (this.op != null)
            return (0.5f + 0.5f * this.op.progress);
        return (0.5f * this.www.progress);
    }

    public virtual void CheckDependences()
    {
        this.dependences = ResMgr.Instance().GetDependences(base.SceneName);
        for (int i = 0; i < this.dependences.Length; ++i)
        {
            string dependence = this.dependences[i];
            if (!ResMgr.Instance().IsLoadedAssetBundle(dependence))
                ResMgr.Instance().LoadRes(dependence);
        }
    }

    public virtual void Unload()
    {
        Logger.Assert(this.assetbundle != null, "Unload a NOT loaded assetbundle");
        this.assetbundle.Unload(false);
    }
}
