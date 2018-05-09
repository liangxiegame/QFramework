/****************************************************************************
 * Copyright (c) 2017 ~ 2018.5 liangxie
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

    public static class UIPanelComponentsCodeTemplate
    {
        public static void Generate(string generateFilePath, string behaviourName, string nameSpace,
            PanelCodeData panelCodeData)
        {
            var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
            var strBuilder = new StringBuilder();

            strBuilder.AppendLine("/****************************************************************************");
            strBuilder.AppendFormat(" * {0}.{1} {2}\n", DateTime.Now.Year, DateTime.Now.Month,SystemInfo.deviceName);
            strBuilder.AppendLine(" ****************************************************************************/");
            strBuilder.AppendLine();
            strBuilder.AppendLine("namespace " + nameSpace);
            strBuilder.AppendLine("{");
            strBuilder.AppendLine("\tusing UnityEngine;");
            strBuilder.AppendLine("\tusing UnityEngine.UI;");
            strBuilder.AppendLine("\tusing QFramework;");
            strBuilder.AppendLine();
            strBuilder.AppendFormat("\tpublic partial class {0}", behaviourName);
            strBuilder.AppendLine("\t{");
            strBuilder.AppendFormat("\t\tpublic const string NAME = \"{0}\";", behaviourName);
            strBuilder.AppendLine();
            
            foreach (var objInfo in panelCodeData.MarkedObjInfos)
            {
                var strUIType = objInfo.MarkObj.ComponentName;
                strBuilder.AppendFormat("\t\t[SerializeField] public {0} {1};\r\n",
                    strUIType, objInfo.Name);
            }

            strBuilder.AppendLine();
            strBuilder.Append("\t\t").AppendLine("protected override void ClearUIComponents()");
            strBuilder.Append("\t\t").AppendLine("{");
            foreach (var markInfo in panelCodeData.MarkedObjInfos)
            {
                strBuilder.AppendFormat("\t\t\t{0} = null;\r\n",
                    markInfo.Name);
            }

            strBuilder.Append("\t\t").AppendLine("}");
            strBuilder.AppendLine("\t}");
            strBuilder.AppendLine("}");
            sw.Write(strBuilder);
            sw.Flush();
            sw.Close();
        }
    }
}