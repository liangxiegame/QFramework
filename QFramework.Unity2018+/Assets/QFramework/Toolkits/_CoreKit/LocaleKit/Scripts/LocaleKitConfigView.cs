/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class LocaleKitConfigView
    {
        public void Init()
        {
        }

        public void OnGUI()
        {
            if (GUILayout.Button(Locale.Config))
            {
                Selection.activeObject = LanguageDefineConfig.Default;
            }
        }

        public void OnWindowGUIEnd()
        {
        }

        public void OnDestroy()
        {
        }


        class Locale
        {
            public static string Config => LocaleKitEditor.IsCN.Value ? "配置" : "Config";
        }
    }
}
#endif