/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
namespace QFramework
{
    public class APIDocLocale
    {
        public bool IsCN => LocaleKitEditor.IsCN.Value;

        public string Description => IsCN ? "描述" : "Description";

        public string ExampleCode => IsCN ? "示例" : "Example";
        public string Methods => IsCN ? "方法" : "Methods";
    }
}
#endif