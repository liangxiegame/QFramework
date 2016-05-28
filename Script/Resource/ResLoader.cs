using UnityEngine;
using System.Collections;

/// <summary>
/// 从Resources中加载资源
/// </summary>
public class ResourceLoader : IResLoader
{
    public delegate void LoadResDoneCallback(string resName, Object resObj);

    protected AsyncOperation req = null;

    protected ResourceLoader()
    { }

    public ResourceLoader(string resName, LoadResDoneCallback loadResDone)
    {
        this.ResName = resName;
        this.LoadDoneCallback = loadResDone;
        int end = resName.LastIndexOf('.');
        string resPath = resName.Substring(0, (end == -1 ? resName.Length : end));
        this.req = Resources.LoadAsync(resPath);
    }

    // 资源名
    public string ResName { get; set; }

    public virtual Object GetResObj()
    {
        return ((ResourceRequest)this.req).asset;
    }

    public virtual bool IsDone()
    {
        return this.req.isDone;
    }

    public LoadResDoneCallback LoadDoneCallback { get; set; }

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
/// 从AssetBundle中加载资源
/// </summary>
public class AssetBundleLoader : ResourceLoader, IResLoader, IAssetBundleLoader
{
    private string[] dependences = null;
    private WWW www = null;
    // 必须从www字节流中反序列化得到assetbundle，才能作为依赖，构造最终的资源对象prefab
    private AssetBundle assetbundle = null;
    // 直接资源，需要从assetbundle中加载对象
    private AssetBundleRequest assetbundleReq = null;

    public AssetBundleLoader(string resName, LoadResDoneCallback loadResDone)
    {
        base.ResName = resName;
        base.LoadDoneCallback = loadResDone;
        // 保证先加载依赖的AssetBundle
        CheckDependences();
        // 再加载自己
        resName = resName.Substring(resName.LastIndexOf("/") + 1).ToLower();
		string wwwPath = LocalPath.AssetBundlePath + resName;
        this.www = new WWW(wwwPath);
    }

    /// <summary>
    /// 只有"直接资源"才有必要调用这个接口，获取资源对象
    /// </summary>
    /// <returns>资源对象</returns>
    public override Object GetResObj()
    {
        return this.assetbundleReq.asset;
    }

    public override bool IsDone()
    {
        // 如果有依赖，要先判断依赖是否已经加载完成
        if (this.dependences != null)
        {
            for (int i = 0; i < this.dependences.Length; ++i)
            {
                if (!ResMgr.Instance().IsLoadedAssetBundle(this.dependences[i]))
                    return false;
            }
        }
        // 判断自己是否加载完成
        if (base.LoadDoneCallback == null)
        {
            // 依赖资源，不需要LoadDone回调
            if (this.assetbundle != null)
                return true;
            if (this.www.isDone)
            {
                this.assetbundle = this.www.assetBundle;
                this.www.Dispose();
                this.www = null;
                return true;
            }
            return false;
        }
        else
        {
            // 直接资源，需要LoadDone回调，所以需要从AssetBundle中提取资源对象
            if (this.assetbundleReq != null)
            {
                // 只有从assetbundle中提取出最终的资源对象，才算加载完成，保证所有的IO操作都是异步的
                return this.assetbundleReq.isDone;
            }
            else
            {
                if (this.www.isDone)
                {
                    this.assetbundle = this.www.assetBundle;
                    // 从assetbundle中提取资源对象
					string resPath = string.Format(LocalPath.AssetBundleFormation, base.ResName);
                    this.assetbundleReq = this.assetbundle.LoadAssetAsync(resPath);
                    this.www.Dispose();
                    this.www = null;
                }
                return false;
            }
        }
    }

    /// <summary>
    /// 检查依赖资源是否已经加载完成，如果没有，则重新加载
    /// </summary>
    public virtual void CheckDependences()
    {
        this.dependences = ResMgr.Instance().GetDependences(base.ResName);
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
