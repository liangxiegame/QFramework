/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;

namespace QFramework
{
    internal class PackageTypeConfigModel : AbstractModel
    {
        private readonly Dictionary<string, string> mTypeName2FullName = new Dictionary<string, string>()
        {
            {"fm", "Framework"},
            {"p", "Plugin"},
            {"s", "Shader"},
            {"agt", "Example/Demo"},
            {"master", "Master"},
        };

        public string GetFullTypeName(string typeName) => mTypeName2FullName.TryGetValue(typeName, out var name) ? name : typeName;

        protected override void OnInit()
        {
            
        }
    }
}