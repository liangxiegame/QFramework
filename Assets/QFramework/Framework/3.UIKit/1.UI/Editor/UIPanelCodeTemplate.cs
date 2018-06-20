/****************************************************************************
 * Copyright (c) 2017 ~ 2018.6 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
	using System;
	using System.IO;
	using System.Text;
	using UnityEngine;

	public static class UIPanelCodeTemplate
	{
		public static void Generate(string generateFilePath, string behaviourName, string nameSpace)
		{
			StreamWriter sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
			StringBuilder strBuilder = new StringBuilder();

			strBuilder.AppendLine("/****************************************************************************");
			strBuilder.AppendFormat(" * {0}.{1} {2}\n", DateTime.Now.Year, DateTime.Now.Month, SystemInfo.deviceName);
			strBuilder.AppendLine(" ****************************************************************************/");

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