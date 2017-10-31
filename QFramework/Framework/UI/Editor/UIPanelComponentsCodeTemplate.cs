/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
    using System.Text;
    using System.IO;
    using System.Collections.Generic;
    
    public class UIPanelComponentsCodeTemplate
    {
        public static void Generate(string generateFilePath,string behaviourName,string nameSpace, PanelCodeData panelCodeData)
        {
            StreamWriter sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine("using UnityEngine;");
            strBuilder.AppendLine("using UnityEngine.UI;");
            strBuilder.AppendLine("using QFramework;");
            strBuilder.AppendLine ();
            strBuilder.AppendLine ("namespace " + nameSpace);
            strBuilder.AppendLine ("{");
            strBuilder.AppendFormat("\tpublic class {0}Components : MonoBehaviour, IUIComponents", behaviourName);
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t{");
			
            foreach (KeyValuePair<string, IUIMark> p in panelCodeData.DicNameToIUIData)
            {
                string strUIType = p.Value.ComponentName;
                strBuilder.AppendFormat("\t\t[SerializeField] public {0} {1};\r\n",
                    strUIType,p.Key);
            }

            strBuilder.AppendLine();
			
            strBuilder.Append("\t\t").AppendLine("public void Clear()");
            strBuilder.Append("\t\t").AppendLine("{");
            foreach (KeyValuePair<string, IUIMark> p in panelCodeData.DicNameToIUIData)
            {
                strBuilder.AppendFormat("\t\t\t{0} = null;\r\n",
                    p.Key);
            }
            strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			

            strBuilder.AppendLine("\t}");
            strBuilder.AppendLine("}");

            sw.Write(strBuilder);
            sw.Flush();
            sw.Close();
        }
    }
}