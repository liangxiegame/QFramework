/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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
        public string DisplayName { get; private set; }
        public string Namespace { get; private set; }

        public string ExampleCode { get; private set; }
        public string GroupName { get; private set; }

        private APIDescriptionCNAttribute mDescriptionCN;
        private APIDescriptionENAttribute mDescriptionEN;

        public List<MethodAPIRenderInfo> Methods { get; private set; }
        public int RenderOrder { get; private set; }


        private Type mType;

        public ClassAPIRenderInfo(Type type, ClassAPIAttribute classAPIAttribute)
        {
            mType = type;
            DisplayName = classAPIAttribute.DisplayName;
            GroupName = classAPIAttribute.GroupName;
            RenderOrder = classAPIAttribute.RenderOrder;
        }

        public void Parse()
        {
            ClassName = mType.Name;
            mDescriptionCN = mType.GetFirstAttribute<APIDescriptionCNAttribute>(false);
            mDescriptionEN = mType.GetFirstAttribute<APIDescriptionENAttribute>(false);
            Namespace = mType.Namespace;

            Methods = mType.GetMethods()
                .Where(m => m.GetFirstAttribute<MethodAPIAttribute>(false) != null)
                .Select(m => new MethodAPIRenderInfo(m)).ToList();


            var exampleCode = mType.GetFirstAttribute<APIExampleCodeAttribute>(false);
            if (exampleCode != null)
            {
                ExampleCode = exampleCode.Code;
            }
        }
    }
}
#endif