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

            strBuilder.AppendLine("using QFramework;").AppendLine();

            strBuilder.AppendLine("namespace " + nameSpace);
            strBuilder.AppendLine("{");
            strBuilder.AppendLine();
            strBuilder.AppendFormat("\tpublic partial class {0}", behaviourName);
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t{");
            strBuilder.Append("\t\t").AppendLine("void InitUI()");
            strBuilder.Append("\t\t").AppendLine("{");
            strBuilder.Append("\t\t").Append("\t").AppendLine("//please add init code here");
            strBuilder.Append("\t\t").AppendLine("}").AppendLine();
            strBuilder.Append("\t\t").AppendLine("void RegisterUIEvent()");
            strBuilder.Append("\t\t").AppendLine("{");
            strBuilder.Append("\t\t").AppendLine("}").AppendLine();
            strBuilder.Append("\t\t").AppendLine("void OnClose()");
            strBuilder.Append("\t\t").AppendLine("{");
            strBuilder.Append("\t\t").AppendLine("}").AppendLine();
            strBuilder.Append("\t}").AppendLine();
            strBuilder.Append("}");

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

            strBuilder.AppendLine("using UnityEngine;");
            strBuilder.AppendLine("using UnityEngine.UI;");
            strBuilder.AppendLine("using QFramework;");
            strBuilder.AppendLine();
            strBuilder.AppendLine("namespace " + nameSpace);
            strBuilder.AppendLine("{");
            strBuilder.AppendFormat("\tpublic partial class {0}", behaviourName);
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t{");
            strBuilder.AppendFormat("\t\tpublic const string NAME = \"{0}\";\n", behaviourName);
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t\tpublic Transform transform;");
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t\tpublic GameObject gameObject { get { return transform.gameObject; }}");
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t\tpublic ILRuntimePanel Behaviour; // TODO:这里要改成继承的");
            strBuilder.AppendLine();
            strBuilder.AppendFormat("\t\tpublic {0}(Transform trans)\n", behaviourName);
            strBuilder.AppendLine("\t\t{");
            strBuilder.AppendLine("\t\t\ttransform = trans;");
            strBuilder.AppendLine("\t\t\tBehaviour = trans.GetComponent<ILRuntimePanel>();");
            strBuilder.AppendLine("\t\t\tInternalInitView(transform);");
            strBuilder.AppendLine("\t\t\tInitUI();");
            strBuilder.AppendLine("\t\t\tRegisterUIEvent();");
            strBuilder.AppendLine("\t\t}");
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t\tpublic void CloseSelf()");
            strBuilder.AppendLine("\t\t{");
            strBuilder.AppendLine("\t\t\tUIMgr.ClosePanel(NAME);");
            strBuilder.AppendLine("\t\t}");
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t\tpublic void InternalClose()");
            strBuilder.AppendLine("\t\t{");
            strBuilder.AppendLine("\t\t\tInternalClearView();");
            strBuilder.AppendLine("\t\t\tOnClose();");
            strBuilder.AppendLine("\t\t}");
            strBuilder.AppendLine();

            foreach (var markInfo in panelCodeInfo.BindInfos)
            {
                var strUIType = markInfo.BindScript.TypeName;
                strBuilder.AppendFormat("\t\tpublic {0} {1};\r\n",
                    strUIType, markInfo.TypeName);
            }

            strBuilder.AppendLine();

            strBuilder.Append("\t\t").AppendLine("private void InternalInitView(Transform transform)");
            strBuilder.Append("\t\t").AppendLine("{");
            foreach (var markInfo in panelCodeInfo.BindInfos)
            {
                var strUIType = markInfo.BindScript.TypeName;
                strBuilder.AppendFormat("\t\t\t{0} = transform.Find(\"{1}\").GetComponent<{2}>();\r\n", markInfo.TypeName,
                    markInfo.PathToRoot, strUIType);
            }

            strBuilder.Append("\t\t").AppendLine("}");
            strBuilder.AppendLine();

            strBuilder.Append("\t\t").AppendLine("public void InternalClearView()");
            strBuilder.Append("\t\t").AppendLine("{");
            foreach (var markInfo in panelCodeInfo.BindInfos)
            {
                strBuilder.AppendFormat("\t\t\t{0} = null;\r\n",
                    markInfo.TypeName);
            }

            strBuilder.Append("\t\t").AppendLine("}").AppendLine();
            strBuilder.AppendLine("\t}");
            strBuilder.AppendLine();
            strBuilder.AppendFormat("\tpublic static class {0}Extention\n", behaviourName);
            strBuilder.AppendLine("\t{");
            strBuilder.AppendFormat("\t\tpublic static {0} As{0}(this PTUIBehaviour selfRuntimePanel)\n",
                behaviourName);
            strBuilder.AppendLine("\t\t{");
            strBuilder.AppendFormat("\t\t\treturn selfRuntimePanel.AsiIlRuntimePanel().ILObject as {0};\n",
                behaviourName);
            strBuilder.AppendLine("\t\t}");
            strBuilder.AppendLine("\t}");
            strBuilder.AppendLine("}");

            sw.Write(strBuilder);
            sw.Flush();
            sw.Close();
        }
    }
}