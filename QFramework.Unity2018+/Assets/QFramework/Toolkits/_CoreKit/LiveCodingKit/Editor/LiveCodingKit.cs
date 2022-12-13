/****************************************************************************
 * Copyright (c) 2018 ~ 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/


using System;
#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
#endif

namespace QFramework
{
    public class LiveCodingKit 
    {
        public static LiveCodingKitSetting Setting => LiveCodingKitSetting.Load();
        [DidReloadScripts]
        public static async void Reload()
        {
            #if UNITY_EDITOR
            // if live coding opened
            if (EditorApplication.isPlaying && Setting.Open)
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
 
            }
            #endif
        }
    }
}
