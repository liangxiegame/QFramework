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
        static bool IsCN => LocaleKitEditor.IsCN.Value;

        public static string Description => IsCN ? "描述" : "Description";

        public static string ExampleCode => IsCN ? "示例" : "Example";
        public static string Methods => IsCN ? "方法" : "Methods";
        public static string Properties => IsCN ? "属性" : "Properties";

        public static string Name => IsCN ? "名称" : "Name";
        
        
    }
}
#endif