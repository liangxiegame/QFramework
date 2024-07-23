/*****************************************************
文件：INetworkingVoiceUtility.cs
作者：Liam
日期：2021/12/25 16:40:34
功能：Nothing
*****************************************************/
using QFramework;
using System;

namespace QFramework.NetworkExtension
{
    public interface INetworkingVoiceUtility : IUtility
    {
        /// <summary>
        /// 初始化语音服务器
        /// </summary>
        void Init();

        /// <summary>
        /// 关闭麦克风
        /// </summary>
        void SetVoiceTransmitEnabled(bool isEnabled);

        /// <summary>
        /// 禁用兴趣组/禁用兴趣组
        /// </summary>
        /// <param name="disableGroups">可以为空,不为空时为禁用这个兴趣组 </param>
        /// <param name="enableGroups">可以为空,不为空时为进入这个兴趣组</param>
        void SetInterestGroups(byte[] disableGroups, byte[] enableGroups);

        /// <summary>
        /// 进入兴趣组
        /// </summary>
        /// <param name="targetGroup"></param>
        void SetTargetGroup(byte targetGroup);

        /// <summary>
        /// 当进入语音Room
        /// </summary>
        event Action<string> onInVoiceRoomEvent;
    }
}