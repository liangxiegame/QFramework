using System;
using QFramework;
using UnityEngine;

/// <summary>
/// 职责:
/// 1. 用户数据管理
/// 2. 玩家数据管理
/// 3. Manager  容器: List/Dictionary  增删改查
/// </summary>
///
///
public class PlayerData
{
    public string Username;
    
    public int Level;
    
    public string Carrer;
}


[QMonoSingletonPath("[Game]/PlayerDataMgr")]
public class PlayerDataMgr : MonoBehaviour,ISingleton
{
    private static PlayerDataMgr mInstance
    {
        get { return MonoSingletonProperty<PlayerDataMgr>.Instance; }
    }
   
    /// <summary>
    /// 对外阉割
    /// </summary>
    void ISingleton.OnSingletonInit()
    {
        mPlayerData = new PlayerData();
        // 从本地加载的一个操作

    }
    
    #region public 对外提供的 API

    public static void SavePlayerData()
    {
        mInstance.Save();
    }
    
    public static PlayerData GetPlayerData()
    {
        return mInstance.mPlayerData;
    }
    
    #endregion

    
    private PlayerData mPlayerData;

    
    private void Save()
    {
        // 保存到本地
    }
}