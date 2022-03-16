/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Reflection;
using System.Text;

#if UNITY_EDITOR
namespace QFramework
{
    public class MethodAPIRenderInfo
    {
        private readonly MethodInfo mMethodInfo;
        public string MethodCode { get; private set; }

        public MethodAPIRenderInfo(MethodInfo methodInfo)
        {
            mMethodInfo = methodInfo;
            MethodCode = methodInfo.Name;
        }

        public void BuildString(StringBuilder builder)
        {
            var exampleCodeAttribute = mMethodInfo.GetCustomAttribute<APIExampleCodeAttribute>();

            var description = string.Empty;
            
            if (LocaleKitEditor.IsCN.Value)
            {
                description = mMethodInfo.GetFirstAttribute<APIDescriptionCNAttribute>(false).Description;
            }
            else
            {
                description = mMethodInfo.GetFirstAttribute<APIDescriptionENAttribute>(false).Description;
            }


            builder.AppendLine("|名称 |描述 |")
                .AppendLine("|-|-|")
                .Append("|").Append(mMethodInfo.Name)
                .Append("|").Append(description).AppendLine("|");

            builder
                .Append("* **").Append(mMethodInfo.Name).AppendLine("**")
                .AppendLine("```")
                .AppendLine(exampleCodeAttribute.Code)
                .AppendLine("```");
        }
    }
}
#endif