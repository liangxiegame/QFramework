/****************************************************************************
 * Copyright (c) 2016 ~ 2023 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;

namespace QFramework
{
    using UnityEngine;

    public class ResKitEditorWindow : EditorWindow
    {
        private static EncryptConfig mConfigInstance = null;

        static EncryptConfig GetConfig()
        {
            if (mConfigInstance == null)
            {
                TextAsset text = Resources.Load<TextAsset>("EncryptConfig");
                if (text)
                {
                    mConfigInstance = JsonUtility.FromJson<EncryptConfig>(text.text);

                    if (mConfigInstance == null)
                    {
                        mConfigInstance = new EncryptConfig();
                    }
                }
                else
                {
                    mConfigInstance = new EncryptConfig();

                    string savePath = Application.dataPath + "QFrameworkData/Resources/EncryptConfig.Json";
                    using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                        {
                            sw.WriteLine(JsonUtility.ToJson(mConfigInstance));
                        }
                    }
                }
            }

            return mConfigInstance;
        }

        [MenuItem("QFramework/Toolkits/Res Kit %#r")]
        public static void OpenWindow()
        {
            var window = (ResKitEditorWindow)GetWindow(typeof(ResKitEditorWindow), true);
            Debug.Log(Screen.width + " screen width*****");
            window.position = new Rect(100, 100, 600, 400);
            window.Show();
        }


        private void OnEnable()
        {
            mResKitView = new ResKitView();
            mResKitView.EditorWindow = this;
            mResKitView.Init();
        }

        ResKitView mResKitView = null;

        public static bool EnableGenerateClass
        {
            get { return EditorPrefs.GetBool(ResKitView.KEY_AUTOGENERATE_CLASS, false); }
        }

        public void OnDisable()
        {
            //string savePath = Application.dataPath + "/QFrameworkData/Resources/EncryptConfig.Json";
            //using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
            //{
            //    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            //    {
            //        sw.WriteLine(JsonUtility.ToJson(GetConfig()));
            //    }
            //}
            //AssetDatabase.Refresh();

            mResKitView.OnDispose();
            mResKitView = null;
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();


            mResKitView.OnGUI();


            GUILayout.EndVertical();
            GUILayout.Space(50);


            // RenderEndCommandExecuter.ExecuteCommand();
        }
    }


  


    internal class EditorLifecycle
    {
        static EditorLifecycle()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            ExecuteCommand();
        }

        private static Queue<System.Action> mPrivateCommands = new Queue<System.Action>();

        private static Queue<System.Action> mCommands
        {
            get { return mPrivateCommands; }
        }

        public static void PushCommand(System.Action command)
        {
            mCommands.Enqueue(command);
        }

        public static void ExecuteCommand()
        {
            while (mCommands.Count > 0)
            {
                mCommands.Dequeue().Invoke();
            }
        }
    }
}