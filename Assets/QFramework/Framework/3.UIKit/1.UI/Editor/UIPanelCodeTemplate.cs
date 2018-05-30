/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using System.Text;
	using System.IO;

	public static class UIPanelCodeTemplate
	{
		public static void Generate(string generateFilePath, string behaviourName, string nameSpace)
		{
			StreamWriter sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
			StringBuilder strBuilder = new StringBuilder();

			strBuilder.AppendLine("using System;");
			strBuilder.AppendLine("using System.Collections.Generic;");
			strBuilder.AppendLine("using UnityEngine;");
			strBuilder.AppendLine("using UnityEngine.UI;");
			strBuilder.AppendLine("using QFramework;").AppendLine();

			strBuilder.AppendLine("namespace " + nameSpace);
			strBuilder.AppendLine("{");
			strBuilder.Append("\t").AppendFormat("public class {0}Data : UIPanelData", behaviourName).AppendLine();
			strBuilder.Append("\t").AppendLine("{");
			strBuilder.Append("\t\t").AppendLine("// TODO: Query Mgr's Data");
			strBuilder.Append("\t").AppendLine("}");
			strBuilder.AppendLine();
			strBuilder.AppendFormat("\tpublic partial class {0} : UIPanel", behaviourName);
			strBuilder.AppendLine();
			strBuilder.AppendLine("\t{");
			strBuilder.Append("\t\t").AppendLine("protected override void InitUI(IUIData uiData = null)");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t").Append("\t")
				.AppendLine("mData = uiData as " + behaviourName + "Data ?? new " + behaviourName + "Data();");
			strBuilder.Append("\t\t").Append("\t").AppendLine("//please add init code here");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void ProcessMsg (int eventId,QMsg msg)");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t").AppendLine("throw new System.NotImplementedException ();");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void RegisterUIEvent()");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void OnShow()");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t").AppendLine("base.OnShow();");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void OnHide()");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t").AppendLine("base.OnHide();");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void OnClose()");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t").AppendLine("base.OnClose();");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("void ShowLog(string content)");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t").AppendFormat("Debug.Log(\"[ {0}:]\" + content);", behaviourName).AppendLine();
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendFormat("{0}Data mData = null;", behaviourName).AppendLine();
			strBuilder.Append("\t}").AppendLine();
			strBuilder.Append("}");

			sw.Write(strBuilder);
			sw.Flush();
			sw.Close();
		}
	}
}