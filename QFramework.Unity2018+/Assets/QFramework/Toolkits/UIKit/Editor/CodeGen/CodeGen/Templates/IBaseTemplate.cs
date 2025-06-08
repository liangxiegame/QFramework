/****************************************************************************
* Copyright (c) 2016 ~ 2025 liangxiegame UNDER MIT LINCENSE
* 
* https://qframework.cn
* https://github.com/liangxiegame/QFramework
* https://gitee.com/liangxiegame/QFramework
****************************************************************************/

namespace QFramework
{
    using UnityEngine;

    public interface IBaseTemplate
    {
        void Generate(string generateFilePath, string behaviourName, string nameSpace, PanelCodeInfo panelCodeInfo);
    }

    public delegate void ScriptKitCodeBind(GameObject uiPrefab, string filePath);

    /// <summary>
    /// 存储一些ScriptKit相关的信息
    /// </summary>
    public class ScriptKitInfo
    {
        public string            HotScriptFilePath;
        public string            HotScriptSuffix;
        public IBaseTemplate[]   Templates;
        public ScriptKitCodeBind CodeBind;
    }
}