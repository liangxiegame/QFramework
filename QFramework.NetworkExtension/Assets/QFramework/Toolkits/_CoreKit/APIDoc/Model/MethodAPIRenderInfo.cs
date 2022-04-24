/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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
                description = mMethodInfo.GetAttribute<APIDescriptionCNAttribute>().Description;
            }
            else
            {
                description = mMethodInfo.GetAttribute<APIDescriptionENAttribute>(false).Description;
            }

            builder
                .Append("|").Append(mMethodInfo.Name).Append("|").Append(description).AppendLine("|")
                .AppendLine("|-|-|")
                .Append("|").Append(APIDocLocale.ExampleCode).Append("|").AppendLine(" |")
                .AppendLine("```")
                .AppendLine(exampleCodeAttribute.Code)
                .AppendLine("```")
                .AppendLine("--------");
        }
    }
}
#endif