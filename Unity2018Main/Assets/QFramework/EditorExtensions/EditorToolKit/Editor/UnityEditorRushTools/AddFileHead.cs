using System;
using System.IO;
using System.Threading;
using QF;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 添加文件头说明
    /// </summary>
    public class AddFileHead 
    {
        #region 更新格式
        [MenuItem("QFramework/Tool/File/2.更新文件头格式")]
        public static void UpdateFileHead()
        {
            string NewBehaviourScriptPath =  EditorApplication.applicationContentsPath + "/Resources/ScriptTemplates/81-C# Script-NewBehaviourScript.cs.txt";

            string head = "" +
                    //自定义部分
                    "/*******************************************************************\n"
                    + "* Copyright(c) #YEAR# #COMPANY#\n"
                    + "* All rights reserved.\n"
                    + "*\n"
                    + "* 文件名称: #SCRIPTFULLNAME#\n"
                    + "* 简要描述:\n"
                    + "* \n"
                    + "* 创建日期: #DATE#\n"
                    + "* 作者:     #AUTHOR#\n"
                    + "* 说明:  \n"
                    + "******************************************************************/\n"
                    //以下部分unity默认文件
                    + "using System.Collections;\n"
                    + "using System.Collections.Generic;\n"
                    + "using UnityEngine;\n"
                    + "\n"
                    + "public class #SCRIPTNAME# : MonoBehaviour\n"
                    + "{\n"
                    + "\t// Use this for initialization\n"
                    + "\tvoid Start()\n"
                    + "\t{\n"
                    + "\t\t#NOTRIM#\n"
                    + "\t}\n"
                    + "\n"
                    + "\t// Update is called once per frame\n"
                    + "\tvoid Update()\n"
                    + "\t{\n"
                    + "\t\t#NOTRIM#\n"
                    + "\t}\n"
                    + "}";
            
            byte[] curTexts = System.Text.Encoding.UTF8.GetBytes(head);
            using (FileStream fs = new FileStream(NewBehaviourScriptPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                if (fs != null)
                {
                    fs.SetLength(0);    //清空文件
                    fs.Write(curTexts, 0, curTexts.Length);
                    fs.Flush();
                    fs.Dispose();

                    Log.I("Update File: 81-C# Script-NewBehaviourScript.cs.txt, Success");
                }
            }
        }
        #endregion

        #region 打开文件
        const string NotePadJJ_APP_NAME = "notepad++.exe";
        const string NotePad_APP_NAME = "notepad.exe";

        /// <summary>
        /// 用notepad++打开文件
        /// </summary>
        [MenuItem("QFramework/Tool/File/1.编辑文件头格式 NotePad++")]
        static public void OpenForNotePadJJ()
        {
            string dir_path = Application.dataPath + "/QFramework/Framework/4.EditorToolKit/Editor/UnityEditorRushTools/AddFileHead.cs";
            InvokeCmd(NotePadJJ_APP_NAME, dir_path);
        }

        /// <summary>
        /// 用记事本打开文件
        /// </summary>
        [MenuItem("QFramework/Tool/File/1.编辑文件头格式 记事本")]
        static public void OpenForNotePad()
        {
            string dir_path = Application.dataPath + "/QFramework/Framework/4.EditorToolKit/Editor/UnityEditorRushTools/AddFileHead.cs";
            InvokeCmd(NotePad_APP_NAME, dir_path);
        }

        /// <summary>
        /// 调用CMD 命令
        /// </summary>
        public static void InvokeCmd(string cmd, string dir_path)
        {
            UnityEngine.Debug.Log(cmd);
            AssetDatabase.Refresh();
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = cmd;
                    p.StartInfo.Arguments = dir_path;
                    p.Start();
                    p.WaitForExit();
                    p.Close();
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            })).Start();
        }
        
        #endregion
    }

    //解析
    public class ParseFileHead : UnityEditor.AssetModificationProcessor
    {
        /// <summary>  
        /// 此函数在asset被创建完，文件已经生成到磁盘上，但是没有生成.meta文件和import之前被调用  
        /// </summary>  
        /// <param name="newFileMeta">newfilemeta 是由创建文件的path加上.meta组成的</param>  
        public static void OnWillCreateAsset(string newFileMeta)
        {
            string newFilePath = newFileMeta.Replace(".meta", "");
            string fileExt = Path.GetExtension(newFilePath);
            if (fileExt != ".cs")
            {
                return;
            }
            //注意，Application.datapath会根据使用平台不同而不同  
            string realPath = Application.dataPath.Replace("Assets", "") + newFilePath;
            string scriptContent = File.ReadAllText(realPath);

            //自定义修改系统信息
            //PlayerSettings.companyName = "";

            //这里现自定义的一些规则  
            scriptContent = scriptContent.Replace("#SCRIPTFULLNAME#", Path.GetFileName(newFilePath));
            scriptContent = scriptContent.Replace("#COMPANY#", PlayerSettings.companyName);
            scriptContent = scriptContent.Replace("#AUTHOR#", "SilenceT");
            scriptContent = scriptContent.Replace("#VERSION#", "1.0");
            scriptContent = scriptContent.Replace("#UNITYVERSION#", Application.unityVersion);
            scriptContent = scriptContent.Replace("#DATE#", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            scriptContent = scriptContent.Replace("#YEAR#", System.DateTime.Now.ToString("yyyy"));

            File.WriteAllText(realPath, scriptContent);
        }
    }
    
}
