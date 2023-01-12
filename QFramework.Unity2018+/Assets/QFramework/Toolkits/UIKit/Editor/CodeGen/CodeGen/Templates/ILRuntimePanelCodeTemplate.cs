/****************************************************************************
 * Copyright (c) 2017 ~ 2018.8 liangxie
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
    using System.Text;
    using System.IO;
    
    public class ILRuntimePanelCodeTemplate
    {
        public static void Generate(string generateFilePath, string behaviourName,string nameSpace)
        {
            var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
            var strBuilder = new StringBuilder();

            var tabChar = string.IsNullOrWhiteSpace(nameSpace) ? "" : "\t";

            strBuilder.AppendLine("using QFramework;").AppendLine();

            if (tabChar != "")
            {
                strBuilder.AppendLine("namespace " + nameSpace);
                strBuilder.AppendLine("{");
                strBuilder.AppendLine();
            }
            
            strBuilder.AppendFormat(tabChar + "public partial class {0}", behaviourName);
            strBuilder.AppendLine();
            strBuilder.AppendLine(tabChar + "{");
            strBuilder.Append(tabChar + "\t").AppendLine("void InitUI()");
            strBuilder.Append(tabChar + "\t").AppendLine("{");
            strBuilder.Append(tabChar + "\t").Append("\t").AppendLine("//please add init code here");
            strBuilder.Append(tabChar + "\t").AppendLine("}").AppendLine();
            strBuilder.Append(tabChar + "\t").AppendLine("void RegisterUIEvent()");
            strBuilder.Append(tabChar + "\t").AppendLine("{");
            strBuilder.Append(tabChar + "\t").AppendLine("}").AppendLine();
            strBuilder.Append(tabChar + "\t").AppendLine("void OnClose()");
            strBuilder.Append(tabChar + "\t").AppendLine("{");
            strBuilder.Append(tabChar + "\t").AppendLine("}").AppendLine();
            strBuilder.Append(tabChar + "}").AppendLine();

            if (tabChar != "")
            {
                strBuilder.Append("}");
            }

            sw.Write(strBuilder);
            sw.Flush();
            sw.Close();
        }
    }

    public class ILRuntimePanelComponentsCodeGenerator
    {
        public static void Generate(string generateFilePath, string behaviourName, string nameSpace,
            PanelCodeInfo panelCodeInfo)
        {
            var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
            var strBuilder = new StringBuilder();

            var tabChar = string.IsNullOrWhiteSpace(nameSpace) ? "" : "\t";

            strBuilder.AppendLine("using UnityEngine;");
            strBuilder.AppendLine("using UnityEngine.UI;");
            strBuilder.AppendLine("using QFramework;");
            strBuilder.AppendLine();

            if (tabChar != "")
            {
                strBuilder.AppendLine("namespace " + nameSpace);
                strBuilder.AppendLine("{");
                strBuilder.AppendLine();
            }

            strBuilder.AppendFormat(tabChar + "public partial class {0}", behaviourName);
            strBuilder.AppendLine();
            strBuilder.AppendLine(tabChar + "{");
            strBuilder.AppendFormat(tabChar + "\tpublic const string NAME = \"{0}\";\n", behaviourName);
            strBuilder.AppendLine();
            strBuilder.AppendLine(tabChar + "\tpublic Transform transform;");
            strBuilder.AppendLine();
            strBuilder.AppendLine(tabChar + "\tpublic GameObject gameObject { get { return transform.gameObject; }}");
            strBuilder.AppendLine();
            strBuilder.AppendLine(tabChar + "\tpublic ILRuntimePanel Behaviour; // TODO:这里要改成继承的");
            strBuilder.AppendLine();
            strBuilder.AppendFormat(tabChar + "\tpublic {0}(Transform trans)\n", behaviourName);
            strBuilder.AppendLine(tabChar + "\t{");
            strBuilder.AppendLine(tabChar + "\t\ttransform = trans;");
            strBuilder.AppendLine(tabChar + "\t\tBehaviour = trans.GetComponent<ILRuntimePanel>();");
            strBuilder.AppendLine(tabChar + "\t\tInternalInitView(transform);");
            strBuilder.AppendLine(tabChar + "\t\tInitUI();");
            strBuilder.AppendLine(tabChar + "\t\tRegisterUIEvent();");
            strBuilder.AppendLine(tabChar + "\t}");
            strBuilder.AppendLine();
            strBuilder.AppendLine(tabChar + "\tpublic void CloseSelf()");
            strBuilder.AppendLine(tabChar + "\t{");
            strBuilder.AppendLine(tabChar + "\t\tUIMgr.ClosePanel(NAME);");
            strBuilder.AppendLine(tabChar + "\t}");
            strBuilder.AppendLine();
            strBuilder.AppendLine(tabChar + "\tpublic void InternalClose()");
            strBuilder.AppendLine(tabChar + "\t{");
            strBuilder.AppendLine(tabChar + "\t\tInternalClearView();");
            strBuilder.AppendLine(tabChar + "\t\tOnClose();");
            strBuilder.AppendLine(tabChar + "\t}");
            strBuilder.AppendLine();

            foreach (var markInfo in panelCodeInfo.BindInfos)
            {
                var strUIType = markInfo.BindScript.TypeName;
                strBuilder.AppendFormat(tabChar + "\tpublic {0} {1};\r\n",
                    strUIType, markInfo.TypeName);
            }

            strBuilder.AppendLine();

            strBuilder.Append(tabChar + "\t").AppendLine("private void InternalInitView(Transform transform)");
            strBuilder.Append(tabChar + "\t").AppendLine("{");
            foreach (var markInfo in panelCodeInfo.BindInfos)
            {
                var strUIType = markInfo.BindScript.TypeName;
                strBuilder.AppendFormat(tabChar + "\t\t{0} = transform.Find(\"{1}\").GetComponent<{2}>();\r\n", markInfo.TypeName,
                    markInfo.PathToRoot, strUIType);
            }

            strBuilder.Append(tabChar + "\t").AppendLine("}");
            strBuilder.AppendLine();

            strBuilder.Append(tabChar + "\t").AppendLine("public void InternalClearView()");
            strBuilder.Append(tabChar + "\t").AppendLine("{");
            foreach (var markInfo in panelCodeInfo.BindInfos)
            {
                strBuilder.AppendFormat(tabChar + "\t\t{0} = null;\r\n",
                    markInfo.TypeName);
            }

            strBuilder.Append(tabChar + "\t").AppendLine("}").AppendLine();
            strBuilder.AppendLine(tabChar + "}");
            strBuilder.AppendLine();
            strBuilder.AppendFormat(tabChar + "public static class {0}Extention\n", behaviourName);
            strBuilder.AppendLine(tabChar + "{");
            strBuilder.AppendFormat(tabChar + "\tpublic static {0} As{0}(this PTUIBehaviour selfRuntimePanel)\n",
                behaviourName);
            strBuilder.AppendLine(tabChar + "\t{");
            strBuilder.AppendFormat(tabChar + "\t\treturn selfRuntimePanel.AsiIlRuntimePanel().ILObject as {0};\n",
                behaviourName);
            strBuilder.AppendLine(tabChar + "\t}");
            strBuilder.AppendLine(tabChar + "}");

            if (tabChar != "")
            {
                strBuilder.AppendLine("}");
            }

            sw.Write(strBuilder);
            sw.Flush();
            sw.Close();
        }
    }
}