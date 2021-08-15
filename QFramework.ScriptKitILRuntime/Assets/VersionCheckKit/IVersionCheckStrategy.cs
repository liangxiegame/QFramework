using System;

namespace QFramework
{
    /// <summary>
    /// 版本下载策略
    /// </summary>
    public interface IVersionCheckStrategy
    {
        string ServerHost { get; }
        
        void UpdateRes(Action done);

        void LocalVersionGetter(Action<DLLVersion> onLocalVersionGetted);

        void ServerVersionGetter(Action<DLLVersion> onServerVersionGetted);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onHasRes"></param>
        void VersionCheck(Action<bool,DLLVersion,DLLVersion> onHasRes);
    }
}