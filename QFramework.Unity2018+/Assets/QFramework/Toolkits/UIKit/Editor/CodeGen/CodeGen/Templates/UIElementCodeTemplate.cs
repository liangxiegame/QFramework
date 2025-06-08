/****************************************************************************
* Copyright (c) 2016 ~ 2025 liangxiegame UNDER MIT LINCENSE
* 
* https://qframework.cn
* https://github.com/liangxiegame/QFramework
* https://gitee.com/liangxiegame/QFramework
****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
	using System.Text;
	using System.IO;

	public static class UIElementCodeTemplate
	{
		public static void Generate(string generateFilePath, string behaviourName, string nameSpace,
			ElementCodeInfo elementCodeInfo)
		{
			var sw = new StreamWriter(generateFilePath, false, new UTF8Encoding(false));
			var strBuilder = new StringBuilder();

			var markType = elementCodeInfo.BindInfo.BindScript.As<IBindOld>().GetBindType();

			strBuilder.AppendLine("/****************************************************************************");
			strBuilder.AppendFormat(" * {0}.{1} {2}\n", DateTime.Now.Year, DateTime.Now.Month, SystemInfo.deviceName);
			strBuilder.AppendLine(" ****************************************************************************/");
			strBuilder.AppendLine();

			strBuilder.AppendLine("using System;");
			strBuilder.AppendLine("using System.Collections.Generic;");
			strBuilder.AppendLine("using UnityEngine;");
			strBuilder.AppendLine("using UnityEngine.UI;");
			strBuilder.AppendLine("using QFramework;").AppendLine();

			strBuilder.AppendLine("namespace " + nameSpace);
			strBuilder.AppendLine("{");
			strBuilder.AppendFormat("\tpublic partial class {0} : {1}", behaviourName,
				markType == BindType.Component ? "UIComponent" : "UIElement");
			strBuilder.AppendLine();
			strBuilder.AppendLine("\t{");
			strBuilder.Append("\t\t").AppendLine("private void Awake()");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t").AppendLine("}");
			strBuilder.AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void OnBeforeDestroy()");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t").AppendLine("}");
			strBuilder.AppendLine("\t}");
			strBuilder.Append("}");

			sw.Write(strBuilder);
			sw.Flush();
			sw.Close();
		}
	}

	public static class UIElementCodeComponentTemplate
	{
		public static void Generate(string generateFilePath, string behaviourName, string nameSpace,
			ElementCodeInfo elementCodeInfo)
		{
			var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
			var strBuilder = new StringBuilder();

			strBuilder.AppendLine("/****************************************************************************");
			strBuilder.AppendFormat(" * {0}.{1} {2}\n", DateTime.Now.Year, DateTime.Now.Month, SystemInfo.deviceName);
			strBuilder.AppendLine(" ****************************************************************************/");
			strBuilder.AppendLine();
			strBuilder.AppendLine("using UnityEngine;");
			strBuilder.AppendLine("using UnityEngine.UI;");
			strBuilder.AppendLine("using QFramework;");
			strBuilder.AppendLine();
			strBuilder.AppendLine("namespace " + nameSpace);
			strBuilder.AppendLine("{");
			strBuilder.AppendFormat("\tpublic partial class {0}", behaviourName);
			strBuilder.AppendLine();
			strBuilder.AppendLine("\t{");

			foreach (var markInfo in elementCodeInfo.BindInfos)
			{
				var strUIType = markInfo.BindScript.TypeName;
				strBuilder.AppendFormat("\t\t[SerializeField] public {0} {1};\r\n",
					strUIType, markInfo.TypeName);
			}

			strBuilder.AppendLine();

			strBuilder.Append("\t\t").AppendLine("public void Clear()");
			strBuilder.Append("\t\t").AppendLine("{");
			foreach (var markInfo in elementCodeInfo.BindInfos)
			{
				strBuilder.AppendFormat("\t\t\t{0} = null;\r\n",
					markInfo.TypeName);
			}

			strBuilder.Append("\t\t").AppendLine("}");
			strBuilder.AppendLine();

			strBuilder.Append("\t\t").AppendLine("public override string ComponentName");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t");
			strBuilder.AppendLine("get { return \"" + elementCodeInfo.BindInfo.BindScript.TypeName + "\";}");
			strBuilder.Append("\t\t").AppendLine("}");
			strBuilder.AppendLine("\t}");
			strBuilder.AppendLine("}");
			sw.Write(strBuilder);
			sw.Flush();
			sw.Close();
		}
	}
}