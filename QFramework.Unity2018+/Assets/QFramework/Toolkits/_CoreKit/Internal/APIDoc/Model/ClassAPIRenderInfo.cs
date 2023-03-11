/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    internal class ClassAPIRenderInfo
    {
        public string Description =>
            LocaleKitEditor.IsCN.Value ? mDescriptionCN.Description : mDescriptionEN.Description;

        public string ClassName { get; private set; }
        public string DisplayMenuName { get; private set; }
        public string DisplayClassName { get; private set; }
        public string Namespace { get; private set; }

        public string ExampleCode { get; private set; }
        public string GroupName { get; private set; }

        private APIDescriptionCNAttribute mDescriptionCN;
        private APIDescriptionENAttribute mDescriptionEN;

        public List<PropertyAPIRenderInfo> Properties { get; private set; }
        public List<MethodAPIRenderInfo> Methods { get; private set; }
        
        public int RenderOrder { get; private set; }


        private Type mType;

        public ClassAPIRenderInfo(Type type, ClassAPIAttribute classAPIAttribute)
        {
            mType = type;
            DisplayMenuName = classAPIAttribute.DisplayMenuName;
            GroupName = classAPIAttribute.GroupName;
            RenderOrder = classAPIAttribute.RenderOrder;
            DisplayClassName = classAPIAttribute.DisplayClassName;
        }

        public void Parse()
        {
            if (DisplayClassName.IsNullOrEmpty())
            {
                ClassName = mType.Name;
            }
            else
            {
                ClassName = DisplayClassName;
            }

            mDescriptionCN = mType.GetAttribute<APIDescriptionCNAttribute>(false);
            mDescriptionEN = mType.GetAttribute<APIDescriptionENAttribute>(false);
            Namespace = mType.Namespace;

            Properties = mType.GetProperties()
                .Where(p => p.HasAttribute<PropertyAPIAttribute>())
                .Select(p => new PropertyAPIRenderInfo(p)).ToList();

            Methods = mType.GetMethods()
                .Where(m => m.HasAttribute<MethodAPIAttribute>())
                .Select(m => new MethodAPIRenderInfo(m)).ToList();


            var exampleCode = mType.GetAttribute<APIExampleCodeAttribute>(false);
            if (exampleCode != null)
            {
                ExampleCode = exampleCode.Code;
            }
        }
    }
}
#endif