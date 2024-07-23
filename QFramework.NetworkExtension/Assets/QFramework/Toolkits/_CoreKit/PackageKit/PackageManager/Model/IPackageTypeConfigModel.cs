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
    internal interface IPackageTypeConfigModel : IModel
    {
        string GetFullTypeName(string typeName);
    }

    internal class PackageTypeConfigModel : AbstractModel, IPackageTypeConfigModel
    {
        private Dictionary<string, string> mTypeName2FullName = new Dictionary<string, string>()
        {
            {"fm", "Framework"},
            {"p", "Plugin"},
            {"s", "Shader"},
            {"agt", "Example/Demo"},
            {"master", "Master"},
        };

        public string GetFullTypeName(string typeName)
        {
            if (mTypeName2FullName.ContainsKey(typeName))
            {
                return mTypeName2FullName[typeName];
            }

            return typeName;
        }

        protected override void OnInit()
        {
            
        }
    }
}