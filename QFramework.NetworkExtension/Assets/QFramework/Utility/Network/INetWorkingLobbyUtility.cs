using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace QFramework.NetworkExtension
{
    public interface INetWorkingLobbyUtility : IUtility
    {
        /// <summary>
        /// 创建玩家昵称
        /// </summary>
        /// <param name="name"></param>
        void CreateName(string name);

        /// <summary>
        /// 一起游戏
        /// </summary>
        /// <param name="PlayerNames">玩家昵称</param>
        /// <param name="gameName">副本名称</param>
        void PlayTogether(string[] PlayerNames, string gameName);

        /// <summary>
        /// 获取大厅玩家列表
        /// </summary>
        void GetPlayerList(Action<string> userNames);

    }
}

