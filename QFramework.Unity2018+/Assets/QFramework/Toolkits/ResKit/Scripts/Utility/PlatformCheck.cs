/****************************************************************************
 * Copyright (c) 2021 ~ 2022 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    public static class PlatformCheck
    {
        public static bool IsAndroid
        {
            get
            {
                bool retValue = false;
#if UNITY_ANDROID
                retValue = true;
#endif
                return retValue;
            }
        }

        public static bool IsEditor
        {
            get
            {
                bool retValue = false;
#if UNITY_EDITOR
                retValue = true;
#endif
                return retValue;
            }
        }

        public static bool IsiOS
        {
            get
            {
                bool retValue = false;
#if UNITY_IOS
				retValue = true;
#endif
                return retValue;
            }
        }

        public static bool IsStandardAlone
        {
            get
            {
                bool retValue = false;
#if UNITY_STANDALONE
                retValue = true;
#endif
                return retValue;
            }
        }

        public static bool IsWin
        {
            get
            {
                bool retValue = false;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                retValue = true;
#endif
                return retValue;
            }
        }

        public static bool IsOSX
        {
            get
            {
                bool retValue = false;
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                retValue = true;
#endif
                return retValue;
            }
        }

        public static bool IsWebGL => Application.platform == RuntimePlatform.WebGLPlayer;

        public static bool IsWeixinMiniGame
        {
            get
            {
#if TUANJIE_WEIXINMINIGAME
                return Application.platform == RuntimePlatform.WeixinMiniGamePlayer;
#endif
                return false;
            }
        }
    }
}