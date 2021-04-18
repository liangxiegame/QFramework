/****************************************************************************
 * Copyright (c) 2021.4 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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

using UnityEngine;

namespace QFramework
{
    public class PlatformFromUnityToDll : IPlatformFromUnity
    {
        public bool IsAndroid
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

        public bool IsEditor
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

        public bool IsiOS
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

        public bool IsStandardAlone
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

        public bool IsWin
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

        public bool IsWebGL
        {
            get { return Application.platform == RuntimePlatform.WebGLPlayer; }
        }
    }
}