/****************************************************************************
 * Copyright (c) 2018 ~ 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;

using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace QFramework
{
    public class LiveCodingKit 
    {
        public static LiveCodingKitSetting Setting => LiveCodingKitSetting.Load();
        
        
        
        [DidReloadScripts]
        public static async void Reload()
        {
            // if live coding opened
            if (EditorApplication.isPlaying && Setting.Open)
            {

                if (Setting.WhenCompileFinish == LiveCodingKitSetting.ReloadMethod.RestartGame)
                {
                    EditorApplication.isPlaying = false;

                    // wait for close unity
                    while (Time.frameCount > 1)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.1f));

                        if (Time.frameCount <= 1)
                        {
                            EditorApplication.isPlaying = true;
                            break;
                        }
                    }
                } else if (Setting.WhenCompileFinish == LiveCodingKitSetting.ReloadMethod.ReloadCurrentScene)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
    }
}

#endif